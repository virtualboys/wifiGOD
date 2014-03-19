using UnityEngine;
using System.Collections;

public class player : MonoBehaviour {

	public static player instance;

	public Rigidbody body;

	public bool inPool;
	public bool onPlate;
	public bool inAir { get { return !inPool && !onPlate; } }

	float leanAmt;


	void Start () {
		instance = this;

		body = GetComponent ("Rigidbody") as Rigidbody;
		body.freezeRotation = true;
		//body.useGravity = false;
	}
	
	// Update is called once per frame
	void Update () {

		if(inAir)
			updateAir();
		else
			updateGround();

	}
	
	void updateGround(){

		var normal = Vector3.up;

		if(inPool){
			normal = (poolScript.instance.transform.position - transform.position).normalized;

		sLerpToVec(normal);
		}


		float horizAxis = -Input.GetAxis ("Horizontal");
		float turnAmt = .05f * body.velocity.magnitude;
		turnAmt = Mathf.Clamp(turnAmt,1,4);
		
		var dYRot = Quaternion.AngleAxis(-turnAmt * horizAxis, Quaternion.Inverse(transform.rotation)*Vector3.up);
		
		float dLean = (horizAxis - leanAmt) /5;
		leanAmt += dLean;
		
		var dZRot = Quaternion.AngleAxis(dLean * 70, Vector3.forward);
		
		var newRot = transform.rotation * dYRot * dZRot;
		transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime *50);

		//forward velocity
		if (Input.GetAxis ("Jump") == 1)
			body.AddForce(10 * transform.forward);

		body.velocity = Quaternion.AngleAxis(-turnAmt*horizAxis,Vector3.up) * body.velocity;
	}

	void updateAir(){

		float horizInput = Input.GetAxis("Horizontal");
		float vertInput = Input.GetAxis("Vertical");

		var zRot = Quaternion.AngleAxis(horizInput, Vector3.forward);
		var xRot = Quaternion.AngleAxis(vertInput, Vector3.right);

		transform.rotation *= xRot * zRot;
	}

	void sLerpToVec(Vector3 vec){
		float angle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(vec, transform.up));
		Vector3 axis = -Vector3.Cross(vec, transform.up);

		transform.rotation *= Quaternion.Slerp(Quaternion.identity, 
			Quaternion.AngleAxis(angle, axis), Time.deltaTime*5);
	}

	float subAngles(float angle1, float angle2){
		float val = angle1 - angle2;
		if (Mathf.Abs (val) > 180)
			return - Mathf.Sign (val) * (360 - Mathf.Abs (val));

		return (angle1 - angle2);
	}

}
