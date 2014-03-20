using UnityEngine;
using System.Collections;

public class cameraController : MonoBehaviour {

	public static cameraController instance;
	
	public Camera camera;
	public playerBehavior player;

	void Start () {

		instance = this;

		camera = GetComponent ("Camera") as Camera;
	}

	// Update is called once per frame
	void Update() {

		if(player.inAir){
			camera.transform.position = player.transform.position 
				+ new Vector3(0, -.5f * player.body.velocity.y + 8, 0)
				+ Quaternion.AngleAxis(player.lookAngle, Vector3.up) * new Vector3(0, 0, -8);
		}
		else{
			camera.transform.position = player.transform.position 
				- 8 * player.transform.forward + new Vector3(0, 4, 0);
		}

		camera.transform.LookAt (player.transform.position);
	}
}
