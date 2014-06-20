using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	public float moveSpeed = 0.25f;
	public float turnSpeed = .5f;

	public Rigidbody moleculePrefab;
	public int numMolecules = 100;
	public float hSliderValue = 0.0F;

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

		if (Input.GetKey (KeyCode.P)) {
			print ("position " + transform.position + " rotation: "  + transform.eulerAngles);
		}

	}

	void OnGUI() {
		if(GUI.Button(new Rect((Screen.width / 2) - 40,40,80,20), "Camera 1")) {
			transform.position = new Vector3(0.0f, 0.0f, -26.0f);
			transform.rotation = Quaternion.Euler(0, 0, 0);
		}
		if(GUI.Button(new Rect((Screen.width / 2) + 50,40,80,20), "Camera 2")) {
			transform.position = new Vector3(-26.0f, 0.0f, -7.0f);
			transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
		}
		if(GUI.Button(new Rect((Screen.width / 2) + 140,40,80,20), "Camera 3")) {
			transform.position = new Vector3(-14.4f, 0.0f, 15.9f);
			transform.rotation = Quaternion.Euler(0.0f, 148.2f, 0.0f);
		}
		if(GUI.Button(new Rect((Screen.width / 2) + 230,40,80,20), "Camera 4")) {
			transform.position = new Vector3(15.6f, 0.0f, 12.8f);
			transform.rotation = Quaternion.Euler(0.0f, 225.0f, 0.0f);
		}
		hSliderValue = GUI.HorizontalSlider(new Rect(25, 25, 100, 30), hSliderValue, 0.0F, 10.0F);
		print (hSliderValue);
	}

}
