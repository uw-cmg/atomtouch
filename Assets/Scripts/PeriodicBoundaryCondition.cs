using UnityEngine;
using System.Collections;

public class PeriodicBoundaryCondition : Boundary {

	//apply the boundary condition on atoms that are out of bound
	public override void Apply()
	{
		Vector3 boxDimension = new Vector3 (CreateEnvironment.myEnvironment.width-2.0f * CreateEnvironment.myEnvironment.errorBuffer, CreateEnvironment.myEnvironment.height-2.0f * CreateEnvironment.myEnvironment.errorBuffer , CreateEnvironment.myEnvironment.depth-2.0f * CreateEnvironment.myEnvironment.errorBuffer);
		
		for (int i = 0; i < Atom.AllAtoms.Count; i++)
		{
			Atom currAtom = Atom.AllAtoms[i];
			
			float sign = Mathf.Sign(currAtom.position.x);
			currAtom.position.x = sign * (((Mathf.Abs(currAtom.position.x) + boxDimension.x / 2.0f) % (boxDimension.x)) - boxDimension.x / 2.0f);
			
			sign = Mathf.Sign(currAtom.position.y);
			currAtom.position.y = sign * (((Mathf.Abs(currAtom.position.y) + boxDimension.y / 2.0f) % (boxDimension.y)) - boxDimension.y / 2.0f);
			
			sign = Mathf.Sign(currAtom.position.z);
			currAtom.position.z = sign * (((Mathf.Abs(currAtom.position.z) + boxDimension.z / 2.0f) % (boxDimension.z)) - boxDimension.z / 2.0f);
			
		}
	}

	public override Vector3 deltaPosition(Atom firstAtom, Atom secondAtom)
	{
		Vector3 boxDimension = new Vector3 (CreateEnvironment.myEnvironment.width-2.0f * CreateEnvironment.myEnvironment.errorBuffer, CreateEnvironment.myEnvironment.height-2.0f * CreateEnvironment.myEnvironment.errorBuffer , CreateEnvironment.myEnvironment.depth-2.0f * CreateEnvironment.myEnvironment.errorBuffer);
		
		Vector3 deltaR = firstAtom.position - secondAtom.position;
		if (Mathf.Abs(deltaR.x) > boxDimension.x / 2.0f)
		{
			float sign = Mathf.Sign(deltaR.x);
			deltaR.x = deltaR.x - sign * boxDimension.x;
		}
		
		if (Mathf.Abs(deltaR.y) > boxDimension.y / 2.0f)
		{
			float sign = Mathf.Sign(deltaR.y);
			deltaR.y = deltaR.y- sign * boxDimension.y;
		}
		
		if (Mathf.Abs(deltaR.z) > boxDimension.z / 2.0f)
		{
			float sign = Mathf.Sign(deltaR.z);
			deltaR.z = deltaR.z- sign * boxDimension.z;
		}
		
		return deltaR;
	}
}
