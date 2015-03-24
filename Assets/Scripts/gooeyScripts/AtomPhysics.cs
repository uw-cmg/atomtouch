using UnityEngine;
using System.Collections;

public class AtomPhysics : MonoBehaviour {
	public GameObject anotherAtom;
	[HideInInspector]public Rigidbody rb;
	void Awake(){
		rb = GetComponent<Rigidbody>();
	}
	// Use this for initialization
	void Start () {
		//set init vel
		rb.velocity = 5* new Vector3(UnityEngine.Random.Range(-0.5f,0.5f),
			UnityEngine.Random.Range(-0.5f,0.5f),
			UnityEngine.Random.Range(-0.5f,0.5f));
		Debug.Log("init vel: " + gameObject.name + ": " + rb.velocity);
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 forceDireciton = transform.position-anotherAtom.transform.position;
		//thrust force = (m1v1-m0v0)/(t1-t0)
		Rigidbody rbOther = anotherAtom.GetComponent<Rigidbody>();
		Vector3 force = (rb.mass * rb.velocity - rbOther.mass * rbOther.velocity)/Time.deltaTime;
		anotherAtom.GetComponent<Rigidbody>().AddForce(force);
	}
}
