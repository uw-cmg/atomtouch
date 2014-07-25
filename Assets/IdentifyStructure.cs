using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class IdentifyStructure : MonoBehaviour {

	List<int> angles = new List<int> ();

	void Update () {
	
		angles.Clear ();
		for (int i = 0; i < 12; i++) {
			angles.Add(60);
		}


		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		for (int i = 0; i < allMolecules.Length; i++) {
			GameObject currAtom = allMolecules[i];

			List<Vector3> atomNeighbors = new List<Vector3>();
			for(int j = 0; j < allMolecules.Length; j++){
				GameObject atomNeighbor = allMolecules[j];
				if(atomNeighbor == currAtom) continue;
				if(Vector3.Distance(currAtom.transform.position, atomNeighbor.transform.position) < StaticVariables.bondDistance){
					atomNeighbors.Add(atomNeighbor.transform.position);
				}
			}


			if(atomNeighbors.Count > 1){
				for(int j = 0; j < atomNeighbors.Count; j++){
					for(int k = j+1; k < atomNeighbors.Count; k++){
						Vector3 vector1 = (atomNeighbors[j] - currAtom.transform.position);
						Vector3 vector2 = (atomNeighbors[k] - currAtom.transform.position);
						int angle = (int)Math.Round(Vector3.Angle(vector1, vector2));
						angles.Remove(angle);
					}
				}
			}
		}

		if (angles.Count == 0) {
			//print ("We have identified the stable state for 4");
		}


	}
}
