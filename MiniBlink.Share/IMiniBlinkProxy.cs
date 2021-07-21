using System;
using System.Runtime.InteropServices;
using AutoDllProxy.Attributes;
using MiniBlink.Share.Events;

namespace MiniBlink.Share
{
    [Dll("node_x86.dll", "node_x64.dll")]
    public interface IMiniBlinkProxy
    {
        /// <summary>
        /// 初始化
        /// </summary>
        [Import(EntryPoint = "wkeInitialize", CallingConvention = CallingConvention.Cdecl)]
        void Initialize();

        /// <summary>
        /// webView是否已初始化。
        /// </summary>
        /// <returns></returns>
        [Import(EntryPoint = "wkeIsInitialize", CallingConvention = CallingConvention.Cdecl)]
        bool IsInitialize();

        // /// <summary>
        // /// 使用指定参数初始化
        // /// </summary>
        // /// <returns></returns>
        // [Import(EntryPoint = "InitializeEx", CallingConvention = CallingConvention.Cdecl)]
        // void InitializeEx(ref wkeSettings settings);
        //
        // /// <summary>
        // /// 设置一些配置项
        // /// </summary>
        // /// <param name="settings"></param>
        // [Import(EntryPoint = "wkeConfigure", CallingConvention = CallingConvention.Cdecl)]
        // void Configure(wkeSettings settings);
        //
        // /// <summary>
        // /// 强制停止MB，一般用于开发调试。
        // /// </summary>
        // [Import(EntryPoint = "wkeShutdownForDebug", CallingConvention = CallingConvention.Cdecl)]
        // void ShutdownForDebug();
        //
        // /// <summary>
        // /// 设置一些实验性选项，debugString目前支持：
        // /// showDevTools：开启开发者工具，此时param要填写开发者工具的资源路径，如file:///c:/MiniBlink.Share-release/front_end/inspector.html，必须是全路径，并且不能有中文。
        // /// wakeMinInterval：设置帧率，默认值是10，值越大帧率越低。
        // /// drawMinInterval：设置帧率，默认值是3，值越大帧率越低。
        // /// antiAlias：设置抗锯齿渲染，param必须设置为“1”。
        // /// minimumFontSize：最小字体。
        // /// minimumLogicalFontSize：最小逻辑字体。
        // /// defaultFontSize：默认字号。
        // /// defaultFixedFontSize：默认Fixed字号。
        // /// imageEnable：是否打开无图模式，param为“0”表示开启无图模式。
        // /// jsEnable：是否禁用js，param为“0”表示禁用。
        // /// </summary>
        // /// <param name="webView"></param>
        // /// <param name="debugString"></param>
        // /// <param name="param"></param>
        // [Import(EntryPoint = "wkeSetDebugConfig", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        // void SetDebugConfig(IntPtr webView, string debugString, string param);
        //
        // /// <summary>
        // /// 获取调试选项，参看SetDebugConfig。
        // /// </summary>
        // /// <param name="webView"></param>
        // /// <param name="debugString"></param>
        // /// <returns></returns>
        // [Import(EntryPoint = "wkeGetDebugConfig", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        // string GetDebugConfig(IntPtr webView, string debugString);
        //
        // /// <summary>
        // /// 获取MB的内部版本号。
        // /// </summary>
        // /// <returns></returns>
        // [Import(EntryPoint = "wkeGetVersion", CallingConvention = CallingConvention.Cdecl)]
        // uint GetVersion();

        /// <summary>
        /// 获取MB的发行版本号。
        /// </summary>
        /// <returns></returns>
        [Import(EntryPoint = "wkeGetVersionString", CallingConvention = CallingConvention.Cdecl,MarshalTypeRef = typeof(Utf8Marshaler))]
        string GetVersionString();

