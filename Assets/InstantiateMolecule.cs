using UnityEngine;
using System.Collections;
using System;

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
	public Texture cameraTexture;
	
	private bool clicked = false;
	private float startTime = 0.0f;
	private bool first = true;
	public float holdTime = 0.05f;
	private bool destroyAtom = false;
	private GameObject atomToDelete;
	public bool changingTemp = false;


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

		if(GUI.Button(new Rect(Screen.width - 225, 20, 50, 50), cameraTexture)){
			Camera.main.transform.position = new Vector3(0.0f, 0.0f, -26.0f);
			Camera.main.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
		}

		GUI.Label (new Rect (25, 25, 350, 20), "Temperature: " + TemperatureCalc.desiredTemperature + "K" + " (" + (Math.Round(TemperatureCalc.desiredTemperature - 272.15, 2)).ToString() + "C)");
		float newTemp = GUI.VerticalSlider (new Rect (75, 55, 30, (Screen.height - 135)), TemperatureCalc.desiredTemperature, StaticVariables.tempRangeHigh, StaticVariables.tempRangeLow);
		if (newTemp != TemperatureCalc.desiredTemperature) {
			TemperatureCalc.desiredTemperature = newTemp;
			changingTemp = true;
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
					CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment>();
					createEnvironment.centerPos = new Vector3(0.0f, 0.0f, 0.0f);
					atomScript.doubleTapped = false;
					Camera.main.transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
				}

				DisplayAtomProperties(allMolecules[i]);

			}
		}


		if (GUI.Button (new Rect (360, Screen.height - 75, 75, 75), garbageTexture)) {
			for(int i = 0; i < allMolecules.Length; i++){
				GameObject currAtom = allMolecules[i];
				Atom atomScript = currAtom.GetComponent<Atom>();
				if(atomScript.selected){
					Destroy(currAtom);
				}
			}
		}

		
		if (Input.GetMouseButtonUp (0)) {

			if(addGraphicCopper && Input.mousePosition.x < Screen.width && Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height){
				Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20.0f));
				Quaternion curRotation = Quaternion.Euler(0, 0, 0);
				curPosition = CheckPosition(curPosition);
				Instantiate(copperPrefab, curPosition, curRotation);
			}

			if(addGraphicGold && Input.mousePosition.x < Screen.width && Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height){
				Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20.0f));
				Quaternion curRotation = Quaternion.Euler(0, 0, 0);
				curPosition = CheckPosition(curPosition);
				Instantiate(goldPrefab, curPosition, curRotation);
			}

			if(addGraphicPlatinum && Input.mousePosition.x < Screen.width && Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height){
				Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20.0f));
				Quaternion curRotation = Quaternion.Euler(0, 0, 0);
				curPosition = CheckPosition(curPosition);
				Instantiate(platinumPrefab, curPosition, curRotation);
			}
			
			addGraphicCopper = false;
			addGraphicGold = false;
			addGraphicPlatinum = false;
			changingTemp = false;
			first = true;
			clicked = false;
			startTime = 0.0f;
		}

	}

	void DisplayAtomProperties(GameObject currAtom){

		String elementName = "";
		String elementSymbol = "";

		//probably a better way to do this via polymorphism
		if (currAtom.GetComponent<Copper> () != null) {
			elementName = "Copper";
			elementSymbol = "Cu";
		}
		else if (currAtom.GetComponent<Gold> () != null) {
			elementName = "Gold";
			elementSymbol = "Au";
		}
		else if (currAtom.GetComponent<Platinum> () != null) {
			elementName = "Platinum";
			elementSymbol = "Pt";
		}

		GUI.Label (new Rect (Screen.width - 225, 100, 225, 30), "Element Name: " + elementName);
		GUI.Label (new Rect (Screen.width - 225, 130, 225, 30), "Element Symbol: " + elementSymbol);
		GUI.Label (new Rect (Screen.width - 225, 160, 225, 30), "Position: " + currAtom.transform.position);

	}

	Vector3 CheckPosition(Vector3 position){
		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment> ();
		if (position.y > createEnvironment.centerPos.y + (createEnvironment.height/2.0f) - createEnvironment.errorBuffer) {
			position.y = createEnvironment.centerPos.y + (createEnvironment.height/2.0f) - createEnvironment.errorBuffer;
		}
		if (position.y < createEnvironment.centerPos.y - (createEnvironment.height/2.0f) + createEnvironment.errorBuffer) {
			position.y = createEnvironment.centerPos.y - (createEnvironment.height/2.0f) + createEnvironment.errorBuffer;;
		}
		if (position.x > createEnvironment.centerPos.x + (createEnvironment.width/2.0f) - createEnvironment.errorBuffer) {
			position.x = createEnvironment.centerPos.x + (createEnvironment.width/2.0f) - createEnvironment.errorBuffer;
		}
		if (position.x < createEnvironment.centerPos.x - (createEnvironment.width/2.0f) + createEnvironment.errorBuffer) {
			position.x = createEnvironment.centerPos.x - (createEnvironment.width/2.0f) + createEnvironment.errorBuffer;
		}
		if (position.z > createEnvironment.centerPos.z + (createEnvironment.depth/2.0f) - createEnvironment.errorBuffer) {
			position.z = createEnvironment.centerPos.z + (createEnvironment.depth/2.0f) - createEnvironment.errorBuffer;
		}
		if (position.z < createEnvironment.centerPos.z - (createEnvironment.depth/2.0f) + createEnvironment.errorBuffer) {
			position.z = createEnvironment.centerPos.z - (createEnvironment.depth/2.0f) + createEnvironment.errorBuffer;
		}
		return position;
	}
	
}
