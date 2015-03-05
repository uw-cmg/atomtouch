using UnityEngine;

public class GlowEffect : MonoBehaviour {
	
	public Material glowMaterial;
	public Shader glowReplaceShader;
	
	// Toggle between using the object's alpha channel for glow. This setting is good for mobile devices - 
	// it uses less memory and doesn't cause as many draw calls though it uses the object's alpha channel.
	public bool useAlphaChannelForGlow = false;
	
	// Toggle between using the simple glow effect. This setting is good for older mobile devices. 
	// sIt further reduces the amount of memory required and the number of draw calls.
	public bool useSimpleGlow = false;
	
	// The number of times the glow texture should be blurred. The more blur iterations the wider the glow. This value is only used if useSimpleGlow is false.
	public int blurIterations = 4;
	
	// The distance of the samples taken for the blurred glow. Too big of a value will cause noise in the blur. This value is only used if useSimpleGlow is false. 
	public float blurSpread = 1.0f;
	
	// Multiplies the glow color by this value.
	public float glowMultiplier = 1.2f;

    // Multiplies the glow color by this color.
    public Color glowColorMultiplier = Color.white;
	
	[HideInInspector]
	public RenderTexture postEffectsRenderTexture;
	
	private RenderTexture cameraRenderTexture;
	private Camera shaderCamera;
	private RenderTexture replaceRenderTexture;
	private RenderTexture blurA;
	private RenderTexture blurB;
	
	public void OnEnable()
	{
		if (!useAlphaChannelForGlow) {
			replaceRenderTexture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
			replaceRenderTexture.wrapMode = TextureWrapMode.Clamp;
			replaceRenderTexture.useMipMap = false;
			replaceRenderTexture.isPowerOfTwo = true;
			replaceRenderTexture.filterMode = FilterMode.Bilinear;
			replaceRenderTexture.Create();
			
			glowMaterial.SetTexture("_Glow", replaceRenderTexture);
						
			shaderCamera = new GameObject("Glow Effect", typeof(Camera)).GetComponent<Camera>();
			shaderCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
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
/*
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
*/
        glowMaterial.SetFloat("_BlurSpread", blurSpread);
        glowMaterial.SetFloat("_GlowMultiplier", glowMultiplier);
        glowMaterial.SetColor("_GlowColorMultiplier", glowColorMultiplier);
	}
	
	public void OnDisable()
	{
		glowMaterial.mainTexture = null;
		GetComponent<Camera>().targetTexture = null;
		DestroyObject(shaderCamera);
		DestroyObject(blurA);
		DestroyObject(blurB);
	}
	
	public void OnPreRender()
	{
		if (!useAlphaChannelForGlow) {
			shaderCamera.CopyFrom(GetComponent<Camera>());
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
}