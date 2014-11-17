using UnityEngine;
using System.Collections;

public class Buckingham : MonoBehaviour {

	//Cutoff distance for calculating LennarJones force. This quantity is unit less and normalized to sigmaValue for atom pair
	public static float cutoff = 10.0f; //[Angstroms]
	public static float cutoffSqr = cutoff * cutoff;
	
	//The mesh size for pre-calculating Lennard Jones force.
	private static float dR = 0.0001f;
	
	//pre-calculated coefficients and forces for Buckingham potential
	private static float[, ,] preBuckinghamAcceleration;
	private static float[, ,] PreBuckinghamPotential;
	
	private static float[,] coeff_A = new float[3, 3];
	private static float[,] coeff_B = new float[3, 3];
	private static float[,] coeff_C = new float[3, 3];
	private static float[,] coeff_D = new float[3, 3];
	
	public static void preBuckingham()
	{
		// precalculate the LennardJones potential and store it in preLennarJones array.
		int nR = (int)(cutoff / dR) + 1;
		preBuckinghamAcceleration = new float[3,3,nR];
		PreBuckinghamPotential = new float[3,3,nR];
		
		//precompute sigma and acceleration coefficient for the Buckingham potential
		for (int i = 0; i < StaticVariables.myEnvironment.molecules.Count; i++)
		{
			Atom firstAtom = StaticVariables.myEnvironment.molecules[i].GetComponent<Atom>();
			for (int j = 0; j < StaticVariables.myEnvironment.molecules.Count; j++)
			{
				Atom secondAtom = StaticVariables.myEnvironment.molecules[j].GetComponent<Atom>();
				
				float currentA = Mathf.Sqrt(firstAtom.buck_A * secondAtom.buck_A);
				coeff_A[firstAtom.atomID, secondAtom.atomID] = currentA;
				
				float currentB = Mathf.Sqrt(firstAtom.buck_B * secondAtom.buck_B);
				coeff_B[firstAtom.atomID, secondAtom.atomID] = currentB;
				
				float currentC = Mathf.Sqrt(firstAtom.buck_C * secondAtom.buck_C);
				coeff_C[firstAtom.atomID, secondAtom.atomID] = currentC;
				
				float currentD = Mathf.Sqrt(firstAtom.buck_D * secondAtom.buck_D);
				coeff_D[firstAtom.atomID, secondAtom.atomID] = currentD;
				
				for (int iR = 0; iR < nR; iR++)
				{
					float distance = (float)iR * dR;
					if (distance < dR)
						distance = dR;
					preBuckinghamAcceleration[firstAtom.atomID,secondAtom.atomID,iR] = calcAcceleration(distance,firstAtom,secondAtom);
					PreBuckinghamPotential[firstAtom.atomID, secondAtom.atomID, iR] = calcPotential(distance, firstAtom, secondAtom);
				}
			}
		}
	}
	
	//the function returns the LennarJones force on the atom given the list of the atoms that are within range of it
	private static float calcAcceleration(float distance,Atom firstAtom, Atom secondAtom)
	{
		float invDistance2 = 1.0f / distance / distance;
		float invDistance6 = invDistance2 * invDistance2 * invDistance2;
		float invDistance7 = invDistance6 / distance;
		float invDistance8 = invDistance2 * invDistance2 * invDistance2 * invDistance2;
		float invDistance9 = invDistance8 / distance;
		float invCutoff2 = 1.0f / cutoff / cutoff;
		float invCutoff6 = invCutoff2 * invCutoff2 * invCutoff2;
		float invCutoff7 = invCutoff6 / cutoff;
		float invCutoff8 = invCutoff2 * invCutoff2 * invCutoff2 * invCutoff2;
		float invCutoff9 = invCutoff8 / cutoff;
		
		float A = coeff_A [firstAtom.atomID,secondAtom.atomID];
		float B = coeff_B [firstAtom.atomID,secondAtom.atomID];
		float C = coeff_C [firstAtom.atomID,secondAtom.atomID];
		float D = coeff_D [firstAtom.atomID,secondAtom.atomID];
		
		float uPrime_r = -A * B * Mathf.Exp(-B * distance) / StaticVariables.angstromsToMeters + 6.0f * C * invDistance7 / StaticVariables.angstromsToMeters + 8.0f * D * invDistance9 / StaticVariables.angstromsToMeters - firstAtom.Q_eff * secondAtom.Q_eff / (4.0f * StaticVariables.epsilon0 * Mathf.PI * StaticVariables.angstromsToMeters * StaticVariables.angstromsToMeters) * invDistance2;
		float uPrime_rc = -A * B * Mathf.Exp(-B * cutoff) / StaticVariables.angstromsToMeters + 6.0f * C * invCutoff7 / StaticVariables.angstromsToMeters + 8.0f * D * invCutoff9 / StaticVariables.angstromsToMeters - firstAtom.Q_eff * secondAtom.Q_eff / (4.0f * StaticVariables.epsilon0 * Mathf.PI * StaticVariables.angstromsToMeters * StaticVariables.angstromsToMeters) * invCutoff2;
		
		//float forceMagnitude = -1.0f * uPrime_r / distance + uPrime_rc / cutoff;
		float forceMagnitude = -1.0f * uPrime_r / distance;
		float acceleration = forceMagnitude / (firstAtom.massamu * StaticVariables.amuToKg * StaticVariables.angstromsToMeters); //Units of [1 / second^2] when multiplied by deltaR gets units of [Angstrom / second^2]
		return acceleration;
	}
	
