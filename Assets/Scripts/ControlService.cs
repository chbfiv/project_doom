using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class ControlService : MonoBehaviour {

	public Camera _main;
	private Transform _mainXform;

	public float minSize = 5f;
	public float maxSize = 30f;
	public float panSensitivity = 0.1f;

	public Dictionary<int, PointerEventData> _drags = new Dictionary<int, PointerEventData>();

	private void Start () {
		Injector.Register<ControlService> (this);

		_mainXform = _main.transform;
	}

	public void BeginDrag(MonoBehaviour sender, PointerEventData e) {
//		Logger.Log ("BeginDrag:" + Input.touchCount + ":" + e.pointerId);
		_drags [e.pointerId] = e;
	}	

	public void EndDrag(MonoBehaviour sender, PointerEventData e) {
		//		Logger.Log ("EndDrag:" + Input._dragstouchCount + ":" + e.pointerId);
		if (_drags.ContainsKey (e.pointerId))
			_drags.Remove (e.pointerId);
	}	

	private void Update() {
		if (_drags.Count == 1) {
			PointerEventData e = _drags.ElementAt(0).Value;

			// Pan
			_mainXform.Translate(Vector3.up * e.delta.y * panSensitivity);
			_mainXform.Translate(Vector3.right * e.delta.x * panSensitivity);
		} else if (_drags.Count == 2) {
			// Zoom
		} 
	}
}
