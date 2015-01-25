using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;



[ExecuteInEditMode]
public class MazeGenerator : MonoBehaviour {

	public GameObject mazeRoot;
	public GameObject exit;

	public float step = 4f;
	public GameObject cornerPrefab;
	public Vector3 cornerOffset = Vector3.zero;
	public GameObject wallPrefab;
	public Vector3 wallOffset = Vector3.zero;
	
	public GameObject cornerLayoutPrefab;
	public GameObject wallLayoutPrefab;

	public float stepDelay = 0.01f;
	
	public GameObject layoutRoot;
	public GameObject layoutPivotsParent;

	public void CleanLayout() {

		if (layoutRoot == null)
			throw new InvalidOperationException ("laytout root not selected");

		Transform rootXform = layoutRoot.transform;

		for (int i = rootXform.childCount - 1; i >= 0; i--) {
			Transform child = rootXform.GetChild(i);	
			if (child != null)
				GameObject.DestroyImmediate(child.gameObject);
		}

		Debug.Log (gameObject.name + " layout clean complete.");
	}

	public void BuildLayout() {

		CleanLayout ();
		
		if (layoutPivotsParent == null)
			throw new InvalidOperationException ("layout pivots should not be null");

		Transform parentXform = layoutPivotsParent.transform;

		if (parentXform.childCount <= 1)
			throw new InvalidOperationException ("layout pivots should be > 1");
		
		Transform previousXform;
		Transform currentXform = parentXform.GetChild (0);

		// make first corner
		CreateLayoutCorner (currentXform.position);

		for (int i=1; i < parentXform.childCount; i++) {
			previousXform = parentXform.GetChild(i-1);
			currentXform = parentXform.GetChild(i);

			CreateLayoutCorner(currentXform.position);

			ConnectLayoutCorners(previousXform.position, currentXform.position);
		}

		// one more time
		previousXform = currentXform;
		currentXform = parentXform.GetChild (0);

		ConnectLayoutCorners(currentXform.position, previousXform.position);

		Debug.Log (gameObject.name + " build layout complete.");
	}

	private void CreateLayoutCorner(Vector3 pos) {
		if (cornerLayoutPrefab == null)
			throw new InvalidOperationException ("Corner layout prefab should not be null");
		
		GameObject obj = GameObject.Instantiate (cornerLayoutPrefab);
		Transform objXform = obj.transform;
		obj.name = BuildTileName("corner_layout", pos);
		objXform.position = pos;
//		objXform.forward = ray.dir * -1f;
		objXform.parent = layoutRoot.transform;
	}

	private void CreateLayoutWall(Vector3 pos, Vector3 dir) {
		if (wallLayoutPrefab == null)
			throw new InvalidOperationException ("Wall layout prefab should not be null");
		
		GameObject obj = GameObject.Instantiate (wallLayoutPrefab);
		Transform objXform = obj.transform;
		obj.name = BuildTileName("wall_layout",pos);
		objXform.position = pos;
		objXform.forward = dir;
		objXform.parent = layoutRoot.transform;
	}

	private void ConnectLayoutCorners(Vector3 startPos, Vector3 endPos) {
		float distance = Vector3.Distance (startPos, endPos);
		int indexDist = Mathf.RoundToInt (distance);
		int stepDist = Mathf.RoundToInt (step);

		if (indexDist < stepDist)
			throw new InvalidOperationException ("min distance is " + stepDist);

		if ((indexDist % step) != 0)
			throw new InvalidOperationException ("distance should be a multiple of " + stepDist + "; " + indexDist);

		Vector3 dir = (endPos - startPos).normalized;

		//HACK: offset 2
		for (int i = 0; i < indexDist; i+= stepDist) {
			int wallOffset = i + 2;
			int cornerOffset = i + 4;
			CreateLayoutWall(startPos + (dir * wallOffset), dir);

			if (cornerOffset < indexDist) {
				CreateLayoutCorner(startPos + (dir * cornerOffset));
			}
		}
	}	

	public void CleanMaze() {
		
		if (mazeRoot == null)
			throw new InvalidOperationException ("maze root not selected");
		
		Transform rootXform = mazeRoot.transform;
		
		for (int i = rootXform.childCount - 1; i >= 0; i--) {
			Transform child = rootXform.GetChild(i);	
			if (child != null)
				GameObject.DestroyImmediate(child.gameObject);
		}
		
		Debug.Log (gameObject.name + " maze clean complete.");
	}
	
	public void BuildMaze() {

		CleanMaze ();

		if (exit == null)
			throw new InvalidOperationException ("Exit not selected");

		if (mazeRoot == null)
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
			if (mazeRoot.transform.FindChild(BuildTileName("tile",ray.origin)) != null) {
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
		
		Debug.Log (gameObject.name + " build maze complete.");
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
  
	private static string BuildTileName(string tag, Vector3 pos) {
		return tag + "(" + pos.x + "," + pos.y + "," + pos.z + ")";
	}

	private void CreateCorner(GameRay ray) {
		if (cornerPrefab == null)
			throw new InvalidOperationException ("Corner prefab should not be null");

		GameObject obj = GameObject.Instantiate (cornerPrefab);
		Transform objXform = obj.transform;
		obj.name = BuildTileName("tile", ray.origin);
		objXform.position = ray.origin + cornerOffset;
		objXform.forward = ray.dir * -1f;
		objXform.parent = mazeRoot.transform;
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
