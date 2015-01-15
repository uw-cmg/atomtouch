using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdateVolume : MonoBehaviour {

	Text text;
	CreateEnvironment createEnvironment;
	public static VolUnitType volUnit;
	public enum VolUnitType{
		Angstrom,
		Nanometer
	}
	void Awake(){
		volUnit = VolUnitType.Nanometer;
	}
	void Start () {
		text = GetComponent<Text>();
		createEnvironment = CreateEnvironment.myEnvironment;
	}
	
	// Update is called once per frame
	void Update () {
		//nm^3
		//1 nm = 10 angstroms
		if(volUnit == VolUnitType.Nanometer){
			text.text =  "Vol" + System.Environment.NewLine + 
			(createEnvironment.volume*0.1f*0.1f*0.1f).ToString("0.00")+ " nm^3";
		}else if(volUnit == VolUnitType.Angstrom){
			text.text = "Vol" + System.Environment.NewLine +
			(createEnvironment.volume).ToString("0.00")+ " Å^3";
		}
		
		
	}
}
