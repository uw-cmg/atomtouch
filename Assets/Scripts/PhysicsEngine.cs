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
		Application.targetFrameRate = 70;
	}
	void Start(){
		//StartCoroutine(DoPhysics());
	}
	//coroutine: I'm crazy
	//TODO: when timer stopped or slowed down, lower/increase update rate
	//IEnumerator DoPhysics()
	void FixedUpdate()
    {
       // while (true)
        //{
          //  yield return new WaitForSeconds(1f / 100f);
 
            if (!StaticVariables.pauseTime && !StaticVariables.draggingAtoms) 
			{

				VelocityVerlet();
				Boundary.myBoundary.Apply();
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
					float timePerStepPerAtom = timePerStep / CreateEnvironment.myEnvironment.numMolecules;
					Debug.Log ("Delta Time = " + deltaTime +" , iTime = " + StaticVariables.iTime + " , time per step = " + timePerStep + " , time per step per atom = " + timePerStepPerAtom);
				}


				StaticVariables.currentTime += StaticVariables.MDTimestepInPicosecond;
				Graph.numMDStepSinceLastRecord ++;
				StaticVariables.iTime ++;
			}
			else
			{
				// update the position of all atoms then initialize the acceleration to be updated
				for (int i=0; i< Atom.AllAtoms.Count; i++)
				{
					Atom currAtom = Atom.AllAtoms[i];
					currAtom.transform.position = currAtom.position;
				}
				Boundary.myBoundary.Apply();
				CalculateEnergy();
			}
	    //}
     } 
     /*
	void Update()
	{
		// check if atom velocity is below a minimum and kick it if necessary.
		
		float minVelocity;
		for(int i = 0; i < Atom.AllAtoms.Count; i++){
			Atom currAtom = Atom.AllAtoms[i];
			minVelocity = tmpCoeff/Mathf.Sqrt(currAtom.massamu)/10.0f;
			if (currAtom.velocity.magnitude < minVelocity)
			{
				//this is maximum random velocity.
				float maxVelocity = 2.0f*minVelocity;
				
				if(UnityEngine.Random.Range(0.0f, 1.0f) > .5f){
					currAtom.velocity.x = UnityEngine.Random.Range(1.0f * maxVelocity, 5.0f * maxVelocity);
				}
				else{
					currAtom.velocity.x = UnityEngine.Random.Range(-5.0f * maxVelocity, -1.0f * maxVelocity);
				}
				if(UnityEngine.Random.Range(0.0f, 1.0f) > .5f){
					currAtom.velocity.y = UnityEngine.Random.Range(1.0f * maxVelocity, 5.0f * maxVelocity);
				}
				else{
					currAtom.velocity.y = UnityEngine.Random.Range(-5.0f * maxVelocity, -1.0f * maxVelocity);
				}
				if(UnityEngine.Random.Range(0.0f, 1.0f) > .5f){
					currAtom.velocity.z = UnityEngine.Random.Range(1.0f * maxVelocity, 5.0f * maxVelocity);
				}
				else{
					currAtom.velocity.z = UnityEngine.Random.Range(-5.0f * maxVelocity, -1.0f * maxVelocity);
				}
			}
		}
	}
	*/
	
	void VelocityVerlet()
	{
		// update the position of all atoms then initialize the acceleration to be updated
		//if (StaticVariables.iTime % 10 != 0)return;
		
		for (int i=0; i< Atom.AllAtoms.Count; i++)
		{
			Atom currAtom = Atom.AllAtoms[i];
			
			currAtom.position = currAtom.position 
				+ StaticVariables.MDTimestep * currAtom.velocity 
				+ 0.5f * StaticVariables.MDTimestepSqr * currAtom.accelerationNew;

			currAtom.accelerationOld = currAtom.accelerationNew;
			currAtom.accelerationNew = Vector3.zero;
			
			currAtom.transform.position = currAtom.position;
			

		}
		
		//if(!Mathf.Approximately(StaticVariables.currentTemperature
		//	,StaticVariables.desiredTemperature)){
		if (StaticVariables.iTime % (StaticVariables.nVerlet) == 0) 
		{
			Potential.myPotential.calculateNeighborList ();
			//PairDistributionFunction.calculateAveragePairDistribution();
		}
		//}
		
		// update the acceleration of all atoms
		for (int i=0; i< Atom.AllAtoms.Count; i++) 
		{
			Atom firstAtom = Atom.AllAtoms[i];
			for (int j = 0; j < firstAtom.neighborList.Count; j++)
			{
				Atom secondAtom = firstAtom.neighborList[j];
				Potential.myPotential.getForce(firstAtom, secondAtom);
			}
		}


		// update the velocity of all atoms
		for (int i = 0; i < Atom.AllAtoms.Count; i++)
		{
			Atom currAtom = Atom.AllAtoms[i];
			currAtom.velocity = currAtom.velocity 
			+ 0.5f * StaticVariables.MDTimestep * (currAtom.accelerationOld + currAtom.accelerationNew);
			currAtom.velocity = StaticVariables.sqrtAlpha * currAtom.velocity;

			//kick if atom looks still
			//TODO: move to a coroutine in Atom.cs
			float tmpCoeff = Mathf.Sqrt(3.0f*StaticVariables.kB*StaticVariables.desiredTemperature/StaticVariables.amuToKg)/StaticVariables.angstromsToMeters;
			float minVelocity = tmpCoeff/Mathf.Sqrt(currAtom.massamu)/10.0f;
			if (currAtom.velocity.magnitude < minVelocity)
			{
				//this is maximum random velocity.
				float maxVelocity = 2.0f*minVelocity;
				
				if(UnityEngine.Random.Range(0.0f, 1.0f) > .5f){
					currAtom.velocity.x = UnityEngine.Random.Range(1.0f * maxVelocity, 5.0f * maxVelocity);
				}
				else{
					currAtom.velocity.x = UnityEngine.Random.Range(-5.0f * maxVelocity, -1.0f * maxVelocity);
				}
				if(UnityEngine.Random.Range(0.0f, 1.0f) > .5f){
					currAtom.velocity.y = UnityEngine.Random.Range(1.0f * maxVelocity, 5.0f * maxVelocity);
				}
				else{
					currAtom.velocity.y = UnityEngine.Random.Range(-5.0f * maxVelocity, -1.0f * maxVelocity);
				}
				if(UnityEngine.Random.Range(0.0f, 1.0f) > .5f){
					currAtom.velocity.z = UnityEngine.Random.Range(1.0f * maxVelocity, 5.0f * maxVelocity);
				}
				else{
					currAtom.velocity.z = UnityEngine.Random.Range(-5.0f * maxVelocity, -1.0f * maxVelocity);
				}
			}
		}	
		
	}

	
	void CalculateEnergy()
	{
		StaticVariables.potentialEnergy = 0.0f;
		StaticVariables.kineticEnergy = 0.0f;
		StaticVariables.currentTemperature = 0.0f;

		for (int i = 0; i < Atom.AllAtoms.Count; i++)
		{
			Atom firstAtom = Atom.AllAtoms[i];
			
			// calculate kinetic energy of each atom
			float velocitySqr = firstAtom.velocity.sqrMagnitude;
			StaticVariables.kineticEnergy += 0.5f * firstAtom.massamu * StaticVariables.amuToKg * velocitySqr * StaticVariables.angstromsToMeters * StaticVariables.angstromsToMeters;

			// calculate potential energy between each pair of atoms
			for (int j = 0; j < firstAtom.neighborList.Count; j++)
			{
				Atom secondAtom = firstAtom.neighborList[j];
				StaticVariables.potentialEnergy += Potential.myPotential.getPotential(firstAtom, secondAtom);
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
		else if (StaticVariables.currentTemperature > StaticVariables.tempRangeHigh * StaticVariables.tempScaler)
		{
			draggedAlpha = alpha;
		}
		else if (alpha > 1)
		{
			draggedTemperature 
				= (StaticVariables.desiredTemperature - StaticVariables.currentTemperature) 
				* StaticVariables.alphaDrag + StaticVariables.currentTemperature;
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