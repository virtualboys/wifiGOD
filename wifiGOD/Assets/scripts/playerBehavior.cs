using UnityEngine;
using System.Collections;

public class playerBehavior : MonoBehaviour {

	public static playerBehavior instance;

	public int numHoverSlowCycles = 50;

	public Rigidbody body;

	public bool inPool;
	public bool onPlate;
	public bool inAir { get { return !inPool && !onPlate; } }

	public bool useG;

	float leanAmt;
	public Vector2 angVel;
	Vector2 airRot;

	Quaternion worldRot;

	float startHoverVel;
	float slowVel;

	Vector2 mouseDrag;
	Vector2 lastMousePos;

	void Awake () {
		Screen.showCursor = false;

		instance = this;

		body = GetComponent ("Rigidbody") as Rigidbody;
		body.freezeRotation = true;
		body.useGravity = useG;
	}

	void Start () {
		worldRot = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		mouseInput ();

		if(inAir)
			updateAir();
		else
			updateGround();

	}
	
	void updateGround(){
		
		setRotGround ();

		Quaternion dYRot = Quaternion.identity;
		Quaternion dZRot = Quaternion.identity;

		float horizAxis = mouseDrag.x/20;

		//how fast to turn based on speed
		float turnAmt = .05f * body.velocity.magnitude;
		turnAmt = Mathf.Clamp(turnAmt, 1, 4);

		dZRot = Quaternion.AngleAxis(horizAxis * 70,  Vector3.forward);
		dYRot = Quaternion.AngleAxis(-turnAmt * horizAxis, 
		(worldRot) * Vector3.up);

		worldRot *= dYRot;
		transform.rotation = worldRot * dZRot;

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

		float horizInput = Input.GetAxis("Mouse X");
		float vertInput = Input.GetAxis("Mouse Y");

		angVel = Vector2.Lerp(angVel, new Vector2(horizInput, vertInput), Time.deltaTime * 5);

		if(Input.GetMouseButton(0)){
			//start hover reset tilt on new press
			if(Input.GetMouseButtonDown(0)){
				StartCoroutine(sLerpToVecCoroutine(Vector3.up));
				startHoverVel = body.velocity.y;
			}

			hover ();
		}
		else{
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

		Vector2 dMousePos = -(mouseDrag - lastMousePos);

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
		Vector3 strafeVel =  -(2 * mouseDrag.y * forward + 2 * mouseDrag.x * right) + new Vector3(0,body.velocity.y,0);

		body.velocity = Vector3.Lerp(body.velocity, strafeVel, Time.deltaTime);

	}

	public void setRotGround(){
		var normal = getSurfaceNormal ();
		sLerpToVec(normal);
	}

	public void launch(){
		//transform.position *= .98f;
		body.velocity = new Vector3(0, body.velocity.magnitude, 0);
	}

	/// <summary>
	/// todo
	/// </summary>
	public void land(){
		resetRot ();

	}

	public void resetRot(){
		transform.up = getSurfaceNormal();
		worldRot = transform.rotation;
		leanAmt = 0;
	}

	public Vector3 getSurfaceNormal(){
		RaycastHit hit = new RaycastHit();
		Vector3 castPos = new Vector3 (transform.position.x, transform.position.y - .25f, transform.position.z);
		if (Physics.Raycast (castPos, -transform.up, out hit)) {
			return hit.normal;
		}

		return Vector3.zero;
	}

	public void mouseInput(){
		float horizInput = Input.GetAxis("Mouse X");
		float vertInput = Input.GetAxis("Mouse Y");

		if (Input.GetMouseButton (0)) {
			lastMousePos = mouseDrag;
			//update and clamp mouse pos
			mouseDrag -= new Vector2(horizInput, vertInput);
			if( Mathf.Abs(mouseDrag.x) > 30)
				mouseDrag.x = Mathf.Sign(mouseDrag.x) * 30;
			if( Mathf.Abs(mouseDrag.y) > 30)
				mouseDrag.y = Mathf.Sign (mouseDrag.y) * 30;

		} else if(Input.GetMouseButtonUp(0)){
			//end hover reset tilt
			StartCoroutine(sLerpToVecCoroutine(Vector3.up));
			mouseDrag = Vector2.zero;

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

		float angle = -Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(vec, worldRot * Vector3.up));
		Vector3 axis = Vector3.Cross(vec, worldRot * Vector3.up);

		var quat = Quaternion.AngleAxis(angle, Quaternion.Inverse(worldRot) * axis);
		worldRot *= Quaternion.Slerp(Quaternion.identity, quat, Time.deltaTime * 5);
	}

	float subAngles(float angle1, float angle2){
		float val = angle1 - angle2;
		if (Mathf.Abs (val) > 180)
			return - Mathf.Sign (val) * (360 - Mathf.Abs (val));

		return (angle1 - angle2);
	}

}
