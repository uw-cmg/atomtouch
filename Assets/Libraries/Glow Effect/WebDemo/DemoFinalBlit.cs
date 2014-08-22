using UnityEngine;
using System.Collections;

public class DemoFinalBlit : MonoBehaviour {
	public DemoGlowEffect glowEffect;
	
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
