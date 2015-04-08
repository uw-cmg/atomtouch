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

public class CameraGooey : MonoBehaviour {

	public float moveSpeed = 0.25f;
	public float turnSpeed = .5f;
	public float rotateSensitivityLeftRight = 6.0f;
	public float rotateSensitivityUpDown = 10.0f;
	public float forwardSpeed = 2.5f;
	public float sideSpeed = 1.5f;

	
	private AtomTouchGUI atomTouchGUI;
	private SettingsControl settingsControl;

	private float colorStartTime;
	private float colorChangeRate = .001f;
	private float redValue = 0.0f;
	private float greenValue = 0.0f;
	private float blueValue = 0.0f;
	private Environment env;
	void Awake(){
		//env = GameObject.Find("Environment").gameObject.GetComponent<Environment> ();
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
		if(!holdingAtom){
			
			Vector3 center = env.initialCenterPos;
			RotateCam(ref center);
			
		}
	}
	//this function handles the rotation of the camera
	void Update () {
		/*
		if(SettingsControl.GamePaused){
		//	Debug.Log("gamepaused");
			return;
		}
		*/
		//Debug.Log("resumed");
		if (Application.isMobilePlatform) {
			if(Input.touchCount == 1){
				Touch touch = Input.GetTouch (0);	
				if (touch.phase == TouchPhase.Moved) {
					UpdateCamera();
				}
			}
		}
		else{
			Navigate();
			if(Input.GetMouseButton(0)){
				UpdateCamera();
			}

		}

	}
	public void Navigate(){
		float fb = Input.GetAxis("Vertical");
		float lr = Input.GetAxis("Horizontal");

		float deltaFB = forwardSpeed * fb * Time.deltaTime;
		float deltaLR = sideSpeed * lr * Time.deltaTime;
		transform.position += transform.forward * deltaFB;
		transform.position += transform.right * deltaLR;
	}
	public void RotateCam(ref Vector3 center){
		
		float x = Input.GetAxis("Mouse X");
		float y = Input.GetAxis("Mouse Y");

		Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.rotation * Vector3.up, rotateSensitivityUpDown *x);
		Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.rotation * Vector3.left, rotateSensitivityLeftRight *y);
	
	
	}


}
