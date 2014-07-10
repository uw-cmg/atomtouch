using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TemperatureCalc : MonoBehaviour {
	
	public static float squareRootAlpha = 1.0f;
	public static float desiredTemperature = 0.001f; //K
	private float averageVelocity = 0.0f; //m/sec = Unity Angstroms/second
	private double totalEnergy;
	private double instantTemp;
	private int moleculeCount;
	private double alpha;
	private double prevTemp1 = 0.0;
	private double prevTemp2 = 0.0;
	
	void FixedUpdate () {

		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		totalEnergy = 0.0f;
		moleculeCount = allMolecules.Length;
		for (int i = 0; i < allMolecules.Length; i++) {
			//compute the total energy in the system
			GameObject molecule = allMolecules[i];
			if(molecule.rigidbody && !molecule.rigidbody.isKinematic){

				double mass = molecule.rigidbody.mass;
				double massKg = mass * StaticVariables.mass100amuToKg; // mass in kg
				double velocityAngstromsPerSecondApparent = molecule.rigidbody.velocity.magnitude;
				double velocityAngstromsPerSecondUnadjusted = velocityAngstromsPerSecondApparent / StaticVariables.eyeAdjustment;
				double velocityMetersPerSecond = velocityAngstromsPerSecondUnadjusted * StaticVariables.angstromsToMeters;
				double velocityMetersPerSecondSquared = Math.Pow(velocityMetersPerSecond, 2);
				totalEnergy += 0.5f * massKg * velocityMetersPerSecondSquared;
			}
		}

		//using sum_KE = (3/2)N*kB*T
		//T = sum_KE / 1.5 / N / kB
		instantTemp = totalEnergy / 1.5f / (float)moleculeCount / StaticVariables.kB;
		if (prevTemp1 == 0.0) {
			prevTemp1 = instantTemp;
			alpha = 1;
		}
		else if(prevTemp2 == 0.0){
			prevTemp2 = instantTemp;
			alpha = 1;
		}
		else{
			double avgInstantTemp = (instantTemp + prevTemp1 + prevTemp2) / 3.0;
			alpha = desiredTemperature / avgInstantTemp;
			prevTemp1 = prevTemp2;
			prevTemp2 = instantTemp;
		}
		squareRootAlpha = (float)Math.Pow (alpha, .5f);
	}
}
