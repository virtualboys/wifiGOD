using UnityEngine;
using System.Collections;

public class cloudBehavior : MonoBehaviour {

	Vector3 velocity;

	// Use this for initialization
	void Start () {
		transform.position = cloudSpawner.instance.spawnLoc + new Vector3 (500 * (Random.value - .5f), 
		              		200 * (Random.value), 500 * (Random.value - .5f));

		float scale = 75 + 50 * (transform.position.y - cloudSpawner.instance.spawnLoc.y);
		transform.localScale = new Vector3 (scale, scale, scale);

		transform.rotation = Quaternion.Euler (new Vector3 (0, Random.value * Mathf.PI * 2, 0));

		velocity = .01f * new Vector3 (Random.value - .5f, 0, Random.value - .5f);
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += velocity;
	}
}
