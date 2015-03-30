using UnityEngine;
using System.Collections;

public class Box : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionStay(Collision other){
		Debug.Log("collided!");
		//bounce the atom off
		if(other.gameObject.tag != "Molecule"){
			Debug.Log("non-atom collided with box");
			return;
		}
		ContactPoint[] cps = other.contacts;
		Debug.Log("contacts leng: " + cps.Length);
		other.gameObject.GetComponent<Rigidbody>().velocity *= -1;
		other.gameObject.transform.position 
			= Vector3.Reflect(-other.gameObject.GetComponent<Rigidbody>().velocity, Vector3.up);

	}
	
}
