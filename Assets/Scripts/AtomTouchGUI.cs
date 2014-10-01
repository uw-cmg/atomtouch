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

	//textures for the UI
	public Texture lightBackground;
	public Texture darkBackground;
	public Texture darkBlueBackground;
	public Texture lightBlueBackground;
	public Texture whiteCornerArrow;
	public Texture downArrow;
	public Texture upArrow;

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

	[HideInInspector]public bool changingSlider = false;
	private float guiVolume;

	public static StaticVariables.TimeSpeed currentTimeSpeed = StaticVariables.TimeSpeed.Normal;

	void Start () {
		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment> ();
		guiVolume = createEnvironment.volume;
	}

	//this function creates all UI elements in the game EXCEPT for the graph
	void OnGUI(){

		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment> ();

		if (sliderControls != null) {
			GUI.skin = sliderControls;
		}

		//create the "Atomtouch" menu
		GUIStyle buttonStyle = GUI.skin.label;
		Rect arrowBackgroundRect = new Rect (0.0f, 0.0f, Screen.width * .14f, Screen.height * .13f * .3f);
		Texture atomTouchArrow = atomTouchActive ? upArrow : downArrow;
		GUI.DrawTexture (arrowBackgroundRect, darkBackground);
		GUI.DrawTexture (new Rect (arrowBackgroundRect.width * .45f, 0.0f, 20.0f, arrowBackgroundRect.height), atomTouchArrow); 
		if (GUI.Button (arrowBackgroundRect, "", buttonStyle)) {
			atomTouchActive = !atomTouchActive;
		}

		Rect atomTouchRect = new Rect (0.0f, Screen.height * .13f * .3f, Screen.width * .14f, (Screen.height * .13f) - Screen.height * .13f * .3f);
		if (atomTouchActive) {
			GUIStyle textStyle = GUI.skin.label;
			textStyle.alignment = TextAnchor.MiddleCenter;
			textStyle.fontSize = 25;
			textStyle.normal.textColor = Color.white;
			GUI.DrawTexture (atomTouchRect, lightBackground);
			if (GUI.Button (atomTouchRect, "Atom Touch", textStyle)) {
				potentialsActive = !potentialsActive;
			}
			GUI.DrawTexture(new Rect(atomTouchRect.x + atomTouchRect.width - 20.0f, atomTouchRect.y + atomTouchRect.height - 20.0f, 20.0f, 20.0f), whiteCornerArrow);

			//create the dropdown of different potentials
			if(potentialsActive){
				GUIStyle potentialText = GUI.skin.label;
				potentialText.alignment = TextAnchor.MiddleCenter;
				potentialText.fontSize = 20;
				potentialText.normal.textColor = Color.white;
				Rect lennardJonesRect = new Rect(atomTouchRect.x, atomTouchRect.y + atomTouchRect.height, atomTouchRect.width, atomTouchRect.height * .75f);
				GUI.DrawTexture(lennardJonesRect, darkBackground);
				if(GUI.Button(lennardJonesRect, "Lennard-Jones", buttonStyle)){
					potentialsActive = false;
					StaticVariables.currentPotential = StaticVariables.Potential.LennardJones;
					createEnvironment.InitAtoms();
				}
				Rect buckinghamRect = new Rect(lennardJonesRect.x, lennardJonesRect.y + lennardJonesRect.height, lennardJonesRect.width, lennardJonesRect.height);
				GUI.DrawTexture(buckinghamRect, lightBackground);
				if(GUI.Button(buckinghamRect, "Buckingham", buttonStyle)){
					potentialsActive = false;
					StaticVariables.currentPotential = StaticVariables.Potential.Buckingham;
					createEnvironment.InitAtoms();
				}
				Rect brennerRect = new Rect(buckinghamRect.x, buckinghamRect.y + buckinghamRect.height, buckinghamRect.width, buckinghamRect.height);
				GUI.DrawTexture(brennerRect, darkBackground);
				if(GUI.Button(brennerRect, "Brenner", buttonStyle)){
					potentialsActive = false;
					StaticVariables.currentPotential = StaticVariables.Potential.Brenner;
					createEnvironment.InitAtoms();
				}
			}

		}

		GUIStyle currStyle = GUI.skin.label;
		currStyle.alignment = TextAnchor.MiddleCenter;
		currStyle.fontSize = 25;
		currStyle.normal.textColor = Color.white;

		Rect arrowBackgroundRectToolbar = new Rect (Screen.width * .14f + 5.0f, 0.0f, Screen.width * .28f, Screen.height * .13f * .3f);
		Texture toolbarArrow = toolbarActive ? upArrow : downArrow;
		GUI.DrawTexture (arrowBackgroundRectToolbar, darkBackground);
		GUI.DrawTexture (new Rect (arrowBackgroundRectToolbar.x + (arrowBackgroundRectToolbar.width*.5f), 0.0f, 20.0f, arrowBackgroundRectToolbar.height), toolbarArrow); 
		if (GUI.Button (arrowBackgroundRectToolbar, "", buttonStyle)) {
			toolbarActive = !toolbarActive;
		}

		Rect panelArrowRect = new Rect (Screen.width * .5f, Screen.height - (Screen.height * .13f * .3f), 20.0f, Screen.height * .13f * .3f);
		Rect panelRect = new Rect (0.0f, Screen.height - (Screen.height * .27f), Screen.width, (Screen.height * .27f));
		Rect openPanelRect = new Rect(0.0f, panelRect.y, (Screen.width * .6f) + 10.0f, panelRect.height);
		Rect bottomRect = new Rect(panelRect.x + openPanelRect.width, panelArrowRect.y, Screen.width - openPanelRect.width, panelArrowRect.height);

		//specify the graph's coordinates
		Graph graph = Camera.main.GetComponent<Graph>();
		graph.xCoord = bottomRect.x;
		graph.yCoord = Screen.height - bottomRect.y;
		graph.width = bottomRect.width;
		graph.height = openPanelRect.height - bottomRect.height;
		if (firstPass) {
			graph.RecomputeMaxDataPoints();
			firstPass = false;
		}

		//create the data panel
		if (dataPanelActive) {
			GUI.DrawTexture(openPanelRect, lightBackground);
			GUI.DrawTexture(panelArrowRect, downArrow);
			GUI.DrawTexture(bottomRect, lightBackground);
			if(GUI.Button(bottomRect, "", buttonStyle)){
				dataPanelActive = !dataPanelActive;
			}
			float buffer = 10.0f;

			bool doubleTapped = false;
			for(int i = 0; i < allMolecules.Length; i++){
				GameObject currAtom = allMolecules[i];
				Atom atomScript = currAtom.GetComponent<Atom>();
				if(atomScript.doubleTapped){
					doubleTapped = true;
				}
			}

			if(!doubleTapped){
				DisplaySystemProperties(openPanelRect);
			}
		}
		else{
			panelRect = new Rect(0.0f, panelArrowRect.y, Screen.width, panelArrowRect.height);
			openPanelRect = new Rect(0.0f, panelRect.y, (Screen.width * .6f) + 10.0f, panelRect.height);
			GUI.DrawTexture(panelRect, lightBackground);
			GUI.DrawTexture(panelArrowRect, upArrow);
		}
		if (GUI.Button (new Rect (0.0f, panelArrowRect.y, Screen.width, panelArrowRect.height), "", buttonStyle)) {
			dataPanelActive = !dataPanelActive;
		}

		//create the toolbar options menu (i.e atomkick, reset camera, etc)
		GUIStyle toolBarButtonStyle = GUI.skin.label;
		toolBarButtonStyle.alignment = TextAnchor.MiddleCenter;
		toolBarButtonStyle.fontSize = 25;
		toolBarButtonStyle.normal.textColor = Color.white;
		
		Rect toolbarRect = new Rect(arrowBackgroundRectToolbar.x, arrowBackgroundRectToolbar.height, arrowBackgroundRectToolbar.width, atomTouchRect.height);
		if (toolbarActive) {
			GUI.DrawTexture(toolbarRect, lightBackground);

			//create reset button
			Texture reset = resetPressed ? resetButtonDown : resetButtonUp;
			if(GUI.Button(new Rect(toolbarRect.x, toolbarRect.y, toolbarRect.width / 6.0f, toolbarRect.height), reset, buttonStyle)){
				resetPressed = true;
				resetTime = Time.realtimeSinceStartup;
				createEnvironment.InitAtoms();
			}
			if(Time.realtimeSinceStartup - resetTime > .05f){
				resetPressed = false;
			}

			//create camera button
			Texture camera = cameraPressed ? cameraButtonDown : cameraButtonUp;
			if(GUI.Button(new Rect(toolbarRect.x + (toolbarRect.width / 6.0f), toolbarRect.y, toolbarRect.width / 6.0f, toolbarRect.height), camera, buttonStyle)){
				cameraPressed = true;
				cameraTime = Time.realtimeSinceStartup;
				Camera.main.transform.position = new Vector3(0.0f, 0.0f, -40.0f);
				Camera.main.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
			}
			if(Time.realtimeSinceStartup - cameraTime > .05f){
				cameraPressed = false;
			}

			//create bond line button
			Texture bondLine = StaticVariables.drawBondLines ? bondLineUp : bondLineDown;
			if(GUI.Button(new Rect(toolbarRect.x + 2*(toolbarRect.width / 6.0f), toolbarRect.y, toolbarRect.width / 6.0f, toolbarRect.height), bondLine, buttonStyle)){
				StaticVariables.drawBondLines = !StaticVariables.drawBondLines;
			}

			//create atom kick button
			Texture atomKick = atomKickPressed ? atomKickDown : atomKickUp;
			if(GUI.Button(new Rect(toolbarRect.x + 3*(toolbarRect.width / 6.0f), toolbarRect.y, toolbarRect.width / 6.0f, toolbarRect.height), atomKick, buttonStyle)){
				atomKickPressed = true;
				atomKickTime = Time.realtimeSinceStartup;
				AtomKick();
			}
			if(Time.realtimeSinceStartup - atomKickTime > .05f){
				atomKickPressed = false;
			}

			//create the current time button
			Texture timeTexture = normalTimeButton;
			if(currentTimeSpeed == StaticVariables.TimeSpeed.Normal){
				timeTexture = normalTimeButton;
				Time.timeScale = 1.0f;
				MotionBlur blur = Camera.main.GetComponent<MotionBlur>();
				blur.blurAmount = 0.0f;
			}
			else if(currentTimeSpeed == StaticVariables.TimeSpeed.SlowMotion){
				timeTexture = slowTimeButton;
				Time.timeScale = .05f;
				MotionBlur blur = Camera.main.GetComponent<MotionBlur>();
				blur.blurAmount = 0.68f;

			}
			else if(currentTimeSpeed == StaticVariables.TimeSpeed.Stopped){
				timeTexture = stoppedTimeButton;
			}

			if(GUI.Button(new Rect(toolbarRect.x + 4*(toolbarRect.width / 6.0f), toolbarRect.y, toolbarRect.width / 6.0f, toolbarRect.height), timeTexture, buttonStyle)){
				if(currentTimeSpeed == StaticVariables.TimeSpeed.Normal){
					currentTimeSpeed = StaticVariables.TimeSpeed.Stopped;
					StaticVariables.pauseTime = true;
				}
				else if(currentTimeSpeed == StaticVariables.TimeSpeed.Stopped){
					currentTimeSpeed = StaticVariables.TimeSpeed.SlowMotion;
					StaticVariables.pauseTime = false;
				}
				else if(currentTimeSpeed == StaticVariables.TimeSpeed.SlowMotion){
					currentTimeSpeed = StaticVariables.TimeSpeed.Normal;
				}
			}

			//create the red x if the user double tapped an atom
			for (int i = 0; i < allMolecules.Length; i++) {
				Atom atomScript = allMolecules[i].GetComponent<Atom>();
				if(atomScript.doubleTapped){
					if(GUI.Button(new Rect(toolbarRect.x + 5*(toolbarRect.width / 6.0f), toolbarRect.y, toolbarRect.width / 6.0f, toolbarRect.height), redXButton, buttonStyle)){
						createEnvironment.centerPos = new Vector3(0.0f, 0.0f, 0.0f);
						atomScript.doubleTapped = false;
						Camera.main.transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
						currentTimeSpeed = StaticVariables.TimeSpeed.Normal;
						atomScript.RemoveBondText();
						atomScript.ResetTransparency();
						RedXClicked();
					}
			
					DisplayAtomProperties(allMolecules[i], openPanelRect);
			
				}
			}
		}



		//create the add atom toolbar
		Rect addAtomRect = new Rect (panelRect.x, panelRect.y - panelArrowRect.height, Screen.width * .2f, panelArrowRect.height);
		GUI.DrawTexture (addAtomRect, darkBackground);
		Texture addAtom = addAtomActive ? downArrow : upArrow;
		GUI.DrawTexture (new Rect (addAtomRect.width * .5f, addAtomRect.y, 20.0f, addAtomRect.height), addAtom);
		if (GUI.Button (addAtomRect, "", buttonStyle)) {
			addAtomActive = !addAtomActive;
		}

		Rect lightAddAtom = new Rect(addAtomRect.x, addAtomRect.y - (Screen.height*.1f), addAtomRect.width, (Screen.height*.1f));
		if (addAtomActive) {
			GUI.DrawTexture(lightAddAtom, lightBackground);

			//create the copper button
			if(GUI.RepeatButton(new Rect(lightAddAtom.x + 5.0f, lightAddAtom.y, addAtomRect.width / 4.0f, lightAddAtom.height), copperTextureAdd, buttonStyle)){
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
			//create the gold button
			if(GUI.RepeatButton(new Rect(lightAddAtom.x + 5.0f+(addAtomRect.width / 4.0f), lightAddAtom.y, addAtomRect.width / 4.0f, lightAddAtom.height), goldTextureAdd, buttonStyle)){
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
			//create the platinum button
			if(GUI.RepeatButton(new Rect(lightAddAtom.x + 5.0f+(2*(addAtomRect.width / 4.0f)), lightAddAtom.y, addAtomRect.width / 4.0f, lightAddAtom.height), platinumTextureAdd, buttonStyle)){
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

			//create the garbage button for deleting atoms
			Texture garbage = garbagePressed ? garbageTextureDown : garbageTexture;
			if(GUI.Button(new Rect(lightAddAtom.x + 5.0f+(3*(addAtomRect.width / 4.0f)), lightAddAtom.y, addAtomRect.width / 4.0f, lightAddAtom.height), garbage, buttonStyle)){
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
					if(atomScript.selected && atomScript.doubleTapped){
						currentTimeSpeed = StaticVariables.TimeSpeed.Normal;
						atomScript.RemoveBondText();
						atomScript.ResetTransparency();
					}
				}
				garbagePressed = true;
				garbageTime = Time.realtimeSinceStartup;
			}

			if(Time.realtimeSinceStartup - garbageTime > .05f){
				garbagePressed = false;
			}
		}

		if (addGraphicCopper) {
			Color guiColor = Color.white;
			guiColor.a = 0.25f;
			GUI.color = guiColor;
			GUI.DrawTexture(new Rect((Input.mousePosition.x - 25.0f), (Screen.height - Input.mousePosition.y) - 25.0f, 50.0f, 50.0f), copperTexture);
			GUI.color = Color.white;
		}
		
		if (addGraphicGold) {
			Color guiColor = Color.white;
			guiColor.a = 0.25f;
			GUI.color = guiColor;
			GUI.DrawTexture(new Rect((Input.mousePosition.x - 25.0f), (Screen.height - Input.mousePosition.y) - 25.0f, 50.0f, 50.0f), goldTexture);
			GUI.color = Color.white;
		}
		
		if (addGraphicPlatinum) {
			Color guiColor = Color.white;
			guiColor.a = 0.25f;
			GUI.color = guiColor;
			GUI.DrawTexture(new Rect((Input.mousePosition.x - 25.0f), (Screen.height - Input.mousePosition.y) - 25.0f, 50.0f, 50.0f), platinumTexture);
			GUI.color = Color.white;
		}

		//create the temperature panel and the temperature slider
		Rect temperatureArrowBackgroundRect = new Rect(addAtomRect.x + addAtomRect.width + 10.0f, addAtomRect.y, (Screen.width - (addAtomRect.width+20)) / 2.0f, addAtomRect.height);
		GUI.DrawTexture(temperatureArrowBackgroundRect, darkBackground);
		Texture tempTexture = temperaturePanelActive ? downArrow : upArrow;
		GUI.DrawTexture(new Rect(temperatureArrowBackgroundRect.x + temperatureArrowBackgroundRect.width * .5f, temperatureArrowBackgroundRect.y, 20.0f, temperatureArrowBackgroundRect.height), tempTexture);
		if(GUI.Button(temperatureArrowBackgroundRect, "", buttonStyle)){
			temperaturePanelActive = !temperaturePanelActive;
		}

		if(temperaturePanelActive){
			Rect temperatureBackgroundRect = new Rect(temperatureArrowBackgroundRect.x, temperatureArrowBackgroundRect.y - (lightAddAtom.height), temperatureArrowBackgroundRect.width, lightAddAtom.height);
			GUI.DrawTexture(temperatureBackgroundRect, lightBackground);

			GUIStyle tempertureText = GUI.skin.label;
			tempertureText.alignment = TextAnchor.MiddleCenter;
			tempertureText.fontSize = 25;
			tempertureText.normal.textColor = Color.white;
			GUI.Label (new Rect(temperatureArrowBackgroundRect.x, temperatureBackgroundRect.y, temperatureBackgroundRect.width, temperatureBackgroundRect.height * .4f), "Temperature", tempertureText);

			GUIStyle tempNumberText = GUI.skin.label;
			tempNumberText.alignment = TextAnchor.MiddleLeft;
			tempNumberText.fontSize = 14;
			tempNumberText.normal.textColor = Color.white;

			GUI.Label (new Rect (temperatureBackgroundRect.x + temperatureBackgroundRect.width - 120.0f, (temperatureBackgroundRect.y + (temperatureBackgroundRect.height/2.0f)), 200.0f, 20), TemperatureCalc.desiredTemperature + "K" + " (" + (Math.Round(TemperatureCalc.desiredTemperature - 272.15, 2)).ToString() + "C)", tempNumberText);
			float newTemp = GUI.HorizontalSlider (new Rect (temperatureBackgroundRect.x + 25.0f, (temperatureBackgroundRect.y + (temperatureBackgroundRect.height/2.0f)), temperatureBackgroundRect.width - 150.0f, 20.0f), TemperatureCalc.desiredTemperature, StaticVariables.tempRangeLow, StaticVariables.tempRangeHigh);
			if (newTemp != TemperatureCalc.desiredTemperature) {
				changingSlider = true;
				TemperatureCalc.desiredTemperature = newTemp;
			}
			else{
				//the gui temperature has been set, we can safely change the desired temperature
				int temp = (int)TemperatureCalc.desiredTemperature;
				int remainder = temp % 20;
				temp -= remainder;
				TemperatureCalc.desiredTemperature = temp;
			}
		}

		//create the volume panel and the volume slider
		Rect volumeArrowBackgroundRect = new Rect (temperatureArrowBackgroundRect.x + temperatureArrowBackgroundRect.width + 10.0f, addAtomRect.y, temperatureArrowBackgroundRect.width, temperatureArrowBackgroundRect.height);
		GUI.DrawTexture (volumeArrowBackgroundRect, darkBackground);
		Texture volumeArrow = volumePanelActive ? downArrow : upArrow;
		GUI.DrawTexture (new Rect (volumeArrowBackgroundRect.x + volumeArrowBackgroundRect.width * .5f, volumeArrowBackgroundRect.y, 20.0f, volumeArrowBackgroundRect.height), volumeArrow);
		if (GUI.Button (volumeArrowBackgroundRect, "", buttonStyle)) {
			volumePanelActive = !volumePanelActive;
		}

		if (volumePanelActive) {
			Rect volumeBackgroundRect = new Rect(volumeArrowBackgroundRect.x, volumeArrowBackgroundRect.y - (lightAddAtom.height), volumeArrowBackgroundRect.width, lightAddAtom.height);
			GUI.DrawTexture(volumeBackgroundRect, lightBackground);

			GUIStyle volumeText = GUI.skin.label;
			volumeText.alignment = TextAnchor.MiddleCenter;
			volumeText.fontSize = 25;
			volumeText.normal.textColor = Color.white;
			GUI.Label (new Rect(volumeBackgroundRect.x, volumeBackgroundRect.y, volumeBackgroundRect.width, volumeBackgroundRect.height * .4f), "Volume", volumeText);

			GUIStyle volNumberText = GUI.skin.label;
			volNumberText.alignment = TextAnchor.UpperLeft;
			volNumberText.fontSize = 14;
			volNumberText.normal.textColor = Color.white;
			GUI.Label (new Rect (volumeBackgroundRect.x + volumeBackgroundRect.width - 120.0f, (volumeBackgroundRect.y + (volumeBackgroundRect.height/2.0f)) - 5.0f, 200.0f, 80.0f), guiVolume + " Angstroms\n cubed", volNumberText);
			float newVolume = GUI.HorizontalSlider (new Rect (volumeBackgroundRect.x + 25.0f, (volumeBackgroundRect.y + (volumeBackgroundRect.height/2.0f)), volumeBackgroundRect.width - 150.0f, 20.0f), guiVolume, 1000.0f, 64000.0f);

			if (newVolume != guiVolume) {
				guiVolume = newVolume;
				changingSlider = true;
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
		}
		CheckAtomVolumePositions();

		if (Input.GetMouseButtonUp (0)) {
		
			//possibly adjust the z value here depending on the position of the camera
			if(addGraphicCopper && Input.mousePosition.x < Screen.width && Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height){
				Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 40.0f));
				Quaternion curRotation = Quaternion.Euler(0, 0, 0);
				curPosition = CheckPosition(curPosition);
				Instantiate(copperPrefab, curPosition, curRotation);
			}
		
			if(addGraphicGold && Input.mousePosition.x < Screen.width && Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height){
				Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 40.0f));
				Quaternion curRotation = Quaternion.Euler(0, 0, 0);
				curPosition = CheckPosition(curPosition);
				Instantiate(goldPrefab, curPosition, curRotation);
			}
		
			if(addGraphicPlatinum && Input.mousePosition.x < Screen.width && Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height){
				Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 40.0f));
				Quaternion curRotation = Quaternion.Euler(0, 0, 0);
				curPosition = CheckPosition(curPosition);
				Instantiate(platinumPrefab, curPosition, curRotation);
			}
					
			addGraphicCopper = false;
			addGraphicGold = false;
			addGraphicPlatinum = false;
			changingSlider = false;
			first = true;
			clicked = false;
			startTime = 0.0f;
		}

		//create the atom selected graphic if the user has select 1 or more atoms
		int selectedAtoms = CountSelectedAtoms ();
		if (selectedAtoms > 0) {
			GUIStyle atomsSelectedText = GUI.skin.label;
			atomsSelectedText.alignment = TextAnchor.MiddleCenter;
			atomsSelectedText.fontSize = 22;
			atomsSelectedText.normal.textColor = Color.white;
			Rect darkBlueRect = new Rect(toolbarRect.x + toolbarRect.width + 5.0f, 0.0f, 200.0f, 65.0f);
			GUI.DrawTexture(darkBlueRect, darkBlueBackground);
			String atomSelectedString = selectedAtoms == 1 ? " Atom Selected" : " Atoms Selected";
			GUI.Label(darkBlueRect, selectedAtoms.ToString() + atomSelectedString);

			Rect lightBlueRect = new Rect(darkBlueRect.x + darkBlueRect.width, 0.0f, 15.0f, darkBlueRect.height);
			GUI.DrawTexture(lightBlueRect, lightBlueBackground);
			GUI.DrawTexture(new Rect(lightBlueRect.x, lightBlueRect.y + (lightBlueRect.height) - 15.0f, lightBlueRect.width, 15.0f), whiteCornerArrow);
			if(GUI.Button(lightBlueRect, "", buttonStyle)){
				whiteCornerActive = !whiteCornerActive;
			}

			if(whiteCornerActive){
				Rect selectAllRect = new Rect(darkBlueRect.x, darkBlueRect.y + darkBlueRect.height, darkBlueRect.width + lightBlueRect.width, 45.0f);
				GUI.DrawTexture(selectAllRect, darkBackground);
				GUIStyle selectAllText = GUI.skin.label;
				selectAllText.alignment = TextAnchor.MiddleCenter;
				selectAllText.fontSize = 22;
				selectAllText.normal.textColor = Color.white;
				if(selectedAtoms == allMolecules.Length){
					GUI.Label(selectAllRect, "Deselect All", selectAllText);
					if(GUI.Button(selectAllRect, "", buttonStyle)){
						DeselectAllAtoms();
						whiteCornerActive = false;
					}
				}
				else{
					GUI.Label(selectAllRect, "Select All", selectAllText);
					if(GUI.Button(selectAllRect, "", buttonStyle)){
						SelectAllAtoms();
						whiteCornerActive = false;
					}
				}

			}
		}

		GUIStyle timeText = GUI.skin.label;
		timeText.alignment = TextAnchor.MiddleLeft;
		timeText.fontSize = 18;
		timeText.normal.textColor = Color.white;
		GUI.Label (new Rect (Screen.width - 75.0f, 10.0f, 70.0f, 40.0f), Math.Round(StaticVariables.currentTime, 1) + "ps");

		//print ("Potential energy: " + PotentialEnergy.finalPotentialEnergy);


	}

	//this function displays properties that are apart of the system as a whole, such as the number of atoms
	void DisplaySystemProperties(Rect displayRect){
		GUIStyle timeText = GUI.skin.label;
		timeText.alignment = TextAnchor.MiddleLeft;
		timeText.fontSize = 14;
		timeText.normal.textColor = Color.white;

		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		int totalAtoms = allMolecules.Length;
		int copperAtoms = 0;
		int goldAtoms = 0;
		int platinumAtoms = 0;
		for (int i = 0; i < allMolecules.Length; i++) {
			GameObject currAtom = allMolecules[i];
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
	void DisplayAtomProperties(GameObject currAtom, Rect displayRect){

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
	void DisplayBondProperties(GameObject currAtom, Rect displayRect){
		
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
		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		for (int i = 0; i < allMolecules.Length; i++) {
			GameObject currAtom = allMolecules[i];
			Atom atomScript = currAtom.GetComponent<Atom>();
			atomScript.selected = true;
			atomScript.SetSelected(true);
		}
	}

	//this function deselects all of the atoms in the scene
	void DeselectAllAtoms(){
		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		for (int i = 0; i < allMolecules.Length; i++) {
			GameObject currAtom = allMolecules[i];
			Atom atomScript = currAtom.GetComponent<Atom>();
			atomScript.selected = false;
			atomScript.SetSelected(false);
		}
	}

	//when an atom is added to the scene, its position must be checked to make sure it is inside of the box
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

	//this function returns the number of atoms that are selected
	int CountSelectedAtoms(){
		int selectedAtoms = 0;
		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		for (int i = 0; i < allMolecules.Length; i++) {
			GameObject currAtom = allMolecules[i];
			Atom atomScript = currAtom.GetComponent<Atom>();
			if(atomScript.selected){
				selectedAtoms++;
			}
		}
		return selectedAtoms;
	}

	//this function checks the position of all of the atoms to make sure they are inside of the box
	void CheckAtomVolumePositions(){
		
		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment>();
		for (int i = 0; i < allMolecules.Length; i++) {
			GameObject currAtom = allMolecules[i];
			Vector3 newPosition = currAtom.transform.position;
			if(currAtom.transform.position.x > createEnvironment.bottomPlane.transform.position.x + (createEnvironment.width/2.0f) - createEnvironment.errorBuffer){
				newPosition.x = createEnvironment.bottomPlane.transform.position.x + (createEnvironment.width/2.0f) - createEnvironment.errorBuffer;
			}
			if(currAtom.transform.position.x < createEnvironment.bottomPlane.transform.position.x - (createEnvironment.width/2.0f) + createEnvironment.errorBuffer){
				newPosition.x = createEnvironment.bottomPlane.transform.position.x - (createEnvironment.width/2.0f) + createEnvironment.errorBuffer;
			}
			if(currAtom.transform.position.y > createEnvironment.bottomPlane.transform.position.y + (createEnvironment.height) - createEnvironment.errorBuffer){
				newPosition.y = createEnvironment.bottomPlane.transform.position.y + (createEnvironment.height) - createEnvironment.errorBuffer;
			}
			if(currAtom.transform.position.y < createEnvironment.bottomPlane.transform.position.y + createEnvironment.errorBuffer){
				newPosition.y = createEnvironment.bottomPlane.transform.position.y + createEnvironment.errorBuffer;
			}
			if(currAtom.transform.position.z > createEnvironment.bottomPlane.transform.position.z + (createEnvironment.depth/2.0f) - createEnvironment.errorBuffer){
				newPosition.z = createEnvironment.bottomPlane.transform.position.z + (createEnvironment.depth/2.0f) - createEnvironment.errorBuffer;
			}
			if(currAtom.transform.position.z < createEnvironment.bottomPlane.transform.position.z - (createEnvironment.depth/2.0f) + createEnvironment.errorBuffer){
				newPosition.z = createEnvironment.bottomPlane.transform.position.z - (createEnvironment.depth/2.0f) + createEnvironment.errorBuffer;
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

	//this function give all atoms in the scene a random velocity
	public void AtomKick(){
		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		for(int i = 0; i < allMolecules.Length; i++){
			GameObject currAtom = allMolecules[i];
			float xVelocity = 0.0f;
			float yVelocity = 0.0f;
			float zVelocity = 0.0f;
			if(UnityEngine.Random.Range(0.0f, 1.0f) > .5f){
				xVelocity = UnityEngine.Random.Range(1.0f, 5.0f);
			}
			else{
				xVelocity = UnityEngine.Random.Range(-5.0f, -1.0f);
			}
			if(UnityEngine.Random.Range(0.0f, 1.0f) > .5f){
				yVelocity = UnityEngine.Random.Range(1.0f, 5.0f);
			}
			else{
				yVelocity = UnityEngine.Random.Range(-5.0f, -1.0f);
			}
			if(UnityEngine.Random.Range(0.0f, 1.0f) > .5f){
				zVelocity = UnityEngine.Random.Range(1.0f, 5.0f);
			}
			else{
				zVelocity = UnityEngine.Random.Range(-5.0f, -1.0f);
			}
			currAtom.rigidbody.velocity = new Vector3(xVelocity, yVelocity, zVelocity);
		}
	}
	
	
}
