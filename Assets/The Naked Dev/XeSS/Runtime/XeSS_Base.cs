using UnityEngine;

namespace TND.XeSS
{
    public enum XeSS_Quality
    {
        Off = 0,
        NativeAA = 1,
        UltraQuality = 2,
        Quality = 3,
        Balanced = 4,
        Performance = 5,
        UltraPerformance = 6,
    }

    public enum IntelQuality
    {
        Off,
        NativeAntiAliasing,
        UltraQualityPlus,
        UltraQuality,
        Quality,
        Balanced,
        Performance,
        UltraPerformance
    }

    [RequireComponent(typeof(Camera))]
    public abstract class XeSS_Base : MonoBehaviour
    {
        [SerializeField]
        private XeSS_Quality _xessQuality = XeSS_Quality.Balanced;

        [SerializeField, Range(0, 1.0f)]
        protected float _antiGhosting = 0.1f;

        public bool sharpening = true;
        [Range(0, 1.0f)]
        public float sharpness = 0.5f;

        private XeSS_Quality _previousXessQuality;
        protected float _scaleFactor;

        protected bool _initialized = false;

        public Camera m_mainCamera;

        internal float jitterX, jitterY;
        private int _frameIndex = 0;

        private GraphicsDevice _graphicsDevice;
        internal GraphicsDevice GraphicsDevice
        {
            get
            {
                if (_graphicsDevice == null)
                {
                    _graphicsDevice = new GraphicsDevice();
                }
                return _graphicsDevice;
            }
        }

        /// <summary>
        /// Internal representation of the quality setting used to communicate with the unity plugin
        /// </summary>
        internal IntelQuality IntelQuality
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the current quality setting.
        /// </summary>
        public XeSS_Quality OnGetQuality()
        {
            return _xessQuality;
        }

        public float OnGetAntiGhosting()
        {
            return _antiGhosting;
        }


        /// <summary>
        /// Checks wether XeSS is compatible using the current build settings
        /// Returns the XeSSResult returned by the XeSS plugin. Can be used to handle specific errors or the warning that the user has old drivers installed
        /// </summary>
        public bool OnIsSupported()
        {
            return IsSupported(out _);
        }

        public bool IsSupported(out XeSSResult result)
        {
            result = GraphicsDevice.CreateXeSSContext();
            return result >= 0;
        }

        public void OnSetQuality(XeSS_Quality quality)
        {
            _xessQuality = _previousXessQuality = quality;
            (_scaleFactor, IntelQuality) = GetScalingFromQualityMode(quality);

            OnQualityChanged();
        }

        public void OnSetQuality(IntelQuality quality)
        {
            IntelQuality = quality;
            _scaleFactor = GetScalingFromQualityMode(quality);

            OnQualityChanged();
        }

        public void OnSetAntiGhosting(float antiGhosting)
        {
            _antiGhosting = antiGhosting;
        }

        private void OnQualityChanged()
        {
            if (IntelQuality == IntelQuality.Off)
            {
                if (_initialized)
                {
                    DisableXeSS();
                }
            }
            else
            {
                Initialize();
            }
        }

        protected virtual void Initialize()
        {
            if (_initialized || !Application.isPlaying)
            {
                return;
            }

            if (IsSupported(out _))
            {
                InitializeXeSS();
                _initialized = true;
            }
            else
            {
                DisableXeSS();
                Debug.LogWarning($"XeSS is not supported");
                enabled = false;
            }
        }

        protected virtual void InitializeXeSS()
        {
            m_mainCamera = GetComponent<Camera>();
            m_mainCamera.allowMSAA = false;
        }

        protected virtual void OnEnable()
        {
            OnSetQuality(_xessQuality);
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            //Detection for if qualityLevel is changed outside of OnSetQualityLevel
            if (_previousXessQuality != _xessQuality)
            {
                OnSetQuality(_xessQuality);
            }
#endif
        }

        protected virtual void OnDisable()
        {
            DisableXeSS();
        }

        protected virtual void OnDestroy()
        {
            DisableXeSS();
        }

        protected virtual void DisableXeSS()
        {
            _initialized = false;
        }

        protected static float GetHaltonValue(int index, int radix)
        {
            float result = 0f;
            float fraction = 1f / radix;

            while (index > 0)
            {
                result += (index % radix) * fraction;

                index /= radix;
                fraction /= radix;
            }

            return result;
        }

        internal Matrix4x4 GetJitteredProjectionMatrix(Matrix4x4 projMatrix, float width, float height)
        {
            return GetJitteredProjectionMatrix(projMatrix, width, height, _antiGhosting, _scaleFactor, ref jitterX, ref jitterY, ref _frameIndex);
        }

        static internal Matrix4x4 GetJitteredProjectionMatrix(Matrix4x4 projMatrix, float width, float height, float antiGhosting, float scaleFactor, ref float jitterX, ref float jitterY, ref int frameIndex)
        {
            jitterX = GetHaltonValue((frameIndex & 1023) + 1, 2) - 0.5f;
            jitterY = GetHaltonValue((frameIndex & 1023) + 1, 3) - 0.5f;

            if (antiGhosting > 0.0f)
            {
                jitterX += Random.Range(-0.1f * antiGhosting, 0.1f * antiGhosting);
                jitterY += Random.Range(-0.1f * antiGhosting, 0.1f * antiGhosting);

                jitterX = Mathf.Clamp(jitterX, -0.5f, 0.5f);
                jitterY = Mathf.Clamp(jitterY, -0.5f, 0.5f);
            }

            if (++frameIndex >= 8 * (scaleFactor * scaleFactor))
            {
                frameIndex = 0;
            }

            projMatrix.m02 += jitterX * 2.0f / width;
            projMatrix.m12 -= jitterY * 2.0f / height;

            return projMatrix;
        }

        static internal (float, IntelQuality) GetScalingFromQualityMode(XeSS_Quality quality)
        {
            switch (quality)
            {
                case XeSS_Quality.Off:
                    return (1.0f, IntelQuality.Off);
                case XeSS_Quality.NativeAA:
                    return (1.0f, IntelQuality.NativeAntiAliasing);
                case XeSS_Quality.UltraQuality:
                    return (1.2f, IntelQuality.UltraQualityPlus);
                case XeSS_Quality.Quality:
                    return (1.5f, IntelQuality.UltraQuality);
                case XeSS_Quality.Balanced:
                    return (1.7f, IntelQuality.Quality);
                case XeSS_Quality.Performance:
                    return (2.0f, IntelQuality.Balanced);
                case XeSS_Quality.UltraPerformance:
                    return (3.0f, IntelQuality.UltraPerformance);
                default:
                    Debug.LogError($"[XeSS Upscaler]: Quality Level {quality} is not implemented, defaulting to balanced");
                    break;
            }

            return (1.7f, IntelQuality.Quality);
        }

        static internal float GetScalingFromQualityMode(IntelQuality quality)
        {
            switch (quality)
            {
                case IntelQuality.Off:
                case IntelQuality.NativeAntiAliasing:
                    return 1.0f;
                case IntelQuality.UltraQualityPlus:
                    return 1.3f;
                case IntelQuality.UltraQuality:
                    return 1.5f;
                case IntelQuality.Quality:
                    return 1.7f;
                case IntelQuality.Balanced:
                    return 2.0f;
                case IntelQuality.Performance:
                    return 2.3f;
                case IntelQuality.UltraPerformance:
                    return 3.0f;
                default:
                    Debug.LogError($"[XeSS Upscaler]: Quality Level {quality} is not implemented, defaulting to quality");
                    break;
            }

            return 1.7f;
        }
    }
}
