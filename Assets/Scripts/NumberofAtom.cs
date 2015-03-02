using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NumberofAtom : MonoBehaviour {


	public static int selectedAtoms;
	
	public GameObject selectedText;
	public AtomTouchGUI atomTouchGUI;
	private Text text;
	public GameObject[] allMolecules;
	// Use this for initialization
	void Awake () {

		selectedAtoms = 0;
		text = selectedText.GetComponent<Text>();
		atomTouchGUI = Camera.main.GetComponent<AtomTouchGUI>();
	}

	// Update is called once per frame
	void Update () {
		selectedAtoms = CountSelectedAtoms ();
		Atom.EnableSelectAtomGroup(selectedAtoms > 0);
		//if(selectedAtoms>0)Debug.Log(selectedAtoms);
		text.text = selectedAtoms + " Atom(s) selected";

		
		if(selectedAtoms ==  allMolecules.Length){
			atomTouchGUI.deselectButton.SetActive(false);
			atomTouchGUI.selectAllText.text = "Deselect All";
		}else{
			atomTouchGUI.deselectButton.SetActive(true);
			atomTouchGUI.selectAllText.text = "Select All";
		}
		
	}



	public int CountSelectedAtoms(){
		int selectedAtoms = 0;
		allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		for (int i = 0; i < Atom.AllAtoms.Count; i++) {
			Atom currAtom = Atom.AllAtoms[i];
			if(currAtom.selected){
				selectedAtoms++;
			}
		}
		return selectedAtoms;
	}
}
