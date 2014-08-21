/**
 * Class: VisualizeInteraction.cs
 * Created by: Justin Moeller
 * Description: This class draws the lines between the atoms. Because it only needs to draw lines between 
 * every pair of atoms it only iterates through each distinct pair rather than every possible pair. (This 
 * reduces the time spent from an O(N^2) to (1/2)O(N^2)). The lines are only drawn if the variable in StaticVariables
 * drawBondLines is true. This variable is controlled from the user interface.
 * 
 * 
 * 
 **/


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
						StaticVariables.DrawLine (currAtom.transform.position, atomNeighbor.transform.position, Color.white, Color.white, .05f, mat);
					}
				}
			}
		}

	}


}
