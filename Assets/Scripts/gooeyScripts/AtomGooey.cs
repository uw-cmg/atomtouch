using UnityEngine;
using System.Collections;


public class AtomGooey : MonoBehaviour {
	public static AtomGooey self;

	public enum Type{
		Na,
		Cu,
		Cl
	}
	public int type;
	public int charge;
	public Vector3 totalForce;
	void Awake(){
		self = this;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//gives a random vel
	public void Kick(){
		float lo = -0.50f;
		float hi = 0.50f;
		float x = UnityEngine.Random.Range(lo, hi);
		float y = UnityEngine.Random.Range(lo, hi);
		float z = UnityEngine.Random.Range(lo, hi);
		GetComponent<Rigidbody>().velocity = new Vector3(x,y,z);
		//Debug.Log(GetComponent<Rigidbody>().velocity);
	}
}
