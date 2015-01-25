using UnityEngine;
using System.Collections;
using System;

public class CameraFollow : MonoBehaviour 
{
	public float xMargin = 1f;		// Distance in the x axis the player can move before the camera follows.
	public float zMargin = 1f;		// Distance in the y axis the player can move before the camera follows.
	public float xSmooth = 8f;		// How smoothly the camera catches up with it's target movement in the x axis.
	public float zSmooth = 8f;		// How smoothly the camera catches up with it's target movement in the y axis.
	public Vector3 maxXAndZ;		// The maximum x and y coordinates the camera can have.
	public Vector3 minXAndZ;		// The minimum x and y coordinates the camera can have.
	public Vector3 playerOffset;

	private Transform xform;
	private Transform player;		// Reference to the player's transform.


	void Awake () {
		xform = transform;

		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

		if (players.Length > 1)
			throw new InvalidOperationException ("more than 1 player found");

		// Setting up the reference.
		player = players[0].transform;

//		transform.position = new Vector3(player.x, transform.position.y, player.z)
	}

	float GetXMargin()
	{
		// Returns true if the distance between the camera and the player in the x axis is greater than the x margin.
		return Mathf.Abs(transform.position.x - currentPlayerPos.x);
  	}
  
	bool CheckXMargin()
	{
		// Returns true if the distance between the camera and the player in the x axis is greater than the x margin.
		return GetXMargin() > xMargin;
	}

	float GetZMargin()
	{
		// Returns true if the distance between the camera and the player in the y axis is greater than the y margin.
		return Mathf.Abs(transform.position.z - currentPlayerPos.z);
 	 }

  	bool CheckZMargin()
	{
		// Returns true if the distance between the camera and the player in the y axis is greater than the y margin.
		return GetZMargin() > zMargin;
	}


	void FixedUpdate ()
	{
		TrackPlayer();
	}
	
	private Vector3 lastPlayerPos = Vector3.zero;

	private Vector3 currentPlayerPos {
		get {
			return player.position + playerOffset;
		}
	}

	void TrackPlayer () {

		Vector3 playerVel = (currentPlayerPos - lastPlayerPos);

		// By default the target x and y coordinates of the camera are it's current x and y coordinates.
		float targetX = xform.position.x;
		float targetZ = xform.position.z;

		// If the player has moved beyond the x margin...
		if (CheckXMargin ()) {
			float localXMargin = GetXMargin();
			// ... the target x coordinate should be a Lerp between the camera's current x position and the player's current x position.
//			targetX = Mathf.Lerp(xform.position.x, player.position.x + playerOffset.x, xSmooth * Time.fixedDeltaTime);
//			targetX = Mathf.Lerp(xform.position.x + playerVel.x, xform.position.x + xMargin - localXMargin, localXMargin / (xMargin*4f));

			targetX = xform.position.x + playerVel.x;
		}

		// If the player has moved beyond the y margin...
		if(CheckZMargin()) {
			float localZMargin = GetZMargin();
			// ... the target y coordinate should be a Lerp between the camera's current y position and the player's current y position.
//			targetZ = Mathf.Lerp(xform.position.z, player.position.z + playerOffset.z, zSmooth * Time.fixedDeltaTime);
//			targetZ = Mathf.Lerp(xform.position.z + playerVel.z, xform.position.z + zMargin - localZMargin, localZMargin / (zMargin*4f));
			targetZ = xform.position.z + playerVel.z;
		}

		// The target x and y coordinates should not be larger than the maximum or smaller than the minimum.
		targetX = Mathf.Clamp(targetX, minXAndZ.x, maxXAndZ.x);
		targetZ = Mathf.Clamp(targetZ, minXAndZ.z, maxXAndZ.z);

		// Set the camera's position to the target position with the same z component.
		xform.position = new Vector3(targetX, xform.position.y, targetZ);

		lastPlayerPos = currentPlayerPos;
	}
}
