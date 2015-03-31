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
using UnityEngine.EventSystems;

public class AtomTouchGUI : MonoBehaviour {
	
	
	private float oldTemperaure = -1;
	//plane materials
	public Material matPlane1;
	public Material matPlane1_5;
	public Material matPlane2;
	public Material matPlane2_5;
	public Material matPlane3;
	public Material matPlane3_5;
	public Material matPlane4;

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
	public GameObject buttonPanel;

	public GameObject cuBatchToggle;
	public GameObject auBatchToggle;
	public GameObject ptBatchToggle;
	//add atom buttons
	public GameObject AddCopperBtn;
	public GameObject AddGoldBtn;
	public GameObject AddPlatBtn;
	public GameObject AddSodiumBtn;
	public GameObject AddChlorineBtn;
	//ADD ATOM button text
	public GameObject copperText;
	public GameObject goldText;
	public GameObject platText;
	public GameObject sodiumText;
	public GameObject chlorineText;

	public Text selectAllText;
	private bool selectedAll;
	private bool settingsActive;

	//prefabs to spawn
	public Rigidbody copperPrefab;
	public Rigidbody goldPrefab;
	public Rigidbody platinumPrefab;
	public Rigidbody sodiumPrefab;
	public Rigidbody chlorinePrefab;
	
	
	
	//time button
	public Texture normalTimeButton;
	public Texture slowTimeButton;
	public Texture stoppedTimeButton;
	
	

