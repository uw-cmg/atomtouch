using UnityEngine;
using System.Collections;

public class PairDistributionFunction : MonoBehaviour {

	//The mesh size for pair distribution function.
	private static float dR = 0.1f; //[Angstrom]
	public static float maxR = 15.0f; //[Angstrom]
	private static float[] pairDistribution = new float[(int)(maxR / dR)];
	private static float[] pairDistributionAverage = new float[(int)(maxR / dR)];
	public static float numberOfCalculations = 0.0f;
	private static float normCoefficient = 0.0f;
	
	public static float[] PairDistributionAverage
	{
		get
		{
			return pairDistributionAverage;
		}
	}
	
	public static void calculateAveragePairDistribution()
	{
		if (numberOfCalculations > 20) 
		{
			numberOfCalculations = 1.0f;

			for (int iR = 1; iR < pairDistribution.Length; iR++)
			{
				pairDistribution[iR] = 0.0f;
			}
		}

		normCoefficient = CreateEnvironment.myEnvironment.volume / ((float)Atom.AllAtoms.Count * (float)Atom.AllAtoms.Count * 4.0f * Mathf.PI * dR * dR * dR);
		updatePairDistribution();
		numberOfCalculations++;
		for (int iR = 1; iR < pairDistribution.Length; iR++)
		{
			pairDistributionAverage[iR] = pairDistribution[iR] * normCoefficient / (float)iR / (float)iR / numberOfCalculations;
		}    
	}
	
	private static void updatePairDistribution()
	{	
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
	}
}
