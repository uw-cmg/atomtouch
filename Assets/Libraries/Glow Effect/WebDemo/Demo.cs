using UnityEngine;
using System.Collections;

public class Demo : MonoBehaviour {
	
	public bool enableGlow = true;
	public bool useAlphaChannelForGlow = false;
	public bool useSimpleGlow = false;
	public int blurIterations = 2;
	public float blurSpread = 1.0f;
    public float glowMultiplier = 1.2f;
    public Color glowColorMultiplier = Color.white;
	
	public GameObject glowGroup;
	public GameObject alphaGlowGroup;
	
	public DemoGlowEffect glowEffect;
	public GameObject renderCamera;
	
	public void Start()
	{
		updateGlow();
	}
	
	public void OnGUI()
	{
		GUILayout.BeginVertical();
		
		bool prevBool = enableGlow;
		enableGlow = GUILayout.Toggle(enableGlow, "Enable Glow");
		if (prevBool != enableGlow) {
			updateGlow();
		}
		
		prevBool = useAlphaChannelForGlow;
		useAlphaChannelForGlow = GUILayout.Toggle(useAlphaChannelForGlow, "Use Alpha Channel For Glow");
		if (prevBool != useAlphaChannelForGlow) {
			updateGlow();	
		}
		prevBool = useSimpleGlow;
		useSimpleGlow = GUILayout.Toggle(useSimpleGlow, "Use Simple Glow");
		if (prevBool != useSimpleGlow) {
			updateGlow();	
		}
		
		GUILayout.Label("Blur Iterations");
		GUILayout.BeginHorizontal();
		int prevInt = blurIterations;
		blurIterations = Mathf.RoundToInt(GUILayout.HorizontalSlider(blurIterations, 1, 20));
		if (prevInt != blurIterations) {
			updateGlow();
		}
		GUILayout.Label(blurIterations.ToString());
		GUILayout.EndHorizontal();
		
		GUILayout.Label("Blur Spread");
		GUILayout.BeginHorizontal();
		float prevFloat = blurSpread;
		blurSpread = GUILayout.HorizontalSlider(blurSpread, 1, 2.5f); 
		if (prevFloat != blurSpread) {
			updateGlow();
		}
		
		GUILayout.Label(decimal.Round((decimal)blurSpread, 2).ToString());
		GUILayout.EndHorizontal();
		
		GUILayout.Label("Glow Multiplier");
		GUILayout.BeginHorizontal();
		prevFloat = glowMultiplier;
		glowMultiplier = GUILayout.HorizontalSlider(glowMultiplier, 0.5f, 10); 
		if (prevFloat != glowMultiplier) {
			updateGlow();
		}
		GUILayout.Label(decimal.Round((decimal)glowMultiplier, 2).ToString());
		GUILayout.EndHorizontal();
			
		GUILayout.EndVertical();
	}
	
	private void updateGlow()
	{
		glowEffect.enabled = enableGlow;
#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
#if UNITY_3_5
		renderCamera.SetActiveRecursively(enableGlow);
#else
		renderCamera.SetActive(enableGlow);
#endif
#endif

        if (enableGlow) {
#if UNITY_3_5
			glowGroup.SetActiveRecursively(!useAlphaChannelForGlow);
			alphaGlowGroup.SetActiveRecursively(useAlphaChannelForGlow);
#else
			glowGroup.SetActive(!useAlphaChannelForGlow);
			alphaGlowGroup.SetActive(useAlphaChannelForGlow);
#endif
			glowEffect.useAlphaChannelForGlow = useAlphaChannelForGlow;
			glowEffect.useSimpleGlow = useSimpleGlow;
			glowEffect.blurIterations = blurIterations;
			glowEffect.blurSpread = blurSpread;
			glowEffect.glowMultiplier = glowMultiplier;
            glowEffect.glowColorMultiplier = glowColorMultiplier;
			
			glowEffect.updateGlowEffect();
		}
	}
}
