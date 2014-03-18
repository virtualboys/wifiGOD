using UnityEngine;
using System.Collections;

public class player : MonoBehaviour {

	public static player instance;

	public Rigidbody body;
	public Quaternion rotation = Quaternion.identity;


	void Start () {

		body = GetComponent ("Rigidbody") as Rigidbody;
		body.freezeRotation = true;
		body.useGravity = false;
	}
	
	// Update is called once per frame
	void Update () {
		float axis = Input.GetAxis ("Horizontal");
		var rot = transform.rotation.eulerAngles;
		float desRot = -80 * axis;
		float rotAmt = dif (desRot,rot.z)/10;
		
		transform.rotation = Quaternion.identity;
		
		
		float vert = Input.GetAxis ("Vertical");
		float vertRotAmt = dif (desRot,rot.z)/10;
		
		rotation = Quaternion.Euler (new Vector3 (0, 0, vert * 80));
		
		//if (Input.GetAxis ("Jump") == 1) {
			Vector3 f =  Quaternion.AngleAxis(transform.eulerAngles.z,new Vector3(0,1,0)) * transform.forward;
			f.Normalize ();
			transform.rotation =   Quaternion.Euler(new Vector3(0,-rotAmt*10,0)) * rotation;
			//transform.rotation *= Quaternion.AngleAxis(-rotAmt,new Vector3(0,1,0));
			//body.AddForce (10 * transform.forward);
			Debug.Log (f);
		//}
		
		//transform.rotation *= rotation;
		
			
	}

	float dif(float rot1, float rot2){
		float val = rot1 - rot2;
		if (Mathf.Abs (val) > 180)
			return - Mathf.Sign (val) * (360 - Mathf.Abs (val));

		return (rot1 - rot2);
	}

}
