/**
 * Class: PhysicsEngine.cs
 * Created by: Amirhossein Davoody
 * Description: The class computes the potential energy of the system. It computes the potential energy
 * as an average over .05 seconds. The static variable finalPotentialEnergy is the final potential energy
 * and its updated every .05 seconds. This is the value that is being graphed in Graph.cs, and this value
 * can be accessed from any script. 
 * 
 **/ 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PhysicsEngine : MonoBehaviour
{
	void Awake(){
		if (StaticVariables.currentPotential == StaticVariables.Potential.LennardJones)
			LennardJones.calculateVerletRadius ();
		if (StaticVariables.currentPotential == StaticVariables.Potential.Buckingham)
			Buckingham.calculateVerletRadius ();
	}

	void FixedUpdate()
	{
		
		if (!StaticVariables.pauseTime)
		{

			VelocityVerlet();
			ReflectFromWalls();
			CalculateEnergy();



			if (StaticVariables.iTime == 0)
			{
				StaticVariables.clockTimeStart = Time.realtimeSinceStartup;
				Debug.Log ("Start Time = " +  StaticVariables.clockTimeStart +" , iTime = " + StaticVariables.iTime);
			}

			if (StaticVariables.iTime == 5000)
			{
				StaticVariables.clockTimeEnd = Time.realtimeSinceStartup;
				Debug.Log ("End Time = " +  StaticVariables.clockTimeEnd +" , iTime = " + StaticVariables.iTime);
				float deltaTime = StaticVariables.clockTimeEnd - StaticVariables.clockTimeStart;
				float timePerStep = deltaTime / (float)StaticVariables.iTime;
				float timePerStepPerAtom = timePerStep / StaticVariables.myEnvironment.numMolecules;
				Debug.Log ("Delta Time = " + deltaTime +" , iTime = " + StaticVariables.iTime + " , time per step = " + timePerStep + " , time per step per atom = " + timePerStepPerAtom);
			}


			StaticVariables.currentTime += StaticVariables.MDTimestepInPicosecond;
			Graph.numMDStepSinceLastRecord ++;
			StaticVariables.iTime ++;

		}
	}

	void VelocityVerlet()
	{
		// update the position of all atoms then initialize the acceleration to be updated
		for (int i=0; i< Atom.AllAtoms.Count; i++)
		{
			Atom currAtom = Atom.AllAtoms[i];
			currAtom.position = currAtom.position + StaticVariables.MDTimestep * currAtom.velocity + 0.5f * StaticVariables.MDTimestepSqr * currAtom.accelerationNew;
			currAtom.accelerationOld = currAtom.accelerationNew;
			currAtom.accelerationNew = Vector3.zero;
			currAtom.transform.position = currAtom.position;
		}


		if (StaticVariables.currentPotential == StaticVariables.Potential.LennardJones)
		{
			if (StaticVariables.iTime % StaticVariables.nVerlet == 0)
				LennardJones.calculateNeighborList();
			// update the acceleration of all atoms
			for (int i=0; i< Atom.AllAtoms.Count-1; i++) 
			{
				Atom firstAtom = Atom.AllAtoms[i];
				for (int j = 0; j < firstAtom.neighborList.Count; j++)
				{
					Atom secondAtom = firstAtom.neighborList[j];
					LennardJones.getForce(firstAtom, secondAtom);
				}
			}
		}
		else if (StaticVariables.currentPotential == StaticVariables.Potential.Buckingham)
		{
			if (StaticVariables.iTime % StaticVariables.nVerlet == 0)
				Buckingham.calculateNeighborList();
			// update the acceleration of all atoms
			for (int i=0; i< Atom.AllAtoms.Count-1; i++) 
			{
				Atom firstAtom = Atom.AllAtoms[i];
				for (int j = 0; j < firstAtom.neighborList.Count; j++)
				{
					Atom secondAtom = firstAtom.neighborList[j];
					Buckingham.getForce(firstAtom, secondAtom);
				}
			}
		}
		
		// update the velocity of all atoms
		for (int i = 0; i < Atom.AllAtoms.Count; i++)
		{
			Atom currAtom = Atom.AllAtoms[i];
			currAtom.velocity = currAtom.velocity + 0.5f * StaticVariables.MDTimestep * (currAtom.accelerationOld + currAtom.accelerationNew);
			currAtom.velocity = StaticVariables.sqrtAlpha * currAtom.velocity;
		}
	}



	//reflect the atoms from the walls
	void ReflectFromWalls()
	{
		Vector3 boxDimension = new Vector3 (StaticVariables.myEnvironment.width-2.0f * StaticVariables.myEnvironment.errorBuffer, StaticVariables.myEnvironment.height-2.0f * StaticVariables.myEnvironment.errorBuffer , StaticVariables.myEnvironment.depth-2.0f * StaticVariables.myEnvironment.errorBuffer);
		
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
	
	void CalculateEnergy()
	{
		StaticVariables.potentialEnergy = 0.0f;
		StaticVariables.kineticEnergy = 0.0f;
		StaticVariables.currentTemperature = 0.0f;

		for (int i = 0; i < Atom.AllAtoms.Count - 1; i++)
		{
			Atom firstAtom = Atom.AllAtoms[i];
			
			// calculate kinetic energy of each atom
			float velocitySqr = firstAtom.velocity.sqrMagnitude;
			StaticVariables.kineticEnergy += 0.5f * firstAtom.massamu * StaticVariables.amuToKg * velocitySqr * StaticVariables.angstromsToMeters * StaticVariables.angstromsToMeters;

			// calculate potential energy between each pair of atoms
			if (StaticVariables.currentPotential == StaticVariables.Potential.LennardJones)
			{
				for (int j = 0; j < firstAtom.neighborList.Count; j++)
				{
					Atom secondAtom = firstAtom.neighborList[j];
					StaticVariables.potentialEnergy += LennardJones.getPotential(firstAtom, secondAtom);
				}
			}
			else if (StaticVariables.currentPotential == StaticVariables.Potential.Buckingham)
			{
				for (int j = 0; j < firstAtom.neighborList.Count; j++)
				{
					Atom secondAtom = firstAtom.neighborList[j];
					StaticVariables.potentialEnergy += Buckingham.getPotential(firstAtom, secondAtom);
				}
			}

		}
		
		StaticVariables.currentTemperature = StaticVariables.kineticEnergy / 1.5f / (float)Atom.AllAtoms.Count / StaticVariables.kB;
		calculateSqrtAlpha();
		
	}
	
	void calculateSqrtAlpha()
	{
		float alpha = StaticVariables.desiredTemperature / StaticVariables.currentTemperature;
		float draggedAlpha = 0.0f;
		float draggedTemperature = 0.0f;
		
		if (StaticVariables.currentTemperature < 0.000000000001f)
		{
			draggedAlpha = 1.0f;
		}
		else if (StaticVariables.currentTemperature > 5000.0f)
		{
			draggedAlpha = alpha;
		}
		else if (alpha > 1)
		{
			draggedTemperature = (StaticVariables.desiredTemperature - StaticVariables.currentTemperature) * StaticVariables.alphaDrag + StaticVariables.currentTemperature;
			draggedAlpha = draggedTemperature / StaticVariables.currentTemperature;
		}
		else if (alpha < 1)
		{
			draggedTemperature = StaticVariables.currentTemperature - ((StaticVariables.currentTemperature - StaticVariables.desiredTemperature) * StaticVariables.alphaDrag);
			draggedAlpha = draggedTemperature / StaticVariables.currentTemperature;
		}
		else
		{
			draggedAlpha = 1.0f;
		}
		StaticVariables.sqrtAlpha = (float)Math.Pow(draggedAlpha, 0.5f);
	}
}