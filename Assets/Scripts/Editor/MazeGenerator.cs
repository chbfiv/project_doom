using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class MazeGenerator : EditorWindow {

	public float step = 4f;
	public GameObject root;

	[MenuItem ("GlobalGameJam2015/Maze Generator %0")]
	public static void ShowMazeGenerator() {
		MazeGenerator window = (MazeGenerator)EditorWindow.GetWindow (typeof (MazeGenerator));
	}
	
	private void OnGUI () {
		GUILayout.Label ("Maze Generator", EditorStyles.boldLabel);
		step = EditorGUILayout.FloatField ("Step", step);
		root = (GameObject)EditorGUILayout.ObjectField ("Root", root, typeof(GameObject), true);

		if (GUILayout.Button("Clear")) {
			Clean();
    	}

		if (GUILayout.Button("Build")) {
			Build();
		}
	}

	private void Clean() {
		
		if (root == null)
			throw new InvalidOperationException ("Root not selected");

		GameObject.DestroyImmediate (root);
	}

	private void Build() {

		if (root == null)
			throw new InvalidOperationException ("Root not selected");

		GameObject go = new GameObject ();
		go.name = "test";

		go.transform.parent = root.transform;
	}
}
