using UnityEngine;
using System.Collections;
using System;

public class Graph : MonoBehaviour {

	public Material mat;
	private Queue dataPoints;
	private float startTime;

	//graph variables
	public float xCoord;
	public float yCoord;
	public float width = 180.0f;
	public float height = 184.0f;
	public float refreshInterval = .1f;
	public float lineWidth = .015f;
	private float zDepth = 5.0f;
	public float spacing = 15.0f;
	private float maxDataPoints;
	private float dataMaximum = -1 * Mathf.Pow(10, -14);
	private float dataMinimum = -1 * Mathf.Pow(10, -16);
	private float lowTime;
	private float highTime;
	private bool first;
	public string yUnitLabel = "J";
	public string xUnitLabel = "ps";
	public string graphLabel = "Potential Energy vs Time";
	public Color axisColor = Color.red;
	public Color lineColor = Color.yellow;

	void Start () {

		xCoord = Screen.width - 250;
		yCoord = 70;
		first = true;
		maxDataPoints = (width / spacing) + 1;
		dataPoints = new Queue ();
		startTime = Time.realtimeSinceStartup;
		lowTime = 0.0f;
		highTime = maxDataPoints * refreshInterval;
	}

	void Update(){

		if (AtomTouchGUI.currentTimeSpeed != StaticVariables.TimeSpeed.Stopped) {
			StaticVariables.currentTime += Time.deltaTime;
		}

		if ((Time.time - startTime > refreshInterval && !StaticVariables.pauseTime) || first) {
			if(dataPoints.Count < maxDataPoints){
				dataPoints.Enqueue(PotentialEnergy.finalPotentialEnergy);
			}
			else{
				dataPoints.Dequeue ();
				dataPoints.Enqueue(PotentialEnergy.finalPotentialEnergy);
				lowTime += refreshInterval;
				highTime += refreshInterval;
			}
			first = false;
			startTime = Time.time;
		}

	}

	public void RecomputeMaxDataPoints(){
		maxDataPoints = (width / spacing) + 1;
		highTime = maxDataPoints * refreshInterval;
	}

	void OnGUI(){

		GUIStyle graphText = GUI.skin.label;
		graphText.alignment = TextAnchor.MiddleLeft;
		graphText.fontSize = 14;
		graphText.normal.textColor = Color.white;
		AtomTouchGUI atomGUI = Camera.main.GetComponent<AtomTouchGUI> ();
		if (atomGUI.dataPanelActive) {
			GUI.Label (new Rect (xCoord + width/2.0f - 60, Screen.height - yCoord, 200, 20), graphLabel);
			GUI.Label (new Rect (xCoord - 53, Screen.height - yCoord - 165, 100, 20), (dataMaximum).ToString () + yUnitLabel);
			GUI.Label (new Rect (xCoord - 53, Screen.height - yCoord - 15, 100, 20), (dataMinimum).ToString () + yUnitLabel);
			GUI.Label (new Rect (xCoord - 5, Screen.height - yCoord, 100, 20), (Math.Round (lowTime)).ToString () + xUnitLabel);
			GUI.Label (new Rect (xCoord + width - 35.0f, Screen.height - yCoord, 100, 20), (Math.Round(highTime)).ToString() + xUnitLabel);
		}

	}
	
	void OnPostRender(){

		AtomTouchGUI atomGUI = Camera.main.GetComponent<AtomTouchGUI> ();
		if (atomGUI.dataPanelActive) {
			Vector3 upperLeft = camera.ScreenToWorldPoint (new Vector3 (xCoord, (yCoord+height), zDepth));
			Vector3 lowerLeft = camera.ScreenToWorldPoint (new Vector3(xCoord, yCoord, zDepth));
			Vector3 upperRight = camera.ScreenToWorldPoint (new Vector3 (xCoord + width, (yCoord+height), zDepth));
			Vector3 lowerRight = camera.ScreenToWorldPoint (new Vector3(xCoord + width, yCoord, zDepth));
			Color customColor = new Color (0.5f, 0.5f, 0.5f, 1.0f);
			StaticVariables.DrawQuad (upperLeft, upperRight, lowerLeft, lowerRight, customColor, mat);
			
			//horizontal line
			StaticVariables.DrawLine (lowerLeft, lowerRight, axisColor, axisColor, lineWidth, mat);
			
			//vertical line
			StaticVariables.DrawLine (upperLeft, lowerLeft, axisColor, axisColor, lineWidth, mat);
			
			object[] dataPointArray = dataPoints.ToArray ();
			for (int i = 0; i < dataPointArray.Length - 1; i++) {
				float firstPercentage = (float)dataPointArray[i] / (dataMaximum - dataMinimum);
				float secondPercentage = (float)dataPointArray[i+1] / (dataMaximum - dataMinimum);
				if(firstPercentage > 1.0f){
					firstPercentage = 1.0f;
				}
				else if(firstPercentage < 0.0f){
					firstPercentage = 0.0f;
				}
				if(secondPercentage > 1.0f){
					secondPercentage = 1.0f;
				}
				else if(secondPercentage < 0.0f){
					secondPercentage = 0.0f;
				}
						
				float firstYAddition = firstPercentage * height;
				float secondYAddition = secondPercentage * height;
				
				Vector3 firstPoint = camera.ScreenToWorldPoint(new Vector3(xCoord + (i*spacing), yCoord + firstYAddition, zDepth));
				Vector3 secondPoint = camera.ScreenToWorldPoint(new Vector3(xCoord + ((i+1)*spacing), yCoord + secondYAddition, zDepth));
				StaticVariables.DrawLine(firstPoint, secondPoint, lineColor, lineColor, lineWidth, mat);
			}
		}



	}
}
