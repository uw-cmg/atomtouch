/**
 * Class: AtomTouchGUI.cs
 * Created by: Justin Moeller
 * Description: The sole purpose of this class is to define the GUI of the game. Because of this,
 * most of the global variables are declared public to easily swap out the graphics of the UI.
 * There are a couple of weird quirks with the creation of the UI. The first is that the graph
 * is NOT defined in this class, it has its own class. This is because the function responsible
 * for creating the UI, OnGUI(), draws over OnPostRender, meaning it draws over the graph. To solve
/**
 * Class: AtomTouchGUI.cs
 * Created by: Justin Moeller
 * Description: The sole purpose of this class is to define the GUI of the game. Because of this,
 * most of the global variables are declared public to easily swap out the graphics of the UI.
 * There are a couple of weird quirks with the creation of the UI. The first is that the graph
 * is NOT defined in this class, it has its own class. This is because the function responsible
 * for creating the UI, OnGUI(), draws over OnPostRender, meaning it draws over the graph. To solve
 * this problem, the UI simply draws around the graph and the graph draws in the blank space. (sort of
 * like a cookie cutter) The second quirk is that, in order to get around Unity's restrictions of drawing
 * buttons, most of the buttons are drawn with a texture behind them and a blank button (with no text)
 * over the top of it. This is to provide the same functionality of the button, but get around Unity's
 * restrictions of drawing buttons
 * 
 * 
 **/ 


