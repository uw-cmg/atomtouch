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
	public GameObject nmOn;
	//waiting for Brenner to be done
	public GameObject brennerOn;
	public GameObject creditHolder;
	public GameObject[] creditEntries;

	public AtomTouchGUI atomTouchGUI;

	private bool mouseExitsSettingsPanel; //aka, pause the game
	private Rect creditViewArea;

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
		//creditViewArea = creditHolder.GetComponent<RectTransform>().rect;
		//creditEntries = GameObject.FindGameObjectsWithTag("Credit");
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
		StaticVariables.drawBondLines = bondLineOn.GetComponent<Toggle>().isOn;
	}
	public void OnToggle_VolUnitNm(){
		if(nmOn.GetComponent<Toggle>().isOn){
			UpdateVolume.volUnit = UpdateVolume.VolUnitType.Nanometer;
		}else{
			UpdateVolume.volUnit = UpdateVolume.VolUnitType.Angstrom;
		}
	}
	public void OnScroll_Credit(){
		//creditViewArea
		foreach(GameObject entry in creditEntries){
			Rect r = entry.GetComponent<RectTransform>().rect;
			if(RectContains(ref creditViewArea, ref r)){
				entry.SetActive(true);
			}else{
				entry.SetActive(false);
			}
		}
	}
	public static bool RectContains(ref Rect r1, ref Rect r2){
		if(r2.yMin > r1.yMin || r2.yMin < r1.yMin){
			return false;
		}
		return false;
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
