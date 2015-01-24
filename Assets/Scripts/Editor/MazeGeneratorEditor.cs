using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;

[CustomEditor(typeof(MazeGenerator))]
public class MazeGeneratorEditor : Editor {

	private MazeGenerator _mazeGenerator;
	private SerializedProperty RootProperty;
	private SerializedProperty StartProperty;

	public void OnEnable() {
		RootProperty = serializedObject.FindProperty("root");
		StartProperty = serializedObject.FindProperty("start");
		_mazeGenerator = target as MazeGenerator;
		
		if (_mazeGenerator == null)
			throw new InvalidOperationException ("MazeGenerator target should not be null");
	}

	public override void OnInspectorGUI() {

		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		serializedObject.Update();
		
		EditorGUI.indentLevel = 0;
		EditorGUILayout.Space();
		
		EditorGUILayout.PropertyField(RootProperty);
		EditorGUILayout.PropertyField(StartProperty);


		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("Clean", GUILayout.ExpandWidth(false))) {
			_mazeGenerator.Clean();
		}

		if (GUILayout.Button("Build", GUILayout.ExpandWidth(false))) {
			_mazeGenerator.Build();
		}

		EditorGUILayout.EndHorizontal();

		// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
		serializedObject.ApplyModifiedProperties();
	}
}