using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class AtomTouchGUI : MonoBehaviour {
	
	//state of the UI
	private bool atomTouchActive = true;
	private bool toolbarActive = true;
	[HideInInspector]public bool dataPanelActive = false;
	private bool addAtomActive = true;
	private bool temperaturePanelActive = true;
	private bool volumePanelActive = true;
	private bool whiteCornerActive = false;
	private bool potentialsActive = false;
	private float oldTemperaure = -1;
	//plane materials
	public Material matPlane1;
	public Material matPlane1_5;
	public Material matPlane2;
	public Material matPlane2_5;
	public Material matPlane3;
	public Material matPlane3_5;
	public Material matPlane4;

	//textures for the UI
	public Texture lightBackground;
	public Texture darkBackground;
	public Texture darkBlueBackground;
	public Texture lightBlueBackground;
	public Texture whiteCornerArrow;
	public Texture downArrow;
	public Texture upArrow;
	//some references to the UI
	public GameObject hud;
	public GameObject timer;
	public GameObject tempSlider;//temperature
	public GameObject volSlider;//volume
	public GameObject bondLineBtn; 
	public GameObject selectAtomPanel;
	public GameObject selectAtomGroup;
	public GameObject settingsCanvas;
	public GameObject deselectButton;
	public GameObject graphPanel;
	public GameObject copperCount;
	public GameObject goldCount;
	public GameObject platinumCount;
	public GameObject numAtomInput;

	public GameObject cuBatchToggle;
	public GameObject auBatchToggle;
	public GameObject ptBatchToggle;

	public Text selectAllText;
	private bool selectedAll;
	private bool settingsActive;
	//sliders
	public GameObject tempFrontFill;
	public GameObject tempBackFill;
	public GameObject tempHandle;
	public GameObject volFrontFill;
	public GameObject volBackFill;
	public GameObject volHandle;

	//prefabs to spawn
	public Rigidbody copperPrefab;
	public Rigidbody goldPrefab;
	public Rigidbody platinumPrefab;
	
	//reset button
	public Texture resetButtonUp;
	public Texture resetButtonDown;
	private bool resetPressed;
	private float resetTime;
	
	//snap camera button
	public Texture cameraButtonUp;
	public Texture cameraButtonDown;
	private bool cameraPressed;
	private float cameraTime;
	
	//bond line button
	public Texture bondLineUp;
	public Texture bondLineDown;
	
	//atom kick button
	public Texture atomKickUp;
	public Texture atomKickDown;
	private bool atomKickPressed;
	private float atomKickTime;
	
	//time button
	public Texture normalTimeButton;
	public Texture slowTimeButton;
	public Texture stoppedTimeButton;
	
	//red x button
	public Texture redXButton;
	
	//add atom buttons
	public Texture copperTexture;
	public Texture goldTexture;
	public Texture platinumTexture;
	public Texture copperTextureAdd;
	public Texture goldTextureAdd;
	public Texture platinumTextureAdd;
	public Texture garbageTexture;
	public Texture garbageTextureDown;
	private bool garbagePressed;
	private float garbageTime;
	private bool clicked = false;
	private float startTime = 0.0f;
	private bool first = true;
	public float holdTime = 0.05f;
	[HideInInspector]public bool addGraphicCopper;
	[HideInInspector]public bool addGraphicGold;
	[HideInInspector]public bool addGraphicPlatinum;
	public GUISkin sliderControls;
	private bool firstPass = true;
	
	[HideInInspector]public bool changingTemp = false;
	[HideInInspector]public bool changingVol = false;
	private float guiVolume;
	
	private int slowMotionFrames;
	
	public static StaticVariables.TimeSpeed currentTimeSpeed = StaticVariables.TimeSpeed.Stopped;
	
	private Slider tempSliderComponent;
	private Slider volSliderComponent;

	[HideInInspector]public static AtomTouchGUI myAtomTouchGUI; 
	void Awake(){
		myAtomTouchGUI = this;

		tempSliderComponent = tempSlider.GetComponent<Slider> ();
		volSliderComponent = volSlider.GetComponent<Slider>();
		//set slider range
		tempSliderComponent.minValue = StaticVariables.minTemp;
		tempSliderComponent.maxValue = StaticVariables.maxTemp;

		volSliderComponent.minValue = StaticVariables.minVol * 0.1f; //to nm
		volSliderComponent.maxValue = StaticVariables.maxVol * 0.1f; //to nm
		
		Atom.EnableSelectAtomGroup(false);
		settingsCanvas.SetActive(false);
		selectedAll = false;
		settingsActive = false;

/*
		if(Application.platform == RuntimePlatform.IPhonePlayer){
		//if(1+1==2){
			//make sliders larger
			float tempFront = tempFrontFill.GetComponent<RectTransform>().localScale.y;
			float tempBack = tempBackFill.GetComponent<RectTransform>().localScale.y;
			float volFront = volFrontFill.GetComponent<RectTransform>().localScale.y;
			float volBack = volBackFill.GetComponent<RectTransform>().localScale.y;

			tempFrontFill.GetComponent<RectTransform>().localScale 
				= new Vector3(1.0f, 2.0f*tempFront,1.0f);
			tempBackFill.GetComponent<RectTransform>().localScale
				= new Vector3(1.0f, 2.0f*tempBack,1.0f);
			tempHandle.GetComponent<RectTransform>().localScale = new Vector3(1.5f,2.0f,1.0f);

			volFrontFill.GetComponent<RectTransform>().localScale 
				= new Vector3(1.0f, 2.0f*volFront,1.0f);
			volBackFill.GetComponent<RectTransform>().localScale
				= new Vector3(1.0f, 2.0f*volBack,1.0f);
			volHandle.GetComponent<RectTransform>().localScale = new Vector3(1.5f,2.0f,1.0f);
		}
		*/
		//Debug.Log("Settings canvas enabled: " + SettingsCanvas.activeSelf);
	}
	void Start () {
		CreateEnvironment myEnvironment = CreateEnvironment.myEnvironment;
		guiVolume = myEnvironment.volume;	

	}
	

	//this function displays properties that are apart of the system as a whole,
	// such as the number of atoms
	void DisplaySystemProperties(Rect displayRect){
		GUIStyle timeText = GUI.skin.label;
		timeText.alignment = TextAnchor.MiddleLeft;
		timeText.fontSize = 14;
		timeText.normal.textColor = Color.white;
		
		int totalAtoms = Atom.AllAtoms.Count;
		int copperAtoms = 0;
		int goldAtoms = 0;
		int platinumAtoms = 0;
		for (int i = 0; i < Atom.AllAtoms.Count; i++) {
			Atom currAtom = Atom.AllAtoms[i];
			if (currAtom.GetComponent<Copper> () != null) {
				copperAtoms++;
			}
			else if (currAtom.GetComponent<Gold> () != null) {
				goldAtoms++;
			}
			else if (currAtom.GetComponent<Platinum> () != null) {
				platinumAtoms++;
			}
		}
		GUI.Label (new Rect(displayRect.x + 10.0f, displayRect.y + 10.0f, 225, 30), "Total Atoms: " + totalAtoms);
		GUI.Label (new Rect(displayRect.x + 10.0f, displayRect.y + 40.0f, 225, 30), "Copper Atoms: " + copperAtoms);
		GUI.Label (new Rect(displayRect.x + 10.0f, displayRect.y + 70.0f, 225, 30), "Gold Atoms: " + goldAtoms);
		GUI.Label (new Rect(displayRect.x + 10.0f, displayRect.y + 100.0f, 225, 30), "Platinum Atoms: " + platinumAtoms);
	}
	
	//this function display the properties that are specific to one specific atom such as its position, velocity, and type
	void DisplayAtomProperties(Atom currAtom, Rect displayRect){
		
		GUIStyle timeText = GUI.skin.label;
		timeText.alignment = TextAnchor.MiddleLeft;
		timeText.fontSize = 14;
		timeText.normal.textColor = Color.white;
		
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
		
		GUI.Label (new Rect (displayRect.x + 10.0f, displayRect.y + 20.0f, 200, 30), "Element Name: " + elementName);
		GUI.Label (new Rect (displayRect.x + 10.0f, displayRect.y + 50.0f, 200, 30), "Element Symbol: " + elementSymbol);
		GUI.Label (new Rect (displayRect.x + 10.0f, displayRect.y + 80.0f, 200, 50), "Position: " + currAtom.transform.position.ToString("E0"));
		GUI.Label (new Rect (displayRect.x + 10.0f, displayRect.y + 130.0f, 200, 50), "Velocity: " + currAtom.transform.rigidbody.velocity.ToString("E0"));
		
		DisplayBondProperties (currAtom, displayRect);
		
	}
	
	//this function displays the angles of the bonds to other atoms
	void DisplayBondProperties(Atom currAtom, Rect displayRect){
		
		List<Vector3> bonds = new List<Vector3>();
		for (int i = 0; i < Atom.AllAtoms.Count; i++) {
			Atom atomNeighbor = Atom.AllAtoms[i];
			if(atomNeighbor.gameObject == currAtom.gameObject) continue;
			if(Vector3.Distance(currAtom.transform.position, atomNeighbor.transform.position) < currAtom.BondDistance(atomNeighbor)){
				bonds.Add(atomNeighbor.transform.position);
			}
		}
		
		//need more than 1 bond to form an angle
		if (bonds.Count > 1) {
			int angleNumber = 1;
			//to display the angles, we must compute the angles between every pair of bonds
			float displayWidth = 200.0f;
			for(int i = 0; i < bonds.Count; i++){
				for(int j = i+1; j < bonds.Count; j++){
					float xCoord;
					if(angleNumber < 7){
						xCoord = displayRect.x + displayWidth;
					}
					else if (angleNumber >= 7 && angleNumber < 13){
						xCoord = displayRect.x + (2*displayWidth);
					}
					else{
						xCoord = displayRect.x + (3*displayWidth);
					}
					if(angleNumber > 18){
						break;
					}
					Vector3 vector1 = (bonds[i] - currAtom.transform.position);
					Vector3 vector2 = (bonds[j] - currAtom.transform.position);
					float angle = (float)Math.Round(Vector3.Angle(vector1, vector2), 3);
					float angleNum = (angleNumber-1) % 6;
					//GUI.Label(new Rect(Screen.width - 285, 230 + (bonds.Count * 30) + ((angleNumber-1)*30), 225, 30), "Angle " + angleNumber + ": " + angle + " degrees");
					GUI.Label(new Rect(xCoord, displayRect.y + 10.0f + (angleNum*30.0f), displayWidth, 30), "Angle " + angleNumber + ": " + Math.Round(angle,1) + " degrees");
					angleNumber++;
				}
			}
		}
		
	}
	
	//this function selects all of the atoms in the scene
	void SelectAllAtoms(){
		for (int i = 0; i < Atom.AllAtoms.Count; i++) {
			Atom currAtom = Atom.AllAtoms[i];
			currAtom.selected = true;
			currAtom.SetSelected(true);
		}
	}
	
	//this function deselects all of the atoms in the scene
	void DeselectAllAtoms(){
		for (int i = 0; i < Atom.AllAtoms.Count; i++) {
			Atom currAtom = Atom.AllAtoms[i];
			currAtom.selected = false;
			currAtom.SetSelected(false);
		}
		Atom.EnableSelectAtomGroup(false);
	}



	public void DeleteSelectedAtoms(){
		Debug.Log("DeleteSelectedAtoms called");
		for(int i=Atom.AllAtoms.Count-1; i >= 0;i--){
			Atom currAtom = Atom.AllAtoms[i];
			if(currAtom.selected){
				//delete this atom from the list
				Debug.Log("deleting atom: " + i);
				currAtom.selected = false;
				currAtom.SetSelected(false);
				
				int instId = currAtom.gameObject.rigidbody.GetInstanceID();
				if(currAtom is Copper){
					Copper.count--;
					copperCount.GetComponent<Text>().text = "Cu: " + Copper.count;
				}else if (currAtom is Gold){
					Gold.count--;
					goldCount.GetComponent<Text>().text = "Au: " + Gold.count;
				}else if (currAtom is Platinum){
					Platinum.count --;
					platinumCount.GetComponent<Text>().text = "Pt: " + Platinum.count;
				}
				
				Atom.UnregisterAtom(currAtom);
				Destroy(currAtom.gameObject);
			}
		}
		Atom.EnableSelectAtomGroup(false);
	}
	//this function returns the number of atoms that are selected
	public int CountSelectedAtoms(){
		return NumberofAtom.selectedAtoms;
	}
	//this function checks the position of all of the atoms to make sure they are inside of the box
	void CheckAtomVolumePositions(){
		
		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment>();
		for (int i = 0; i < Atom.AllAtoms.Count; i++) {
			Atom currAtom = Atom.AllAtoms[i];
			Vector3 newPosition = currAtom.transform.position;
			if(currAtom.transform.position.x > CreateEnvironment.bottomPlane.transform.position.x + (createEnvironment.width/2.0f) - createEnvironment.errorBuffer){
				newPosition.x = CreateEnvironment.bottomPlane.transform.position.x + (createEnvironment.width/2.0f) - createEnvironment.errorBuffer;
			}
			if(currAtom.transform.position.x < CreateEnvironment.bottomPlane.transform.position.x - (createEnvironment.width/2.0f) + createEnvironment.errorBuffer){
				newPosition.x = CreateEnvironment.bottomPlane.transform.position.x - (createEnvironment.width/2.0f) + createEnvironment.errorBuffer;
			}
			if(currAtom.transform.position.y > CreateEnvironment.bottomPlane.transform.position.y + (createEnvironment.height) - createEnvironment.errorBuffer){
				newPosition.y = CreateEnvironment.bottomPlane.transform.position.y + (createEnvironment.height) - createEnvironment.errorBuffer;
			}
			if(currAtom.transform.position.y < CreateEnvironment.bottomPlane.transform.position.y + createEnvironment.errorBuffer){
				newPosition.y = CreateEnvironment.bottomPlane.transform.position.y + createEnvironment.errorBuffer;
			}
			if(currAtom.transform.position.z > CreateEnvironment.bottomPlane.transform.position.z + (createEnvironment.depth/2.0f) - createEnvironment.errorBuffer){
				newPosition.z = CreateEnvironment.bottomPlane.transform.position.z + (createEnvironment.depth/2.0f) - createEnvironment.errorBuffer;
			}
			if(currAtom.transform.position.z < CreateEnvironment.bottomPlane.transform.position.z - (createEnvironment.depth/2.0f) + createEnvironment.errorBuffer){
				newPosition.z = CreateEnvironment.bottomPlane.transform.position.z - (createEnvironment.depth/2.0f) + createEnvironment.errorBuffer;
			}
			currAtom.transform.position = newPosition;
		}
		
		
	}
	
	//this function changes the panels so they are in double clicked mode
	public void SetDoubleClicked(){
		dataPanelActive = true;
		atomTouchActive = false;
		toolbarActive = true;
		addAtomActive = false;
		temperaturePanelActive = false;
		volumePanelActive = false;
	}
	
	//this function disables the panels so it goes back to the default mode
	void RedXClicked(){
		dataPanelActive = false;
		atomTouchActive = true;
		toolbarActive = true;
		addAtomActive = true;
		temperaturePanelActive = true;
		volumePanelActive = true;
	}
	//kick all selected atoms or all if none is selected
	public void SelectedAtomsKick(){
		bool hasAtomSelected = false;
		for(int i = 0; i < Atom.AllAtoms.Count; i++){
			Atom currAtom = Atom.AllAtoms[i];
			if(currAtom.selected){
				hasAtomSelected = true;
				AtomKick(i);
			}
		}
		if(!hasAtomSelected)AllAtomsKick();
	}
	public void AllAtomsKick(){
		for(int i = 0; i < Atom.AllAtoms.Count; i++){
			//Atom currAtom = Atom.AllAtoms[i];
			AtomKick(i);
		}
	}

	//kick only one atom
	public void AtomKick(int i){
		Atom currAtom = Atom.AllAtoms[i];
		float xVelocity = 0.0f;
		float yVelocity = 0.0f;
		float zVelocity = 0.0f;
		//this is maximum random velocity.
		float maxVelocity = 2.0f*Mathf.Sqrt(3.0f*StaticVariables.kB*StaticVariables.desiredTemperature/currAtom.massamu/StaticVariables.amuToKg)/StaticVariables.angstromsToMeters;

		if(UnityEngine.Random.Range(0.0f, 1.0f) > .5f){
			xVelocity = UnityEngine.Random.Range(1.0f * maxVelocity, 5.0f * maxVelocity);
		}
		else{
			xVelocity = UnityEngine.Random.Range(-5.0f * maxVelocity, -1.0f * maxVelocity);
		}
		if(UnityEngine.Random.Range(0.0f, 1.0f) > .5f){
			yVelocity = UnityEngine.Random.Range(1.0f * maxVelocity, 5.0f * maxVelocity);
		}
		else{
			yVelocity = UnityEngine.Random.Range(-5.0f * maxVelocity, -1.0f * maxVelocity);
		}
		if(UnityEngine.Random.Range(0.0f, 1.0f) > .5f){
			zVelocity = UnityEngine.Random.Range(1.0f * maxVelocity, 5.0f * maxVelocity);
		}
		else{
			zVelocity = UnityEngine.Random.Range(-5.0f * maxVelocity, -1.0f * maxVelocity);
		}
		currAtom.velocity = new Vector3(xVelocity, yVelocity, zVelocity);
	}
	
	public static int IntParseFast(string value)
    {
		// An optimized int parse method.
		int result = 0;
		for (int i = 0; i < value.Length; i++)
		{
		    result = 10 * result + (value[i] - 48);
		}
		return result;
    }

	public void BatchCreateAtoms(){
		//sanity check 
		//0-20
		//TODO: detect selected toggle
		int numToCreate = IntParseFast(numAtomInput.GetComponent<Text>().text);
		if(numToCreate < 0 || numToCreate > 20)return; //too much

		if(cuBatchToggle.GetComponent<Toggle>().isOn){
			for(int i=0; i < numToCreate; i++) 
				CreateEnvironment.myEnvironment.createAtom(copperPrefab);
		}else if (auBatchToggle.GetComponent<Toggle>().isOn){
			for(int i=0;i<numToCreate;i++)
				CreateEnvironment.myEnvironment.createAtom(goldPrefab);
		}else if (ptBatchToggle.GetComponent<Toggle>().isOn){
			for(int i=0;i<numToCreate;i++)
				CreateEnvironment.myEnvironment.createAtom(platinumPrefab);
		}
	}

	public void BatchRemoveAtoms(){
		int numToRemove = IntParseFast(numAtomInput.GetComponent<Text>().text);
		if(numToRemove <= 0)return;
		if(cuBatchToggle.GetComponent<Toggle>().isOn){
			//TODO: error notice
			if(numToRemove > Copper.count)return;
			for(int i=0; i<Atom.AllAtoms.Count && numToRemove > 0;i++){
				Atom currAtom = Atom.AllAtoms[i];
				if(currAtom is Copper){
					Atom.UnregisterAtom(currAtom);
					Destroy(currAtom.gameObject);
					numToRemove--;
					Copper.count--;
				}
			}
		}else if (auBatchToggle.GetComponent<Toggle>().isOn){
			if(numToRemove > Gold.count)return;
			for(int i=0; i<Atom.AllAtoms.Count && numToRemove > 0;i++){
				Atom currAtom = Atom.AllAtoms[i];
				if(currAtom is Gold){
					Atom.UnregisterAtom(currAtom);
					Destroy(currAtom.gameObject);
					numToRemove--;
					Gold.count--;
				}
			}
			
		}else if (ptBatchToggle.GetComponent<Toggle>().isOn){
			if(numToRemove > Platinum.count)return;
			for(int i=0; i<Atom.AllAtoms.Count && numToRemove > 0;i++){
				Atom currAtom = Atom.AllAtoms[i];
				if(currAtom is Platinum){
					Atom.UnregisterAtom(currAtom);
					Destroy(currAtom.gameObject);
					numToRemove--;
					Platinum.count--;
				}
			}
		}

	}

	public void AddPlatinumAtom(){
		
		if(Input.mousePosition.x < Screen.width && Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height){
			//Vector3 curPosition = new Vector3 (createEnvironment.centerPos.x + (UnityEngine.Random.Range (-(createEnvironment.width / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.width / 2.0f) - createEnvironment.errorBuffer)), createEnvironment.centerPos.y + (UnityEngine.Random.Range (-(createEnvironment.height / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.height / 2.0f) - createEnvironment.errorBuffer)), createEnvironment.centerPos.z + (UnityEngine.Random.Range (-(createEnvironment.depth / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.depth / 2.0f) - createEnvironment.errorBuffer)));
			CreateEnvironment myEnvironment = CreateEnvironment.myEnvironment;
			Vector3 curPosition = new Vector3 (myEnvironment.centerPos.x - myEnvironment.width/2.0f+myEnvironment.errorBuffer, myEnvironment.centerPos.y - myEnvironment.height/2.0f+myEnvironment.errorBuffer, myEnvironment.centerPos.z - myEnvironment.depth/2.0f+myEnvironment.errorBuffer);
			Quaternion curRotation = Quaternion.Euler(0, 0, 0);
			//Instantiate(platinumPrefab, curPosition, curRotation);
			myEnvironment.createAtom(platinumPrefab);
			
		}
	
	}

	public void AddGoldAtom(){
		
		if(Input.mousePosition.x < Screen.width && Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height){
			//Vector3 curPosition = new Vector3 (createEnvironment.centerPos.x + (UnityEngine.Random.Range (-(createEnvironment.width / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.width / 2.0f) - createEnvironment.errorBuffer)), createEnvironment.centerPos.y + (UnityEngine.Random.Range (-(createEnvironment.height / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.height / 2.0f) - createEnvironment.errorBuffer)), createEnvironment.centerPos.z + (UnityEngine.Random.Range (-(createEnvironment.depth / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.depth / 2.0f) - createEnvironment.errorBuffer)));
			CreateEnvironment myEnvironment = CreateEnvironment.myEnvironment;
			Vector3 curPosition = new Vector3 (myEnvironment.centerPos.x - myEnvironment.width/2.0f+myEnvironment.errorBuffer, myEnvironment.centerPos.y - myEnvironment.height/2.0f+myEnvironment.errorBuffer, myEnvironment.centerPos.z - myEnvironment.depth/2.0f+myEnvironment.errorBuffer);
			Quaternion curRotation = Quaternion.Euler(0, 0, 0);
			//Instantiate(goldPrefab, curPosition, curRotation);
			myEnvironment.createAtom(goldPrefab);
			
		}

	}

	public void AddCopperAtom(){
		
		if(Input.mousePosition.x < Screen.width 
			&& Input.mousePosition.x > 0 && Input.mousePosition.y > 0 
			&& Input.mousePosition.y < Screen.height){
			//Vector3 curPosition = new Vector3 (createEnvironment.centerPos.x + (UnityEngine.Random.Range (-(createEnvironment.width / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.width / 2.0f) - createEnvironment.errorBuffer)), createEnvironment.centerPos.y + (UnityEngine.Random.Range (-(createEnvironment.height / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.height / 2.0f) - createEnvironment.errorBuffer)), createEnvironment.centerPos.z + (UnityEngine.Random.Range (-(createEnvironment.depth / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.depth / 2.0f) - createEnvironment.errorBuffer)));
			CreateEnvironment myEnvironment = CreateEnvironment.myEnvironment;
			Vector3 curPosition = new Vector3 (myEnvironment.centerPos.x - myEnvironment.width/2.0f+myEnvironment.errorBuffer, myEnvironment.centerPos.y - myEnvironment.height/2.0f+myEnvironment.errorBuffer, myEnvironment.centerPos.z - myEnvironment.depth/2.0f+myEnvironment.errorBuffer);
			Quaternion curRotation = Quaternion.Euler(0, 0, 0);
			//Instantiate(copperPrefab, curPosition, curRotation);
			myEnvironment.createAtom(copperPrefab);
			
		}

		
	

	}
	public void ResetAll(){
		CreateEnvironment myEnvironment = CreateEnvironment.myEnvironment;
		myEnvironment.InitAtoms ();
		slowMotionFrames = StaticVariables.slowMotionFrames;
		Atom.EnableSelectAtomGroup(false);
		//reset temp and vol
		tempSliderComponent.value = StaticVariables.tempRangeHigh - StaticVariables.tempDefault;
		volSliderComponent.value = StaticVariables.volRangeHigh - StaticVariables.volDefault;
		changingVol = false;
		changingTemp = false;
	}
	//for the left panel
	public void createBondline(){
		RawImage ri = bondLineBtn.GetComponent<RawImage>();
		Texture bondLine = StaticVariables.drawBondLines ? bondLineUp : bondLineDown;
		ri.texture = bondLine;
		StaticVariables.drawBondLines = !StaticVariables.drawBondLines;
	}

	//for volume slider
	//range: 1.5 - 4.5 nm 
	//step size: 0.5
	public void SnapVolumeToInterval(float stepSize){
		float rawVal = volSliderComponent.value;
		float floor = Mathf.Floor(rawVal / stepSize);
		if(!Mathf.Approximately(rawVal / stepSize, floor))
			volSliderComponent.value = floor * stepSize + stepSize;

	}

	public void changeTimer(){
		RawImage ri = timer.GetComponent<RawImage>();
		if(currentTimeSpeed == StaticVariables.TimeSpeed.Normal){
			currentTimeSpeed = StaticVariables.TimeSpeed.Stopped;
			StaticVariables.pauseTime = true;
			StaticVariables.MDTimestep = StaticVariables.MDTimestepStop;
			StaticVariables.MDTimestepSqr = StaticVariables.MDTimestep * StaticVariables.MDTimestep;


			ri.texture = stoppedTimeButton;

		}
		else if(currentTimeSpeed == StaticVariables.TimeSpeed.Stopped){
			currentTimeSpeed = StaticVariables.TimeSpeed.SlowMotion;

			StaticVariables.MDTimestep = StaticVariables.MDTimestepSlow;
			StaticVariables.MDTimestepSqr = StaticVariables.MDTimestep * StaticVariables.MDTimestep;

			StaticVariables.pauseTime = false;

			ri.texture = slowTimeButton;
		}
		else if(currentTimeSpeed == StaticVariables.TimeSpeed.SlowMotion){
			currentTimeSpeed = StaticVariables.TimeSpeed.Normal;

			StaticVariables.MDTimestep = StaticVariables.MDTimestepNormal;
			StaticVariables.MDTimestepSqr = StaticVariables.MDTimestep * StaticVariables.MDTimestep;

			StaticVariables.pauseTime = false;
			ri.texture = normalTimeButton;
		}	
	}


	
	
	public void resetCamera(){
		Camera.main.transform.position = new Vector3(0.0f, 0.0f, -40.0f);
		Camera.main.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
	}

	public void ToggleSelectAll() {
		if(NumberofAtom.selectedAtoms == Atom.AllAtoms.Count){
			DeselectAllAtoms();
		}else{
			SelectAllAtoms();
		}
		/*
		selectedAll = !selectedAll;
		if(selectedAll)
		{
			SelectAllAtoms();
		}
		else
		{
			DeselectAllAtoms();
		}
		*/

	}

	public void deselectAll(){
		DeselectAllAtoms ();
	}
	public void ChangeAtomVolume(){
		
		CreateEnvironment createEnvironment = CreateEnvironment.myEnvironment;
		//these are in angstroms
		float offset = StaticVariables.maxVol + StaticVariables.minVol;
		createEnvironment.width = Math.Abs(offset -volSliderComponent.value*10.0f);
		createEnvironment.height = Math.Abs(offset -volSliderComponent.value*10.0f);
		createEnvironment.depth = Math.Abs(offset - volSliderComponent.value*10.0f);
		createEnvironment.volume = 
			createEnvironment.width*
			createEnvironment.height*
			createEnvironment.depth; //to nm^3
		//since slider is upside down...
		float realVol = createEnvironment.width * 0.1f;
		ChangePlaneMaterial(realVol);
		changingVol = true;
	}

	public void ChangePlaneMaterial(float realVol){

		MeshRenderer topMesh = CreateEnvironment.topPlane.GetComponent<MeshRenderer>();
		MeshRenderer backMesh = CreateEnvironment.backPlane.GetComponent<MeshRenderer>();
		MeshRenderer frontMesh = CreateEnvironment.frontPlane.GetComponent<MeshRenderer>();
		MeshRenderer leftMesh = CreateEnvironment.leftPlane.GetComponent<MeshRenderer>();
		MeshRenderer rightMesh = CreateEnvironment.rightPlane.GetComponent<MeshRenderer>();
		MeshRenderer bottomMesh = CreateEnvironment.bottomPlane.GetComponent<MeshRenderer>();

		if(Mathf.Approximately(realVol, 1.0f)){
			topMesh.material = matPlane1;
			backMesh.material = matPlane1;
			frontMesh.material = matPlane1;
			leftMesh.material = matPlane1;
			rightMesh.material = matPlane1;
			bottomMesh.material = matPlane1;
		}
		else if(Mathf.Approximately(realVol, 1.5f)){
			topMesh.material = matPlane1_5;
			backMesh.material = matPlane1_5;
			frontMesh.material = matPlane1_5;
			leftMesh.material = matPlane1_5;
			rightMesh.material = matPlane1_5;
			bottomMesh.material = matPlane1_5;
		}
		else if(Mathf.Approximately(realVol,2.0f)){
			topMesh.material = matPlane2;
			backMesh.material = matPlane2;
			frontMesh.material = matPlane2;
			leftMesh.material = matPlane2;
			rightMesh.material = matPlane2;
			bottomMesh.material = matPlane2;
		}
		else if(Mathf.Approximately(realVol,2.5f)){
			topMesh.material = matPlane2_5;
			backMesh.material = matPlane2_5;
			frontMesh.material = matPlane2_5;
			leftMesh.material = matPlane2_5;
			rightMesh.material = matPlane2_5;
			bottomMesh.material = matPlane2_5;
		}
		else if(Mathf.Approximately(realVol,3.0f)){
			topMesh.material = matPlane3;
			backMesh.material = matPlane3;
			frontMesh.material = matPlane3;
			leftMesh.material = matPlane3;
			rightMesh.material = matPlane3;
			bottomMesh.material = matPlane3;
		}
		else if(Mathf.Approximately(realVol,3.5f)){
			topMesh.material = matPlane3_5;
			backMesh.material = matPlane3_5;
			frontMesh.material = matPlane3_5;
			leftMesh.material = matPlane3_5;
			rightMesh.material = matPlane3_5;
			bottomMesh.material = matPlane3_5;
		}
		else if(Mathf.Approximately(realVol,4.0f)){
			topMesh.material = matPlane4;
			backMesh.material = matPlane4;
			frontMesh.material = matPlane4;
			leftMesh.material = matPlane4;
			rightMesh.material = matPlane4;
			bottomMesh.material = matPlane4;
		}
	}
	//check if all of the atoms are static
	public bool CheckAllAtomsStatic(){
		for(int i=0;i<Atom.AllAtoms.Count;i++){
			Atom currentAtom = Atom.AllAtoms[i];
			Vector3 atomVel = currentAtom.rigidbody.velocity;
			float diff = Vector3.Distance(atomVel, Vector3.zero);
			if(!Mathf.Approximately(diff, 0.0f)){
				return false;
			}
		}
		return true;
	}

	public void ChangeTimeScaleWithTemperature(float oldTemp){
		float ratio = (StaticVariables.maxTimeScale-StaticVariables.baseTimeScale)
						/(StaticVariables.maxTemp - StaticVariables.minTemp);
		float tempChange = StaticVariables.desiredTemperature - StaticVariables.minTemp;
		Time.timeScale = StaticVariables.baseTimeScale + ratio * tempChange;

		//0.003, 0.03 at 300k
		//0.01, 0.05 at 5000k
		float timestepRatio = (0.01f-0.003f)/(5000.0f-300.0f);
		Time.fixedDeltaTime = 0.003f + timestepRatio * tempChange;
		float maxTimeRatio = (0.05f-0.03f)/(5000.0f-300.0f);
		Time.maximumDeltaTime = 0.03f + maxTimeRatio*tempChange;
		
	}
	//3 / 4700
	//base Normal timeScale = 1.0f
	//timeScale = baseTimeScale + (maxTimeScale-baseTimeScale)/(maxTemp-minTemp) * (desiredTemp-currTemp)
	
	public void ChangeAtomTemperature(){
		oldTemperaure = StaticVariables.desiredTemperature;
		StaticVariables.desiredTemperature 
		= Math.Abs(StaticVariables.maxTemp - tempSliderComponent.value)*StaticVariables.tempScaler;
		//ChangeTimeScaleWithTemperature(oldTemperaure);
		//Debug.Log("temp changing");
		if(oldTemperaure < 0){
			return;
		}else if(Mathf.Approximately(oldTemperaure, 0.0f)){
			//if all atoms are static, kick all
			if(CheckAllAtomsStatic()){
				AllAtomsKick();
			}
		}
		changingTemp = true;
		
	}
	public void OnPointerUp_TempSlider(){
		changingTemp = false;
	}
	public void OnPointerUp_VolSlider(){
		changingVol= false;
	}
	

	/*
	public void displayInfo(){

			
		//Boolean toDisplay;
		if (toDisplay == true) {
			GameObject atomDisplay = GameObject.FindGameObjectWithTag("atomInfo");
			toDisplay = false;			
			atomDisplay.SetActive(false);
	
						
		} 
		else {

			//str.text ="Atom info";
			GameObject atomDisplay = GameObject.FindGameObjectWithTag("atomInfo");




		
			

		}
		
	
	}
	*/

}
