using UnityEngine;
using System.Collections;

public class ReflectingBoundaryCondition : Boundary {

	//reflect the atoms from the walls
	public override void Apply()
	{
		Vector3 boxDimension = Vector3.zero;
		boxDimension.x = CreateEnvironment.myEnvironment.width - 2.0f * CreateEnvironment.myEnvironment.errorBuffer;
		boxDimension.y = CreateEnvironment.myEnvironment.height - 2.0f * CreateEnvironment.myEnvironment.errorBuffer;
		boxDimension.z = CreateEnvironment.myEnvironment.depth - 2.0f * CreateEnvironment.myEnvironment.errorBuffer;
		
		for (int i = 0; i < Atom.AllAtoms.Count; i++)
		{
			Atom currAtom = Atom.AllAtoms[i];
			
			float sign = Mathf.Sign(currAtom.position.x);
			float remainder = ((Mathf.Abs(currAtom.position.x) + boxDimension.x/2.0f) % (2.0f * boxDimension.x));
			if (remainder < boxDimension.x)
			{
				currAtom.position.x = remainder - boxDimension.x / 2.0f;
				currAtom.velocity.x = +1.0f * currAtom.velocity.x;
			}
			else
			{
				currAtom.position.x = 3.0f * boxDimension.x / 2.0f - remainder;
				currAtom.velocity.x = -1.0f * currAtom.velocity.x;
			}
			currAtom.position.x = sign * currAtom.position.x;
			
			sign = Mathf.Sign(currAtom.position.y);
			remainder = ((Mathf.Abs(currAtom.position.y) + boxDimension.y/2.0f) % (2.0f * boxDimension.y));
			if (remainder < boxDimension.y)
			{
				currAtom.position.y = remainder - boxDimension.y / 2.0f;
				currAtom.velocity.y = +1.0f * currAtom.velocity.y;
			}
			else
			{
				currAtom.position.y = 3.0f * boxDimension.y / 2.0f - remainder;
				currAtom.velocity.y = -1.0f * currAtom.velocity.y;
			}
			currAtom.position.y = sign * currAtom.position.y;


			sign = Mathf.Sign(currAtom.position.z);
			remainder = ((Mathf.Abs(currAtom.position.z) + boxDimension.z/2.0f) % (2.0f * boxDimension.z));
			if (remainder < boxDimension.z)
			{
				currAtom.position.z = remainder - boxDimension.z / 2.0f;
				currAtom.velocity.z = +1.0f * currAtom.velocity.z;
			}
			else
			{
				currAtom.position.z = 3.0f * boxDimension.z / 2.0f - remainder;
				currAtom.velocity.z = -1.0f * currAtom.velocity.z;
			}
			currAtom.position.z = sign * currAtom.position.z;
		}
	}

	public override Vector3 deltaPosition(Atom firstAtom, Atom secondAtom)
	{	
		Vector3 deltaR = firstAtom.position - secondAtom.position;
		return deltaR;
	}
}
