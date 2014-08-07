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
	
		centerPos = new Vector3 (-5.0f, 0.0f, 0.0f);
		StaticVariables.sigmaValues = new Dictionary<String, float> ();

		for (int i = 0; i < molecules.Count; i++) {
			Atom atomScript = molecules[i].GetComponent<Atom>();
			StaticVariables.sigmaValues.Add(atomScript.atomName+atomScript.atomName, atomScript.sigma);
		}

		for (int i = 0; i < molecules.Count; i++) {
			Atom firstAtomScript = molecules[i].GetComponent<Atom>();
			for(int j = i+1; j < molecules.Count; j++){
				Atom secondAtomScript = molecules[j].GetComponent<Atom>();
				StaticVariables.sigmaValues.Add(firstAtomScript.atomName+secondAtomScript.atomName, Mathf.Sqrt(firstAtomScript.sigma+secondAtomScript.sigma));
				StaticVariables.sigmaValues.Add(secondAtomScript.atomName+firstAtomScript.atomName, Mathf.Sqrt(firstAtomScript.sigma+secondAtomScript.sigma));
			}
		}

		initialCenterPos = centerPos;
		CameraScript cameraScript = Camera.main.GetComponent<CameraScript> ();

		//create the atoms
		for (int i = 0; i < numMolecules; i++) {
			Vector3 position = new Vector3(centerPos.x + (UnityEngine.Random.Range(-(width/2.0f) + errorBuffer, (width/2.0f) - errorBuffer)), centerPos.y + (UnityEngine.Random.Range(-(height/2.0f) + errorBuffer, (height/2.0f) - errorBuffer)), centerPos.z + (UnityEngine.Random.Range(-(depth/2.0f) + errorBuffer, (depth/2.0f) - errorBuffer)));
			Quaternion rotation = Quaternion.Euler(0, 0, 0);
			Instantiate(molecules[moleculeToSpawn].rigidbody, position, rotation);
		}

		width = (float)Math.Pow (volume, (1.0f / 3.0f));
		height = (float)Math.Pow (volume, (1.0f / 3.0f));
		depth = (float)Math.Pow (volume, (1.0f / 3.0f));
		
		//create the box
		Quaternion bottonPlaneRotation = Quaternion.Euler (0.0f, 0.0f, 0.0f);
		Vector3 bottomPlanePos = new Vector3 (centerPos.x, centerPos.y - (height/2.0f), centerPos.z);
		bottomPlane = Instantiate (plane, bottomPlanePos, bottonPlaneRotation) as GameObject;
		bottomPlane.transform.localScale = new Vector3 (width / 10.0f, height / 10.0f, depth / 10.0f);
		bottomPlane.name = "BottomPlane";
		bottomPlane.tag = "Plane";

		Quaternion topPlaneRotation = Quaternion.Euler (0.0f, 180.0f, 180.0f);
		Vector3 topPlanePos = new Vector3 (centerPos.x, centerPos.y + (height/2.0f), centerPos.z);
		topPlane = Instantiate (plane, topPlanePos, topPlaneRotation) as GameObject;
		topPlane.transform.localScale = new Vector3 (width / 10.0f, height / 10.0f, depth / 10.0f);
		topPlane.name = "TopPlane";
		topPlane.tag = "Plane";
		
		Quaternion backPlaneRotation = Quaternion.Euler (270.0f, 0.0f, 0.0f);
		Vector3 backPlanePos = new Vector3 (centerPos.x, centerPos.y, centerPos.z + (depth/2.0f));
		backPlane = Instantiate (plane, backPlanePos, backPlaneRotation) as GameObject;
		backPlane.transform.localScale = new Vector3 (width / 10.0f, depth / 10.0f, height / 10.0f);
		backPlane.name = "BackPlane";
		backPlane.tag = "Plane";
		
		Quaternion frontPlaneRotation = Quaternion.Euler (90.0f, 0.0f, 0.0f);
		Vector3 frontPlanePos = new Vector3 (centerPos.x, centerPos.y, centerPos.z - (depth/2.0f));
		frontPlane = Instantiate (plane, frontPlanePos, frontPlaneRotation) as GameObject;
		frontPlane.transform.localScale = new Vector3 (width / 10.0f, depth / 10.0f, height / 10.0f);
		frontPlane.name = "FrontPlane";
		frontPlane.tag = "Plane";
		
		Quaternion rightPlaneRotation = Quaternion.Euler (0.0f, 0.0f, 90.0f);
		Vector3 rightPlanePos = new Vector3 (centerPos.x + (width/2.0f), centerPos.y, centerPos.z);
		rightPlane = Instantiate (plane, rightPlanePos, rightPlaneRotation) as GameObject;
		rightPlane.transform.localScale = new Vector3 (height / 10.0f, width / 10.0f, depth / 10.0f);
		rightPlane.name = "RightPlane";
		rightPlane.tag = "Plane";
		
		Quaternion leftPlaneRotation = Quaternion.Euler (0.0f, 0.0f, 270.0f);
		Vector3 leftPlanePos = new Vector3 (centerPos.x - (width/2.0f), centerPos.y, centerPos.z);
		leftPlane = Instantiate (plane, leftPlanePos, leftPlaneRotation) as GameObject;
		leftPlane.transform.localScale = new Vector3 (height / 10.0f, width / 10.0f, depth / 10.0f);
		leftPlane.name = "LeftPlane";
		leftPlane.tag = "Plane";
		
		//name the atoms
		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		for (int i = 0; i < allMolecules.Length; i++) {
			GameObject currAtom = allMolecules[i];
			currAtom.name = i.ToString();
			//currAtom.rigidbody.AddForce(new Vector3(0.0f, 5.0f, 0.0f), ForceMode.Impulse);
		}

		//create the lines and the labels
		bottomText = Instantiate(textMeshPrefab, new Vector3(bottomPlanePos.x - 2.0f, bottomPlanePos.y - 1.0f, bottomPlanePos.z - (depth/2.0f)), Quaternion.identity) as TextMesh;
		bottomText.text = width.ToString() + " Angstroms";
		LineRenderer bottomLine = bottomText.transform.gameObject.AddComponent<LineRenderer> ();
		bottomLine.material = mat;
		bottomLine.SetColors(Color.yellow, Color.yellow);
		bottomLine.SetWidth(0.2F, 0.2F);
		bottomLine.SetVertexCount(2);
		
		sideText = Instantiate(textMeshPrefab, new Vector3(bottomPlanePos.x + (width/2.0f) + 1.0f, bottomPlanePos.y + (height/2.0f), bottomPlanePos.z - (depth/2.0f)), Quaternion.identity) as TextMesh;
		sideText.text = height.ToString() + " Angstroms";
		LineRenderer sideLine = sideText.transform.gameObject.AddComponent<LineRenderer> ();
		sideLine.material = mat;
		sideLine.SetColors(Color.yellow, Color.yellow);
		sideLine.SetWidth(0.2F, 0.2F);
		sideLine.SetVertexCount(2);


		depthText = Instantiate(textMeshPrefab, new Vector3(centerPos.x + (width/2.0f), bottomPlanePos.y - 1.0f, centerPos.z - 2.0f), Quaternion.Euler(0.0f, -90.0f, 0.0f)) as TextMesh;
		depthText.text = depth.ToString() + " Angstroms";
		LineRenderer depthLine = depthText.transform.gameObject.AddComponent<LineRenderer> ();
		depthLine.material = mat;
		depthLine.SetColors(Color.yellow, Color.yellow);
		depthLine.SetWidth(0.2F, 0.2F);
		depthLine.SetVertexCount(2);
	}

	void Update () {

		width = (float)Math.Pow (volume, (1.0f / 3.0f));
		height = (float)Math.Pow (volume, (1.0f / 3.0f));
		depth = (float)Math.Pow (volume, (1.0f / 3.0f));
		
		CameraScript cameraScript = Camera.main.GetComponent<CameraScript> ();

		LineRenderer bottomLine = bottomText.GetComponent<LineRenderer> ();
		bottomLine.SetPosition(0, new Vector3(bottomPlane.transform.position.x - (width/2.0f), bottomText.transform.position.y + 0.5f, bottomText.transform.position.z));
		bottomLine.SetPosition(1, new Vector3(bottomPlane.transform.position.x + (width/2.0f), bottomText.transform.position.y + 0.5f, bottomText.transform.position.z));
		
		LineRenderer sideLine = sideText.GetComponent<LineRenderer> ();
		sideLine.SetPosition (0, new Vector3 (bottomPlane.transform.position.x + (width/2.0f) + .5f, bottomPlane.transform.position.y, sideText.transform.position.z));
		sideLine.SetPosition (1, new Vector3 (bottomPlane.transform.position.x + (width/2.0f) + .5f, bottomPlane.transform.position.y + height, sideText.transform.position.z));

		LineRenderer depthLine = depthText.GetComponent<LineRenderer> ();
		depthLine.SetPosition(0, new Vector3(bottomPlane.transform.position.x + (width/2.0f), depthText.transform.position.y + .5f, bottomPlane.transform.position.z - (depth/2.0f)));
		depthLine.SetPosition(1, new Vector3(bottomPlane.transform.position.x + (width/2.0f), depthText.transform.position.y + .5f, bottomPlane.transform.position.z + (depth/2.0f)));

		bottomText.text = width.ToString() + " Angstroms";
		sideText.text = height.ToString() + " Angstroms";
		depthText.text = depth.ToString() + " Angstroms";
		sideText.transform.position = new Vector3 (bottomPlane.transform.position.x + (width / 2.0f) + 1.0f, bottomPlane.transform.position.y + (height / 2.0f), bottomPlane.transform.position.z - (depth / 2.0f));
		depthText.transform.position = new Vector3 (bottomPlane.transform.position.x + (width / 2.0f), bottomPlane.transform.position.y - 1.0f, bottomPlane.transform.position.z - 2.0f);
		bottomText.transform.position = new Vector3 (bottomPlane.transform.position.x - 2.0f, bottomPlane.transform.position.y - 1.0f, bottomPlane.transform.position.z - (depth / 2.0f));

		rightPlane.transform.position = new Vector3 (initialCenterPos.x + (width/2.0f), initialCenterPos.y, initialCenterPos.z);
		leftPlane.transform.position = new Vector3 (initialCenterPos.x - (width/2.0f), initialCenterPos.y, initialCenterPos.z);
		bottomPlane.transform.position = new Vector3 (initialCenterPos.x, initialCenterPos.y - (height/2.0f), initialCenterPos.z);
		topPlane.transform.position = new Vector3 (initialCenterPos.x, initialCenterPos.y + (height/2.0f), initialCenterPos.z);
		backPlane.transform.position = new Vector3 (initialCenterPos.x, initialCenterPos.y, initialCenterPos.z + (depth/2.0f));
		frontPlane.transform.position = new Vector3 (initialCenterPos.x, initialCenterPos.y, initialCenterPos.z - (depth/2.0f));
		
		bottomPlane.transform.localScale = new Vector3 (width / 10.0f, height / 10.0f, depth / 10.0f);
		topPlane.transform.localScale = new Vector3 (width / 10.0f, height / 10.0f, depth / 10.0f);
		backPlane.transform.localScale = new Vector3 (width / 10.0f, depth / 10.0f, height / 10.0f);
		frontPlane.transform.localScale = new Vector3 (width / 10.0f, depth / 10.0f, height / 10.0f);
		rightPlane.transform.localScale = new Vector3 (height / 10.0f, width / 10.0f, depth / 10.0f);
		leftPlane.transform.localScale = new Vector3 (height / 10.0f, width / 10.0f, depth / 10.0f);

	}
}
