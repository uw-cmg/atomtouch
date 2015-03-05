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

	private float colorStartTime;
	private float colorChangeRate = .001f;
	private float redValue = 0.0f;
	private float greenValue = 0.0f;
	private float blueValue = 0.0f;
	private CreateEnvironment createEnvironment;
	void Awake(){
		atomTouchGUI = Camera.main.GetComponent<AtomTouchGUI>();
		settingsControl = gameControl.GetComponent<SettingsControl>();
		createEnvironment = Camera.main.GetComponent<CreateEnvironment> ();
	}
	void Start(){
		colorStartTime = Time.realtimeSinceStartup;
	}

	public bool HasAtomHeld(){
		for (int i = 0; i < Atom.AllAtoms.Count; i++) {
			Atom currAtom = Atom.AllAtoms[i];
			if(currAtom.held){
				return true;
			}
		}
		return false;
	}
	public void UpdateCamera(){
		bool holdingAtom = HasAtomHeld();
		if(!holdingAtom && !atomTouchGUI.addGraphicCopper 
			&& !atomTouchGUI.addGraphicGold && !atomTouchGUI.addGraphicPlatinum 
			&& !atomTouchGUI.changingTemp && !atomTouchGUI.changingVol){
			
			Vector3 center = createEnvironment.centerPos;
			RotateCam(ref center);
			
		}
	}
	//this function handles the rotation of the camera
	void Update () {
		if(SettingsControl.GamePaused)return;
		
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			if(Input.touchCount == 1){
				Touch touch = Input.GetTouch (0);	
				if (touch.phase == TouchPhase.Moved) {
					UpdateCamera();
				}
			}
		}
		else{
			if(Input.GetMouseButton(0)){
				UpdateCamera();
			}

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
		
		float x = Input.GetAxis("Mouse X");
		float y = Input.GetAxis("Mouse Y");

		Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.rotation * Vector3.up, x);
		Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.rotation * Vector3.left, y);
	
	
	}

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
		GetComponent<Camera>().backgroundColor = new Color(GetComponent<Camera>().backgroundColor.r + redValue, GetComponent<Camera>().backgroundColor.g + greenValue, GetComponent<Camera>().backgroundColor.b + blueValue);
		if (GetComponent<Camera>().backgroundColor.r > colorMaximum) {
			GetComponent<Camera>().backgroundColor = new Color(colorMaximum, GetComponent<Camera>().backgroundColor.g, GetComponent<Camera>().backgroundColor.b);
		}
		else if (GetComponent<Camera>().backgroundColor.r < colorMinimum) {
			GetComponent<Camera>().backgroundColor = new Color(colorMinimum, GetComponent<Camera>().backgroundColor.g, GetComponent<Camera>().backgroundColor.b);
		}
		if (GetComponent<Camera>().backgroundColor.g > colorMaximum) {
			GetComponent<Camera>().backgroundColor = new Color(GetComponent<Camera>().backgroundColor.r, colorMaximum, GetComponent<Camera>().backgroundColor.b);
		}
		else if (GetComponent<Camera>().backgroundColor.g < colorMinimum) {
			GetComponent<Camera>().backgroundColor = new Color(GetComponent<Camera>().backgroundColor.r, colorMinimum, GetComponent<Camera>().backgroundColor.b);
		}
		if (GetComponent<Camera>().backgroundColor.b > colorMaximum) {
			GetComponent<Camera>().backgroundColor = new Color(GetComponent<Camera>().backgroundColor.r, GetComponent<Camera>().backgroundColor.g, colorMaximum);
		}
		else if (GetComponent<Camera>().backgroundColor.b < colorMinimum) {
			GetComponent<Camera>().backgroundColor = new Color(GetComponent<Camera>().backgroundColor.r, GetComponent<Camera>().backgroundColor.g, colorMinimum);
		}
	}

}
