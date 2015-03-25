using UnityEngine;
using System.Collections;

public class AtomPhysics : MonoBehaviour {
	public GameObject anotherAtom;
	public GameObject[] allAtoms;
	[HideInInspector]public Rigidbody rb;
	void Awake(){
		rb = GetComponent<Rigidbody>();
		allAtoms = GameObject.FindGameObjectsWithTag("Molecule");
	}
	// Use this for initialization
	void Start () {
		//set init vel
		/*
		rb.velocity = 5f* new Vector3(UnityEngine.Random.Range(-0.5f,0.5f),
			UnityEngine.Random.Range(-0.5f,0.5f),
			0);
		Debug.Log("init vel: " + gameObject.name + ": " + rb.velocity);
		*/
	}
	
	// Update is called once per frame
	void Update () {
		
		foreach (GameObject other in allAtoms){
			if(Vector3.Distance(other.transform.position, 
				gameObject.transform.position) > 0.3f){
				continue;
			}
			Rigidbody rbOther = other.GetComponent<Rigidbody>();

			Vector3 forceDireciton = transform.position-other.transform.position;
			Vector3 force = (rb.mass * rb.velocity - rbOther.mass * rbOther.velocity)/Time.deltaTime;
			//other.GetComponent<Rigidbody>().AddForce(force);
			rbOther.velocity += force/rbOther.mass * Time.deltaTime; 
		}
		
	}
}
