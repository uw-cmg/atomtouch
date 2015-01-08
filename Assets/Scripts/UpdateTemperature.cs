using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdateTemperature : MonoBehaviour {

	Text text;
	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		//if(oldTemperature < 0)
		text.text = "Temp" + System.Environment.NewLine +  StaticVariables.desiredTemperature + "K";
	}
}
