using UnityEngine;
using System.Collections;
using System;

public class Platinum : Atom {
	
	private float sigmaValue = 2.5394f;
	public Material platinumMaterial;
	public Material selectedMaterial;
	public Material transparentMaterial;

	public override String atomName 
	{ 
		get{ return "Platinum"; } 
	}
	
	public override float epsilon
	{
		get { return ((float)(1.0922 * Math.Pow(10, -19))); } // J
	}
		
	public override float sigma
	{
		get { return sigmaValue; }
	}
	
	protected override float massamu
	{
		get { return 195.084f; } //amu
	}


	protected override void SetSelected (bool selected){
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
		SetSelected (false);
		gameObject.transform.localScale = new Vector3(sigmaValue * .5f, sigmaValue * .5f, sigmaValue * .5f);
	}

}
