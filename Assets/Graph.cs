using UnityEngine;
using System.Collections;

public class Graph : MonoBehaviour {

	public Material mat;
	private Queue dataPoints;
	private float startTime;

	//graph variables
	private float xCoord = 3.75f;
	private float yCoord = -2.3f;
	private float width = 1.5f;
	private float height = 1.5f;
	private float lineWidth = .015f;
	private float zDepth = 5.0f;
	private float refreshInterval = 2.0f;
	private float spacing = .1f;
	private float maxDataPoints;
	private float dataMaximum = StaticVariables.tempRangeHigh;
	private float dataMinimum = StaticVariables.tempRangeLow;
	private float lowTime;
	private float highTime;
	private bool first;

	void Start () {
	
		first = true;
		maxDataPoints = (width / spacing) + 1;
		dataPoints = new Queue ();
		startTime = Time.realtimeSinceStartup;
		lowTime = 0.0f;
		highTime = maxDataPoints * refreshInterval;
		
	}

	void Update(){

		if ((Time.realtimeSinceStartup - startTime > refreshInterval && !StaticVariables.pauseTime) || first) {
			if(dataPoints.Count < maxDataPoints){
				dataPoints.Enqueue(TemperatureCalc.desiredTemperature);
			}
			else{
				dataPoints.Dequeue ();
				dataPoints.Enqueue(TemperatureCalc.desiredTemperature);
				lowTime += 2.0f;
				highTime += 2.0f;
			}
			first = false;
			startTime = Time.realtimeSinceStartup;
		}

	}

	void OnGUI(){

		GUI.Label (new Rect (Screen.width - 235, Screen.height - 280, 200, 20), "Temperature vs Time");
		GUI.Label (new Rect (Screen.width - 305, Screen.height - 260, 100, 20), (StaticVariables.tempRangeHigh).ToString () + "K");
		GUI.Label (new Rect (Screen.width - 305, Screen.height - 90, 100, 20), (StaticVariables.tempRangeLow).ToString () + "K");
		GUI.Label (new Rect (Screen.width - 265, Screen.height - 70, 100, 20), (lowTime).ToString () + "s");
		GUI.Label (new Rect (Screen.width - 90, Screen.height - 70, 100, 20), (highTime).ToString() + "s");
	}
	
	void OnPostRender(){

		Quaternion cameraRotation = Camera.main.transform.rotation;

		Vector3 upperLeft = cameraRotation * new Vector3 (xCoord, yCoord+height, zDepth);
		upperLeft += Camera.main.transform.position;
		Vector3 upperRight = cameraRotation * new Vector3 (xCoord+width, yCoord+height, zDepth);
		upperRight += Camera.main.transform.position;
		Vector3 lowerLeft = cameraRotation * new Vector3 (xCoord, yCoord, zDepth);
		lowerLeft += Camera.main.transform.position;
		Vector3 lowerRight = cameraRotation * new Vector3 (xCoord+width, yCoord, zDepth);
		lowerRight += Camera.main.transform.position;
		Color customColor = new Color (1.0f, 1.0f, 1.0f, .2f);
		StaticVariables.DrawQuad (upperLeft, upperRight, lowerLeft, lowerRight, customColor, mat);

		//horizontal line
		Vector3 vectorToAdd = cameraRotation * new Vector3 (xCoord + (width/2.0f), yCoord, zDepth);
		Vector3 midpoint = Camera.main.transform.position + vectorToAdd;
		Vector3 startingXDiff = cameraRotation * new Vector3 (-(width/2.0f), 0.0f, 0.0f);
		Vector3 endingXDiff = cameraRotation * new Vector3 ((width/2.0f), 0.0f, 0.0f);
		Vector3 startingPos = midpoint + startingXDiff;
		Vector3 endingPos = midpoint + endingXDiff;
		StaticVariables.DrawLine (startingPos, endingPos, Color.red, Color.red, lineWidth, mat);

		//vertical line
		Vector3 vectorToAdd2 = cameraRotation * new Vector3 (xCoord, yCoord + (height/2.0f), zDepth);
		Vector3 midpoint2 = Camera.main.transform.position + vectorToAdd2;
		Vector3 startingYDiff = cameraRotation * new Vector3 (0.0f, height/2.0f, 0.0f);
		Vector3 endingYDiff = cameraRotation * new Vector3 (0.0f, -(height/2.0f), 0.0f);
		Vector3 startingPos2 = midpoint2 + startingYDiff;
		Vector3 endingPos2 = midpoint2 + endingYDiff;
		StaticVariables.DrawLine (startingPos2, endingPos2, Color.red, Color.red, lineWidth, mat);

		object[] dataPointArray = dataPoints.ToArray ();
		for (int i = 0; i < dataPointArray.Length - 1; i++) {
			float firstPercentage = (float)dataPointArray[i] / (dataMaximum - dataMinimum);
			float secondPercentage = (float)dataPointArray[i+1] / (dataMaximum - dataMinimum);
			float firstYAddition = firstPercentage * height;
			float secondYAddition = secondPercentage * height;
			Vector3 diff1 = cameraRotation * new Vector3(xCoord + (i*spacing), yCoord + firstYAddition, zDepth);
			Vector3 firstPoint3d = Camera.main.transform.position + diff1;
			Vector3 diff2 = cameraRotation * new Vector3(xCoord + ((i+1)*spacing), yCoord + secondYAddition, zDepth);
			Vector3 secondPoint3d = Camera.main.transform.position + diff2;
			StaticVariables.DrawLine(firstPoint3d, secondPoint3d, Color.yellow, Color.yellow, lineWidth, mat);
		}

	}
}
