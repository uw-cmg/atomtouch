using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AtomPhysics : MonoBehaviour {
	public List<GameObject> NaCls;

	void Awake(){
		//find objects with of the same type
		GameObject[] loadedNas = GameObject.FindGameObjectsWithTag("Na");
		GameObject[] loadedCls = GameObject.FindGameObjectsWithTag("Cl");
		foreach(GameObject g in loadedNas){
			NaCls.Add(g);
		}
		foreach(GameObject g in loadedCls){
			NaCls.Add(g);
		}
	}
	// Use this for initialization
	void Start () {
		Time.timeScale = 1;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//return;
		for(int i=0; i < NaCls.Count;i++){
			AtomGooey curr = NaCls[i].GetComponent<AtomGooey>();
			Rigidbody currRb = NaCls[i].GetComponent<Rigidbody>();

			for(int j=i+1; j < NaCls.Count;j++){
				AtomGooey other = NaCls[j].GetComponent<AtomGooey>();
				Rigidbody otherRb = NaCls[j].GetComponent<Rigidbody>();

				float distance = Vector3.Distance(curr.gameObject.transform.position, 
				other.gameObject.transform.position);
				//repel
				Vector3 forceDireciton = -curr.gameObject.transform.position + other.gameObject.transform.position;
				//attract
				if(curr.charge * other.charge < 0){
					forceDireciton *= -1;
				}
				float c = 9 * Mathf.Pow(10, 9) * 1.602f *1.602f * Mathf.Pow(10,-11);
				Vector3 force = (currRb.mass * currRb.velocity - otherRb.mass * otherRb.velocity)/Time.deltaTime;
				forceDireciton.Normalize();
				otherRb.velocity = (forceDireciton * c / distance/distance) ;
				//Debug.Log(otherRb.velocity);
				currRb.velocity = -1 * otherRb.velocity; 
			}
		}
		/*
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
		*/
	}
}
