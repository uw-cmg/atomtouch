using UnityEngine;
using System.Collections;

public class VisualizeInteraction : MonoBehaviour {
	
	public Material mat;
	public float lineDistance = 5.0f;

	void OnPostRender(){
		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");


		for (int i = 0; i < allMolecules.Length; i++) {
			GameObject currAtom = allMolecules[i];
			for(int j = i + 1; j < allMolecules.Length; j++){
				GameObject atomNeighbor = allMolecules[j];
				if(Vector3.Distance(currAtom.transform.position, atomNeighbor.transform.position) < lineDistance){
					//draw a line from currAtom to atomNeighbor
					Atom currAtomScript = currAtom.GetComponent<Atom>();
					Atom neighAtomScript = atomNeighbor.GetComponent<Atom>();
					StaticVariables.DrawLine (currAtom.transform.position, atomNeighbor.transform.position, currAtomScript.GetColor(), neighAtomScript.GetColor(), .2f, mat);
				}
			}
		}
	}


}
