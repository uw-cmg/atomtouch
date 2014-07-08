using UnityEngine;
using System.Collections;

public class InstantiateMolecule : MonoBehaviour {

	public Rigidbody copperPrefab;
	public Rigidbody goldPrefab;
	public Rigidbody platinumPrefab;
	public GUISkin sliderControls;

	public Texture copperTexture;
	public Texture addCopperTexture;
	[HideInInspector]public bool addGraphicCopper;

	public Texture goldTexture;
	public Texture addGoldTexture;
	[HideInInspector]public bool addGraphicGold;

	public Texture platinumTexture;
	public Texture addPlatinumTexture;
	[HideInInspector]public bool addGraphicPlatinum;

	public Texture garbageTexture;
	
	private bool clicked = false;
	private float startTime = 0.0f;
	private bool first = true;
	public float holdTime = 0.05f;


	void Start(){
		addGraphicCopper = false;
		addGraphicGold = false;
	}

	void OnGUI(){
		
		
//		GUI.Label(new Rect(25, 15, 200, 20), "Time Scale: " + Atom.timeScale);
//		float timeScale = GUI.HorizontalSlider(new Rect(25, 55, 100, 30), Atom.timeScale, 0.0001f, 5.0f);
//		if (timeScale != SphereScript.timeScale) {
//			Atom.timeScale = timeScale;
//		}

		if (sliderControls != null) {
			GUI.skin = sliderControls;
		}

		GUI.Label (new Rect (25, 25, 250, 20), "Temperature: " + TemperatureCalc.desiredTemperature);
		float newTemp = GUI.VerticalSlider (new Rect (75, 55, 30, (Screen.height - 135)), TemperatureCalc.desiredTemperature, 0.001f, 1.0f);
		if (newTemp != SphereScript.desiredTemperature) {
			TemperatureCalc.desiredTemperature = newTemp;
		}

		GUI.Label (new Rect (Screen.width - 100, 25, 250, 20), "Time: " + Time.time);

		if (addGraphicCopper) {
			Color guiColor = Color.white;
			guiColor.a = 0.25f;
			GUI.color = guiColor;
			GUI.DrawTexture(new Rect((Input.mousePosition.x - 25.0f), (Screen.height - Input.mousePosition.y) - 25.0f, 50.0f, 50.0f), addCopperTexture);
			GUI.color = Color.white;
		}

		if (addGraphicGold) {
			Color guiColor = Color.white;
			guiColor.a = 0.25f;
			GUI.color = guiColor;
			GUI.DrawTexture(new Rect((Input.mousePosition.x - 25.0f), (Screen.height - Input.mousePosition.y) - 25.0f, 50.0f, 50.0f), addGoldTexture);
			GUI.color = Color.white;
		}

		if (addGraphicPlatinum) {
			Color guiColor = Color.white;
			guiColor.a = 0.25f;
			GUI.color = guiColor;
			GUI.DrawTexture(new Rect((Input.mousePosition.x - 25.0f), (Screen.height - Input.mousePosition.y) - 25.0f, 50.0f, 50.0f), addPlatinumTexture);
			GUI.color = Color.white;
		}

		if (GUI.RepeatButton (new Rect (75, Screen.height - 75, 75, 75), copperTexture)) {
			if(!clicked){
				clicked = true;
				startTime = Time.time;
				first = true;
			}
			else{
				float currTime = Time.time - startTime;
				if(currTime > holdTime){
					if(first){
						first = false;
						addGraphicCopper = true;
					}
				}
			}
		}

		if (GUI.RepeatButton (new Rect (170, Screen.height - 75, 75, 75), goldTexture)) {
			if(!clicked){
				clicked = true;
				startTime = Time.time;
				first = true;
			}
			else{
				float currTime = Time.time - startTime;
				if(currTime > holdTime){
					if(first){
						first = false;
						addGraphicGold = true;
					}
				}
			}
		}

		if (GUI.RepeatButton (new Rect (265, Screen.height - 75, 75, 75), platinumTexture)) {
			if(!clicked){
				clicked = true;
				startTime = Time.time;
				first = true;
			}
			else{
				float currTime = Time.time - startTime;
				if(currTime > holdTime){
					if(first){
						first = false;
						addGraphicPlatinum = true;
					}
				}
			}
		}

		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		GameObject atomToDelete = null;
		bool holdingAtom = false;
		for (int i = 0; i < allMolecules.Length; i++) {
			Atom atomScript = allMolecules[i].GetComponent<Atom>();
			if(atomScript.held){
				holdingAtom = true;
				atomToDelete = allMolecules[i];
				break;
			}
		}
		
		if (holdingAtom && Input.mousePosition.x < 435 && Input.mousePosition.x > 360 && Input.mousePosition.y < 75 && Input.mousePosition.y > 0) {
			Color guiColor = Color.red;
			guiColor.a = 0.25f;
			GUI.color = guiColor;
			if(Input.GetMouseButtonUp(0) && atomToDelete != null){
				Destroy(atomToDelete);
			}
		}
		
		GUI.DrawTexture (new Rect (360, Screen.height - 75, 75, 75), garbageTexture);

		
		if (Input.GetMouseButtonUp (0)) {

			if(addGraphicCopper && Input.mousePosition.x < Screen.width && Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height){
				Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20.0f));
				Quaternion curRotation = Quaternion.Euler(0, 0, 0);
				Instantiate(copperPrefab, curPosition, curRotation);
			}

			if(addGraphicGold && Input.mousePosition.x < Screen.width && Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height){
				Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20.0f));
				Quaternion curRotation = Quaternion.Euler(0, 0, 0);
				Instantiate(goldPrefab, curPosition, curRotation);
			}

			if(addGraphicPlatinum && Input.mousePosition.x < Screen.width && Input.mousePosition.x > 0 && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height){
				Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20.0f));
				Quaternion curRotation = Quaternion.Euler(0, 0, 0);
				Instantiate(platinumPrefab, curPosition, curRotation);
			}
			
			addGraphicCopper = false;
			addGraphicGold = false;
			addGraphicPlatinum = false;
			first = true;
			clicked = false;
			startTime = 0.0f;
		}

	}

}
