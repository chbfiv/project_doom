using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ControlService : MonoBehaviour {

	private void Start () {
		Injector.Register<ControlService> (this);
	}

	public void BeginDrag(MonoBehaviour sender, PointerEventData e) {
		Logger.Log ("BeginDrag:" + Input.touchCount + ":" + e.pointerId);

	}	

	public void EndDrag(MonoBehaviour sender, PointerEventData e) {
		Logger.Log ("EndDrag:" + Input.touchCount + ":" + e.pointerId);
	}	
}
