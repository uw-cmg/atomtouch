using UnityEngine;
using System.Collections;
using System;

public class Gold : Atom
{
	private Color goldColor;

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

	protected override Color color {
		get {
			return goldColor;
		}
	}

	protected override void ChangeColor (bool selected){
		if (selected) {
			goldColor = new Color(0.0f, 1.0f, 0.0f);
		}
		else{
			goldColor = new Color(1.0f, .8431f, 0.0f, 1.0f);
		}
	}
	
	void Start ()
	{
		ChangeColor (false);
		gameObject.transform.localScale = new Vector3(sigma * .5f, sigma * .5f, sigma * .5f);
	}
}

