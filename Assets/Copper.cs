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

	public override Color color {
		get {
			return currentColor;
		}
	}

	protected override void SetSelected (bool selected){
		if (selected) {
			currentColor = new Color(0.0f, 1.0f, 0.0f);
		}
		else{
			currentColor = copperColor;
		}
	}

	public override void ChangeColor (Color color){
		if (color == Color.black) {
			currentColor = copperColor;
		}
		else{
			currentColor = color;
		}
	}

	void Start () {
		SetSelected (false);
		gameObject.transform.localScale = new Vector3(sigma * .5f, sigma * .5f, sigma * .5f);
		//gameObject.rigidbody.velocity = new Vector3(0.0f, 5.0f, 0.0f);
	}

}
