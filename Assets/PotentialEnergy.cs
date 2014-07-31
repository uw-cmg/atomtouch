/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PotentialEnergy : MonoBehaviour {

	public double totalPotentialEnergyJ;
	public double totalKineticEnergyJ;
	
	void Start () {
	
	}

	void Update () {
	
		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
//		List<GameObject> molecules = new List<GameObject> ();
//
//		for (int i=0; i< allMolecules.Length; i++){
//			 double distance = Vector3.Distance(transform.position, allMolecules[i].transform.position);
//			 if(allMolecules[i] != gameObject && distance < (StaticVariables.cutoff * sigma)){
//				molecules.Add(allMolecules[i]);
//			 }
//			 double singlepotentialEnergy = GetLennardJonesPotentialEnergy(molecules);
//			 totalPotentialEnergyJ += singlepotentialEnergy;
//		}

		for (int i = 0; i < allMolecules.Length; i++) {
			GameObject currAtom = allMolecules[i];
			for(int j = 0; j < allMolecules.Length; j++){
				GameObject atomNeighbor = allMolecules[j];
				double distance = Vector3.Distance(currAtom.transform.position, atomNeighbor.transform.position);
				if(currAtom != atomNeighbor && distance < (StaticVariables.cutoff * sigma)){

				}
			}
		}


	}

	//TTM add potential energy function
	double GetLennardJonesPotentialEnergy(List<GameObject> objectsInRange){
		double finalPotentialEnergy = 0.0;
		for (int i = 0; i < objectsInRange.Count; i++) {
			Atom otherAtomScript = objectsInRange[i].GetComponent<Atom>();
			float otherSigma = otherAtomScript.sigma;
			float finalSigma = sigma;
			if(otherSigma != sigma) finalSigma = (float)Math.Pow(sigma + otherSigma, .5f);
			//Vector3 vect = molecules[i].transform.position - transform.position;
			Vector3 direction = new Vector3(objectsInRange[i].transform.position.x - transform.position.x, objectsInRange[i].transform.position.y - transform.position.y, objectsInRange[i].transform.position.z - transform.position.z);
			direction.Normalize();
			double distance = Vector3.Distance(transform.position, objectsInRange[i].transform.position);
			double potentialEnergy = 4*epsilon*(Math.Pow ((finalSigma/distance),12)-Math.Pow ((finalSigma/distance),6)); //distance and sigma are both in angstroms, so units cancel
			finalPotentialEnergy+= potentialEnergy;
		}
		//epsilon was in J, so no unit conversion is necessary.
		return finalPotentialEnergy;
	}
}
*/