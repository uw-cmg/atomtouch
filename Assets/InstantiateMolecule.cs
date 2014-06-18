using UnityEngine;
using System.Collections;

public class InstantiateMolecule : MonoBehaviour {

	public Rigidbody moleculePrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);
		//Vector3 curScreenPoint = new Vector3 (0.0f, 0.0f, 0.0f);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
		Quaternion curRotation = Quaternion.Euler(0, 0, 0);
		
		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(!Physics.Raycast(ray,out hit) || hit.collider.gameObject.tag != "Molecule"){
				Rigidbody moleculeInstance;
				moleculeInstance = Instantiate(moleculePrefab, curPosition, curRotation) as Rigidbody;
			}
		}
	}
}