	//the function returns the LennarJones force on the atom given the list of the atoms that are within range of it
	private static float calcPotential(float distance, Atom firstAtom, Atom secondAtom)
	{
		float invDistance2 = 1.0f / distance / distance;
		float invDistance6 = invDistance2 * invDistance2 * invDistance2;
		float invDistance7 = invDistance6 / distance;
		float invDistance8 = invDistance2 * invDistance2 * invDistance2 * invDistance2;
		float invDistance9 = invDistance8 / distance;
		float invCutoff2 = 1.0f / cutoff / cutoff;
		float invCutoff6 = invCutoff2 * invCutoff2 * invCutoff2;
		float invCutoff7 = invCutoff6 / cutoff;
		float invCutoff8 = invCutoff2 * invCutoff2 * invCutoff2 * invCutoff2;
		float invCutoff9 = invCutoff8 / cutoff;
		
		float A = coeff_A[firstAtom.atomID, secondAtom.atomID];
		float B = coeff_B[firstAtom.atomID, secondAtom.atomID];
		float C = coeff_C[firstAtom.atomID, secondAtom.atomID];
		float D = coeff_D[firstAtom.atomID, secondAtom.atomID];
		
		float uPrime_r = -A * B * Mathf.Exp(-B * distance) / StaticVariables.angstromsToMeters + 6.0f * C * invDistance7 / StaticVariables.angstromsToMeters + 8.0f * D * invDistance9 / StaticVariables.angstromsToMeters - firstAtom.Q_eff * secondAtom.Q_eff / (4.0f * StaticVariables.epsilon0 * Mathf.PI * StaticVariables.angstromsToMeters * StaticVariables.angstromsToMeters) * invDistance2;
		float uPrime_rc = -A * B * Mathf.Exp(-B * cutoff) / StaticVariables.angstromsToMeters + 6.0f * C * invCutoff7 / StaticVariables.angstromsToMeters + 8.0f * D * invCutoff9 / StaticVariables.angstromsToMeters - firstAtom.Q_eff * secondAtom.Q_eff / (4.0f * StaticVariables.epsilon0 * Mathf.PI * StaticVariables.angstromsToMeters * StaticVariables.angstromsToMeters) * invCutoff2;
		
		float u_r = A * Mathf.Exp(-B * distance) - C * invDistance6 - D * invDistance8 + firstAtom.Q_eff * secondAtom.Q_eff / (4.0f * StaticVariables.epsilon0 * Mathf.PI * StaticVariables.angstromsToMeters) / distance;
		float u_rc = A * Mathf.Exp(-B * cutoff) - C * invCutoff6 - D * invCutoff8 + firstAtom.Q_eff * secondAtom.Q_eff / (4.0f * StaticVariables.epsilon0 * Mathf.PI * StaticVariables.angstromsToMeters) / cutoff;
		
		//float potential = u_r - (uPrime_rc * cutoff * StaticVariables.angstromsToMeters / 2.0f) * (distance * distance / cutoff / cutoff) - u_rc + (uPrime_rc * cutoff * StaticVariables.angstromsToMeters / 2.0f) ; //Units of Joules
		float potential = u_r; //Units of Joules
		return potential;
	}
	
	//the function returns the Lennard-Jones force on the atom given the list of all the atoms in the simulation
	public static void getForce(Atom firstAtom, Atom secondAtom)
	{
		Vector3 deltaR = firstAtom.position - secondAtom.position;
		float distanceSqr = deltaR.sqrMagnitude;
		
		//only get the forces of the atoms that are within the cutoff range
		if (distanceSqr <= cutoffSqr)
		{
			int iR = (int)(Mathf.Sqrt(distanceSqr) / (dR));
			firstAtom.accelerationNew = firstAtom.accelerationNew + preBuckinghamAcceleration[firstAtom.atomID, secondAtom.atomID,iR] * deltaR;
			secondAtom.accelerationNew = secondAtom.accelerationNew - preBuckinghamAcceleration[secondAtom.atomID, firstAtom.atomID,iR] * deltaR;

		}
	}
	
	//the function returns the Lennard-Jones force on the atom given the list of all the atoms in the simulation
	public static float getPotential(Atom firstAtom, Atom secondAtom)
	{
		float potential = 0.0f;
		Vector3 deltaR = firstAtom.position - secondAtom.position;
		float distanceSqr = deltaR.sqrMagnitude;
		
		//only get the forces of the atoms that are within the cutoff range
		if (distanceSqr <= cutoffSqr)
		{
			int iR = (int)(Mathf.Sqrt(distanceSqr) / (dR));
			potential = (PreBuckinghamPotential[firstAtom.atomID, secondAtom.atomID,iR] + PreBuckinghamPotential[firstAtom.atomID, secondAtom.atomID,iR]) / 2.0f ;
		}
		return potential;
	}
	
	public static void calculateVerletRadius()
	{
		for (int i = 0; i < Atom.AllAtoms.Count - 1; i++)
		{
			Atom currAtom = Atom.AllAtoms[i];
			currAtom.verletRadius = cutoff + 2.0f;
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
				if (distanceSqr < (firstAtom.verletRadius * firstAtom.verletRadius))
				{
					firstAtom.neighborList.Add(secondAtom);
				}
			}
		}
	}
}
