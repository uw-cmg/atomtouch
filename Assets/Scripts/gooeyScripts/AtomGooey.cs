using UnityEngine;
using System.Collections;


public class AtomGooey : MonoBehaviour {
	public static AtomGooey self;

	public enum Type{
		Na,
		Cl
	}
	public int type;
	public int charge;
	public Vector3 vel;
	void Awake(){
		self = this;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
