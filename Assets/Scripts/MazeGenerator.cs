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
	public Vector3 cornerOffset = Vector3.zero;
	public GameObject wallPrefab;
	public Vector3 wallOffset = Vector3.zero;
	
	public float stepDelay = 0.01f;

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

		Step (new GameRay(exit.transform));
	}

	private void Step(GameRay ray) {

		Stack<GameRay> stack = new Stack<GameRay> ();
		stack.Push (ray);

		if (Application.isPlaying) {
			StartCoroutine (DoStep (stack));
		} else {
			//HACK: editor doesn't support coroutines
			IEnumerator e = DoStep (stack);
			while(e.MoveNext()) {

			}
		}
	}

	private IEnumerator DoStep(Stack<GameRay> rayStack) { 
		
		Stack<GameRay> testRayStack = new Stack<GameRay> ();

		while (rayStack.Count > 0) {
			GameRay ray = rayStack.Pop();

			// double check not already created
			if (root.transform.FindChild(BuildTileName(ray.origin)) != null) {
				throw new Exception("PROBLEM!");
			}

			CreateCorner (ray);

			PushRandomDirections(testRayStack, ray.origin);

			ProcessStacks(rayStack, testRayStack);

			if (Application.isPlaying)
				yield return new WaitForSeconds(stepDelay);
			else
				yield return null;
    	}
		
		Debug.Log (gameObject.name + " build complete.");
	}

	private bool ProcessStacks(Stack<GameRay> stack, Stack<GameRay> queue) {

		while (queue.Count > 0) {
			GameRay testRay = queue.Pop();
			
			RaycastHit info;
			if (!Physics.Raycast (testRay.origin, testRay.dir, out info, step)) {
				Debug.DrawRay(testRay.origin, testRay.dir * step, Color.green, stepDelay * 10f);
				//					Debug.Log("miss: " + origin + ", " + (dir * step));
				stack.Push(new GameRay(testRay.origin + (testRay.dir * step), testRay.dir));
				return true;
			} else {
				//					Debug.LogWarning("hit: " + info.collider.name + ", " + origin + ", " + (dir * step));
				Debug.DrawRay(testRay.origin, testRay.dir * step, Color.red, stepDelay * 10f);
			}
		}

		return false;
	}
  
	private static string BuildTileName(Vector3 pos) {
		return "tile(" + pos.x + "," + pos.y + "," + pos.z + ")";
	}

	private void CreateCorner(GameRay ray) {
		if (cornerPrefab == null)
			throw new InvalidOperationException ("Corner prefab should not be null");

		GameObject obj = GameObject.Instantiate (cornerPrefab);
		Transform objXform = obj.transform;
		obj.name = BuildTileName(ray.origin);
		objXform.position = ray.origin + cornerOffset;
		objXform.forward = ray.dir * -1f;
		objXform.parent = root.transform;
	}

	private void PushRandomDirections(Stack<GameRay> queue, Vector3 origin) {

		List<Vector3> options = new List<Vector3> ();
		options.Add (Vector3.left);
		options.Add (Vector3.right);
		options.Add (Vector3.forward);
		options.Add (Vector3.back);

		int count = options.Count;

		for (int i = 0; i < count; i++) {
			int index = GetRandomDirectionIndex(options);
			queue.Push(new GameRay(origin, options[index]));
			options.RemoveAt(index);
		}
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
