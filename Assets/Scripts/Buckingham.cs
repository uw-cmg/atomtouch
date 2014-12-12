using UnityEngine;
using System.Collections;

public class Buckingham : Potential {

	//Cutoff distance for calculating Buckingham force. Beyond this distance the force is taken to be zero.
	private float cutoff = 10.0f; //[Angstrom]
	private float cutoffSqr;
	
	//Cutoff distance for using the spline interpolation function. Beyond this distance the force smoothed to zero.
	private float rSpline; //[Angstrom]
	
	//The mesh size for pre-calculating Lennard Jones force.
	private float dR = 0.0001f;
	
	//pre-calculated coefficients and forces for Buckingham potential
	private float[, ,] preBuckinghamAcceleration;
	private float[, ,] PreBuckinghamPotential;
	
	private float[,] coeff_A = new float[3, 3];
	private float[,] coeff_B = new float[3, 3];
	private float[,] coeff_C = new float[3, 3];
	private float[,] coeff_D = new float[3, 3];
	
	public Buckingham()
	{
		cutoffSqr = cutoff * cutoff;
		rSpline = cutoff - 2.0f;
	}
	
	public override void preCompute()
	{
		// precalculate the LennardJones potential and store it in preLennarJones array.
		int nR = (int)(cutoff / dR) + 1;
		preBuckinghamAcceleration = new float[3,3,nR];
		PreBuckinghamPotential = new float[3,3,nR];
		
		//precompute sigma and acceleration coefficient for the Buckingham potential
		for (int i = 0; i < CreateEnvironment.myEnvironment.molecules.Count; i++)
		{
			Atom firstAtom = CreateEnvironment.myEnvironment.molecules[i].GetComponent<Atom>();
			for (int j = 0; j < CreateEnvironment.myEnvironment.molecules.Count; j++)
			{
				Atom secondAtom = CreateEnvironment.myEnvironment.molecules[j].GetComponent<Atom>();
				
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
					if (distance < 0.7f)
						distance = 0.7f;
					preBuckinghamAcceleration[firstAtom.atomID,secondAtom.atomID,iR] = calcAcceleration(distance,firstAtom,secondAtom);
					PreBuckinghamPotential[firstAtom.atomID, secondAtom.atomID, iR] = calcPotential(distance, firstAtom, secondAtom);
				}
			}
		}
		//WriteData.WritePotential(PreBuckinghamPotential);
		//WriteData.WriteForce(preBuckinghamAcceleration);
	}
	
	//the function returns the LennarJones force on the atom given the list of the atoms that are within range of it
	private float calcAcceleration(float distance,Atom firstAtom, Atom secondAtom)
	{
		float invDistance2 = 1.0f / distance / distance;
		float invDistance6 = invDistance2 * invDistance2 * invDistance2;
		float invDistance7 = invDistance6 / distance;
		float invDistance8 = invDistance7 / distance;
		float invDistance9 = invDistance8 / distance;
		float invrSpline2 = 1.0f / rSpline / rSpline;
		float invrSpline6 = invrSpline2 * invrSpline2 * invrSpline2;
		float invrSpline7 = invrSpline6 / rSpline;
		float invrSpline8 = invrSpline7 / rSpline;
		float invrSpline9 = invrSpline8 / rSpline;
		
		float A = coeff_A [firstAtom.atomID,secondAtom.atomID];
		float B = coeff_B [firstAtom.atomID,secondAtom.atomID];
		float C = coeff_C [firstAtom.atomID,secondAtom.atomID];
		float D = coeff_D [firstAtom.atomID,secondAtom.atomID];
		
		float y1 = A * Mathf.Exp(-B * rSpline) - C * invrSpline6 - D * invrSpline8 + firstAtom.Q_eff * secondAtom.Q_eff / (4.0f * StaticVariables.epsilon0 * Mathf.PI * StaticVariables.angstromsToMeters) / rSpline;
		float y2 = 0.0f;
		float k1 = -A * B * Mathf.Exp(-B * rSpline) + 6.0f * C * invrSpline7 + 8.0f * D * invrSpline9 - firstAtom.Q_eff * secondAtom.Q_eff / (4.0f * StaticVariables.epsilon0 * Mathf.PI * StaticVariables.angstromsToMeters) * invrSpline2; //units of this derivative is [J/Angstrom]
		float k2 = 0.0f;
		
		float uPrime_r = 0.0f;
		if (distance <= rSpline)
		{
			uPrime_r = -A * B * Mathf.Exp(-B * distance) / StaticVariables.angstromsToMeters + 6.0f * C * invDistance7 / StaticVariables.angstromsToMeters + 8.0f * D * invDistance9 / StaticVariables.angstromsToMeters - firstAtom.Q_eff * secondAtom.Q_eff / (4.0f * StaticVariables.epsilon0 * Mathf.PI * StaticVariables.angstromsToMeters * StaticVariables.angstromsToMeters) * invDistance2;
		}
		else if (distance <= cutoff)
		{
			float t = (distance - rSpline) / (cutoff - rSpline);
			float a = +k1 * (cutoff - rSpline) - (y2 - y1);
			float b = -k2 * (cutoff - rSpline) + (y2 - y1);
			uPrime_r = (- y1 + y2 + (1-2.0f * t) * (a*(1.0f - t)+b*t) + (t-t*t)*(b-a)) / (cutoff - rSpline) / StaticVariables.angstromsToMeters;
		}
		else
		{
			uPrime_r = 0.0f;
		}
		
		//float forceMagnitude = -1.0f * uPrime_r / distance + uPrime_rc / cutoff;
		float forceMagnitude = -1.0f * uPrime_r / distance;
		float acceleration = forceMagnitude / (firstAtom.massamu * StaticVariables.amuToKg * StaticVariables.angstromsToMeters); //Units of [1 / second^2] when multiplied by deltaR gets units of [Angstrom / second^2]
		return acceleration;
	}
	
	//the function returns the LennarJones force on the atom given the list of the atoms that are within range of it
	private float calcPotential(float distance, Atom firstAtom, Atom secondAtom)
	{
		float invDistance2 = 1.0f / distance / distance;
		float invDistance6 = invDistance2 * invDistance2 * invDistance2;
		float invDistance7 = invDistance6 / distance;
		float invDistance8 = invDistance7 / distance;
		float invrSpline2 = 1.0f / rSpline / rSpline;
		float invrSpline6 = invrSpline2 * invrSpline2 * invrSpline2;
		float invrSpline7 = invrSpline6 / rSpline;
		float invrSpline8 = invrSpline7 / rSpline;
		float invrSpline9 = invrSpline8 / rSpline;
		
		float A = coeff_A[firstAtom.atomID, secondAtom.atomID];
		float B = coeff_B[firstAtom.atomID, secondAtom.atomID];
		float C = coeff_C[firstAtom.atomID, secondAtom.atomID];
		float D = coeff_D[firstAtom.atomID, secondAtom.atomID];
		
		float y1 = A * Mathf.Exp(-B * rSpline) - C * invrSpline6 - D * invrSpline8 + firstAtom.Q_eff * secondAtom.Q_eff / (4.0f * StaticVariables.epsilon0 * Mathf.PI * StaticVariables.angstromsToMeters) / rSpline;
		float y2 = 0.0f;
		float k1 = -A * B * Mathf.Exp(-B * rSpline) + 6.0f * C * invrSpline7 + 8.0f * D * invrSpline9 - firstAtom.Q_eff * secondAtom.Q_eff / (4.0f * StaticVariables.epsilon0 * Mathf.PI * StaticVariables.angstromsToMeters) * invrSpline2; //units of this derivative is [J/Angstrom]
		float k2 = 0.0f;
		
		float u_r = 0.0f;
		
		if (distance <= rSpline)
		{
			u_r = A * Mathf.Exp(-B * distance) - C * invDistance6 - D * invDistance8 + firstAtom.Q_eff * secondAtom.Q_eff / (4.0f * StaticVariables.epsilon0 * Mathf.PI * StaticVariables.angstromsToMeters) / distance;
		}
		else if (distance <= cutoff)
		{
			float t = (distance - rSpline) / (cutoff - rSpline);
			float a = +k1 * (cutoff - rSpline) - (y2 - y1);
			float b = -k2 * (cutoff - rSpline) + (y2 - y1);
			u_r = (1.0f - t) * y1 + t * y2 + t * (1.0f - t) * (a * (1.0f - t) + b * t);
		}
		else
		{
			u_r = 0.0f;
		}
		
		float potential = u_r; //Units of Joules
		return potential;
	}
	
	//the function returns the Lennard-Jones force on the atom given the list of all the atoms in the simulation
	public override void getForce(Atom firstAtom, Atom secondAtom)
	{
		Vector3 deltaR = Boundary.myBoundary.deltaPosition(firstAtom,secondAtom);
		float distanceSqr = deltaR.sqrMagnitude;
		
		//only get the forces of the atoms that are within the cutoff range
		if (distanceSqr <= cutoffSqr)
		{
			int iR = (int)(Mathf.Sqrt(distanceSqr) / (dR));
			firstAtom.accelerationNew = firstAtom.accelerationNew+ preBuckinghamAcceleration[firstAtom.atomID, secondAtom.atomID,iR] * deltaR;
			secondAtom.accelerationNew = secondAtom.accelerationNew - preBuckinghamAcceleration[secondAtom.atomID, firstAtom.atomID,iR] * deltaR;
		}
	}
	
	//the function returns the Lennard-Jones force on the atom given the list of all the atoms in the simulation
	public override float getPotential(Atom firstAtom, Atom secondAtom)
	{
		float potential = 0.0f;
		Vector3 deltaR = Boundary.myBoundary.deltaPosition(firstAtom,secondAtom);
		float distanceSqr = deltaR.sqrMagnitude;
		
		//only get the forces of the atoms that are within the cutoff range
		if (distanceSqr <= cutoffSqr)
		{
			int iR = (int)(Mathf.Sqrt(distanceSqr) / (dR));
			potential = (PreBuckinghamPotential[firstAtom.atomID, secondAtom.atomID,iR] + PreBuckinghamPotential[firstAtom.atomID, secondAtom.atomID,iR]) / 2.0f ;
		}
		return potential;
	}
	
	public override void calculateVerletRadius()
	{
		for (int i = 0; i < Atom.AllAtoms.Count - 1; i++)
		{
			Atom currAtom = Atom.AllAtoms[i];
			currAtom.verletRadius = cutoff + 1.0f;
		}
	}
	
	//This function creates a list of all neighbor list for each atom
	public override void calculateNeighborList()
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
				Vector3 deltaR = Boundary.myBoundary.deltaPosition(firstAtom, secondAtom);
				float distanceSqr = deltaR.sqrMagnitude;
				if (distanceSqr < firstAtom.verletRadius * firstAtom.verletRadius)
					firstAtom.neighborList.Add(secondAtom);
			}
		}
	}
}
