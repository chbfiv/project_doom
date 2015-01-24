using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class TouchCapture : MonoBehaviour {

	private ControlService _ctrlService;

	private void Start() {
		_ctrlService = Injector.Get<ControlService> ();
	}

	public void BeginDrag(BaseEventData ed) {
		PointerEventData pd = ed as PointerEventData;
		_ctrlService.BeginDrag (this, pd);
	}

	public void EndDrag(BaseEventData ed) {
		PointerEventData pd = ed as PointerEventData;
		_ctrlService.EndDrag (this, pd);
	}
}
