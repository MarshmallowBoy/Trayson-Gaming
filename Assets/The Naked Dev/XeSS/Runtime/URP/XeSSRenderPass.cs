using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;


#if UNITY_6000_0_OR_NEWER
using UnityEngine.Rendering.RenderGraphModule;
#endif

namespace TND.XeSS
{
    public class XeSSRenderPass : ScriptableRenderPass
    {
        private XeSS_URP m_upscaler;
        private const string blitPass = "[XeSS] Upscaler";

        private Material _sharpeningMaterial;
        private readonly int _sharpness = Shader.PropertyToID("Sharpness");
        private readonly string _sharpeningShader = "Hidden/RcasFinalBlit_URP";

        //Legacy
        private Vector4 _scaleBias;

        public XeSSRenderPass(XeSS_URP _upscaler, bool usingRenderGraph)
        {
            m_upscaler = _upscaler;
            renderPassEvent = usingRenderGraph ? RenderPassEvent.AfterRenderingPostProcessing : RenderPassEvent.AfterRendering + 2;

            _sharpeningMaterial = CoreUtils.CreateEngineMaterial(_sharpeningShader);
            _scaleBias = SystemInfo.graphicsUVStartsAtTop ? new Vector4(1, -1, 0, 1) : Vector4.one;
        }

        #region Unity 6

#if UNITY_2023_3_OR_NEWER
        private class PassData
        {
            public TextureHandle Source;
            public TextureHandle Depth;
            public TextureHandle MotionVector;
            public TextureHandle Destination;
            public Rect PixelRect;
        }

        private const string _upscaledTextureName = "_XeSS_UpscaledTexture";

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            // Setting up the render pass in RenderGraph
            using (var builder = renderGraph.AddUnsafePass<PassData>(blitPass, out var passData))
            {
                var cameraData = frameData.Get<UniversalCameraData>();
                var resourceData = frameData.Get<UniversalResourceData>();

                RenderTextureDescriptor upscaledDesc = cameraData.cameraTargetDescriptor;
                upscaledDesc.depthBufferBits = 0;
                upscaledDesc.width = m_upscaler.displayWidth;
                upscaledDesc.height = m_upscaler.displayHeight;

                TextureHandle upscaled = UniversalRenderer.CreateRenderGraphTexture(
                    renderGraph,
                    upscaledDesc,
                    _upscaledTextureName,
                    false
                );

                passData.Source = resourceData.activeColorTexture;
                passData.Depth = resourceData.activeDepthTexture;
                passData.MotionVector = resourceData.motionVectorColor;
                passData.Destination = upscaled;
                passData.PixelRect = cameraData.camera.pixelRect;

                builder.UseTexture(passData.Source, AccessFlags.Read);
                builder.UseTexture(passData.Depth, AccessFlags.Read);
                builder.UseTexture(passData.MotionVector, AccessFlags.Read);
                builder.UseTexture(passData.Destination, AccessFlags.Write);

                builder.AllowPassCulling(false);

                resourceData.cameraColor = upscaled;
                builder.SetRenderFunc((PassData data, UnsafeGraphContext context) => ExecutePass(data, context));
            }
        }

        private void ExecutePass(PassData data, UnsafeGraphContext context)
        {
            CommandBuffer unsafeCmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);

            m_upscaler.colorBuffer = data.Source;
            m_upscaler.depthBuffer = data.Depth;
            m_upscaler.motionVectorBuffer = data.MotionVector;

            if (!m_upscaler.depthBuffer || !m_upscaler.motionVectorBuffer)
            {
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

                m_upscaler.GraphicsDevice.InitXeSS(unsafeCmd, initParam);
                m_upscaler.requiresReintialization = false;
            }

            var executeParam = new XeSSExecuteParam()
            {
                colorInput = m_upscaler.colorBuffer.rt.GetNativeTexturePtr(),
                depth = m_upscaler.depthBuffer ? m_upscaler.depthBuffer.GetNativeTexturePtr() : IntPtr.Zero,
                motionVectors = m_upscaler.motionVectorBuffer ? m_upscaler.motionVectorBuffer.GetNativeTexturePtr() : IntPtr.Zero,
                output = m_upscaler.output.rt.GetNativeTexturePtr(),
                inputWidth = (uint)m_upscaler.renderWidth,
                inputHeight = (uint)m_upscaler.renderHeight,
                jitterOffsetX = m_upscaler.jitterX,
                jitterOffsetY = -m_upscaler.jitterY,
                exposureScale = 1.0f,
                resetHistory = false,
            };
            m_upscaler.GraphicsDevice.ExecuteXeSS(unsafeCmd, executeParam);


