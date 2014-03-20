using UnityEngine;
using System.Collections;

public class plateScript : MonoBehaviour {
	
	public static plateScript instance;
	
	// Use this for initialization
	void Awake () {
		instance = this;
	}
	
	void OnCollisionEnter(Collision collision){
		player.instance.onPlate = true;

		if(player.instance.body.velocity.y < 0)
			player.instance.transform.up = Vector3.up;
	}
	
	void OnCollisionExit(Collision collision){
		player.instance.onPlate = false;
	}
}
