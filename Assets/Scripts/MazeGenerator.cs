using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;



[ExecuteInEditMode]
public class MazeGenerator : MonoBehaviour {

	public GameObject mazeRoot;
	public GameObject mazeExit;

	public float step = 4f;
	public GameObject cornerPrefab;
	public Vector3 cornerOffset = Vector3.zero;
	public GameObject wallPrefab;
	public Vector3 wallOffset = Vector3.zero;
	
	public GameObject cornerLayoutPrefab;
	public GameObject wallLayoutPrefab;

	public float stepDelay = 0.01f;

	public GameObject layoutPivotsParent;
	
	public GameObject gameRoot;
	public GameObject gameExit;
	
	public GameObject cornerGamePrefab;
	public GameObject wallGamePrefab;
	
	public float gameStep {
		get { return step / 2f; }
	}

	public void CleanGame() {
		
		if (gameRoot == null)
			throw new InvalidOperationException ("game root not selected");
		
		Transform rootXform = gameRoot.transform;
		
		for (int i = rootXform.childCount - 1; i >= 0; i--) {
			Transform child = rootXform.GetChild(i);	
			if (child != null)
				GameObject.DestroyImmediate(child.gameObject);
		}
		
		Debug.Log (gameObject.name + " game clean complete.");
	}
	
	public void BuildGame() {
		
		CleanGame ();

		if (gameExit == null)
			throw new InvalidOperationException ("Exit not selected");
		
		if (gameRoot == null)
			throw new InvalidOperationException ("Root not selected");
		
		BuildGameCore (new GameRay(gameExit.transform));
	}

	private void BuildGameCore(GameRay ray) {
		
		Stack<GameRay> stack = new Stack<GameRay> ();
		stack.Push (ray);
		
		if (Application.isPlaying) {
			StartCoroutine (BuildGameTask (stack));
		} else {
			//HACK: editor doesn't support coroutines
			IEnumerator e = BuildGameTask (stack);
			while(e.MoveNext()) {
				
			}
		}
	}
	
	private IEnumerator BuildGameTask(Stack<GameRay> rayStack) { 
		
		Stack<GameRay> testRayStack = new Stack<GameRay> ();
		
		while (rayStack.Count > 0) {
			GameRay ray = rayStack.Pop();
			
			// double check not already created
			if (gameRoot.transform.FindChild(BuildTileName("tile",ray.origin)) != null) {
				throw new Exception("PROBLEM!");
			}

			CreateGameTile (ray);
			
			PushGameDirections(testRayStack, ray.origin);
			
			while (testRayStack.Count > 0) {
				GameRay testRay = testRayStack.Pop();
				
				RaycastHit info;
				if (!Physics.Raycast (testRay.origin, testRay.dir, out info, gameStep)) {
					Debug.DrawRay(testRay.origin, testRay.dir * gameStep, Color.green, stepDelay * 10f);
					//					Debug.Log("miss: " + origin + ", " + (dir * step));
					rayStack.Push(new GameRay(testRay.origin + (testRay.dir * gameStep), testRay.dir));
					return true;
				} else {
					//					Debug.LogWarning("hit: " + info.collider.name + ", " + origin + ", " + (dir * step));
					Debug.DrawRay(testRay.origin, testRay.dir * gameStep, Color.red, stepDelay * 10f);
				}
			}
			
			if (Application.isPlaying)
				yield return new WaitForSeconds(stepDelay);
		      else
	       		 yield return null;
	    }
		    
	    Debug.Log (gameObject.name + " build game complete.");
	}

