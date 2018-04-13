using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(Animator))]
public class SoyBoyController : MonoBehaviour {
	
	public float				speed = 14f;
	public float				accel = 6f;
	private Vector2				input;
	private SpriteRenderer		sr;
	private Rigidbody2D			rb;
	private Animator			animator;

	void Awake () {
		sr = GetComponent<SpriteRenderer>();
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		input.x = Input.GetAxis ("Horizontal");
		input.y = Input.GetAxis ("Jump");

		if (input.x > 0f)
			sr.flipX = false;
		else if (input.x < 0f)
			sr.flipX = true;
	}

	void FixedUpdate () {
		float acceleration = accel;
		float xVelocity = 0f;

		if (input.x == 0)
			xVelocity = 0f;
		else
			xVelocity = rb.velocity.x;

		rb.AddForce (new Vector2(((input.x * speed) - rb.velocity.x) * acceleration, 0));

		rb.velocity = new Vector2 (xVelocity, rb.velocity.y);
	}
}
