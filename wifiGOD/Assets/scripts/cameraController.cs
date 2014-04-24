using UnityEngine;
using System.Collections;

public class cameraController : MonoBehaviour {

	public static cameraController instance;
	
	public Camera cam;
	public playerBehavior player;

	void Start () {

		instance = this;

		//camera = GetComponent ("Camera") as Camera;
	}

	// Update is called once per frame
	bool record;
	Vector2 rim;

	void Update() {

		if (player.transform.position.y < .41f) {
			if(record){
				var bowlradius=11.5f;
				var playervec= new Vector2(player.transform.position.x,player.transform.position.z);
				playervec.Normalize();
				rim= bowlradius*playervec;

				record=false;
			}
			camera.transform.position=Vector3.Slerp(camera.transform.position,new Vector3(rim.x,0,rim.y),Time.deltaTime);
			camera.transform.LookAt (player.transform.position);
		}
		else{
			record=true;
		if(player.inAir){
			camera.transform.position = player.transform.position 
				+ new Vector3(0, -.5f * player.body.velocity.y + 8, 0)
				+ Quaternion.AngleAxis(player.lookAngle, Vector3.up) * new Vector3(0, 0, -8);
			camera.transform.LookAt (player.transform.position);
		}

		else{
			var offset= new Vector3();
			offset=(new Vector3(0,-10,0)-player.transform.position);
			offset.x*=.4f;
			offset.y*=.3f;
			offset.z*= .4f;
			camera.transform.position = player.transform.position 
				-   (new Vector3(0,-10,0)-player.transform.position) + new Vector3(0, 3, 0);
			camera.transform.LookAt (new Vector3(0,-10,0));
		}
}


	}
}
