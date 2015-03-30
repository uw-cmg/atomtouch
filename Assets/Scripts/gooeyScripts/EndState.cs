using UnityEngine;
using System.Collections;

public class EndState : MonoBehaviour{
	void OnCollisionEnter(Collision other){
		if(other.gameObject.tag == "Molecule"){
			//end of game
			Debug.Log("you win");
		}
	}
}