using UnityEngine;
using System.Collections;

public class playerBehavior : MonoBehaviour {

	public static playerBehavior instance;

	public int numHoverSlowCycles = 50;

	public Rigidbody body;
	public Collider collider;

	public bool inPool;
	public bool onPlate;
	public bool inAir { get {return !IsGrounded();} }

	public bool useG;

	float leanAmt;
	public Vector2 angVel;
	Vector2 airRot;

	float startHoverVel;
	float slowVel;

	Vector2 mousePos;
	Vector2 lastMousePos;

	float distToGround;

	void Awake () {
		Screen.showCursor = false;

		instance = this;

		//body = GetComponent ("Rigidbody") as Rigidbody;
		//body.freezeRotation = true;
		body.useGravity = useG;

		distToGround = collider.bounds.extents.y;
		Debug.Log(distToGround);

	}
	
	// Update is called once per frame
	void Update () {
		transform.position = body.position;

		if(inAir)
			updateAir();
		else
			updateGround();

		//transform.rotation = body.transform.rotation;
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

			dYRot = Quaternion.AngleAxis(-turnAmt * horizAxis, 
     			Quaternion.Inverse(transform.rotation) * Vector3.up);
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
			body.velocity *= 1.005f;

			if(playerBehavior.instance.body.velocity.y > 0 && Input.GetKeyDown("space"))
				playerBehavior.instance.launch();
		}
	}

	void updateAir(){

		//float horizInput = Input.GetAxis("Horizontal");
		//float vertInput = Input.GetAxis("Vertical");

		float horizInput = Input.GetAxis("Mouse X");
		float vertInput = Input.GetAxis("Mouse Y");

		angVel = Vector2.Lerp(angVel, new Vector2(horizInput, vertInput), Time.deltaTime * 5);

		if(Input.GetMouseButton(0)){
			//start hover reset tilt on new press
			if(Input.GetMouseButtonDown(0)){
				StartCoroutine(sLerpToVecCoroutine(Vector3.up));
				startHoverVel = body.velocity.y;
			}

			//update and clamp mouse pos
			mousePos -= new Vector2(horizInput, vertInput);
			if( Mathf.Abs(mousePos.x) > 30)
				mousePos.x = Mathf.Sign(mousePos.x) * 30;
			if(Mathf.Abs(mousePos.y) > 30)
				mousePos.y = Mathf.Sign (mousePos.y) * 30;

			hover ();
		}
		else{
			//end hover reset tilt
			if(Input.GetMouseButtonUp(0)){
				StartCoroutine(sLerpToVecCoroutine(Vector3.up));
				mousePos = Vector2.zero;
			}

			startHoverVel = 0;

			fall();

			//slow horiz vel
			var horizV = new Vector2(body.velocity.x, body.velocity.z);
			horizV = Vector2.Lerp(horizV, Vector2.zero, Time.deltaTime);
			body.velocity = new Vector3(horizV.x, body.velocity.y, horizV.y);
		}
	}

	void fall(){
		var xRot = Quaternion.AngleAxis(50 *.2f * angVel.y, Vector3.right);
		var yRot = Quaternion.AngleAxis(50 *.2f * angVel.x, Quaternion.Inverse(transform.rotation) * Vector3.up);

		transform.rotation *=  yRot;
	}

	void hover(){

		Vector2 dMousePos = mousePos - lastMousePos;
		lastMousePos = mousePos;


		var zRot = Quaternion.AngleAxis(-20 * dMousePos.x, Vector3.forward);
		var xRot = Quaternion.AngleAxis(20 * dMousePos.y, Quaternion.Inverse(transform.rotation) 
			* cameraController.instance.camera.transform.right);

		transform.rotation *= Quaternion.Slerp(Quaternion.identity, zRot * xRot, Time.deltaTime * 5);

		//slow y vel
		float newYV = Mathf.SmoothDamp(body.velocity.y, startHoverVel * .2f, ref slowVel, 10f);
		body.velocity = new Vector3(body.velocity.x, newYV, body.velocity.z);

		Vector3 forward = cameraController.instance.transform.forward;
		forward.y = 0;
		forward.Normalize ();
		Vector3 right = -Vector3.Cross(forward, Vector3.up);


		//float angVelMult = Mathf.Min(1, 1 / angVel.magnitude);
		Vector3 strafeVel =  (2 * mousePos.y * forward + 2 * mousePos.x * right) + new Vector3(0,body.velocity.y,0);

		body.velocity = Vector3.Lerp(body.velocity, strafeVel, Time.deltaTime);

	}

	public void launch(){
		//transform.position *= .98f;
		body.velocity = new Vector3(0, body.velocity.magnitude, 0);
	}

	public void resetRot(){
		transform.up = Vector3.up;
		leanAmt = 0;
	}

	IEnumerator hoverSlow(){

		for(int i = 0; i < numHoverSlowCycles; i++){
			body.AddForce(-Physics.gravity*body.mass);
			var v = body.velocity;
			v.y *= 1 - (numHoverSlowCycles - i) * .001f;


			yield return null;
		}
	}

	IEnumerator sLerpToVecCoroutine(Vector3 vec){
		Vector3 axis = Vector3.Cross(vec, transform.up);


		for(int i = 0; i < 50; i++){
			float angle = -Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(vec, transform.up));
			var quat = Quaternion.AngleAxis(angle, Quaternion.Inverse(transform.rotation) * axis);
			transform.rotation *= Quaternion.Slerp(Quaternion.identity, quat, Time.deltaTime * 5);

			yield return null;
		}
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
	/*
	void OnCollisionEnter(Collision collision){
		
		inAir = false;
		resetRot();
	}
	
	void OnCollisionExit(Collision collision){
		inAir = true;
	}
	*/
	bool IsGrounded() {
		RaycastHit hit;
		bool r = Physics.Raycast(transform.position, -Vector3.up, out hit, distToGround + 0.1f);
	
		//Debug.Log(hit.rigidbody.gameObject);
		return r;
	}

}




