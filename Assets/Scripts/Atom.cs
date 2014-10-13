/**
 * Class: Atom.cs
 * Created by: Justin Moeller
 * Description: This is the main class for dealing with anything related to the atoms.
 * The atoms' forces are calculated in FixedUpdate, then their velocities are scaled based
 * on the temperature of the system. FixedUpdate is currently called every .0005 seconds,
 * which is Time.fixedDeltaTime. In the FixedUpdate function, it is clearly marked where the
 * code for the new potentials should go. The rest of the class handles the user interactions with 
 * the atoms, such as double tapping or moving an atom. Because OnMouseDown, OnMouseDrag, and
 * OnMouseUp are not supported for iOS, the code that handles dragging an atom had to be implemented
 * twice, once for iOS and once for PC. This class is the base class for copper, gold, and platinum
 * and every atom runs this script. The abstract variables and functions that are declared must be 
 * defined by the children of this class because they are probably different per child. (i.e sigma and epsilon)
 * The collision detection for each atom is NOT handled by Unity, but rather in the Update function.
 * It simply checks if the atom is within the box, and if its not, it reverses its velocity to go back
 * inside the box. Transparency and selection are handled by passing the call to their child, then
 * changing the material of the atom.
 * 
 * 
 **/

//L-J potentials from Zhen and Davies, Phys. Stat. Sol. a, 78, 595 (1983)
//Symbol, epsilon/k_Boltzmann (K) n-m version, 12-6 version, sigma (Angstroms),
//     mass in amu, mass in (20 amu) for Unity 
//     FCC lattice parameter in Angstroms, expected NN bond (Angs)
//Au: 4683.0, 5152.9, 2.6367, 196.967, 9.848, 4.080, 2.88
//Cu: 3401.1, 4733.5, 2.3374,  63.546, 3.177, 3.610, 2.55
//Pt: 7184.2, 7908.7, 2.5394, 165.084, 8.254, 3.920, 2.77

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class Atom : MonoBehaviour
{
	//variables for computing where the atom goes when touched
	private Vector3 offset;
	private Vector3 screenPoint;
	private Vector3 lastMousePosition;
	private Vector3 lastTouchPosition;
	private float deltaTouch2 = 0.0f;
	private bool moveZDirection = false;
	private float lastTapTime;
	private float tapTime = .35f;
	[HideInInspector]public bool selected = false;
	[HideInInspector]public bool doubleTapped = false;
	//these dictionaries are used when moving groups of atoms. The key is the atom's name: (i.e "0" or "1")
	private Dictionary<String, Vector3> gameObjectOffsets;
	private Dictionary<String, Vector3> gameObjectScreenPoints;
	private float dragStartTime;
	private bool dragCalled;
	//dictionary for holding the TextMeshes of distances between atoms
	private Dictionary<String, TextMesh> bondDistanceText;

	protected static List<Atom> m_AllMolecules = new List<Atom> ();

	public TextMesh textMeshPrefab;
	public bool held { get; set; }

	//variables that must be implemented because they are declared as abstract in the base class
	public abstract float epsilon{ get; } // J
	public abstract float sigma { get; }
	protected abstract float massamu{ get; } //amu
	public abstract void SetSelected (bool selected);
	public abstract void SetTransparent (bool transparent);
	public abstract String atomName { get; }
	public abstract int atomID { get;}

	public abstract float buck_A { get; } // Buckingham potential coefficient
	public abstract float buck_B { get; } // Buckingham potential coefficient
	public abstract float buck_C { get; } // Buckingham potential coefficient
	public abstract float buck_D { get; } // Buckingham potential coefficient
	public abstract float Q_eff { get; } // Ion effective charge for use in Buckingham potential


	//variables for computing the forces on atoms
	private Vector3 lastVelocity = Vector3.zero;
	private Vector3 a_n = Vector3.zero;
	private Vector3 a_nplus1 = Vector3.zero;

	void Awake(){
		RegisterAtom (this);

		gameObject.rigidbody.velocity = new Vector3 (UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f));
		bondDistanceText = new Dictionary<String, TextMesh> ();
	}

	void OnDestroy(){
		UnregisterAtom (this);
	}


	// method to extract the list of allMolecules
	public static List<Atom> AllMolecules { 
		get {
			return m_AllMolecules;
		}
	}

	// method to register an added atom to the list of allMolecules
	protected static void RegisterAtom( Atom atom ) {
		m_AllMolecules.Add (atom);
	}

	// method to unregister a removed atom from the list of allMolecules
	protected static void UnregisterAtom( Atom atom ) { 
		m_AllMolecules.Remove( atom );
	}


	void FixedUpdate(){

		if (!StaticVariables.pauseTime) {
			Vector3 force = Vector3.zero;

			if(StaticVariables.currentPotential == StaticVariables.Potential.LennardJones){
				force = GetLennardJonesForce (m_AllMolecules);
			}
			else if(StaticVariables.currentPotential == StaticVariables.Potential.Brenner){
				force = GetLennardJonesForce (m_AllMolecules);
			}
			else{
				force = GetBuckinghamForce (m_AllMolecules);
			}

			//zero out any angular velocity
			if(!gameObject.rigidbody.isKinematic) gameObject.rigidbody.angularVelocity = Vector3.zero;
			gameObject.rigidbody.AddForce (force, mode:ForceMode.Force);

			//scale the velocity based on the temperature of the system
			if ((rigidbody.velocity.magnitude != 0) && !rigidbody.isKinematic && !float.IsInfinity(TemperatureCalc.squareRootAlpha) && m_AllMolecules.Count > 1) {
				Vector3 newVelocity = gameObject.rigidbody.velocity * TemperatureCalc.squareRootAlpha;
				gameObject.rigidbody.velocity = newVelocity;
			}

		}
		else{
			//zero out all of the velocities of all of the atoms when time is stopped
			for(int i = 0; i < m_AllMolecules.Count; i++){
				Atom currAtom = m_AllMolecules[i];
				if(!currAtom.rigidbody.isKinematic){
					currAtom.rigidbody.velocity = Vector3.zero;
				}
			}
		}



	}


	//the function returns the Lennard-Jones force on the atom given the list of all the atoms in the simulation
	Vector3 GetLennardJonesForce(List<Atom> objectsInRange){
		Vector3 finalForce = Vector3.zero;

		for (int i = 0; i < objectsInRange.Count; i++) {
			Vector3 deltaR = transform.position - objectsInRange [i].transform.position;
			float distanceSqr = deltaR.sqrMagnitude;

			//only get the forces of the atoms that are within the cutoff range
			if (objectsInRange[i].gameObject != gameObject && (distanceSqr < StaticVariables.cutoffSqr)) {

				float finalSigma = StaticVariables.sigmaValues[atomID * objectsInRange[i].atomID];

				int iR = (int) ((Mathf.Sqrt(distanceSqr)/finalSigma)/(StaticVariables.deltaR/StaticVariables.sigmaValueMax))+2;
				float magnitude = StaticVariables.preLennardJones[iR];
				magnitude = magnitude * 48.0f * epsilon / StaticVariables.angstromsToMeters/ finalSigma / finalSigma;
				finalForce += deltaR * magnitude;
			}
		}
		
		Vector3 adjustedForce = finalForce / StaticVariables.mass100amuToKg;
		adjustedForce = adjustedForce / StaticVariables.angstromsToMeters;
		adjustedForce = adjustedForce * StaticVariables.fixedUpdateIntervalToRealTime * StaticVariables.fixedUpdateIntervalToRealTime;
		return adjustedForce;
	}

	//the function returns the Buckingham force on the atom given the list of all the atoms in the simulation
	Vector3 GetBuckinghamForce(List<Atom> objectsInRange){
		Vector3 finalForce = Vector3.zero;
		
		for (int i = 0; i < objectsInRange.Count; i++) {
			Vector3 deltaR = transform.position - objectsInRange [i].transform.position;
			float distanceSqr = deltaR.sqrMagnitude;
			float magnitude = 0.0f;
			
			//only get the forces of the atoms that are within the cutoff range
			if ((objectsInRange[i].gameObject != gameObject) && (distanceSqr < StaticVariables.cutoffSqr)) {
				float distance = Mathf.Sqrt(distanceSqr);

				float final_A = StaticVariables.coeff_A [atomName + objectsInRange[i].atomName];
				float final_B = StaticVariables.coeff_B [atomName + objectsInRange[i].atomName];
				float final_C = StaticVariables.coeff_C [atomName + objectsInRange[i].atomName];
				float final_D = StaticVariables.coeff_D [atomName + objectsInRange[i].atomName];

				magnitude = final_A * final_B * Mathf.Exp (-final_B * distance) / distance;
				magnitude = magnitude - 6.0f * final_C / Mathf.Pow (distanceSqr, 4);
				magnitude = magnitude - 8.0f * final_D / Mathf.Pow (distanceSqr, 5);
				magnitude = magnitude / StaticVariables.angstromsToMeters;
				magnitude = magnitude + Q_eff * objectsInRange[i].Q_eff / (4.0f * Mathf.PI * StaticVariables.epsilon0 * distanceSqr * distance * StaticVariables.angstromsToMeters * StaticVariables.angstromsToMeters);

				finalForce += deltaR * magnitude;
			}
								
		}
		
		Vector3 adjustedForce = finalForce / StaticVariables.mass100amuToKg;
		adjustedForce = adjustedForce / StaticVariables.angstromsToMeters;
		adjustedForce = adjustedForce * StaticVariables.fixedUpdateIntervalToRealTime * StaticVariables.fixedUpdateIntervalToRealTime;
		return adjustedForce;
	}

	//this function takes care of double tapping, collision detection, and detecting OnMouseDown, OnMouseDrag, and OnMouseUp on iOS
	void Update(){

		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			if(Input.touchCount > 0){
				Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
				RaycastHit hitInfo;
				if(!held && Physics.Raycast(ray, out hitInfo) && hitInfo.transform.gameObject.tag == "Molecule" && hitInfo.transform.gameObject == gameObject){
					if(Input.GetTouch(0).phase == TouchPhase.Began){
						if((Time.realtimeSinceStartup - lastTapTime) < tapTime){
							//user double tapped an atom on iOS
							AtomTouchGUI atomTouchGUI = Camera.main.GetComponent<AtomTouchGUI>();
							atomTouchGUI.SetDoubleClicked();
							ResetDoubleTapped();
							doubleTapped = true;
							RemoveAllBondText();
							AtomTouchGUI.currentTimeSpeed = StaticVariables.TimeSpeed.SlowMotion;
						}
						//user touch an atom at this point
						OnMouseDownIOS();
						lastTapTime = Time.realtimeSinceStartup;
					}
				}
				else if(held){
					if(Input.GetTouch(0).phase == TouchPhase.Moved && Input.touchCount == 1){
						//user is now dragging an atom
						OnMouseDragIOS();
					}
					else if(Input.touchCount == 2){
						//handle z axis movement
						HandleZAxisTouch();
					}
					else if(Input.GetTouch(0).phase == TouchPhase.Canceled || Input.GetTouch(0).phase == TouchPhase.Ended){
						//user let go of the atom
						OnMouseUpIOS();
					}
					lastTouchPosition = Input.GetTouch(0).position;
				}
			}
		}
		else{
			if(Input.GetMouseButtonDown(0)){
				if((Time.realtimeSinceStartup - lastTapTime) < tapTime){
					//user double tapped an atom on PC
					AtomTouchGUI atomTouchGUI = Camera.main.GetComponent<AtomTouchGUI>();
					atomTouchGUI.SetDoubleClicked();
					ResetDoubleTapped();
					doubleTapped = true;
					RemoveAllBondText();
					AtomTouchGUI.currentTimeSpeed = StaticVariables.TimeSpeed.SlowMotion;
				}
				Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
				RaycastHit hitInfo;
				if (Physics.Raycast( ray, out hitInfo ) && hitInfo.transform.gameObject.tag == "Molecule" && hitInfo.transform.gameObject == gameObject){
					lastTapTime = Time.realtimeSinceStartup;
				}
			}
			
			HandleRightClick();
		}
		if (doubleTapped) {
			//this is what happens when double tapped is true
			CameraScript cameraScript = Camera.main.GetComponent<CameraScript>();
			cameraScript.setCameraCoordinates(transform);
			UpdateBondText();
			ApplyTransparency();
		}
		CheckVelocity ();
	}

	//another method for selecting atoms
	void HandleRightClick(){
		if (Input.GetMouseButtonDown (1)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if(Physics.Raycast(ray, out hitInfo) && hitInfo.transform.gameObject.tag == "Molecule" && hitInfo.transform.gameObject == gameObject){
				selected = !selected;
				SetSelected(selected);
			}
		}
	}

	//this function gives the user the ability to control the z-axis of the atom on iOS
	void HandleZAxisTouch(){
		if(Input.touchCount == 2){
			Touch touch2 = Input.GetTouch(1);
			if(touch2.phase == TouchPhase.Began){
				moveZDirection = true;
			}
			else if(touch2.phase == TouchPhase.Moved){
				if(!selected){
					//this is for one atom
					Vector2 touchOnePrevPos = touch2.position - touch2.deltaPosition;
					float deltaMagnitudeDiff = touch2.position.y - touchOnePrevPos.y;
					deltaTouch2 = deltaMagnitudeDiff / 10.0f;
					Quaternion cameraRotation = Camera.main.transform.rotation;
					Vector3 projectPosition = transform.position;
					projectPosition += (cameraRotation * new Vector3(0.0f, 0.0f, deltaTouch2));
					transform.position = CheckPosition(projectPosition);
					screenPoint += new Vector3(0.0f, 0.0f, deltaTouch2);
				}
				else{
					//this is for a group of atoms
					Vector2 touchOnePrevPos = touch2.position - touch2.deltaPosition;
					float deltaMagnitudeDiff = touch2.position.y - touchOnePrevPos.y;
					deltaTouch2 = deltaMagnitudeDiff / 10.0f;
					GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
					Dictionary<String, Vector3> newAtomPositions = new Dictionary<String, Vector3>();
					bool moveAtoms = true;
					for(int i = 0; i < allMolecules.Length; i++){
						GameObject currAtom = allMolecules[i];
						Atom atomScript = currAtom.GetComponent<Atom>();
						if(!atomScript.selected) continue;
						Quaternion cameraRotation = Camera.main.transform.rotation;
						Vector3 projectPosition = currAtom.transform.position;
						projectPosition += (cameraRotation * new Vector3(0.0f, 0.0f, deltaTouch2));
						Vector3 newAtomPosition = CheckPosition(projectPosition);
						if(newAtomPosition != projectPosition){
							moveAtoms = false;
						}
						if(gameObjectScreenPoints != null){
							gameObjectScreenPoints[currAtom.name] += new Vector3(0.0f, 0.0f, deltaTouch2);
						}
						newAtomPositions.Add(currAtom.name, newAtomPosition);
					}

					if(newAtomPositions.Count > 0 && moveAtoms){
						for(int i = 0; i < allMolecules.Length; i++){
							GameObject currAtom = allMolecules[i];
							Atom atomScript = currAtom.GetComponent<Atom>();
							if(!atomScript.selected) continue;
							Vector3 newAtomPosition = newAtomPositions[currAtom.name];
							currAtom.transform.position = newAtomPosition;
						}
					}
				}
			}
		}
		else if(Input.touchCount == 0 && moveZDirection){
			//this resets the neccesary variables so the atom can move in two dimensions again. It also resets the atom's material
			moveZDirection = false;
			held = false;
			GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
			for(int i = 0; i < allMolecules.Length; i++){
				GameObject currAtom = allMolecules[i];
				Atom atomScript = currAtom.GetComponent<Atom>();
				atomScript.SetSelected(atomScript.selected);
			}
			
		}
	}

	//reset all of the atoms double tapped to false
	void ResetDoubleTapped(){
		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		for (int i = 0; i < allMolecules.Length; i++) {
			Atom atomScript = allMolecules[i].GetComponent<Atom>();
			atomScript.doubleTapped = false;
		}
	}
		
	//this is the equivalent of OnMouseDown, but for iOS
	void OnMouseDownIOS(){
		dragStartTime = Time.realtimeSinceStartup;
		dragCalled = false;
		held = true;
		if (!selected) {
			//this is for one atom
			screenPoint = Camera.main.WorldToScreenPoint(transform.position);
			//the -15.0f here is for moving the atom above your finger
			offset = transform.position - Camera.main.ScreenToWorldPoint(
				new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y - 15.0f, screenPoint.z));
		}
		else{
			//this is for a group of atoms
			GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
			gameObjectOffsets = new Dictionary<String, Vector3>();
			gameObjectScreenPoints = new Dictionary<String, Vector3>();
			for(int i = 0; i < allMolecules.Length; i++){
				GameObject currAtom = allMolecules[i];
				Atom atomScript = currAtom.GetComponent<Atom>();
				if(atomScript.selected){
					currAtom.rigidbody.isKinematic = true;
					Vector3 pointOnScreen = Camera.main.WorldToScreenPoint(currAtom.transform.position);
					//the -15.0f here is for moving the atom above your finger
					Vector3 atomOffset = currAtom.transform.position - Camera.main.ScreenToWorldPoint(
						new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y - 15.0f, pointOnScreen.z));
					atomScript.held = true;
					gameObjectOffsets.Add(currAtom.name, atomOffset);
					gameObjectScreenPoints.Add(currAtom.name, pointOnScreen);
				}
			}
		}
	}
	
	//controls for debugging on pc
	void OnMouseDown (){
		if (Application.platform != RuntimePlatform.IPhonePlayer) {
			dragStartTime = Time.realtimeSinceStartup;
			dragCalled = false;
			held = true;

			if(!selected){
				//this is for one atom
				screenPoint = Camera.main.WorldToScreenPoint(transform.position);
				//the -15.0 here is for moving the atom above your mouse
				offset = transform.position - Camera.main.ScreenToWorldPoint(
					new Vector3(Input.mousePosition.x, Input.mousePosition.y - 15.0f, screenPoint.z));

			}
			else{
				//this is for a group of atoms
				GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
				gameObjectOffsets = new Dictionary<String, Vector3>();
				gameObjectScreenPoints = new Dictionary<String, Vector3>();
				for(int i = 0; i < allMolecules.Length; i++){
					GameObject currAtom = allMolecules[i];
					Atom atomScript = currAtom.GetComponent<Atom>();
					if(atomScript.selected){
						currAtom.rigidbody.isKinematic = true;
						Vector3 pointOnScreen = Camera.main.WorldToScreenPoint(currAtom.transform.position);
						//the -15.0 here is for moving the atom above your mouse
						Vector3 atomOffset = currAtom.transform.position - Camera.main.ScreenToWorldPoint(
							new Vector3(Input.mousePosition.x, Input.mousePosition.y - 15.0f, pointOnScreen.z));
						atomScript.held = true;
						gameObjectOffsets.Add(currAtom.name, atomOffset);
						gameObjectScreenPoints.Add(currAtom.name, pointOnScreen);
					}
				}
			}
		}
	}

	//this is the equivalent of OnMouseDrag for iOS
	void OnMouseDragIOS(){
		if (Time.realtimeSinceStartup - dragStartTime > 0.1f) {
			dragCalled = true;
			Quaternion cameraRotation = Camera.main.transform.rotation;
			ApplyTransparency();
			rigidbody.isKinematic = true;
			if(!selected){
				//this is for one atom
				Vector3 diffVector = new Vector3(lastTouchPosition.x, lastTouchPosition.y) - new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
				if(diffVector.magnitude > 0 && !doubleTapped && Input.touchCount == 1){
					Vector3 curScreenPoint = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, screenPoint.z);
					Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
					curPosition = CheckPosition(curPosition);
					transform.position = curPosition;
				}
			}
			else{
				//this is for a group of atoms
				GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
				bool noneDoubleTapped = true;
				for(int i = 0; i < allMolecules.Length; i++){
					GameObject currAtom = allMolecules[i];
					Atom atomScript = currAtom.GetComponent<Atom>();
					if(atomScript.doubleTapped && atomScript.selected) noneDoubleTapped = false;
				}

				//only move the atoms if none of them have been double tapped
				if(noneDoubleTapped){
					List<Vector3> atomPositions = new List<Vector3>();
					bool moveAtoms = true;
					for(int i = 0; i < allMolecules.Length; i++){
						GameObject currAtom = allMolecules[i];
						Atom atomScript = currAtom.GetComponent<Atom>();
						Vector3 newAtomPosition = currAtom.transform.position;
						Vector3 diffVector = new Vector3(lastTouchPosition.x, lastTouchPosition.y) - new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
						if(diffVector.magnitude > 0 && !doubleTapped && atomScript.selected && Input.touchCount == 1){
							if(gameObjectOffsets != null && gameObjectScreenPoints != null){
								Vector3 currScreenPoint = gameObjectScreenPoints[currAtom.name];
								Vector3 currOffset = gameObjectOffsets[currAtom.name];
								Vector3 objScreenPoint = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, currScreenPoint.z);
								Vector3 curPosition = Camera.main.ScreenToWorldPoint(objScreenPoint) + currOffset;
								newAtomPosition = CheckPosition(curPosition);
								if(newAtomPosition != curPosition){
									moveAtoms = false;
								}
							}
						}
						Vector3 finalPosition = newAtomPosition;
						atomPositions.Add(finalPosition);
					}
					//only move the atoms if none of them have hit the wall of the box
					if(atomPositions.Count > 0 && moveAtoms){
						for(int i = 0; i < allMolecules.Length; i++){
							Vector3 newAtomPosition = atomPositions[i];
							GameObject currAtom = allMolecules[i];
							currAtom.transform.position = newAtomPosition;
						}
					}
				}
			}
		}
	}
	
	void OnMouseDrag(){
		if (Application.platform != RuntimePlatform.IPhonePlayer) {

			if(Time.realtimeSinceStartup - dragStartTime > 0.1f){
				dragCalled = true;
				Quaternion cameraRotation = Camera.main.transform.rotation;
				ApplyTransparency();
				rigidbody.isKinematic = true;

				if(!selected){
					//this is for one atom
					if((lastMousePosition - Input.mousePosition).magnitude > 0 && !doubleTapped){
						Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
						Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
						curPosition = CheckPosition(curPosition);
						transform.position = curPosition;
					}

					//this is the implementation of moving the atom in the z-direction
					float deltaZ = -Input.GetAxis("Mouse ScrollWheel");
					Vector3 projectPosition = transform.position;
					projectPosition += (cameraRotation * new Vector3(0.0f, 0.0f, deltaZ));
					transform.position = CheckPosition(projectPosition);
					screenPoint += new Vector3(0.0f, 0.0f, deltaZ);
				}
				else{
					//this is for a group of atoms
					GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
					bool noneDoubleTapped = true;
					for(int i = 0; i < allMolecules.Length; i++){
						GameObject currAtom = allMolecules[i];
						Atom atomScript = currAtom.GetComponent<Atom>();
						if(atomScript.doubleTapped && atomScript.selected) noneDoubleTapped = false;
					}

					//only move the atoms if none of them have been double tapped
					if(noneDoubleTapped){
						List<Vector3> atomPositions = new List<Vector3>();
						bool moveAtoms = true;
						for(int i = 0; i < allMolecules.Length; i++){
							GameObject currAtom = allMolecules[i];
							Atom atomScript = currAtom.GetComponent<Atom>();
							Vector3 newAtomPosition = currAtom.transform.position;
							if((lastMousePosition - Input.mousePosition).magnitude > 0 && atomScript.selected){
								Vector3 currScreenPoint = gameObjectScreenPoints[currAtom.name];
								Vector3 currOffset = gameObjectOffsets[currAtom.name];
								Vector3 objScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, currScreenPoint.z);
								Vector3 curPosition = Camera.main.ScreenToWorldPoint(objScreenPoint) + currOffset;
								newAtomPosition = CheckPosition(curPosition);
								if(newAtomPosition != curPosition){
									moveAtoms = false;
								}
								//currAtom.transform.position = newAtomPosition;
							}
							
							Vector3 finalPosition = newAtomPosition;
							
							if(atomScript.selected){
								float deltaZ = -Input.GetAxis("Mouse ScrollWheel");
								Vector3 projectPosition = newAtomPosition;
								projectPosition += (cameraRotation * new Vector3(0.0f, 0.0f, deltaZ));
								finalPosition = CheckPosition(projectPosition);
								gameObjectScreenPoints[currAtom.name] += new Vector3(0.0f, 0.0f, deltaZ);
								if(finalPosition != projectPosition){
									moveAtoms = false;
								}
							}
							atomPositions.Add(finalPosition);
						}

						//only move the atoms if none of them have hit the walls of the box
						if(atomPositions.Count > 0 && moveAtoms){
							for(int i = 0; i < allMolecules.Length; i++){
								Vector3 newAtomPosition = atomPositions[i];
								GameObject currAtom = allMolecules[i];
								currAtom.transform.position = newAtomPosition;
							}
						}
					}
				}
			}
			
			//always keep track of the last mouse position for the next frame for flinging atoms
			lastMousePosition = Input.mousePosition;
		}
		
	}

	//this function is the equivalent of OnMouseUp for iOS
	void OnMouseUpIOS(){
		if (!dragCalled) {
			//if the user only tapped the atom, this is executed
			selected = !selected;
			SetSelected(selected);
			rigidbody.isKinematic = false;
		}
		else{
			GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");

			if(!selected){
				//this is for one atom
				rigidbody.isKinematic = false;

				Quaternion cameraRotation = Camera.main.transform.rotation;
				Vector3 direction = cameraRotation * (new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0.0f) - new Vector3(lastTouchPosition.x, lastTouchPosition.y, 0.0f));
				float directionMagnitude = direction.magnitude;
				direction.Normalize();
				float magnitude = 2.0f * directionMagnitude;
				Vector3 flingVector = magnitude * new Vector3(direction.x, direction.y, 0.0f);
				gameObject.rigidbody.velocity = flingVector;
			}
			else{
				//this is for a group of atoms
				for(int i = 0; i < allMolecules.Length; i++){
					GameObject currAtom = allMolecules[i];
					Atom atomScript = currAtom.GetComponent<Atom>();
					if(atomScript.selected){
						currAtom.rigidbody.isKinematic = false;
						atomScript.held = false;

						Quaternion cameraRotation = Camera.main.transform.rotation;
						Vector3 direction = cameraRotation * (new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0.0f) - new Vector3(lastTouchPosition.x, lastTouchPosition.y, 0.0f));
						float directionMagnitude = direction.magnitude;
						direction.Normalize();
						float magnitude = 2.0f * directionMagnitude;
						Vector3 flingVector = magnitude * new Vector3(direction.x, direction.y, 0.0f);
						currAtom.rigidbody.velocity = flingVector;
					}
				}
			}

			//reset the selection status of all the atoms
			for(int i = 0; i < allMolecules.Length; i++){
				GameObject currAtom = allMolecules[i];
				Atom atomScript = currAtom.GetComponent<Atom>();
				atomScript.SetSelected(atomScript.selected);
			}

		}
		held = false;
	}
	
	void OnMouseUp (){
		if (Application.platform != RuntimePlatform.IPhonePlayer) {
			if(!dragCalled){
				//this is executed if an atom is only tapped
				selected = !selected;
				SetSelected(selected);
				rigidbody.isKinematic = false;
			}
			else{
				GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");

				if(!selected){
					//this is for one atom
					rigidbody.isKinematic = false;

					Quaternion cameraRotation = Camera.main.transform.rotation;
					Vector2 direction = cameraRotation * (Input.mousePosition - lastMousePosition);
					direction.Normalize();
					float magnitude = 10.0f;
					Vector3 flingVector = magnitude * new Vector3(direction.x, direction.y, 0.0f);
					gameObject.rigidbody.velocity = flingVector;
				}
				else{
					//this is for a group of atoms
					for(int i = 0; i < allMolecules.Length; i++){
						GameObject currAtom = allMolecules[i];
						Atom atomScript = currAtom.GetComponent<Atom>();
						if(atomScript.selected){
							currAtom.rigidbody.isKinematic = false;
							atomScript.held = false;

							Quaternion cameraRotation = Camera.main.transform.rotation;
							Vector3 direction = cameraRotation * (Input.mousePosition - lastMousePosition);
							direction.Normalize();
							float magnitude = 10.0f;
							Vector3 flingVector = magnitude * new Vector3(direction.x, direction.y, 0.0f);
							currAtom.rigidbody.velocity = flingVector;
						}
					}
				}

				//reset the selection status of all of the atoms
				for(int i = 0; i < allMolecules.Length; i++){
					GameObject currAtom = allMolecules[i];
					Atom atomScript = currAtom.GetComponent<Atom>();
					atomScript.SetSelected(atomScript.selected);
				}
			}
			held = false;
		}
	}

	//this functions returns the appropriate bond distance, given two atoms
	public float BondDistance(GameObject otherAtom){
		Atom otherAtomScript = otherAtom.GetComponent<Atom> ();
		return 1.225f * StaticVariables.sigmaValues [atomID*otherAtomScript.atomID];
	}

	//this functions checks the position of the atoms, and if its outside of the box, it reverses the atoms velocity to go back inside the box
	void CheckVelocity(){

		if (gameObject.rigidbody.isKinematic) return;

		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment> ();
		Vector3 bottomPlanePos = createEnvironment.bottomPlane.transform.position;
		Vector3 newVelocity = gameObject.rigidbody.velocity;
		if (gameObject.transform.position.x > bottomPlanePos.x + (createEnvironment.width / 2.0f) - createEnvironment.errorBuffer) {
			newVelocity.x = Math.Abs(newVelocity.x) * -1;
		}
		if (gameObject.transform.position.x < bottomPlanePos.x - (createEnvironment.width / 2.0f) + createEnvironment.errorBuffer) {
			newVelocity.x = Math.Abs(newVelocity.x);
		}
		if (gameObject.transform.position.y > bottomPlanePos.y + (createEnvironment.height) - createEnvironment.errorBuffer) {
			newVelocity.y = Math.Abs(newVelocity.y) * -1;
		}
		if (gameObject.transform.position.y < bottomPlanePos.y + createEnvironment.errorBuffer) {
			newVelocity.y = Math.Abs(newVelocity.y);
		}
		if (gameObject.transform.position.z > bottomPlanePos.z + (createEnvironment.depth / 2.0f) - createEnvironment.errorBuffer) {
			newVelocity.z = Math.Abs(newVelocity.z) * -1;
		}
		if (gameObject.transform.position.z < bottomPlanePos.z - (createEnvironment.depth / 2.0f) + createEnvironment.errorBuffer) {
			newVelocity.z = Math.Abs(newVelocity.z);
		}
		gameObject.rigidbody.velocity = newVelocity;
	}

	//this function checks the position of an atom, and if its outside of the box, simply place the atom back inside the box
	Vector3 CheckPosition(Vector3 position){
		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment> ();
		Vector3 bottomPlanePos = createEnvironment.bottomPlane.transform.position;
		if (position.y > bottomPlanePos.y + (createEnvironment.height) - createEnvironment.errorBuffer) {
			position.y = bottomPlanePos.y + (createEnvironment.height) - createEnvironment.errorBuffer;
		}
		if (position.y < bottomPlanePos.y + createEnvironment.errorBuffer) {
			position.y = bottomPlanePos.y + createEnvironment.errorBuffer;
		}
		if (position.x > bottomPlanePos.x + (createEnvironment.width/2.0f) - createEnvironment.errorBuffer) {
			position.x = bottomPlanePos.x + (createEnvironment.width/2.0f) - createEnvironment.errorBuffer;
		}
		if (position.x < bottomPlanePos.x - (createEnvironment.width/2.0f) + createEnvironment.errorBuffer) {
			position.x = bottomPlanePos.x - (createEnvironment.width/2.0f) + createEnvironment.errorBuffer;
		}
		if (position.z > bottomPlanePos.z + (createEnvironment.depth/2.0f) - createEnvironment.errorBuffer) {
			position.z = bottomPlanePos.z + (createEnvironment.depth/2.0f) - createEnvironment.errorBuffer;
		}
		if (position.z < bottomPlanePos.z - (createEnvironment.depth/2.0f) + createEnvironment.errorBuffer) {
			position.z = bottomPlanePos.z - (createEnvironment.depth/2.0f) + createEnvironment.errorBuffer;
		}
		return position;
	}

	//makes all atoms transparency except for the current atom and all atoms that are "close" to it
	void ApplyTransparency(){
		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		for (int i = 0; i < allMolecules.Length; i++) {
			GameObject neighbor = allMolecules[i];
			if(neighbor == gameObject) continue;
			Atom neighborScript = neighbor.GetComponent<Atom>();
			if(neighborScript.selected){
				neighborScript.SetSelected(neighborScript.selected);
			}
			else if(!neighborScript.selected && Vector3.Distance(gameObject.transform.position, neighbor.transform.position) > BondDistance(neighbor)){
				neighborScript.SetTransparent(true);
			}
			else{
				neighborScript.SetTransparent(false);
			}
		}
	}

	//resets all atoms transparency back to normal or to selected depending on its status
	public void ResetTransparency(){
		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		for (int i = 0; i < allMolecules.Length; i++) {
			GameObject currAtom = allMolecules[i];
			Atom atomScript = currAtom.GetComponent<Atom>();
			if(atomScript.selected){
				atomScript.SetSelected(atomScript.selected);
			}
			else{
				atomScript.SetTransparent(false);
			}
		}
	}

	//this functions creates, moves, and destroys bond distance text depending on the distance to other atoms in the system
	void UpdateBondText(){
		Quaternion cameraRotation = Camera.main.transform.rotation;
		Vector3 left = cameraRotation * -Vector3.right;
		Vector3 right = cameraRotation * Vector3.right;

		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		for (int i = 0; i < allMolecules.Length; i++) {
			GameObject atomNeighbor = allMolecules[i];
			if(atomNeighbor == gameObject) continue;
			float distance = Vector3.Distance(gameObject.transform.position, atomNeighbor.transform.position);
			if(distance < BondDistance(atomNeighbor)){

				TextMesh bondDistance = null;

				Vector3 midpoint = new Vector3((gameObject.transform.position.x + atomNeighbor.transform.position.x) / 2.0f, (gameObject.transform.position.y + atomNeighbor.transform.position.y) / 2.0f, (gameObject.transform.position.z + atomNeighbor.transform.position.z) / 2.0f);
				
				if(atomNeighbor.transform.position.x > gameObject.transform.position.x){
					Vector3 direction = gameObject.transform.position - atomNeighbor.transform.position;
					float angle = Vector3.Angle(direction, right);
					float percentToChange = (angle - 90) / 90.0f;
					midpoint += (direction * (.15f * percentToChange));
				}
				else{
					Vector3 direction = atomNeighbor.transform.position - gameObject.transform.position;
					float angle = Vector3.Angle(direction, right);
					float percentToChange = (angle - 90) / 90.0f;
					midpoint += (direction * (.15f * percentToChange));
				}

				try{
					bondDistance = bondDistanceText[atomNeighbor.name];
					bondDistance.transform.rotation = cameraRotation;
					bondDistance.transform.position = midpoint;
				}catch (KeyNotFoundException e){
					bondDistance = Instantiate(textMeshPrefab, midpoint, cameraRotation) as TextMesh;
					bondDistanceText.Add(atomNeighbor.name, bondDistance);
				}
				bondDistance.text = (Math.Round(distance, 1)).ToString();
			}
			else{
				//we need to check if there is text and if there is, remove it. Otherwise dont do anything
				try{
					TextMesh bondDistance = bondDistanceText[atomNeighbor.name];
					Destroy(bondDistance);
					bondDistanceText.Remove(atomNeighbor.name);
				}catch(KeyNotFoundException e){} //dont do anything with the caught exception
			}
		}

	}
	
	public void RemoveBondText(){
		foreach (KeyValuePair<String, TextMesh> keyValue in bondDistanceText) {
			TextMesh bondDistance = keyValue.Value;
			Destroy(bondDistance);
		}
		bondDistanceText.Clear ();
	}

	//this function destroys all bond distance text in the scene
	void RemoveAllBondText(){
		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		for (int i = 0; i < allMolecules.Length; i++) {
			GameObject currAtom = allMolecules[i];
			Atom atomScript = currAtom.GetComponent<Atom>();
			atomScript.RemoveBondText();
		}
	}


}
