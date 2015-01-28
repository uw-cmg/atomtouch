using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Chart : MonoBehaviour {
	[HideInInspector]public static bool show = false;
	public GameObject yMaxText;
	public Material mat;

	private float[] dataPointArray;
	private float xSpacing;
	private Rect graphRect;
	private GameObject graphPanel;

	private AtomTouchGUI atomTouchGUI;
	private Vector2 graphOrigin;
	private float yMax;
	private float graphHeight;
	private float canvasScale;
	void Awake(){
			
	}
	void Start () {
		atomTouchGUI = AtomTouchGUI.myAtomTouchGUI;
		graphPanel = atomTouchGUI.graphPanel;
		graphRect = graphPanel.GetComponent<RectTransform>().rect;
		canvasScale = atomTouchGUI.hud.GetComponent<Canvas>().scaleFactor;
		graphHeight = graphRect.height * canvasScale;

		float graphOriginX = graphPanel.GetComponent<RectTransform>().anchorMin.x * Screen.width;
		float graphOriginY = graphPanel.GetComponent<RectTransform>().anchorMin.y * Screen.height;

		graphOrigin = new Vector2(graphOriginX, graphOriginY);
		//Debug.Log(graphOrigin.x + ", " + graphOrigin.y);
	}

	void OnPostRender(){
		if(!show)return;
		graphPanel.SetActive(show);
		PlotGraph();

	}
	void TestPlot(){
		dataPointArray = PairDistributionFunction.PairDistributionAverage;
		PlotManager.Instance.PlotCreate("MouseX", 0, 5, Color.green, new Vector2(100,100));
		for(int i=1; i < dataPointArray.Length; i++){
			PlotManager.Instance.PlotAdd("MouseX", graphHeight * dataPointArray[i]/yMax );
		}
		
	}
	void PlotGraph(){
		if(Atom.AllAtoms.Count < 2)return;
		Vector3 p1, p2;
		Vector3 screenPos1, screenPos2;
		dataPointArray = PairDistributionFunction.PairDistributionAverage;
		if(dataPointArray.Length <= 1){
			xSpacing = 0;
		}else{
			xSpacing = graphRect.width * canvasScale / (dataPointArray.Length-1);
		}

		yMax = Mathf.Max(dataPointArray);
		yMaxText.GetComponent<Text>().text = yMax.ToString("0.0");
		if(yMax <= 0)return;
		for(int i=0; i < dataPointArray.Length-1; i++){
			
			float d1 = (float)dataPointArray[i];
			float d2 = (float)dataPointArray[i+1];
			float percent1 = d1/yMax;
			float percent2 = d2/yMax;

			
			float yVal1 = percent1 * graphHeight; 
			float yVal2 = percent2 * graphHeight;

			screenPos1 = new Vector3(graphOrigin.x + (i * xSpacing), graphOrigin.y + yVal1, 5.0f);
			screenPos2 = new Vector3(screenPos1.x + xSpacing, graphOrigin.y + yVal2, 5.0f);

			//screen
			
			p1 = Camera.main.ScreenToWorldPoint(screenPos1);
			p2 = Camera.main.ScreenToWorldPoint(screenPos2);
			
			
			//Debug.Log(p1);
			StaticVariables.DrawLine(p1, p2, Color.white, Color.white, 0.015f, mat);
			
		}
	}

}
