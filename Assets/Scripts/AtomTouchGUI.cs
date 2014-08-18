using UnityEngine;
using System.Collections;
using System;

public class AtomTouchGUI : MonoBehaviour {
	
	private bool atomTouchActive = true;
	private bool toolbarActive = true;
	private bool dataPanelActive = false;
	private bool addAtomActive = true;
	private bool temperaturePanelActive = true;
	private bool volumePanelActive = true;
	public Texture lightBackground;
	public Texture darkBackground;
	public Texture downArrow;
	public Texture upArrow;
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
	public Texture garbageTexture;
	private bool clicked = false;
	private float startTime = 0.0f;
	private bool first = true;
	public float holdTime = 0.05f;
	[HideInInspector]public bool addGraphicCopper;
	[HideInInspector]public bool addGraphicGold;
	[HideInInspector]public bool addGraphicPlatinum;

	[HideInInspector]public bool changingSlider = false;
	private float guiVolume;

	public static StaticVariables.TimeSpeed currentTimeSpeed = StaticVariables.TimeSpeed.Normal;

	void Start () {
		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment> ();
		guiVolume = createEnvironment.volume;
	}


	void OnGUI(){

		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment> ();

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
				print ("atomTouchPressed");
			}
		}

		Rect arrowBackgroundRectToolbar = new Rect (Screen.width * .14f + 5.0f, 0.0f, Screen.width * .28f, Screen.height * .13f * .3f);
		Texture toolbarArrow = toolbarActive ? upArrow : downArrow;
		GUI.DrawTexture (arrowBackgroundRectToolbar, darkBackground);
		GUI.DrawTexture (new Rect (arrowBackgroundRectToolbar.x + (arrowBackgroundRectToolbar.width*.5f), 0.0f, 20.0f, arrowBackgroundRectToolbar.height), toolbarArrow); 
		if (GUI.Button (arrowBackgroundRectToolbar, "", buttonStyle)) {
			toolbarActive = !toolbarActive;
		}

		if (toolbarActive) {
			Rect toolbarRect = new Rect(arrowBackgroundRectToolbar.x, arrowBackgroundRectToolbar.height, arrowBackgroundRectToolbar.width, atomTouchRect.height);
			GUI.DrawTexture(toolbarRect, lightBackground);

			Texture reset = resetPressed ? resetButtonDown : resetButtonUp;
			if(GUI.Button(new Rect(toolbarRect.x, toolbarRect.y, toolbarRect.width / 6.0f, toolbarRect.height), reset, buttonStyle)){
				resetPressed = true;
				resetTime = Time.realtimeSinceStartup;
				createEnvironment.ResetAtoms();
			}
			if(Time.realtimeSinceStartup - resetTime > .05f){
				resetPressed = false;
			}

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

			Texture bondLine = StaticVariables.drawBondLines ? bondLineUp : bondLineDown;
			if(GUI.Button(new Rect(toolbarRect.x + 2*(toolbarRect.width / 6.0f), toolbarRect.y, toolbarRect.width / 6.0f, toolbarRect.height), bondLine, buttonStyle)){
				StaticVariables.drawBondLines = !StaticVariables.drawBondLines;
			}

			Texture atomKick = atomKickPressed ? atomKickDown : atomKickUp;
			if(GUI.Button(new Rect(toolbarRect.x + 3*(toolbarRect.width / 6.0f), toolbarRect.y, toolbarRect.width / 6.0f, toolbarRect.height), atomKick, buttonStyle)){
				atomKickPressed = true;
				atomKickTime = Time.realtimeSinceStartup;
				for(int i = 0; i < allMolecules.Length; i++){
					GameObject currAtom = allMolecules[i];
					currAtom.rigidbody.velocity = new Vector3(UnityEngine.Random.Range(-5.0f, 5.0f), UnityEngine.Random.Range(-5.0f, 5.0f), UnityEngine.Random.Range(-5.0f, 5.0f));
				}
			}
			if(Time.realtimeSinceStartup - atomKickTime > .05f){
				atomKickPressed = false;
			}

			Texture timeTexture = normalTimeButton;
			if(currentTimeSpeed == StaticVariables.TimeSpeed.Normal){
				timeTexture = normalTimeButton;
			}
			else if(currentTimeSpeed == StaticVariables.TimeSpeed.SlowMotion){
				timeTexture = slowTimeButton;
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
					Time.timeScale = .05f;
				}
				else if(currentTimeSpeed == StaticVariables.TimeSpeed.SlowMotion){
					currentTimeSpeed = StaticVariables.TimeSpeed.Normal;
					Time.timeScale = 1.0f;
				}
			}


			for (int i = 0; i < allMolecules.Length; i++) {
				Atom atomScript = allMolecules[i].GetComponent<Atom>();
				if(atomScript.doubleTapped){
					if(GUI.Button(new Rect(toolbarRect.x + 5*(toolbarRect.width / 6.0f), toolbarRect.y, toolbarRect.width / 6.0f, toolbarRect.height), redXButton, buttonStyle)){
						createEnvironment.centerPos = new Vector3(0.0f, 0.0f, 0.0f);
						atomScript.doubleTapped = false;
						Camera.main.transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
						Time.timeScale = 1.0f;
						currentTimeSpeed = StaticVariables.TimeSpeed.Normal;
						atomScript.RemoveBondText();
						atomScript.ResetTransparency();
					}
			
					//DisplayAtomProperties(allMolecules[i]);
			
				}
			}
		}

		Rect panelRect;
		Rect panelArrowRect = new Rect (Screen.width * .5f, Screen.height - (Screen.height * .13f * .3f), 20.0f, Screen.height * .13f * .3f);
		if (dataPanelActive) {
			panelRect = new Rect (0.0f, Screen.height - (Screen.height * .27f), Screen.width, (Screen.height * .27f));
			GUI.DrawTexture(panelRect, lightBackground);
			GUI.DrawTexture(panelArrowRect, downArrow);
			float buffer = 10.0f;
			GUI.DrawTexture (new Rect (panelRect.x + buffer, panelRect.y + buffer, panelRect.width - (buffer*2), panelRect.height - panelArrowRect.height - buffer), darkBackground);
		}
		else{
			panelRect = new Rect(0.0f, panelArrowRect.y, Screen.width, panelArrowRect.height);
			GUI.DrawTexture(panelRect, lightBackground);
			GUI.DrawTexture(panelArrowRect, upArrow);
		}
		if (GUI.Button (new Rect (0.0f, panelArrowRect.y, Screen.width, panelArrowRect.height), "", buttonStyle)) {
			dataPanelActive = !dataPanelActive;
		}

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

			if(GUI.RepeatButton(new Rect(lightAddAtom.x, lightAddAtom.y, addAtomRect.width / 4.0f, lightAddAtom.height), copperTexture, buttonStyle)){
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
			if(GUI.RepeatButton(new Rect(lightAddAtom.x+(addAtomRect.width / 4.0f), lightAddAtom.y, addAtomRect.width / 4.0f, lightAddAtom.height), goldTexture, buttonStyle)){
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
			if(GUI.RepeatButton(new Rect(lightAddAtom.x+(2*(addAtomRect.width / 4.0f)), lightAddAtom.y, addAtomRect.width / 4.0f, lightAddAtom.height), platinumTexture, buttonStyle)){
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
			if(GUI.RepeatButton(new Rect(lightAddAtom.x+(3*(addAtomRect.width / 4.0f)), lightAddAtom.y, addAtomRect.width / 4.0f, lightAddAtom.height), garbageTexture, buttonStyle)){
				print ("pressing garbage");
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
			tempertureText.normal.textColor = new Color(32.0f / 255.0f, 70.0f / 255.0f, 119.0f / 255.0f, 1.0f);
			GUI.Label (new Rect(temperatureArrowBackgroundRect.x, temperatureBackgroundRect.y, temperatureBackgroundRect.width, temperatureBackgroundRect.height * .4f), "Temperature", tempertureText);

			GUIStyle tempNumberText = GUI.skin.label;
			tempNumberText.alignment = TextAnchor.MiddleLeft;
			tempNumberText.fontSize = 14;
			tempNumberText.normal.textColor = Color.white;

			GUI.Label (new Rect (temperatureBackgroundRect.x + temperatureBackgroundRect.width - 120.0f, (temperatureBackgroundRect.y + (temperatureBackgroundRect.height/2.0f)) - 5.0f, 200.0f, 20), TemperatureCalc.desiredTemperature + "K" + " (" + (Math.Round(TemperatureCalc.desiredTemperature - 272.15, 2)).ToString() + "C)", tempNumberText);
			float newTemp = GUI.HorizontalSlider (new Rect (temperatureBackgroundRect.x + 25.0f, (temperatureBackgroundRect.y + (temperatureBackgroundRect.height/2.0f)), temperatureBackgroundRect.width - 150.0f, 200.0f), TemperatureCalc.desiredTemperature, StaticVariables.tempRangeLow, StaticVariables.tempRangeHigh);
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
			volumeText.normal.textColor = new Color(32.0f / 255.0f, 70.0f / 255.0f, 119.0f / 255.0f, 1.0f);
			GUI.Label (new Rect(volumeBackgroundRect.x, volumeBackgroundRect.y, volumeBackgroundRect.width, volumeBackgroundRect.height * .4f), "Volume", volumeText);

			GUIStyle volNumberText = GUI.skin.label;
			volNumberText.alignment = TextAnchor.UpperLeft;
			volNumberText.fontSize = 14;
			volNumberText.normal.textColor = Color.white;
			GUI.Label (new Rect (volumeBackgroundRect.x + volumeBackgroundRect.width - 120.0f, (volumeBackgroundRect.y + (volumeBackgroundRect.height/2.0f)) - 5.0f, 200.0f, 80.0f), guiVolume + " Angstroms\n cubed", volNumberText);
			float newVolume = GUI.HorizontalSlider (new Rect (volumeBackgroundRect.x + 25.0f, (volumeBackgroundRect.y + (volumeBackgroundRect.height/2.0f)), volumeBackgroundRect.width - 150.0f, 200.0f), guiVolume, 1000.0f, 64000.0f);

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
		
			if(addGraphicCopper && Input.mousePosition.x < Screen.width && Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height){
				Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 40.0f));
				Quaternion curRotation = Quaternion.Euler(0, 0, 0);
				curPosition = CheckPosition(curPosition);
				Instantiate(copperPrefab, curPosition, curRotation);
				print ("spawning copper");
			}
		
			if(addGraphicGold && Input.mousePosition.x < Screen.width && Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height){
				Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 40.0f));
				Quaternion curRotation = Quaternion.Euler(0, 0, 0);
				curPosition = CheckPosition(curPosition);
				Instantiate(goldPrefab, curPosition, curRotation);
				print ("spawning gold");
			}
		
			if(addGraphicPlatinum && Input.mousePosition.x < Screen.width && Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height){
				Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 40.0f));
				Quaternion curRotation = Quaternion.Euler(0, 0, 0);
				curPosition = CheckPosition(curPosition);
				Instantiate(platinumPrefab, curPosition, curRotation);
				print ("spawning platinum");
			}
					
			addGraphicCopper = false;
			addGraphicGold = false;
			addGraphicPlatinum = false;
			changingSlider = false;
			first = true;
			clicked = false;
			startTime = 0.0f;
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
