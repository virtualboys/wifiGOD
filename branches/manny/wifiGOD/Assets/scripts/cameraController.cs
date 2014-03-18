using UnityEngine;
using System.Collections;

public class cameraController : MonoBehaviour {

	// Use this for initialization
	public Camera cam;
	public player player;
	void Start () {
		cam = GetComponent ("Camera") as Camera;

	}

	// Update is called once per frame
	public void Update() {
		cam.transform.position = player.transform.position -  8*player.transform.forward + (new Vector3(0,4,0));
		cam.transform.LookAt (player.transform.position);
	}
}
