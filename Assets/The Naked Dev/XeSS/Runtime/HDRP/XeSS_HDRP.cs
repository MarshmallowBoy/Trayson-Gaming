using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace TND.XeSS
{
    public class XeSS_HDRP : XeSS_Base
    {
        internal int displayWidth, displayHeight;
        internal int renderWidth, renderHeight;

        /// <summary>
        /// If set to true <see cref="XeSSRenderPass"/> will reinitialize hte xess context on the next frame
        /// Therefore this boolean should only be set to true in this script
        /// </summary>
        internal bool requiresReintialization = false;

        private Volume _postProcessVolume;
        private XeSSRenderPass _renderPass;

        private HDCamera _hdCamera, _prevHDCamera;

        private float _previousScaleFactor;

        private bool m_usePhysicalProperties;

        protected override void InitializeXeSS()
        {
            base.InitializeXeSS();

            SetupResolution();

            if (!_initialized)
            {
                _postProcessVolume = gameObject.AddComponent<Volume>();
                _postProcessVolume.hideFlags = HideFlags.HideInInspector;
                _postProcessVolume.isGlobal = true;
                _renderPass = _postProcessVolume.profile.Add<XeSSRenderPass>();
                _renderPass.enable.value = true;
                _renderPass.enable.Override(true);

                RenderPipelineManager.beginContextRendering += OnBeginContextRendering;
                RenderPipelineManager.endContextRendering += OnEndContextRendering;
            }

            enabled = CheckHDRPSetup();
        }

        private bool CheckHDRPSetup()
        {
#if !TND_HDRP_EDITEDSOURCE
            Debug.LogError("[FSR 3] HDRP Source edits are not confirmed, please make sure the edits are made correctly and press the 'Confirmation' button on the Upscaler Component");
            return false;
#elif UNITY_EDITOR
            try
            {
                RenderPipelineGlobalSettings _hdRenderPipeline = GraphicsSettings.GetSettingsForRenderPipeline<HDRenderPipeline>();
                string _hdRenderPipelineName = "";
                if (_hdRenderPipeline != null)
                {
                    _hdRenderPipelineName = _hdRenderPipeline.name;
                }
                string[] guids = AssetDatabase.FindAssets(_hdRenderPipelineName + " t:HDRenderPipelineGlobalSettings ", null);
                bool containsUpscalerPass = false;

                for (int i = 0; i < guids.Length; i++)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    if (_hdRenderPipeline == AssetDatabase.LoadAssetAtPath(path, typeof(RenderPipelineGlobalSettings)))
                    {
                        if (File.ReadAllText(path).Contains("TND.XeSS.XeSSRenderPass"))
                        {
                            containsUpscalerPass = true;
                            break;
                        }
                    }
                }

                if (!containsUpscalerPass)
                {
                    Debug.LogError("[XeSS] Upscaler has not been added to the 'Before Post Process' in the 'Custom Post Process Orders' of the HDRP Global Settings Asset, please see the Quick Start: HDRP chapter of the documentation");
                    return false;
                }
            }
            catch { }
#endif
            return true;
        }

        private void OnBeginContextRendering(ScriptableRenderContext renderContext, List<Camera> cameras)
        {
      
            GetHDCamera();
            DynamicResolutionHandler.SetDynamicResScaler(SetDynamicResolutionScale, DynamicResScalePolicyType.ReturnsPercentage);

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
        }

        private void OnEndContextRendering(ScriptableRenderContext context, List<Camera> cameras)
        {
            m_mainCamera.usePhysicalProperties = m_usePhysicalProperties;
            if (!m_mainCamera.usePhysicalProperties)
                m_mainCamera.ResetProjectionMatrix();
        }

        protected override void DisableXeSS()
        {
            base.DisableXeSS();

            GraphicsDevice.ReleaseResources();
            RenderPipelineManager.beginContextRendering -= OnBeginContextRendering;
            RenderPipelineManager.endContextRendering -= OnEndContextRendering;

            DynamicResolutionHandler.SetDynamicResScaler(() => { return 100; }, DynamicResScalePolicyType.ReturnsPercentage);

            if (_hdCamera != null)
            {
#if TND_HDRP_EDITEDSOURCE
                _hdCamera.tndUpscalerEnabled = false;
#endif
                _hdCamera = _prevHDCamera = null;
            }

            if (_postProcessVolume)
            {
                _renderPass.Cleanup();
                Destroy(_postProcessVolume);
            }
        }

        private void SetupResolution()
        {
            displayWidth = m_mainCamera.pixelWidth;
            displayHeight = m_mainCamera.pixelHeight;

            requiresReintialization = true;
            SetupScaleFactor();
        }

        private void SetupScaleFactor()
        {
            _previousScaleFactor = _scaleFactor;

            renderWidth = (int)(displayWidth / _scaleFactor);
            renderHeight = (int)(displayHeight / _scaleFactor);

            requiresReintialization = true;
        }

        private float SetDynamicResolutionScale()
        {
            return 100f / _scaleFactor;
        }

        private void GetHDCamera()
        {
            _hdCamera = HDCamera.GetOrCreate(m_mainCamera);

            if (_prevHDCamera != _hdCamera)
            {
                _prevHDCamera = _hdCamera;
#if TND_HDRP_EDITEDSOURCE
                _hdCamera.tndUpscalerEnabled = true;
#endif

                HDAdditionalCameraData additionalCameraData = _hdCamera.camera.GetComponent<HDAdditionalCameraData>();
                additionalCameraData.allowDynamicResolution = true;
                additionalCameraData.volumeLayerMask |= (1 << _hdCamera.camera.gameObject.layer);
            }

        }
    }
}
