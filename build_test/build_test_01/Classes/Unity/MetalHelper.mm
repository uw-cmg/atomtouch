
#if UNITY_CAN_USE_METAL

#include "UnityMetalSupport.h"
#include <QuartzCore/QuartzCore.h>
#include <libkern/OSAtomic.h>

#include <stdio.h>

#include "MetalHelper.h"
#include "UnityAppController.h"
#include "CVTextureCache.h"

extern bool _supportsMSAA;

extern "C" void InitMetal()
{
	_supportsMSAA = true;
}

static inline Class MTLTextureDescriptorClass()
{
	static Class _MTLTextureDescriptorClass = nil;
	if(_MTLTextureDescriptorClass == nil)
		_MTLTextureDescriptorClass = [UnityGetMetalBundle() classNamed:@"MTLTextureDescriptor"];
	return _MTLTextureDescriptorClass;
}


extern "C" void CreateSystemRenderingSurfaceMTL(UnityDisplaySurfaceMTL* surface)
{
	DestroySystemRenderingSurfaceMTL(surface);

	MTLPixelFormat colorFormat = surface->srgb ? MTLPixelFormatBGRA8Unorm_sRGB : MTLPixelFormatBGRA8Unorm;

	surface->layer.presentsWithTransaction = NO;
	surface->layer.drawsAsynchronously = YES;
	CGFloat backgroundColor[] = {0,0,0,1};
	surface->layer.backgroundColor = CGColorCreate(CGColorSpaceCreateDeviceRGB(), backgroundColor);
	surface->layer.device = surface->device;
	surface->layer.pixelFormat = colorFormat;
	//surface->layer.framebufferOnly = YES;
	surface->layer.framebufferOnly = NO;
	surface->colorFormat = colorFormat;
}

extern "C" void CreateRenderingSurfaceMTL(UnityDisplaySurfaceMTL* surface)
{
	DestroyRenderingSurfaceMTL(surface);

	const int w = surface->targetW, h = surface->targetH;

	if(w != surface->systemW || h != surface->systemH || surface->useCVTextureCache)
	{
		if(surface->useCVTextureCache)
			surface->cvTextureCache = CreateCVTextureCache();

		if(surface->cvTextureCache)
		{
			surface->cvTextureCacheTexture = CreateReadableRTFromCVTextureCache(surface->cvTextureCache, surface->targetW, surface->targetH, &surface->cvPixelBuffer);
			surface->targetColorRT = GetMetalTextureFromCVTextureCache(surface->cvTextureCacheTexture);
		}
		else
		{
			MTLTextureDescriptor* txDesc = [MTLTextureDescriptorClass() new];
			txDesc.textureType = MTLTextureType2D;
			txDesc.width = w;
			txDesc.height = h;
			txDesc.depth = 1;
			txDesc.pixelFormat = surface->srgb ? MTLPixelFormatBGRA8Unorm_sRGB : MTLPixelFormatBGRA8Unorm;
			txDesc.arrayLength = 1;
			txDesc.mipmapLevelCount = 1;
			surface->targetColorRT = [surface->device newTextureWithDescriptor:txDesc];
		}
		surface->targetColorRT.label = @"targetColorRT";
	}

	if(surface->msaaSamples > 1)
	{
		MTLTextureDescriptor* txDesc = [MTLTextureDescriptorClass() new];
		txDesc.textureType = MTLTextureType2DMultisample;
		txDesc.width = w;
		txDesc.height = h;
		txDesc.depth = 1;
		txDesc.pixelFormat = surface->srgb ? MTLPixelFormatBGRA8Unorm_sRGB : MTLPixelFormatBGRA8Unorm;
		txDesc.arrayLength = 1;
		txDesc.mipmapLevelCount = 1;
		txDesc.sampleCount = surface->msaaSamples;
		surface->targetAAColorRT = [surface->device newTextureWithDescriptor:txDesc];
		surface->targetAAColorRT.label = @"targetAAColorRT";
	}
}



extern "C" void  DestroyRenderingSurfaceMTL(UnityDisplaySurfaceMTL* surface)
{
	surface->targetColorRT = nil;
	surface->targetAAColorRT = nil;

	if(surface->cvTextureCacheTexture)	CFRelease(surface->cvTextureCacheTexture);
	if(surface->cvPixelBuffer)			CFRelease(surface->cvPixelBuffer);
	if(surface->cvTextureCache)			CFRelease(surface->cvTextureCache);
	surface->cvTextureCache = 0;
}

