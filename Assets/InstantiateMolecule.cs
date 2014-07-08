using UnityEngine;
using System.Collections;

public class InstantiateMolecule : MonoBehaviour {

	public Rigidbody copperPrefab;
	public Rigidbody goldPrefab;
	public Rigidbody platinumPrefab;
	public GUISkin sliderControls;
	public Texture copperTexture;
	public Texture addCopperTexture;
	public bool addGraphic = false;

	private bool clicked = false;
	private float startTime = 0.0f;
	private bool first = true;
	public float holdTime = 0.05f;



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

		if (addGraphic) {
			Color guiColor = Color.white;
			guiColor.a = 0.25f;
			GUI.color = guiColor;
			GUI.DrawTexture(new Rect((Input.mousePosition.x - 25.0f), (Screen.height - Input.mousePosition.y) - 25.0f, 50.0f, 50.0f), addCopperTexture);
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
						addGraphic = true;
					}
				}
			}
		}

		if (Input.GetMouseButtonUp (0)) {

			if(addGraphic){
				Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20.0f));
				Quaternion curRotation = Quaternion.Euler(0, 0, 0);
				Instantiate(copperPrefab, curPosition, curRotation);
			}

			addGraphic = false;
			first = true;
			clicked = false;
			startTime = 0.0f;
		}

	}

}
