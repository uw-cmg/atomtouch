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

	//private float kB = 1.381 * Math.Pow(10,-23); // J/K
	//private float kB = 0.00008617f; // eV/K

	//Argon
	//public float epsilon = .997f; //(kJ/mol)
	//public float sigma = 3.4f; //Angstroms = ((1x10)^-10)m

	//L-J potentials from Zhen and Davies, Phys. Stat. Sol. a, 78, 595 (1983)
	//Symbol, epsilon/k_Boltzmann (K) n-m version, 12-6 version, sigma (Angstroms),
	//     mass in amu, mass in (20 amu) for Unity 
	//     FCC lattice parameter in Angstroms, expected NN bond (Angs)
	//Au: 4683.0, 5152.9, 2.6367, 196.967, 9.848, 4.080, 2.88
	//Cu: 3401.1, 4733.5, 2.3374,  63.546, 3.177, 3.610, 2.55
	//Pt: 7184.2, 7908.7, 2.5394, 165.084, 8.254, 3.920, 2.77

	private float epsilon = 5152.9f * 0.00008617f; // eV
	private float sigma = 2.6367f; // Angstroms
	public float massamu = 196.967f; //amu
	
	//copper
	//public float epsilon = (6.537 * Math.Pow(10, -20)); //J
	//public float sigmaAng = 2.3374f; //Angstroms

	//platinum
	//public float epsilon = (1.0922 * Math.Pow(10, -19)); //J
	//public float sigmaAng = 2.5394f; //Angstroms



	// Use this for initialization
	void Start () {

		//Color moleculeColor = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), 1.0f);
		//gameObject.renderer.material.color = moleculeColor;

	}
	
	void FixedUpdate(){

		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		molecules = new List<GameObject>();	
		for(int i = 0; i < allMolecules.Length; i++){
			double distance = Vector3.Distance(transform.position, allMolecules[i].transform.position);
			if(allMolecules[i] != gameObject && distance < (2.5 * sigma)){
				molecules.Add(allMolecules[i]);
			}
		}

		Vector3 finalForce = new Vector3 (0.000f, 0.000f, 0.000f); //TTM increase precision
		for (int i = 0; i < molecules.Count; i++) {
			//Vector3 vect = molecules[i].transform.position - transform.position;
			Vector3 direction = new Vector3(molecules[i].transform.position.x - transform.position.x, molecules[i].transform.position.y - transform.position.y, molecules[i].transform.position.z - transform.position.z);
			direction.Normalize();
			
			double distance = Vector3.Distance(transform.position, molecules[i].transform.position);
			//print ("distance: " + distance);
			double part1 = ((-48 * epsilon) / Math.Pow(distance, 2));
			//print ("part1: " + part1);
			double part2 = (Math.Pow ((sigma / distance), 12) - (.5f * Math.Pow ((sigma / distance), 6)));
			//print ("part2: " + part2);
			double magnitude = (part1 * part2 * distance);
			finalForce += (direction * (float)magnitude);
		}
		//finalForce is in units of eV/Angstroms, or 1.602*10^-9 N
		//mass is entered into Unity3D in units of 20 amu, so the mass of Au
		//    should be given as 9.84 (*20 amu)
		//force = mass * acceleration = eV/A = 1.602*10^-19 J/Angstrom
		//force = 1.602*10^-19 kg m2/s2/ Angstrom
		//      = 1.602 * 10^-19 kg * 10^10 Angs * 10^10 Angs / Angs / s2
		//      = 1.602 * 10^-1 kg * Angstroms/s^2
		//however, the time should be in something like femtosecond steps (10^-15)
		//force = 1.602 * 10^-1 kg * Angstroms / 10^15 fs * 10^15 fs
		//force = 1.602 * 10^-31 kg * Angstroms/ femtoseconds^2
		//acceleration is in Angstroms/(fs)^2
		//mass is in 1.602 * 10^-31 kg
		//one amu = 1.6605 * 10^-27 kg
		//therefore, mass is in (1.602*10^-31/1.6605*10^-27) amu = 9.6477*10^-5 amu
		//if we give Unity mass in amu/20
		//force = unitymass * unityacceleration
		//force = realmass/20 * 20*realacceleration
		//force = force*20*9.6477*10^-5
		//if we give Unity mass in amu/100, then 100*
		//finalForce = finalForce * 100.0f * 0.000096477f; //converting mass
		//However, Unity seems to be able to adjust the timestep - so we do not
		//know what the time unit is, and therefore what the mass and divisors 
		//should be
		finalForce = finalForce / 20.0f; 
		print ("finalForce: " + finalForce);
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
