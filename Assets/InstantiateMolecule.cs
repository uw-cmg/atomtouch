using UnityEngine;
using System.Collections;

public class InstantiateMolecule : MonoBehaviour {

	public Rigidbody moleculePrefab;
	public float holdTime = 1.0f;
	private bool isClicked;
	private float startTime;
	private Vector3 curScreenPoint;
	private float buttonPosX;
	private float buttonPosY;
	private bool first;
	private Vector3 clickedPoint;


	// Use this for initialization
	void Start () {
		isClicked = false;
		buttonPosX = -1000.0f;
		buttonPosY = -1000.0f;
		first = true;
	}
	
	// Update is called once per frame
	void Update () {
		curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);

		//if(Input.GetMouseButtonDown(0) && !isClicked && Input.touchCount == 1)
		if(Input.GetMouseButtonDown(0) && !isClicked)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(!Physics.Raycast(ray,out hit) || hit.collider.gameObject.tag != "Molecule"){
				isClicked = true;
				startTime = Time.time;
			}
		}
		//if (Input.GetMouseButton (0) && isClicked && Time.time - startTime > holdTime && Input.touchCount == 1)
		if (Input.GetMouseButton (0) && isClicked && Time.time - startTime > holdTime) {
			if(first){
				buttonPosX = curScreenPoint.x;
				buttonPosY = (Screen.height - curScreenPoint.y) - 60;
				first = false;
				clickedPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);
			}
		}

		if (Input.GetMouseButtonUp (0)) {
			startTime = 0.0f;
			isClicked = false;
		}

	}

	void OnGUI(){

		if(GUI.Button(new Rect(buttonPosX - 60,buttonPosY,120,20), "Add Argon")) {
			isClicked = false;
			startTime = 0.0f;
			buttonPosX = -1000.0f;
			buttonPosY = -1000.0f;
			first = true;
			
			Vector3 curPosition = Camera.main.ScreenToWorldPoint(clickedPoint);
			Quaternion curRotation = Quaternion.Euler(0, 0, 0);

			Instantiate(moleculePrefab, curPosition, curRotation);
		}


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


		GUI.Label(new Rect(25, 15, 200, 20), "Time Scale: " + SphereScript.timeScale);
		float timeScale = GUI.HorizontalSlider(new Rect(25, 40, 100, 30), SphereScript.timeScale, 0.0F, 5.0F);
		if (timeScale != SphereScript.timeScale) {
			SphereScript.timeScale = timeScale;
			isClicked = false;
		}

		GUI.Label (new Rect (25, 55, 250, 20), "Temperature: " + SphereScript.desiredTemperature);
		float newTemp = GUI.HorizontalSlider (new Rect (25, 75, 100, 30), SphereScript.desiredTemperature, 0.0f, 2000.0f);
		if (newTemp != SphereScript.desiredTemperature) {
			SphereScript.desiredTemperature = newTemp;
			isClicked = false;
		}
	}
}
