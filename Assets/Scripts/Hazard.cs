using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour {

	public GameObject				playerDeathPrefab;
	public AudioClip				deathClip;
	public Sprite 					hitSprite;
	private SpriteRenderer			spriteRenderer;

	// Use this for initialization
	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter2D (Collision2D coll)
	{
		if (coll.transform.tag == "Player")
		{
			AudioSource audioSource = GetComponent<AudioSource>();
			if (audioSource != null && deathClip != null)
			{
				audioSource.PlayOneShot (deathClip);
			}

			Instantiate (playerDeathPrefab, coll.contacts[0].point, Quaternion.identity);
			spriteRenderer.sprite = hitSprite;

			Destroy (coll.gameObject);

			// Reload the scene once the player dies
			GameManager.instance.RestartLevel (1.25f);
		}
	}
}
