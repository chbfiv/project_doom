using UnityEngine;
using System.Collections;
using Pathfinding;

public class PlayerAI : MonoBehaviour {

	public Transform target;

	private Seeker seeker;
	private CharacterController controller;
	
	//The AI's speed per second
	public float speed = 100;
	
	//The max distance from the AI to a waypoint for it to continue to the next waypoint
	public float nextWaypointDistance = 3;
	
	//The waypoint we are currently moving towards
	private int currentWaypoint = 0;

	private ControlService _ctrlService;

	private Path path {
		get { return seeker.GetCurrentPath (); }
	}

	// Use this for initialization
	void Start () {
		_ctrlService = Injector.Get<ControlService> ();
		_ctrlService.PlayerStateChanged += HandlePlayerStateChanged;
		seeker = GetComponent<Seeker> ();
		controller = GetComponent<CharacterController> ();

		seeker.pathCallback += OnPathCompelte;
	}

	private void HandlePlayerStateChanged ()
	{
		if (_ctrlService.playerState == PlayerState.Explore) {
			ResetPath();

			if (path != null && !path.IsDone()) 
				seeker.StartPath (transform.position, _ctrlService.targetToExplore);
			else
				seeker.StartPath (transform.position, _ctrlService.targetToExplore);
		}
	}

	private void ResetPath() {
		currentWaypoint = 0;

		Path p = seeker.path;
		if (p != null) {
			p.Claim (this);
			seeker.path = null;
			p.Release(this);
		}
	}

	void OnDestroy() {
		seeker.pathCallback -= OnPathCompelte;
		_ctrlService.PlayerStateChanged -= HandlePlayerStateChanged;
	}
	
	private void OnPathCompelte(Path p) {

		if (!p.error) {
			Debug.Log ("new path found.");
		} else {
			Debug.LogError("Failed to locate path.");
		}
	}

	private void FixedUpdate() {

		if (_ctrlService.playerState != PlayerState.Explore) {
			return;
		}

		if (path == null) {
			//We have no path to move after yet
			return;
		}
		
		if (currentWaypoint >= path.vectorPath.Count) {
			Debug.Log ("End Of Path Reached");
			ResetPath();
			_ctrlService.playerState = PlayerState.Rest;
			return;
		}
		
		//Direction to the next waypoint
		Vector3 dir = (path.vectorPath[currentWaypoint]-transform.position).normalized;
		dir *= speed * Time.fixedDeltaTime;
		controller.SimpleMove (dir);
		
		transform.rotation = Quaternion.LookRotation (dir);

		//Check if we are close enough to the next waypoint
		//If we are, proceed to follow the next waypoint
		if (Vector3.Distance (transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
			currentWaypoint++;
			return;
		}
	}
}
