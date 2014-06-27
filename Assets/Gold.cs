using UnityEngine;
using System.Collections;
using System;

public class Gold : Atom
{
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

	void Start ()
	{
		Color moleculeColor = new Color(1.0f, .8431f, 0.0f, 1.0f);
		gameObject.renderer.material.color = moleculeColor;
	}
}

