using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using System;

public enum ControlState {
	Default = 0,
	Pan = 1,
	Zoom = 2
}

public enum PlayerState {
	Rest = 0,
	Explore = 1
}

public class ControlService : MonoBehaviour {

	public Camera _main;
	private Transform _mainXform;

	public float minSize = 25f;
	public float maxSize = 100f;
	public float panSensitivity = 0.05f;   
	public float orthoZoomSpeed = 0.25f;
	
	private Dictionary<int, PointerEventData> _drags = new Dictionary<int, PointerEventData>();

	private ControlState _ctrlState = ControlState.Default;
	private PlayerState _playerState = PlayerState.Rest;

	private Vector3 _panVelocity = Vector2.zero;
	private float _zoomVelocity = 0f;

	public Vector3 targetToExplore = Vector3.zero;

	public event Action PlayerStateChanged;

	private void Start () {
		Injector.Register<ControlService> (this);

		_mainXform = _main.transform;
	}

	public PlayerState playerState {
		get { return _playerState; }
		set {
			_playerState = value;
			Action temp = PlayerStateChanged;
			if (temp != null)
				temp();
		}
	}
  
  	public ControlState ctrlState {
		get { return _ctrlState; }
	}

  	public IDictionary<int, PointerEventData> drags {
		get { return _drags; }
	}

	public void PointerClick(MonoBehaviour sender, PointerEventData e) {
		ProcessPlayerClick (e.position);
	}

	public void BeginDrag(MonoBehaviour sender, PointerEventData e) {
//		Logger.Log ("BeginDrag:" + Input.touchCount + ":" + e.pointerId);
		_drags [e.pointerId] = e;

		if (_ctrlState == ControlState.Default && _drags.Count == 1) {
			_ctrlState = ControlState.Pan;
		} else if (_drags.Count == 2) { 
			_ctrlState = ControlState.Zoom;
		}
	}	

	public void EndDrag(MonoBehaviour sender, PointerEventData e) {
		//		Logger.Log ("EndDrag:" + Input._dragstouchCount + ":" + e.pointerId);
		if (_drags.ContainsKey (e.pointerId))
			_drags.Remove (e.pointerId);
	}	

	private void ProcessPlayerClick(Vector2 pos) {
		
		if (_ctrlState == ControlState.Default || playerState == PlayerState.Explore) {
			
			Vector3 worldPos = _main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, _main.nearClipPlane));
			
			RaycastHit rayHit;
			//HACK: bug?
			//			if (Physics.Raycast(worldPos, _main.transform.forward, out rayHit, 400f, LayerMask.NameToLayer("Enemies"))) {
			if (Physics.Raycast(worldPos, _main.transform.forward, out rayHit, 400f)) {
				
				//				Debug.DrawRay(worldPos, _main.transform.forward * 400f, Color.green, 2f);
				if (rayHit.collider != null) {
					Logger.Log("hit:" + rayHit.collider.name + " " + rayHit.collider.tag);
					targetToExplore = rayHit.point;
					playerState = PlayerState.Explore;
				}
			} 
			
			//			else {
			//				Debug.DrawRay(pos, _main.transform.forward * 400f, Color.red, 2f);
			//			}
			
		}
	}

	private void Update() {
#if UNITY_EDITOR
		if (Input.GetMouseButton(0)) {
			ProcessPlayerClick(Input.mousePosition);
		}
#endif
		if (_ctrlState == ControlState.Pan && _drags.Count == 1) {
			// Pan
			PointerEventData t = _drags.ElementAt(0).Value;

			Vector3 currentPos = _mainXform.position;

			_mainXform.Translate(Vector3.up * t.delta.y * panSensitivity * -1f);
			_mainXform.Translate(Vector3.right * t.delta.x * panSensitivity * -1f);
			
			_panVelocity = _mainXform.position - currentPos;
		} else if (_ctrlState == ControlState.Zoom && _drags.Count == 2) {
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

			float currentSize = _main.orthographicSize;

			// ... change the orthographic size based on the change in distance between the touches.
			_main.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

			//clamp
			_main.orthographicSize = Mathf.Min(_main.orthographicSize, maxSize);
			_main.orthographicSize = Mathf.Max(_main.orthographicSize, minSize);

			_zoomVelocity = _main.orthographicSize - currentSize;
		} else { 
			_ctrlState = ControlState.Default;

			if (_panVelocity.x > 0.1f || _panVelocity.y > 0.1f) {
				_mainXform.Translate(_panVelocity);
				_panVelocity = _panVelocity * 0.01f;
			}

			if (_zoomVelocity > 0.1f || _zoomVelocity < -0.1f) {
				_main.orthographicSize += _zoomVelocity;
				_zoomVelocity = _zoomVelocity * 0.01f;
			}
		}
	}
}
