using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Text;

public class Logger : MonoBehaviour {

	public Text guiText;
	
	public  float updateInterval = 0.5F;
	
	private float accum   = 0; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval

	private static List<string> info_logs = new List<string>();
	private static List<string> warning_logs = new List<string>();
	private static List<string> error_logs = new List<string>();

	private float purgeAccum = 0;
	public  float purgeInterval = 5F;

	public static void Log(string msg) {
		Debug.Log (msg);
		info_logs.Add (msg);
	}

	public static void LogWarning(string msg) {
		Debug.LogWarning (msg);
		warning_logs.Add (msg);
	}

	public static void LogError(string msg) {
		Debug.LogError (msg);
		error_logs.Add (msg);
	}

	void Start()
	{
		if( !guiText )
		{
			Debug.Log("UtilityFramesPerSecond needs a GUIText component!");
			enabled = false;
			return;
		}
		timeleft = updateInterval;  
	}
	
	void Update()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		purgeAccum += Time.deltaTime;

		if (purgeAccum > purgeInterval) {
			info_logs.Clear();
			warning_logs.Clear();
			error_logs.Clear();
			purgeAccum = 0;
		}

		// Interval ended - update GUI text and start new interval
//		if( timeleft <= 0.0 ) {
			StringBuilder sr = new StringBuilder();

			foreach(String str in info_logs) {
				sr.Append(str + "\n");
			}

			foreach(String str in warning_logs) {
				sr.Append(str + "\n");
			}

			foreach(String str in error_logs) {
				sr.Append(str + "\n");
			}

			guiText.text = sr.ToString();
			
			if(error_logs.Count > 0)
				guiText.color = Color.red;
			else if(warning_logs.Count > 0)
				guiText.color = Color.yellow;
			else
				guiText.color = Color.green;

//			timeleft = updateInterval;
//			accum = 0.0F;
//			frames = 0;
//		}
	}
}
