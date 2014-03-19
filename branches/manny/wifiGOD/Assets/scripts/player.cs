using UnityEngine;
using System.Collections;

public class player : MonoBehaviour {

	public static player instance;

	public Rigidbody body;
	public Quaternion yRot;

	public bool inPool;

	float leanAmt;


	void Start () {
		instance = this;

		body = GetComponent ("Rigidbody") as Rigidbody;
		body.freezeRotation = true;
		body.useGravity = false;
		yRot = Quaternion.identity;
	}
	
	// Update is called once per frame
	void Update () {
		//transform.up = Vector3.up;

		float horizAxis = -Input.GetAxis ("Horizontal");
		float vertAxis = Input.GetAxis("Vertical");
		Quaternion desZRot = Quaternion.identity;

		float turnAmt = .05f * body.velocity.magnitude;
		turnAmt = Mathf.Clamp(turnAmt,1,4);

		var deltaYRot = Quaternion.AngleAxis(-turnAmt * horizAxis, transform.up);
		yRot *= deltaYRot;

		if(inPool){
			//var normal = (poolScript.instance.transform.position - transform.position).normalized;
			//transform.up = Vector3.Lerp(transform.up, normal, 10*Time.deltaTime);
		}
		//else
			//desZRot = Quaternion.AngleAxis(horizAxis * 70, Vector3.forward);

		float dLean = (vertAxis - leanAmt) ;
		leanAmt += dLean;


		transform.rotation *= deltaYRot;
		var dZRot = Quaternion.AngleAxis(dLean * 70, Vector3.forward);
		transform.rotation *= dZRot;// * deltaYRot;// * dZRot;
		//transform.rotation = yRot * desZRot;
		//transform.rotation = Quaternion.Slerp(transform.rotation, yRot * desZRot, 5*Time.deltaTime);
		
		//if (Input.GetAxis ("Jump") == 1)
		//	body.AddForce(10 * transform.forward);
		
		//body.velocity = deltaYRot * body.velocity;
		
	}
	
	void updateRotBowl(){
		transform.up = Vector3.up;
	}

	float subAngles(float angle1, float angle2){
		float val = angle1 - angle2;
		if (Mathf.Abs (val) > 180)
			return - Mathf.Sign (val) * (360 - Mathf.Abs (val));

		return (angle1 - angle2);
	}

}
