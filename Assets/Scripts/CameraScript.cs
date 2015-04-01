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
	public float rotateSensitivityLeftRight = 6.0f;
	public float rotateSensitivityUpDown = 10.0f;
	public GameObject gameControl;
	public GameObject sliderPanel;
	public GameObject hudCanvas;
	
	private AtomTouchGUI atomTouchGUI;
	private SettingsControl settingsControl;

	private CreateEnvironment createEnvironment;
	void Awake(){
		atomTouchGUI = Camera.main.GetComponent<AtomTouchGUI>();
		settingsControl = gameControl.GetComponent<SettingsControl>();
		createEnvironment = Camera.main.GetComponent<CreateEnvironment> ();
	}
	void Start(){
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
		if(!holdingAtom
			&& !atomTouchGUI.changingTemp && !atomTouchGUI.changingVol){
			
			Vector3 center = createEnvironment.centerPos;
			RotateCam(ref center);
			
		}
	}
	//this function handles the rotation of the camera
	void Update () {
		if(SettingsControl.GamePaused){
		//	Debug.Log("gamepaused");
			return;
		}
		//Debug.Log("resumed");
		if (Application.isMobilePlatform) {
			if(Input.touchCount == 1){
				Touch touch = Input.GetTouch (0);	
				if (touch.phase == TouchPhase.Moved) {
					UpdateCamera();
				}
			}else if(Input.touchCount == 2){
				//Two finger rotation
				//how to differentiate rotate from zoom?
				
				Touch finger0 = Input.GetTouch(0);
				Touch finger1 = Input.GetTouch(1);
				if(finger0.phase == TouchPhase.Moved
					|| finger1.phase == TouchPhase.Moved){
					
					Vector2 oldFinger0 = finger0.position - finger0.deltaPosition;
					Vector2 oldFinger1 = finger1.position - finger1.deltaPosition;
					float angle = Vector2.Angle(oldFinger1-oldFinger0, finger1.position - finger0.position);
					Debug.Log(angle);
					if(angle < 5)return;
					Camera.main.gameObject.transform.eulerAngles
						+= new Vector3(0,0,angle);
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

		Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.rotation * Vector3.up, rotateSensitivityUpDown *x);
		Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.rotation * Vector3.left, rotateSensitivityLeftRight *y);
	
	
	}

	

}
