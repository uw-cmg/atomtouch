using UnityEngine;
using System.Collections;

public class Graph : MonoBehaviour {

	public Material mat;
	public TextMesh textMeshPrefab;
	private Queue dataPoints;
	private float startTime;

	//graph variables
	private float xCoord;
	private float yCoord;
	private float width = 180.0f;
	private float height = 184.0f;
	private float lineWidth = .015f;
	private float zDepth = 5.0f;
	private float refreshInterval = 2.0f;
	private float spacing = 15.0f;
	private float maxDataPoints;
	private float dataMaximum = StaticVariables.tempRangeHigh;
	private float dataMinimum = StaticVariables.tempRangeLow;
	private float lowTime;
	private float highTime;
	private bool first;

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

		//change this to be in terms of xCoord
		GUI.Label (new Rect (Screen.width - 235, Screen.height - 280, 200, 20), "Temperature vs Time");
		GUI.Label (new Rect (xCoord - 40, Screen.height - 260, 100, 20), (StaticVariables.tempRangeHigh).ToString () + "K");
		GUI.Label (new Rect (xCoord - 40, Screen.height - 85, 100, 20), (StaticVariables.tempRangeLow).ToString () + "K");
		GUI.Label (new Rect (xCoord - 5, Screen.height - 70, 100, 20), (lowTime).ToString () + "s");
		GUI.Label (new Rect (xCoord + width - 5, Screen.height - 70, 100, 20), (highTime).ToString() + "s");
		//print ("lowTimePosition: " + new Rect (Screen.width - 265, Screen.height - 70, 100, 20));
		//print ("screen height: " + Screen.height);
	}
	
	void OnPostRender(){

		Quaternion cameraRotation = Camera.main.transform.rotation;

		Vector3 upperLeft = camera.ScreenToWorldPoint (new Vector3 (xCoord, (yCoord+height), zDepth));
		Vector3 lowerLeft = camera.ScreenToWorldPoint (new Vector3(xCoord, yCoord, zDepth));
		Vector3 upperRight = camera.ScreenToWorldPoint (new Vector3 (xCoord + width, (yCoord+height), zDepth));
		Vector3 lowerRight = camera.ScreenToWorldPoint (new Vector3(xCoord + width, yCoord, zDepth));
		Color customColor = new Color (1.0f, 1.0f, 1.0f, .2f);
		StaticVariables.DrawQuad (upperLeft, upperRight, lowerLeft, lowerRight, customColor, mat);

		//horizontal line
		StaticVariables.DrawLine (lowerLeft, lowerRight, Color.red, Color.red, lineWidth, mat);

		//vertical line
		StaticVariables.DrawLine (upperLeft, lowerLeft, Color.red, Color.red, lineWidth, mat);

		object[] dataPointArray = dataPoints.ToArray ();
		for (int i = 0; i < dataPointArray.Length - 1; i++) {
			float firstPercentage = (float)dataPointArray[i] / (dataMaximum - dataMinimum);
			float secondPercentage = (float)dataPointArray[i+1] / (dataMaximum - dataMinimum);

			float firstYAddition = firstPercentage * height;
			float secondYAddition = secondPercentage * height;

			Vector3 firstPoint = camera.ScreenToWorldPoint(new Vector3(xCoord + (i*spacing), yCoord + firstYAddition, zDepth));
			Vector3 secondPoint = camera.ScreenToWorldPoint(new Vector3(xCoord + ((i+1)*spacing), yCoord + secondYAddition, zDepth));
			StaticVariables.DrawLine(firstPoint, secondPoint, Color.yellow, Color.yellow, lineWidth, mat);
		}

	}
}