        /// <summary>
        /// 创建浏览器对象，需要先初始化才可以调用该方法
        /// </summary>
        /// <returns>返回句柄用于后续操作</returns>
        [Import(EntryPoint = "wkeCreateWebView",ReturnValue="webView", CallingConvention = CallingConvention.Cdecl)]
        IntPtr CreateWebView();
        /// <summary>
        /// 设置wkeWebView对应的窗口句柄，用于无头模式，如果是wkeCreateWebWindow创建的webview，则已经自带窗口句柄了。
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="wndHandle"></param>
        [Import(EntryPoint = "wkeSetHandle",Parameter="webView", CallingConvention = CallingConvention.Cdecl)]
        void SetHandle( IntPtr wndHandle);
/*

        #region 浏览器操作



        /// <summary>
        /// 设置是否支持拖拽文件到页面，默认开启。
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="enable"></param>
        [Import(EntryPoint = "wkeSetDragEnable", CallingConvention = CallingConvention.Cdecl)]
        void SetDragEnable(IntPtr webView, bool enable);

        /// <summary>
        /// 获取webWindow句柄。
        /// </summary>
        /// <param name="webView"></param>
        /// <returns></returns>
        [Import(EntryPoint = "wkeGetWindowHandle", CallingConvention = CallingConvention.Cdecl)]
        IntPtr GetWindowHandle(IntPtr webView);

        /// <summary>
        /// 创建一个带真实窗口的wkeWebView，type可取值为：
        ///WKE_WINDOW_TYPE_POPUP为普通窗口。
        ///WKE_WINDOW_TYPE_TRANSPARENT为透明窗口，通过layer window实现。
        ///WKE_WINDOW_TYPE_CONTROL为嵌入在父窗口里的子窗口，需设置parent句柄。
        /// </summary>
        /// <returns></returns>
        [Import(EntryPoint = "wkeCreateWebWindow", CallingConvention = CallingConvention.Cdecl)]
        IntPtr CreateWebWindow(wkeWindowType type, IntPtr parent, int x, int y, int width, int height);

        /// <summary>
        /// 开启无头模式，默认关。开启后页面不会被渲染上屏，直接内存运行，大幅提升性能。
        /// 注意：目前很多网站对这种模式已经进行了识别和反制。
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="enable"></param>
        [Import(EntryPoint = "wkeSetHeadlessEnabled", CallingConvention = CallingConvention.Cdecl)]
        void SetHeadlessEnabled(IntPtr webView, bool enable);

        /// <summary>
        /// 设置是否支持拖拽页面元素，默认开启。
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="enable"></param>
        [Import(EntryPoint = "wkeSetDragDropEnable", CallingConvention = CallingConvention.Cdecl)]
        void SetDragDropEnable(IntPtr webView, bool enable);

        [Import(EntryPoint = "wkeSetNavigationToNewWindowEnable", CallingConvention = CallingConvention.Cdecl)]
        void SetNavigationToNewWindowEnable(IntPtr webView, bool enable);

        /// <summary>
        /// 设置是否开启csp跨域检查，默认开。如需进行跨域操作，如跨域ajax，跨域设置iframe等，则需要先关闭此开关。
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="enable"></param>
        [Import(EntryPoint = "wkeSetCspCheckEnable", CallingConvention = CallingConvention.Cdecl)]
        void SetCspCheckEnable(IntPtr webView, bool enable);

        /// <summary>
        /// 获取网页H5源码。
        /// </summary>
        /// <param name="webView"></param>
        /// <returns></returns>
        [Import(EntryPoint = "wkeGetSource", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))]
        string GetSource(IntPtr webView);

        #endregion


        #region LocalStorage

        /// <summary>
        /// 设置local storage的全路径。如c:\mb\LocalStorage\，必须且只能是目录，默认是当前目录。
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="path"></param>
        [Import(EntryPoint = "wkeSetLocalStorageFullPath", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        void SetLocalStorageFullPath(IntPtr webView, string path);

        #endregion

        #region UserAgent

        /// <summary>
        /// 设置webview的UA，全局生效。
        /// 注意：标准版接口的UA是全局的。
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="userAgent"></param>
        [Import(EntryPoint = "wkeSetUserAgent", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        void SetUserAgent(IntPtr webView,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))]
            string userAgent);

        /// <summary>
        /// 获取webview的UA。
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        [Import(EntryPoint = "wkeGetUserAgent", CallingConvention = CallingConvention.Cdecl)]
        string GetUserAgent(IntPtr webView,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))]
            string userAgent);

        #endregion

        #region Url

        /// <summary>
        /// 获取主frame的Url
        /// </summary>
        /// <returns></returns>
        [Import(EntryPoint = "wkeGetURL", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))]
        string GetUrl(IntPtr webView);

        /// <summary>
        /// 加载网址。注意：建议写完整url，如：http://www.MiniBlink.Share.net。
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="url"></param>
        [Import(EntryPoint = "wkeLoadURL", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        void LoadURL(IntPtr webView, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))]
            string url);

        [Import(EntryPoint = "wkePostURL", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        void PostURL(IntPtr webView, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))]
            string url, string postData, int postLen);

        /// <summary>
        /// 加载一段H5代码。
        /// 注意：如果代码里有相对路径的文件操作，则是当前目录。
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="html"></param>
        [Import(EntryPoint = "wkeLoadHTML", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        void LoadHTML(IntPtr webView, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))]
            string html);

        /// <summary>
        /// 同LoadHTML，但可以指定相对于哪个目录。
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="html"></param>
        /// <param name="baseUrl"></param>
        [Import(EntryPoint = "wkeLoadHtmlWithBaseUrl", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        void LoadHtmlWithBaseUrl(IntPtr webView,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))]
            string html, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))]
            string baseUrl);

        #endregion

        #region Cookie

        /// <summary>
        /// 设置cookie文件的全路径，默认是程序运行的当前目录，默认文件名为cookie.dat。
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="path"></param>
        [Import(EntryPoint = "wkeSetCookieJarFullPath", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        void SetCookieJarFullPath(IntPtr webView, string path);


        /// <summary>
        /// 设置cookie文件的目录，默认是程序运行的当前目录，文件名为：cookie.dat。
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="path"></param>
        [Import(EntryPoint = "wkeSetCookieJarPath", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        void SetCookieJarPath(IntPtr webView, string path);

        /// <summary>
        /// 判断cookie是否开启。
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        [Import(EntryPoint = "wkeIsCookieEnabled", CallingConvention = CallingConvention.Cdecl)]
        bool IsCookieEnabled(IntPtr webView);

        /// <summary>
        /// 开启或关闭cookie。
        /// 注意：这个接口只是影响当前渲染的页面，并不会设置curl。所以还是会生成curl的cookie文件。
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="enable"></param>
        [Import(EntryPoint = "wkeSetCookieEnabled", CallingConvention = CallingConvention.Cdecl)]
        void SetCookieEnabled(IntPtr webView, bool enable);

        /// <summary>
        /// 获取页面的cookie字符串
        /// </summary>
        /// <param name="webView"></param>
        /// <returns></returns>
        [Import(EntryPoint = "wkeGetCookie", CallingConvention = CallingConvention.Cdecl)]
        string GetCookie(IntPtr webView);

        [Import(EntryPoint = "wkeClearCookie", CallingConvention = CallingConvention.Cdecl)]
        void ClearCookie(IntPtr webView);

        /// <summary>
        /// 设置页面的cookie，全局生效。
        /// 标准版的Cookie是全局的，而VIP版可以针对每个webView单独设置。
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="url"></param>
        /// <param name="cookie">必须符合curl的标准写法。如：PERSONALIZE=123;expires=Monday 13-Jun-2022 03:04:55 GMT; domain=.fidelity.com; path=/; secure</param>
        /// <returns></returns>
        [Import(EntryPoint = "wkeSetCookie", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        string SetCookie(IntPtr webView,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))]
            string url, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))]
            string cookie);

        #endregion

        #region Events

        [Import(EntryPoint = "wkeOnTitleChanged", CallingConvention = CallingConvention.Cdecl)]
        void OnTitleChanged(IntPtr webView, wkeTitleChangedCallback callback,
            IntPtr callbackParam);

        #endregion
        
        */
    }

