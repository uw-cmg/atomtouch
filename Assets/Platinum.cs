using UnityEngine;
using System.Collections;
using System;

public class Platinum : Atom {

	private Color currentColor;
	private Color platinumColor = new Color (.898f, .8941f, 0.8863f, 1.0f);

	protected override float epsilon
	{
		get { return ((float)(1.0922 * Math.Pow(10, -19))); } // J
	}
	
	protected override float sigma
	{
		get { return 2.5394f; } // m=Angstroms for Unit
	}
	
	protected override float massamu
	{
		get { return 195.084f; } //amu
	}

	protected override Color color {
		get {
			return platinumColor;
		}
	}

	protected override void ChangeColor (bool selected){
		if (selected) {
			platinumColor = new Color(0.0f, 1.0f, 0.0f);
		}
		else{
			platinumColor = platinumColor;
		}
	}

	protected override void ChangeIntersection (bool intersected){
		if (intersected) {
			currentColor = new Color(1.0f, 0.0f, 0.0f);
		}
		else{
			currentColor = platinumColor;
		}
	}

	void Start () {
		ChangeColor (false);
		gameObject.transform.localScale = new Vector3(sigma * .5f, sigma * .5f, sigma * .5f);
	}

}
