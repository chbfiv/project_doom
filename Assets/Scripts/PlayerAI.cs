﻿using UnityEngine;
using System.Collections;
using Pathfinding;

public class PlayerAI : MonoBehaviour {

	public Transform target;

	private Seeker seeker;
	private CharacterController controller;
	
	//The calculated path
	public Path path;
	
	//The AI's speed per second
	public float speed = 100;
	
	//The max distance from the AI to a waypoint for it to continue to the next waypoint
	public float nextWaypointDistance = 3;
	
	//The waypoint we are currently moving towards
	private int currentWaypoint = 0;

	// Use this for initialization
	void Start () {
		seeker = GetComponent<Seeker> ();
		controller = GetComponent<CharacterController> ();
		seeker.StartPath (transform.position, target.position, OnPathCompelte);
	}
	
	private void OnPathCompelte(Path p) {

		if (!p.error) {
			path = p;
			//Reset the waypoint counter
			currentWaypoint = 0;
			Debug.Log ("Path found.");
		} else {
			Debug.LogError("Failed to locate path.");
		}
	}

	private void FixedUpdate() {
		if (path == null) {
			//We have no path to move after yet
			return;
		}
		
		if (currentWaypoint >= path.vectorPath.Count) {
			Debug.Log ("End Of Path Reached");
			return;
		}
		
		//Direction to the next waypoint
		Vector3 dir = (path.vectorPath[currentWaypoint]-transform.position).normalized;
		dir *= speed * Time.fixedDeltaTime;
		controller.SimpleMove (dir);
		
		//Check if we are close enough to the next waypoint
		//If we are, proceed to follow the next waypoint
		if (Vector3.Distance (transform.position,path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
			currentWaypoint++;
			return;
		}
	}
}
