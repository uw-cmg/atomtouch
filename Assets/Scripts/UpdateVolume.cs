using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdateVolume : MonoBehaviour {

	Text text;
	CreateEnvironment createEnvironment;
	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
		createEnvironment = CreateEnvironment.myEnvironment;
	}
	
	// Update is called once per frame
	void Update () {
		
		text.text =  "Vol" + System.Environment.NewLine + (createEnvironment.volume*0.1f*0.1f*0.1f).ToString("0.00")+ " nm^3";
		
	}
}
