using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tooltip : MonoBehaviour {
	public static Tooltip self;
	private float playedTime = 0f;
	private float fadeInLength = 0.5f;
	private float stayLength = 3f;
	private float fadeOutLength = 0.5f;
	private bool fading = false;
	private float totalLength;
	private Color fadeInStepColor;
	private Color fadeOutStepColor;
	private float fadeStepTime = 0.001f;
	public static bool fadePlayed = false;
	//set in inspector
	public Color hiddenColor;
	public Color shownColor;
	void Awake(){
		self = this;
		totalLength = fadeInLength + stayLength + fadeOutLength;
		fadeInStepColor = new Color(0,0,0, (shownColor.a-hiddenColor.a)/fadeInLength);
		fadeOutStepColor = new Color(0,0,0, (shownColor.a-hiddenColor.a)/fadeOutLength);
	
	}
	// Use this for initialization
	void Start () {
		GetComponent<Image>().color = hiddenColor;
	}
	public IEnumerator Fade(){
		fadePlayed = true;
		fading = true;
		while(fading){
			yield return new WaitForSeconds(fadeStepTime);
			//Debug.Log(playedTime);
			if(playedTime <= fadeInLength){

				GetComponent<Image>().color += (Time.deltaTime/Time.timeScale) * fadeInStepColor;
			}else if(playedTime > fadeInLength
				&& playedTime < (fadeInLength+stayLength)){
				//do nothing
				GetComponent<Image>().color = shownColor;

			}else if(playedTime <= totalLength){
				GetComponent<Image>().color -= (Time.deltaTime/Time.timeScale) *fadeOutStepColor;
			}else if(playedTime > totalLength){

				fading = false;
				playedTime = 0f;
				GetComponent<Image>().color = hiddenColor;
				continue;
			}
			playedTime += Time.deltaTime/Time.timeScale;
		}
		
	}
	// Update is called once per frame
	void Update () {
		
	}

}
