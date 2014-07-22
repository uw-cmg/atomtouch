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
	private Vector3 mouseDelta;
	private GameObject moleculeToMove = null;
	private Vector2 prevTouchPosition = new Vector2(0.0f, 0.0f);
	private float deltaTouch2 = 0.0f;
	private bool moveZDirection = false;
	private float lastTapTime;
	private float tapTime = .35f;
	[HideInInspector]public bool selected = false;
	[HideInInspector]public bool doubleTapped = false;
	private Dictionary<String, Vector3> gameObjectOffsets;
	private Dictionary<String, Vector3> gameObjectScreenPoints;
	private Vector3 velocityBeforeCollision;
	private bool atomIsClicked = false;

	public Material lineMaterial;
	public bool held { get; set; }

	//variables that must be implemented because they are declared as abstract in the base class
	protected abstract float epsilon{ get; } // J
	protected abstract float sigma{ get; } // m=Angstroms for Unity
	protected abstract float massamu{ get; } //amu
	protected abstract void SetSelected (bool selected);
	public abstract Color color { get; }
	public abstract void ChangeColor (Color color);

	void FixedUpdate(){
		Time.timeScale = StaticVariables.timeScale;
		if (!StaticVariables.pauseTime) {
			GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
			List<GameObject> molecules = new List<GameObject>();
			float totalEnergy = 0.0f;
			
			for(int i = 0; i < allMolecules.Length; i++){
				double distance = Vector3.Distance(transform.position, allMolecules[i].transform.position);
				if(allMolecules[i] != gameObject && distance < (StaticVariables.cutoff * sigma)){
					molecules.Add(allMolecules[i]);
				}
			}
			
			Vector3 force = GetLennardJonesForce (molecules);
			rigidbody.AddForce (force);
			
			//adjust velocity for the desired temperature of the system
			//if (Time.time > StaticVariables.tempDelay) {
			Vector3 newVelocity = gameObject.rigidbody.velocity * TemperatureCalc.squareRootAlpha;
			if (!rigidbody.isKinematic && !float.IsInfinity(TemperatureCalc.squareRootAlpha) && allMolecules.Length > 1) {
				rigidbody.velocity = newVelocity;
			}
			//}
			
			velocityBeforeCollision = rigidbody.velocity;
			//print (gameObject.name + " velocityX: " + rigidbody.velocity.x + " velocityY: " + rigidbody.velocity.y + " velocityZ: " + rigidbody.velocity.z);
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
		double finalMagnitude = 0;
		Vector3 finalForce = new Vector3 (0.000f, 0.000f, 0.000f);
		for (int i = 0; i < objectsInRange.Count; i++) {
			//Vector3 vect = molecules[i].transform.position - transform.position;
			Vector3 direction = new Vector3(objectsInRange[i].transform.position.x - transform.position.x, objectsInRange[i].transform.position.y - transform.position.y, objectsInRange[i].transform.position.z - transform.position.z);
			direction.Normalize();
			
			double distance = Vector3.Distance(transform.position, objectsInRange[i].transform.position);
			double distanceMeters = distance * StaticVariables.angstromsToMeters; //distance in meters, though viewed in Angstroms
			double part1 = ((-48 * epsilon) / Math.Pow(distanceMeters, 2));
			double part2 = (Math.Pow ((sigma / distance), 12) - (.5f * Math.Pow ((sigma / distance), 6)));
			double magnitude = (part1 * part2 * distanceMeters);
			finalForce += (direction * (float)magnitude);
			finalMagnitude += magnitude;
		}
		
		double adjustedForceMagnitude = finalMagnitude / StaticVariables.mass100amuToKg;
		adjustedForceMagnitude = adjustedForceMagnitude * StaticVariables.eyeAdjustment;
		Vector3 adjustedForce = finalForce / StaticVariables.mass100amuToKg; //adjust mass input for units of 100 amu
		//Distances are all in meters right now; do not distance-correct adjustedForce = adjustedForce * (float)(Math		.Pow (10, -10)); //normalize back Angstroms = m from extra r_ij denomintor term
		adjustedForce = adjustedForce * StaticVariables.eyeAdjustment;
		return adjustedForce;
	}

	void Update(){
		gameObject.renderer.material.color = color;
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			if(StaticVariables.touchScreen){
				HandleTouch ();
			}
			else{
				HandleTouchSelect();
			}
		}
		else{
			if(Input.GetMouseButtonDown(0)){
				if((Time.time - lastTapTime) < tapTime){
					ResetDoubleTapped();
					doubleTapped = true;
				}
				Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
				RaycastHit hitInfo;
				if (Physics.Raycast( ray, out hitInfo ) && hitInfo.transform.gameObject.tag == "Molecule" && hitInfo.transform.gameObject == gameObject){
					lastTapTime = Time.time;
				}
			}
			
			HandleRightClick();
		}
		if (doubleTapped) {
			CameraScript cameraScript = Camera.main.GetComponent<CameraScript>();
			cameraScript.setCameraCoordinates(transform);
		}
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
						HighlightAtoms();
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
					HighlightAtoms();
					for(int i = 0; i < allMolecules.Length; i++){
						GameObject currAtom = allMolecules[i];
						Quaternion cameraRotation = Camera.main.transform.rotation;
						Vector3 projectPosition = currAtom.transform.position;
						projectPosition += (cameraRotation * new Vector3(0.0f, 0.0f, deltaTouch2));
						currAtom.transform.position = CheckPosition(projectPosition);
						if(gameObjectScreenPoints != null){
							gameObjectScreenPoints[currAtom.name] += new Vector3(0.0f, 0.0f, deltaTouch2);
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
			if (Physics.Raycast( ray, out hitInfo ) && hitInfo.transform.gameObject.tag == "Molecule" && hitInfo.transform.gameObject == gameObject)
			{
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
		else if(touch.phase == TouchPhase.Moved){
			atomIsClicked = true;
			if(!selected){
				if(moleculeToMove != null && !doubleTapped){
					HighlightAtoms();
					Vector3 curScreenPoint = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, screenPoint.z);
					Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
					mouseDelta = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0.0f) - lastMousePosition;
					lastMousePosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0.0f);
					curPosition = CheckPosition(curPosition);
					moleculeToMove.transform.position = curPosition;
				}
			}
			else{
				if (held){
					HighlightAtoms();
					GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
					for(int i = 0; i < allMolecules.Length; i++){
						GameObject currAtom = allMolecules[i];
						Atom atomScript = currAtom.GetComponent<Atom>();
						if(atomScript.selected){
							if(gameObjectOffsets != null && gameObjectScreenPoints != null){
								Vector3 currScreenPoint = gameObjectScreenPoints[currAtom.name];
								Vector3 currOffset = gameObjectOffsets[currAtom.name];
								Vector3 objScreenPoint = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, currScreenPoint.z);
								Vector3 curPosition = Camera.main.ScreenToWorldPoint(objScreenPoint) + currOffset;
								curPosition = CheckPosition(curPosition);
								currAtom.transform.position = curPosition;
							}
						}
					}
				}
			}
		}
		else if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled){
			atomIsClicked = false;
			GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
			if(!selected){
				if(moleculeToMove != null){
					moleculeToMove = null;
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
	

	//controls for debugging on pc
	void OnMouseDown (){
		if (Application.platform != RuntimePlatform.IPhonePlayer) {

			if(StaticVariables.touchScreen){
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
			else{
				selected = !selected;
				SetSelected(selected);
			}
		}
	}
	
	void OnMouseDrag(){
		if (Application.platform != RuntimePlatform.IPhonePlayer) {
			atomIsClicked = true;
			HighlightAtoms();

			if(StaticVariables.touchScreen){
				if(!selected){
					if((lastMousePosition - Input.mousePosition).magnitude > 0 && !doubleTapped){
						Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
						Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
						curPosition = CheckPosition(curPosition);
						transform.position = curPosition;
					}
					
					float deltaZ = -Input.GetAxis("Mouse ScrollWheel");
					Quaternion cameraRotation = Camera.main.transform.rotation;
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
						for(int i = 0; i < allMolecules.Length; i++){
							GameObject currAtom = allMolecules[i];
							Atom atomScript = currAtom.GetComponent<Atom>();
							if((lastMousePosition - Input.mousePosition).magnitude > 0 && atomScript.selected){
								//print ("looking for key: " + currAtom.name);
								Vector3 currScreenPoint = gameObjectScreenPoints[currAtom.name];
								Vector3 currOffset = gameObjectOffsets[currAtom.name];
								Vector3 objScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, currScreenPoint.z);
								Vector3 curPosition = Camera.main.ScreenToWorldPoint(objScreenPoint) + currOffset;
								curPosition = CheckPosition(curPosition);
								currAtom.transform.position = curPosition;
							}
							
							if(atomScript.selected){
								float deltaZ = -Input.GetAxis("Mouse ScrollWheel");
								Quaternion cameraRotation = Camera.main.transform.rotation;
								Vector3 projectPosition = currAtom.transform.position;
								projectPosition += (cameraRotation * new Vector3(0.0f, 0.0f, deltaZ));
								currAtom.transform.position = CheckPosition(projectPosition);
								gameObjectScreenPoints[currAtom.name] += new Vector3(0.0f, 0.0f, deltaZ);
							}
						}
					}
				}
			}
			mouseDelta = Input.mousePosition - lastMousePosition;
			lastMousePosition = Input.mousePosition;
		}

	}

	void OnMouseUp (){
		if (Application.platform != RuntimePlatform.IPhonePlayer) {
			atomIsClicked = false;
			GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
			if(StaticVariables.touchScreen){
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

			}
		}
	}

	//hasnt been tested
	void SetTransparency(float transparency){
		Color newColor = new Color (color.r, color.g, color.b, transparency);
		ChangeColor (newColor);
	}

	void HighlightAtoms(){

		if (StaticVariables.axisUI) {
			GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
			CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment> ();
			for (int i = 0; i < allMolecules.Length; i++) {
				GameObject currAtom = allMolecules[i];
				if(currAtom == gameObject) continue;
				Color finalColor = Color.black;
				if(currAtom.transform.position.x < gameObject.transform.position.x + createEnvironment.errorBuffer
				   && currAtom.transform.position.x > gameObject.transform.position.x - createEnvironment.errorBuffer){
					//green
					finalColor += Color.green;
				}
				if(currAtom.transform.position.y < gameObject.transform.position.y + createEnvironment.errorBuffer
				   && currAtom.transform.position.y > gameObject.transform.position.y - createEnvironment.errorBuffer){
					//blue
					finalColor += Color.blue;
				}
				if(currAtom.transform.position.z < gameObject.transform.position.z + createEnvironment.errorBuffer
				   && currAtom.transform.position.z > gameObject.transform.position.z - createEnvironment.errorBuffer){
					//red
					finalColor += Color.red;
				}
				Atom atomScript = currAtom.GetComponent<Atom>();
				if(finalColor == Color.black){
					if(atomScript.selected){
						finalColor = StaticVariables.selectedColor;
					}
				}
				atomScript.ChangeColor(finalColor);
			}
		}

	}

	void OnRenderObject(){
		if (StaticVariables.axisUI) {
			GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");
			for (int i = 0; i < allMolecules.Length; i++) {
				GameObject currAtom = allMolecules[i];
				Atom atomScript = currAtom.GetComponent<Atom>();
				if(atomScript.atomIsClicked && atomScript.held && !atomScript.doubleTapped){
					DrawLineBorders(currAtom);
				}
			}
		}
	}

	void DrawLineBorders(GameObject currAtom){

		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment> ();
		Vector3 bottomPlanePos = createEnvironment.bottomPlane.transform.position;

		//x-y plane
		StaticVariables.DrawLine (new Vector3 (bottomPlanePos.x - (createEnvironment.width / 2.0f), bottomPlanePos.y, currAtom.transform.position.z), new Vector3 (bottomPlanePos.x + (createEnvironment.width / 2.0f), bottomPlanePos.y, currAtom.transform.position.z),
		                         Color.red, Color.red, .1f, lineMaterial);
		StaticVariables.DrawLine (new Vector3 (bottomPlanePos.x - (createEnvironment.width / 2.0f), bottomPlanePos.y, currAtom.transform.position.z), new Vector3 (bottomPlanePos.x - (createEnvironment.width / 2.0f), bottomPlanePos.y + createEnvironment.height, currAtom.transform.position.z),
		                          Color.red, Color.red, .1f, lineMaterial);
		StaticVariables.DrawLine (new Vector3 (bottomPlanePos.x + (createEnvironment.width / 2.0f), bottomPlanePos.y, currAtom.transform.position.z), new Vector3 (bottomPlanePos.x + (createEnvironment.width / 2.0f), bottomPlanePos.y + createEnvironment.height, currAtom.transform.position.z),
		                          Color.red, Color.red, .1f, lineMaterial);
		StaticVariables.DrawLine (new Vector3 (bottomPlanePos.x - (createEnvironment.width / 2.0f), bottomPlanePos.y + createEnvironment.height, currAtom.transform.position.z), new Vector3 (bottomPlanePos.x + (createEnvironment.width / 2.0f), bottomPlanePos.y + createEnvironment.height, currAtom.transform.position.z),
		                          Color.red, Color.red, .1f, lineMaterial);

		//x-z plane
		StaticVariables.DrawLine (new Vector3 (bottomPlanePos.x - (createEnvironment.width / 2.0f), currAtom.transform.position.y, bottomPlanePos.z - (createEnvironment.depth/2.0f)), new Vector3 (bottomPlanePos.x + (createEnvironment.width / 2.0f), currAtom.transform.position.y, bottomPlanePos.z - (createEnvironment.depth/2.0f)),
		                          Color.blue, Color.blue, .1f, lineMaterial);
		StaticVariables.DrawLine (new Vector3 (bottomPlanePos.x - (createEnvironment.width / 2.0f), currAtom.transform.position.y, bottomPlanePos.z + (createEnvironment.depth/2.0f)), new Vector3 (bottomPlanePos.x + (createEnvironment.width / 2.0f), currAtom.transform.position.y, bottomPlanePos.z + (createEnvironment.depth/2.0f)),
		                          Color.blue, Color.blue, .1f, lineMaterial);
		StaticVariables.DrawLine (new Vector3 (bottomPlanePos.x - (createEnvironment.width / 2.0f), currAtom.transform.position.y, bottomPlanePos.z - (createEnvironment.depth/2.0f)), new Vector3 (bottomPlanePos.x - (createEnvironment.width / 2.0f), currAtom.transform.position.y, bottomPlanePos.z + (createEnvironment.depth/2.0f)),
		                          Color.blue, Color.blue, .1f, lineMaterial);
		StaticVariables.DrawLine (new Vector3 (bottomPlanePos.x + (createEnvironment.width / 2.0f), currAtom.transform.position.y, bottomPlanePos.z - (createEnvironment.depth/2.0f)), new Vector3 (bottomPlanePos.x + (createEnvironment.width / 2.0f), currAtom.transform.position.y, bottomPlanePos.z + (createEnvironment.depth/2.0f)),
		                          Color.blue, Color.blue, .1f, lineMaterial);

		//y-z plane
		StaticVariables.DrawLine (new Vector3 (currAtom.transform.position.x, bottomPlanePos.y, bottomPlanePos.z - (createEnvironment.depth/2.0f)), new Vector3 (currAtom.transform.position.x, bottomPlanePos.y + createEnvironment.height, bottomPlanePos.z - (createEnvironment.depth/2.0f)),
		                          Color.green, Color.green, .1f, lineMaterial);
		StaticVariables.DrawLine (new Vector3 (currAtom.transform.position.x, bottomPlanePos.y, bottomPlanePos.z + (createEnvironment.depth/2.0f)), new Vector3 (currAtom.transform.position.x, bottomPlanePos.y + createEnvironment.height, bottomPlanePos.z + (createEnvironment.depth/2.0f)),
		                          Color.green, Color.green, .1f, lineMaterial);
		StaticVariables.DrawLine (new Vector3 (currAtom.transform.position.x, bottomPlanePos.y, bottomPlanePos.z - (createEnvironment.depth/2.0f)), new Vector3 (currAtom.transform.position.x, bottomPlanePos.y, bottomPlanePos.z + (createEnvironment.depth/2.0f)),
		                          Color.green, Color.green, .1f, lineMaterial);
		StaticVariables.DrawLine (new Vector3 (currAtom.transform.position.x, bottomPlanePos.y + createEnvironment.height, bottomPlanePos.z - (createEnvironment.depth/2.0f)), new Vector3 (currAtom.transform.position.x, bottomPlanePos.y + createEnvironment.height, bottomPlanePos.z + (createEnvironment.depth/2.0f)),
		                          Color.green, Color.green, .1f, lineMaterial);
	}

	void OnCollisionEnter(Collision other){
		//for (int i = 0; i < 100; i++) print ("");
		//print (gameObject.name + " velocityBeforeCollisionX: " + velocityBeforeCollision.x + " y: " + velocityBeforeCollision.y + " z: " + velocityBeforeCollision.z);
		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment> ();
		GameObject collidedPlane = other.transform.gameObject;
		//rigidbody.AddForce (Vector3.Reflect (velocityBeforeCollision, (cameraScript.centerPos - collidedPlane.transform.position).normalized), ForceMode.Impulse);
		Vector3 newVelocity = Vector3.Reflect (velocityBeforeCollision, (createEnvironment.centerPos - collidedPlane.transform.position).normalized);
		rigidbody.velocity = newVelocity;
	}

	Vector3 CheckPosition(Vector3 position){
		CreateEnvironment createEnvironment = Camera.main.GetComponent<CreateEnvironment> ();
		Vector3 bottomPlanePos = createEnvironment.bottomPlane.transform.position;
		if (position.y > bottomPlanePos.y + (createEnvironment.height) - createEnvironment.errorBuffer) {
			position.y = bottomPlanePos.y + (createEnvironment.height) - createEnvironment.errorBuffer;
		}
		if (position.y < bottomPlanePos.y + createEnvironment.errorBuffer) {
			position.y = bottomPlanePos.y + createEnvironment.errorBuffer;;
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

