using UnityEngine;
using System.Collections;
using System;

public class Copper : Atom {

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
			return new Color(.7216f, .451f, 0.2f, 1.0f);
		}
	}
	
	void Start () {
		gameObject.renderer.material.color = color;
		gameObject.transform.localScale = new Vector3(sigma * .5f, sigma * .5f, sigma * .5f);
	}
}
