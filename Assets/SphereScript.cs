using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SphereScript : MonoBehaviour {

	private List<GameObject> molecules;
	private Vector3 offset;
	private Vector3 screenPoint;
	private Vector3 lastMousePosition;
	private Vector3 mouseDelta;

	public float epsilon = .997f; //(kJ/mol)
	public float sigma = 3.4f; //Angstroms = ((1x10)^-10)m



	// Use this for initialization
	void Start () {

		Color moleculeColor = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), 1.0f);
		gameObject.renderer.material.color = moleculeColor;

		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		molecules = new List<GameObject>();	
		for(int i = 0; i < allMolecules.Length; i++){
			double distance = Vector3.Distance(transform.position, allMolecules[i].transform.position);
			if(allMolecules[i] != gameObject && distance < (2.5 * sigma)){
				molecules.Add(allMolecules[i]);
			}
		}
	}
	
	void FixedUpdate(){
		Vector3 finalForce = new Vector3 (0.0f, 0.0f, 0.0f);
		for (int i = 0; i < molecules.Count; i++) {
			Vector3 direction = new Vector3(transform.position.x - molecules[i].transform.position.x, transform.position.y - molecules[i].transform.position.y, transform.position.z - molecules[i].transform.position.z);
			direction.Normalize();
			
			double distance = Vector3.Distance(transform.position, molecules[i].transform.position);
			double part1 = ((48 * epsilon) / Math.Pow(distance, 2));
			//print ("part1: " + part1);
			double part2 = (Math.Pow ((epsilon / distance), 12) - (.5f * Math.Pow ((epsilon / distance), 6)));
			//print ("part2: " + part2);
			double magnitude = (part1 * part2 * distance);
			finalForce += (direction * (float)magnitude);
		}
		rigidbody.AddForce (finalForce);
	}

	void OnMouseDown (){
		rigidbody.isKinematic = true;

		screenPoint = Camera.main.WorldToScreenPoint(transform.position);
		offset = transform.position - Camera.main.ScreenToWorldPoint(
			new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
	}

	void OnMouseDrag()
	{
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;

		if (curPosition.y > 5) {
			curPosition.y = 5;
		}
		if (curPosition.y < -5) {
			curPosition.y = -5;
		}
		if (curPosition.x > 10) {
			curPosition.x = 10;
		}
		if (curPosition.x < -10) {
			curPosition.x = -10;
		}
		if (curPosition.z > 5) {
			curPosition.z = 5;
		}
		if (curPosition.z < -15) {
			curPosition.z = -15;
		}


		transform.position = curPosition;



		mouseDelta = Input.mousePosition - lastMousePosition;
		lastMousePosition = Input.mousePosition;
	}

	void OnMouseUp (){
		Quaternion cameraRotation = Camera.main.transform.rotation;
		rigidbody.isKinematic = false;
		//print ("Adding force: " + mouseDelta);
		rigidbody.AddForce (cameraRotation * mouseDelta * 50.0f);
	}

	void OnMouseOver(){
		if(Input.GetMouseButtonDown(1)){
			transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
			rigidbody.mass += 0.1f;
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
			rigidbody.mass -= 0.1f;
		}
	}

	void OnCollisionEnter (Collision col){
		float magnitude = 100.0f;
		if (col.gameObject.name == "BackPlane") {
			//print(gameObject.name + " hit the BackPlane");
			rigidbody.AddForce(-Vector3.forward * magnitude);
		}
		if (col.gameObject.name == "FrontPlane") {
			//print(gameObject.name + " hit the FrontPlane");
			rigidbody.AddForce(Vector3.forward * magnitude);
		}
		if (col.gameObject.name == "TopPlane") {
			//print(gameObject.name + " hit the TopPlane");
			rigidbody.AddForce(-Vector3.up * magnitude);
		}
		if (col.gameObject.name == "BottomPlane") {
			//print(gameObject.name + " hit the BottomPlane");
			rigidbody.AddForce(Vector3.up * magnitude);
		}
		if (col.gameObject.name == "RightPlane") {
			//print(gameObject.name + " hit the RightPlane");
			rigidbody.AddForce(-Vector3.right * magnitude);
		}
		if (col.gameObject.name == "LeftPlane") {
			//print(gameObject.name + " hit the LeftPlane");
			rigidbody.AddForce(Vector3.right * magnitude);
		}
	}
}
