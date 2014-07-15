using UnityEngine;
using System.Collections;
using System;

public class StaticVariables {

	//Boltzmann constant in J/K
	public static float kB = 1.381f * (float) Math.Pow(10,-23);

	//Convert units of 100 amu to kg
	public static float mass100amuToKg = 100f * 1.6605f * (float) Math.Pow(10,-27); 

	//Eye adjustment for converting 1000 meters/second into a length scale that we can 
	//see on the Unity screen, which is 1 Angstrom per second.
	//So, if the real vibration is supposed to be 1000 m/s, we see it as
	//1 Angstrom per second.
	public static float eyeAdjustment = (float) Math.Pow (10, -13); 

	//Convert units of Angstroms to meters
	public static float angstromsToMeters = (float) Math.Pow (10,-10);

	//Delay before any temperature effects are applied
	public static float tempDelay = 5.0f;

	//Cutoff for "seeing" other atoms, in Angstroms
	//multiplied by sigma for Lennard-Jones potential
	public static float cutoff = 20; //mutliplier for cutoff

	//Temperature slider bounds in K
	public static float tempRangeLow = 0.0000001f; 
	public static float tempRangeHigh = 3000.0f; 

	//Time scale
	public static float timeScale = 1.0f; //1.0f;

	public static bool touchScreen = true;

}
