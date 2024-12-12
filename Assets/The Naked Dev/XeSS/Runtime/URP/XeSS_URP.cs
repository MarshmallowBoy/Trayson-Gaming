using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TND.XeSS
{
    public class XeSS_URP : XeSS_Base
    {
        internal RTHandle colorBuffer;
        internal Texture depthBuffer;
        internal Texture motionVectorBuffer;
        public RTHandle output;

        private List<XeSSRenderFeature> _renderFeatures;
        private UniversalRenderPipelineAsset _universalRenderPipelineAsset;
        private bool m_usePhysicalProperties;

        internal int displayWidth, displayHeight;
        internal int renderWidth, renderHeight;

        private readonly GraphicsFormat _colorGraphicsFormat = GraphicsFormat.B10G11R11_UFloatPack32;

        /// <summary>
        /// If set to true <see cref="XeSSRenderPass"/> will reinitialize hte xess context on the next frame
        /// Therefore this boolean should only be set to true in this script
        /// </summary>
        internal bool requiresReintialization = false;

        private float _previousScaleFactor;

        //Camera Stacking
        private bool _cameraStacking = false;
        private Camera _topCamera;
        private UniversalAdditionalCameraData _cameraData;
        private int _prevCameraStackCount;
        private bool _isBaseCamera;
        private List<XeSS_URP> _prevCameraStack = new List<XeSS_URP>();
        private IntelQuality _prevStackQuality;


        protected override void InitializeXeSS()
        {
            base.InitializeXeSS();
            m_mainCamera.depthTextureMode = DepthTextureMode.Depth | DepthTextureMode.MotionVectors;

            SetupResolution();

            if (!_initialized)
            {
                RenderPipelineManager.beginContextRendering += PreRenderCamera;
                RenderPipelineManager.endContextRendering += PostRenderCamera;
            }

            if (_cameraData == null)
            {
                _cameraData = m_mainCamera.GetUniversalAdditionalCameraData();
                if (_cameraData != null)
                {
                    if (_cameraData.renderType == CameraRenderType.Base)
                    {
                        _isBaseCamera = true;
                        SetupCameraStacking();
                    }
                }
            }
        }

        private void PreRenderCamera(ScriptableRenderContext context, List<Camera> cameras)
        {
  
            if (displayWidth != m_mainCamera.pixelWidth || displayHeight != m_mainCamera.pixelHeight)
            {
                SetupResolution();
            }

            if (_scaleFactor != _previousScaleFactor)
            {
                SetupScaleFactor();
            }

            m_usePhysicalProperties = m_mainCamera.usePhysicalProperties;

            m_mainCamera.nonJitteredProjectionMatrix = m_mainCamera.projectionMatrix;
            m_mainCamera.projectionMatrix = GetJitteredProjectionMatrix(m_mainCamera.projectionMatrix, renderWidth, renderHeight);
            m_mainCamera.useJitteredProjectionMatrixForTransparentRendering = true;

            if (_isBaseCamera && _cameraData != null && _cameraStacking)
            {
                if (_topCamera != _cameraData.cameraStack[_cameraData.cameraStack.Count - 1] || _prevCameraStackCount != _cameraData.cameraStack.Count || _prevStackQuality != IntelQuality)
                {
                    SetupCameraStacking();
                }
            }
        }

        private void PostRenderCamera(ScriptableRenderContext context, List<Camera> cameras)
        {
            m_mainCamera.usePhysicalProperties = m_usePhysicalProperties;
            if (!m_mainCamera.usePhysicalProperties)
                m_mainCamera.ResetProjectionMatrix();
        }

        private void SetupResolution()
        {
            displayWidth = m_mainCamera.pixelWidth;
            displayHeight = m_mainCamera.pixelHeight;

            if (output != null)
            {
                output.Release();
            }
            output = RTHandles.Alloc(displayWidth, displayHeight, enableRandomWrite: true, colorFormat: _colorGraphicsFormat, msaaSamples: MSAASamples.None, name: "XeSS Output");

            requiresReintialization = true;
            SetupScaleFactor();
        }

        private void SetupScaleFactor()
        {
            _previousScaleFactor = _scaleFactor;

            bool containsRenderFeature = true;
            if (_renderFeatures == null)
            {
                containsRenderFeature = GetRenderFeatures();
            }

            renderWidth = (int)(displayWidth / _scaleFactor);
            renderHeight = (int)(displayHeight / _scaleFactor);

#if !UNITY_2022_1_OR_NEWER
            if(colorBuffer != null)
            {
                colorBuffer.Release();
            }

            colorBuffer = RTHandles.Alloc(renderWidth, renderHeight, enableRandomWrite: false, colorFormat: _colorGraphicsFormat, msaaSamples: MSAASamples.None, name: "XeSS INPUT");
#endif

            SetDynamicResolution(_scaleFactor);
            requiresReintialization = true;

            if (!containsRenderFeature)
            {
                Debug.LogError("Current Universal Render Data is missing the 'XeSS Scriptable Render Pass' Rendering Feature");
            }
            else
            {
                foreach (var renderFeature in _renderFeatures)
                {
                    renderFeature.OnSetReference(this);
                    renderFeature.IsEnabled = true;
                }
            }
        }

        protected override void DisableXeSS()
        {
            base.DisableXeSS();

            GraphicsDevice.ReleaseResources();
            RenderPipelineManager.beginContextRendering -= PreRenderCamera;
            RenderPipelineManager.endContextRendering -= PostRenderCamera;

            SetDynamicResolution(1);
            if (_renderFeatures != null)
            {
                foreach (var renderFeature in _renderFeatures)
                {
                    renderFeature.IsEnabled = false;
                }
                _renderFeatures = null;
            }

            if (output != null)
            {
                output.Release();
                output = null;
            }

            if (colorBuffer != null)
            {
                colorBuffer.Release();
                colorBuffer = null;
            }

            CleanupOverlayCameras();
        }

        private bool GetRenderFeatures()
        {
            _renderFeatures = new List<XeSSRenderFeature>();
            _universalRenderPipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

            bool renderFeatureFound = false;

            if (_universalRenderPipelineAsset != null)
            {
#if UNITY_2022_1_OR_NEWER
                _universalRenderPipelineAsset.upscalingFilter = UpscalingFilterSelection.Linear;
#endif
                var type = _universalRenderPipelineAsset.GetType();
                var propertyInfo = type.GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);

                if (propertyInfo != null)
                {
                    var scriptableRenderData = (ScriptableRendererData[])propertyInfo.GetValue(_universalRenderPipelineAsset);

                    if (scriptableRenderData != null && scriptableRenderData.Length > 0)
                    {
                        foreach (var renderData in scriptableRenderData)
                        {
                            foreach (var renderFeature in renderData.rendererFeatures)
                            {
                                XeSSRenderFeature xessFeature = renderFeature as XeSSRenderFeature;

                                if (xessFeature != null)
                                {
                                    _renderFeatures.Add(xessFeature);
                                    renderFeatureFound = true;

                                    //Stop looping the current renderer, we only allow 1 instance per renderer 
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("XeSS: Can't find UniversalRenderPipelineAsset");
            }
            return renderFeatureFound;
        }

        private void SetDynamicResolution(float value)
        {
            if (_universalRenderPipelineAsset != null)
            {
                float renderScale = 1.0f / value;
                _universalRenderPipelineAsset.renderScale = renderScale;
            }
        }

        /// <summary>
        /// Automatically Setup camera stacking
        /// </summary>
        private void SetupCameraStacking()
        {
            _prevCameraStackCount = _cameraData.cameraStack.Count;
            if (_cameraData.renderType == CameraRenderType.Base)
            {
                _isBaseCamera = true;

                _cameraStacking = _cameraData.cameraStack.Count > 0;
                if (_cameraStacking)
                {
                    CleanupOverlayCameras();
                    _prevStackQuality = IntelQuality;

                    _topCamera = _cameraData.cameraStack[_cameraData.cameraStack.Count - 1];

                    for (int i = 0; i < _cameraData.cameraStack.Count; i++)
                    {
                        XeSS_URP stackedCamera = _cameraData.cameraStack[i].gameObject.GetComponent<XeSS_URP>();
                        if (stackedCamera == null)
                        {
                            stackedCamera = _cameraData.cameraStack[i].gameObject.AddComponent<XeSS_URP>();
                        }
                        _prevCameraStack.Add(_cameraData.cameraStack[i].gameObject.GetComponent<XeSS_URP>());

                        stackedCamera._cameraStacking = true;
                        stackedCamera._topCamera = _topCamera;

                        stackedCamera.OnSetQuality(IntelQuality);

                        stackedCamera.sharpness = sharpness;
                        stackedCamera._antiGhosting = _antiGhosting;
                    }
                }
            }
        }

        private void CleanupOverlayCameras()
        {
            for (int i = 0; i < _prevCameraStack.Count; i++)
            {
                if (!_prevCameraStack[i]._isBaseCamera)
                    DestroyImmediate(_prevCameraStack[i]);
            }
            _prevCameraStack = new List<XeSS_URP>();
        }
    }
}
