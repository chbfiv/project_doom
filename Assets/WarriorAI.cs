using UnityEngine;
using System.Collections;

public class WarriorAI : MonoBehaviour {

	private void Start () {
		GetComponent<Animation>().Play ("run");
	}
}
