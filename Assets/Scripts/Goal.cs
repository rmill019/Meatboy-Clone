using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

	public AudioClip			goalClip;

	void OnTriggerEnter2D (Collider2D coll)
	{
		if (coll.gameObject.tag == "Player")
		{
			coll.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			coll.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
			AudioSource audioSource = GetComponent<AudioSource>();
			if (audioSource != null && goalClip != null)
			{
				audioSource.PlayOneShot (goalClip);
			}
			GameManager.instance.RestartLevel (0.5f);
		}
	}
}
