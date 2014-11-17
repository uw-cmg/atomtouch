using UnityEngine;
using System.Collections;

public class LennardJones : MonoBehaviour {
	
	//Cutoff distance for calculating LennarJones force. This quantity is unit less and normalized to sigmaValue for atom pair
	public static float cutoff = 2.5f; //[unit less]
	public static float cutoffSqr = cutoff * cutoff;
	
	//The mesh size for pre-calculating Lennard Jones force.
	private static float dR = 0.000001f;
	
	//When r_ij is small, the Lennard-Jones potential is extremely large.
	//At a certain r_min, we will substitute the L-J potential with a function that
	//curves to a constant as r_ij goes to zero.
	//Multiplier for transition between actual L-J potential and curve to constant
	//This number will be multiplied by sigma to find the transition distance
	private static float rMinMultiplier = 0.75f;
	
	//pre-calculated coefficients and forces for Lennard-Jones potential
	private static float[,] sigmaValues = new float[3, 3];
	private static float[,] accelCoefficient = new float[3, 3]; // this is the coefficient that is multiplied by the preLennardJones vector to get the acceleration of each atom for each combinations
	private static float[] preLennardJonesForce; //This is the pre-calculated value of LennardJones force for some mesh points.
	private static float[] preLennardJonesPotential; //This is the pre-calculated value of LennardJones potential for some mesh points.
	
	
	public static void preLennardJones()
	{
		//precompute sigma and acceleration coefficient for the LJ potential
		for (int i = 0; i < StaticVariables.myEnvironment.molecules.Count; i++)
		{
			Atom firstAtom = StaticVariables.myEnvironment.molecules[i].GetComponent<Atom>();
			for (int j = 0; j < StaticVariables.myEnvironment.molecules.Count; j++)
			{
				Atom secondAtom = StaticVariables.myEnvironment.molecules[j].GetComponent<Atom>();
				
				float currentSigma = Mathf.Sqrt(firstAtom.sigma * secondAtom.sigma);
				sigmaValues[firstAtom.atomID, secondAtom.atomID] = currentSigma;
				
				// when the pre-calculated normalized Lennard Jones force is multiplied by this coefficient the acceleration units is [Angstrom/second^2]
				float currentAccelCoeff = 24.0f * firstAtom.epsilon / (currentSigma * currentSigma * StaticVariables.angstromsToMeters * StaticVariables.angstromsToMeters * firstAtom.massamu * StaticVariables.amuToKg);
				accelCoefficient[firstAtom.atomID, secondAtom.atomID] = currentAccelCoeff;
			}
		}
		
		// precalculate the LennardJones potential and store it in preLennarJones array.
		int nR = (int)(cutoff / dR) + 1;
		preLennardJonesForce = new float[nR];
		preLennardJonesPotential = new float[nR];
		
		for (int i = 0; i < nR; i++)
		{
			float distance = (float)i * dR;
			preLennardJonesForce[i] = calcForce(distance);
			preLennardJonesPotential[i] = calcPotential(distance);
		}
	}
	
	//the function returns the LennarJones force on the atom given the list of the atoms that are within range of it
	private static float calcForce(float distance)
	{
		float invDistance2 = 1.0f / distance / distance;
		float invDistance6 = invDistance2 * invDistance2 * invDistance2;
		float invCutoff2 = 1.0f / cutoff / cutoff;
		float invCutoff6 = invCutoff2 * invCutoff2 * invCutoff2;
		float r_min = rMinMultiplier;
		
		float forceMagnitude = 0.0f;
		
		if (distance > r_min)
		{
			forceMagnitude = invDistance2 * ((2.0f * invDistance6 * invDistance6 - invDistance6) - (invCutoff2 / invDistance2) * (2.0f * invCutoff6 * invCutoff6 - invCutoff6));
		}
		// Smooth the potential to go to a constant not infinity at r=0
		else
		{
			float invr_min = 1 / r_min;
			float invr_min2 = invr_min * invr_min;
			float invr_min6 = invr_min2 * invr_min2 * invr_min2;
			float magnitude_Vmin = invr_min2 * ((2.0f * invr_min6 * invr_min6 - invr_min6) - (invCutoff2 / invr_min2) * (2.0f * invCutoff6 * invCutoff6 - invCutoff6));
			
			float r_Vmax = r_min / 1.5f;
			float invr_Vmax2 = 1 / r_Vmax / r_Vmax;
			float invr_Vmax6 = invr_Vmax2 * invr_Vmax2 * invr_Vmax2;
			float magnitude_Vmax = invr_Vmax2 * ((2.0f * invr_Vmax6 * invr_Vmax6 - invr_Vmax6) - (invCutoff2 / invr_Vmax2) * (2.0f * invCutoff6 * invCutoff6 - invCutoff6));
			
			float part1 = (distance / r_min) * (Mathf.Exp(distance - r_min));
			float part2 = magnitude_Vmax - magnitude_Vmin;
			forceMagnitude = magnitude_Vmax - (part1 * part2);
		}
		
		return forceMagnitude;
	}
	
