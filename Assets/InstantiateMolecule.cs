using UnityEngine;
using System.Collections;

public class InstantiateMolecule : MonoBehaviour {

	public Rigidbody copperPrefab;
	public Rigidbody goldPrefab;
	public Rigidbody platinumPrefab;
	public float holdTime = 1.0f;
	private bool isClicked;
	private float startTime;
	private Vector3 curScreenPoint;
	private bool first;
	private Vector3 clickedPoint;

	private float copperButtonPosX;
	private float copperButtonPosY;
	private float goldButtonPosX;
	private float goldButtonPosY;
	private float platinumButtonPosX;
	private float platinumButtonPosY;

	
	void Start () {
		isClicked = false;
		copperButtonPosX = -1000.0f;
		copperButtonPosY = -1000.0f;
		goldButtonPosX = -1000.0f;
		goldButtonPosY = -1000.0f;
		platinumButtonPosX = -1000.0f;
		platinumButtonPosY = -1000.0f;
		first = true;
	}

	void FixedUpdate () {
		curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);

		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		bool holdingAtom = false;
		for (int i = 0; i < allMolecules.Length; i++) {
			Atom atomScript = allMolecules[i].GetComponent<Atom>();
			if(atomScript.held){
				holdingAtom = true;
				break;
			}
		}

