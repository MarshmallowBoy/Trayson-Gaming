using System;
using UnityEngine;

namespace TND.XeSS
{
    /// <summary>
    /// XeSS Internal representation of the quality setting, should not changed or used in another context!
    /// </summary>
    internal enum InitQualitySetting
    {
        XESS_QUALITY_SETTING_ULTRA_PERFORMANCE = 100,
        XESS_QUALITY_SETTING_PERFORMANCE = 101,
        XESS_QUALITY_SETTING_BALANCED = 102,
        XESS_QUALITY_SETTING_QUALITY = 103,
        XESS_QUALITY_SETTING_ULTRA_QUALITY = 104,
        XESS_QUALITY_SETTING_ULTRA_QUALITY_PLUS = 105,
        XESS_QUALITY_SETTING_AA = 106,
    }

    [Flags]
    internal enum XeSSInitFlags
    {
        XESS_INIT_FLAG_NONE = 0,
        /** Use motion vectors at target resolution. */
        XESS_INIT_FLAG_HIGH_RES_MV = 1 << 0,
        /** Use inverted (increased precision) depth encoding */
        XESS_INIT_FLAG_INVERTED_DEPTH = 1 << 1,
        /** Use exposure texture to scale input color. */
        XESS_INIT_FLAG_EXPOSURE_SCALE_TEXTURE = 1 << 2,
        /** Use responsive pixel mask texture. */
        XESS_INIT_FLAG_RESPONSIVE_PIXEL_MASK = 1 << 3,
        /** Use velocity in NDC */
        XESS_INIT_FLAG_USE_NDC_VELOCITY = 1 << 4,
        /** Use external descriptor heap */
        XESS_INIT_FLAG_EXTERNAL_DESCRIPTOR_HEAP = 1 << 5,
        /** Disable tonemapping for input and output */
        XESS_INIT_FLAG_LDR_INPUT_COLOR = 1 << 6,
        /** Remove jitter from input velocity*/
        XESS_INIT_FLAG_JITTERED_MV = 1 << 7,
        /** Enable automatic exposure calculation. */
        XESS_INIT_FLAG_ENABLE_AUTOEXPOSURE = 1 << 8
    }

    internal struct XeSSInitParam
    {
        public Vector2Int resolution;
        public InitQualitySetting qualitySetting;
        public XeSSInitFlags flags;
        public float jitterScaleX;
        public float jitterScaleY;
        public float motionVectorScaleX;
        public float motionVectorScaleY;
    }
}