	//the function returns the LennarJones force on the atom given the list of the atoms that are within range of it
	private static float calcPotential(float distance)
	{
		float invDistance2 = 1.0f / distance / distance;
		float invDistance6 = invDistance2 * invDistance2 * invDistance2;
		float invCutoff2 = 1.0f / cutoff / cutoff;
		float invCutoff6 = invCutoff2 * invCutoff2 * invCutoff2;
		float r_min = rMinMultiplier;
		
		float potential = 0.0f;
		
		if (distance > 0.0f)
		{
			potential = 4.0f * ((invDistance6 * invDistance6 - invDistance6) + (6.0f * invCutoff6 * invCutoff6 - 3.0f * invCutoff6) * (invCutoff2 / invDistance2) - 7.0f * invCutoff6 * invCutoff6 + 4.0f * invCutoff6);
		}
		
		return potential;
	}
	
	//the function returns the Lennard-Jones force on the atom given the list of all the atoms in the simulation
	public static void getForce(Atom firstAtom, Atom secondAtom)
	{
		float[] firstAtomAcceleration = new float[3];
		float[] secondAtomAcceleration = new float[3];
		
		Vector3 deltaR = firstAtom.position - secondAtom.position;
		float distanceSqr = deltaR.sqrMagnitude;
		float finalSigma = sigmaValues[firstAtom.atomID, secondAtom.atomID];
		float normDistanceSqr = distanceSqr / finalSigma / finalSigma; // this is normalized distanceSqr to the sigmaValue
		
		//only get the forces of the atoms that are within the cutoff range
		if (normDistanceSqr <= cutoffSqr)
		{
			int iR = (int)(Mathf.Sqrt(normDistanceSqr) / (dR));
			for (int idx = 0; idx < 3; idx++)
			{
				firstAtom.accelerationNew[idx] = firstAtom.accelerationNew[idx] + preLennardJonesForce[iR] * accelCoefficient[firstAtom.atomID, secondAtom.atomID] * deltaR[idx];
				secondAtom.accelerationNew[idx] = secondAtom.accelerationNew[idx] - preLennardJonesForce[iR] * accelCoefficient[secondAtom.atomID, firstAtom.atomID] * deltaR[idx];
			}
		}
	}
	
	//the function returns the Lennard-Jones force on the atom given the list of all the atoms in the simulation
	public static float getPotential(Atom firstAtom, Atom secondAtom)
	{
		float potential = 0.0f;
		Vector3 deltaR = firstAtom.position - secondAtom.position;
		float distanceSqr = deltaR.sqrMagnitude;
		float finalSigma = sigmaValues[firstAtom.atomID, secondAtom.atomID];
		float normDistanceSqr = distanceSqr / finalSigma / finalSigma; // this is normalized distanceSqr to the sigmaValue
		
		//only get the forces of the atoms that are within the cutoff range
		if (normDistanceSqr <= cutoffSqr)
		{
			int iR = (int)(Mathf.Sqrt(normDistanceSqr) / (dR));
			potential = firstAtom.epsilon * preLennardJonesPotential[iR];
		}
		return potential;
	}
	
	public static void calculateVerletRadius()
	{
		// find the the minimum mass of an atom
		/*
            float minMassAmu = 10000.0f;
            for (int i = 0; i < Atom.templateAtoms.Count - 1; i++)
            {
                Atom currAtom = Atom.templateAtoms[i];
                if (minMassAmu > currAtom.massamu)
                {
                    minMassAmu = currAtom.massamu;
                }
            }
            float averageVelocity = Mathf.Sqrt(3.0f * StaticVariables.kB * StaticVariables.desiredTemperature / minMassAmu / StaticVariables.amuToKg) / StaticVariables.angstromsToMeters;

            for (int i = 0; i < Atom.AllAtoms.Count - 1; i++)
            {
                Atom currAtom = Atom.AllAtoms[i];
                currAtom.verletRadius = cutoff + 15.0f * (float)StaticVariables.nVerlet * averageVelocity * StaticVariables.MDTimestep / currAtom.sigma;
            }
            */
		
		for (int i = 0; i < Atom.AllAtoms.Count - 1; i++)
		{
			Atom currAtom = Atom.AllAtoms[i];
			currAtom.verletRadius = cutoff + 1.0f;
		}
	}
	
	//This function creates a list of all neighbor list for each atom
	public static void calculateNeighborList()
	{
		//clear the old neighborList
		for (int i = 0; i < Atom.AllAtoms.Count - 1; i++)
		{
			Atom currAtom = Atom.AllAtoms[i];
			currAtom.neighborList.Clear();
		}
		
		//create the new neighborList
		for (int i = 0; i < Atom.AllAtoms.Count - 1; i++)
		{
			Atom firstAtom = Atom.AllAtoms[i];
			for (int j = i + 1; j < Atom.AllAtoms.Count; j++)
			{
				Atom secondAtom = Atom.AllAtoms[j];
				Vector3 deltaR = firstAtom.position - secondAtom.position;
				float distanceSqr = deltaR.sqrMagnitude;
				float finalSigma = sigmaValues[firstAtom.atomID, secondAtom.atomID];
				float normDistanceSqr = distanceSqr / finalSigma / finalSigma; // this is normalized distanceSqr to the sigmaValue
				if (normDistanceSqr < (firstAtom.verletRadius * firstAtom.verletRadius))
				{
					firstAtom.neighborList.Add(secondAtom);
				}
			}
		}
	}
}
