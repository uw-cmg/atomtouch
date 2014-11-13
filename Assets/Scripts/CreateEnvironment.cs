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

	void Awake(){
		StaticVariables.myEnvironment = this;
	}

	void Start () {

		// pre-compute coefficients used in various types of potentials so that we don't have to calculate them dynamically
		preCompute ();

		//create the atoms
		InitAtoms ();

		centerPos = Vector3.zero;
		initialCenterPos = centerPos;
		CameraScript cameraScript = Camera.main.GetComponent<CameraScript> ();
		
		//figure out the dimensions of the box based on the volume
		width = Mathf.Pow (volume, (1.0f / 3.0f));
		height = Mathf.Pow (volume, (1.0f / 3.0f));
		depth = Mathf.Pow (volume, (1.0f / 3.0f));
		
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

	void Update ()
	{

		//when the volume of the box changes the width, height, and depth must change as well. The lines and the text must respond accordingly
		width = Mathf.Pow (volume, (1.0f / 3.0f));
		height = Mathf.Pow (volume, (1.0f / 3.0f));
		depth = Mathf.Pow (volume, (1.0f / 3.0f));
		
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

	// This function pre-computes coefficients used in various types of potentials so that we don't have to calculate them dynamically
	public void preCompute()
	{
		for (int i = 0; i < molecules.Count; i++) {
			Atom firstAtom = molecules[i].GetComponent<Atom>();
			for(int j = 0; j < molecules.Count; j++){
				Atom secondAtom = molecules[j].GetComponent<Atom>();
				
				float currentSigma = Mathf.Sqrt(firstAtom.sigma*secondAtom.sigma);
				StaticVariables.sigmaValues[firstAtom.atomID,secondAtom.atomID] = currentSigma;
				StaticVariables.sigmaValuesSqr[firstAtom.atomID,secondAtom.atomID] = currentSigma * currentSigma;

				// when the pre-calculated normalized Lennard Jones force is multiplied by this coefficient the acceleration units is [Angstrom/second^2]
				float currentAccelCoeff = 24.0f * firstAtom.epsilon / (currentSigma * currentSigma * StaticVariables.angstromsToMeters * StaticVariables.angstromsToMeters * firstAtom.massamu * StaticVariables.amuToKg);
				StaticVariables.accelCoefficient[firstAtom.atomID, secondAtom.atomID] = currentAccelCoeff;
				
				float currentA = Mathf.Sqrt(firstAtom.buck_A*secondAtom.buck_A);
				StaticVariables.coeff_A[firstAtom.atomID,secondAtom.atomID] = currentA;
				
				float currentB = Mathf.Sqrt(firstAtom.buck_B * secondAtom.buck_B);
				StaticVariables.coeff_A[firstAtom.atomID, secondAtom.atomID] = currentB;
				
				float currentC = Mathf.Sqrt(firstAtom.buck_C * secondAtom.buck_C);
				StaticVariables.coeff_A[firstAtom.atomID, secondAtom.atomID] = currentC;
				
				float currentD = Mathf.Sqrt(firstAtom.buck_D * secondAtom.buck_D);
				StaticVariables.coeff_A[firstAtom.atomID, secondAtom.atomID] = currentD;

				StaticVariables.forceCoeffBK[firstAtom.atomID,secondAtom.atomID] = StaticVariables.fixedUpdateIntervalToRealTime * StaticVariables.fixedUpdateIntervalToRealTime / StaticVariables.mass100amuToKg / StaticVariables.angstromsToMeters;
			}
		}
		
		// precalculate the LennardJones potential and store it in preLennarJones array.
		int nR = (int)(StaticVariables.cutoff/StaticVariables.deltaR)+1;
		StaticVariables.preLennardJonesForce = new float[nR];
		StaticVariables.preLennardJonesPotential = new float[nR];
		
		for (int i = 0; i < nR; i++)
		{
			float distance = (float)i * StaticVariables.deltaR;
			StaticVariables.preLennardJonesForce[i] = calcLennardJonesForce(distance);
			StaticVariables.preLennardJonesPotential[i] = calcLennardJonesPotential(distance);
		}
	}

	//the function returns the LennarJones force on the atom given the list of the atoms that are within range of it
	private float calcLennardJonesForce(float distance)
	{
		float invDistance2 = 1.0f / distance / distance;
		float invDistance6 = invDistance2 * invDistance2 * invDistance2;
		float invCutoff2 = 1.0f / StaticVariables.cutoff / StaticVariables.cutoff;
		float invCutoff6 = invCutoff2 * invCutoff2 * invCutoff2;
		float r_min = StaticVariables.rMinMultiplier;
		
		float forceMagnitude = 0.0f;
		
		if (distance > r_min)
		{
			forceMagnitude = invDistance2 * ((2.0f * invDistance6 * invDistance6 - invDistance6) - (invCutoff2 / invDistance2) * (2.0f * invCutoff6 * invCutoff6 - invCutoff6 ));
		}
		// Smooth the potential to go to a constant not infinity at r=0
		else
		{
			float invr_min = 1 / r_min;
			float invr_min2 = invr_min * invr_min;
			float invr_min6 = invr_min2 * invr_min2 * invr_min2;
			float magnitude_Vmin = invr_min2 * ((2.0f * invr_min6 * invr_min6 - invr_min6) - (invCutoff2 / invr_min2) * (2.0f * invCutoff6 * invCutoff6 - invCutoff6));
			
			float r_Vmax = r_min / 1.5f;
			float invr_Vmax2 = 1 / r_Vmax / r_Vmax;
			float invr_Vmax6 = invr_Vmax2 * invr_Vmax2 * invr_Vmax2;
			float magnitude_Vmax = invr_Vmax2 * ((2.0f * invr_Vmax6 * invr_Vmax6 - invr_Vmax6) - (invCutoff2 / invr_Vmax2) * (2.0f * invCutoff6 * invCutoff6 - invCutoff6));
			
			float part1 = (distance / r_min) * ((float)Math.Exp(distance - r_min));
			float part2 = magnitude_Vmax - magnitude_Vmin;
			forceMagnitude = magnitude_Vmax - (part1 * part2);
		}
		
		return forceMagnitude;
	}
	
	//the function returns the LennarJones force on the atom given the list of the atoms that are within range of it
	private float calcLennardJonesPotential(float distance)
	{
		float invDistance2 = 1.0f / distance / distance;
		float invDistance6 = invDistance2 * invDistance2 * invDistance2;
		float invCutoff2 = 1.0f / StaticVariables.cutoff / StaticVariables.cutoff;
		float invCutoff6 = invCutoff2 * invCutoff2 * invCutoff2;
		float r_min = StaticVariables.rMinMultiplier;
		
		float potential = 0.0f;
		
		if (distance > 0.0f)
		{
			potential = 4.0f * ((invDistance6 * invDistance6 - invDistance6) + (6.0f * invCutoff6 * invCutoff6 - 3.0f * invCutoff6) * (invCutoff2 / invDistance2) - 7.0f * invCutoff6 * invCutoff6 + 4.0f * invCutoff6);
		}
		
		return potential;
	}

	//initialize the atoms to a random position and to the original number of atoms
	public void InitAtoms()
	{
		//destroy and unregister all the current atoms
		for (int i = Atom.AllAtoms.Count-1; i >= 0; i--)
		{
			Atom currAtom = Atom.AllAtoms [i];
			Atom.UnregisterAtom(currAtom);
			Destroy (currAtom.gameObject);
		}

		//initialize the new atoms
		for (int i = 0; i < numMolecules; i++)
		{
			createAtom(molecules[moleculeToSpawn]);
		}

		AtomTouchGUI atomTouchGUI = Camera.main.GetComponent<AtomTouchGUI> ();
		atomTouchGUI.AtomKick();
	}


	// this method creates a new atom from the type of the preFab and checks the position to have a far enough distance from other atoms
	public void createAtom(Rigidbody preFab)
	{
		CreateEnvironment myEnvironment = StaticVariables.myEnvironment;
		Quaternion curRotation = Quaternion.Euler(0, 0, 0);
		Instantiate(preFab, myEnvironment.centerPos, curRotation);
		int i = Atom.AllAtoms.Count-1;
		Atom currAtom = Atom.AllAtoms[i];
		currAtom.gameObject.name = i.ToString();

		float realWidth = myEnvironment.width - 2.0f * myEnvironment.errorBuffer;
		float realHeight = myEnvironment.height - 2.0f * myEnvironment.errorBuffer;
		float realDepth = myEnvironment.depth - 2.0f * myEnvironment.errorBuffer;
		Vector3 centerPos = myEnvironment.centerPos;

		int tryNumber = 0;

		bool proximityFlag = false;
		while ((proximityFlag == false) && (tryNumber < 100))
		{
			tryNumber ++;
			currAtom.position =  new Vector3 (centerPos.x + (UnityEngine.Random.Range (-realWidth/2.0f, realWidth/2.0f)), centerPos.y + (UnityEngine.Random.Range (-realHeight/2.0f, realHeight/2.0f)), centerPos.z + (UnityEngine.Random.Range (-realDepth/2.0f, realDepth/2.0f)));
			proximityFlag = myEnvironment.checkProximity(currAtom);
		}
		currAtom.transform.position = currAtom.position;

		if ((tryNumber == 100) && (proximityFlag == false)) 
		{
			Atom.UnregisterAtom(currAtom);
			Destroy (currAtom.gameObject);
			Debug.Log ("No space for atoms!");
		}
		
	}
	
	//check the distance between the atoms, if it is larger than the equilibrium position move accept the random number, otherwise pick another set of random positions.
	private bool checkProximity(Atom currAtom)
	{
		bool proximityFlag = true;
		for (int i = 0; i < Atom.AllAtoms.Count - 1; i++)
		{
			Atom otherAtom = Atom.AllAtoms[i];
			Vector3 deltaR = Vector3.zero;
			deltaR = currAtom.position - otherAtom.position;

			float distanceSqr = deltaR.sqrMagnitude;
			float finalSigmaSqr = StaticVariables.sigmaValuesSqr[currAtom.atomID, otherAtom.atomID];
			float normDistanceSqr = distanceSqr / finalSigmaSqr; // this is normalized distanceSqr to the sigmaValue
			
			//only get the forces of the atoms that are within the cutoff range
			if (normDistanceSqr < 1.259921)
			{
				proximityFlag = false;
			}
		}
		return proximityFlag;
	}

}
