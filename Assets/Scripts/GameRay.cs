using UnityEngine;
using System.Collections;

public struct GameRay {
	private Vector3 _origin;
	private Vector3 _dir;

	
	public GameRay(Vector3 origin, Vector3 dir) {
		_origin = origin;
		_dir = dir;
	}

	public GameRay(Transform xform) {
		_origin = xform.position;
		_dir = xform.forward;
	}
	
	public Vector3 origin {
		get { return _origin; }
		set { _origin = value; }
	}
	
	public Vector3 dir {
		get { return _dir; }
		set { _dir = value; }
	}
}
