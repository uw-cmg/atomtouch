using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsControl : MonoBehaviour {
	public GameObject settingsCanvas;
	public GameObject settingsPanel;
	public GameObject hudCanvas;
	public GameObject settingsButton;

	private bool mouseExitsSettingsPanel;

	void Awake(){
		mouseExitsSettingsPanel = false;
	}
	void Start () {
		
	}
	
	void Update(){
		if(Input.GetMouseButtonDown(0)){
			if(mouseExitsSettingsPanel){
				Debug.Log("mio");
				settingsCanvas.SetActive(false);
				hudCanvas.SetActive(true);
				//resume
				StaticVariables.pauseTime = false;
			}
		}
	}

	public void OnClick_OutsideSettings(bool exits){
		mouseExitsSettingsPanel = exits;
	}

	public void OnClick_SettingsButton(){
		settingsCanvas.SetActive(true);
		hudCanvas.SetActive(false);
		//pause
		AtomTouchGUI.currentTimeSpeed = StaticVariables.TimeSpeed.Stopped;
		StaticVariables.pauseTime = true;
	}
}
