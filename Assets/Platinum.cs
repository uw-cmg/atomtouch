using UnityEngine;
using System.Collections;
using System;

public class Platinum : Atom {

	private Color currentColor;
	private Color platinumColor = new Color (.898f, .8941f, 0.8863f, 1.0f);
	private float sigmaValue = 2.5394f;

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
			currentColor = platinumColor;
		}
	}

	public override void ChangeColor (Color color){
		if (color == Color.black) {
			currentColor = platinumColor;
		}
		else{
			currentColor = color;
		}
	}

	void Start () {
		SetSelected (false);
		gameObject.transform.localScale = new Vector3(sigmaValue * .5f, sigmaValue * .5f, sigmaValue * .5f);
	}

}
