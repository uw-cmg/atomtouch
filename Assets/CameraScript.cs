using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CameraScript : MonoBehaviour {

	public float moveSpeed = 0.25f;
	public float turnSpeed = .5f;
	
	private Vector2 touchPrevPos;
	private bool rotateAroundY = false;
	private bool first = true;
	

	void Update () {

		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment> ();

		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			if(Input.touchCount == 1){
				Touch touch = Input.GetTouch (0);
				if(touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended){
					first = true;
				}
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

					if(!holdingAtom && !instan.addGraphicCopper && !instan.addGraphicGold && !instan.addGraphicPlatinum && !instan.changingTemp){
						Quaternion cameraRotation = Camera.main.transform.rotation;
						Vector2 touchPrevPos = touch.position - touch.deltaPosition;
						float deltaMagnitudeDiffX = touch.position.x - touchPrevPos.x;
						float deltaTouchX = deltaMagnitudeDiffX / 10.0f;

						//for rotation over the x-axis
						float deltaMagnitudeDiffY = touch.position.y - touchPrevPos.y;
						float deltaTouchY = deltaMagnitudeDiffY / 10.0f;

						if(first && (Math.Abs(deltaTouchX) > .5f || Math.Abs(deltaTouchY) > .5f)){
							if(Math.Abs(deltaTouchX) > Math.Abs(deltaTouchY)){
								rotateAroundY = true;
							}
							else{
								rotateAroundY = false;
							}
							first = false;
						}

						if(rotateAroundY){
							Camera.main.transform.RotateAround(createEnvironment.centerPos, cameraRotation * Vector3.up, deltaTouchX);
						}
						else{
							Camera.main.transform.RotateAround(createEnvironment.centerPos, cameraRotation * Vector3.right, deltaTouchY);
						}
					}
				}
			}
		}
		else{
			if(Input.GetMouseButtonUp(0)){
				first = true;
			}
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
				if(!holdingAtom && !instan.addGraphicCopper && !instan.addGraphicGold && !instan.addGraphicPlatinum && !instan.changingTemp){
					Quaternion cameraRotation = Camera.main.transform.rotation;
					float deltaMagnitudeDiffX = Input.mousePosition.x - touchPrevPos.x;
					float deltaTouchX = deltaMagnitudeDiffX / 10.0f;

					//for rotation over the x-axis
					float deltaMagnitudeDiffY = Input.mousePosition.y - touchPrevPos.y;
					float deltaTouchY = deltaMagnitudeDiffY / 10.0f;

					if(first && (Math.Abs(deltaTouchX) > .5f || Math.Abs(deltaTouchY) > .5f)){
						if(Math.Abs(deltaTouchX) > Math.Abs(deltaTouchY)){
							rotateAroundY = true;
						}
						else{
							rotateAroundY = false;
						}
						first = false;
					}

					if(rotateAroundY){
						Camera.main.transform.RotateAround(createEnvironment.centerPos, cameraRotation * Vector3.up, deltaTouchX);
					}
					else{
						Camera.main.transform.RotateAround(createEnvironment.centerPos, cameraRotation * Vector3.right, deltaTouchY);
					}
				}
			}

			touchPrevPos = Input.mousePosition;
		}
		
	}

	public void setCameraCoordinates(Transform objTransform){
		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment> ();
		//transform.position = new Vector3 (objTransform.position.x, objTransform.position.y, objTransform.position.z - 10.0f);
		createEnvironment.centerPos = objTransform.position;
		transform.LookAt (objTransform);
	}

}
