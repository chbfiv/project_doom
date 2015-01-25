using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;

[CustomEditor(typeof(MazeGenerator))]
public class MazeGeneratorEditor : Editor {

	private MazeGenerator _mazeGenerator;
//	private SerializedProperty RootProperty;
//	private SerializedProperty StartProperty;

	public void OnEnable() {
//		RootProperty = serializedObject.FindProperty("root");
//		StartProperty = serializedObject.FindProperty("start");
		_mazeGenerator = target as MazeGenerator;
		
		if (_mazeGenerator == null)
			throw new InvalidOperationException ("MazeGenerator target should not be null");
	}

	public override void OnInspectorGUI() {

		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		serializedObject.Update();
		
		EditorGUI.indentLevel = 0;
		EditorGUILayout.Space();

		base.OnInspectorGUI ();
//		EditorGUILayout.PropertyField(RootProperty);
//		EditorGUILayout.PropertyField(StartProperty);

		EditorGUILayout.BeginHorizontal();
		
		if (GUILayout.Button("Clean Layout", GUILayout.ExpandWidth(false))) {
			_mazeGenerator.CleanLayout();
		}
		
		if (GUILayout.Button("Build Layout", GUILayout.ExpandWidth(false))) {
			_mazeGenerator.BuildLayout();
		}
		
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("Clean Maze", GUILayout.ExpandWidth(false))) {
			_mazeGenerator.CleanMaze();
		}

		if (GUILayout.Button("Build Maze", GUILayout.ExpandWidth(false))) {
			_mazeGenerator.BuildMaze();
		}

		EditorGUILayout.EndHorizontal();

		// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
		serializedObject.ApplyModifiedProperties();
	}
}
