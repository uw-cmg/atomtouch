using UnityEngine;
using System.Collections;

public class ZPlaneTrigger : MonoBehaviour {
	
	private GameObject clickedAtom;
	
	public void SetClickedAtom(GameObject a){
		clickedAtom = a;
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.gameObject != clickedAtom) {
			Atom atomScript = other.gameObject.GetComponent<Atom>();
			atomScript.ChangeAtomIntersection(true);
		}
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject != clickedAtom) {
			Atom atomScript = other.gameObject.GetComponent<Atom>();
			atomScript.ChangeAtomIntersection(false);
		}
	}
	
}

