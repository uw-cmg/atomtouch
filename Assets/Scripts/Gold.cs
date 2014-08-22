using UnityEngine;
using System.Collections;
using System;

public class Gold : Atom
{
	private float sigmaValue = 2.6367f;
	public Material goldMaterial;
	public Material selectedMaterial;
	public Material transparentMaterial;

	public override String atomName 
	{ 
		get{ return "Gold"; } 
	}
	
	public override float epsilon
	{
		get { return 5152.9f * 1.381f * (float)Math.Pow (10, -23); } // J
	}
	
	public override float sigma
	{
		get { return sigmaValue; }
	}
	
	protected override float massamu
	{
		get { return 196.967f; } //amu
	}

	public override void SetSelected (bool selected){
		if (selected) {
			gameObject.renderer.material = selectedMaterial;
		}
		else{
			gameObject.renderer.material = goldMaterial;
		}
	}

	public override void SetTransparent(bool transparent){
		if (transparent) {
			gameObject.renderer.material = transparentMaterial;
		}
		else{
			gameObject.renderer.material = goldMaterial;
		}
	}

	void Start ()
	{
		SetSelected (false);
		gameObject.transform.localScale = new Vector3(sigmaValue * .5f, sigmaValue * .5f, sigmaValue * .5f);
	}
}

