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

		Quaternion dYRot = Quaternion.identity;
		Quaternion dZRot = Quaternion.identity;

		float horizAxis = -Input.GetAxis ("Horizontal");
		float turnAmt = .05f * body.velocity.magnitude;
		turnAmt = Mathf.Clamp(turnAmt, 1, 4);

		if(!inPool){
			float dLean = (horizAxis - leanAmt) / 5;
			leanAmt += dLean;
			
			dZRot = Quaternion.AngleAxis(dLean * 70, Vector3.forward);

			dYRot = Quaternion.AngleAxis(-turnAmt * horizAxis, Quaternion.Inverse(transform.rotation) * Vector3.up);
		}
		else{
			dYRot = Quaternion.AngleAxis(-3 * turnAmt * horizAxis, Vector3.up);
		}

		var dRot = dYRot * dZRot;
		transform.rotation *= Quaternion.Slerp(Quaternion.identity, dRot, Time.deltaTime * 50);

		//forward velocity
		if (Input.GetAxis ("Jump") == 1)
			body.AddForce(10 * transform.forward);

		if(!inPool){
			body.velocity = transform.rotation * Vector3.forward * body.velocity.magnitude;
		}
		else{
			//body.velocity *= 1.01f;
		}
	}

	void updateAir(){

		float horizInput = Input.GetAxis("Horizontal");
		float vertInput = Input.GetAxis("Vertical");

		var zRot = Quaternion.AngleAxis(5 * horizInput, Vector3.forward);
		var xRot = Quaternion.AngleAxis(5 * vertInput, Vector3.right);

		transform.rotation *= Quaternion.Slerp(Quaternion.identity, xRot * zRot, Time.deltaTime * 30);

		if(Input.GetAxis("Jump") == 1){
			hover ();
		}
	}

	void hover(){
		body.velocity *= .95f;
	}

	public void resetRot(){
		transform.up = Vector3.up;
		leanAmt = 0;
	}

	void sLerpToVec(Vector3 vec){

		float angle = -Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(vec, transform.up));
		Vector3 axis = Vector3.Cross(vec, transform.up);

		var quat = Quaternion.AngleAxis(angle, Quaternion.Inverse(transform.rotation) * axis);
		transform.rotation *= Quaternion.Slerp(Quaternion.identity, quat, Time.deltaTime * 5);
	}

	float subAngles(float angle1, float angle2){
		float val = angle1 - angle2;
		if (Mathf.Abs (val) > 180)
			return - Mathf.Sign (val) * (360 - Mathf.Abs (val));

		return (angle1 - angle2);
	}

}
