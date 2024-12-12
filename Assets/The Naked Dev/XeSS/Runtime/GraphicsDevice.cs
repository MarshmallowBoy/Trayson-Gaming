using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

[assembly: InternalsVisibleTo("com.thenakeddev.xess.Runtime.URP")]
[assembly: InternalsVisibleTo("com.thenakeddev.xess.Runtime.HDRP")]
[assembly: InternalsVisibleTo("Unity.Postprocessing.Runtime")]
namespace TND.XeSS
{
    [StructLayout(LayoutKind.Sequential)]
    public struct XeSSVersion
    {
        /// <summary>
        /// A major version increment indicates a new API and potentially a break in functionality.
        /// </summary>
        public ushort major;
        /// <summary>
        /// A minor version increment indicates incremental changes such as optional inputs or flags.This does not break existing functionality.
        /// </summary>
        public ushort minor;
        /// <summary>
        /// A patch version increment may include performance or quality tweaks or fixes for known issues.
        /// There's no change in the interfaces.
        /// Versions beyond 90 used for development builds to change the interface for the next release.
        /// </summary>
        public ushort patch;
        /// <summary>
        /// Reserved for future use by XeSS.
        /// </summary>
        public ushort reserved;
    }

    public class GraphicsDevice
    {
        private readonly IntPtr _initParamPtr;
        private readonly IntPtr _executeParamPtr;

        private bool _contextCreated = false;
        private XeSSResult _contextResult;

        private const SyncMethod _textureSyncMethod = SyncMethod.Fence;
        private const SyncMethod _copyBackSyncMethod = SyncMethod.QueryOnly;

        public GraphicsDevice()
        {
            _initParamPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(XeSSInitParam)));
            _executeParamPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(XeSSExecuteParam)));
        }

        /// <summary>
        /// Gets the XeSS version. This is baked into the XeSS SDK release.
        /// </summary>
        /// <returns></returns>
        public XeSSVersion GetXeSSVersion()
        {
            return XUP_OnGetXeSSVersion();
        }

        /// <summary>
        /// Gets the version of the loaded Intel XeFX library.
        /// When running on Intel platforms this function will return the version of the loaded library
        /// For other platforms 0.0.0 will be returned
        /// </summary>
        public XeSSVersion GetIntelXeFXVersion()
        {
            if(!_contextCreated)
            {
                CreateXeSSContext();
            }

            return XUP_OnGetIntelXeFXVersion();
        }

        /// <summary>
        /// Creates the XeSS context in the native plugin, used to communicate with the plugin
        /// </summary>
        /// <returns>Result code returned by XeSS</returns>
        internal XeSSResult CreateXeSSContext()
        {
            if (!_contextCreated)
            {
                _contextResult = XUP_OnInitializeContext();
                _contextCreated = true;
            }

            return _contextResult;
        }

        internal XeSSResult ForceCreateXeSSContext()
        {
            _contextCreated = true;
            return XUP_OnInitializeContext();
        }

        internal void InitXeSS(CommandBuffer cmd, XeSSInitParam initParam)
        {
            if (!_contextCreated)
            {
                Debug.LogError("[XeSS Graphics Device] context needs to be created before trying to initialize XeSS");
                return;
            }

            XUP_OnSetDx12onDx11SyncMethods(_textureSyncMethod, _copyBackSyncMethod);

            Marshal.StructureToPtr(initParam, _initParamPtr, true);
            cmd.IssuePluginEventAndData(XUP_GetInitEvent(), 0, _initParamPtr);
        }

        internal void ExecuteXeSS(CommandBuffer cmd, XeSSExecuteParam executeParam)
        {
            if (!_contextCreated)
            {
                Debug.LogError("[XeSS Graphics Device] context needs to be created before trying to execute XeSS");
                return;
            }

            Marshal.StructureToPtr(executeParam, _executeParamPtr, true);
            cmd.IssuePluginEventAndData(XUP_GetExecuteEvent(), 347, _executeParamPtr);
        }

        internal void ReleaseResources()
        {
            XUP_OnReleaseResources();
            _contextCreated = false;
        }

        internal static InitQualitySetting QualityModeToInitSetting(IntelQuality quality)
        {
            switch (quality)
            {
                case IntelQuality.Off:
                case IntelQuality.NativeAntiAliasing:
                    return InitQualitySetting.XESS_QUALITY_SETTING_AA;
                case IntelQuality.UltraQualityPlus:
                    return InitQualitySetting.XESS_QUALITY_SETTING_ULTRA_QUALITY_PLUS;
                case IntelQuality.UltraQuality:
                    return InitQualitySetting.XESS_QUALITY_SETTING_ULTRA_QUALITY;
                case IntelQuality.Quality:
                    return InitQualitySetting.XESS_QUALITY_SETTING_QUALITY;
                case IntelQuality.Balanced:
                    return InitQualitySetting.XESS_QUALITY_SETTING_BALANCED;
                case IntelQuality.Performance:
                    return InitQualitySetting.XESS_QUALITY_SETTING_PERFORMANCE;
                case IntelQuality.UltraPerformance:
                    return InitQualitySetting.XESS_QUALITY_SETTING_ULTRA_PERFORMANCE;
                default:
                    Debug.LogError($"[XeSS Upscaler]: Quality Level {quality} is not implemented, defaulting to balanced");
                    break;
            }

            return InitQualitySetting.XESS_QUALITY_SETTING_QUALITY;
        }

        const string _xessPlugin = "XeSSUnityPlugin";

        [DllImport(_xessPlugin, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern XeSSVersion XUP_OnGetXeSSVersion();

        [DllImport(_xessPlugin, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern XeSSVersion XUP_OnGetIntelXeFXVersion();

        [DllImport(_xessPlugin, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern XeSSResult XUP_OnInitializeContext();

        [DllImport(_xessPlugin, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern IntPtr XUP_GetInitEvent();

        [DllImport(_xessPlugin, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern IntPtr XUP_GetExecuteEvent();

        [DllImport(_xessPlugin, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern IntPtr XUP_OnReleaseResources();

        [DllImport(_xessPlugin, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern IntPtr XUP_OnSetDx12onDx11SyncMethods(SyncMethod textureSyncMethod, SyncMethod copyBackSyncMethod);

        private enum SyncMethod
        {
            NoSyncing = 0,
            Fence = 1,
            FenceAndFlush = 2,
            FenceAndEvent = 3,
            FenceFlushAndEvent = 4,
            QueryOnly = 5,
        }
    }
}
