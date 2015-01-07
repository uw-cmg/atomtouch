using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayAtomInfo : MonoBehaviour {
	
	

	
	Text text;
	public bool toDisplay;
	

	// Use this for initialization
	void Awake () {


		text = GetComponent<Text> ();
		//toDisplay = true;
		
		
	}
	
	// Update is called once per frame
	public void FixedUpdate () {
		
		if (toDisplay == false) {
						text.enabled = false;
				
		} 
		if(toDisplay==true){
			text.enabled=true;		
		}



				
	}
	
	public void displayInfo(){

		if (toDisplay == true) {
			toDisplay = false;
			//this.text.enabled = false;
		}
		else if(toDisplay ==false){
			toDisplay = true;
			//text.enabled = true;
		}

		//text.enabled = false;
		
	}
	

}
