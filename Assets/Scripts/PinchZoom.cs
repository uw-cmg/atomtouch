using UnityEngine;
using System.Collections;

public class PinchZoom : MonoBehaviour {

	public float touchPerspectiveZoomSpeed = 0.5f;
	public float pcPerspectiveZoomSpeed = 5.0f;
	public float orthoZoomSpeed = 0.5f;
	private GameObject doubleTappedAtom;
	
	// Update is called once per frame
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
				projectPosition -= (cameraRotation * new Vector3(0.0f, 0.0f, deltaMagnitudeDiff * pcPerspectiveZoomSpeed));
				if(Vector3.Distance(projectPosition, doubleTappedAtom.transform.position) > 1.0f){
					camera.transform.position -= (cameraRotation * new Vector3(0.0f, 0.0f, deltaMagnitudeDiff * touchPerspectiveZoomSpeed));
				}
			}
			else{
				Vector3 projectPosition = camera.transform.position;
				projectPosition -= (cameraRotation * new Vector3(0.0f, 0.0f, deltaMagnitudeDiff * touchPerspectiveZoomSpeed));
				CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment>();
				Vector3 centerPos = new Vector3(createEnvironment.bottomPlane.transform.position.x, createEnvironment.bottomPlane.transform.position.y + (createEnvironment.height/2.0f), createEnvironment.bottomPlane.transform.position.z);
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
				projectPosition -= (cameraRotation * new Vector3(0.0f, 0.0f, deltaMagnitudeDiff * pcPerspectiveZoomSpeed));
				if(Vector3.Distance(projectPosition, doubleTappedAtom.transform.position) > 1.0f){
					camera.transform.position -= (cameraRotation * new Vector3(0.0f, 0.0f, deltaMagnitudeDiff * pcPerspectiveZoomSpeed));
				}
			}
			else{
				Vector3 projectPosition = camera.transform.position;
				projectPosition -= (cameraRotation * new Vector3(0.0f, 0.0f, deltaMagnitudeDiff * pcPerspectiveZoomSpeed));
				CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment>();
				Vector3 centerPos = new Vector3(createEnvironment.bottomPlane.transform.position.x, createEnvironment.bottomPlane.transform.position.y + (createEnvironment.height/2.0f), createEnvironment.bottomPlane.transform.position.z);
				if(Vector3.Distance(projectPosition, centerPos) < 70.0f && Vector3.Distance(projectPosition, centerPos) > 10.0f){
					camera.transform.position = projectPosition;
				}
			}
		}
	}
}
