using UnityEngine;
using System.Collections;

public abstract class Boundary {
	public static Boundary myBoundary = new ReflectingBoundaryCondition();

	public abstract void Apply ();
	public abstract Vector3 deltaPosition(Atom firstAtom, Atom secondAtom);

}
