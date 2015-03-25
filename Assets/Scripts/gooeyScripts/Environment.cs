

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class Environment : MonoBehaviour {
	
	public int numMolecules = 1;
	public Rigidbody[] molecules = new Rigidbody[5];
	
	public GameObject plane;
	public Vector3 centerPos;
	[HideInInspector]public float errorBuffer = 0.5f;
	//line
	public Material lineMat; 
	public Color lineColor;
	public float lineWidth = 0.2f;
	//cube
	[HideInInspector]public float width;
	[HideInInspector]public float height;
	[HideInInspector]public float depth;
	[HideInInspector]public float volume = 8000.0f;

	public TextMesh textMeshPrefab;
	private TextMesh bottomText;
	private TextMesh sideText;
	private TextMesh depthText;

	private TextMesh bottomPlusZText;
	private TextMesh bottomPlusYText;
	private TextMesh bottomPlusYZText;

	private TextMesh sidePlusXText;
	private TextMesh sidePlusZText;
	private TextMesh sidePlusXZText;

	private TextMesh depthPlusYText;
	private TextMesh depthPlusZText;
	private TextMesh depthPlusYZText;

	private Vector3 vx;
	private Vector3 vy;
	private Vector3 vz;
	//planes
	public static GameObject bottomPlane;
	public static GameObject topPlane;
	public static GameObject backPlane;
	public static GameObject frontPlane;
	public static GameObject rightPlane;
	public static GameObject leftPlane;
	[HideInInspector]public AtomTouchGUI atomTouchGUI;
	public Vector3 initialCenterPos;
	
	//this variables points to the instance of the create environment
	public static Environment myEnvironment;
	
	void Awake(){
		Environment.myEnvironment = this;
		//when first started, pause timer
		StaticVariables.pauseTime = false;
		//figure out the dimensions of the box based on the volume
		width = Mathf.Pow (volume, (1.0f / 3.0f));
		height = Mathf.Pow (volume, (1.0f / 3.0f));
		depth = Mathf.Pow (volume, (1.0f / 3.0f));
	}
	void CreatePlanes(){
		//create the bottom plane
		Quaternion bottonPlaneRotation = Quaternion.Euler (0.0f, 0.0f, 0.0f);
		Vector3 bottomPlanePos = new Vector3 (centerPos.x, centerPos.y - (height/2.0f), centerPos.z);
		bottomPlane = Instantiate (plane, bottomPlanePos, bottonPlaneRotation) as GameObject;
		bottomPlane.transform.localScale = new Vector3 (width / 10.0f, height / 10.0f, depth / 10.0f);
		bottomPlane.name = "BottomPlane";
		bottomPlane.tag = "Plane";
		bottomPlane.GetComponent<Collider>().enabled = true;
		
		//create the top plane
		Quaternion topPlaneRotation = Quaternion.Euler (0.0f, 180.0f, 180.0f);
		Vector3 topPlanePos = new Vector3 (centerPos.x, centerPos.y + (height/2.0f), centerPos.z);
		topPlane = Instantiate (plane, topPlanePos, topPlaneRotation) as GameObject;
		topPlane.transform.localScale = new Vector3 (width / 10.0f, height / 10.0f, depth / 10.0f);
		topPlane.name = "TopPlane";
		topPlane.tag = "Plane";
		topPlane.GetComponent<Collider>().enabled = true;
		
		//create the back plane
		Quaternion backPlaneRotation = Quaternion.Euler (270.0f, 0.0f, 0.0f);
		Vector3 backPlanePos = new Vector3 (centerPos.x, centerPos.y, centerPos.z + (depth/2.0f));
		backPlane = Instantiate (plane, backPlanePos, backPlaneRotation) as GameObject;
		backPlane.transform.localScale = new Vector3 (width / 10.0f, depth / 10.0f, height / 10.0f);
		backPlane.name = "BackPlane";
		backPlane.tag = "Plane";
		backPlane.GetComponent<Collider>().enabled = true;
		
		//create the front plane
		Quaternion frontPlaneRotation = Quaternion.Euler (90.0f, 0.0f, 0.0f);
		Vector3 frontPlanePos = new Vector3 (centerPos.x, centerPos.y, centerPos.z - (depth/2.0f));
		frontPlane = Instantiate (plane, frontPlanePos, frontPlaneRotation) as GameObject;
		frontPlane.transform.localScale = new Vector3 (width / 10.0f, depth / 10.0f, height / 10.0f);
		frontPlane.name = "FrontPlane";
		frontPlane.tag = "Plane";
		frontPlane.GetComponent<Collider>().enabled = true;
		
		//create the right plane
		Quaternion rightPlaneRotation = Quaternion.Euler (0.0f, 0.0f, 90.0f);
		Vector3 rightPlanePos = new Vector3 (centerPos.x + (width/2.0f), centerPos.y, centerPos.z);
		rightPlane = Instantiate (plane, rightPlanePos, rightPlaneRotation) as GameObject;
		rightPlane.transform.localScale = new Vector3 (height / 10.0f, width / 10.0f, depth / 10.0f);
		rightPlane.name = "RightPlane";
		rightPlane.tag = "Plane";
		rightPlane.GetComponent<Collider>().enabled = true;
		
		//create the left plane
		Quaternion leftPlaneRotation = Quaternion.Euler (0.0f, 0.0f, 270.0f);
		Vector3 leftPlanePos = new Vector3 (centerPos.x - (width/2.0f), centerPos.y, centerPos.z);
		leftPlane = Instantiate (plane, leftPlanePos, leftPlaneRotation) as GameObject;
		leftPlane.transform.localScale = new Vector3 (height / 10.0f, width / 10.0f, depth / 10.0f);
		leftPlane.name = "LeftPlane";
		leftPlane.tag = "Plane";
		leftPlane.GetComponent<Collider>().enabled = true;
	}
	void Start () {
		// pre-compute coefficients used in various types of potentials so that we don't have to calculate them dynamically
		preCompute ();
		
		//create the atoms
		InitAtoms ();
		


		//give all of the atoms a random velocity on startup
		//atomTouchGUI.AllAtomsKick ();
	}
	
	void Update ()
	{
		
	}
	
	
	public void preCompute()
	{
		if (Potential.currentPotential == Potential.potentialType.LennardJones)
			Potential.myPotential = new LennardJones();
		else if(Potential.currentPotential == Potential.potentialType.Buckingham)
			Potential.myPotential = new BuckinghamGooey();
		
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

		CreateRandomConfiguration();

	}


	private void CreateRandomConfiguration()
	{
		//initialize the new atoms
		if (Potential.currentPotential == Potential.potentialType.LennardJones)
		{
			for (int i = 0; i < numMolecules; i++)
			{
				Debug.Log("HERE");
				createAtom (molecules [0]);
			}
		}
		else if (Potential.currentPotential == Potential.potentialType.Buckingham)
		{
			for (int i = 0; i < numMolecules/2; i++)
			{
				//sodium
				Debug.Log("HERE");
				createAtom (molecules [3]);
			}
			for (int i = numMolecules/2; i < numMolecules; i++)
			{
				//chlorine
				Debug.Log("HERE");
				createAtom (molecules [4]);
			}
		}
		//atomTouchGUI.AllAtomsKick();
	}
	
	// this method creates a new atom from the type of the preFab and checks the position to have a far enough distance from other atoms
	public void createAtom(Rigidbody preFab)
	{
		int preFabID = preFab.GetInstanceID();

		Environment myEnvironment = Environment.myEnvironment;
		Quaternion curRotation = Quaternion.Euler(0, 0, 0);
		preFab.gameObject.GetComponent<MeshRenderer>().enabled = SettingsControl.renderAtoms;

		Instantiate(preFab, myEnvironment.centerPos, curRotation);

		int i = Atom.AllAtoms.Count-1;
		Atom currAtom = Atom.AllAtoms[i];
		currAtom.gameObject.name = currAtom.GetInstanceID().ToString();
		//Debug.Log(currAtom.GetInstanceID());
		currAtom.GetComponent<Rigidbody>().freezeRotation = true;
		//currAtom.GetComponent<TrailRenderer>().enabled = SettingsControl.mySettings.trailsToggle.isOn;

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
		//kick it
		AtomKick(i);
		Potential.myPotential.calculateVerletRadius (currAtom);

		if ((tryNumber == 100) && (proximityFlag == false)) 
		{
			Atom.UnregisterAtom(currAtom);
			Destroy (currAtom.gameObject);
			Debug.Log ("No space for atoms!");
		}
		
	}
	//kick only one atom
	public void AtomKick(int i){
		Atom currAtom = Atom.AllAtoms[i];
		float xVelocity = 0.0f;
		float yVelocity = 0.0f;
		float zVelocity = 0.0f;
		//this is maximum random velocity.
		float maxVelocity = 2.0f*Mathf.Sqrt(3.0f*StaticVariables.kB*StaticVariables.desiredTemperature/currAtom.massamu/StaticVariables.amuToKg)/StaticVariables.angstromsToMeters;

		if(UnityEngine.Random.Range(0.0f, 1.0f) > .5f){
			xVelocity = UnityEngine.Random.Range(1.0f * maxVelocity, 5.0f * maxVelocity);
		}
		else{
			xVelocity = UnityEngine.Random.Range(-5.0f * maxVelocity, -1.0f * maxVelocity);
		}
		if(UnityEngine.Random.Range(0.0f, 1.0f) > .5f){
			yVelocity = UnityEngine.Random.Range(1.0f * maxVelocity, 5.0f * maxVelocity);
		}
		else{
			yVelocity = UnityEngine.Random.Range(-5.0f * maxVelocity, -1.0f * maxVelocity);
		}
		if(UnityEngine.Random.Range(0.0f, 1.0f) > .5f){
			zVelocity = UnityEngine.Random.Range(1.0f * maxVelocity, 5.0f * maxVelocity);
		}
		else{
			zVelocity = UnityEngine.Random.Range(-5.0f * maxVelocity, -1.0f * maxVelocity);
		}
		currAtom.velocity = new Vector3(xVelocity, yVelocity, zVelocity);
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
