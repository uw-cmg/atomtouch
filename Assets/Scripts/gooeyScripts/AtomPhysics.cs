using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AtomPhysics : MonoBehaviour {
	public static AtomPhysics self;
	public List<GameObject> Ions;

	void Awake(){
		//find objects with of the same type
		self = this;
		Application.targetFrameRate = 150;
		GameObject[] loadedNas = GameObject.FindGameObjectsWithTag("Na");
		GameObject[] loadedCls = GameObject.FindGameObjectsWithTag("Cl");
		GameObject[] loaddedCus = GameObject.FindGameObjectsWithTag("Cu");
		foreach(GameObject g in loadedNas){
			Ions.Add(g);
		}
		foreach(GameObject g in loadedCls){
			Ions.Add(g);
		}
		foreach(GameObject g in loaddedCus){
			Ions.Add(g);
		}
	}
	// Use this for initialization
	void Start () {
		Time.timeScale = 1;
	}
	void Update(){
		//kick atom if not moving
		
	}
	// Update is called once per frame
	void FixedUpdate () {
		for(int i=0; i < Ions.Count;i++){
			Ions[i].GetComponent<AtomGooey>().totalForce = Vector3.zero;
		}
		AtomGooey curr;
		AtomGooey other;
		for(int i=0; i < Ions.Count;i++){
			Rigidbody rb = Ions[i].GetComponent<Rigidbody>();
			if(rb.velocity.magnitude < 1f){
				rb.gameObject.GetComponent<AtomGooey>().Kick();
			}
		}
		for(int i=0; i < Ions.Count;i++){
			curr = Ions[i].GetComponent<AtomGooey>();
			Rigidbody currRb = Ions[i].GetComponent<Rigidbody>();

			for(int j=i+1; j < Ions.Count;j++){
				other = Ions[j].GetComponent<AtomGooey>();
				Rigidbody otherRb = Ions[j].GetComponent<Rigidbody>();

				float distance = Vector3.Distance(curr.gameObject.transform.position, 
				other.gameObject.transform.position);
				//repel
				//current to other
				Vector3 forceDireciton = curr.gameObject.transform.position - other.gameObject.transform.position;
				//attract
				if(curr.charge * other.charge < 0){
					forceDireciton *= -1;
				}
				float otherToCurr = 9 * Mathf.Pow(10, 9) * 1.602f *1.602f 
					* Mathf.Abs(other.charge) * Mathf.Abs(curr.charge) * Mathf.Pow(10,-8);
				float currToOther = otherToCurr;
				//Vector3 force = (currRb.mass * currRb.velocity - otherRb.mass * otherRb.velocity)/Time.deltaTime;
				forceDireciton.Normalize();

				curr.totalForce += forceDireciton * otherToCurr / distance/distance;
				//Debug.Log(otherRb.velocity);
				other.totalForce += -forceDireciton * currToOther / distance/distance; 
			}
			//currRb.velocity = curr.vel;
			currRb.velocity = Vector3.zero;
			currRb.AddForce(curr.totalForce);
		}
	}
}