extern "C" void CreateSharedDepthbufferMTL(UnityDisplaySurfaceMTL* surface)
{
	DestroySharedDepthbufferMTL(surface);

	MTLTextureDescriptor* depthTexDesc = [MTLTextureDescriptorClass() texture2DDescriptorWithPixelFormat:MTLPixelFormatDepth32Float width:surface->targetW height:surface->targetH mipmapped:NO];
	if(surface->msaaSamples > 1)
	{
		depthTexDesc.textureType = MTLTextureType2DMultisample;
		depthTexDesc.sampleCount = surface->msaaSamples;
	}
	surface->depthRB = [surface->device newTextureWithDescriptor:depthTexDesc];

	MTLTextureDescriptor* stencilTexDesc = [MTLTextureDescriptorClass() texture2DDescriptorWithPixelFormat:MTLPixelFormatStencil8 width:surface->targetW height:surface->targetH mipmapped:NO];
	if(surface->msaaSamples > 1)
	{
		stencilTexDesc.textureType = MTLTextureType2DMultisample;
		stencilTexDesc.sampleCount = surface->msaaSamples;
	}
	surface->stencilRB = [surface->device newTextureWithDescriptor:stencilTexDesc];
}

extern "C" void DestroySharedDepthbufferMTL(UnityDisplaySurfaceMTL* surface)
{
	surface->depthRB = nil;
	surface->stencilRB = nil;
}

extern "C" void CreateUnityRenderBuffersMTL(UnityDisplaySurfaceMTL* surface)
{
	surface->systemColorBuffer = 0;
	surface->systemDepthBuffer = 0;

	surface->unityColorBuffer = 0;
	surface->unityDepthBuffer = 0;

	UnityRenderBufferDesc sys_desc = { surface->systemW, surface->systemH, 1, 1 };
	UnityRenderBufferDesc tgt_desc	= { surface->targetW, surface->targetH, (unsigned int)surface->msaaSamples, 1 };

	// color texture we will be updating on every frame
	surface->systemColorBuffer = UnityCreateDummySurface(apiMetal, surface->systemColorBuffer, true, &sys_desc);

	if(surface->targetColorRT || surface->targetAAColorRT)
	{
		// in case of resolving directly to backbuffer we will re-update surface on frame start
		if(surface->targetAAColorRT)
			surface->unityColorBuffer = UnityCreateExternalColorSurfaceMTL(surface->unityColorBuffer, surface->targetAAColorRT, surface->targetColorRT, &tgt_desc);
		else
			surface->unityColorBuffer = UnityCreateExternalColorSurfaceMTL(surface->unityColorBuffer, surface->targetColorRT, nil, &tgt_desc);
	}

	// decide where we want actual depth attached
	if(surface->targetColorRT || surface->targetAAColorRT)
	{
		surface->systemDepthBuffer	= UnityCreateDummySurface(apiMetal, surface->systemDepthBuffer, false, &sys_desc);
		surface->unityDepthBuffer	= UnityCreateExternalDepthSurfaceMTL(surface->unityDepthBuffer, surface->depthRB, surface->stencilRB, &tgt_desc);
	}
	else
	{
		surface->systemDepthBuffer	= UnityCreateExternalDepthSurfaceMTL(surface->systemDepthBuffer, surface->depthRB, surface->stencilRB, &tgt_desc);
		surface->unityDepthBuffer	= UnityCreateDummySurface(apiMetal, surface->unityDepthBuffer, false, &tgt_desc);
	}
}


extern "C" void DestroySystemRenderingSurfaceMTL(UnityDisplaySurfaceMTL* surface)
{
	surface->systemColorRB = nil;
}


extern "C" void DestroyUnityRenderBuffersMTL(UnityDisplaySurfaceMTL* surface)
{
	// TODO: unify gles/metal impl (they are same)
	if(surface->unityColorBuffer)	UnityDestroyExternalSurface(surface->api, surface->unityColorBuffer);
	if(surface->systemColorBuffer)	UnityDestroyExternalSurface(surface->api, surface->systemColorBuffer);
	surface->unityColorBuffer = surface->systemColorBuffer = 0;

	if(surface->unityDepthBuffer)	UnityDestroyExternalSurface(surface->api, surface->unityDepthBuffer);
	if(surface->systemDepthBuffer)	UnityDestroyExternalSurface(surface->api, surface->systemDepthBuffer);
	surface->unityDepthBuffer = surface->systemDepthBuffer = 0;
}

static int s_MetalFrameCounter = 0;


