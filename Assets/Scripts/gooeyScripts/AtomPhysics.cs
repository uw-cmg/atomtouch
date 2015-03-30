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
		for(int i=0; i < NaCls.Count;i++){
			NaCls[i].GetComponent<AtomGooey>().vel = Vector3.zero;
		}
		AtomGooey curr;
		AtomGooey other;
		for(int i=0; i < NaCls.Count;i++){
			curr = NaCls[i].GetComponent<AtomGooey>();
			Rigidbody currRb = NaCls[i].GetComponent<Rigidbody>();

			for(int j=i+1; j < NaCls.Count;j++){
				other = NaCls[j].GetComponent<AtomGooey>();
				Rigidbody otherRb = NaCls[j].GetComponent<Rigidbody>();

				float distance = Vector3.Distance(curr.gameObject.transform.position, 
				other.gameObject.transform.position);
				//repel
				Vector3 forceDireciton = curr.gameObject.transform.position - other.gameObject.transform.position;
				//attract
				if(curr.charge * other.charge < 0){
					forceDireciton *= -1;
				}
				float c = 9 * Mathf.Pow(10, 9) * 1.602f *1.602f * Mathf.Pow(10,-11);
				Vector3 force = (currRb.mass * currRb.velocity - otherRb.mass * otherRb.velocity)/Time.deltaTime;
				forceDireciton.Normalize();

				curr.vel += (forceDireciton * c / distance/distance) ;
				//Debug.Log(otherRb.velocity);
				other.vel += -1 * curr.vel; 
			}
			currRb.velocity = curr.vel;
		}
	}
}
