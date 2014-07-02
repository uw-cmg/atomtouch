using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraScript : MonoBehaviour {

	public float moveSpeed = 0.25f;
	public float turnSpeed = .5f;
	public int numMolecules = 10;
	public List<Rigidbody> molecules = new List<Rigidbody>();
	public int moleculeToSpawn = 0;

	public GameObject plane;
	public int width = 10;
	public int height = 10;
	public int depth = 10;
	public Vector3 centerPos = new Vector3(0.0f, 0.0f, 0.0f);
	public float errorBuffer = 0.5f;
	
	void Start () {

		//create the atoms
		for (int i = 0; i < numMolecules; i++) {
			Vector3 position = new Vector3(centerPos.x + (Random.Range(-(width/2.0f) + errorBuffer, (width/2.0f) - errorBuffer)), centerPos.y + (Random.Range(-(height/2.0f) + errorBuffer, (height/2.0f) - errorBuffer)), centerPos.z + (Random.Range(-(depth/2.0f) + errorBuffer, (depth/2.0f) - errorBuffer)));
			Quaternion rotation = Quaternion.Euler(0, 0, 0);
			Instantiate(molecules[moleculeToSpawn].rigidbody, position, rotation);
		}

		//create the box
		Quaternion bottonPlaneRotation = Quaternion.Euler (0.0f, 0.0f, 0.0f);
		Vector3 bottomPlanePos = new Vector3 (centerPos.x, centerPos.y - (height/2.0f), centerPos.z);
		GameObject bottomPlane = Instantiate (plane, bottomPlanePos, bottonPlaneRotation) as GameObject;
		bottomPlane.transform.localScale = new Vector3 (width / 10.0f, height / 10.0f, depth / 10.0f);
		bottomPlane.name = "BottomPlane";
		bottomPlane.tag = "Plane";

		Quaternion topPlaneRotation = Quaternion.Euler (0.0f, 180.0f, 180.0f);
		Vector3 topPlanePos = new Vector3 (centerPos.x, centerPos.y + (height/2.0f), centerPos.z);
		GameObject topPlane = Instantiate (plane, topPlanePos, topPlaneRotation) as GameObject;
		topPlane.transform.localScale = new Vector3 (width / 10.0f, height / 10.0f, depth / 10.0f);
		topPlane.name = "TopPlane";
		topPlane.tag = "Plane";

		Quaternion backPlaneRotation = Quaternion.Euler (270.0f, 0.0f, 0.0f);
		Vector3 backPlanePos = new Vector3 (centerPos.x, centerPos.y, centerPos.z + (depth/2.0f));
		GameObject backPlane = Instantiate (plane, backPlanePos, backPlaneRotation) as GameObject;
		backPlane.transform.localScale = new Vector3 (width / 10.0f, depth / 10.0f, height / 10.0f);
		backPlane.name = "BackPlane";
		backPlane.tag = "Plane";

		Quaternion frontPlaneRotation = Quaternion.Euler (90.0f, 0.0f, 0.0f);
		Vector3 frontPlanePos = new Vector3 (centerPos.x, centerPos.y, centerPos.z - (depth/2.0f));
		GameObject frontPlane = Instantiate (plane, frontPlanePos, frontPlaneRotation) as GameObject;
		frontPlane.transform.localScale = new Vector3 (width / 10.0f, depth / 10.0f, height / 10.0f);
		frontPlane.name = "FrontPlane";
		frontPlane.tag = "Plane";

		Quaternion rightPlaneRotation = Quaternion.Euler (0.0f, 0.0f, 90.0f);
		Vector3 rightPlanePos = new Vector3 (centerPos.x + (width/2.0f), centerPos.y, centerPos.z);
		GameObject rightPlane = Instantiate (plane, rightPlanePos, rightPlaneRotation) as GameObject;
		rightPlane.transform.localScale = new Vector3 (height / 10.0f, width / 10.0f, depth / 10.0f);
		rightPlane.name = "RightPlane";
		rightPlane.tag = "Plane";

		Quaternion leftPlaneRotation = Quaternion.Euler (0.0f, 0.0f, 270.0f);
		Vector3 leftPlanePos = new Vector3 (centerPos.x - (width/2.0f), centerPos.y, centerPos.z);
		GameObject leftPlane = Instantiate (plane, leftPlanePos, leftPlaneRotation) as GameObject;
		leftPlane.transform.localScale = new Vector3 (height / 10.0f, width / 10.0f, depth / 10.0f);
		leftPlane.name = "LeftPlane";
		leftPlane.tag = "Plane";
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
		if (Input.GetKey (KeyCode.F)) {
			transform.Translate(Vector3.up * moveSpeed);
		}
		if (Input.GetKey (KeyCode.V)) {
			transform.Translate(-Vector3.up * moveSpeed);
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

		if (Input.GetKey (KeyCode.P)) {
			print ("position " + transform.position + " rotation: "  + transform.eulerAngles);
		}

	}

}
