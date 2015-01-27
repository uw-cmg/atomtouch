/**
 * Class: CameraScript.cs
 * Created by: Justin Moeller
 * Description: This class handles any movement that is related to the camera. Its main function
 * is to rotate the camera when the user swipes/drags the screen. Because there is a touch API
 * for iOS and the scroll rate is different on iOS, the scrolling function has been implemented
 * twice, once for iOS and once for PC. The class also handles (optionally) dynamically changing
 * the background of the simulation as well as changing the camera so it looks at an atom when 
 * it is double tapped.
 * 
 * 
 **/ 


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CameraScript : MonoBehaviour {

	public float moveSpeed = 0.25f;
	public float turnSpeed = .5f;
	public GameObject gameControl;
	public GameObject sliderPanel;
	public GameObject hudCanvas;

	private AtomTouchGUI atomTouchGUI;
	private SettingsControl settingsControl;

	private Vector2 touchPrevPos;
	private bool rotateAroundY = false;
	private bool first = true;
	private float colorStartTime;
	private float colorChangeRate = .001f;
	private float redValue = 0.0f;
	private float greenValue = 0.0f;
	private float blueValue = 0.0f;
	
	void Awake(){
		atomTouchGUI = Camera.main.GetComponent<AtomTouchGUI>();
		settingsControl = gameControl.GetComponent<SettingsControl>();
	}
	void Start(){
		colorStartTime = Time.realtimeSinceStartup;
	}

	//this function handles the rotation of the camera
	void Update () {
		if(SettingsControl.GamePaused)return;
		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment> ();
		
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			//the rotation of the camera is calculated by determining the difference in position of the touches on screen, and
			//computing a magnitude based on the displacement
			if(Input.touchCount == 1){
				Touch touch = Input.GetTouch (0);
				if(touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended){
					first = true;
				}
				if (touch.phase == TouchPhase.Moved) {
					bool holdingAtom = false;
					for (int i = 0; i < Atom.AllAtoms.Count; i++) {
						Atom currAtom = Atom.AllAtoms[i];
						if(currAtom.held){
							holdingAtom = true;
							break;
						}
					}

					AtomTouchGUI atomGUI = Camera.main.GetComponent<AtomTouchGUI>();
					if(!holdingAtom && !atomGUI.addGraphicCopper 
						&& !atomGUI.addGraphicGold && !atomGUI.addGraphicPlatinum 
						&& !atomGUI.changingTemp && !atomGUI.changingVol){
			
						Quaternion cameraRotation = Camera.main.transform.rotation;
						Vector2 touchPrevPos = touch.position - touch.deltaPosition;
						float deltaMagnitudeDiffX = touch.position.x - touchPrevPos.x;
						float deltaTouchX = deltaMagnitudeDiffX / 10.0f;

						//for rotation over the x-axis
						float deltaMagnitudeDiffY = touch.position.y - touchPrevPos.y;
						float deltaTouchY = deltaMagnitudeDiffY / -10.0f;

						//this code is neccesary because we only want to rotate around one axis at once
						if(first && (Math.Abs(deltaTouchX) > .5f || Math.Abs(deltaTouchY) > .5f)){
							if(Math.Abs(deltaTouchX) > Math.Abs(deltaTouchY)){
								rotateAroundY = true;
							}
							else{
								rotateAroundY = false;
							}
							first = false;
						}
						
						Vector3 center = createEnvironment.centerPos;
						RotateCam(ref center);
						
					}
				}
			}
		}
		else{
			//the rotation of the camera is calculated by determining 
			//the difference in position of the touches on screen, and
			//computing a magnitude based on the displacement
			if(Input.GetMouseButtonUp(0)){
				first = true;
			}
			if(Input.GetMouseButton(0)){
				/*
				bool onSlider = Input.mousePosition.x >= 
			Screen.width-sliderPanel.GetComponent<RectTransform>().rect.width
				*hudCanvas.GetComponent<Canvas>().scaleFactor;
				

				if(onSlider)return;
				*/
				bool holdingAtom = false;
				for (int i = 0; i < Atom.AllAtoms.Count; i++) {
					Atom currAtom = Atom.AllAtoms[i];
					if(currAtom.held){
						holdingAtom = true;
						break;
					}
				}

				AtomTouchGUI atomGUI = Camera.main.GetComponent<AtomTouchGUI>();
				if(!holdingAtom && !atomGUI.addGraphicCopper 
					&& !atomGUI.addGraphicGold && !atomGUI.addGraphicPlatinum 
					&& !atomGUI.changingTemp && !atomGUI.changingVol){
					Quaternion cameraRotation = Camera.main.transform.rotation;
					float deltaMagnitudeDiffX = Input.mousePosition.x - touchPrevPos.x;
					float deltaTouchX = deltaMagnitudeDiffX / 10.0f;

					//for rotation over the x-axis
					float deltaMagnitudeDiffY = Input.mousePosition.y - touchPrevPos.y;
					float deltaTouchY = deltaMagnitudeDiffY / -10.0f;

					//this code is neccesary because we only want to rotate around one axis at once
					if(first && (Math.Abs(deltaTouchX) > .5f || Math.Abs(deltaTouchY) > .5f)){
						if(Math.Abs(deltaTouchX) > Math.Abs(deltaTouchY)){
							rotateAroundY = true;
						}
						else{
							rotateAroundY = false;
						}
						first = false;
					}

					Vector3 center = createEnvironment.centerPos;
					RotateCam(ref center);
					
				}
				
			}

			touchPrevPos = Input.mousePosition;
		}
		//ChangeBackgroundColor ();

	}

	//this function is called when the user double taps an atom
	public void setCameraCoordinates(Transform objTransform){
		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment> ();
		createEnvironment.centerPos = objTransform.position;
		transform.LookAt (objTransform);
	}
	public void RotateCam(ref Vector3 center){
		/*
		if(Input.mousePosition.x < 
			(Screen.width-sliderPanel.GetComponent<RectTransform>().rect.width
				*hudCanvas.GetComponent<Canvas>().scaleFactor)){
		*/			
		if(true){
			float x = Input.GetAxis("Mouse X");
			float y = Input.GetAxis("Mouse Y");

			Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.rotation * Vector3.up, x);
			Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.rotation * Vector3.left, y);
			
			
		}
	}

	/*
	public void RotateCam(ref CreateEnvironment createEnvironment, 
		Quaternion cameraRotation, float deltaTouchX, float deltaTouchY){
		if(Input.mousePosition.x < 
			(Screen.width-sliderPanel.GetComponent<RectTransform>().rect.width
				*hudCanvas.GetComponent<Canvas>().scaleFactor)){
			float x = Input.GetAxis("Mouse X");
			float y = Input.GetAxis("Mouse Y");

			//Camera.main.transform.LookAt(createEnvironment.gameObject.transform);
			//Camera.main.transform.Rotate(y*Time.deltaTime*10, x*Time.deltaTime*10,0);
			//Debug.Log("x: " + x);
			Camera.main.transform.RotateAround(createEnvironment.centerPos, cameraRotation * Vector3.up, x);
			Camera.main.transform.RotateAround(createEnvironment.centerPos, cameraRotation * Vector3.left, y);
			
			//Camera.main.transform.localEulerAngles = new Vector3(y, x,0);
			
			if(rotateAroundY){
				//Camera.main.transform.Rotate( Quaternion.Euler(0, 30, 0));	
				Camera.main.transform.RotateAround(
					createEnvironment.centerPos, cameraRotation * Vector3.up, deltaTouchX);
				Camera.main.transform.RotateAround(
					createEnvironment.centerPos, cameraRotation * Vector3.right, deltaTouchY);
			}else{
				Camera.main.transform.RotateAround(
					createEnvironment.centerPos, cameraRotation * Vector3.right, deltaTouchY);
				Camera.main.transform.RotateAround(
					createEnvironment.centerPos, cameraRotation * Vector3.up, deltaTouchX);
			}
			
		}
	}
	*/
	//this function will change the background color of the system randomly and dynamically
	void ChangeBackgroundColor(){
		if (Time.realtimeSinceStartup - colorStartTime > 10.0f) {
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
		
		
		float colorMaximum = .37f;
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
	}

}