            CoreUtils.SetRenderTarget(unsafeCmd, data.Destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, ClearFlag.None, Color.clear);
            unsafeCmd.SetViewport(data.PixelRect);

            if (m_upscaler.sharpening)
            {
                unsafeCmd.SetGlobalFloat(_sharpness, 1 - Mathf.Clamp01(m_upscaler.sharpness));
                Blitter.BlitTexture(unsafeCmd, m_upscaler.output, new Vector4(1, 1, 0, 0), _sharpeningMaterial, 0);
            }
            else
            {
                Blitter.BlitTexture(unsafeCmd, m_upscaler.output, new Vector4(1, 1, 0, 0), 0, false);
            }
        }
#endif

        #endregion

        #region Unity Legacy
#if UNITY_6000_0_OR_NEWER
        [Obsolete]
#endif
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(blitPass);

            CoreUtils.SetRenderTarget(cmd, BuiltinRenderTextureType.CameraTarget, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, ClearFlag.None, Color.clear);
            cmd.SetViewport(renderingData.cameraData.camera.pixelRect);

            if (renderingData.cameraData.camera.targetTexture != null)
            {
                _scaleBias = Vector2.one;
            }

            if (m_upscaler.sharpening)
            {
                cmd.SetGlobalFloat(_sharpness, 1 - Mathf.Clamp01(m_upscaler.sharpness));
                Blitter.BlitTexture(cmd, m_upscaler.output, _scaleBias, _sharpeningMaterial, 0);
            }
            else
            {
                Blitter.BlitTexture(cmd, m_upscaler.output, _scaleBias, 0, false);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    public class XeSSBufferPass : ScriptableRenderPass
    {
        private XeSS_URP m_upscaler;

        private readonly int _depthTexturePropertyID = Shader.PropertyToID("_CameraDepthTexture");
        private readonly int _motionTexturePropertyID = Shader.PropertyToID("_MotionVectorTexture");
        private const string blitPass = "[XeSS] Upscaler";

        public XeSSBufferPass(XeSS_URP _upscaler)
        {
            m_upscaler = _upscaler;

            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            ConfigureInput(ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Motion);
        }

        // The actual execution of the pass. This is where custom rendering occurs.
#if UNITY_6000_0_OR_NEWER
        [Obsolete]
#endif
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(blitPass);

#if UNITY_2022_1_OR_NEWER
            m_upscaler.colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;
            m_upscaler.depthBuffer = Shader.GetGlobalTexture(_depthTexturePropertyID);
            m_upscaler.motionVectorBuffer = Shader.GetGlobalTexture(_motionTexturePropertyID);
#else
            Blit(cmd, renderingData.cameraData.renderer.cameraColorTarget, m_upscaler.colorBuffer);
            m_upscaler.depthBuffer = Shader.GetGlobalTexture(_depthTexturePropertyID);
            m_upscaler.motionVectorBuffer = Shader.GetGlobalTexture(_motionTexturePropertyID);
#endif

            if (!m_upscaler.depthBuffer || !m_upscaler.motionVectorBuffer)
            {
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

            var executeParam = new XeSSExecuteParam()
            {
                colorInput = m_upscaler.colorBuffer.rt.GetNativeTexturePtr(),
                depth = m_upscaler.depthBuffer ? m_upscaler.depthBuffer.GetNativeTexturePtr() : IntPtr.Zero,
                motionVectors = m_upscaler.motionVectorBuffer ? m_upscaler.motionVectorBuffer.GetNativeTexturePtr() : IntPtr.Zero,
                output = m_upscaler.output.rt.GetNativeTexturePtr(),
                inputWidth = (uint)m_upscaler.renderWidth,
                inputHeight = (uint)m_upscaler.renderHeight,
                jitterOffsetX = m_upscaler.jitterX,
                jitterOffsetY = -m_upscaler.jitterY,
                exposureScale = 1.0f,
                resetHistory = false,
            };
            m_upscaler.GraphicsDevice.ExecuteXeSS(cmd, executeParam);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        #endregion
    }
}
