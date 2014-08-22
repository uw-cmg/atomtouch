/**
 * Class: PinchZoom.cs
 * Created by: Justin Moeller
 * Description: This class handles the zooming of the camera on both iOS and PC. The camera actually doesn't
 * zoom, its z position simply changes. Because of the different scroll rates on iOS and PC there are two different
 * speeds at which the camera can zoom. There is also a limit on how far away or how close the camera can move to the 
 * box. There is also a limit on how far the camera can move in a single frame. This is to prevent the user from moving
 * the camera really far in one frame, and going "through" the minimum boundary for the box.
 * 
 * 
 **/ 


using UnityEngine;
using System.Collections;

public class PinchZoom : MonoBehaviour {

	public float touchPerspectiveZoomSpeed = 0.5f;
	public float pcPerspectiveZoomSpeed = 5.0f;
	public float orthoZoomSpeed = 0.5f;
	private GameObject doubleTappedAtom;

	//this function handles the zooming in and out of the camera. the camera actually doesnt zoom, its z-coorindate simply changes
	void Update () {

		bool beingHeld = false;
		doubleTappedAtom = null;
		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		for (int i = 0; i < allMolecules.Length; i++) {
			Atom atomScript = allMolecules[i].GetComponent<Atom>();
			if(atomScript.held){
				beingHeld = true;
			}
			if(atomScript.doubleTapped){
				doubleTappedAtom = allMolecules[i];
			}
		}

		if (Application.platform == RuntimePlatform.IPhonePlayer && Input.touchCount == 2 && !beingHeld) {
			Touch touchZero = Input.GetTouch (0);
			Touch touchOne = Input.GetTouch (1);

			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
			Quaternion cameraRotation = camera.transform.rotation;

			if(doubleTappedAtom != null){
				Vector3 projectPosition = camera.transform.position;
				float zChange = deltaMagnitudeDiff * touchPerspectiveZoomSpeed;
				//enforce a maximum value that the camera can move in one frame
				//this is to avoid going "through" the minimum and maximum distances of the box
				if(zChange > 3.0f){
					zChange = 3.0f;
				}
				else if(zChange < -3.0f){
					zChange = -3.0f;
				}
				projectPosition -= (cameraRotation * new Vector3(0.0f, 0.0f, zChange));
				//enforce a minimum and maximum distance from the center of the box that the user can scroll
				if(Vector3.Distance(projectPosition, doubleTappedAtom.transform.position) > 1.0f){
					camera.transform.position = projectPosition;
				}
			}
			else{
				Vector3 projectPosition = camera.transform.position;
				float zChange = deltaMagnitudeDiff * touchPerspectiveZoomSpeed;
				//enforce a maximum value that the camera can move in one frame
				//this is to avoid going "through" the minimum and maximum distances of the box
				if(zChange > 3.0f){
					zChange = 3.0f;
				}
				else if(zChange < -3.0f){
					zChange = -3.0f;
				}
				projectPosition -= (cameraRotation * new Vector3(0.0f, 0.0f, zChange));
				CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment>();
				Vector3 centerPos = new Vector3(createEnvironment.bottomPlane.transform.position.x, createEnvironment.bottomPlane.transform.position.y + (createEnvironment.height/2.0f), createEnvironment.bottomPlane.transform.position.z);
				//enforce a minimum and maximum distance from the center of the box that the user can scroll
				if(Vector3.Distance(projectPosition, centerPos) < 70.0f && Vector3.Distance(projectPosition, centerPos) > 10.0f){
					camera.transform.position = projectPosition;
				}
			}
		}
		else if(Application.platform != RuntimePlatform.IPhonePlayer && !beingHeld){
			float deltaMagnitudeDiff = Input.GetAxis("Mouse ScrollWheel");
			Quaternion cameraRotation = camera.transform.rotation;
			if(doubleTappedAtom != null){
				Vector3 projectPosition = camera.transform.position;
				float zChange = deltaMagnitudeDiff * pcPerspectiveZoomSpeed;
				//enforce a maximum value that the camera can move in one frame
				//this is to avoid going "through" the minimum and maximum distances of the box
				if(zChange > 3.0f){
					zChange = 3.0f;
				}
				else if(zChange < -3.0f){
					zChange = -3.0f;
				}
				projectPosition -= (cameraRotation * new Vector3(0.0f, 0.0f, zChange));
				//enforce a minimum and maximum distance from the center of the box that the user can scroll
				if(Vector3.Distance(projectPosition, doubleTappedAtom.transform.position) > 1.0f){
					camera.transform.position = projectPosition;
				}
			}
			else{
				Vector3 projectPosition = camera.transform.position;
				float zChange = deltaMagnitudeDiff * pcPerspectiveZoomSpeed;
				//enforce a maximum value that the camera can move in one frame
				//this is to avoid going "through" the minimum and maximum distances of the box
				if(zChange > 3.0f){
					zChange = 3.0f;
				}
				else if(zChange < -3.0f){
					zChange = -3.0f;
				}
				projectPosition -= (cameraRotation * new Vector3(0.0f, 0.0f, zChange));
				CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment>();
				Vector3 centerPos = new Vector3(createEnvironment.bottomPlane.transform.position.x, createEnvironment.bottomPlane.transform.position.y + (createEnvironment.height/2.0f), createEnvironment.bottomPlane.transform.position.z);
				//enforce a minimum and maximum distance from the center of the box that the user can scroll
				if(Vector3.Distance(projectPosition, centerPos) < 70.0f && Vector3.Distance(projectPosition, centerPos) > 10.0f){
					camera.transform.position = projectPosition;
				}
			}
		}
	}
}
