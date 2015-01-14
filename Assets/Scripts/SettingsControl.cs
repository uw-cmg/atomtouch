using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsControl : MonoBehaviour {
	public GameObject settingsCanvas;
	public GameObject settingsPanel;
	public GameObject hudCanvas;
	public GameObject settingsButton;
	public GameObject bondLineOn;

	private bool mouseExitsSettingsPanel; //aka, pause the game
	private static bool gamePaused;
    public static bool GamePaused
    {
    	get { return gamePaused; }
       	//set { this._Name = value; }  
    }
	void Awake(){
		mouseExitsSettingsPanel = false;
		gamePaused = false;
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
}
