using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace TND.XeSS
{
    public class XeSSRenderPass : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [HideInInspector]
        public BoolParameter enable = new BoolParameter(false);
        public bool IsActive() => enable.value;

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.BeforePostProcess;

        private readonly int _depthTexturePropertyID = Shader.PropertyToID("_CameraDepthTexture");
        private readonly int _motionTexturePropertyID = Shader.PropertyToID("_CameraMotionVectorsTexture");

        private XeSS_HDRP m_upscaler;

        private Material _sharpeningMaterial;
        private readonly string _sharpeningShader = "Hidden/Shader/Rcas_HDRP";
        private readonly int _sharpness = Shader.PropertyToID("Sharpness");
        private RTHandle _sharpeningInput;

        public override void Setup()
        {
            _sharpeningMaterial = CoreUtils.CreateEngineMaterial(_sharpeningShader);

            _sharpeningInput = RTHandles.Alloc(scaleFactor: Vector2.one, filterMode: FilterMode.Point, wrapMode: TextureWrapMode.Clamp, colorFormat: GraphicsFormat.B10G11R11_UFloatPack32, dimension: TextureDimension.Tex2DArray, enableRandomWrite: true, useDynamicScale: false);
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif

            if (!IsActive())
            {
                cmd.Blit(source, destination, 0, 0);
                return;
            }

            if (m_upscaler == null && !camera.camera.TryGetComponent(out m_upscaler))
            {
                cmd.Blit(source, destination, 0, 0);
                return;
            }

            if (camera.camera.cameraType != CameraType.Game)
            {
                cmd.Blit(source, destination, 0, 0);
                return;
            }

            if (m_upscaler.requiresReintialization)
            {
                var initParam = new XeSSInitParam()
                {
                    resolution = new Vector2Int(m_upscaler.displayWidth, m_upscaler.displayHeight),
                    qualitySetting = GraphicsDevice.QualityModeToInitSetting(m_upscaler.IntelQuality),
                    flags = XeSSInitFlags.XESS_INIT_FLAG_INVERTED_DEPTH | XeSSInitFlags.XESS_INIT_FLAG_JITTERED_MV | XeSSInitFlags.XESS_INIT_FLAG_USE_NDC_VELOCITY,
                    jitterScaleX = -1f,
                    jitterScaleY = -1f,
                    motionVectorScaleX = -2f,
                    motionVectorScaleY = 2f,
                };

                m_upscaler.GraphicsDevice.InitXeSS(cmd, initParam);
                m_upscaler.requiresReintialization = false;
            }

            Texture depthTexture = Shader.GetGlobalTexture(_depthTexturePropertyID);
            Texture mvTexture = Shader.GetGlobalTexture(_motionTexturePropertyID);

            if (!depthTexture || !mvTexture)
            {
                CoreUtils.SetRenderTarget(cmd, destination);
                cmd.SetViewport(new Rect(0, 0, destination.rt.width, destination.rt.height));
                Blitter.BlitCameraTexture(cmd, source, destination);

                return;
            }

            var executeParam = new XeSSExecuteParam()
            {
                colorInput = source.rt.GetNativeTexturePtr(),
                depth = depthTexture ? depthTexture.GetNativeTexturePtr() : IntPtr.Zero,
                motionVectors = mvTexture ? mvTexture.GetNativeTexturePtr() : IntPtr.Zero,
                output = m_upscaler.sharpening ? _sharpeningInput.rt.GetNativeTexturePtr() : destination.rt.GetNativeTexturePtr(),

                inputWidth = (uint)m_upscaler.renderWidth,
                inputHeight = (uint)m_upscaler.renderHeight,
                jitterOffsetX = m_upscaler.jitterX,
                jitterOffsetY = -m_upscaler.jitterY,
                exposureScale = 1.0f,
                resetHistory = false,
            };
            m_upscaler.GraphicsDevice.ExecuteXeSS(cmd, executeParam);

            if (m_upscaler.sharpening)
            {
                _sharpeningMaterial.SetFloat(_sharpness, 1 - Mathf.Clamp01(m_upscaler.sharpness));

                CoreUtils.SetRenderTarget(cmd, destination);
                cmd.SetViewport(new Rect(0, 0, destination.rt.width, destination.rt.height)); //Scale the viewport to full render size
                Blitter.BlitTexture(cmd, _sharpeningInput, Vector2.one, _sharpeningMaterial, 0);
            }
        }

        public override void Cleanup()
        {
            _sharpeningInput?.Release();
            CoreUtils.Destroy(_sharpeningMaterial);
        }
    }
}
