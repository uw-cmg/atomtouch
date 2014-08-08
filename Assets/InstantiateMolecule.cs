using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

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
	public Texture axisTexture;
	public Texture bondLines;
	public Texture timeTexture;
	public Texture velocityTexture;
	
	private bool clicked = false;
	private float startTime = 0.0f;
	private bool first = true;
	public float holdTime = 0.05f;
	private bool destroyAtom = false;
	private GameObject atomToDelete;
	[HideInInspector]public bool changingTemp = false;

	private float guiVolume;

	//bond text
	public TextMesh textMeshPrefab;
	bool createDistanceText = true;

	void Start(){
		addGraphicCopper = false;
		addGraphicGold = false;
		addGraphicPlatinum = false;

		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment> ();
		guiVolume = createEnvironment.volume;
	}

	void OnGUI(){

		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");

		if (sliderControls != null) {
			GUI.skin = sliderControls;
		}

		if (!StaticVariables.drawBondLines) {
			GUI.color = Color.black;
		}
		if(GUI.Button(new Rect(Screen.width - 105, 20, 50, 50), bondLines)){
			StaticVariables.drawBondLines = !StaticVariables.drawBondLines;
		}
		GUI.color = Color.white;

		if(GUI.Button(new Rect(Screen.width - 165, 20, 50, 50), cameraTexture)){
			Camera.main.transform.position = new Vector3(0.0f, 0.0f, -26.0f);
			Camera.main.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
		}

		if (StaticVariables.pauseTime) {
			GUI.color = Color.black;
		}
		if(GUI.Button(new Rect(Screen.width - 225, 20, 50, 50), timeTexture)){
			StaticVariables.pauseTime = !StaticVariables.pauseTime;
		}
		GUI.color = Color.white;

		if (GUI.Button (new Rect (Screen.width - 285, 20, 50, 50), velocityTexture)) {
			for(int i = 0; i < allMolecules.Length; i++){
				GameObject currAtom = allMolecules[i];
				currAtom.rigidbody.velocity = new Vector3(UnityEngine.Random.Range(-5.0f, 5.0f), UnityEngine.Random.Range(-5.0f, 5.0f), UnityEngine.Random.Range(-5.0f, 5.0f));
			}
		}

		GUI.Label (new Rect(Screen.width - 285, 80, 250, 50), "Potential Energy: " + PotentialEnergy.finalPotentialEnergy);


		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment> ();
		GUI.Label (new Rect (25, 25, 350, 20), "Volume: " + guiVolume);
		float newVolume = GUI.VerticalSlider (new Rect (75, 55, 30, Screen.height - 135), guiVolume, 64000.0f, 1000.0f);
		if (newVolume != guiVolume) {
			guiVolume = newVolume;
			changingTemp = true; //hack for not adding another variable
		}
		else{
			int volume = (int)guiVolume;
			int remainder10 = Math.Abs(1000 - volume);
			int remainder15 = Math.Abs(3375 - volume);
			int remainder20 = Math.Abs(8000 - volume);
			int remainder25 = Math.Abs(15625 - volume);
			int remainder30 = Math.Abs(27000 - volume);
			int remainder35 = Math.Abs(42875 - volume);
			int remainder40 = Math.Abs(64000 - volume);
			if(remainder10 < remainder15 && remainder10 < remainder20 && remainder10 < remainder25 && remainder10 < remainder30 && remainder10 < remainder35 && remainder10 < remainder40){
				createEnvironment.volume = 1000;
				guiVolume = 1000;
			}
			else if(remainder15 < remainder10 && remainder15 < remainder20 && remainder15 < remainder25 && remainder15 < remainder30 && remainder15 < remainder35 && remainder15 < remainder40){
				createEnvironment.volume = 3375;
				guiVolume = 3375;
			}
			else if(remainder20 < remainder15 && remainder20 < remainder10 && remainder20 < remainder25 && remainder20 < remainder30 && remainder20 < remainder35 && remainder20 < remainder40){
				createEnvironment.volume = 8000;
				guiVolume = 8000;
			}
			else if(remainder25 < remainder10 && remainder25 < remainder15 && remainder25 < remainder20 && remainder25 < remainder30 && remainder25 < remainder35 && remainder25 < remainder40){
				createEnvironment.volume = 15625;
				guiVolume = 15625;
			}
			else if(remainder30 < remainder15 && remainder30 < remainder20 && remainder30 < remainder25 && remainder30 < remainder10 && remainder30 < remainder35 && remainder30 < remainder40){
				createEnvironment.volume = 27000;
				guiVolume = 27000;
			}
			else if(remainder35 < remainder10 && remainder35 < remainder15 && remainder35 < remainder20 && remainder35 < remainder25 && remainder35 < remainder30 && remainder35 < remainder40){
				createEnvironment.volume = 42875;
				guiVolume = 42875;
			}
			else if(remainder40 < remainder15 && remainder40 < remainder20 && remainder40 < remainder25 && remainder40 < remainder30 && remainder40 < remainder35 && remainder40 < remainder10){
				createEnvironment.volume = 64000;
				guiVolume = 64000;
			}
		}
		CheckAtomVolumePositions();

		
		GUI.Label (new Rect (170, 25, 350, 20), "Temperature: " + TemperatureCalc.desiredTemperature + "K" + " (" + (Math.Round(TemperatureCalc.desiredTemperature - 272.15, 2)).ToString() + "C)");
		float newTemp = GUI.VerticalSlider (new Rect (170, 55, 30, (Screen.height - 135)), TemperatureCalc.desiredTemperature, StaticVariables.tempRangeHigh, StaticVariables.tempRangeLow);
		if (newTemp != TemperatureCalc.desiredTemperature) {
			changingTemp = true;
			TemperatureCalc.desiredTemperature = newTemp;
		}
		else{
			//the gui temperature has been set, we can safely change the desired temperature
			int temp = (int)TemperatureCalc.desiredTemperature;
			int remainder = temp % 20;
			temp -= remainder;
			TemperatureCalc.desiredTemperature = temp;
		}

		if (Time.timeScale < 1.0f) GUI.Label (new Rect (Screen.width - 150, Screen.height - 75, 250, 20), "Slow Motion!");
		GUI.Label (new Rect (Screen.width - 150, (Screen.height - 50), 250, 20), "Time: " + Time.time);
		GUI.Label (new Rect (Screen.width - 150, (Screen.height - 25), 250, 20), "Realtime: " + Time.realtimeSinceStartup);

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
				startTime = Time.realtimeSinceStartup;
				first = true;
			}
			else{
				float currTime = Time.realtimeSinceStartup - startTime;
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
				startTime = Time.realtimeSinceStartup;
				first = true;
			}
			else{
				float currTime = Time.realtimeSinceStartup - startTime;
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
				startTime = Time.realtimeSinceStartup;
				first = true;
			}
			else{
				float currTime = Time.realtimeSinceStartup - startTime;
				if(currTime > holdTime){
					if(first){
						first = false;
						addGraphicPlatinum = true;
					}
				}
			}
		}



		for (int i = 0; i < allMolecules.Length; i++) {
			Atom atomScript = allMolecules[i].GetComponent<Atom>();
			if(atomScript.doubleTapped){
				if(GUI.Button(new Rect(455, Screen.height - 75, 75, 75), redXTexture)){
					atomScript.ResetTransparency();
					createEnvironment.centerPos = new Vector3(0.0f, 0.0f, 0.0f);
					atomScript.doubleTapped = false;
					Camera.main.transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
					Time.timeScale = 1.0f;
					atomScript.RemoveBondText();
				}

				DisplayAtomProperties(allMolecules[i]);

			}
		}

		//remember to call remove bond distance text on the garbage texture too

		if (GUI.Button (new Rect (360, Screen.height - 75, 75, 75), garbageTexture)) {
			for(int i = 0; i < allMolecules.Length; i++){
				GameObject currAtom = allMolecules[i];
				Atom atomScript = currAtom.GetComponent<Atom>();
				if(atomScript.doubleTapped){
					createEnvironment.centerPos = new Vector3(0.0f, 0.0f, 0.0f);
					Camera.main.transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
				}
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

		GUI.Label (new Rect (Screen.width - 285, 100, 225, 30), "Element Name: " + elementName);
		GUI.Label (new Rect (Screen.width - 285, 130, 225, 30), "Element Symbol: " + elementSymbol);
		GUI.Label (new Rect (Screen.width - 285, 160, 225, 30), "Position: " + currAtom.transform.position.ToString("E0"));
		GUI.Label (new Rect (Screen.width - 285, 180, 225, 30), "Velocity: " + currAtom.transform.rigidbody.velocity.ToString("E0"));

		DisplayBondProperties (currAtom);

	}
	
	void DisplayBondProperties(GameObject currAtom){

		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		List<Vector3> bonds = new List<Vector3>();
		for (int i = 0; i < allMolecules.Length; i++) {
			GameObject atomNeighbor = allMolecules[i];
			if(atomNeighbor == currAtom) continue;
			Atom atomScript = currAtom.GetComponent<Atom>();
			if(Vector3.Distance(currAtom.transform.position, atomNeighbor.transform.position) < atomScript.BondDistance(atomNeighbor)){
				bonds.Add(atomNeighbor.transform.position);
			}
		}

		if (bonds.Count == 1) {
			GUI.Label(new Rect(Screen.width - 285, 200, 225, 30), "Bond 1: " + "Distance: " + Math.Round(Vector3.Distance(bonds[0], currAtom.transform.position), 3).ToString());
		}
		else{
			//figure out the angles between the vectors
			for(int i = 0; i < bonds.Count; i++){
				GUI.Label(new Rect(Screen.width - 285, 200 + (i*30), 225, 30), "Bond " + (i+1).ToString() + " Distance: " + Math.Round (Vector3.Distance(bonds[i], currAtom.transform.position), 3).ToString());
			}

			int angleNumber = 1;
			//to display the angles, we must compute the angles between every pair of bonds
			for(int i = 0; i < bonds.Count; i++){
				for(int j = i+1; j < bonds.Count; j++){
					Vector3 vector1 = (bonds[i] - currAtom.transform.position);
					Vector3 vector2 = (bonds[j] - currAtom.transform.position);
					float angle = (float)Math.Round(Vector3.Angle(vector1, vector2), 3);
					GUI.Label(new Rect(Screen.width - 285, 230 + (bonds.Count * 30) + ((angleNumber-1)*30), 225, 30), "Angle " + angleNumber + ": " + angle);
					angleNumber++;
				}
			}

		}

	}

	Vector3 CheckPosition(Vector3 position){
		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment> ();
		Vector3 bottomPlanePos = createEnvironment.bottomPlane.transform.position;
		if (position.y > bottomPlanePos.y + (createEnvironment.height) - createEnvironment.errorBuffer) {
			position.y = bottomPlanePos.y + (createEnvironment.height) - createEnvironment.errorBuffer;
		}
		if (position.y < bottomPlanePos.y + createEnvironment.errorBuffer) {
			position.y = bottomPlanePos.y + createEnvironment.errorBuffer;
		}
		if (position.x > bottomPlanePos.x + (createEnvironment.width/2.0f) - createEnvironment.errorBuffer) {
			position.x = bottomPlanePos.x + (createEnvironment.width/2.0f) - createEnvironment.errorBuffer;
		}
		if (position.x < bottomPlanePos.x - (createEnvironment.width/2.0f) + createEnvironment.errorBuffer) {
			position.x = bottomPlanePos.x - (createEnvironment.width/2.0f) + createEnvironment.errorBuffer;
		}
		if (position.z > bottomPlanePos.z + (createEnvironment.depth/2.0f) - createEnvironment.errorBuffer) {
			position.z = bottomPlanePos.z + (createEnvironment.depth/2.0f) - createEnvironment.errorBuffer;
		}
		if (position.z < bottomPlanePos.z - (createEnvironment.depth/2.0f) + createEnvironment.errorBuffer) {
			position.z = bottomPlanePos.z - (createEnvironment.depth/2.0f) + createEnvironment.errorBuffer;
		}
		return position;
	}

	void CheckAtomVolumePositions(){

		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment>();
		for (int i = 0; i < allMolecules.Length; i++) {
			GameObject currAtom = allMolecules[i];
			Vector3 newPosition = currAtom.transform.position;
			if(currAtom.transform.position.x > createEnvironment.centerPos.x + (createEnvironment.width/2.0f) - createEnvironment.errorBuffer){
				newPosition.x = createEnvironment.centerPos.x + (createEnvironment.width/2.0f) - createEnvironment.errorBuffer;
			}
			if(currAtom.transform.position.x < createEnvironment.centerPos.x - (createEnvironment.width/2.0f) + createEnvironment.errorBuffer){
				newPosition.x = createEnvironment.centerPos.x - (createEnvironment.width/2.0f) + createEnvironment.errorBuffer;
			}
			if(currAtom.transform.position.y > createEnvironment.centerPos.y + (createEnvironment.height/2.0f) - createEnvironment.errorBuffer){
				newPosition.y = createEnvironment.centerPos.y + (createEnvironment.height/2.0f) - createEnvironment.errorBuffer;
			}
			if(currAtom.transform.position.y < createEnvironment.centerPos.y - (createEnvironment.height/2.0f) + createEnvironment.errorBuffer){
				newPosition.y = createEnvironment.centerPos.y - (createEnvironment.height/2.0f) + createEnvironment.errorBuffer;
			}
			if(currAtom.transform.position.z > createEnvironment.centerPos.z + (createEnvironment.depth/2.0f) - createEnvironment.errorBuffer){
				newPosition.z = createEnvironment.centerPos.z + (createEnvironment.depth/2.0f) - createEnvironment.errorBuffer;
			}
			if(currAtom.transform.position.z < createEnvironment.centerPos.z - (createEnvironment.depth/2.0f) + createEnvironment.errorBuffer){
				newPosition.z = createEnvironment.centerPos.z - (createEnvironment.depth/2.0f) + createEnvironment.errorBuffer;
			}
			currAtom.transform.position = newPosition;
		}

	}
	
}
