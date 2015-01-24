using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

[ExecuteInEditMode]
public class MazeGenerator : MonoBehaviour {

	public GameObject root;
	public GameObject exit;

	public float step = 4f;
	public GameObject cornerPrefab;
	public GameObject wallPrefab;

	public void Clean() {

		if (root == null)
			throw new InvalidOperationException ("Root not selected");

		Transform rootXform = root.transform;

		for (int i = rootXform.childCount - 1; i >= 0; i--) {
			Transform child = rootXform.GetChild(i);	
			if (child != null)
				GameObject.DestroyImmediate(child.gameObject);
		}
		
		Debug.Log (gameObject.name + " clean complete.");
	}
	
	public void Build() {

		Clean ();

		if (exit == null)
			throw new InvalidOperationException ("Exit not selected");

		if (root == null)
			throw new InvalidOperationException ("Root not selected");

		Step (exit.transform.position);

		Debug.Log (gameObject.name + " build complete.");
	}

//	private void Update() {
//		
//		Debug.DrawRay(Vector3.zero, Vector3.forward * 1000f, Color.green);
//	}

	private void Step(Vector3 origin) {

		Queue<Vector3> queue = GetDirectionsQueue ();

		CreateCorner (origin);

		while (queue.Count > 0) {
			Vector3 dir = queue.Dequeue();
			
			RaycastHit info;
			if (!Physics.Raycast (origin, dir, out info, step)) {
//				Debug.DrawRay(origin, dir * 1000f, Color.green, 4f);
//				Debug.LogWarning("miss: " + origin + ", " + dir);
				Step (origin + (dir * step));
			} else {
//				Debug.LogWarning("hit: " + info.collider.name + ", " + origin + ", " + (dir * step));
//				Debug.DrawRay(origin, dir * 1000f, Color.red, 4f);
			}
		}
	}

	private void CreateCorner(Vector3 pos) {
		if (cornerPrefab == null)
			throw new InvalidOperationException ("Corner prefab should not be null");

		GameObject obj = GameObject.Instantiate (cornerPrefab);
		Transform objXform = obj.transform;
		objXform.position = pos;
		objXform.parent = root.transform;
	}

	private Queue<Vector3> GetDirectionsQueue() {

		Queue<Vector3> queue = new Queue<Vector3> ();

		List<Vector3> options = new List<Vector3> ();
		options.Add (Vector3.left);
		options.Add (Vector3.right);
		options.Add (Vector3.forward);
		options.Add (Vector3.back);

		int count = options.Count;

		for (int i = 0; i < count; i++) {
			int index = GetRandomDirectionIndex(options);
			queue.Enqueue(options[index]);
			options.RemoveAt(index);
		}

		return queue;
	}

	private int GetRandomDirectionIndex(IList<Vector3> options) {
		if (options == null)
			throw new InvalidOperationException ("Options should not be null");
		
		if (options.Count <= 0)
			throw new InvalidOperationException ("Options should be > 0");

		int index = UnityEngine.Random.Range (0, options.Count);
		return index;
	}
}
