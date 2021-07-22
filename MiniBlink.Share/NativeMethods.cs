using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Reflection.Emit;
using System.Threading.Tasks;
using AutoDllProxy;

namespace MiniBlink.Share
{
    #region 定义委托

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeTitleChangedCallback(IntPtr webView, IntPtr param, IntPtr title);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeMouseOverUrlChangedCallback(IntPtr webView, IntPtr param, IntPtr url);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeURLChangedCallback2(IntPtr webView, IntPtr param, IntPtr frame, IntPtr url);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkePaintUpdatedCallback(IntPtr webView, IntPtr param, IntPtr buffer, int x, int y, int cx, int cy);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkePaintBitUpdatedCallback(IntPtr webView, IntPtr param, IntPtr hdc, ref wkeRect r, int width, int height);


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeAlertBoxCallback(IntPtr webView, IntPtr param, IntPtr msg);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte wkeConfirmBoxCallback(IntPtr webView, IntPtr param, IntPtr msg);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte wkePromptBoxCallback(IntPtr webView, IntPtr param, IntPtr msg, IntPtr defaultResult,
        IntPtr result);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte wkeNavigationCallback(IntPtr webView, IntPtr param, wkeNavigationType navigationType,
        IntPtr url);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr wkeCreateViewCallback(IntPtr webView, IntPtr param, wkeNavigationType navigationType,
        IntPtr url, IntPtr windowFeatures);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeDocumentReadyCallback(IntPtr webView, IntPtr param);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeDocumentReady2Callback(IntPtr webView, IntPtr param, IntPtr frame);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeLoadingFinishCallback(IntPtr webView, IntPtr param, IntPtr url, wkeLoadingResult result,
        IntPtr failedReason);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte wkeDownloadCallback(IntPtr webView, IntPtr param, IntPtr url);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte wkeDownload2Callback(IntPtr webView, IntPtr param, uint expectedContentLength, IntPtr url,
        IntPtr mime, IntPtr disposition, IntPtr job, IntPtr dataBind);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeConsoleCallback(IntPtr webView, IntPtr param, wkeConsoleLevel level, IntPtr message,
        IntPtr sourceName, uint sourceLine, IntPtr stackTrace);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte wkeLoadUrlBeginCallback(IntPtr webView, IntPtr param, IntPtr url, IntPtr job);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeLoadUrlEndCallback(IntPtr webView, IntPtr param, IntPtr url, IntPtr job, IntPtr buf,
        int len);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeLoadUrlFailCallback(IntPtr webView, IntPtr param, IntPtr url, IntPtr job);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeDidCreateScriptContextCallback(IntPtr webView, IntPtr param, IntPtr frame, IntPtr context,
        int extensionGroup, int worldId);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeWillReleaseScriptContextCallback(IntPtr webView, IntPtr param, IntPtr frame, IntPtr context,
        int worldId);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate byte wkeNetResponseCallback(IntPtr webView, IntPtr param, IntPtr url, IntPtr job);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeWillMediaLoadCallback(IntPtr webView, IntPtr param, IntPtr url, IntPtr info);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeOnOtherLoadCallback(IntPtr webView, IntPtr param, wkeOtherLoadType type, IntPtr info);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long wkeJsNativeFunction(IntPtr jsExecState, IntPtr param);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeOnShowDevtoolsCallback(IntPtr webView, IntPtr param);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeOnNetGetFaviconCallback(IntPtr webView, IntPtr param, IntPtr utf8Url, ref wkeMemBuf buf);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeNetJobDataRecvCallback(IntPtr ptr, IntPtr job, IntPtr data, int length);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeNetJobDataFinishCallback(IntPtr ptr, IntPtr job, wkeLoadingResult result);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate long jsGetPropertyCallback(IntPtr es, long obj, string propertyName);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate byte jsSetPropertyCallback(IntPtr es, long obj, string propertyName, long value);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long jsCallAsFunctionCallback(IntPtr es, long obj, IntPtr args, int argCount);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void jsFinalizeCallback(IntPtr data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeOnUrlRequestWillRedirectCallback(IntPtr webView, IntPtr param, IntPtr oldRequest,
        IntPtr request, IntPtr redirectResponse);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeOnUrlRequestDidReceiveResponseCallback(IntPtr webView, IntPtr param, IntPtr request,
        IntPtr response);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeOnUrlRequestDidReceiveDataCallback(IntPtr webView, IntPtr param, IntPtr request,
        IntPtr data, int dataLength);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeOnUrlRequestDidFailCallback(IntPtr webView, IntPtr param, IntPtr request, IntPtr error);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void wkeOnUrlRequestDidFinishLoadingCallback(IntPtr webView, IntPtr param, IntPtr request,
        long finishTime);

    /// <summary>
    /// 访问Cookie回调
    /// </summary>
    /// <param name="userData">用户数据</param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="domain">域名</param>
    /// <param name="path">路径</param>
    /// <param name="secure">安全，如果非0则仅发送到https请求</param>
    /// <param name="httpOnly">如果非0则仅发送到http请求</param>
    /// <param name="expires">过期时间 The cookie expiration date is only valid if |has_expires| is true.</param>
    /// <returns>返回true 则应用程序自己处理MiniBlink.Share不处理</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool wkeCookieVisitor(IntPtr userData, [MarshalAs(UnmanagedType.LPStr)] string name,
        [MarshalAs(UnmanagedType.LPStr)] string value, [MarshalAs(UnmanagedType.LPStr)] string domain,
        [MarshalAs(UnmanagedType.LPStr)] string path, int secure, int httpOnly, ref int expires);

