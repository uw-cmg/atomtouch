using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tooltip : MonoBehaviour {
	public static Tooltip self;
	private float playedTime = 0f;
	private float fadeInLength = 0.3f;
	private float stayLength = 4f;
	private float fadeOutLength = 0.3f;
	private bool fading = false;
	private float totalLength;
	//panel
	private Color fadeInStepColor;
	private Color fadeOutStepColor;
	private Color textFadeInStepColor;
	private Color textFadeOutStepColor;

	private float fadeStepTime = 0.001f;

	private Image image;
	public Text text;
	public static bool fadePlayed = false;
	//set in inspector
	//panel
	public Color hiddenColor;
	public Color shownColor;
	//text
	public Color textHiddenColor;
	public Color textShownColor;
	void Awake(){
		self = this;
		totalLength = fadeInLength + stayLength + fadeOutLength;
		fadeInStepColor = new Color(0,0,0, (shownColor.a-hiddenColor.a)/fadeInLength);
		fadeOutStepColor = new Color(0,0,0, (shownColor.a-hiddenColor.a)/fadeOutLength);
		textFadeInStepColor = new Color(0,0,0, (textShownColor.a-textHiddenColor.a)/fadeInLength);
		textFadeOutStepColor = new Color(0,0,0, (textShownColor.a-textHiddenColor.a)/fadeOutLength);

		image = GetComponent<Image>();

	}
	// Use this for initialization
	void Start () {
		image.color = hiddenColor;
		text.color = textHiddenColor;
	}
	public IEnumerator Fade(string tipText){
		fadePlayed = true;
		fading = true;
		text.text = tipText;
		while(fading){
			yield return new WaitForSeconds(fadeStepTime);
			//Debug.Log(playedTime);
			if(playedTime <= fadeInLength){
				//fade the text color too
				text.color += (Time.deltaTime/Time.timeScale) * textFadeInStepColor;
				image.color += (Time.deltaTime/Time.timeScale) * fadeInStepColor;
			}else if(playedTime > fadeInLength
				&& playedTime < (fadeInLength+stayLength)){
				//do nothing
				image.color = shownColor;
				text.color = textShownColor;

			}else if(playedTime <= totalLength){

				text.color -= (Time.deltaTime/Time.timeScale) * textFadeOutStepColor;
				image.color -= (Time.deltaTime/Time.timeScale) *fadeOutStepColor;
			}else if(playedTime > totalLength){

				fading = false;
				playedTime = 0f;
				image.color = hiddenColor;
				text.color = textHiddenColor;

				continue;
			}
			playedTime += Time.deltaTime/Time.timeScale;
		}
		
	}
	// Update is called once per frame
	void Update () {
		
	}

}
