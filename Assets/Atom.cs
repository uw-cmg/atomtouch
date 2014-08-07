using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//L-J potentials from Zhen and Davies, Phys. Stat. Sol. a, 78, 595 (1983)
//Symbol, epsilon/k_Boltzmann (K) n-m version, 12-6 version, sigma (Angstroms),
//     mass in amu, mass in (20 amu) for Unity 
//     FCC lattice parameter in Angstroms, expected NN bond (Angs)
//Au: 4683.0, 5152.9, 2.6367, 196.967, 9.848, 4.080, 2.88
//Cu: 3401.1, 4733.5, 2.3374,  63.546, 3.177, 3.610, 2.55
//Pt: 7184.2, 7908.7, 2.5394, 165.084, 8.254, 3.920, 2.77

public abstract class Atom : MonoBehaviour
{
	private Vector3 offset;
	private Vector3 screenPoint;
	private Vector3 lastMousePosition;
	private GameObject moleculeToMove = null;
	private float deltaTouch2 = 0.0f;
	private bool moveZDirection = false;
	private float lastTapTime;
	private float tapTime = .35f;
	[HideInInspector]public bool selected = false;
	[HideInInspector]public bool doubleTapped = false;
	private Dictionary<String, Vector3> gameObjectOffsets;
	private Dictionary<String, Vector3> gameObjectScreenPoints;
	private bool atomIsClicked = false;
	private TextMesh angstromText;
	private Vector3 velocityBeforeCollision;
	private float dragStartTime;
	private bool dragCalled;

	public Material lineMaterial;
	public TextMesh textMeshPrefab;
	public bool held { get; set; }

	//variables that must be implemented because they are declared as abstract in the base class
	public abstract float epsilon{ get; } // J
	public abstract float sigma { get; }
	protected abstract float massamu{ get; } //amu
	protected abstract void SetSelected (bool selected);
	public abstract Color color { get; }
	public abstract void ChangeColor (Color color);
	public abstract String atomName { get; }
	
	private Vector3 lastVelocity = Vector3.zero;
	private Vector3 a_n = Vector3.zero;
	private Vector3 a_nplus1 = Vector3.zero;