		if (!holdingAtom) {
			if(!isClicked && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Stationary)
			//if(Input.GetMouseButtonDown(0) && !isClicked)
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if(!Physics.Raycast(ray,out hit) || hit.collider.gameObject.tag != "Molecule"){
					isClicked = true;
					startTime = Time.time;
				}
			}
			if (isClicked && Time.time - startTime > holdTime && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Stationary)
			//if (Input.GetMouseButton (0) && isClicked && Time.time - startTime > holdTime)
			{
				if(first){
					copperButtonPosX = curScreenPoint.x;
					copperButtonPosY = (Screen.height - curScreenPoint.y) - 60;
					goldButtonPosX = curScreenPoint.x;
					goldButtonPosY = (Screen.height - curScreenPoint.y) + 60;
					platinumButtonPosX = curScreenPoint.x - 120;
					platinumButtonPosY = (Screen.height - curScreenPoint.y);
					first = false;
					clickedPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);
				}
			}
			if(Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled){
				startTime = 0.0f;
				isClicked = false;
			}
		}
		else{
			startTime = 0.0f;
			isClicked = false;
		}
	}

	void OnGUI(){

		if(GUI.Button(new Rect(copperButtonPosX - 60,copperButtonPosY,120,20), "Add Copper")) {
			isClicked = false;
			startTime = 0.0f;
			copperButtonPosX = -1000.0f;
			copperButtonPosY = -1000.0f;
			goldButtonPosX = -1000.0f;
			goldButtonPosY = -1000.0f;
			platinumButtonPosX = -1000.0f;
			platinumButtonPosY = -1000.0f;
			first = true;
			
			Vector3 curPosition = Camera.main.ScreenToWorldPoint(clickedPoint);
			Quaternion curRotation = Quaternion.Euler(0, 0, 0);
			curPosition = CheckPosition(curPosition);
			Instantiate(copperPrefab, curPosition, curRotation);
		}

		if(GUI.Button(new Rect(goldButtonPosX - 60,goldButtonPosY,120,20), "Add Gold")) {
			isClicked = false;
			startTime = 0.0f;
			copperButtonPosX = -1000.0f;
			copperButtonPosY = -1000.0f;
			goldButtonPosX = -1000.0f;
			goldButtonPosY = -1000.0f;
			platinumButtonPosX = -1000.0f;
			platinumButtonPosY = -1000.0f;
			first = true;
			
			Vector3 curPosition = Camera.main.ScreenToWorldPoint(clickedPoint);
			Quaternion curRotation = Quaternion.Euler(0, 0, 0);
			curPosition = CheckPosition(curPosition);
			Instantiate(goldPrefab, curPosition, curRotation);
		}

		if(GUI.Button(new Rect(platinumButtonPosX - 60,platinumButtonPosY,120,20), "Add Platinum")) {
			isClicked = false;
			startTime = 0.0f;
			copperButtonPosX = -1000.0f;
			copperButtonPosY = -1000.0f;
			goldButtonPosX = -1000.0f;
			goldButtonPosY = -1000.0f;
			platinumButtonPosX = -1000.0f;
			platinumButtonPosY = -1000.0f;
			first = true;
			
			Vector3 curPosition = Camera.main.ScreenToWorldPoint(clickedPoint);
			Quaternion curRotation = Quaternion.Euler(0, 0, 0);
			curPosition = CheckPosition(curPosition);
			Instantiate(platinumPrefab, curPosition, curRotation);
		}

		if(GUI.Button(new Rect(platinumButtonPosX + 180,platinumButtonPosY,120,20), "Dismiss")) {
			isClicked = false;
			startTime = 0.0f;
			copperButtonPosX = -1000.0f;
			copperButtonPosY = -1000.0f;
			goldButtonPosX = -1000.0f;
			goldButtonPosY = -1000.0f;
			platinumButtonPosX = -1000.0f;
			platinumButtonPosY = -1000.0f;
			first = true;
		}
		
		
		if(GUI.Button(new Rect((Screen.width / 2) - 40,40,80,20), "Front")) {
			transform.position = new Vector3(0.0f, 0.0f, -26.0f);
			transform.rotation = Quaternion.Euler(0, 0, 0);
		}
		if(GUI.Button(new Rect((Screen.width / 2) + 50,40,80,20), "Left")) {
			transform.position = new Vector3(-30.7f, 0.0f, -1.5f);
			transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
		}
		if(GUI.Button(new Rect((Screen.width / 2) + 140,40,80,20), "Back")) {
			transform.position = new Vector3(0.0f, 0.0f, 36.7f);
			transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
		}
		if(GUI.Button(new Rect((Screen.width / 2) + 230,40,80,20), "Right")) {
			transform.position = new Vector3(33.5f, 0.0f, -1.3f);
			transform.rotation = Quaternion.Euler(0.0f, 270.0f, 0.0f);
		}
		if(GUI.Button(new Rect((Screen.width / 2) + 320,40,80,20), "Top")) {
			transform.position = new Vector3(0.0f, 40.0f, 0.0f);
			transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
		}
		if(GUI.Button(new Rect((Screen.width / 2) +410,40,80,20), "Bottom")) {
			transform.position = new Vector3(-1.5f, -37.5f, 0.0f);
			transform.rotation = Quaternion.Euler(270.0f, 0.0f, 0.0f);
		}
		
		
		GUI.Label(new Rect(25, 15, 200, 20), "Time Scale: " + Atom.timeScale);
		float timeScale = GUI.HorizontalSlider(new Rect(25, 55, 100, 30), Atom.timeScale, 0.0001f, 5.0f);
		if (timeScale != SphereScript.timeScale) {
			Atom.timeScale = timeScale;
			isClicked = false;
		}
		
		GUI.Label (new Rect (25, 95, 250, 20), "Temperature: " + Atom.desiredTemperature);
		float newTemp = GUI.HorizontalSlider (new Rect (25, 135, 100, 30), Atom.desiredTemperature, 0.0001f, 800.0f);
		if (newTemp != SphereScript.desiredTemperature) {
			Atom.desiredTemperature = newTemp;
			isClicked = false;
		}

		GUI.Label (new Rect (25, 175, 250, 20), "Time: " + Time.realtimeSinceStartup);
	}

	Vector3 CheckPosition(Vector3 curPosition){
		CameraScript cameraScript = Camera.main.GetComponent<CameraScript> ();
		if (curPosition.y > cameraScript.centerPos.y + (cameraScript.height/2.0f) - cameraScript.errorBuffer) {
			curPosition.y = cameraScript.centerPos.y + (cameraScript.height/2.0f) - cameraScript.errorBuffer;
		}
		if (curPosition.y < cameraScript.centerPos.y - (cameraScript.height/2.0f) + cameraScript.errorBuffer) {
			curPosition.y = cameraScript.centerPos.y - (cameraScript.height/2.0f) + cameraScript.errorBuffer;
		}
		if (curPosition.x > cameraScript.centerPos.x + (cameraScript.width/2.0f) - cameraScript.errorBuffer) {
			curPosition.x = cameraScript.centerPos.x + (cameraScript.width/2.0f) - cameraScript.errorBuffer;
		}
		if (curPosition.x < cameraScript.centerPos.x - (cameraScript.width/2.0f) + cameraScript.errorBuffer) {
			curPosition.x = cameraScript.centerPos.x - (cameraScript.width/2.0f) + cameraScript.errorBuffer;
		}
		if (curPosition.z > cameraScript.centerPos.z + (cameraScript.depth/2.0f) - cameraScript.errorBuffer) {
			curPosition.z = cameraScript.centerPos.z + (cameraScript.depth/2.0f) - cameraScript.errorBuffer;
		}
		if (curPosition.z < cameraScript.centerPos.z - (cameraScript.depth/2.0f) + cameraScript.errorBuffer) {
			curPosition.z = cameraScript.centerPos.z - (cameraScript.depth/2.0f) + cameraScript.errorBuffer;
		}
		return curPosition;
	}

}
