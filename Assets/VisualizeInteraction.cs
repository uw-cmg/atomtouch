using UnityEngine;
using System.Collections;

public class VisualizeInteraction : MonoBehaviour {
	
	public Material mat;
	public float lineWidth = .2f;
	public float lineDistance = 5.0f;

	void OnPostRender(){
		GameObject[] allMolecules = GameObject.FindGameObjectsWithTag("Molecule");


		for (int i = 0; i < allMolecules.Length; i++) {
			GameObject currAtom = allMolecules[i];
			for(int j = i + 1; j < allMolecules.Length; j++){
				GameObject atomNeighbor = allMolecules[j];
				if(Vector3.Distance(currAtom.transform.position, atomNeighbor.transform.position) < lineDistance){
					//draw a line from currAtom to atomNeighbor
					Atom currAtomScript = currAtom.GetComponent<Atom>();
					Atom neighAtomScript = atomNeighbor.GetComponent<Atom>();
					DrawLine (currAtom.transform.position, atomNeighbor.transform.position, currAtomScript.GetColor(), neighAtomScript.GetColor());
				}
			}
		}
	}

	void DrawLine(Vector3 startingPos, Vector3 endingPos, Color atomColor1, Color atomColor2){

		Vector3 startingPos2 = (startingPos - endingPos);
		startingPos2.Normalize ();
		startingPos2 = Quaternion.Euler (new Vector3 (0.0f, 0.0f, -90.0f)) * startingPos2;
		startingPos2 *= -lineWidth;
		startingPos2 += startingPos;

		Vector3 endingPos2 = (endingPos - startingPos);
		endingPos2.Normalize ();
		endingPos2 = Quaternion.Euler (new Vector3 (0.0f, 0.0f, -90.0f)) * endingPos2;
		endingPos2 *= lineWidth;
		endingPos2 += endingPos;

		Vector3 startingPos3 = (startingPos - endingPos);
		startingPos3.Normalize ();
		startingPos3 = Quaternion.Euler (new Vector3 (0.0f, -90.0f, 0.0f)) * startingPos3;
		startingPos3 *= -lineWidth;
		startingPos3 += startingPos;

		Vector3 endingPos3 = (endingPos - startingPos);
		endingPos3.Normalize ();
		endingPos3 = Quaternion.Euler (new Vector3 (0.0f, -90.0f, 0.0f)) * endingPos3;
		endingPos3 *= lineWidth;
		endingPos3 += endingPos;

		Vector3 startingPos4 = (startingPos - endingPos);
		startingPos4.Normalize ();
		startingPos4 = Quaternion.Euler (new Vector3 (-90.0f, 00.0f, 0.0f)) * startingPos4;
		startingPos4 *= -lineWidth;
		startingPos4 += startingPos;
		
		Vector3 endingPos4 = (endingPos - startingPos);
		endingPos4.Normalize ();
		endingPos4 = Quaternion.Euler (new Vector3 (-90.0f, 00.0f, 0.0f)) * endingPos4;
		endingPos4 *= lineWidth;
		endingPos4 += endingPos;
		
		if (!mat) {
			return;
		}
		GL.LoadProjectionMatrix (Camera.main.projectionMatrix);
		GL.PushMatrix();
		mat.SetPass (0);
		GL.Begin (GL.QUADS);

		GL.Color (atomColor1);
		GL.Vertex (startingPos);
		GL.Color (atomColor2);
		GL.Vertex (endingPos);
		GL.Vertex (endingPos2);
		GL.Color (atomColor1);
		GL.Vertex (startingPos2);

		GL.Vertex (startingPos);
		GL.Color (atomColor2);
		GL.Vertex (endingPos);
		GL.Vertex (endingPos3);
		GL.Color (atomColor1);
		GL.Vertex (startingPos3);

		GL.Vertex (startingPos);
		GL.Color (atomColor2);
		GL.Vertex (endingPos);
		GL.Vertex (endingPos4);
		GL.Color (atomColor1);
		GL.Vertex (startingPos4);

		GL.End ();
		GL.PopMatrix();
	}
}
