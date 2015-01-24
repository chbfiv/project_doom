using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class MazeGeneratorWindow : EditorWindow {
//
//	public float step = 4f;
//	public GameObject root;

	[MenuItem ("GlobalGameJam2015/Maze Generator %0")]
	public static void ShowMazeGenerator() {
		MazeGeneratorWindow window = (MazeGeneratorWindow)EditorWindow.GetWindow (typeof (MazeGeneratorWindow));
	}
	
	private void OnGUI () {
		GUILayout.Label ("Maze Generator", EditorStyles.boldLabel);
//		step = EditorGUILayout.FloatField ("Step", step);
//		root = (GameObject)EditorGUILayout.ObjectField ("Root", root, typeof(GameObject), true);

		if (GUILayout.Button("Clear All")) {
			CleanAll();
    	}

		if (GUILayout.Button("Build All")) {
			BuildAll();
		}
	}

	private void CleanAll() {
		foreach(MazeGenerator mazeGenerator in GameObject.FindObjectsOfType<MazeGenerator> ()) {
			mazeGenerator.Clean();
		}
	}

	private void BuildAll() {
		foreach(MazeGenerator mazeGenerator in GameObject.FindObjectsOfType<MazeGenerator> ()) {
			mazeGenerator.Build();
		}
	}
}
