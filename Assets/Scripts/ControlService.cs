using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ControlService : MonoBehaviour {

	public void BeginDrag(BaseEventData ed) {
		PointerEventData pd = ed as PointerEventData;
		Logger.Log ("BeginDrag:" + Input.touchCount + ":" + pd.pointerId);
	}

	public void EndDrag(BaseEventData ed) {
		PointerEventData pd = ed as PointerEventData;
		Logger.Log ("EndDrag:" + Input.touchCount + ":" + pd.pointerId);
	}
}
