using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLerpToTransform : MonoBehaviour {

	public Transform			camTarget;
	public float				trackingSpeed;
	public float				minX;
	public float				minY;
	public float 				maxX;
	public float				maxY;

	// We will be using FixedUpdate to track the player because our player will move based
	// on Physics. Therefore we want the camera to update in sync with the Physics Engine thus using FixedUpdate and not Update()
	void FixedUpdate () {

		if (camTarget != null)
		{
			Vector2 newPos = Vector2.Lerp (transform.position, camTarget.position, Time.deltaTime * trackingSpeed);

			Vector3 camPosition = new Vector3 (newPos.x, newPos.y, -10f);
			Vector3 v3 = camPosition;
			float clampX = Mathf.Clamp (v3.x, minX, maxX);
			float clampY = Mathf.Clamp (v3.y, minY, maxY);
			transform.position = new Vector3 (clampX, clampY, -10f);
		}
	}
}