    #endregion


    #region 枚举

    public enum wkeMouseFlags
    {
        WKE_LBUTTON = 0x01,
        WKE_RBUTTON = 0x02,
        WKE_SHIFT = 0x04,
        WKE_CONTROL = 0x08,
        WKE_MBUTTON = 0x10,
    }
    public enum  wkeWindowType
    {
        WKE_WINDOW_TYPE_POPUP,
        WKE_WINDOW_TYPE_TRANSPARENT,
        WKE_WINDOW_TYPE_CONTROL
    }
    

    public enum wkeKeyFlags
    {
        WKE_EXTENDED = 0x0100,
        WKE_REPEAT = 0x4000,
    }

    public enum jsType
    {
        NUMBER,
        STRING,
        BOOLEAN,
        OBJECT,
        FUNCTION,
        UNDEFINED,
        ARRAY,
        NULL
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct jsKeys
    {
        public int length;
        public IntPtr keys;
    }

    public enum wkeConsoleLevel
    {
        Debug = 4,
        Log = 1,
        Info = 5,
        Warning = 2,
        Error = 3,
        RevokedError = 6,
    }

    public enum wkeLoadingResult
    {
        Succeeded,
        Failed,
        Canceled
    }

    public enum wkeNavigationType
    {
        LinkClick,
        FormSubmit,
        BackForward,
        ReLoad,
        FormReSubmit,
        Other
    }

    public enum wkeCursorStyle
    {
        Pointer,
        Cross,
        Hand,
        IBeam,
        Wait,
        Help,
        EastResize,
        NorthResize,
        NorthEastResize,
        NorthWestResize,
        SouthResize,
        SouthEastResize,
        SouthWestResize,
        WestResize,
        NorthSouthResize,
        EastWestResize,
        NorthEastSouthWestResize,
        NorthWestSouthEastResize,
        ColumnResize,
        RowResize,
        MiddlePanning,
        EastPanning,
        NorthPanning,
        NorthEastPanning,
        NorthWestPanning,
        SouthPanning,
        SouthEastPanning,
        SouthWestPanning,
        WestPanning,
        Move,
        VerticalText,
        Cell,
        ContextMenu,
        Alias,
        Progress,
        NoDrop,
        Copy,
        None,
        NotAllowed,
        ZoomIn,
        ZoomOut,
        Grab,
        Grabbing,
        Custom
    }

    public enum wkeCookieCommand
    {
        ClearAllCookies,
        ClearSessionCookies,
        FlushCookiesToFile,
        ReloadCookiesFromFile
    }

    public enum wkeProxyType
    {
        NONE,
        HTTP,
        SOCKS4,
        SOCKS4A,
        SOCKS5,
        SOCKS5HOSTNAME
    }

    public enum wkeSettingMask
    {
        PROXY = 1,
        PAINTCALLBACK_IN_OTHER_THREAD = 4,
    }

    public enum wkeOtherLoadType
    {
        WKE_DID_START_LOADING,
        WKE_DID_STOP_LOADING,
        WKE_DID_NAVIGATE,
        WKE_DID_NAVIGATE_IN_PAGE,
        WKE_DID_GET_RESPONSE_DETAILS,
        WKE_DID_GET_REDIRECT_REQUEST
    }

    public enum wkeResourceType
    {
        MAIN_FRAME = 0, // top level page
        SUB_FRAME = 1, // frame or iframe
        STYLESHEET = 2, // a CSS stylesheet
        SCRIPT = 3, // an external script
        IMAGE = 4, // an image (jpg/gif/png/etc)
        FONT_RESOURCE = 5, // a font
        SUB_RESOURCE = 6, // an "other" subresource.
        OBJECT = 7, // an object (or embed) tag for a plugin, or a resource that a plugin requested.
        MEDIA = 8, // a media resource.
        WORKER = 9, // the main resource of a dedicated worker.
        SHARED_WORKER = 10, // the main resource of a shared worker.
        PREFETCH = 11, // an explicitly requested prefetch
        FAVICON = 12, // a favicon
        XHR = 13, // a XMLHttpRequest
        PING = 14, // a ping request for <a ping>
        SERVICE_WORKER = 15, // the main resource of a service worker.
    }

