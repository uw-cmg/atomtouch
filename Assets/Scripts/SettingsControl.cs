using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsControl : MonoBehaviour {
	public static SettingsControl mySettings;

	public GameObject bottomLayer;
	public GameObject settingsCanvas;
	public GameObject settingsPanel;
	public GameObject hudCanvas;
	public GameObject settingsButton;
	public GameObject bondLineOn;
	public GameObject lenJonesOn;
	public GameObject buckinghamOn;
	public GameObject nmOn;
	public GameObject trailsOn;
	public GameObject atomRendererOn;
	public GameObject sliderPanel;
	public GameObject graphOn;
	public GameObject graphPanel;

	public GameObject preferences;
	public GameObject credits;
	public GameObject sceneLoader;
	//waiting for Brenner to be done
	public GameObject brennerOn;
	public AtomTouchGUI atomTouchGUI;

	public static bool renderAtoms = true;
	public static bool mouseExitsSettingsPanel; //aka, pause the game

	public static Potential.potentialType currentPotentialType;
	private static bool simTypeChanged;
	
	private static bool gamePaused;
	private Toggle nmToggle;
	private Toggle atomRendererToggle;
	public Toggle texturedToggle;
	public static bool textureOn = true;
	[HideInInspector]public Toggle trailsToggle;

    public static bool GamePaused{
    	get { return gamePaused; }
       	//set { this._Name = value; }  
    }
	void Awake(){
		mouseExitsSettingsPanel = true;
		gamePaused = false;
		mySettings = this;

		atomTouchGUI = Camera.main.GetComponent<AtomTouchGUI>();
		nmToggle = nmOn.GetComponent<Toggle>();
		trailsToggle = trailsOn.GetComponent<Toggle>();
		atomRendererToggle = atomRendererOn.GetComponent<Toggle>();
		simTypeChanged = false;
	}
	void Start(){
		currentPotentialType = Potential.currentPotential;
	}
	
	public void ResumeGame(){
		//Debug.Log("mio");
		settingsCanvas.SetActive(false);
		hudCanvas.SetActive(true);
		//resume
		//Time.timeScale = 1.0f;
		atomTouchGUI.ChangeAtomTemperature();
		atomTouchGUI.changingTemp = false;
		atomTouchGUI.changingVol = false;
		//if sim type is changed, reset
		if(simTypeChanged){
			CreateEnvironment.myEnvironment.preCompute();
			//CreateEnvironment.myEnvironment.InitAtoms();
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
		//pause
		Time.timeScale = 0.0f;
	}

	

	public void OnClick_SettingsButton(){
		PauseGame();
		sceneLoader.SetActive(false);
		credits.SetActive(true);
		preferences.SetActive(true);
	}
	//scene selector callback
	public void OnLoad_PresetAtoms(string filename){
		CreateEnvironment env = CreateEnvironment.myEnvironment;
		env.LoadPresetAtoms(filename);
	}
	public void OnToggle_textured(){
		MeshRenderer mr;
		if(texturedToggle.isOn){
			textureOn = true;
			for(int i=0; i < Atom.AllAtoms.Count; i++){
				Atom atom = Atom.AllAtoms[i];
				mr = atom.gameObject.GetComponent<MeshRenderer>();
				if(atom.selected){
					mr.material = atom.selectedMaterial;
				}else if(atom.isTransparent){
					mr.material = atom.transparentMaterial;
				}else{
					mr.material = atom.defaultMaterial;
				}
			}
		}else{
			textureOn = false;
			for(int i=0; i < Atom.AllAtoms.Count; i++){
				Atom atom = Atom.AllAtoms[i];
				mr = atom.gameObject.GetComponent<MeshRenderer>();
				if(atom.selected){
					mr.material = atom.untexturedSelectedMaterial;
				}else if(atom.isTransparent){
					mr.material = atom.untexturedTransparentMaterial;
				}else{
					mr.material = atom.untexturedDefaultMaterial;
				}
			}
		}
	}

	public void OnToggle_Bondline(){
		StaticVariables.drawBondLines = bondLineOn.GetComponent<Toggle>().isOn;
	}
	public void OnToggle_Graph(){
		Chart.show = graphOn.GetComponent<Toggle>().isOn;
	}
	public void OnToggle_VolUnitNm(){
		if(nmToggle.isOn){
			UpdateVolume.volUnit = UpdateVolume.VolUnitType.Nanometer;
		}else{
			UpdateVolume.volUnit = UpdateVolume.VolUnitType.Angstrom;
		}
	}
	//turn trail renderers on/off
	public void OnToggle_Trails(){
		for(int i=0; i<Atom.AllAtoms.Count;i++){
			Atom currAtom = Atom.AllAtoms[i];
			currAtom.gameObject.GetComponent<TrailRenderer>().enabled = trailsToggle.isOn;
		}
	}
	//turn on/off mesh renderer
	public void OnToggle_AtomRenderer(){
		if(atomRendererToggle.isOn){
			if(!renderAtoms){
				//update atom mesh renderers only if settings has been changed
				renderAtoms = true;
			}else{
				return;
			}	
		}else{
			if(renderAtoms){
				renderAtoms = false;
			}else{
				return;
			}
		}
		//update renderers
		for(int i=0; i < Atom.AllAtoms.Count;i++){
			Atom.AllAtoms[i].gameObject.GetComponent<MeshRenderer>().enabled = renderAtoms;
		}

	}
	//checks if mouse clicks outside the settings, if so, exit settings and resume
	public void CheckExitSettings(){
		
		Vector3 mp = Input.mousePosition;
		RectTransform rt = settingsPanel.GetComponent<RectTransform>();
		if(mp.x > rt.anchorMin.x * Screen.width //lower left
			&& mp.x < rt.anchorMax.x * Screen.width //upper right
			&& mp.y > rt.anchorMin.y * Screen.height
			&& mp.y < rt.anchorMax.y * Screen.height){ 
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
			Debug.Log("changeing to LennardJones");
		}else if(buckinghamOn.GetComponent<Toggle>().isOn){
			Potential.currentPotential = Potential.potentialType.Buckingham;
			if(currentPotentialType != Potential.potentialType.Buckingham){
				simTypeChanged = true;
			}
			currentPotentialType = Potential.potentialType.Buckingham;
			Debug.Log("changeing to buckingham");
		}
		atomTouchGUI.SetAtomBtnsVisibility();
	}
	/*
	public void OpenScreenLoader(){
		PauseGame();
	}
	*/
	public void OnClick_SceneLoader(){
		PauseGame();
		sceneLoader.SetActive(true);
		credits.SetActive(false);
		preferences.SetActive(false);
		//pause
		
	}
}
