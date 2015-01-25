using UnityEngine;
using System.Collections;
using Pathfinding;

public class PlayerAI : MonoBehaviour {

	public Transform target;

	private Seeker _seeker;

	// Use this for initialization
	void Start () {
		_seeker = GetComponent<Seeker> ();
		_seeker.StartPath (transform.position, target.position, OnPathCompelte);
	}
	
	private void OnPathCompelte(Path p) {
		Debug.Log ("Nice!");
	}
}
