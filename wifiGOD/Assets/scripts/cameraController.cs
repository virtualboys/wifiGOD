using UnityEngine;
using System.Collections;

public class cameraController : MonoBehaviour {

	public static cameraController instance;

	public Color clearColor;

	public Camera cam;
	public playerBehavior player;

	public Vector3 horizOffset;
	public Vector3 newHOffset;
	int numCyclesTurning;

	void Start () {

		instance = this;

		cam = GetComponent ("Camera") as Camera;
		cam.backgroundColor = clearColor;

		horizOffset = player.transform.rotation * new Vector3(0,0,-1);
	}

	// Update is called once per frame
	void Update() {

		if(player.inAir){
			cam.transform.position = player.transform.position 
				+ new Vector3(0, -.5f * player.body.velocity.y + 8, 0);

			//falling
			if(Input.GetAxis("Jump") != 1){
				if(player.angVel.magnitude > .05f)
					numCyclesTurning++;
				else
					numCyclesTurning = 0;

				//if(numCyclesTurning < 30){
					Vector3 rot = new Vector3(1,0,0);
					rot = player.transform.rotation * rot;
					newHOffset = new Vector3(rot.z, 0, -rot.x);
				//}

				float timeConst = 2 * Mathf.Min(1, 5 * player.angVel.magnitude);
				horizOffset = Vector3.Slerp(horizOffset, newHOffset, Time.deltaTime * 5);
				//}

				cam.transform.position += 5 * horizOffset;
			}

			else{
				newHOffset = horizOffset/10;
				horizOffset = Vector3.Slerp(horizOffset, newHOffset, Time.deltaTime * 5);
				cam.transform.position += 5 * horizOffset;
			}


		}
		else{
			cam.transform.position = player.transform.position 
				- 8 * player.transform.forward + new Vector3(0, 4, 0);
		}

		cam.transform.LookAt (player.transform.position);
	}
}
