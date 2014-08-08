using UnityEngine;
using System.Collections;
using System;

public class Gold : Atom
{
	private Color currentColor;
	private Color goldColor;
	private float sigmaValue = 2.6367f;

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
			currentColor = goldColor;
		}
	}

	void Start ()
	{
		goldColor = new Color (1.0f, .8431f, 0.0f, 1.0f);
		SetSelected (false);
		gameObject.transform.localScale = new Vector3(sigmaValue * .5f, sigmaValue * .5f, sigmaValue * .5f);
	}
}

