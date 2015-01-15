using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsControl : MonoBehaviour {
	public GameObject settingsCanvas;
	public GameObject settingsPanel;
	public GameObject hudCanvas;
	public GameObject settingsButton;
	public GameObject bondLineOn;
	public GameObject lenJonesOn;
	public GameObject buckinghamOn;
	//waiting for Brenner to be done
	public GameObject brennerOn;
	public AtomTouchGUI atomTouchGUI;

	private bool mouseExitsSettingsPanel; //aka, pause the game
	private static Potential.potentialType currentPotentialType;
	private static bool simTypeChanged;

	private static bool gamePaused;
    public static bool GamePaused
    {
    	get { return gamePaused; }
       	//set { this._Name = value; }  
    }
	void Awake(){
		mouseExitsSettingsPanel = false;
		gamePaused = false;
		currentPotentialType = Potential.potentialType.LennardJones;
		atomTouchGUI = Camera.main.GetComponent<AtomTouchGUI>();
		simTypeChanged = false;
	}
	void Start () {
		
	}
	
	void Update(){
		if(Input.GetMouseButtonDown(0)){
			if(mouseExitsSettingsPanel){
				ResumeGame();
			}
		}
	}
	public void ResumeGame(){
		Debug.Log("mio");
		gamePaused = false;
		settingsCanvas.SetActive(false);
		hudCanvas.SetActive(true);
		//resume
		StaticVariables.pauseTime = false;
		//if sim type is changed, reset
		if(simTypeChanged){
			atomTouchGUI.ResetAll();
			simTypeChanged = false;
		}
	}

	public void PauseGame(){
		gamePaused = true;
		settingsCanvas.SetActive(true);
		hudCanvas.SetActive(false);
		//pause
		AtomTouchGUI.currentTimeSpeed = StaticVariables.TimeSpeed.Stopped;
		StaticVariables.pauseTime = true;
	}

	public void OnClick_OutsideSettings(bool exits){
		mouseExitsSettingsPanel = exits;
	}

	public void OnClick_SettingsButton(){
		PauseGame();
	}

	public void OnToggle_Bondline(){
		//RawImage ri = bondLineBtn.GetComponent<RawImage>();
		//Texture bondLine = StaticVariables.drawBondLines ? bondLineUp : bondLineDown;
		//ri.texture = bondLine;
		//StaticVariables.drawBondLines = !StaticVariables.drawBondLines;
		StaticVariables.drawBondLines = bondLineOn.GetComponent<Toggle>().isOn;
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