	private bool clicked = false;
	private float startTime = 0.0f;
	private bool first = true;

	
	[HideInInspector]public bool changingTemp = false;
	[HideInInspector]public bool changingVol = false;
	public EventSystem eventSystem;
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
		eventSystem = GameObject.Find("EventSystem").gameObject.GetComponent<EventSystem>();

	}
	void Start () {
		CreateEnvironment myEnvironment = CreateEnvironment.myEnvironment;

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
				
				int instId = currAtom.gameObject.GetComponent<Rigidbody>().GetInstanceID();
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
		AtomTouchGUI.myAtomTouchGUI.TryEnableAddAtomBtns();
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

	public void SetAtomBtnsVisibility(){
		if(Potential.currentPotential == Potential.potentialType.Buckingham){
			AddCopperBtn.SetActive(false);
			AddGoldBtn.SetActive(false);
			AddPlatBtn.SetActive(false);
			AddSodiumBtn.SetActive(true);
			AddChlorineBtn.SetActive(true);
		}else if(Potential.currentPotential == Potential.potentialType.LennardJones){
			AddCopperBtn.SetActive(true);
			AddGoldBtn.SetActive(true);
			AddPlatBtn.SetActive(true);
			AddSodiumBtn.SetActive(false);
			AddChlorineBtn.SetActive(false);
		}
	}
	public void TryEnableAddAtomBtns(){
		bool tooMuch = Atom.AllAtoms.Count >= StaticVariables.maxAtoms;
		
		AddCopperBtn.GetComponent<Button>().interactable = !tooMuch;
		AddGoldBtn.GetComponent<Button>().interactable = !tooMuch;
		AddPlatBtn.GetComponent<Button>().interactable = !tooMuch;
		AddSodiumBtn.GetComponent<Button>().interactable = !tooMuch;
		AddChlorineBtn.GetComponent<Button>().interactable = !tooMuch;

		Text cuText = copperText.GetComponent<Text>();
		Text auText = goldText.GetComponent<Text>();
		Text ptText = platText.GetComponent<Text>();
		Text naText = sodiumText.GetComponent<Text>();
		Text clText = chlorineText.GetComponent<Text>();

		if(tooMuch){
			cuText.color = StaticVariables.atomDisabledColor;
			auText.color = StaticVariables.atomDisabledColor;
			ptText.color = StaticVariables.atomDisabledColor;
			naText.color = StaticVariables.atomDisabledColor;
			clText.color = StaticVariables.atomDisabledColor;

		}else{
			cuText.color = StaticVariables.atomEnabledColor;
			auText.color = StaticVariables.atomEnabledColor;
			ptText.color = StaticVariables.atomEnabledColor;
			naText.color = StaticVariables.atomEnabledColor;
			clText.color = StaticVariables.atomEnabledColor;
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
		TryEnableAddAtomBtns();
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
		TryEnableAddAtomBtns();
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
		TryEnableAddAtomBtns();

	}
	public void AddSodiumAtom(){
		
		if(Input.mousePosition.x < Screen.width 
			&& Input.mousePosition.x > 0 && Input.mousePosition.y > 0 
			&& Input.mousePosition.y < Screen.height){
			//Vector3 curPosition = new Vector3 (createEnvironment.centerPos.x + (UnityEngine.Random.Range (-(createEnvironment.width / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.width / 2.0f) - createEnvironment.errorBuffer)), createEnvironment.centerPos.y + (UnityEngine.Random.Range (-(createEnvironment.height / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.height / 2.0f) - createEnvironment.errorBuffer)), createEnvironment.centerPos.z + (UnityEngine.Random.Range (-(createEnvironment.depth / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.depth / 2.0f) - createEnvironment.errorBuffer)));
			CreateEnvironment myEnvironment = CreateEnvironment.myEnvironment;
			Vector3 curPosition = new Vector3 (myEnvironment.centerPos.x - myEnvironment.width/2.0f+myEnvironment.errorBuffer, myEnvironment.centerPos.y - myEnvironment.height/2.0f+myEnvironment.errorBuffer, myEnvironment.centerPos.z - myEnvironment.depth/2.0f+myEnvironment.errorBuffer);
			Quaternion curRotation = Quaternion.Euler(0, 0, 0);
			//Instantiate(copperPrefab, curPosition, curRotation);
			myEnvironment.createAtom(sodiumPrefab);
			
		}
		TryEnableAddAtomBtns();

	}
	public void AddChlorineAtom(){
		
		if(Input.mousePosition.x < Screen.width 
			&& Input.mousePosition.x > 0 && Input.mousePosition.y > 0 
			&& Input.mousePosition.y < Screen.height){
			//Vector3 curPosition = new Vector3 (createEnvironment.centerPos.x + (UnityEngine.Random.Range (-(createEnvironment.width / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.width / 2.0f) - createEnvironment.errorBuffer)), createEnvironment.centerPos.y + (UnityEngine.Random.Range (-(createEnvironment.height / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.height / 2.0f) - createEnvironment.errorBuffer)), createEnvironment.centerPos.z + (UnityEngine.Random.Range (-(createEnvironment.depth / 2.0f) + createEnvironment.errorBuffer, (createEnvironment.depth / 2.0f) - createEnvironment.errorBuffer)));
			CreateEnvironment myEnvironment = CreateEnvironment.myEnvironment;
			Vector3 curPosition = new Vector3 (myEnvironment.centerPos.x - myEnvironment.width/2.0f+myEnvironment.errorBuffer, myEnvironment.centerPos.y - myEnvironment.height/2.0f+myEnvironment.errorBuffer, myEnvironment.centerPos.z - myEnvironment.depth/2.0f+myEnvironment.errorBuffer);
			Quaternion curRotation = Quaternion.Euler(0, 0, 0);
			//Instantiate(copperPrefab, curPosition, curRotation);
			myEnvironment.createAtom(chlorinePrefab);
			
		}
		TryEnableAddAtomBtns();

	}
	public void ResetAll(){
		
		CreateEnvironment myEnvironment = CreateEnvironment.myEnvironment;
		myEnvironment.InitAtoms ();
		slowMotionFrames = StaticVariables.slowMotionFrames;
		Atom.EnableSelectAtomGroup(false);
		//reset temp and vol
		tempSliderComponent.value = StaticVariables.tempRangeHigh - StaticVariables.tempDefault;
		volSliderComponent.value = StaticVariables.volRangeHigh - StaticVariables.volDefault;

		SnapTempToInterval(10.0f);
		SnapVolumeToInterval(0.5f);

		//ChangeTimeScaleWithTemperature(oldTemperaure);
		
		changingVol = false;
		changingTemp = false;
		//SettingsControl.renderAtoms = true;
	}
	

	public void SnapTempToInterval(float stepSize){
		changingTemp = true;
		float rawVal = tempSliderComponent.value;
		float floor = Mathf.Floor(rawVal / stepSize);
		if(!Mathf.Approximately(rawVal / stepSize, floor))
			tempSliderComponent.value = floor * stepSize + stepSize;

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
			Vector3 atomVel = currentAtom.GetComponent<Rigidbody>().velocity;
			float diff = Vector3.Distance(atomVel, Vector3.zero);
			if(!Mathf.Approximately(diff, 0.0f)){
				return false;
			}
		}
		return true;
	}
	
	public void ChangeTimeScaleWithTemperature(float oldTemp){
		if(Potential.currentPotential == Potential.potentialType.LennardJones){

			float ratio = (StaticVariables.maxTimeScale-StaticVariables.baseTimeScale)
						/(StaticVariables.maxTemp - StaticVariables.defaultTemp);
			float tempChange = StaticVariables.desiredTemperature - StaticVariables.defaultTemp;
			if(tempChange < 0)return;
			Time.timeScale = StaticVariables.baseTimeScale + ratio * tempChange;

		}else if(Potential.currentPotential == Potential.potentialType.Buckingham){
			float ratio = (StaticVariables.maxTimeScaleBuck-StaticVariables.baseTimeScaleBuck)
						/(StaticVariables.maxTemp - StaticVariables.defaultTemp);
			float tempChange = StaticVariables.desiredTemperature - StaticVariables.defaultTemp;
			if(tempChange < 0)return;
			Time.timeScale = StaticVariables.baseTimeScaleBuck + ratio * tempChange;
		}
		

		//0.003, 0.03 at 300k
		//0.01, 0.05 at 5000k
		float timestepRatio = (0.01f-0.003f)/(5000.0f-300.0f);
		//Time.fixedDeltaTime = 0.003f + timestepRatio * tempChange;
		float maxTimeRatio = (0.05f-0.03f)/(5000.0f-300.0f);
		//Time.maximumDeltaTime = 0.03f + maxTimeRatio*tempChange;
		
	}
	//3 / 4700
	//base Normal timeScale = 1.0f
	//timeScale = baseTimeScale + (maxTimeScale-baseTimeScale)/(maxTemp-minTemp) * (desiredTemp-currTemp)
	
	public void ChangeAtomTemperature(){
		oldTemperaure = StaticVariables.desiredTemperature;
		StaticVariables.desiredTemperature 
		= Math.Abs(StaticVariables.maxTemp - tempSliderComponent.value)*StaticVariables.tempScaler;
		ChangeTimeScaleWithTemperature(oldTemperaure);
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


}
