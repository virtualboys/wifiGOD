using UnityEngine;
using System.Collections;

public class poolScript : MonoBehaviour {

	public static poolScript instance;

	// Use this for initialization
	void Awake () {
		instance = this;
	}

	void OnCollisionEnter(Collision collision){
		playerBehavior.instance.inPool = true;
	}

	void OnCollisionExit(Collision collision){
		playerBehavior.instance.inPool = false;
	}
}