    public enum wkeMenuItemId
    {
        kWkeMenuSelectedAllId = 1 << 1,
        kWkeMenuSelectedTextId = 1 << 2,
        kWkeMenuUndoId = 1 << 3,
        kWkeMenuCopyImageId = 1 << 4,
        kWkeMenuInspectElementAtId = 1 << 5,
        kWkeMenuCutId = 1 << 6,
        kWkeMenuPasteId = 1 << 7,
        kWkeMenuPrintId = 1 << 8,
        kWkeMenuGoForwardId = 1 << 9,
        kWkeMenuGoBackId = 1 << 10,
        kWkeMenuReloadId = 1 << 11,
    }

    public enum wkeRequestType
    {
        Invalidation,
        Get,
        Post,
        Put,
    }

    public enum wkeHttBodyElementType
    {
        wkeHttBodyElementTypeData,
        wkeHttBodyElementTypeFile
    }

    #endregion

    #region 结构体

    public struct jsData
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string typeName;

        [MarshalAs(UnmanagedType.FunctionPtr)] public jsGetPropertyCallback propertyGet;

        [MarshalAs(UnmanagedType.FunctionPtr)] public jsSetPropertyCallback propertySet;

        [MarshalAs(UnmanagedType.FunctionPtr)] public jsFinalizeCallback finalize;

        [MarshalAs(UnmanagedType.FunctionPtr)] public jsCallAsFunctionCallback callAsFunction;
    }

    public struct wkeNetJobDataBind
    {
        IntPtr param;

        [MarshalAs(UnmanagedType.FunctionPtr)] public wkeNetJobDataRecvCallback recvCallback;

        [MarshalAs(UnmanagedType.FunctionPtr)] public wkeNetJobDataFinishCallback finishCallback;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct wkeRect
    {
        public int x;
        public int y;
        public int w;
        public int h;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct wkeProxy
    {
        public wkeProxyType Type;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string HostName;

        public ushort Port;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string UserName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string Password;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct wkeSettings
    {
        public wkeProxy Proxy;
        public wkeSettingMask Mask;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct wkeWindowFeatures
    {
        public int x;
        public int y;
        public int width;
        public int height;

        [MarshalAs(UnmanagedType.I1)] public bool menuBarVisible;

        [MarshalAs(UnmanagedType.I1)] public bool statusBarVisible;

        [MarshalAs(UnmanagedType.I1)] public bool toolBarVisible;

        [MarshalAs(UnmanagedType.I1)] public bool locationBarVisible;

        [MarshalAs(UnmanagedType.I1)] public bool scrollbarsVisible;

        [MarshalAs(UnmanagedType.I1)] public bool resizable;

        [MarshalAs(UnmanagedType.I1)] public bool fullscreen;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct wkeMediaLoadInfo
    {
        public int size;
        public int width;
        public int height;
        public double duration;
    }

    public struct wkeWillSendRequestInfo
    {
        public bool isHolded;
        public string url;
        public string newUrl;
        public wkeResourceType resourceType;
        public int httpResponseCode;
        public string method;
        public string referrer;
        public IntPtr headers;
    }

    public struct wkeTempCallbackInfo
    {
        public int size;
        public IntPtr frame;
        public IntPtr willSendRequestInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct wkeMemBuf
    {
        public int size;
        public IntPtr data;
        public int length;
    }

    public struct jsExceptionInfo
    {
        public string Message;
        public string SourceLine;
        public string ScriptResourceName;
        public int LineNumber;
        public int StartPosition;
        public int EndPosition;
        public int StartColumn;
        public int EndColoumn;
        public string CallStackString;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct wkeViewSettings
    {
        public int size;
        public uint bgColor;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct wkeSlist
    {
        public IntPtr str;
        public IntPtr next;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct wkePostBodyElement
    {
        public int size;
        public wkeHttBodyElementType type;
        public IntPtr data;
        public string filePath;
        public long fileStart;
        public long fileLength;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct wkePostBodyElements
    {
        public int size;
        public IntPtr element;
        public int elementSize;
        public bool isDirty;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct wkeUrlRequestCallbacks
    {
        wkeOnUrlRequestWillRedirectCallback willRedirectCallback;
        wkeOnUrlRequestDidReceiveResponseCallback didReceiveResponseCallback;
        wkeOnUrlRequestDidReceiveDataCallback didReceiveDataCallback;
        wkeOnUrlRequestDidFailCallback didFailCallback;
        wkeOnUrlRequestDidFinishLoadingCallback didFinishLoadingCallback;
    }

    #endregion
}