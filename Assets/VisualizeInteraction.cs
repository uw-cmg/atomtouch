using UnityEngine;
using System.Collections;

public class VisualizeInteraction : MonoBehaviour {
	
	public Material mat;

	void OnPostRender(){

		if (StaticVariables.drawBondLines) {
			GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
			
			for (int i = 0; i < allMolecules.Length; i++) {
				GameObject currAtom = allMolecules[i];
				for(int j = i + 1; j < allMolecules.Length; j++){
					GameObject atomNeighbor = allMolecules[j];
					Atom atomScript = currAtom.GetComponent<Atom>();
					if(Vector3.Distance(currAtom.transform.position, atomNeighbor.transform.position) < atomScript.BondDistance(atomNeighbor)){
						//draw a line from currAtom to atomNeighbor
						Atom currAtomScript = currAtom.GetComponent<Atom>();
						Atom neighAtomScript = atomNeighbor.GetComponent<Atom>();
						StaticVariables.DrawLine (currAtom.transform.position, atomNeighbor.transform.position, currAtomScript.color, neighAtomScript.color, .05f, mat);
					}
				}
			}
		}

	}


}
