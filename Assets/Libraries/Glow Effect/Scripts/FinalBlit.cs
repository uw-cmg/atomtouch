using UnityEngine;
using System.Collections;

// Low end devices only support a render texture format of RGB565 within OnRenderImage. Therefore an extra step neeeds to be taken
// in order to use a render texture format that supports the alpha channel. This step is not needed for other platforms so just
// destroy ourself.
public class FinalBlit : MonoBehaviour {
	public GlowEffect glowEffect;
	
	public void Start()
	{
#if UNITY_EDITOR || (!UNITY_IPHONE && !UNITY_ANDROID)
		Destroy(gameObject);
#endif
	}
	
	public void OnPreRender()
	{
		Graphics.Blit(glowEffect.postEffectsRenderTexture, (RenderTexture)null);
	}

}
