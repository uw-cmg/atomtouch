using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PairDistributionFunction : MonoBehaviour {

	//The mesh size for pair distribution function.
	private static float dR = 0.1f; //[Angstrom]
	private static float maxR = 15.0f; //[Angstrom]
	private static int avgLength = 30; // this variable keeps the number of steps that the distribution function is averaged over
	private static int calculationNumber = 0; // this variable keeps the index number for calculating the distribution number. Its maximum is avgLength
	private static float[] pairDistributionAverage = new float[(int)(maxR / dR)];
	private static float normCoefficient = 0.0f;
	private static float[][] pairDistributionLog = new float[avgLength][];
	
	public static float[] PairDistributionAverage
	{
		get
		{
			return pairDistributionAverage;
		}
	}

	public static float MaxR
	{
		get
		{
			return maxR;
		}
	}
	
	public static void calculateAveragePairDistribution()
	{
		if (calculationNumber == avgLength)
			calculationNumber = 0;

		normCoefficient = CreateEnvironment.myEnvironment.volume / ((float)Atom.AllAtoms.Count * (float)Atom.AllAtoms.Count * 4.0f * Mathf.PI * dR * dR * dR);
		pairDistributionLog[calculationNumber] = updatePairDistribution();
		calculationNumber++;
		for (int iR = 1; iR < (int)(maxR / dR); iR++)
		{
			pairDistributionAverage[iR]=0;
			for(int iC = 0; iC < avgLength; iC++)
			{
				pairDistributionAverage[iR] += pairDistributionLog[iC][iR] * normCoefficient / (float)iR / (float)iR / (float)avgLength;
			}
		}    
	}
	
	private static float[] updatePairDistribution()
	{	
		float[] pairDistribution = new float[(int)(maxR / dR)];
		for (int i = 0; i < Atom.AllAtoms.Count - 1; i++)
		{
			Atom firstAtom = Atom.AllAtoms[i];
			for (int j = i + 1; j < Atom.AllAtoms.Count; j++)
			{
				Atom secondAtom = Atom.AllAtoms[j];
				Vector3 deltaR = Boundary.myBoundary.deltaPosition(firstAtom, secondAtom);
				float distance = deltaR.magnitude;
				int iR = (int)Mathf.Floor(distance / dR);
				if (iR < pairDistribution.Length)
					pairDistribution[iR] += 2.0f;
			}
		}
		return pairDistribution;
	}
}