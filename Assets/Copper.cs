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


	// Use this for initialization
	void Start () {
		Color moleculeColor = new Color(.7216f, .451f, 0.2f, 1.0f);
		gameObject.renderer.material.color = moleculeColor;
		gameObject.transform.localScale = new Vector3(sigma, sigma, sigma);
	}
}
