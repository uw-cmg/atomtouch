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
	public Vector3 centerPos = new Vector3(0.0f, 0.0f, 0.0f);
	public float errorBuffer = 0.5f;
	private Vector2 touchPrevPos;

	void Start () {

		float width = 10.0f;
		float height = 10.0f;
		float depth = 10.0f;

		//create the atoms
		for (int i = 0; i < numMolecules; i++) {
			Vector3 position = new Vector3(centerPos.x + (Random.Range(-(width/2.0f) + errorBuffer, (width/2.0f) - errorBuffer)), centerPos.y + (Random.Range(-(height/2.0f) + errorBuffer, (height/2.0f) - errorBuffer)), centerPos.z + (Random.Range(-(depth/2.0f) + errorBuffer, (depth/2.0f) - errorBuffer)));
			Quaternion rotation = Quaternion.Euler(0, 0, 0);
			Instantiate(molecules[moleculeToSpawn].rigidbody, position, rotation);
		}

//		//create the box
//		Quaternion bottonPlaneRotation = Quaternion.Euler (0.0f, 0.0f, 0.0f);
//		Vector3 bottomPlanePos = new Vector3 (centerPos.x, centerPos.y - (height/2.0f), centerPos.z);
//		GameObject bottomPlane = Instantiate (plane, bottomPlanePos, bottonPlaneRotation) as GameObject;
//		bottomPlane.transform.localScale = new Vector3 (width / 10.0f, height / 10.0f, depth / 10.0f);
//		bottomPlane.name = "BottomPlane";
//		bottomPlane.tag = "Plane";
//
//		Quaternion topPlaneRotation = Quaternion.Euler (0.0f, 180.0f, 180.0f);
//		Vector3 topPlanePos = new Vector3 (centerPos.x, centerPos.y + (height/2.0f), centerPos.z);
//		GameObject topPlane = Instantiate (plane, topPlanePos, topPlaneRotation) as GameObject;
//		topPlane.transform.localScale = new Vector3 (width / 10.0f, height / 10.0f, depth / 10.0f);
//		topPlane.name = "TopPlane";
//		topPlane.tag = "Plane";
//
//		Quaternion backPlaneRotation = Quaternion.Euler (270.0f, 0.0f, 0.0f);
//		Vector3 backPlanePos = new Vector3 (centerPos.x, centerPos.y, centerPos.z + (depth/2.0f));
//		GameObject backPlane = Instantiate (plane, backPlanePos, backPlaneRotation) as GameObject;
//		backPlane.transform.localScale = new Vector3 (width / 10.0f, depth / 10.0f, height / 10.0f);
//		backPlane.name = "BackPlane";
//		backPlane.tag = "Plane";
//
//		Quaternion frontPlaneRotation = Quaternion.Euler (90.0f, 0.0f, 0.0f);
//		Vector3 frontPlanePos = new Vector3 (centerPos.x, centerPos.y, centerPos.z - (depth/2.0f));
//		GameObject frontPlane = Instantiate (plane, frontPlanePos, frontPlaneRotation) as GameObject;
//		frontPlane.transform.localScale = new Vector3 (width / 10.0f, depth / 10.0f, height / 10.0f);
//		frontPlane.name = "FrontPlane";
//		frontPlane.tag = "Plane";
//
//		Quaternion rightPlaneRotation = Quaternion.Euler (0.0f, 0.0f, 90.0f);
//		Vector3 rightPlanePos = new Vector3 (centerPos.x + (width/2.0f), centerPos.y, centerPos.z);
//		GameObject rightPlane = Instantiate (plane, rightPlanePos, rightPlaneRotation) as GameObject;
//		rightPlane.transform.localScale = new Vector3 (height / 10.0f, width / 10.0f, depth / 10.0f);
//		rightPlane.name = "RightPlane";
//		rightPlane.tag = "Plane";
//
//		Quaternion leftPlaneRotation = Quaternion.Euler (0.0f, 0.0f, 270.0f);
//		Vector3 leftPlanePos = new Vector3 (centerPos.x - (width/2.0f), centerPos.y, centerPos.z);
//		GameObject leftPlane = Instantiate (plane, leftPlanePos, leftPlaneRotation) as GameObject;
//		leftPlane.transform.localScale = new Vector3 (height / 10.0f, width / 10.0f, depth / 10.0f);
//		leftPlane.name = "LeftPlane";
//		leftPlane.tag = "Plane";
	}

	void Update () {

		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			if(Input.touchCount == 1){
				Touch touch = Input.GetTouch (0);
				if (touch.phase == TouchPhase.Moved) {
					GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
					bool holdingAtom = false;
					for (int i = 0; i < allMolecules.Length; i++) {
						Atom atomScript = allMolecules[i].GetComponent<Atom>();
						if(atomScript.held){
							holdingAtom = true;
							break;
						}
					}
					
					InstantiateMolecule instan = Camera.main.GetComponent<InstantiateMolecule>();
					
					if(!holdingAtom && !instan.addGraphicCopper && !instan.addGraphicGold && !instan.addGraphicPlatinum){
						Vector2 touchPrevPos = touch.position - touch.deltaPosition;
						float deltaMagnitudeDiffX = touch.position.x - touchPrevPos.x;
						float deltaTouchX = deltaMagnitudeDiffX / 10.0f;
						Camera.main.transform.RotateAround(centerPos, Vector3.up, deltaTouchX);

						//for rotation over the x-axis
//						float deltaMagnitudeDiffY = touch.position.y - touchPrevPos.y;
//						float deltaTouchY = deltaMagnitudeDiffY / 10.0f;
//						Camera.main.transform.RotateAround(centerPos, Vector3.right, deltaTouchY);
					}
				}
			}
		}
		else{
			if(Input.GetMouseButton(0)){
				GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
				bool holdingAtom = false;
				for (int i = 0; i < allMolecules.Length; i++) {
					Atom atomScript = allMolecules[i].GetComponent<Atom>();
					if(atomScript.held){
						holdingAtom = true;
						break;
					}
				}
				
				InstantiateMolecule instan = Camera.main.GetComponent<InstantiateMolecule>();
				if(!holdingAtom && !instan.addGraphicCopper && !instan.addGraphicGold && !instan.addGraphicPlatinum){
					float deltaMagnitudeDiffX = Input.mousePosition.x - touchPrevPos.x;
					float deltaTouchX = deltaMagnitudeDiffX / 10.0f;
					Camera.main.transform.RotateAround(centerPos, Vector3.up, deltaTouchX);

					//for rotation over the x-axis
//					float deltaMagnitudeDiffY = Input.mousePosition.y - touchPrevPos.y;
//					float deltaTouchY = deltaMagnitudeDiffY / 10.0f;
//					Camera.main.transform.RotateAround(centerPos, Vector3.right, deltaTouchY);
				}
			}
			touchPrevPos = Input.mousePosition;
		}
		
	}

	public void setCameraCoordinates(Transform objTransform){
		//transform.position = new Vector3 (objTransform.position.x, objTransform.position.y, objTransform.position.z - 10.0f);
		centerPos = objTransform.position;
		transform.LookAt (objTransform);
	}

}
