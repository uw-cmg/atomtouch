﻿using UnityEngine;
using System.Collections;
using System;

public class Copper : Atom {
	
	private float sigmaValue = 2.3374f;
	public Material copperMaterial;
	public Material selectedMaterial;
	public Material transparentMaterial;
		
	public override String atomName 
	{ 
		get{ return "Copper"; } 
	}

	public override float epsilon
	{
		get { return ((float)(6.537 * Math.Pow(10, -20))); } // J
	}
		
	public override float sigma
	{
		get { return sigmaValue; }
	}
	
	protected override float massamu
	{
		get { return 63.546f; } //amu
	}

	public override void SetSelected (bool selected){
		if (selected) {
			gameObject.renderer.material = selectedMaterial;
		}
		else{
			gameObject.renderer.material = copperMaterial;
		}
	}

	public override void SetTransparent(bool transparent){
		if (transparent) {
			gameObject.renderer.material = transparentMaterial;
		}
		else{
			gameObject.renderer.material = copperMaterial;
		}
	}
		
	void Start () {
		SetSelected (false);
		gameObject.transform.localScale = new Vector3(sigmaValue * .5f, sigmaValue * .5f, sigmaValue * .5f);
	}

}