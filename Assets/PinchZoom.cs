using UnityEngine;
using System.Collections;

public class PinchZoom : MonoBehaviour {

	public float perspectiveZoomSpeed = 0.5f;
	public float orthoZoomSpeed = 0.5f;


	// Use this for initialization
	void Start () {
	
	}
	
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

		if (Input.touchCount == 2 && !beingHeld) {
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
				camera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;
				camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 0.1f, 179.9f);
			}
		}
	}
}
