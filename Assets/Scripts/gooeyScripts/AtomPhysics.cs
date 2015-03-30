using UnityEngine;
using System.Collections;

public class AtomPhysics : MonoBehaviour {
	public GameObject[] sameTypeAtoms;
	[HideInInspector]public Rigidbody rb;
	void Awake(){
		rb = GetComponent<Rigidbody>();
		//find objects with of the same type
		sameTypeAtoms = GameObject.FindGameObjectsWithTag(gameObject.tag);
		
	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
		foreach (GameObject other in sameTypeAtoms){
			if(other == gameObject)return;
			float distance = Vector3.Distance(other.transform.position, 
				gameObject.transform.position);
			if( distance > 5f){
				continue;
			}
			Rigidbody rbOther = other.GetComponent<Rigidbody>();

			Vector3 forceDireciton = transform.position-other.transform.position;
			Vector3 force = (rb.mass * rb.velocity - rbOther.mass * rbOther.velocity)/Time.deltaTime;
			//Debug.Log(force);
			forceDireciton.Normalize();
			//other.GetComponent<Rigidbody>().AddForce(force);
			rbOther.velocity += (forceDireciton /distance )* Time.deltaTime; 
		}
		
	}
}
