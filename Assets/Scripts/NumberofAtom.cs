using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NumberofAtom : MonoBehaviour {


	public static int selectedAtoms;

	public GameObject selectedText;

	private Text text;
	// Use this for initialization
	void Awake () {

		selectedAtoms = 0;
		text = selectedText.GetComponent<Text> ();
	}

	// Update is called once per frame
	void Update () {
		selectedAtoms = CountSelectedAtoms ();
		Atom.EnableSelectAtomGroup(selectedAtoms>0);
		//if(selectedAtoms>0)Debug.Log(selectedAtoms);
		text.text = selectedAtoms + " Atom(s) selected";

	}



	public int CountSelectedAtoms(){
		int selectedAtoms = 0;
		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		for (int i = 0; i < Atom.AllAtoms.Count; i++) {
			Atom currAtom = Atom.AllAtoms[i];
			if(currAtom.selected){
				selectedAtoms++;
			}
		}
		return selectedAtoms;
	}
}
