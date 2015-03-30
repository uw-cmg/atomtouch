using UnityEngine;
using System.Collections;

public class ChlorineGooey : AtomGooey {

	void Awake(){
		type = (int)AtomGooey.Type.Cl;
		charge = -1;
	}
}
