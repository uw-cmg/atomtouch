/**
 * Class: TemperatureCalc.cs
 * Created by: Justin Moeller
 * Description: This class handles the computation of the scalar that the atoms' velocities are multiplied by.
 * The desiredTemperature variable is the temperature that the user desires the system to be at, and if the
 * system is not at that temperature, a scalar is calculated that will either speed up or slow down the atoms
 * so that they are the appropriate velocities that correspond to the desiredTemperature. This scalar value
 * is smoothed over a couple of frames so the atoms' velocities don't change drastically in one frame, but rather
 * gradually over a couple of frames. This is so the atoms look like they gain/lose speed naturally rather than 
 * instantly. The squareRootAlpha variable is the scalar that the atoms' velocities are scaled by.
 * 
 * 
 * 
 **/ 


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TemperatureCalc : MonoBehaviour {
	
	public static float squareRootAlpha = 1.0f;
	public static float desiredTemperature = 300.0f; //K
	public static double totalKineticEnergyJ;
	public double instantTemp;
	private int moleculeCount;
	public double alpha;
	public double draggedAlpha;
	
	void FixedUpdate () {
		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		totalKineticEnergyJ = 0.0f;
		moleculeCount = allMolecules.Length;
		for (int i = 0; i < allMolecules.Length; i++) {
			//compute the total energy in the system
			GameObject molecule = allMolecules[i];
			if(molecule.rigidbody && !molecule.rigidbody.isKinematic){
				
				double mass = molecule.rigidbody.mass;
				double massKg = mass * StaticVariables.mass100amuToKg; // mass in kg
				double velocityAngstromsPerSecondApparent = molecule.rigidbody.velocity.magnitude;
				double velocityMetersPerSecond = velocityAngstromsPerSecondApparent * StaticVariables.angstromsToMeters / StaticVariables.fixedUpdateIntervalToRealTime;
				double velocityMetersPerSecondSquared = Math.Pow(velocityMetersPerSecond, 2);
				totalKineticEnergyJ += 0.5f * massKg * velocityMetersPerSecondSquared;
			}
		}
		
		//using sum_KE = (3/2)N*kB*T
		//T = sum_KE / 1.5 / N / kB
		instantTemp = totalKineticEnergyJ / 1.5f / (float)moleculeCount / StaticVariables.kB;
		
		alpha = desiredTemperature / instantTemp; 
		
		//do not make adjustment to desired temperature all at once
		//desiredTemperature = 1000K, instantTemp = 100K, alpha = 10;
		//with drag of 0.01, desiredTemperature is (1000K-100K) * 0.01 = 9K away
		//from 100K, = 109K, and alpha = 109/100 = 1.09
		//desiredTemperature = 1K, instantTemp = 100K, alpha = 0.01;
		//with drag of 0.01, desiredTemperature is (100K - 1K) * 0.01 = 0.99K away
		//from 100K, = 99.01K, and alpha = 99.01/100 = 0.9901
		//desiredTemperature = 150K, instantTemp = 100K, alpha = 1.5;
		//with drag of 0.01, desiredTemperature is (150K - 100K) * 0.01 = 0.5K away
		//from 100K, = 100.5K, and alpha = 100.5/100 = 1.0050
		double draggedTemp = 0.0;
		if (Math.Abs (instantTemp) < 0.000000000001) {
						/*
			//Temp is zero; assign atoms a random velocity
			draggedTemp = desiredTemperature * StaticVariables.alphaDrag;
			double avgKE = 1.5 * StaticVariables.kB * draggedTemp;
			double avgKEunity = avgKE / StaticVariables.mass100amuToKg;
			avgKEunity = avgKEunity / StaticVariables.angstromsToMeters;
			avgKEunity = avgKEunity / StaticVariables.angstromsToMeters;
			avgKEunity = avgKEunity * StaticVariables.fixedUpdateIntervalToRealTime;
			avgKEunity = avgKEunity * StaticVariables.fixedUpdateIntervalToRealTime;
			//KE = 0.5 * mass * v^2
			for (int i = 0; i < allMolecules.Length; i++) {
				float newVelocity = 2.0f * (float)avgKE / allMolecules[i].rigidbody.mass;
				newVelocity = (float) Math.Pow (newVelocity, 0.5);
				float vDirection = UnityEngine.Random.Range(1,3);
				if (vDirection == 1){
					allMolecules[i].rigidbody.velocity = new Vector3(newVelocity, 0.0f, 0.0f);
				}
				else if (vDirection == 2){
					allMolecules[i].rigidbody.velocity = new Vector3(0.0f,newVelocity,0.0f);
				}
				else{
					allMolecules[i].rigidbody.velocity = new Vector3(0.0f,0.0f,newVelocity);
				}
			}
			*/
			draggedAlpha = 1.0; //allow time for system to develop velocities based on forces;
		} 
		else if (instantTemp > 5000) {
			//adjust to damp very high temperatures quickly
			draggedAlpha = alpha; 
			//draggedTemp = (desiredTemperature - instantTemp) * StaticVariables.alphaDrag * 100 + instantTemp;
			//draggedAlpha = draggedTemp / instantTemp;
		}
		else if (alpha > 1){
			draggedTemp = (desiredTemperature - instantTemp) * StaticVariables.alphaDrag + instantTemp;
			draggedAlpha = draggedTemp/instantTemp;
		}
		else if(alpha < 1){
			draggedTemp = instantTemp - ((instantTemp - desiredTemperature) * StaticVariables.alphaDrag);
			draggedAlpha = draggedTemp/instantTemp;
		}
		else{
			draggedAlpha = 1.0;
		}
		
		squareRootAlpha = (float)Math.Pow (draggedAlpha, .5f);
	}
}
