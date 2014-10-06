/**
 * Class: CreateEnvironment.cs
 * Created by: Justin Moeller
 * Description: This class handles anything that has to do with the creation of the environment.
 * (i.e the atoms, the box, the lines, and the width/height/depth text). Most of the functionality 
 * of this class happens when the game begins, however, the box, the lines, and the text all must be 
 * scaled when the user changes the volume of the box. Additionally, to cut down on some computation,
 * all of the sigma values are pre-computed and then stored in a dictionary. The key to the dictionary
 * is a String made up of the two atoms for which you want the sigma value for. (i.e "CopperCopper" or
 * "CopperGold" or "GoldPlatinum") All of the atoms are also named (given a number 0-numAtoms) for easier 
 * access and debugging. 
 * 
 * 
 **/ 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CreateEnvironment : MonoBehaviour {

	public int numMolecules = 10;
	public List<Rigidbody> molecules = new List<Rigidbody>();
	public int moleculeToSpawn = 0;
	public GameObject plane;
	public Vector3 centerPos;
	public float errorBuffer = 0.5f;
	public Material mat;
	public float width;
	public float height;
	public float depth;
	public float volume = 8000.0f;
	public TextMesh textMeshPrefab;

	private TextMesh bottomText;
	private TextMesh sideText;
	private TextMesh depthText;
	[HideInInspector]public GameObject bottomPlane;
	private GameObject topPlane;
	private GameObject backPlane;
	private GameObject frontPlane;
	private GameObject rightPlane;
	private GameObject leftPlane;
	public Vector3 initialCenterPos;
	
	void Start () {

		//the min sigma value and max sigma value are used for precalculating LJ forces.
		Atom atomScript = molecules [1].GetComponent<Atom> ();
		StaticVariables.sigmaValueMin = atomScript.sigma;
		StaticVariables.sigmaValueMax = atomScript.sigma;

		//precompute all of the coefficients for potentials in the system so it doesnt have to be done dynamically
		for (int i = 0; i < molecules.Count; i++) {
			atomScript = molecules[i].GetComponent<Atom>();
			float currentSigma = atomScript.sigma;
			StaticVariables.sigmaValues[atomScript.atomID*atomScript.atomID] = atomScript.sigma;

			if (currentSigma > StaticVariables.sigmaValueMax) {
				StaticVariables.sigmaValueMax = currentSigma;
			}
			if (currentSigma < StaticVariables.sigmaValueMin) {
				StaticVariables.sigmaValueMin = currentSigma;
			}
			float currentA = atomScript.buck_A;
			StaticVariables.coeff_A.Add(atomScript.atomName+atomScript.atomName, currentA);
			float currentB = atomScript.buck_B;
			StaticVariables.coeff_B.Add(atomScript.atomName+atomScript.atomName, currentB);
			float currentC = atomScript.buck_C;
			StaticVariables.coeff_C.Add(atomScript.atomName+atomScript.atomName, currentC);
			float currentD = atomScript.buck_D;
			StaticVariables.coeff_D.Add(atomScript.atomName+atomScript.atomName, currentD);
		}
		for (int i = 0; i < molecules.Count; i++) {
			Atom firstAtomScript = molecules[i].GetComponent<Atom>();
			for(int j = i+1; j < molecules.Count; j++){
				Atom secondAtomScript = molecules[j].GetComponent<Atom>();

				float currentSigma = Mathf.Sqrt(firstAtomScript.sigma+secondAtomScript.sigma);
				StaticVariables.sigmaValues[firstAtomScript.atomID*secondAtomScript.atomID] = currentSigma;

				if (currentSigma > StaticVariables.sigmaValueMax) {
					StaticVariables.sigmaValueMax = currentSigma;
				}
				if (currentSigma < StaticVariables.sigmaValueMin) {
					StaticVariables.sigmaValueMin = currentSigma;
				}


				float currentA = Mathf.Sqrt(firstAtomScript.buck_A*secondAtomScript.buck_A);
				StaticVariables.coeff_A.Add(firstAtomScript.atomName+secondAtomScript.atomName, currentA);
				StaticVariables.coeff_A.Add(secondAtomScript.atomName+firstAtomScript.atomName, currentA);

				float currentB = Mathf.Sqrt(firstAtomScript.buck_B*secondAtomScript.buck_B);
				StaticVariables.coeff_B.Add(firstAtomScript.atomName+secondAtomScript.atomName, currentB);
				StaticVariables.coeff_B.Add(secondAtomScript.atomName+firstAtomScript.atomName, currentB);

				float currentC = Mathf.Sqrt(firstAtomScript.buck_C*secondAtomScript.buck_C);
				StaticVariables.coeff_C.Add(firstAtomScript.atomName+secondAtomScript.atomName, currentC);
				StaticVariables.coeff_C.Add(secondAtomScript.atomName+firstAtomScript.atomName, currentC);

				float currentD = Mathf.Sqrt(firstAtomScript.buck_D*secondAtomScript.buck_D);
				StaticVariables.coeff_D.Add(firstAtomScript.atomName+secondAtomScript.atomName, currentD);
				StaticVariables.coeff_D.Add(secondAtomScript.atomName+firstAtomScript.atomName, currentD);
			}
		}

		// precalculate the LennardJones potential and store it in preLennarJones list.

		int nR = (int)((StaticVariables.cutoff/StaticVariables.sigmaValueMin)/(StaticVariables.deltaR/StaticVariables.sigmaValueMax))+2;

		StaticVariables.preLennardJones = new float[nR];

		for (int i = 0; i < nR; i++) {
			float distance = (float)i * StaticVariables.deltaR / StaticVariables.sigmaValueMax;
			float magnitude = CalcLennardJonesForce (distance);
			StaticVariables.preLennardJones[i] = magnitude;
		}

		centerPos = new Vector3 (0.0f, 0.0f, 0.0f);
		initialCenterPos = centerPos;
		CameraScript cameraScript = Camera.main.GetComponent<CameraScript> ();

		//create the atoms
		InitAtoms ();

		//figure out the dimensions of the box based on the volume
		width = (float)Math.Pow (volume, (1.0f / 3.0f));
		height = (float)Math.Pow (volume, (1.0f / 3.0f));
		depth = (float)Math.Pow (volume, (1.0f / 3.0f));
		
		//create the bottom plane
		Quaternion bottonPlaneRotation = Quaternion.Euler (0.0f, 0.0f, 0.0f);
		Vector3 bottomPlanePos = new Vector3 (centerPos.x, centerPos.y - (height/2.0f), centerPos.z);
		bottomPlane = Instantiate (plane, bottomPlanePos, bottonPlaneRotation) as GameObject;
		bottomPlane.transform.localScale = new Vector3 (width / 10.0f, height / 10.0f, depth / 10.0f);
		bottomPlane.name = "BottomPlane";
		bottomPlane.tag = "Plane";

		//create the top plane
		Quaternion topPlaneRotation = Quaternion.Euler (0.0f, 180.0f, 180.0f);
		Vector3 topPlanePos = new Vector3 (centerPos.x, centerPos.y + (height/2.0f), centerPos.z);
		topPlane = Instantiate (plane, topPlanePos, topPlaneRotation) as GameObject;
		topPlane.transform.localScale = new Vector3 (width / 10.0f, height / 10.0f, depth / 10.0f);
		topPlane.name = "TopPlane";
		topPlane.tag = "Plane";

		//create the back plane
		Quaternion backPlaneRotation = Quaternion.Euler (270.0f, 0.0f, 0.0f);
		Vector3 backPlanePos = new Vector3 (centerPos.x, centerPos.y, centerPos.z + (depth/2.0f));
		backPlane = Instantiate (plane, backPlanePos, backPlaneRotation) as GameObject;
		backPlane.transform.localScale = new Vector3 (width / 10.0f, depth / 10.0f, height / 10.0f);
		backPlane.name = "BackPlane";
		backPlane.tag = "Plane";

		//create the front plane
		Quaternion frontPlaneRotation = Quaternion.Euler (90.0f, 0.0f, 0.0f);
		Vector3 frontPlanePos = new Vector3 (centerPos.x, centerPos.y, centerPos.z - (depth/2.0f));
		frontPlane = Instantiate (plane, frontPlanePos, frontPlaneRotation) as GameObject;
		frontPlane.transform.localScale = new Vector3 (width / 10.0f, depth / 10.0f, height / 10.0f);
		frontPlane.name = "FrontPlane";
		frontPlane.tag = "Plane";

		//create the right plane
		Quaternion rightPlaneRotation = Quaternion.Euler (0.0f, 0.0f, 90.0f);
		Vector3 rightPlanePos = new Vector3 (centerPos.x + (width/2.0f), centerPos.y, centerPos.z);
		rightPlane = Instantiate (plane, rightPlanePos, rightPlaneRotation) as GameObject;
		rightPlane.transform.localScale = new Vector3 (height / 10.0f, width / 10.0f, depth / 10.0f);
		rightPlane.name = "RightPlane";
		rightPlane.tag = "Plane";

		//create the left plane
		Quaternion leftPlaneRotation = Quaternion.Euler (0.0f, 0.0f, 270.0f);
		Vector3 leftPlanePos = new Vector3 (centerPos.x - (width/2.0f), centerPos.y, centerPos.z);
		leftPlane = Instantiate (plane, leftPlanePos, leftPlaneRotation) as GameObject;
		leftPlane.transform.localScale = new Vector3 (height / 10.0f, width / 10.0f, depth / 10.0f);
		leftPlane.name = "LeftPlane";
		leftPlane.tag = "Plane";
		
		//name the atoms, simply make their name a number (i.e "0" or "1")
		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		for (int i = 0; i < allMolecules.Length; i++) {
			GameObject currAtom = allMolecules[i];
			currAtom.name = i.ToString();
		}

		//create the lines that border the box and the text of width, height, and depth
		//bottom line and text
		Color lineColor = new Color (Color.yellow.r, Color.yellow.g, Color.yellow.b, .6f);
		bottomText = Instantiate(textMeshPrefab, new Vector3(bottomPlanePos.x - 2.0f, bottomPlanePos.y - 1.0f, bottomPlanePos.z - (depth/2.0f)), Quaternion.identity) as TextMesh;
		bottomText.text = width.ToString() + " Angstroms";
		LineRenderer bottomLine = bottomText.transform.gameObject.AddComponent<LineRenderer> ();
		bottomLine.material = mat;
		bottomLine.SetColors(lineColor, lineColor);
		bottomLine.SetWidth(0.2F, 0.2F);
		bottomLine.SetVertexCount(2);

		//side line and text
		sideText = Instantiate(textMeshPrefab, new Vector3(bottomPlanePos.x + (width/2.0f) + 1.0f, bottomPlanePos.y + (height*7/10.0f), bottomPlanePos.z - (depth/2.0f)), Quaternion.identity) as TextMesh;
		sideText.text = VerticalText(height.ToString() + " Angstroms");
		LineRenderer sideLine = sideText.transform.gameObject.AddComponent<LineRenderer> ();
		sideLine.material = mat;
		sideLine.SetColors(lineColor, lineColor);
		sideLine.SetWidth(0.2F, 0.2F);
		sideLine.SetVertexCount(2);

		//depth line and text
		depthText = Instantiate(textMeshPrefab, new Vector3(centerPos.x + (width/2.0f), bottomPlanePos.y - 1.0f, centerPos.z - 2.0f), Quaternion.Euler(0.0f, -90.0f, 0.0f)) as TextMesh;
		depthText.text = depth.ToString() + " Angstroms";
		LineRenderer depthLine = depthText.transform.gameObject.AddComponent<LineRenderer> ();
		depthLine.material = mat;
		depthLine.SetColors(lineColor, lineColor);
		depthLine.SetWidth(0.2F, 0.2F);
		depthLine.SetVertexCount(2);

		//give all of the atoms a random velocity on startup
		AtomTouchGUI atomTouchGUI = Camera.main.GetComponent<AtomTouchGUI> ();
		atomTouchGUI.AtomKick ();
	}

	void Update () {

		//when the volume of the box changes the width, height, and depth must change as well. The lines and the text must respond accordingly
		width = (float)Math.Pow (volume, (1.0f / 3.0f));
		height = (float)Math.Pow (volume, (1.0f / 3.0f));
		depth = (float)Math.Pow (volume, (1.0f / 3.0f));
		
		CameraScript cameraScript = Camera.main.GetComponent<CameraScript> ();

		//change the position of the bottom line
		LineRenderer bottomLine = bottomText.GetComponent<LineRenderer> ();
		bottomLine.SetPosition(0, new Vector3(bottomPlane.transform.position.x - (width/2.0f), bottomPlane.transform.position.y, bottomText.transform.position.z));
		bottomLine.SetPosition(1, new Vector3(bottomPlane.transform.position.x + (width/2.0f), bottomPlane.transform.position.y, bottomText.transform.position.z));

		//change the position of the side line
		LineRenderer sideLine = sideText.GetComponent<LineRenderer> ();
		sideLine.SetPosition (0, new Vector3 (bottomPlane.transform.position.x + (width/2.0f), bottomPlane.transform.position.y, sideText.transform.position.z));
		sideLine.SetPosition (1, new Vector3 (bottomPlane.transform.position.x + (width/2.0f), bottomPlane.transform.position.y + height, sideText.transform.position.z));

		//change the position of the depth line
		LineRenderer depthLine = depthText.GetComponent<LineRenderer> ();
		depthLine.SetPosition(0, new Vector3(bottomPlane.transform.position.x + (width/2.0f), bottomPlane.transform.position.y, bottomPlane.transform.position.z - (depth/2.0f)));
		depthLine.SetPosition(1, new Vector3(bottomPlane.transform.position.x + (width/2.0f), bottomPlane.transform.position.y, bottomPlane.transform.position.z + (depth/2.0f)));

		//change the text of the labels and change their positions
		bottomText.text = width.ToString() + " Angstroms";
		sideText.text = VerticalText(height.ToString() + " Angstroms");
		depthText.text = depth.ToString() + " Angstroms";
		sideText.transform.position = new Vector3 (bottomPlane.transform.position.x + (width / 2.0f) + 1.0f, bottomPlane.transform.position.y + (height*7/10.0f), bottomPlane.transform.position.z - (depth / 2.0f));
		depthText.transform.position = new Vector3 (bottomPlane.transform.position.x + (width / 2.0f), bottomPlane.transform.position.y - 1.0f, bottomPlane.transform.position.z - 2.0f);
		bottomText.transform.position = new Vector3 (bottomPlane.transform.position.x - 2.0f, bottomPlane.transform.position.y - 1.0f, bottomPlane.transform.position.z - (depth / 2.0f));

		//change the position of the box
		rightPlane.transform.position = new Vector3 (initialCenterPos.x + (width/2.0f), initialCenterPos.y, initialCenterPos.z);
		leftPlane.transform.position = new Vector3 (initialCenterPos.x - (width/2.0f), initialCenterPos.y, initialCenterPos.z);
		bottomPlane.transform.position = new Vector3 (initialCenterPos.x, initialCenterPos.y - (height/2.0f), initialCenterPos.z);
		topPlane.transform.position = new Vector3 (initialCenterPos.x, initialCenterPos.y + (height/2.0f), initialCenterPos.z);
		backPlane.transform.position = new Vector3 (initialCenterPos.x, initialCenterPos.y, initialCenterPos.z + (depth/2.0f));
		frontPlane.transform.position = new Vector3 (initialCenterPos.x, initialCenterPos.y, initialCenterPos.z - (depth/2.0f));

		//change the size of the box
		bottomPlane.transform.localScale = new Vector3 (width / 10.0f, height / 10.0f, depth / 10.0f);
		topPlane.transform.localScale = new Vector3 (width / 10.0f, height / 10.0f, depth / 10.0f);
		backPlane.transform.localScale = new Vector3 (width / 10.0f, depth / 10.0f, height / 10.0f);
		frontPlane.transform.localScale = new Vector3 (width / 10.0f, depth / 10.0f, height / 10.0f);
		rightPlane.transform.localScale = new Vector3 (height / 10.0f, width / 10.0f, depth / 10.0f);
		leftPlane.transform.localScale = new Vector3 (height / 10.0f, width / 10.0f, depth / 10.0f);


	}

	//make the side text become vertical
	String VerticalText(String text){
		System.Text.StringBuilder verticalText = new System.Text.StringBuilder (text.Length * 2);
		for (int i = 0; i < text.Length; i++) {
			verticalText.Append(text[i]).Append("\n");
		}
		return verticalText.ToString();
	}

	//initialize the atoms to a random position and to the original number of atoms
	public void InitAtoms(){
		//GameObject[] oldAllMolecules = GameObject.FindGameObjectsWithTag ("Molecule");
		//for (int i = 0; i < oldAllMolecules.Length; i++) {
		//	GameObject currAtom = oldAllMolecules [i];
		//	Destroy (currAtom);
		//}

		for (int i = Atom.AllMolecules.Count-1; i >= 0; i--) {
			Atom currAtom = Atom.AllMolecules [i];
			Destroy (currAtom.gameObject);
		}
		//Debug.Log ("Atom.AllMolecules.Count" + Atom.AllMolecules.Count);

		if (StaticVariables.currentPotential == StaticVariables.Potential.Buckingham) {
			for (int i = 0; i < (int)(numMolecules/2); i++) {
				Vector3 position = new Vector3 (centerPos.x + (UnityEngine.Random.Range (-(width / 2.0f) + errorBuffer, (width / 2.0f) - errorBuffer)), centerPos.y + (UnityEngine.Random.Range (-(height / 2.0f) + errorBuffer, (height / 2.0f) - errorBuffer)), centerPos.z + (UnityEngine.Random.Range (-(depth / 2.0f) + errorBuffer, (depth / 2.0f) - errorBuffer)));
				Quaternion rotation = Quaternion.Euler (0, 0, 0);
				Instantiate (molecules [0].rigidbody, position, rotation);
			}
			for (int i = (int)(numMolecules/2); i < numMolecules; i++) {
				Vector3 position = new Vector3 (centerPos.x + (UnityEngine.Random.Range (-(width / 2.0f) + errorBuffer, (width / 2.0f) - errorBuffer)), centerPos.y + (UnityEngine.Random.Range (-(height / 2.0f) + errorBuffer, (height / 2.0f) - errorBuffer)), centerPos.z + (UnityEngine.Random.Range (-(depth / 2.0f) + errorBuffer, (depth / 2.0f) - errorBuffer)));
				Quaternion rotation = Quaternion.Euler (0, 0, 0);
				Instantiate (molecules [1].rigidbody, position, rotation);
			}
		} else {
			for (int i = 0; i < numMolecules; i++) {
				Vector3 position = new Vector3 (centerPos.x + (UnityEngine.Random.Range (-(width / 2.0f) + errorBuffer, (width / 2.0f) - errorBuffer)), centerPos.y + (UnityEngine.Random.Range (-(height / 2.0f) + errorBuffer, (height / 2.0f) - errorBuffer)), centerPos.z + (UnityEngine.Random.Range (-(depth / 2.0f) + errorBuffer, (depth / 2.0f) - errorBuffer)));
				Quaternion rotation = Quaternion.Euler (0, 0, 0);
				Instantiate (molecules [moleculeToSpawn].rigidbody, position, rotation);
			}
		}

		for (int i = 0; i < Atom.AllMolecules.Count; i++) {
			Atom currAtom = Atom.AllMolecules[i];
			currAtom.name = i.ToString();
		}
		AtomTouchGUI atomTouchGUI = Camera.main.GetComponent<AtomTouchGUI> ();
		atomTouchGUI.AtomKick();
	}


	//the function returns the LennarJones force on the atom given the list of the atoms that are within range of it
	float CalcLennardJonesForce(float distance){
		double invDistance2 = 1.0 / distance / distance;
		double invDistance6 = invDistance2 * invDistance2 * invDistance2;
		double r_min = StaticVariables.r_min_multiplier;
		
		double forceMagnitude = 0.0;

		if (distance > r_min) {
			forceMagnitude = invDistance2* invDistance6 * (invDistance6 - 0.5);
		
		// Smooth the potential to go to a constant not infinity at r=0
		} else {
			double invr_min = 1 / r_min; 
			double invr_min2 = invr_min * invr_min;
			double invr_min6 = invr_min2 * invr_min2 * invr_min2;
					
			double magnitude_Vmin = invr_min2 * invr_min6 * (invr_min6 - 0.5);
					
			double r_Vmax = r_min / 1.5;
			double invr_Vmax2 = 1 / r_Vmax / r_Vmax;
			double invr_Vmax6 = invr_Vmax2 * invr_Vmax2 * invr_Vmax2;
			
			double magnitude_Vmax = invr_Vmax2 * invr_Vmax6 * (invr_Vmax6 - 0.5);
					
			double part1 = (distance / r_min) * (Math.Exp (distance - r_min));
			double part2 = magnitude_Vmax - magnitude_Vmin;
			forceMagnitude = magnitude_Vmax - (part1 * part2);
		}
		float magnitude = (float)forceMagnitude;
		return magnitude;
	}

}
