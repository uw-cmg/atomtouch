using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Globalization;
using System;

public class DevDebug : MonoBehaviour {
	//refs 
	public GameObject timestepInputVal;
	public GameObject maxTimeInputVal;
	public GameObject timescaleInputVal;
	//walk around for unity bug: input field does not get updated
	//solution: disabling and enabling input fields
	public GameObject timestepInput;
	public GameObject maxTimeInput;
	public GameObject timescaleInput;

	// Use this for initialization
	void Awake(){
		timestepInput.GetComponent<InputField>().enabled = false;
		maxTimeInput.GetComponent<InputField>().enabled = false;
		timescaleInput.GetComponent<InputField>().enabled = false;

		timestepInputVal.GetComponent<Text>().text = Time.fixedDeltaTime.ToString("");
		maxTimeInputVal.GetComponent<Text>().text = Time.maximumDeltaTime.ToString("");
		timescaleInputVal.GetComponent<Text>().text = Time.timeScale.ToString("");
	}
	public void OnEndInput_TimeStep(){
		string text = timestepInputVal.GetComponent<Text>().text;
		if(text == "" || text == null){
			timestepInput.GetComponent<InputField>().enabled = false;
			timestepInputVal.GetComponent<Text>().text = Time.fixedDeltaTime.ToString("");
		}
	}
	public void OnEndInput_MaxTime(){
		string text = maxTimeInputVal.GetComponent<Text>().text;
		if(text == "" || text == null){
			maxTimeInput.GetComponent<InputField>().enabled = false;
			maxTimeInputVal.GetComponent<Text>().text = Time.maximumDeltaTime.ToString("");
		}
	}
	public void OnEndInput_TimeScale(){
		string text = timescaleInputVal.GetComponent<Text>().text;
		if(text == "" || text == null){
			timescaleInput.GetComponent<InputField>().enabled = false;
			timescaleInputVal.GetComponent<Text>().text = Time.timeScale.ToString("");
		}
	}
	public void OnClickInputField(GameObject inputField){
		inputField.GetComponent<InputField>().enabled = true;
		inputField.GetComponent<InputField>().Select();
		inputField.GetComponent<InputField>().ActivateInputField();
	}

	public void OnClickPlusMinus_Timestep(bool plus){
		float step = 0.0001f;
		if(!plus){
			step *= -1;
		}
		timestepInput.GetComponent<InputField>().enabled = false;

		float currVal = float.Parse(timestepInputVal.GetComponent<Text>().text);
		if(currVal + step > 0){
			timestepInputVal.GetComponent<Text>().text = (currVal + step).ToString("");
		}
	}

	public void OnClickPlusMinus_MaxTime(bool plus){
		float step = 0.001f;
		if(!plus){
			step *= -1;
		}
		maxTimeInput.GetComponent<InputField>().enabled = false;

		float currVal = float.Parse(maxTimeInputVal.GetComponent<Text>().text);
		if(currVal + step > 0){
			maxTimeInputVal.GetComponent<Text>().text = (currVal + step).ToString("");
		}
	}



	public void OnClickPlusMinus_TimeScale(bool plus){
		float step = 0.1f;
		if(!plus){
			step *= -1;
		}
		timescaleInput.GetComponent<InputField>().enabled = false;

		float currVal = float.Parse(timescaleInputVal.GetComponent<Text>().text);
		if(currVal + step > 0){
			timescaleInputVal.GetComponent<Text>().text = (currVal + step).ToString("");
		}
	}
	public void OnSubmitTimeChange(){
		string timestepStr = timestepInputVal.GetComponent<Text>().text;
		float timestep = float.Parse(timestepStr);
		Debug.Log(timestep);
		
		string maxTimeStr = maxTimeInputVal.GetComponent<Text>().text;
		float maxTime = float.Parse(maxTimeStr);
		Debug.Log(maxTime);

		string timescaleStr = timescaleInputVal.GetComponent<Text>().text;
		float timescale = float.Parse(timescaleStr);
		Debug.Log(timescale);
		
		if(maxTime < timestep){
			return;
		}
		Time.fixedDeltaTime = timestep;
		Time.maximumDeltaTime = maxTime;
		Time.timeScale = timescale;
	}
}
