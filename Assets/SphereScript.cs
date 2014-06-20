using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SphereScript : MonoBehaviour {

	private List<GameObject> positiveForceMolecules;
	private List<GameObject> molecules;
	private Vector3 offset;
	private Vector3 screenPoint;
	private Vector3 lastMousePosition;
	private Vector3 mouseDelta;
	public static float timeScale = 1.0f;

	public static float desiredTemperature = 100.0f;

	//private double sigma; //meters

	private double kB = 1.381 * Math.Pow (10, -23); //(J / K)

	//test
	public float epsilon = .997f;
	public float sigma = 5.997f;

	//Argon
	//public float epsilon = .997f; //(kJ/mol)
	//public float sigma = 3.4f; //Angstroms = ((1x10)^-10)m

	//gold
	//public double epsilon = (double)(7.1162 * Math.Pow (10, -20)); //J
	//public float sigmaAng = 2.6367f; //Angstroms
	//mass = 3.27 in units of 10^-25 kg per atom = 197 amu

	//copper
	//public float epsilon = (6.537 * Math.Pow(10, -20)); //J
	//public float sigmaAng = 2.3374f; //Angstroms

	//platinum
	//public float epsilon = (1.0922 * Math.Pow(10, -19)); //J
	//public float sigmaAng = 2.5394f; //Angstroms



	// Use this for initialization
	void Start () {

		Color moleculeColor = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), 1.0f);
		gameObject.renderer.material.color = moleculeColor;

//		LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer> ();
//		lineRenderer.material = new Material (Shader.Find ("Particles/Additive"));
//		lineRenderer.SetColors (Color.yellow, Color.red);
//		lineRenderer.SetWidth (0.2f, 0.2f);
	}
	
	void FixedUpdate(){
		Time.timeScale = timeScale;

		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		molecules = new List<GameObject>();	
		float totalEnergy = 0.0f;
		for(int i = 0; i < allMolecules.Length; i++){
			double distance = Vector3.Distance(transform.position, allMolecules[i].transform.position);
			if(allMolecules[i] != gameObject && distance < (2.5 * sigma)){
				molecules.Add(allMolecules[i]);
			}

			GameObject molecule = allMolecules[i];
			if(molecule.rigidbody && !molecule.rigidbody.isKinematic){
				double velocity = Math.Pow((molecule.rigidbody.velocity.magnitude * 100), 2);
				//mass is hardcoded
				totalEnergy += (float)(.5f * (3.27 * Math.Pow (10, -25)) * velocity);
				//print ("velocity: " + velocity);
			}
		}

		positiveForceMolecules = new List<GameObject> ();
		Vector3 finalForce = new Vector3 (0.0f, 0.0f, 0.0f);
		for (int i = 0; i < molecules.Count; i++) {
			//Vector3 vect = molecules[i].transform.position - transform.position;
			Vector3 direction = new Vector3(molecules[i].transform.position.x - transform.position.x, molecules[i].transform.position.y - transform.position.y, molecules[i].transform.position.z - transform.position.z);
			direction.Normalize();
			
			double distance = Vector3.Distance(transform.position, molecules[i].transform.position);
			double part1 = ((-48 * epsilon) / Math.Pow(distance, 2));
			//print ("part1: " + part1);
			double part2 = (Math.Pow ((sigma / distance), 12) - (.5f * Math.Pow ((sigma / distance), 6)));
			//print ("part2: " + part2);
			double magnitude = (part1 * part2 * distance);
			if(magnitude < 0){
				positiveForceMolecules.Add(molecules[i]);
			}
			finalForce += (direction * (float)magnitude);
		}
		rigidbody.AddForce (finalForce);


//		LineRenderer lineRenderer = GetComponent<LineRenderer> ();
//		for (int i = 0; i < positiveForceMolecules.Count; i++) {
//			lineRenderer.SetPosition (0, new Vector3 (transform.position.x, transform.position.y, transform.position.z));
//			lineRenderer.SetPosition (1, new Vector3 (positiveForceMolecules[i].transform.position.x, positiveForceMolecules[i].transform.position.y, positiveForceMolecules[i].transform.position.z));
//		}
		
		
		double instantTemp = totalEnergy / (3.0f / 2.0f) / allMolecules.Length / kB;
		//print ("Instant Temp: " + instantTemp);
		
		double alpha = desiredTemperature / instantTemp;
		Vector3 newVelocity = gameObject.rigidbody.velocity * (float)Math.Pow (alpha, .5f);
		//print ("alpha: " + alpha);
		//print ("Velocity: " + newVelocity);

		if (!rigidbody.isKinematic && !float.IsInfinity((float)alpha)) {
			rigidbody.velocity = newVelocity;
		}




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
