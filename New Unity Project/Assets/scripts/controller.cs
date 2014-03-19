using UnityEngine;
using System.Collections;

public class player : MonoBehaviour {

	// Use this for initialization
	public static CharacterController characterController;
	public static player instance;
	public CameraController camera;
	void Start () {

		characterController = GetComponent ("CharacterController") as CharacterController;
		camera = GetComponent ("Camera") as Camera;
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {

		C
	}

}
