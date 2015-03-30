using UnityEngine;
using System.Collections;

public class SodiumGooey : AtomGooey {

	void Awake(){
		type = (int)AtomGooey.Type.Na;
		charge = 1;
	}
}
