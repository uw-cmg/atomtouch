using UnityEngine;
using System.Collections;

public class PairDistributionFunction : MonoBehaviour {

	//The mesh size for pair distribution function.
	private static float dR = 0.1f; //[Angstrom]
	private static float maxR = 60.0f; //[Angstrom]
	private static float[] pairDistribution = new float[(int)(maxR / dR)];
	private static float[] pairDistributionAverage = new float[(int)(maxR / dR)];
	private static float numberOfCalculations = 0.0f;
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
		if (StaticVariables.iTime % StaticVariables.nVerlet == 0)
		{
			normCoefficient = CreateEnvironment.myEnvironment.volume / ((float)Atom.AllAtoms.Count * (float)Atom.AllAtoms.Count * 4.0f * Mathf.PI * dR * dR * dR);
			updatePairDistribution();
			for (int iR = 1; iR < pairDistribution.Length; iR++)
			{
				pairDistributionAverage[iR] = (pairDistributionAverage[iR] * numberOfCalculations + pairDistribution[iR] * normCoefficient / (float)iR / (float)iR);
				numberOfCalculations++;
				pairDistributionAverage[iR] = pairDistributionAverage[iR] / numberOfCalculations;
			}    
		}
	}
	
	public static void updatePairDistribution()
	{
		pairDistribution.Initialize();
		
		for (int i = 0; i < Atom.AllAtoms.Count - 1; i++)
		{
			Atom firstAtom = Atom.AllAtoms[i];
			for (int j = i + 1; j < Atom.AllAtoms.Count; j++)
			{
				Atom secondAtom = Atom.AllAtoms[j];
				Vector3 deltaR = Boundary.myBoundary.deltaPosition(firstAtom, secondAtom);
				float distance = deltaR.magnitude;
				int iR = (int)Mathf.Floor(distance / dR);
				pairDistribution[iR] += 2.0f;
			}
		}
	}
}
