using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ControlTest : MonoBehaviour {

	public GameObject target;
	public Vector3 rot;
	public Material skybox;
	public float exposure;

	public void HandleMinusClick() {
		if (target != null) {
			target.transform.Rotate(-1f*rot);
		}
		if (skybox != null) {
			float currentExposure = skybox.GetFloat("_Exposure");
			skybox.SetFloat("_Exposure", currentExposure + (-1f*exposure));
		}
	}
	
	public void HandlePlusClick() {
		if (target != null) {
			target.transform.Rotate(rot);
		}

		if (skybox != null) {
			float currentExposure = skybox.GetFloat("_Exposure");
			skybox.SetFloat("_Exposure", currentExposure + exposure);
		}
	}
}
