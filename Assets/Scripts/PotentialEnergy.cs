/**
 * Class: PotentialEnergy.cs
 * Created by: Justin Moeller
 * Description: The class computes the potential energy of the system. It computes the potential energy
 * as an average over .05 seconds. The static variable finalPotentialEnergy is the final potential energy
 * and its updated every .05 seconds. This is the value that is being graphed in Graph.cs, and this value
 * can be accessed from any script. 
 * 
 **/ 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PotentialEnergy : MonoBehaviour {

	private double totalPotentialEnergyJ;
	private float startTime = 0.0f;
	private float timeToUpdate = .05f;
	private int updateCalls;
	public static float finalPotentialEnergy = 0.0f;
	private bool first = true;

	
	void Start () {
		totalPotentialEnergyJ = 0.0f;
		startTime = Time.realtimeSinceStartup;
		updateCalls = 0;
	}

	void Update () {
	
		//this function computes the potential energy of the system every frame
		//we only are interested in the average though, so we take the average potential energy over .05s 

		for (int i = 0; i < Atom.AllMolecules.Count; i++) {
			Atom currAtom = Atom.AllMolecules[i];
			double potentialEnergyPerAtom = 0.0f;
			for(int j = 0; j < Atom.AllMolecules.Count; j++){
				Atom neighborAtom = Atom.AllMolecules[j];
				if(currAtom.gameObject == neighborAtom.gameObject) continue;

				float finalSigma = StaticVariables.sigmaValues[currAtom.atomID*neighborAtom.atomID];
				float distanceSqr = (currAtom.transform.position-neighborAtom.transform.position).sqrMagnitude;
				if(distanceSqr < (StaticVariables.cutoffSqr)){
					double potentialEnergy = 4 * currAtom.epsilon * (Mathf.Pow((finalSigma*finalSigma/distanceSqr), 6) - Mathf.Pow((finalSigma*finalSigma/distanceSqr), 3));
					potentialEnergyPerAtom += potentialEnergy;
				}
			}
			totalPotentialEnergyJ += potentialEnergyPerAtom;
		}
		updateCalls++;

		//update the potential energy every .05s
		if (Time.realtimeSinceStartup - startTime > timeToUpdate || first) {
			first = false;
			finalPotentialEnergy = (float) (totalPotentialEnergyJ / updateCalls); //take the average of the potential energy
			totalPotentialEnergyJ = 0.0f;
			updateCalls = 0;
			startTime = Time.realtimeSinceStartup;
		}


	}
}