extern "C" void PreparePresentMTL(UnityDisplaySurfaceMTL* surface)
{
	if(surface->allowScreenshot && UnityIsCaptureScreenshotRequested())
	{
		if(surface->unityColorBuffer && surface->unityDepthBuffer)
			UnitySetFBOMetal(surface->unityColorBuffer, surface->unityDepthBuffer);
		else
			UnitySetFBOMetal(surface->systemColorBuffer, surface->systemDepthBuffer);
		UnityCaptureScreenshot();
	}

	if(surface->targetColorRT)
	{
		assert(surface->systemColorBuffer != 0 && surface->systemDepthBuffer != 0);
		UnitySetAsDefaultFBOMetal(surface->systemColorBuffer, surface->systemDepthBuffer);
		UnitySetFBOMetal(surface->systemColorBuffer, surface->systemDepthBuffer);
		UnityBlitToSystemFBOMetal(surface->targetColorRT, surface->targetW, surface->targetH, surface->systemW, surface->systemH);
	}

	id<MTLCommandBuffer> commandBuffer = UnityPrepareEndMetalFrame();
	if(commandBuffer != nil)
	{
		int frameNumber = s_MetalFrameCounter;

		[commandBuffer presentDrawable:surface->drawable];
		[commandBuffer addCompletedHandler:^(id<MTLCommandBuffer> buffer)
		{
			UnityFinishedMetalFrame (frameNumber);
		}];
		[commandBuffer commit];

		commandBuffer = nil;
	}

	surface->systemColorRB = nil;
	surface->drawable = nil;

	APP_CONTROLLER_RENDER_PLUGIN_METHOD(onFrameResolved);
}

extern "C" void PrepareRenderingMTL(UnityDisplaySurfaceMTL* surface)
{
	// in case of skipping present we want to nullify prev drawable explicitly to poke ARC
	// TODO: revisit our surface recreation
	surface->drawable		= nil;
	surface->drawable		= [surface->layer nextDrawable];
	surface->systemColorRB	= [surface->drawable texture];

	{
		UnityRenderBufferDesc sys_desc = { surface->systemW, surface->systemH, 1, 1};
		UnityRenderBufferDesc tgt_desc = { surface->targetW, surface->targetH, (unsigned int)surface->msaaSamples, 1};

		surface->systemColorBuffer = UnityCreateExternalColorSurfaceMTL(surface->systemColorBuffer, surface->systemColorRB, nil, &sys_desc);

		if(surface->targetAAColorRT && !surface->targetColorRT)
			surface->unityColorBuffer = UnityCreateExternalColorSurfaceMTL(surface->unityColorBuffer, surface->targetAAColorRT, surface->systemColorRB, &tgt_desc);
	}

	const int frameNumber = OSAtomicAdd32Barrier (1, &s_MetalFrameCounter);

	if(surface->unityColorBuffer && surface->unityDepthBuffer)
		UnityStartMetalFrame(surface->unityColorBuffer, surface->unityDepthBuffer, frameNumber);
	else
		UnityStartMetalFrame(surface->systemColorBuffer, surface->systemDepthBuffer, frameNumber);
}

extern "C" void TeardownRenderingMTL(UnityDisplaySurfaceMTL* surface)
{
	surface->systemColorRB	= nil;
	surface->drawable		= nil;
}



#else

extern "C" void InitMetal()													{}
extern "C" void CreateSystemRenderingSurfaceMTL(UnityDisplaySurfaceMTL*)	{}
extern "C" void CreateRenderingSurfaceMTL(UnityDisplaySurfaceMTL*)			{}
extern "C" void DestroyRenderingSurfaceMTL(UnityDisplaySurfaceMTL*)			{}
extern "C" void CreateSharedDepthbufferMTL(UnityDisplaySurfaceMTL*)			{}
extern "C" void DestroySharedDepthbufferMTL(UnityDisplaySurfaceMTL*)		{}
extern "C" void CreateUnityRenderBuffersMTL(UnityDisplaySurfaceMTL*)		{}
extern "C" void DestroySystemRenderingSurfaceMTL(UnityDisplaySurfaceMTL*)	{}
extern "C" void DestroyUnityRenderBuffersMTL(UnityDisplaySurfaceMTL*)		{}
extern "C" void PreparePresentMTL(UnityDisplaySurfaceMTL*)					{}
extern "C" void PrepareRenderingMTL(UnityDisplaySurfaceMTL*)				{}
extern "C" void TeardownRenderingMTL(UnityDisplaySurfaceMTL*)				{}

#endif
