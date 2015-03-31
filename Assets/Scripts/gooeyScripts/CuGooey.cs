using UnityEngine;
using System.Collections;

public class CuGooey : AtomGooey {

	void Awake(){
		type = (int)AtomGooey.Type.Cu;
		charge = 2;
	}
}