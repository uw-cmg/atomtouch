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
			for (int i = 0; i < Atom.AllMolecules.Count; i++) {
				for(int j = i + 1; j < Atom.AllMolecules.Count; j++){
					Atom currAtom = Atom.AllMolecules[i];
					Atom neighborAtom = Atom.AllMolecules[j];
					if((currAtom.transform.position - neighborAtom.transform.position).magnitude < currAtom.BondDistance(neighborAtom.gameObject)){
						//draw a line from currAtom to atomNeighbor
						StaticVariables.DrawLine (currAtom.transform.position, neighborAtom.transform.position, Color.white, Color.white, .05f, mat);
					}
				}
			}
		}
	}


}
