using UnityEngine;
using System.Collections;

public class player : MonoBehaviour {

	public static player instance;

	public Rigidbody body;
	public Quaternion yRot = Quaternion.identity;


	void Start () {

		body = GetComponent ("Rigidbody") as Rigidbody;
		body.freezeRotation = true;
		//body.useGravity = false;
		
		yRot = Quaternion.identity;
	}
	
	// Update is called once per frame
	void Update () {
		//smooth
		//rotation *= Quaternion.AngleAxis(rotAmt, new Vector3(0,0,1));
		//transform.rotation = rotation;
		//var rot = rotation.eulerAngles;
		//float desRot = -80 * axis;
		//float rotAmt = dif (desRot,rot.z)/10;
		//discrete
		//transform.rotation = Quaternion.AngleAxis(axis * 80,new Vector3(0,0,1));
		//transform.rotation *= Quaternion.AngleAxis(vert*80, new Vector3(0,1,0));
		
		
		float horizAxis = -Input.GetAxis ("Horizontal");
		
		var desZRot = Quaternion.AngleAxis(horizAxis * 70, Vector3.forward);
		
		float v = .05f * body.velocity.magnitude;
		v = Mathf.Clamp(v,0,4);
		
		var deltaYRot = Quaternion.AngleAxis(-v * horizAxis, Vector3.up);
		yRot *= deltaYRot;
		
		//ok?
		
		transform.rotation = Quaternion.Slerp(transform.rotation, yRot * desZRot, 5*Time.deltaTime);
		//transform.rotation *= rotation;
		
		if (Input.GetAxis ("Jump") == 1)
			body.AddForce(10 * transform.forward);
		
		body.velocity = deltaYRot * body.velocity;
		
		
	}

	float dif(float rot1, float rot2){
		float val = rot1 - rot2;
		if (Mathf.Abs (val) > 180)
			return - Mathf.Sign (val) * (360 - Mathf.Abs (val));

		return (rot1 - rot2);
	}

}
