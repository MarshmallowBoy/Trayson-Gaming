using System;
using System.Runtime.InteropServices;

namespace TND.XeSS
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct XeSSExecuteParam
    {
        public IntPtr colorInput;
        public IntPtr depth;
        public IntPtr motionVectors;
        public IntPtr exposureTexture;
        public IntPtr responsivePixelMask;
        public IntPtr output;

        public uint inputWidth;
        public uint inputHeight;
        public float jitterOffsetX;
        public float jitterOffsetY;
        public float exposureScale;
        public bool resetHistory;
    }
}
