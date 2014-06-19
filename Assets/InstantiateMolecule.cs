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

	}
}
