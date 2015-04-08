using UnityEngine;
using System.Collections;

public class BoxScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnCollisionEnter(Collision other){
		GameObject go = other.gameObject;
		if(go.tag != "Na"
			&& go.tag != "Cl"
			&& go.tag != "Cu" ){
			return;
		}
		AtomGooey atom = go.GetComponent<AtomGooey>();
		if(atom.isTarget){
			Debug.Log("win!");
		}
		GameControl.gameState = (int)GameControl.GameState.Win;
	}
}
