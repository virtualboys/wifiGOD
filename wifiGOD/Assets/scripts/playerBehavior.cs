using UnityEngine;
using System.Collections;

public class playerBehavior : MonoBehaviour {

	public static playerBehavior instance;

	public Rigidbody body;

	public bool inPool;
	public bool onPlate;
	public bool inAir { get { return !inPool && !onPlate; } }

	public float lookAngle;

	float leanAmt;
	Vector2 angVel;


	void Awake () {

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
			//body.velocity = transform.forward * body.velocity.magnitude;
			body.velocity *= 1.015f;
		}
	}

	void updateAir(){

		float horizInput = Input.GetAxis("Horizontal");
		float vertInput = Input.GetAxis("Vertical");
		//float mouseInput = 10 * Input.GetAxis("Mouse X");

		//lookAngle += mouseInput;

		//Vector2 angInput =  new Vector2(Mathf.Cos(Mathf.Deg2Rad*lookAngle)* horizInput, Mathf.Sin(Mathf.Deg2Rad*lookAngle)*vertInput);
		angVel = Vector2.Lerp(angVel, new Vector2(horizInput, vertInput), Time.deltaTime * 5);

		var zRot = Quaternion.AngleAxis(-50 * angVel.x, Vector3.forward);
		var xRot = Quaternion.AngleAxis(50 * angVel.y, Quaternion.Inverse(transform.rotation) * Vector3.right);
		//var yRot = Quaternion.AngleAxis(mouseInput, Vector3.up);

		transform.rotation *= Quaternion.Slerp(Quaternion.identity, xRot * zRot, Time.deltaTime * 5);
		//transform.rotation *= yRot;

		if(Input.GetAxis("Jump") == 1){
			hover ();
		}
		else{
			var horizV = new Vector2(body.velocity.x, body.velocity.z);
			horizV = Vector2.Lerp(horizV, Vector2.zero, Time.deltaTime);
			body.velocity = new Vector3(horizV.x, body.velocity.y, horizV.y);
		}
	}

	void hover(){

		var v = body.velocity;
		v.y *= .95f;
		body.velocity = v;

		//Vector3 euler = transform.rotation.eulerAngles;

		float rad = Mathf.Deg2Rad * lookAngle;
		body.velocity += new Vector3(Mathf.Cos(rad) * angVel.x, 0, Mathf.Sin(rad) * angVel.y);
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
