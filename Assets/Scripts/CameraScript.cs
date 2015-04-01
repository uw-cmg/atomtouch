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
	private bool twoFingerRotating = false;
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
		//if(true){
		if (Application.isMobilePlatform) {
			if(Input.touchCount == 1 ){
				Touch touch = Input.GetTouch (0);
				//if one finger lifed up during two finger rotation,
				//ignore the remaining finger
				//until it ends too
				ProcessOneFingerRotate(touch);
			}else if(Input.touchCount == 2){
				//Two finger rotation
				//how to differentiate rotate from zoom?
				
				Touch finger0 = Input.GetTouch(0);
				Touch finger1 = Input.GetTouch(1);
				if(finger0.phase == TouchPhase.Moved
					|| finger1.phase == TouchPhase.Moved){
					
					ProcessTwoFingerRotate(finger0, finger1);
				}
				
			}
		}
		else{
			if(Input.GetMouseButton(0))
				UpdateCamera();
		}

	}
	public void ProcessOneFingerRotate(Touch touch){
		if(twoFingerRotating){
			if(touch.phase != TouchPhase.Ended 
				&& touch.phase != TouchPhase.Canceled ){
				return;
			}else{
				twoFingerRotating = false;
				return;
			}
		}
		else if (touch.phase == TouchPhase.Moved) {
			UpdateCamera();
		}
	}
	public void ProcessTwoFingerRotate(Touch finger0, Touch finger1){
		Vector2 oldFingerLeft, oldFingerRight;
		Vector2 newFingerLeft, newFingerRight;
		//determine left and right
		DeterminLeftRightFingers(
			finger0, finger1,
			out oldFingerLeft, 
			out oldFingerRight,
			out newFingerLeft,
			out newFingerRight
		);

		float diffX = oldFingerLeft.x - oldFingerRight.x;
		//if the two fingers are too close, leave it for pinchzoom
		if(Mathf.Abs(diffX) < Screen.width * 0.35f){
			return;
		}
		TwoFingerRotate(
			oldFingerLeft,oldFingerRight,
			newFingerLeft,newFingerRight
		);
	}

	public void TwoFingerRotate(
		Vector2 oldFingerLeft,
		Vector2 oldFingerRight,
		Vector2 newFingerLeft,
		Vector2 newFingerRight
	){
		twoFingerRotating = true;
		Vector2 v_old = oldFingerRight-oldFingerLeft;
		Vector2 v_new = newFingerRight-newFingerLeft;
		

		float angle = Vector2.Angle(v_old, v_new);
		if(newFingerRight.y > oldFingerRight.y
			&& newFingerLeft.y < oldFingerLeft.y){
			//counterclockwise
			angle *= -1;
			Camera.main.gameObject.transform.eulerAngles
			+= new Vector3(0,0,angle);
		}else if(newFingerRight.y < oldFingerRight.y
			&& newFingerLeft.y > oldFingerLeft.y){
			//clockwise
			Camera.main.gameObject.transform.eulerAngles
			+= new Vector3(0,0,angle);
		}
	}
	//given two finger touches, determine which is left and which is right

	public void DeterminLeftRightFingers(
		Touch finger0,
		Touch finger1,
		out Vector2 oldFingerLeft, 
		out Vector2 oldFingerRight,
		out Vector2 newFingerLeft,
		out Vector2 newFingerRight){

		//determine left and right
		if(finger1.position.x-finger1.deltaPosition.x
			> finger0.position.x-finger0.deltaPosition.x){

			newFingerLeft = finger0.position;
			newFingerRight = finger1.position;
			oldFingerLeft = finger0.position - finger0.deltaPosition;
			oldFingerRight = finger1.position - finger1.deltaPosition;
		}else{
			newFingerLeft = finger1.position;
			newFingerRight = finger0.position;
			oldFingerLeft = finger1.position - finger1.deltaPosition;
			oldFingerRight = finger0.position - finger0.deltaPosition;
		}
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
