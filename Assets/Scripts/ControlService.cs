using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public enum ControlState {
	Default = 0,
	Pan = 1,
	Zoom = 2
}

public class ControlService : MonoBehaviour {

	public Camera _main;
	private Transform _mainXform;

	public float minSize = 25f;
	public float maxSize = 100f;
	public float panSensitivity = 0.05f;   
	public float orthoZoomSpeed = 0.25f;
	
	public Dictionary<int, PointerEventData> _drags = new Dictionary<int, PointerEventData>();

	private ControlState _state = ControlState.Default;

	private void Start () {
		Injector.Register<ControlService> (this);

		_mainXform = _main.transform;
	}

	public void BeginDrag(MonoBehaviour sender, PointerEventData e) {
//		Logger.Log ("BeginDrag:" + Input.touchCount + ":" + e.pointerId);
		_drags [e.pointerId] = e;

		if (_state == ControlState.Default && _drags.Count == 1) {
			_state = ControlState.Pan;
		} else if (_drags.Count == 2) { 
			_state = ControlState.Zoom;
		}
	}	

	public void EndDrag(MonoBehaviour sender, PointerEventData e) {
		//		Logger.Log ("EndDrag:" + Input._dragstouchCount + ":" + e.pointerId);
		if (_drags.ContainsKey (e.pointerId))
			_drags.Remove (e.pointerId);
	}	

	private void Update() {
		if (_state == ControlState.Pan && _drags.Count == 1) {
			// Pan
			PointerEventData t = _drags.ElementAt(0).Value;
			_mainXform.Translate(Vector3.up * t.delta.y * panSensitivity * -1f);
			_mainXform.Translate(Vector3.right * t.delta.x * panSensitivity * -1f);
		} else if (_state == ControlState.Zoom && _drags.Count == 2) {
			// Zoom
			PointerEventData t0 = _drags.ElementAt(0).Value;
			PointerEventData t1 = _drags.ElementAt(1).Value;

			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = t0.position - t0.delta;
			Vector2 touchOnePrevPos = t1.position - t1.delta;
			
			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (t0.position - t1.position).magnitude;
			
			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
			
			// ... change the orthographic size based on the change in distance between the touches.
			_main.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

			//clamp
			_main.orthographicSize = Mathf.Min(_main.orthographicSize, maxSize);
			_main.orthographicSize = Mathf.Max(_main.orthographicSize, minSize);

		} else { 
			_state = ControlState.Default;
		}
	}
}
