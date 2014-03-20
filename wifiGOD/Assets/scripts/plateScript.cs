using UnityEngine;
using System.Collections;

public class plateScript : MonoBehaviour {
	
	public static plateScript instance;
	
	// Use this for initialization
	void Awake () {
		instance = this;
	}
	
	void OnCollisionEnter(Collision collision){
		playerBehavior.instance.onPlate = true;

		if(playerBehavior.instance.body.velocity.y < 0)
			playerBehavior.instance.resetRot();
	}
	
	void OnCollisionExit(Collision collision){
		playerBehavior.instance.onPlate = false;
	}
}
