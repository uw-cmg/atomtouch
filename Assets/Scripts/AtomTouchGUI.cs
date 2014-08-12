using UnityEngine;
using System.Collections;

public class AtomTouchGUI : MonoBehaviour {
	
	private bool atomTouchActive = true;
	private bool toolbarActive = true;
	public Texture lightBackground;
	public Texture darkBackground;
	public Texture downArrow;
	public Texture upArrow;

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

	public static StaticVariables.TimeSpeed currentTimeSpeed = StaticVariables.TimeSpeed.Normal;

	void Start () {
	
	}


	void OnGUI(){

		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment> ();

		GUIStyle buttonStyle = GUI.skin.label;
		Rect arrowBackgroundRect = new Rect (0.0f, 0.0f, Screen.width * .14f, Screen.height * .13f * .3f);
		Texture atomTouchArrow = atomTouchActive ? downArrow : upArrow;
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
			GUI.DrawTexture (atomTouchRect, lightBackground);
			if (GUI.Button (atomTouchRect, "Atom Touch", textStyle)) {
				print ("atomTouchPressed");
			}
		}

		Rect arrowBackgroundRectToolbar = new Rect (Screen.width * .14f + 5.0f, 0.0f, Screen.width * .28f, Screen.height * .13f * .3f);
		Texture toolbarArrow = toolbarActive ? downArrow : upArrow;
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
				Camera.main.transform.position = new Vector3(0.0f, 0.0f, -35.0f);
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
					print ("setting to stopped");
					currentTimeSpeed = StaticVariables.TimeSpeed.Stopped;
					StaticVariables.pauseTime = true;
				}
				else if(currentTimeSpeed == StaticVariables.TimeSpeed.Stopped){
					print ("setting to slow");
					currentTimeSpeed = StaticVariables.TimeSpeed.SlowMotion;
					StaticVariables.pauseTime = false;
					Time.timeScale = .05f;
				}
				else if(currentTimeSpeed == StaticVariables.TimeSpeed.SlowMotion){
					print ("setting to normal");
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
		
		
	}
}
