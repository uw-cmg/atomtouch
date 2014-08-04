using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PotentialEnergy : MonoBehaviour {

	private double totalPotentialEnergyJ;
	//public double totalKineticEnergyJ;
	private float startTime = 0.0f;
	private float timeToUpdate = 1.0f;
	private int updateCalls;
	public static float finalPotentialEnergy = 0.0f;
	private bool first = true;

	
	void Start () {
		totalPotentialEnergyJ = 0.0f;
		startTime = Time.realtimeSinceStartup;
		updateCalls = 0;
	}

	void Update () {
	

		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");

		for (int i = 0; i < allMolecules.Length; i++) {
			GameObject currAtom = allMolecules[i];
			Atom currAtomScript = currAtom.GetComponent<Atom>();
			double potentialEnergyPerAtom = 0.0f;
			for(int j = 0; j < allMolecules.Length; j++){
				GameObject atomNeighbor = allMolecules[j];
				if(currAtom == atomNeighbor) continue;

				float finalSigma = currAtomScript.sigma(atomNeighbor);
				Atom atomNeighborScript = atomNeighbor.GetComponent<Atom>();
				float distance = Vector3.Distance(currAtom.transform.position, atomNeighbor.transform.position);
				if(distance < (StaticVariables.cutoff * currAtomScript.sigma(atomNeighbor))){
					double potentialEnergy = 4 * currAtomScript.epsilon * (Mathf.Pow((finalSigma/distance), 12) - Mathf.Pow((finalSigma), 6));
					potentialEnergyPerAtom += potentialEnergy;
				}
			}
			totalPotentialEnergyJ += potentialEnergyPerAtom;
		}
		updateCalls++;

		if (Time.realtimeSinceStartup - startTime > timeToUpdate || first) {
			first = false;
			finalPotentialEnergy = (float) (totalPotentialEnergyJ / updateCalls); //take the average of the potential energy
			totalPotentialEnergyJ = 0.0f;
			updateCalls = 0;
			startTime = Time.realtimeSinceStartup;
		}


	}
}