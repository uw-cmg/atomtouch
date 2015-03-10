using UnityEngine;
using System.Collections;

//public abstract class Potential : MonoBehaviour {
public abstract class Potential {
	//this varaible keeps track of the current potential that is being used. (Note: only Lennard-Jones is currently implemented)
	public static potentialType currentPotential = potentialType.LennardJones;

	//Types of potential in the simulation
	public enum potentialType
	{
		LennardJones,
		Brenner,
		Buckingham
	};
		
	public static Potential myPotential;

	abstract public void preCompute();
	abstract public void getForce(Atom firstAtom, Atom secondAtom);
	abstract public float getPotential(Atom firstAtom, Atom secondAtom);
	abstract public void calculateVerletRadius(Atom currAtom);
	abstract public void calculateNeighborList();
}
