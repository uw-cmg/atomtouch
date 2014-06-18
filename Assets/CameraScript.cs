using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	public float moveSpeed = 0.25f;
	public float turnSpeed = .5f;

	public Rigidbody moleculePrefab;
	public int numMolecules = 100;

	// Use this for initialization
	void Start () {

		for (int i = 0; i < numMolecules; i++) {
			Vector3 position = new Vector3(Random.Range(-9.5F, 9.5F), Random.Range(-4.5F, 4.5F), Random.Range(-14.5F, 4.5F));
			Quaternion rotation = Quaternion.Euler(0, 0, 0);
			Instantiate(moleculePrefab, position, rotation);
		}

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey (KeyCode.W)) {
			transform.Translate(Vector3.forward * moveSpeed);
		}
		if (Input.GetKey (KeyCode.S)) {
			transform.Translate(-Vector3.forward * moveSpeed);
		}
		if (Input.GetKey (KeyCode.D)) {
			transform.Translate(Vector3.right * moveSpeed);
		}
		
		if (Input.GetKey (KeyCode.A)) {
			transform.Translate(-Vector3.right * moveSpeed);
		}

		if (Input.GetKey (KeyCode.J)) {
			transform.Rotate(Vector3.up, -turnSpeed);
		}
		if (Input.GetKey (KeyCode.K)) {
			transform.Rotate(Vector3.up, turnSpeed);
		}
		if (Input.GetKey (KeyCode.N)) {
			transform.Rotate(Vector3.right, turnSpeed);
		}
		if (Input.GetKey (KeyCode.M)) {
			transform.Rotate(Vector3.right, -turnSpeed);
		}
	}
}
