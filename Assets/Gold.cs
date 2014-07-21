using UnityEngine;
using System.Collections;
using System;

public class Gold : Atom
{
	private Color currentColor;
	private Color goldColor = new Color (1.0f, .8431f, 0.0f, 1.0f);

	protected override float epsilon
	{
		get { return 5152.9f * 1.381f * (float)Math.Pow (10, -23); } // J
	}

	protected override float sigma
	{
		get { return 2.6367f; } // m=Angstroms for Unit
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
			currentColor = new Color(0.0f, 1.0f, 0.0f);
		}
		else{
			currentColor = goldColor;
		}
	}

	public override void ChangeColor (Color color){
		if (color == Color.black) {
			currentColor = goldColor;
		}
		else{
			currentColor = color;
		}
	}

	void Start ()
	{
		SetSelected (false);
		gameObject.transform.localScale = new Vector3(sigma * .5f, sigma * .5f, sigma * .5f);
		//gameObject.rigidbody.velocity = new Vector3(0.0f, 5.0f, 0.0f);
	}
}

