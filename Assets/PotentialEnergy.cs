using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PotentialEnergy : MonoBehaviour {

	public double totalPotentialEnergyJ;
	//public double totalKineticEnergyJ;
	private float startTime = 0.0f;
	private float timeToUpdate = 5.0f;
	
	void Start () {
		totalPotentialEnergyJ = 0.0f;
		startTime = Time.realtimeSinceStartup;
	}

	void Update () {
	
		totalPotentialEnergyJ = 0.0f;
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

		if (Time.realtimeSinceStartup - startTime > timeToUpdate) {
			print ("time: " + Time.realtimeSinceStartup + " Potential Energy: " + totalPotentialEnergyJ);
			startTime = Time.realtimeSinceStartup;
		}


	}
}