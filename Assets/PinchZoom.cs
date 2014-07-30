using UnityEngine;
using System.Collections;

public class PinchZoom : MonoBehaviour {

	public float touchPerspectiveZoomSpeed = 0.5f;
	public float pcPerspectiveZoomSpeed = 5.0f;
	public float orthoZoomSpeed = 0.5f;
	
	// Update is called once per frame
	void Update () {

		bool beingHeld = false;
		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		for (int i = 0; i < allMolecules.Length; i++) {
			Atom atomScript = allMolecules[i].GetComponent<Atom>();
			if(atomScript.held){
				beingHeld = true;
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

			if(camera.isOrthoGraphic){
				camera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;
				camera.orthographicSize = Mathf.Max (camera.orthographicSize, 0.1f);
			}
			else{
				camera.fieldOfView += deltaMagnitudeDiff * touchPerspectiveZoomSpeed;
				camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 5.0f, 150.0f);
			}
		}
		else if(Application.platform != RuntimePlatform.IPhonePlayer && !beingHeld){
			float deltaMagnitudeDiff = Input.GetAxis("Mouse ScrollWheel");
			camera.fieldOfView += deltaMagnitudeDiff * pcPerspectiveZoomSpeed;
			camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 5.0f, 150.0f);
		}
	}
}