	void Awake(){

		gameObject.rigidbody.velocity = new Vector3 (UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f));

	}

	void FixedUpdate(){
		//Time.timeScale = StaticVariables.timeScale;
		if (!StaticVariables.pauseTime) {
			GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
			List<GameObject> molecules = new List<GameObject>();
			
			for(int i = 0; i < allMolecules.Length; i++){
				double distance = Vector3.Distance(transform.position, allMolecules[i].transform.position);
				if(allMolecules[i] != gameObject && distance < (StaticVariables.cutoff)){
					molecules.Add(allMolecules[i]);
				}
			}
			
			Vector3 force = GetLennardJonesForce (molecules);

			//TTM clear out old velocities - actually, this seems to NOT work
			//gameObject.rigidbody.velocity = Vector3.zero;
			if(!gameObject.rigidbody.isKinematic) gameObject.rigidbody.angularVelocity = Vector3.zero;
			//gameObject.rigidbody.AddForce (force, mode:ForceMode.Force);
			//gameObject.rigidbody.AddForce (force*StaticVariables.fixedUpdateIntervalToRealTime, mode:ForceMode.Impulse);
			//gameObject.rigidbody.velocity= new Vector3 (0.5f, 0.5f, 0.5f);

			//TTM velocity verlet: v_n+1 = v_n + 0.5*(a_n+1 + a_n)*delta_t
			// we have v_n (current velocity before update)
			//         a_n+1 (acceleration it should have after update)
			//         x_n (current position)
			//         delta_t (time step)
			// we do not have a_n (current acceleration)
			// we do not have v_n-1 (previous velocity), to calculate a_n
			Vector3 v_n = gameObject.rigidbody.velocity;
			//float mymass = massamu/100.0f;
			//a_nplus1 = force/mymass; //force was already adjusted to 100 amu * Angstroms / unity time ^2
			a_nplus1 = force/gameObject.rigidbody.mass; 
			float delta_t = Time.fixedDeltaTime;
			Vector3 v_verlet = Vector3.zero;
			//TTM if no velocity to start out with, set a velocity
			if (v_n.magnitude == 0){
				v_verlet = a_nplus1 * delta_t;
				//gameObject.rigidbody.AddForce (force, mode:ForceMode.Force);
			}
			else{
				a_n = (v_n - lastVelocity) / delta_t;
				v_verlet = v_n + 0.5f*(a_nplus1 + a_n)*delta_t;
			}
			lastVelocity = v_n;
			Vector3 newVelocity = v_verlet * TemperatureCalc.squareRootAlpha;
			//Vector3 newVelocity = gameObject.rigidbody.velocity * TemperatureCalc.squareRootAlpha;
			//TTM only reset velocity if not zero
			if (!rigidbody.isKinematic && !float.IsInfinity(TemperatureCalc.squareRootAlpha) && allMolecules.Length > 1) {
				gameObject.rigidbody.velocity = newVelocity;
			}

			velocityBeforeCollision = gameObject.rigidbody.velocity;

			//CheckVelocity();
		}
		else{
			GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
			for(int i = 0; i < allMolecules.Length; i++){
				GameObject currAtom = allMolecules[i];
				if(!currAtom.rigidbody.isKinematic){
					currAtom.rigidbody.velocity = new Vector3(0.0f, 0.0f, 0.0f);
				}
			}
		}

	}

	Vector3 GetLennardJonesForce(List<GameObject> objectsInRange){
		//double startTime = Time.realtimeSinceStartup;
		Vector3 finalForce = new Vector3 (0.000f, 0.000f, 0.000f);
		for (int i = 0; i < objectsInRange.Count; i++) {
			Vector3 direction = new Vector3(objectsInRange[i].transform.position.x - transform.position.x, objectsInRange[i].transform.position.y - transform.position.y, objectsInRange[i].transform.position.z - transform.position.z);
			direction.Normalize();

			Atom otherAtomScript = objectsInRange[i].GetComponent<Atom>();
			float finalSigma = StaticVariables.sigmaValues[atomName+otherAtomScript.atomName];
			//TTM add transition to smooth curve to constant, instead of asymptote to infinity
			double r_min = StaticVariables.r_min_multiplier * finalSigma;

			double distance = Vector3.Distance(transform.position, objectsInRange[i].transform.position);
			double distanceMeters = distance * StaticVariables.angstromsToMeters; //distance in meters, though viewed in Angstroms
			double magnitude = 0.0;

			if(distance > r_min){
				double part1 = ((-48 * epsilon) / Math.Pow(distanceMeters, 2));
				double part2 = (Math.Pow ((finalSigma / distance), 12) - (.5f * Math.Pow ((finalSigma / distance), 6)));
				magnitude = (part1 * part2 * distanceMeters);
			}
			else{
				double r_min_meters = r_min * StaticVariables.angstromsToMeters;
				double V_rmin_part1 = ((-48 * epsilon) / Math.Pow(r_min_meters, 2));
				double V_rmin_part2 = (Math.Pow ((finalSigma / r_min), 12) - (.5f * Math.Pow ((finalSigma / r_min), 6)));
				double V_rmin_magnitude = (V_rmin_part1 * V_rmin_part2 * r_min_meters);

				double r_Vmax = StaticVariables.r_min_multiplier * finalSigma/1.5;
				double r_Vmax_meters = r_Vmax * StaticVariables.angstromsToMeters;
				double Vmax_part1 = ((-48 * epsilon) / Math.Pow(r_Vmax_meters, 2));
				double Vmax_part2 = (Math.Pow ((finalSigma / r_Vmax), 12) - (.5f * Math.Pow ((finalSigma / r_Vmax), 6)));
				double Vmax_magnitude = (Vmax_part1 * Vmax_part2 * r_Vmax_meters);

				double part1 = (distance/r_min)*(Math.Exp (distance)/Math.Exp (r_min));
				double part2 = Vmax_magnitude - V_rmin_magnitude;
				magnitude = Vmax_magnitude - (part1* part2);
			}
			finalForce += (direction * (float)magnitude);
			//double endTime = Time.realtimeSinceStartup;
			//print ("elapsedTime: " + (endTime - startTime));
		}

		Vector3 adjustedForce = finalForce / StaticVariables.mass100amuToKg;
		adjustedForce = adjustedForce / StaticVariables.angstromsToMeters;
		adjustedForce = adjustedForce * StaticVariables.fixedUpdateIntervalToRealTime * StaticVariables.fixedUpdateIntervalToRealTime;
		return adjustedForce;
	}



	void Update(){
		//print ("UpdateDelta: " + Time.deltaTime);
		gameObject.renderer.material.color = color;
		//gameObject.renderer.material.renderQueue = 3100;
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			HandleTouch ();
		}
		else{
			if(Input.GetMouseButtonDown(0)){
				if((Time.realtimeSinceStartup - lastTapTime) < tapTime){
					ResetDoubleTapped();
					doubleTapped = true;
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
			ApplyTransparency(StaticVariables.atomTransparency);
			Time.timeScale = .05f;
			CameraScript cameraScript = Camera.main.GetComponent<CameraScript>();
			cameraScript.setCameraCoordinates(transform);
		}
		CheckVelocity ();
	}

	void HandleTouchSelect(){
		if (Input.touchCount == 1) {
			if(Input.GetTouch(0).phase == TouchPhase.Began){
				Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
				RaycastHit hitInfo;
				if(Physics.Raycast(ray, out hitInfo) && hitInfo.transform.gameObject.tag == "Molecule" && hitInfo.transform.gameObject == gameObject){
					selected = !selected;
					SetSelected(selected);
				}
			}
		}
	}

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

	//controls for touch devices
	void HandleTouch(){
		if (Input.touchCount == 1) {
			HandleMovingAtom();
		}
		else if(Input.touchCount == 2){
			MoveAngstromText();
			Touch touch2 = Input.GetTouch(1);
			if(touch2.phase == TouchPhase.Began){
				moveZDirection = true;
			}
			else if(touch2.phase == TouchPhase.Moved){
				if(!selected){
					Vector2 touchOnePrevPos = touch2.position - touch2.deltaPosition;
					float deltaMagnitudeDiff = touch2.position.y - touchOnePrevPos.y;
					deltaTouch2 = deltaMagnitudeDiff / 10.0f;
					if(moleculeToMove != null){
						HighlightCloseAtoms();
						Quaternion cameraRotation = Camera.main.transform.rotation;
						Vector3 projectPosition = moleculeToMove.transform.position;
						projectPosition += (cameraRotation * new Vector3(0.0f, 0.0f, deltaTouch2));
						moleculeToMove.transform.position = CheckPosition(projectPosition);
						screenPoint += new Vector3(0.0f, 0.0f, deltaTouch2);
					}
				}
				else{
					Vector2 touchOnePrevPos = touch2.position - touch2.deltaPosition;
					float deltaMagnitudeDiff = touch2.position.y - touchOnePrevPos.y;
					deltaTouch2 = deltaMagnitudeDiff / 10.0f;
					GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
					List<Vector3> atomPositions = new List<Vector3>();
					bool moveAtoms = true;
					for(int i = 0; i < allMolecules.Length; i++){
						GameObject currAtom = allMolecules[i];
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
						atomPositions.Add(newAtomPosition);
					}

					if(atomPositions.Count > 0 && moveAtoms){
						for(int i = 0; i < allMolecules.Length; i++){
							GameObject currAtom = allMolecules[i];
							Vector3 newAtomPosition = atomPositions[i];
							currAtom.transform.position = newAtomPosition;
						}
					}
				}
			}
		}
		else if(Input.touchCount == 0 && moveZDirection){
			moveZDirection = false;
			moleculeToMove = null;
			held = false;
			GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
			for(int i = 0; i < allMolecules.Length; i++){
				GameObject currAtom = allMolecules[i];
				Atom atomScript = currAtom.GetComponent<Atom>();
				atomScript.SetSelected(atomScript.selected);
			}
			
		}
	}

	void ResetDoubleTapped(){
		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		for (int i = 0; i < allMolecules.Length; i++) {
			Atom atomScript = allMolecules[i].GetComponent<Atom>();
			atomScript.doubleTapped = false;
		}
	}

	void HandleMovingAtom(){
		Touch touch = Input.GetTouch(0);

		if(touch.phase == TouchPhase.Began){
			if((Time.time - lastTapTime) < tapTime){
				ResetDoubleTapped();
				doubleTapped = true;
			}
			Ray ray = Camera.main.ScreenPointToRay( Input.touches[0].position );
			RaycastHit hitInfo;
			//this is the iOS equivalent to OnMouseUp
			if (Physics.Raycast( ray, out hitInfo ) && hitInfo.transform.gameObject.tag == "Molecule" && hitInfo.transform.gameObject == gameObject)
			{
				dragStartTime = Time.realtimeSinceStartup;
				dragCalled = false;
				SpawnAngstromText();
				ApplyTransparency(StaticVariables.atomTransparency);
				if(!selected){
					moleculeToMove = gameObject;
					screenPoint = Camera.main.WorldToScreenPoint(transform.position);
					offset = moleculeToMove.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y - 50, screenPoint.z));
					held = true;
					rigidbody.isKinematic = true;
				}
				else{
					GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
					gameObjectOffsets = new Dictionary<String, Vector3>();
					gameObjectScreenPoints = new Dictionary<String, Vector3>();
					for(int i = 0; i < allMolecules.Length; i++){
						GameObject currAtom = allMolecules[i];
						Atom atomScript = currAtom.GetComponent<Atom>();
						if(atomScript.selected){
							currAtom.rigidbody.isKinematic = true;
							Vector3 pointOnScreen = Camera.main.WorldToScreenPoint(currAtom.transform.position);
							Vector3 atomOffset = currAtom.transform.position - Camera.main.ScreenToWorldPoint(
								new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y - 15.0f, pointOnScreen.z));
							held = true;
							//print ("adding key: " + currAtom.name);
							gameObjectOffsets.Add(currAtom.name, atomOffset);
							gameObjectScreenPoints.Add(currAtom.name, pointOnScreen);
						}
					}
				}
				lastTapTime = Time.time;
			}
		}
		//this is the iOS equivalent to OnMouseDrag
		else if(touch.phase == TouchPhase.Moved){

			if(Time.realtimeSinceStartup - dragStartTime > 0.1f){
				dragCalled = true;
				atomIsClicked = true;
				MoveAngstromText();
				if(!selected){
					if(moleculeToMove != null && !doubleTapped){
						HighlightCloseAtoms();
						Vector3 curScreenPoint = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, screenPoint.z);
						Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
						lastMousePosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0.0f);
						curPosition = CheckPosition(curPosition);
						moleculeToMove.transform.position = curPosition;
					}
				}
				else{
					if (held){
						GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
						List<Vector3> atomPositions = new List<Vector3>();
						bool moveAtoms = true;
						for(int i = 0; i < allMolecules.Length; i++){
							GameObject currAtom = allMolecules[i];
							Atom atomScript = currAtom.GetComponent<Atom>();
							if(atomScript.selected){
								if(gameObjectOffsets != null && gameObjectScreenPoints != null){
									Vector3 currScreenPoint = gameObjectScreenPoints[currAtom.name];
									Vector3 currOffset = gameObjectOffsets[currAtom.name];
									Vector3 objScreenPoint = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, currScreenPoint.z);
									Vector3 curPosition = Camera.main.ScreenToWorldPoint(objScreenPoint) + currOffset;
									Vector3 newAtomPosition = CheckPosition(curPosition);
									if(newAtomPosition != curPosition){
										moveAtoms = false;
									}
									atomPositions.Add(newAtomPosition);
								}
							}
						}
						
						if(atomPositions.Count > 0 && moveAtoms){
							for(int i = 0; i < allMolecules.Length; i++){
								GameObject currAtom = allMolecules[i];
								Vector3 newAtomPosition = atomPositions[i];
								currAtom.transform.position = newAtomPosition;
							}
						}
						
						
					}
				}
			}


		}
		//this is the iOS equivalent to OnMouseUp
		else if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled){
			if(!dragCalled && moleculeToMove != null){
				selected = !selected;
				SetSelected(selected);
			}
			atomIsClicked = false;
			DestroyAngstromText();
			GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
			moleculeToMove = null;
			if(!selected){
				if(moleculeToMove != null){
					//Quaternion cameraRotation = Camera.main.transform.rotation;
					rigidbody.isKinematic = false;
					//rigidbody.AddForce (cameraRotation * mouseDelta * 50.0f);
					held = false;
				}
			}
			else{
				for(int i = 0; i < allMolecules.Length; i++){
					GameObject currAtom = allMolecules[i];
					Atom atomScript = currAtom.GetComponent<Atom>();
					if(atomScript.selected){
						currAtom.rigidbody.isKinematic = false;
						atomScript.held = false;
					}
				}
			}

			for(int i = 0; i < allMolecules.Length; i++){
				GameObject currAtom = allMolecules[i];
				Atom atomScript = currAtom.GetComponent<Atom>();
				atomScript.SetSelected(atomScript.selected);
			}
		}
	}

	void SpawnAngstromText(){
		Quaternion cameraRotation = Camera.main.transform.rotation;
		Vector3 up = cameraRotation * Vector3.up;
		Vector3 left = cameraRotation * -Vector3.right;
		angstromText = Instantiate(textMeshPrefab, new Vector3(0.0f, 0.0f, 0.0f), cameraRotation) as TextMesh;
		angstromText.renderer.material.renderQueue = StaticVariables.overlay;
		Vector3 newPosition = transform.position + (left * 1.0f) + (up * 2.0f);
		angstromText.transform.position = newPosition;
		angstromText.text = "1 Angstrom";
		LineRenderer angstromLine = angstromText.transform.gameObject.AddComponent<LineRenderer> ();
		angstromLine.material = lineMaterial;
		angstromLine.SetColors(Color.yellow, Color.yellow);
		angstromLine.SetWidth(0.2F, 0.2F);
		angstromLine.SetVertexCount(2);
	}

	void MoveAngstromText(){
		Quaternion cameraRotation = Camera.main.transform.rotation;
		Vector3 up = cameraRotation * Vector3.up;
		Vector3 left = cameraRotation * -Vector3.right;
		Vector3 newPosition = transform.position + (left * 1.0f) + (up * 2.0f);
		if (angstromText != null) {
			angstromText.transform.position = newPosition;
			LineRenderer angstromLine = angstromText.GetComponent<LineRenderer> ();
			Vector3 position1 = transform.position + (left * .5f) + (up);
			Vector3 position2 = transform.position + (left * -.5f) + (up);
			angstromLine.SetPosition(0, position1);
			angstromLine.SetPosition(1, position2);
		}
	}

	void DestroyAngstromText(){
		if (angstromText != null) {
			LineRenderer angstromLine = angstromText.GetComponent<LineRenderer> ();
			Destroy(angstromLine);
			Destroy(angstromText);
		}
	}
	
	
	//controls for debugging on pc
	void OnMouseDown (){
		if (Application.platform != RuntimePlatform.IPhonePlayer) {
			dragStartTime = Time.realtimeSinceStartup;
			dragCalled = false;
			SpawnAngstromText();
			ApplyTransparency(StaticVariables.atomTransparency);
			
			if(!selected){
				rigidbody.isKinematic = true;
				screenPoint = Camera.main.WorldToScreenPoint(transform.position);
				offset = transform.position - Camera.main.ScreenToWorldPoint(
					new Vector3(Input.mousePosition.x, Input.mousePosition.y - 15.0f, screenPoint.z));
				held = true;
			}
			else{
				GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
				gameObjectOffsets = new Dictionary<String, Vector3>();
				gameObjectScreenPoints = new Dictionary<String, Vector3>();
				for(int i = 0; i < allMolecules.Length; i++){
					GameObject currAtom = allMolecules[i];
					Atom atomScript = currAtom.GetComponent<Atom>();
					if(atomScript.selected){
						currAtom.rigidbody.isKinematic = true;
						Vector3 pointOnScreen = Camera.main.WorldToScreenPoint(currAtom.transform.position);
						Vector3 atomOffset = currAtom.transform.position - Camera.main.ScreenToWorldPoint(
							new Vector3(Input.mousePosition.x, Input.mousePosition.y - 15.0f, pointOnScreen.z));
						atomScript.held = true;
						//print ("adding key: " + currAtom.name);
						gameObjectOffsets.Add(currAtom.name, atomOffset);
						gameObjectScreenPoints.Add(currAtom.name, pointOnScreen);
					}
				}
			}
		}
	}
	
	void OnMouseDrag(){
		if (Application.platform != RuntimePlatform.IPhonePlayer) {

			if(Time.realtimeSinceStartup - dragStartTime > 0.1f){
				dragCalled = true;
				atomIsClicked = true;
				Quaternion cameraRotation = Camera.main.transform.rotation;
				MoveAngstromText();
				
				HighlightCloseAtoms();

				if(!selected){
					if((lastMousePosition - Input.mousePosition).magnitude > 0 && !doubleTapped){
						Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
						Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
						curPosition = CheckPosition(curPosition);
						transform.position = curPosition;
					}
					
					float deltaZ = -Input.GetAxis("Mouse ScrollWheel");
					Vector3 projectPosition = transform.position;
					projectPosition += (cameraRotation * new Vector3(0.0f, 0.0f, deltaZ));
					transform.position = CheckPosition(projectPosition);
					screenPoint += new Vector3(0.0f, 0.0f, deltaZ);
				}
				else{
					GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
					bool noneDoubleTapped = true;
					for(int i = 0; i < allMolecules.Length; i++){
						GameObject currAtom = allMolecules[i];
						Atom atomScript = currAtom.GetComponent<Atom>();
						if(atomScript.doubleTapped && atomScript.selected) noneDoubleTapped = false;
					}
					
					if(noneDoubleTapped){
						List<Vector3> atomPositions = new List<Vector3>();
						bool moveAtoms = true;
						for(int i = 0; i < allMolecules.Length; i++){
							GameObject currAtom = allMolecules[i];
							Atom atomScript = currAtom.GetComponent<Atom>();
							Vector3 newAtomPosition = currAtom.transform.position;
							if((lastMousePosition - Input.mousePosition).magnitude > 0 && atomScript.selected){
								//print ("looking for key: " + currAtom.name);
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
			
			
			lastMousePosition = Input.mousePosition;
		}
		
	}
	
	void OnMouseUp (){
		if (Application.platform != RuntimePlatform.IPhonePlayer) {
			if(!dragCalled){
				selected = !selected;
				SetSelected(selected);
			}
			atomIsClicked = false;
			GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
			DestroyAngstromText();

			if(!selected){
				rigidbody.isKinematic = false;
				held = false;
			}
			else{
				for(int i = 0; i < allMolecules.Length; i++){
					GameObject currAtom = allMolecules[i];
					Atom atomScript = currAtom.GetComponent<Atom>();
					if(atomScript.selected){
						currAtom.rigidbody.isKinematic = false;
						atomScript.held = false;
					}
				}
			}
			
			for(int i = 0; i < allMolecules.Length; i++){
				GameObject currAtom = allMolecules[i];
				Atom atomScript = currAtom.GetComponent<Atom>();
				atomScript.SetSelected(atomScript.selected);
			}
			
			Quaternion cameraRotation = Camera.main.transform.rotation;
			Vector2 direction = (Input.mousePosition - lastMousePosition);
			direction.Normalize();
			float magnitude = 10.0f;
			Vector3 flingVector = magnitude * new Vector3(direction.x, direction.y, 0.0f);
			gameObject.rigidbody.velocity = flingVector;
//			gameObject.rigidbody.AddForce(cameraRotation * flingVector, ForceMode.Impulse);

		}
	}

	
	void SetTransparency(float transparency){
		Color newColor = new Color (color.r, color.g, color.b, transparency);
		ChangeColor (newColor);
	}

	public float BondDistance(GameObject otherAtom){
		Atom otherAtomScript = otherAtom.GetComponent<Atom> ();
		return 1.225f * StaticVariables.sigmaValues [atomName+otherAtomScript.atomName];
	}
		
	
	void ApplyTransparency(float transparency){
		ResetTransparency ();
		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		for (int i = 0; i < allMolecules.Length; i++) {
			GameObject currAtom = allMolecules[i];
			if(currAtom == gameObject) continue;
			Color transparentColor = new Color(currAtom.renderer.material.color.r, currAtom.renderer.material.color.g, currAtom.renderer.material.color.b, transparency);
			Atom currAtomScript = currAtom.GetComponent<Atom>();
			if(!currAtomScript.selected){
				currAtomScript.ChangeColor(transparentColor);
			}
		}
	}

	public void ResetTransparency(){
		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		for (int i = 0; i < allMolecules.Length; i++) {
			GameObject currAtom = allMolecules[i];
			Color transparentColor = new Color(currAtom.renderer.material.color.r, currAtom.renderer.material.color.g, currAtom.renderer.material.color.b, 1.0f);
			Atom currAtomScript = currAtom.GetComponent<Atom>();
			currAtomScript.ChangeColor(transparentColor);
		}
	}

	void HighlightCloseAtoms(){
		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
		for (int i = 0; i < allMolecules.Length; i++) {
			GameObject currAtom = allMolecules[i];
			if(currAtom == gameObject) continue;
			if(Vector3.Distance(currAtom.transform.position, gameObject.transform.position) < 5.0f){
				Color solidColor = new Color(currAtom.renderer.material.color.r, currAtom.renderer.material.color.g, currAtom.renderer.material.color.b, 1.0f);
				Atom currAtomScript = currAtom.GetComponent<Atom>();
				currAtomScript.ChangeColor(solidColor);
			}
			else{
				Color transparentColor = new Color(currAtom.renderer.material.color.r, currAtom.renderer.material.color.g, currAtom.renderer.material.color.b, StaticVariables.atomTransparency);
				Atom currAtomScript = currAtom.GetComponent<Atom>();
				currAtomScript.ChangeColor(transparentColor);
			}
		}
	}
	

	void CheckVelocity(){

		if (gameObject.rigidbody.isKinematic) return;

		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment> ();
		Vector3 newVelocity = gameObject.rigidbody.velocity;
		if (gameObject.transform.position.x > createEnvironment.centerPos.x + (createEnvironment.width / 2.0f) - createEnvironment.errorBuffer) {
			newVelocity.x = Math.Abs(newVelocity.x) * -1;
		}
		if (gameObject.transform.position.x < createEnvironment.centerPos.x - (createEnvironment.width / 2.0f) + createEnvironment.errorBuffer) {
			newVelocity.x = Math.Abs(newVelocity.x);
		}
		if (gameObject.transform.position.y > createEnvironment.centerPos.y + (createEnvironment.height / 2.0f) - createEnvironment.errorBuffer) {
			newVelocity.y = Math.Abs(newVelocity.y) * -1;
		}
		if (gameObject.transform.position.y < createEnvironment.centerPos.y - (createEnvironment.height / 2.0f) + createEnvironment.errorBuffer) {
			newVelocity.y = Math.Abs(newVelocity.y);
		}
		if (gameObject.transform.position.z > createEnvironment.centerPos.z + (createEnvironment.depth / 2.0f) - createEnvironment.errorBuffer) {
			newVelocity.z = Math.Abs(newVelocity.z) * -1;
		}
		if (gameObject.transform.position.z < createEnvironment.centerPos.z - (createEnvironment.depth / 2.0f) + createEnvironment.errorBuffer) {
			newVelocity.z = Math.Abs(newVelocity.z);
		}
		gameObject.rigidbody.velocity = newVelocity;
	}

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


}

