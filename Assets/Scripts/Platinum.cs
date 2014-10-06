/**
 * Class: Platinum.cs
 * Created by: Justin Moeller
 * Description: This class defines anything that is platinum specific, and NOT related to all of
 * the atoms. This class is derived from the base class of Atom.cs, and takes on all of its behavior.
 * It must override all of the abstract variables and functions that are defined in Atom.cs, such as
 * atomName, epsilon, sigma, massamu, SetSelected(), and SetTransparent().
 * 
 * 
 **/


using UnityEngine;
using System.Collections;
using System;

public class Platinum : Atom {
	
	private float sigmaValue = 2.5394f;
	public Material platinumMaterial;
	public Material selectedMaterial;
	public Material transparentMaterial;

	public override String atomName { 
		get{ return "Platinum"; } 
	}

	public override int atomID {
		get{ return 3;}
	}
	
	public override float epsilon {
		get { return ((float)(1.0922 * Math.Pow(10, -19))); } // J
	}
		
	public override float sigma {
		get { return sigmaValue; }
	}
	
	protected override float massamu {
		get { return 195.084f; } //amu
	}

	// These are just dummy variables for platinum
	public override float buck_A {
		get { return 0.0f; } //units of [J]
	}
	
	public override float buck_B {
		get { return 0.0f; } //units of [1/Angstrom]
	}
	
	public override float buck_C {
		get { return 0.0f; } //units of [J.Anstrom^6]
	}
	
	public override float buck_D {
		get { return 0.0f; } //units of [J.Angstrom^8]
	}
	
	public override float Q_eff {
		get { return 0.0f; } //units of Coulomb
	}

	public override void SetSelected (bool selected){
		if (selected) {
			gameObject.renderer.material = selectedMaterial;
		}
		else{
			gameObject.renderer.material = platinumMaterial;
		}
	}

	public override void SetTransparent(bool transparent){
		if (transparent) {
			gameObject.renderer.material = transparentMaterial;
		}
		else{
			gameObject.renderer.material = platinumMaterial;
		}
	}
	

	void Start () {
		//make the atom its original color to start
		SetSelected (false);
		//scale the atom according to sigma
		gameObject.transform.localScale = new Vector3(sigmaValue * .5f, sigmaValue * .5f, sigmaValue * .5f);
	}

}
