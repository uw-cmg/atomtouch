using UnityEngine;
using System.Collections;

public class InstantiateMolecule : MonoBehaviour {

	public Rigidbody copperPrefab;
	public Rigidbody goldPrefab;
	public Rigidbody platinumPrefab;
	public GUISkin sliderControls;

	public Texture copperTexture;
	public Texture addCopperTexture;
	[HideInInspector]public bool addGraphicCopper;

	public Texture goldTexture;
	public Texture addGoldTexture;
	[HideInInspector]public bool addGraphicGold;

	public Texture platinumTexture;
	public Texture addPlatinumTexture;
	[HideInInspector]public bool addGraphicPlatinum;

	public Texture garbageTexture;
	public Texture redXTexture;

	public Texture touchIcon;
	public Texture clickIcon;
	
	private bool clicked = false;
	private float startTime = 0.0f;
	private bool first = true;
	public float holdTime = 0.05f;
	private bool destroyAtom = false;
	private GameObject atomToDelete;


	void Start(){
		addGraphicCopper = false;
		addGraphicGold = false;
		addGraphicPlatinum = false;
	}

	void OnGUI(){

		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");

		if (sliderControls != null) {
			GUI.skin = sliderControls;
		}

		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			if(StaticVariables.touchScreen){
				if(GUI.Button(new Rect(Screen.width - 165, 20, 50, 50), touchIcon)){
					StaticVariables.touchScreen = false;
				}
			}
			else{
				if(GUI.Button(new Rect(Screen.width - 165, 20, 50, 50), clickIcon)){
					StaticVariables.touchScreen = true;
				}
			}
		}

		GUI.Label (new Rect (25, 25, 250, 20), "Temperature: " + TemperatureCalc.desiredTemperature);
		float newTemp = GUI.VerticalSlider (new Rect (75, 55, 30, (Screen.height - 135)), TemperatureCalc.desiredTemperature, StaticVariables.tempRangeHigh, StaticVariables.tempRangeLow);
		if (newTemp != SphereScript.desiredTemperature) {
			TemperatureCalc.desiredTemperature = newTemp;
		}

		GUI.Label (new Rect (Screen.width - 100, 25, 250, 20), "Drag: " + allMolecules[0].rigidbody.drag);
		float newDrag = GUI.VerticalSlider (new Rect (Screen.width - 75, 55, 30, (Screen.height - 135)), allMolecules[0].rigidbody.drag, 30.0f, 0.0f);
		for (int i = 0; i < allMolecules.Length; i++) {
			allMolecules[i].rigidbody.drag = newDrag;
		}

		GUI.Label (new Rect (Screen.width - 100, (Screen.height - 50), 250, 20), "Time: " + Time.time);

		if (addGraphicCopper) {
			Color guiColor = Color.white;
			guiColor.a = 0.25f;
			GUI.color = guiColor;
			GUI.DrawTexture(new Rect((Input.mousePosition.x - 25.0f), (Screen.height - Input.mousePosition.y) - 25.0f, 50.0f, 50.0f), addCopperTexture);
			GUI.color = Color.white;
		}

		if (addGraphicGold) {
			Color guiColor = Color.white;
			guiColor.a = 0.25f;
			GUI.color = guiColor;
			GUI.DrawTexture(new Rect((Input.mousePosition.x - 25.0f), (Screen.height - Input.mousePosition.y) - 25.0f, 50.0f, 50.0f), addGoldTexture);
			GUI.color = Color.white;
		}

		if (addGraphicPlatinum) {
			Color guiColor = Color.white;
			guiColor.a = 0.25f;
			GUI.color = guiColor;
			GUI.DrawTexture(new Rect((Input.mousePosition.x - 25.0f), (Screen.height - Input.mousePosition.y) - 25.0f, 50.0f, 50.0f), addPlatinumTexture);
			GUI.color = Color.white;
		}

		if (GUI.RepeatButton (new Rect (75, Screen.height - 75, 75, 75), copperTexture)) {
			if(!clicked){
				clicked = true;
				startTime = Time.time;
				first = true;
			}
			else{
				float currTime = Time.time - startTime;
				if(currTime > holdTime){
					if(first){
						first = false;
						addGraphicCopper = true;
					}
				}
			}
		}

		if (GUI.RepeatButton (new Rect (170, Screen.height - 75, 75, 75), goldTexture)) {
			if(!clicked){
				clicked = true;
				startTime = Time.time;
				first = true;
			}
			else{
				float currTime = Time.time - startTime;
				if(currTime > holdTime){
					if(first){
						first = false;
						addGraphicGold = true;
					}
				}
			}
		}

		if (GUI.RepeatButton (new Rect (265, Screen.height - 75, 75, 75), platinumTexture)) {
			if(!clicked){
				clicked = true;
				startTime = Time.time;
				first = true;
			}
			else{
				float currTime = Time.time - startTime;
				if(currTime > holdTime){
					if(first){
						first = false;
						addGraphicPlatinum = true;
					}
				}
			}
		}


		GameObject atomBeingHeld = null;
		bool holdingAtom = false;
		for (int i = 0; i < allMolecules.Length; i++) {
			Atom atomScript = allMolecules[i].GetComponent<Atom>();
			if(atomScript.held){
				holdingAtom = true;
				atomBeingHeld = allMolecules[i];
			}
			if(atomScript.doubleTapped){
				if(GUI.Button(new Rect(455, Screen.height - 75, 75, 75), redXTexture)){
					CameraScript cameraScript = Camera.main.GetComponent<CameraScript>();
					cameraScript.centerPos = new Vector3(0.0f, 0.0f, 0.0f);
					atomScript.doubleTapped = false;
					Camera.main.transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
				}
			}
		}

		
		if (Application.platform == RuntimePlatform.IPhonePlayer && Input.touchCount == 0 && destroyAtom) {
			Destroy(atomToDelete);
			destroyAtom = false;
		}
		
		if (holdingAtom && Input.mousePosition.x < 435 && Input.mousePosition.x > 360 && Input.mousePosition.y < 75 && Input.mousePosition.y > 0) {
			Color guiColor = Color.red;
			guiColor.a = 0.25f;
			GUI.color = guiColor;
			if(Application.platform != RuntimePlatform.IPhonePlayer){
				if(Input.GetMouseButtonUp(0) && atomBeingHeld != null){
					Destroy(atomBeingHeld);
				}
			}
			else if(Input.touchCount == 1){
				destroyAtom = true;
				atomToDelete = atomBeingHeld;
			}
		}
		
		GUI.DrawTexture (new Rect (360, Screen.height - 75, 75, 75), garbageTexture);

		
		if (Input.GetMouseButtonUp (0)) {

			if(addGraphicCopper && Input.mousePosition.x < Screen.width && Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height){
				Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20.0f));
				Quaternion curRotation = Quaternion.Euler(0, 0, 0);
				Instantiate(copperPrefab, curPosition, curRotation);
			}

			if(addGraphicGold && Input.mousePosition.x < Screen.width && Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height){
				Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20.0f));
				Quaternion curRotation = Quaternion.Euler(0, 0, 0);
				Instantiate(goldPrefab, curPosition, curRotation);
			}

			if(addGraphicPlatinum && Input.mousePosition.x < Screen.width && Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height){
				Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20.0f));
				Quaternion curRotation = Quaternion.Euler(0, 0, 0);
				Instantiate(platinumPrefab, curPosition, curRotation);
			}
			
			addGraphicCopper = false;
			addGraphicGold = false;
			addGraphicPlatinum = false;
			first = true;
			clicked = false;
			startTime = 0.0f;
		}

	}

}