    public static class MiniBlinkExtensions
    {
        // public static void LoadUrl(this MiniBlink.Share MiniBlink.Share, string url)
        // {
        //     MiniBlink.Share.Instance.LoadURL(MiniBlink.Share, url);
        // }
    }

    public struct MiniBlink
    {
        IntPtr _handle;
        static readonly IMiniBlinkProxy Proxy;
        internal IMiniBlinkProxy Instance => Proxy;

        public bool Equals(MiniBlink other)
            => other._handle == _handle;

        public static implicit operator MiniBlink(IntPtr ptr)
            => new MiniBlink() {_handle = ptr};

        public static implicit operator IntPtr(MiniBlink value)
            => value._handle;

        public bool HaveValue => _handle != IntPtr.Zero;

        static MiniBlink()
        {
            Proxy = AutoDllProxy.DllModuleBuilder.Create().Build<IMiniBlinkProxy>();
        }

        public static bool IsGlobalInitialization = false;
        public static object GlobalRoot = new object();

        public static void Init()
        {
            if (!IsGlobalInitialization)
                lock (GlobalRoot)
                    if (!IsGlobalInitialization)
                    {
                        Proxy.Initialize();
                        IsGlobalInitialization = Proxy.IsInitialize();
                    }
        }

        public static MiniBlink Create()
        {
            Init();
            var handle = Proxy.CreateWebView();
            if (handle == IntPtr.Zero)
            {
                throw new NullReferenceException(nameof(handle));
            }

            // Proxy.OnTitleChanged(handle,Cb,IntPtr.Zero);
            return handle;
        }


        // public void LoadUrl(string url) => Proxy.LoadURL(this, url);
        // public void SetCookieJarFullPath(string path) => Proxy.SetCookieJarFullPath(this, path);
        // public void SetCookieJarPath(string path) => Proxy.SetCookieJarPath(this, path);
        // public void SetLocalStorageFullPath(string path) => Proxy.SetLocalStorageFullPath(this, path);
        // public void SetDragEnable(bool enable) => Proxy.SetDragEnable(this, enable);
        // public void SetDragDropEnable(bool enable) => Proxy.SetDragDropEnable(this, enable);
        // public void SetNavigationToNewWindowEnable(bool enable) => Proxy.SetNavigationToNewWindowEnable(this, enable);

        public event EventHandler<TitleChangeEventArgs> OnTitleChanged;
    }
}