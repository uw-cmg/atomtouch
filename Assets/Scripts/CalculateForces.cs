/**
 * Class: CalculateForces.cs
 * Created by: Amirhossein Davoody
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

public class CalculateForces : MonoBehaviour {

	public static Dictionary<Atom,Vector3> allForces = new Dictionary<Atom, Vector3> ();
	
	void FixedUpdate(){
		if (!StaticVariables.pauseTime) {
			
			for (int i = 0; i<Atom.AllMolecules.Count; i++) {
				Atom currAtom = Atom.AllMolecules[i];
				allForces [currAtom] = Vector3.zero;
			}
			
			
			for (int i = 0; i < Atom.AllMolecules.Count; i++) {
				Atom firstAtom = Atom.AllMolecules [i];
				for (int j = i+1; j<Atom.AllMolecules.Count; j++) {
					Atom secondAtom = Atom.AllMolecules [j];
					Vector3 force = Vector3.zero;
					
					if (StaticVariables.currentPotential == StaticVariables.Potential.LennardJones) {
						force = GetLennardJonesForce (firstAtom,secondAtom);
						allForces [firstAtom] += force * StaticVariables.forceCoeffLJ[firstAtom.atomID,secondAtom.atomID];
						allForces [secondAtom] -= force * StaticVariables.forceCoeffLJ[secondAtom.atomID,firstAtom.atomID];
					}
					else if (StaticVariables.currentPotential == StaticVariables.Potential.Buckingham) {
						force = GetBuckinghamForce (firstAtom,secondAtom);
						allForces [firstAtom] += force ;
						allForces [secondAtom] -= force;
					}
				}
				
			}
			for (int i = 0; i<Atom.AllMolecules.Count; i++) {
				Atom currAtom = Atom.AllMolecules[i];

				if(!currAtom.rigidbody.isKinematic) currAtom.rigidbody.angularVelocity = Vector3.zero;
				currAtom.rigidbody.AddForce (allForces[currAtom], mode:ForceMode.Force);
				
				//scale the velocity based on the temperature of the system
				if ((currAtom.rigidbody.velocity.magnitude != 0) && !currAtom.rigidbody.isKinematic && !float.IsInfinity(TemperatureCalc.squareRootAlpha) && Atom.AllMolecules.Count > 1) {
					Vector3 newVelocity = currAtom.rigidbody.velocity * TemperatureCalc.squareRootAlpha;
					currAtom.rigidbody.velocity = newVelocity;
				}

			}

		}
		else{
			//zero out all of the velocities of all of the atoms when time is stopped
			for(int i = 0; i < Atom.AllMolecules.Count; i++){
				Atom currAtom = Atom.AllMolecules[i];
				if(!currAtom.rigidbody.isKinematic){
					currAtom.rigidbody.velocity = Vector3.zero;
				}
			}
		}
	}
	
	//the function returns the Lennard-Jones force on the atom given the list of all the atoms in the simulation
	Vector3 GetLennardJonesForce(Atom firstAtom, Atom secondAtom){
		Vector3 finalForce = Vector3.zero;
		Vector3 deltaR = firstAtom.transform.position - secondAtom.transform.position;
		float distanceSqr = deltaR.sqrMagnitude;
		
		//only get the forces of the atoms that are within the cutoff range
		if (distanceSqr < StaticVariables.cutoffSqr) {
			
			float finalSigma = StaticVariables.sigmaValues [firstAtom.atomID,secondAtom.atomID];
			
			int iR = (int)((Mathf.Sqrt (distanceSqr) / finalSigma) / (StaticVariables.deltaR / StaticVariables.sigmaValueMax)) + 2;
			float magnitude = StaticVariables.preLennardJones [iR];

			finalForce = deltaR * magnitude;
		}
		return finalForce;
	}

	//the function returns the Buckingham force on the atom given the list of all the atoms in the simulation
	Vector3 GetBuckinghamForce(Atom firstAtom, Atom secondAtom){
		Vector3 finalForce = Vector3.zero;
		Vector3 deltaR = firstAtom.transform.position - secondAtom.transform.position;
		float distanceSqr = deltaR.sqrMagnitude;
			
		//only get the forces of the atoms that are within the cutoff range
		if ( (distanceSqr < StaticVariables.cutoffSqr)) {
			float distance = Mathf.Sqrt(distanceSqr);
				
			float final_A = StaticVariables.coeff_A [firstAtom.atomID,secondAtom.atomID];
			float final_B = StaticVariables.coeff_B [firstAtom.atomID,secondAtom.atomID];
			float final_C = StaticVariables.coeff_C [firstAtom.atomID,secondAtom.atomID];
			float final_D = StaticVariables.coeff_D [firstAtom.atomID,secondAtom.atomID];
				
			float magnitude = final_A * final_B * Mathf.Exp (-final_B * distance) / distance;
			magnitude = magnitude - 6.0f * final_C / Mathf.Pow (distanceSqr, 4);
			magnitude = magnitude - 8.0f * final_D / Mathf.Pow (distanceSqr, 5);
			magnitude = magnitude / StaticVariables.angstromsToMeters;
			magnitude = magnitude + firstAtom.Q_eff * secondAtom.Q_eff / (4.0f * Mathf.PI * StaticVariables.epsilon0 * distanceSqr * distance * StaticVariables.angstromsToMeters * StaticVariables.angstromsToMeters);
				
				finalForce = deltaR * magnitude;
			}
		
		Vector3 adjustedForce = finalForce / StaticVariables.mass100amuToKg;
		adjustedForce = adjustedForce / StaticVariables.angstromsToMeters;
		adjustedForce = adjustedForce * StaticVariables.fixedUpdateIntervalToRealTime * StaticVariables.fixedUpdateIntervalToRealTime;
		return adjustedForce;
	}
}