	private void CreateGameTile(GameRay ray) {
		string left = BuildTileName ("tile", ray.origin + (Vector3.left * gameStep));
		string right = BuildTileName ("tile", ray.origin + (Vector3.right * gameStep));
		string back = BuildTileName ("tile", ray.origin + (Vector3.back * gameStep));
		string forward = BuildTileName ("tile", ray.origin + (Vector3.forward * gameStep));

		Transform mazeXform = mazeRoot.transform;
		Transform gameXform = gameRoot.transform;
		bool hasLeft = (mazeXform.FindChild (left) != null) || (gameXform.FindChild (left) != null);
		bool hasRight = (mazeXform.FindChild (right) != null) || (gameXform.FindChild (right) != null);
		bool hasBack = (mazeXform.FindChild (back) != null) || (gameXform.FindChild (back) != null);
		bool hasForward = (mazeXform.FindChild (forward) != null) || (gameXform.FindChild (forward) != null);

		bool hasOne = hasLeft || hasRight || hasBack || hasForward;

		bool hasTwo = 	(hasLeft && hasRight) || 
						(hasLeft && hasForward) ||
						(hasLeft && hasBack) ||
						(hasRight && hasForward) || 
						(hasRight && hasBack) ||
        				(hasBack && hasForward);

        bool hasThree = (hasLeft && hasRight && hasBack) || 
						(hasRight && hasBack && hasForward);

		bool hasFour = hasLeft && hasRight && hasBack && hasForward;

		if (hasFour || hasThree || hasTwo) {
			CreateGameCorner(ray);
		} else {
			CreateGameWall(ray);
		}
  	}

  	private void CreateGameCorner(GameRay ray) {
		if (cornerGamePrefab == null)
			throw new InvalidOperationException ("Corner prefab should not be null");
		
		GameObject obj = GameObject.Instantiate (cornerGamePrefab);
		Transform objXform = obj.transform;
		obj.name = BuildTileName("tile", ray.origin);
		objXform.position = ray.origin + cornerOffset;
		objXform.forward = ray.dir;
		objXform.parent = gameRoot.transform;
   }

	private void CreateGameWall(GameRay ray) {
		if (wallGamePrefab == null)
			throw new InvalidOperationException ("Wall prefab should not be null");
		
		GameObject obj = GameObject.Instantiate (wallGamePrefab);
		Transform objXform = obj.transform;
		obj.name = BuildTileName("tile", ray.origin);
		objXform.position = ray.origin + cornerOffset;
		objXform.forward = ray.dir;
		objXform.parent = gameRoot.transform;
 	}

	private void PushGameDirections(Stack<GameRay> queue, Vector3 origin) {
		queue.Push(new GameRay(origin, Vector3.left));
		queue.Push(new GameRay(origin, Vector3.right));
		queue.Push(new GameRay(origin, Vector3.forward));
		queue.Push(new GameRay(origin, Vector3.back));
	}

	// **************************************** //

	public void BuildLayout() {

		CleanMaze ();
		
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
		obj.name = BuildTileName("tile", pos);
		objXform.position = pos;
//		objXform.forward = ray.dir * -1f;
		objXform.parent = mazeRoot.transform;
	}

	private void CreateLayoutWall(Vector3 pos, Vector3 dir) {
		if (wallLayoutPrefab == null)
			throw new InvalidOperationException ("Wall layout prefab should not be null");
		
		GameObject obj = GameObject.Instantiate (wallLayoutPrefab);
		Transform objXform = obj.transform;
		obj.name = BuildTileName("tile",pos);
		objXform.position = pos;
		objXform.forward = dir;
		objXform.parent = mazeRoot.transform;
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

		if (mazeExit == null)
			throw new InvalidOperationException ("Exit not selected");

		if (mazeRoot == null)
			throw new InvalidOperationException ("Root not selected");

		BuildMazeCore (new GameRay(mazeExit.transform));
	}

	private void BuildMazeCore(GameRay ray) {

		Stack<GameRay> stack = new Stack<GameRay> ();
		stack.Push (ray);

		if (Application.isPlaying) {
			StartCoroutine (BuildMazeTask (stack));
		} else {
			//HACK: editor doesn't support coroutines
			IEnumerator e = BuildMazeTask (stack);
			while(e.MoveNext()) {

			}
		}
	}

	private IEnumerator BuildMazeTask(Stack<GameRay> rayStack) { 
		
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
