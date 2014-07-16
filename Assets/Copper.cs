using UnityEngine;
using System.Collections;
using System;

public class Copper : Atom {

	private Color currentColor;
	private Color copperColor = new Color (.7216f, .451f, 0.2f, 1.0f);

	protected override float epsilon
	{
		get { return ((float)(6.537 * Math.Pow(10, -20))); } // J
	}

	protected override float sigma
	{
		get { return 2.3374f; } // m=Angstroms for Unit
	}
	
	protected override float massamu
	{
		get { return 63.546f; } //amu
	}

	protected override Color color {
		get {
			return currentColor;
		}
	}

	protected override void ChangeColor (bool selected){
		if (selected) {
			currentColor = new Color(0.0f, 1.0f, 0.0f);
		}
		else{
			currentColor = copperColor;
		}
	}

	protected override void ChangeIntersection (bool intersected){
		if (intersected) {
			currentColor = new Color(1.0f, 0.0f, 0.0f);
		}
		else{
			currentColor = copperColor;
		}
	}

	void Start () {
		ChangeColor (false);
		gameObject.transform.localScale = new Vector3(sigma * .5f, sigma * .5f, sigma * .5f);
	}

}
