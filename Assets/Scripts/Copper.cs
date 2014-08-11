using UnityEngine;
using System.Collections;
using System;

public class Copper : Atom {

	private Color currentColor;
	private Color copperColor;
	private float sigmaValue = 2.3374f;
		
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

	public override Color color {
		get {
			return currentColor;
		}
	}

	protected override void SetSelected (bool selected){
		if (selected) {
			currentColor = StaticVariables.selectedColor;
		}
		else{
			currentColor = copperColor;
		}
	}
		
	void Start () {
		copperColor = new Color (.7216f, .451f, 0.2f, 1.0f);
		SetSelected (false);
		gameObject.transform.localScale = new Vector3(sigmaValue * .5f, sigmaValue * .5f, sigmaValue * .5f);
	}

}
