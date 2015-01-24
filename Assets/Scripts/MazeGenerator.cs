using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class MazeGenerator : MonoBehaviour {

	public GameObject root;
	public GameObject start;
	
	public void Clean() {

		if (root == null)
			throw new InvalidOperationException ("Root not selected");

		foreach (Transform child in root.transform) {
			if (child != null)
				GameObject.DestroyImmediate(child.gameObject);
		}
		
		Debug.Log (gameObject.name + " clean complete.");
	}
	
	public void Build() {
		
		if (start == null)
			throw new InvalidOperationException ("Start not selected");

		if (root == null)
			throw new InvalidOperationException ("Root not selected");
		
		GameObject go = new GameObject ();
		go.name = "test";
		
		go.transform.parent = root.transform;

		Debug.Log (gameObject.name + " build complete.");
	}
}
