using UnityEngine;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;

public class InputOutput : MonoBehaviour {

	public static void WritePotential(float[, ,] myPotential)
	{
		StreamWriter potentialFile;
		potentialFile = new StreamWriter("potential.txt");
		int numAtomTypes = myPotential.GetLength(0);
		int nR = myPotential.GetLength(2);
		
		for (int iR = 0; iR < nR; iR++)
		{
			for (int iAtom1 = 0; iAtom1 < numAtomTypes; iAtom1++)
			{
				for (int iAtom2 = 0; iAtom2 < numAtomTypes; iAtom2++)
				{
					potentialFile.WriteLine(myPotential[iAtom1, iAtom2, iR].ToString("E6"));
				}
			}
		}
		potentialFile.Close();
	}
	
	public static void WriteForce(float[, ,] myForce)
	{
		StreamWriter forceFile;
		forceFile = new StreamWriter("force.txt");
		int numAtomTypes = myForce.GetLength(0);
		int nR = myForce.GetLength(2);
		
		for (int iR = 0; iR < nR; iR++)
		{
			for (int iAtom1 = 0; iAtom1 < numAtomTypes; iAtom1++)
			{
				for (int iAtom2 = 0; iAtom2 < numAtomTypes; iAtom2++)
				{
					forceFile.WriteLine(myForce[iAtom1, iAtom2, iR].ToString("E6"));
				}
			}
		}
		forceFile.Close();
	}
	
	public static void ReadPotential(float[, ,] myPotential)
	{
		//string allLines;
		string [] lineArray;
		int numAtomTypes = myPotential.GetLength(0);
		int nR = myPotential.GetLength(2);
		
		lineArray = File.ReadAllLines ("potential.txt");
		if (lineArray.Length != (nR*numAtomTypes*numAtomTypes))
		{
			Debug.Log("Input file does not match!");
			Debug.Break();
		}
		
		
		int lineNumber = 0;
		for (int iR = 0; iR < nR; iR++)
		{
			for (int iAtom1 = 0; iAtom1 < numAtomTypes; iAtom1++)
			{
				for (int iAtom2 = 0; iAtom2 < numAtomTypes; iAtom2++)
				{
					myPotential[iAtom1, iAtom2, iR] = float.Parse(lineArray[lineNumber]);
					lineNumber++;
					
				}
			}
		}
	}
	
	public static void ReadForce(float[, ,] myForce)
	{
		//string allLines;
		string [] lineArray;
		int numAtomTypes = myForce.GetLength(0);
		int nR = myForce.GetLength(2);

		lineArray = File.ReadAllLines ("force.txt");
		Debug.Log ("Input file lines = " + lineArray.Length);
		int dummy = nR * numAtomTypes * numAtomTypes;
		Debug.Log ("Number of needed lines = " + dummy);
		if (lineArray.Length != (nR*numAtomTypes*numAtomTypes))
		{
			Debug.Log("Input file does not match!");
			Debug.Break();
		}


		int lineNumber = 0;
		for (int iR = 0; iR < nR; iR++)
		{
			for (int iAtom1 = 0; iAtom1 < numAtomTypes; iAtom1++)
			{
				for (int iAtom2 = 0; iAtom2 < numAtomTypes; iAtom2++)
				{
					myForce[iAtom1, iAtom2, iR] = float.Parse(lineArray[lineNumber]);
					lineNumber++;

				}
			}
		}
	}

}
