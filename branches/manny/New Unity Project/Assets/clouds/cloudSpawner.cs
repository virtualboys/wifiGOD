using UnityEngine;
using System.Collections;

public class cloudSpawner : MonoBehaviour {

	public static cloudSpawner instance;

	public int numClouds;
	public Vector3 spawnLoc;
	public GameObject c1Prefab;
	public GameObject c2Prefab;
	public GameObject c3Prefab;

	// Use this for initialization
	void Awake () {

		instance = this;

		for (int i = 0; i < numClouds; i++) {
			int r = (int)(Random.value * 3);

			switch(r){
			case 0:
				Instantiate(c1Prefab);
				break;
			case 1:
				Instantiate(c2Prefab);
				break;
			case 2:
				Instantiate(c3Prefab);
				break;
			}
		}
	}
}
