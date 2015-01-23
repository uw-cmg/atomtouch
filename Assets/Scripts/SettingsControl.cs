using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsControl : MonoBehaviour {
	public GameObject bottomLayer;
	public GameObject settingsCanvas;
	public GameObject settingsPanel;
	public GameObject hudCanvas;
	public GameObject settingsButton;
	public GameObject bondLineOn;
	public GameObject lenJonesOn;
	public GameObject buckinghamOn;
	public GameObject nmOn;
	//waiting for Brenner to be done
	public GameObject brennerOn;
	public static bool enteredHud = false;
	public AtomTouchGUI atomTouchGUI;

	public static bool mouseExitsSettingsPanel; //aka, pause the game

	private static Potential.potentialType currentPotentialType;
	private static bool simTypeChanged;
	
	private static bool gamePaused;
	private static bool tempUpdating;
	private static bool volUpdating;
	private Toggle nmToggle;
    public static bool GamePaused{
    	get { return gamePaused; }
       	//set { this._Name = value; }  
    }
    public static bool TempUpdating{
    	get{return tempUpdating;}
    	set{tempUpdating = value;}
    }
    public static bool VolUpdating{
    	get{return volUpdating;}
    	set{volUpdating = value;}
    }
	void Awake(){
		mouseExitsSettingsPanel = true;
		gamePaused = false;
		currentPotentialType = Potential.potentialType.LennardJones;
		atomTouchGUI = Camera.main.GetComponent<AtomTouchGUI>();
		nmToggle = nmOn.GetComponent<Toggle>();
		simTypeChanged = false;
		tempUpdating = false;
		volUpdating = false;
	}
	void Start () {
		
	}
	
	void Update(){
		/*
		if(Input.GetMouseButtonDown(0) && mouseExitsSettingsPanel){
			StaticVariables.mouseClickProcessed = true;
			ResumeGame();
		}
		*/
	}
	public void ResumeGame(){
		Debug.Log("mio");
		settingsCanvas.SetActive(false);
		hudCanvas.SetActive(true);
		//bottomLayer.SetActive(true);
		//resume
		Time.timeScale = 1.0f;

		//if sim type is changed, reset
		if(simTypeChanged){
			CreateEnvironment.myEnvironment.preCompute();
			atomTouchGUI.ResetAll();
			simTypeChanged = false;
		}
		gamePaused = false;
		StaticVariables.mouseClickProcessed = false;
	}

	public void PauseGame(){
		gamePaused = true;
		settingsCanvas.SetActive(true);
		hudCanvas.SetActive(false);
		//bottomLayer.SetActive(false);
		enteredHud = false;
		//pause
		Time.timeScale = 0.0f;
	}

	public void OnClick_OutsideSettings(bool exits){
		mouseExitsSettingsPanel = exits;
	}

	public void OnClick_SettingsButton(){
		PauseGame();

	}

	public void OnToggle_Bondline(){
		StaticVariables.drawBondLines = bondLineOn.GetComponent<Toggle>().isOn;
	}
	public void OnToggle_VolUnitNm(){
		if(nmToggle.isOn){
		//if(nmOn.GetComponent<Toggle>().isOn){
			UpdateVolume.volUnit = UpdateVolume.VolUnitType.Nanometer;
		}else{
			UpdateVolume.volUnit = UpdateVolume.VolUnitType.Angstrom;
		}
	}

	public void bottomlayertest(){
		Rect r = settingsPanel.GetComponent<RectTransform>().rect;
		
		
		Debug.Log("mouse position at bottom layer: " + Input.mousePosition);

		Vector3 p = Vector3.zero;

		Vector3 v = Input.mousePosition
			+new Vector3(-Screen.width/2.0f,
				-Screen.height/2.0f,0);
		Debug.Log(Mathf.Abs(v.x));
		Debug.Log(Mathf.Abs(v.y));
		if(Mathf.Abs(v.x) < r.width*settingsCanvas.GetComponent<Canvas>().scaleFactor/2.0f 
			&& Mathf.Abs(v.y) < r.height*settingsCanvas.GetComponent<Canvas>().scaleFactor/2.0f){
			Debug.Log("IN RECT");

		}else{
			Debug.Log("OUT RECT");
			StaticVariables.mouseClickProcessed = true;
			ResumeGame();
		}
		
	}

	public void OnChange_SimType(){
		if(lenJonesOn.GetComponent<Toggle>().isOn){
			Potential.currentPotential = Potential.potentialType.LennardJones;
			if(currentPotentialType != Potential.potentialType.LennardJones){
				simTypeChanged = true;
			}
			currentPotentialType = Potential.potentialType.LennardJones;
		}else if(buckinghamOn.GetComponent<Toggle>().isOn){
			Potential.currentPotential = Potential.potentialType.Buckingham;
			if(currentPotentialType != Potential.potentialType.Buckingham){
				simTypeChanged = true;
			}
			currentPotentialType = Potential.potentialType.Buckingham;
		}
	}
}
