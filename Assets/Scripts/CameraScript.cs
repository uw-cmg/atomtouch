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
	private float colorStartTime;
	private float colorChangeRate = .001f;
	private float redValue = 0.0f;
	private float greenValue = 0.0f;
	private float blueValue = 0.0f;
	
	void Start(){
		colorStartTime = Time.realtimeSinceStartup;
	}

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

					AtomTouchGUI atomGUI = Camera.main.GetComponent<AtomTouchGUI>();
					if(!holdingAtom && !atomGUI.addGraphicCopper && !atomGUI.addGraphicGold && !atomGUI.addGraphicPlatinum && !atomGUI.changingSlider){
						Quaternion cameraRotation = Camera.main.transform.rotation;
						Vector2 touchPrevPos = touch.position - touch.deltaPosition;
						float deltaMagnitudeDiffX = touch.position.x - touchPrevPos.x;
						float deltaTouchX = deltaMagnitudeDiffX / 10.0f;

						//for rotation over the x-axis
						float deltaMagnitudeDiffY = touch.position.y - touchPrevPos.y;
						float deltaTouchY = deltaMagnitudeDiffY / -10.0f;

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

				AtomTouchGUI atomGUI = Camera.main.GetComponent<AtomTouchGUI>();
				if(!holdingAtom && !atomGUI.addGraphicCopper && !atomGUI.addGraphicGold && !atomGUI.addGraphicPlatinum && !atomGUI.changingSlider){
					Quaternion cameraRotation = Camera.main.transform.rotation;
					float deltaMagnitudeDiffX = Input.mousePosition.x - touchPrevPos.x;
					float deltaTouchX = deltaMagnitudeDiffX / 10.0f;

					//for rotation over the x-axis
					float deltaMagnitudeDiffY = Input.mousePosition.y - touchPrevPos.y;
					float deltaTouchY = deltaMagnitudeDiffY / -10.0f;

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


		if (Time.realtimeSinceStartup - colorStartTime > 5.0f) {
			if (UnityEngine.Random.Range (0.0f, 1.0f) > .5f) {
				if(UnityEngine.Random.Range (0.0f, 1.0f) > .5f){
					redValue = -colorChangeRate;
				}
				else{
					redValue = colorChangeRate;
				}
			}
			else{
				redValue = 0.0f;
			}
			if (UnityEngine.Random.Range (0.0f, 1.0f) > .5f) {
				if (UnityEngine.Random.Range (0.0f, 1.0f) > .5f){
					greenValue = -colorChangeRate;
				}
				else{
					greenValue = colorChangeRate;
				}
			}
			else{
				greenValue = 0.0f;
			}
			if (UnityEngine.Random.Range (0.0f, 1.0f) > .5f) {
				if (UnityEngine.Random.Range (0.0f, 1.0f) > .5f){
					blueValue = -colorChangeRate;
				}
				else{
					blueValue = colorChangeRate;
				}
			}
			else{
				blueValue = 0.0f;
			}
			colorStartTime = Time.realtimeSinceStartup;
		}


		float colorMaximum = .45f;
		float colorMinimum = 0.1f;
		camera.backgroundColor = new Color(camera.backgroundColor.r + redValue, camera.backgroundColor.g + greenValue, camera.backgroundColor.b + blueValue);
		if (camera.backgroundColor.r > colorMaximum) {
			camera.backgroundColor = new Color(colorMaximum, camera.backgroundColor.g, camera.backgroundColor.b);
		}
		else if (camera.backgroundColor.r < colorMinimum) {
			camera.backgroundColor = new Color(colorMinimum, camera.backgroundColor.g, camera.backgroundColor.b);
		}
		if (camera.backgroundColor.g > colorMaximum) {
			camera.backgroundColor = new Color(camera.backgroundColor.r, colorMaximum, camera.backgroundColor.b);
		}
		else if (camera.backgroundColor.g < colorMinimum) {
			camera.backgroundColor = new Color(camera.backgroundColor.r, colorMinimum, camera.backgroundColor.b);
		}
		if (camera.backgroundColor.b > colorMaximum) {
			camera.backgroundColor = new Color(camera.backgroundColor.r, camera.backgroundColor.g, colorMaximum);
		}
		else if (camera.backgroundColor.b < colorMinimum) {
			camera.backgroundColor = new Color(camera.backgroundColor.r, camera.backgroundColor.g, colorMinimum);
		}
		print ("Color: " + camera.backgroundColor);
	}

	public void setCameraCoordinates(Transform objTransform){
		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment> ();
		//transform.position = new Vector3 (objTransform.position.x, objTransform.position.y, objTransform.position.z - 10.0f);
		createEnvironment.centerPos = objTransform.position;
		transform.LookAt (objTransform);
	}

}
