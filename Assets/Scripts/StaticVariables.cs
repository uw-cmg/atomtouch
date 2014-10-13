/**
 * Class: StaticVariables.cs
 * Created By: Justin Moeller
 * Description: This class is simply a list of static variables and static functions
 * that can be called from any class. The static variables in this list are either
 * constants or variables that can be controlled from across the entire system of
 * atom (i.e currentPotential). There are two functions in this class, DrawLine and
 * DrawQuad. DrawLine draw a line in 3D space and DrawQuad draws a quad in 3D space.
 * To use these functions in 2D space, the coordinates given to the function must be
 * translated such that they are rotated based on the rotation of the camera. 
 * 
 * 
 **/ 


using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class StaticVariables {

	public static float MDTimestep = 3.0f * (float) Math.Pow (10, -15);
	//Suppose every FixedUpdate physics interval (e.g. 0.02 seconds) is the
	//Molecular Dynamics timestep of 0.5 * 10^-15 seconds
	public static float fixedUpdateIntervalToRealTime = MDTimestep / Time.fixedDeltaTime;

	//Suppose every Update physics interval (e.g. 0.02 seconds) is the
	//Molecular Dynamics timestep of 0.5 * 10^-15 seconds
	public static float updateIntervalToRealTime = MDTimestep;

	//do not scale temperature all at once
	public static float alphaDrag = 1.0f * Time.fixedDeltaTime;

	//Boltzmann constant in J/K
	public static float kB = 1.381f * (float) Math.Pow(10,-23);

	//Permittivity of free space
	public static float epsilon0 = 8.85f * Mathf.Pow (10, -12);

	//Convert units of 100 amu to kg
	public static float mass100amuToKg = 100f * 1.6605f * (float) Math.Pow(10,-27); 

	//Convert units of Angstroms to meters
	public static float angstromsToMeters = (float) Math.Pow (10,-10);
		
	//Cutoff for "seeing" other atoms, in Angstroms
	//multiplied by sigma for Lennard-Jones potential
	public static float cutoff = 10.0f; //mutliplier for cutoff
	public static float cutoffSqr = cutoff * cutoff;

	//Forces are precomputed for a number of discrete separation points and then used as a look up table.
	//The following is the step size in precalculated forces. It is in the same units as cutoff variable
	public static float deltaR = 0.05f;
	public static float sigmaValueMax = 0.0f;
	public static float sigmaValueMin = 0.0f;
	//This array holds the pre-calculated value of LennardJones potential for some sample points.
	public static float[] preLennardJones;

	//When r_ij is small, the Lennard-Jones potential is extremely large.
	//At a certain r_min, we will substitute the L-J potential with a function that
	//curves to a constant as r_ij goes to zero.

	//Multiplier for transition between actual L-J potential and curve to constant
	//    This number will be multiplied by sigma to find the transition distance
	public static float r_min_multiplier = 0.75f;
		
	//Temperature slider bounds in K
	public static float tempRangeLow = 0.01f;
	public static float tempRangeHigh = 5000.0f; 

	//this variable causes the bond lines to either draw or not draw
	public static bool drawBondLines = true;
	//the variable pauses the simulation of physics
	public static bool pauseTime = false;
	//access to sigma values by appending the two atomNames together e.g. "CopperCopper" or "CopperGold" etc
	//public static Dictionary<String, float> sigmaValues = new Dictionary<String, float> ();
	public static float[] sigmaValues = new float[10];

	//access to coefficients in Buckingham potential by appending the two atomNames together e.g. "CopperCopper" or "CopperGold" etc
	public static Dictionary<String, float> coeff_A = new Dictionary<String, float> ();
	public static Dictionary<String, float> coeff_B = new Dictionary<String, float> ();
	public static Dictionary<String, float> coeff_C = new Dictionary<String, float> ();
	public static Dictionary<String, float> coeff_D = new Dictionary<String, float> ();

	//this varaible keeps track of the current potential that is being used. (Note: only Lennard-Jones is currently implemented)
	public static Potential currentPotential = Potential.LennardJones;
	//this variable keeps track of the amount of simulation time that has passed
	public static float currentTime = 0.0f;

	//There are three potentials, but currently Lennard-Jones is the only one that is implemented so changing
	//between these potentials doesnt do anything
	public enum Potential{
		LennardJones,
		Brenner,
		Buckingham
	};

	//this is an enum of the different states that time can pass
	public enum TimeSpeed{
		Normal,
		SlowMotion,
		Stopped
	};
	
	//this function will draw a line from startinPos to endingPos. The atom colors will color each side of the line.
	//Note: this function can only be called within OnPostRender(). It will not display if called from a different function
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

	//this function will draw a quad in 3D space given four coordinates and a color
	//Note: this function must be called from within OnPostRender(). It will not display if called from another function
	public static void DrawQuad(Vector3 upperLeft, Vector3 upperRight, Vector3 lowerLeft, Vector3 lowerRight, Color color, Material mat){

		if (!mat) {
			return;
		}
		GL.LoadProjectionMatrix (Camera.main.projectionMatrix);
		GL.PushMatrix ();
		mat.SetPass (0);
		GL.Begin (GL.QUADS);
		GL.Color (color);
		GL.Vertex (upperLeft);
		GL.Vertex (upperRight);
		GL.Vertex (lowerRight);
		GL.Vertex (lowerLeft);
		GL.End ();
		GL.PopMatrix ();
	}

}
