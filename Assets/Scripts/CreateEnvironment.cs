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

	//this variables points to the instance of the create environment
	public static CreateEnvironment myEnvironment;

	void Awake(){
		CreateEnvironment.myEnvironment = this;
	}

	void Start () {

		// pre-compute coefficients used in various types of potentials so that we don't have to calculate them dynamically
		preCompute ();

		//create the atoms
		InitAtoms ();
		//InitAtomsDebug ();

		centerPos = Vector3.zero;
		initialCenterPos = centerPos;
		
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
	}

	void Update ()
	{

		//when the volume of the box changes the width, height, and depth must change as well. The lines and the text must respond accordingly
		width = Mathf.Pow (volume, (1.0f / 3.0f));
		height = Mathf.Pow (volume, (1.0f / 3.0f));
		depth = Mathf.Pow (volume, (1.0f / 3.0f));

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

	public void preCompute()
	{
		if (Potential.currentPotential == Potential.potentialType.LennardJones)
			Potential.myPotential = new LennardJones();
		else if(Potential.currentPotential == Potential.potentialType.Buckingham)
			Potential.myPotential = new Buckingham();

		Potential.myPotential.preCompute ();
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
		if (Potential.currentPotential == Potential.potentialType.LennardJones)
		{
			for (int i = 0; i < numMolecules; i++)
			{
				createAtom (molecules [0]);
			}
		}
		else if (Potential.currentPotential == Potential.potentialType.Buckingham)
		{
			for (int i = 0; i < numMolecules/2; i++)
			{
				createAtom (molecules [0]);
			}
			for (int i = numMolecules/2; i < numMolecules; i++)
			{
				createAtom (molecules [1]);
			}
		}
		Potential.myPotential.calculateVerletRadius ();

		myEnvironment.numMolecules = Atom.AllAtoms.Count;

		AtomTouchGUI atomTouchGUI = Camera.main.GetComponent<AtomTouchGUI> ();
		atomTouchGUI.AtomKick();
	}


	//initialize two atoms to fixed positions with zero velocity for debuging purposes.
	public void InitAtomsDebug()
	{
		//destroy and unregister all the current atoms
		for (int i = Atom.AllAtoms.Count-1; i >= 0; i--)
		{
			Atom currAtom = Atom.AllAtoms [i];
			Atom.UnregisterAtom(currAtom);
			Destroy (currAtom.gameObject);
		}

		{
			CreateEnvironment myEnvironment = CreateEnvironment.myEnvironment;
			Vector3 centerPos = myEnvironment.centerPos;
			Quaternion curRotation = Quaternion.Euler(0, 0, 0);

			Instantiate(molecules [0], myEnvironment.centerPos, curRotation);
			int i = Atom.AllAtoms.Count-1;
			Atom currAtom = Atom.AllAtoms[i];
			currAtom.gameObject.name = i.ToString();
        	currAtom.position =  new Vector3 (centerPos.x + 2.0f, centerPos.y , centerPos.z );
			currAtom.transform.position = currAtom.position;
		
			Instantiate(molecules [1], myEnvironment.centerPos, curRotation);
			i = Atom.AllAtoms.Count-1;
			currAtom = Atom.AllAtoms[i];
			currAtom.gameObject.name = i.ToString();
			currAtom.position =  new Vector3 (centerPos.x - 2.0f, centerPos.y , centerPos.z );
			currAtom.transform.position = currAtom.position;

			Potential.myPotential.calculateVerletRadius ();
			myEnvironment.numMolecules = Atom.AllAtoms.Count;
		}
	}



	// this method creates a new atom from the type of the preFab and checks the position to have a far enough distance from other atoms
	public void createAtom(Rigidbody preFab)
	{
		CreateEnvironment myEnvironment = CreateEnvironment.myEnvironment;
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
			Vector3 deltaR = Boundary.myBoundary.deltaPosition(currAtom, otherAtom);

			float distanceSqr = deltaR.sqrMagnitude;
			//only get the forces of the atoms that are within the cutoff range
			if (distanceSqr < (3.0f * 3.0f))
			{
				proximityFlag = false;
			}
		}
		return proximityFlag;
	}

}
