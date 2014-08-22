using UnityEngine;

// Do not use this class in your game. Use GlowEffect instead. This class was made to easily switch between
// the different types of glow effects. 
public class DemoGlowEffect : MonoBehaviour {
	
	public Material glowMaterial;
	public Shader glowReplaceShader;
	
	public bool useAlphaChannelForGlow;
	public bool useSimpleGlow;
	public int blurIterations;
	public float blurSpread;
	public float glowMultiplier;
    public Color glowColorMultiplier;
	
	[HideInInspector]
	public RenderTexture postEffectsRenderTexture;
	
	private RenderTexture cameraRenderTexture;
	private Camera shaderCamera;
	private RenderTexture replaceRenderTexture;
	private RenderTexture blurA;
	private RenderTexture blurB;
		
	public void updateGlowEffect()
	{
		if (!useAlphaChannelForGlow) {
			replaceRenderTexture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
			replaceRenderTexture.wrapMode = TextureWrapMode.Clamp;
			replaceRenderTexture.useMipMap = false;
			replaceRenderTexture.isPowerOfTwo = true;
			replaceRenderTexture.filterMode = FilterMode.Bilinear;
			replaceRenderTexture.Create();
			
			glowMaterial.SetTexture("_Glow", replaceRenderTexture);
		}
		
		if (!useSimpleGlow) {
			blurA = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGB32);
			blurA.wrapMode = TextureWrapMode.Clamp;
			blurA.useMipMap = false;	
			blurA.filterMode = FilterMode.Bilinear;
			blurA.Create();	
			
			blurB = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGB32);
			blurB.wrapMode = TextureWrapMode.Clamp;
			blurB.useMipMap = false;	
			blurB.filterMode = FilterMode.Bilinear;
			blurB.Create();
				
			if (blurIterations % 2 == 0) {
				glowMaterial.SetTexture("_Glow", blurA);
			} else {
				glowMaterial.SetTexture("_Glow", blurB);
			}
        }

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
		// this will be used as the target texture so it can be blitted to another camera
		cameraRenderTexture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
		cameraRenderTexture.wrapMode = TextureWrapMode.Clamp;
		cameraRenderTexture.useMipMap = false;
		cameraRenderTexture.isPowerOfTwo = false;
		cameraRenderTexture.filterMode = FilterMode.Bilinear;
		cameraRenderTexture.Create();
		
		camera.targetTexture = cameraRenderTexture;
		camera.depthTextureMode = DepthTextureMode.None;
		
		// create a render texture to blit to the final camera
		postEffectsRenderTexture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
		postEffectsRenderTexture.wrapMode = TextureWrapMode.Clamp;
		postEffectsRenderTexture.useMipMap = false;
		postEffectsRenderTexture.isPowerOfTwo = false;
		postEffectsRenderTexture.filterMode = FilterMode.Bilinear;
		postEffectsRenderTexture.Create();
#endif

        glowMaterial.SetFloat("_BlurSpread", blurSpread);
        glowMaterial.SetFloat("_GlowMultiplier", glowMultiplier);
        glowMaterial.SetColor("_GlowColorMultiplier", glowColorMultiplier);
#if UNITY_3_5
		shaderCamera.gameObject.SetActiveRecursively(!useAlphaChannelForGlow);
#else
		shaderCamera.gameObject.SetActive(!useAlphaChannelForGlow);
#endif
	}
	
	public void OnPreRender()
	{
		if (!useAlphaChannelForGlow) {
			shaderCamera.CopyFrom(camera);
			shaderCamera.backgroundColor = Color.clear;
			shaderCamera.clearFlags = CameraClearFlags.SolidColor;
			shaderCamera.renderingPath = RenderingPath.Forward;
			shaderCamera.targetTexture = replaceRenderTexture;
			shaderCamera.RenderWithShader(glowReplaceShader, "RenderType");
		}
	}
	
	// UNITY_IPHONE and UNITY_ANDROID can't use OnRenderImage because the source and destination
    // render texture are in RGB565 format and we need the alpha layer.
#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
	public void OnPostRender()
	{
		calculateGlow(cameraRenderTexture, postEffectsRenderTexture);
	}
#else
    public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		calculateGlow(source, destination);	
	}
#endif
	
	private void calculateGlow(RenderTexture source, RenderTexture destination)
	{
		if (!useSimpleGlow) {
			// blur
			if (useAlphaChannelForGlow) {
				Graphics.Blit(source, blurB, glowMaterial, 2);
			} else {
				Graphics.Blit(replaceRenderTexture, blurB, glowMaterial, 1);
			}
			for (int i = 1; i < blurIterations; ++i) {
				if (i % 2 == 0) {
					Graphics.Blit(blurA, blurB, glowMaterial, 1); 
				} else {
					Graphics.Blit(blurB, blurA, glowMaterial, 1);
				}
			}
			// calculate glow
			Graphics.Blit(source, destination, glowMaterial, 0);
		} else {
			Graphics.Blit(source, destination, glowMaterial, (useAlphaChannelForGlow ? 4 : 3));
		}	
	}
	
	public void OnEnable()
	{
		if (!useAlphaChannelForGlow) {
			if (shaderCamera != null) {
#if UNITY_3_5
				shaderCamera.gameObject.SetActiveRecursively(true);
#else
				shaderCamera.gameObject.SetActive(true);
#endif
			} else {
				shaderCamera = new GameObject("Glow Effect", typeof(Camera)).camera;
			}
		}
		updateGlowEffect(); 
	}
	
	public void OnDisable()
	{
		glowMaterial.mainTexture = null;
		camera.targetTexture = null;
		if (shaderCamera != null) {
#if UNITY_3_5
			shaderCamera.gameObject.SetActiveRecursively(false);
#else
			shaderCamera.gameObject.SetActive(false);
#endif
		}
	}
}