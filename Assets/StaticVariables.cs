using UnityEngine;
using System.Collections;
using System;

public class StaticVariables {

	public static float MDTimestep = 0.5f * (float) Math.Pow (10, -15);
	//Suppose every FixedUpdate physics interval (e.g. 0.02 seconds) is the
	//Molecular Dynamics timestep of 0.5 * 10^-15 seconds
	public static float fixedUpdateIntervalToRealTime = MDTimestep / Time.fixedDeltaTime;
	//do not scale temperature all at once
	public static float alphaDrag = 1.0f * Time.fixedDeltaTime;

	//Boltzmann constant in J/K
	public static float kB = 1.381f * (float) Math.Pow(10,-23);

	//Convert units of 100 amu to kg
	public static float mass100amuToKg = 100f * 1.6605f * (float) Math.Pow(10,-27); 

	//Convert units of Angstroms to meters
	public static float angstromsToMeters = (float) Math.Pow (10,-10);

	//Delay before any temperature effects are applied
	public static float tempDelay = 5.0f;

	//Cutoff for "seeing" other atoms, in Angstroms
	//multiplied by sigma for Lennard-Jones potential
	public static float cutoff = 2.5f; //mutliplier for cutoff 

	//When r_ij is small, the Lennard-Jones potential is extremely large.
	//At a certain r_min, we will substitute the L-J potential with a function that
	//curves to a constant as r_ij goes to zero.

	//Multiplier for transition between actual L-J potential and curve to constant
	//    This number will be multiplied by sigma to find the transition distance
	public static float r_min_multiplier = 0.75f;

	//melting temperatures
	//Copper, 1358 K

	//Temperature slider bounds in K
	//public static float tempRangeLow = 0.0000001f; 
	public static float tempRangeLow = 0.01f;
	public static float tempRangeHigh = 5000.0f; 

	//Time scale
	public static float timeScale = (1.0f/40.0f); //1.0f;

	public static bool touchScreen = true;
	public static bool axisUI = false;
	public static Color selectedColor = new Color (.25f, .25f, .25f);
	//public static float bondDistance = 5.0f;
	public static bool drawBondLines = true;
	public static bool pauseTime = false;
	public static int transparent = 3000;
	public static int overlay = 4000;
	public static float atomTransparency = .5f;

	public static void DrawLine(Vector3 startingPos, Vector3 endingPos, Color atomColor1, Color atomColor2, float lineWidth, Material mat){
		
		Vector3 startingPos2 = (startingPos - endingPos);
		startingPos2.Normalize ();
		startingPos2 = Quaternion.Euler (new Vector3 (0.0f, 0.0f, -90.0f)) * startingPos2;
		startingPos2 *= -lineWidth;
		startingPos2 += startingPos;
		
		Vector3 endingPos2 = (endingPos - startingPos);
		endingPos2.Normalize ();
		endingPos2 = Quaternion.Euler (new Vector3 (0.0f, 0.0f, -90.0f)) * endingPos2;
		endingPos2 *= lineWidth;
		endingPos2 += endingPos;
		
		Vector3 startingPos3 = (startingPos - endingPos);
		startingPos3.Normalize ();
		startingPos3 = Quaternion.Euler (new Vector3 (0.0f, -90.0f, 0.0f)) * startingPos3;
		startingPos3 *= -lineWidth;
		startingPos3 += startingPos;
		
		Vector3 endingPos3 = (endingPos - startingPos);
		endingPos3.Normalize ();
		endingPos3 = Quaternion.Euler (new Vector3 (0.0f, -90.0f, 0.0f)) * endingPos3;
		endingPos3 *= lineWidth;
		endingPos3 += endingPos;
		
		Vector3 startingPos4 = (startingPos - endingPos);
		startingPos4.Normalize ();
		startingPos4 = Quaternion.Euler (new Vector3 (-90.0f, 00.0f, 0.0f)) * startingPos4;
		startingPos4 *= -lineWidth;
		startingPos4 += startingPos;
		
		Vector3 endingPos4 = (endingPos - startingPos);
		endingPos4.Normalize ();
		endingPos4 = Quaternion.Euler (new Vector3 (-90.0f, 00.0f, 0.0f)) * endingPos4;
		endingPos4 *= lineWidth;
		endingPos4 += endingPos;
		
		if (!mat) {
			return;
		}
		GL.LoadProjectionMatrix (Camera.main.projectionMatrix);
		GL.PushMatrix();
		mat.SetPass (0);
		GL.Begin (GL.QUADS);
		
		GL.Color (atomColor1);
		GL.Vertex (startingPos);
		GL.Color (atomColor2);
		GL.Vertex (endingPos);
		GL.Vertex (endingPos2);
		GL.Color (atomColor1);
		GL.Vertex (startingPos2);
		
		GL.Vertex (startingPos);
		GL.Color (atomColor2);
		GL.Vertex (endingPos);
		GL.Vertex (endingPos3);
		GL.Color (atomColor1);
		GL.Vertex (startingPos3);
		
		GL.Vertex (startingPos);
		GL.Color (atomColor2);
		GL.Vertex (endingPos);
		GL.Vertex (endingPos4);
		GL.Color (atomColor1);
		GL.Vertex (startingPos4);
		
		GL.End ();
		GL.PopMatrix();
	}

}
