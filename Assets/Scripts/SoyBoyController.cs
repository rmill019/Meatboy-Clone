using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(Animator))]
public class SoyBoyController : MonoBehaviour {
	
	// Public variables
	public AudioClip			runClip;
	public AudioClip			jumpClip;
	public AudioClip			slideClip;
	public float				speed = 14f;
	public float				accel = 6f;
	public float				airAccel = 3f;
	public float				jump = 14f;
	public bool					isJumping;
	public float				jumpSpeed = 8f;
	public float				jumpDurationThreshold = 0.25f;

	// Private variables
	private AudioSource			audioSource;
	private float				jumpDuration;
	private float 				rayCastLengthCheck = 0.005f;
	private float				width;
	private float				height;
	private Vector2				input;
	private SpriteRenderer		sr;
	private Rigidbody2D			rb;
	private Animator			animator;

	void Awake () {
		sr = GetComponent<SpriteRenderer>();
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		audioSource = GetComponent<AudioSource>();

		width = GetComponent<Collider2D>().bounds.extents.x + 0.1f;
		height = GetComponent<Collider2D>().bounds.extents.y + 0.2f;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		input.x = Input.GetAxis ("Horizontal");
		input.y = Input.GetAxis ("Jump");
		animator.SetFloat ("Speed", Mathf.Abs (input.x));

		if (input.x > 0f)
			sr.flipX = false;
		else if (input.x < 0f)
			sr.flipX = true;

		// Incrementing jumpDuration if we are pressing jump
		if (input.y >= 1f)
		{
			jumpDuration += Time.deltaTime;
			animator.SetBool ("IsJumping", true);
		}
		else
		{
			isJumping = false;
			animator.SetBool ("IsJumping", false);
			jumpDuration = 0f;
		}

		// Jumping Check and Execution
		if (PlayerIsOnGround() && !isJumping)
		{
			if (input.y > 0f)
			{
				isJumping = true;
				PlayAudioClip (jumpClip);
			}
			animator.SetBool ("IsOnWall", false);

			// If we are running left or right play the running clip
			if (input.x < 0f || input.x > 0f)
			{
				PlayAudioClip (runClip);
			}
		}

		if (jumpDuration > jumpDurationThreshold) input.y = 0f;
			
	}

	void FixedUpdate () {
		float acceleration = 0f;
		float xVelocity = 0f;

		// If we are on the ground we have normal acceleration, else we have air acceleration (half of normal acceleration)
		if (PlayerIsOnGround()){
			acceleration = accel;
			//Debug.LogError ("Acceleration: " + accel);
		}
		else{
			acceleration = airAccel;
			//Debug.LogError ("Acceleration: " + airAccel);
		}

		// Set the players x-velocity to 0 if they are on the ground and not pressing left or right
		if (PlayerIsOnGround() && input.x == 0)
			xVelocity = 0f;
		else
			xVelocity = rb.velocity.x;

		// Setting our yVelocity
		float yVelocity = 0f;
		if (PlayerIsTouchingGroundOrWall() && input.y == 1)
			yVelocity = jump;
		else
			yVelocity = rb.velocity.y;
			
		rb.AddForce (new Vector2(((input.x * speed)- rb.velocity.x) * acceleration, 0));
		rb.velocity = new Vector2 (xVelocity, yVelocity);

		// Have our jump direction be based on whether there is a wall to our left or right
		if (IsWallToLeftOrRight() && !PlayerIsOnGround() && input.y == 1)
		{
			// 0.75 is used to dampen the strength of the jump
			// We are jumping opposite of the number returned by GetWallDirection.
			// Wall on left = -1 and Wall on right = 1
			Vector2 jumpDirection = new Vector2 (-GetWallDirection() * speed * 0.75f, rb.velocity.y);
			rb.velocity = jumpDirection;
			animator.SetBool ("IsOnWall", false);
			animator.SetBool ("IsJumping", true);
			PlayAudioClip (jumpClip);
		}
		else if (!IsWallToLeftOrRight())
		{	
			animator.SetBool ("IsOnWall", false);
			animator.SetBool ("IsJumping", true);
		}

		if (IsWallToLeftOrRight() && !PlayerIsOnGround())
		{
			animator.SetBool ("IsOnWall", true);
			PlayAudioClip (slideClip);
		}

		// Handling the jumping physics here because we are affecting a rigidbody2D to simulate jumping
		if (isJumping && jumpDuration < jumpDurationThreshold)
		{
			rb.velocity = new Vector2 (rb.velocity.x, jumpSpeed);
		}
	}




	/*********************
	*  Custom Functions  * 
	*********************/
	public bool PlayerIsOnGround()
	{
		// First Raycast Ground check
		Vector2 firstRaycastOrigin = new Vector2 (transform.position.x, transform.position.y - height);
		bool groundCheck1 = Physics2D.Raycast (firstRaycastOrigin, -Vector2.up, rayCastLengthCheck);

		// Second Raycast Ground check
		Vector2 secondRaycastOrigin = new Vector2 (transform.position.x + (width - 0.2f), transform.position.y - height);
		bool groundCheck2 = Physics2D.Raycast (secondRaycastOrigin, -Vector2.up, rayCastLengthCheck);
		
		// Third Raycast Ground check
		Vector2 thirdRaycastOrigin = new Vector2 (transform.position.x - (width - 0.2f), transform.position.y - height);
		bool groundCheck3 = Physics2D.Raycast (thirdRaycastOrigin, -Vector2.up, rayCastLengthCheck);
		
		// If any three of our groundChecks are true then we are on the ground, return true
		if (groundCheck1 || groundCheck2 || groundCheck3)
			return true;
		// else we are not on the ground
		else
			return false;
	}

	public bool IsWallToLeftOrRight()
	{
		Vector2 leftSideRaycast = new Vector2 (transform.position.x - width, transform.position.y);
		bool wallOnLeft = Physics2D.Raycast (leftSideRaycast, Vector2.left, rayCastLengthCheck);

		Vector2 rightSideRaycast = new Vector2 (transform.position.x + width, transform.position.y);
		bool wallOnRight = Physics2D.Raycast (rightSideRaycast, Vector2.right, rayCastLengthCheck);

		if (wallOnLeft || wallOnRight)
			return true;
		else
			return false;
	}

	// This will return true if we are on the ground or have a wall to our left or right
	public bool PlayerIsTouchingGroundOrWall ()
	{
		if (PlayerIsOnGround() || IsWallToLeftOrRight())
			return true;
		else
			return false;
	}

	public int GetWallDirection ()
	{
		Vector2 wallLeftRayCast = new Vector2 (transform.position.x - width, transform.position.y);
		bool isWallLeft = Physics2D.Raycast (wallLeftRayCast, Vector2.left, rayCastLengthCheck);

		Vector2 wallRightRayCast = new Vector2 (transform.position.x + width, transform.position.y);
		bool isWallRight = Physics2D.Raycast (wallRightRayCast, Vector3.right, rayCastLengthCheck);

		if (isWallLeft)
			return -1;
		else if (isWallRight)
			return 1;
		else
			return 0;
	}

	void PlayAudioClip (AudioClip clip)
	{
		if (audioSource != null && clip != null)
		{
			if (!audioSource.isPlaying)
				audioSource.PlayOneShot (clip);
		}
	}
}
