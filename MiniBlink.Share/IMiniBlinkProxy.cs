using System;
using System.Runtime.InteropServices;
using System.Text;
using AutoDllProxy.Attributes;
using MiniBlink.Share.Events;

namespace MiniBlink.Share
{
    [Dll("node_x86.dll", "node_x64.dll")]
    public interface IMiniBlinkProxy
    {
        #region 基础

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
        /// <summary>
        /// 获取MB的内部版本号。
        /// </summary>
        /// <returns></returns>
        [Import(EntryPoint = "wkeGetVersion", CallingConvention = CallingConvention.Cdecl)]
        uint GetVersion();

        /// <summary>
        /// 获取MB的发行版本号。
        /// </summary>
        /// <returns></returns>
        [Import(EntryPoint = "wkeGetVersionString", CallingConvention = CallingConvention.Cdecl,
            MarshalTypeRef = typeof(Utf8Marshaler))]
        string GetVersionString();

        /// <summary>
        /// 创建浏览器对象，需要先初始化才可以调用该方法,添加了ReturnValue则表示生成一个字段在内部实现，则其它方法可以不用传这个参数
        /// </summary>
        /// <returns>返回句柄用于后续操作</returns>
        [Import(EntryPoint = "wkeCreateWebView", ReturnValue = Const.WebViewHandle,
            CallingConvention = CallingConvention.Cdecl)]
        IntPtr CreateWebViewEx();

        /// <summary>
        /// 创建浏览器对象，需要先初始化才可以调用该方法
        /// </summary>
        /// <returns></returns>
        [Import(EntryPoint = "wkeCreateWebView",
            CallingConvention = CallingConvention.Cdecl)]
        IntPtr CreateWebView();

        /// <summary>
        /// 设置wkeWebView对应的窗口句柄，用于无头模式，如果是wkeCreateWebWindow创建的webview，则已经自带窗口句柄了。
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="wndHandle"></param>
        [Import(EntryPoint = "wkeSetHandle", Parameter = Const.WebViewHandle,
            CallingConvention = CallingConvention.Cdecl)]
        void SetHandle(IntPtr wndHandle);

        [Import(EntryPoint = "wkeSetHandle",
            CallingConvention = CallingConvention.Cdecl)]
        void SetHandle(IntPtr webView, IntPtr wndHandle);

        #endregion

        #region 浏览器操作

        /// <summary>
        /// 显示DevTools窗口
        /// </summary>
        /// <param name="WebView"></param>
        /// <param name="path">路径</param>
        /// <param name="callback"></param>
        /// <param name="param"></param>
        [Import(EntryPoint = "wkeShowDevtools", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        void ShowDevTools(IntPtr webView, string path, wkeOnShowDevtoolsCallback callback, IntPtr param);

        [Import(EntryPoint = "wkeGetCursorInfoType", CallingConvention = CallingConvention.Cdecl)]
        wkeCursorInfo GetCursorInfoType(IntPtr webView);

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

        /// <summary>
        /// 当有新页面需要打开时（如标签），是否在新窗口打开，默认是当前窗口，如开启，则会触发wkeOnCreateView回调接口。
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="enable"></param>
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

        /// <summary>
        /// 设置webview获得焦点。 注意：如果webveiw绑定了窗口，窗口也会获得焦点。
        /// </summary>
        /// <param name="webView"></param>
        [Import(EntryPoint = "wkeSetFocus", CallingConvention = CallingConvention.Cdecl)]
        void SetFocus(IntPtr webView);

        /// <summary>
        /// 设置webview放弃焦点。
        /// </summary>
        /// <param name="webView"></param>
        [Import(EntryPoint = "wkeKillFocus", CallingConvention = CallingConvention.Cdecl)]
        void KillFocus(IntPtr webView);

        [Import(EntryPoint = "wkeGC", CallingConvention = CallingConvention.Cdecl)]
        void Gc(IntPtr webView, int delayMs);

        [Import(EntryPoint = "wkeGetWebView", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        IntPtr GetWebView(string name);

        /// <summary>
        /// 干掉webView，释放其内存。
        /// </summary>
        /// <param name="webView"></param>
        [Import(EntryPoint = "wkeDestroyWebView", CallingConvention = CallingConvention.Cdecl)]
        void DestroyWebView(IntPtr webView);

        /// <summary>
        /// 获取编辑框中光标的位置
        /// </summary>
        /// <param name="webView"></param>
        /// <returns></returns>
        [Import(EntryPoint = "wkeGetCaretRect", CallingConvention = CallingConvention.Cdecl)]
        wkeRect GetCaretRect(IntPtr webView);

        #endregion

        #region Frame

        /// <summary>
        /// 判断是否是主frame。
        /// </summary>
        /// <param name="webview"></param>
        /// <param name="webFrame"></param>
        /// <returns></returns>
        [Import(EntryPoint = "wkeIsMainFrame", CallingConvention = CallingConvention.Cdecl)]
        bool IsMainFrame(IntPtr webView, IntPtr webFrame);

        /// <summary>
        /// 获取指定frame的url
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="frameId"></param>
        /// <returns></returns>
        [Import(EntryPoint = "wkeGetFrameUrl", CallingConvention = CallingConvention.Cdecl,
            MarshalTypeRef = typeof(Utf8Marshaler))]
        string GetFrameUrl(IntPtr webView, IntPtr frameId);

        /// <summary>
        /// 判断是否是远程frame。
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="frameId"></param>
        /// <returns></returns>
        [Import(EntryPoint = "wkeIsWebRemoteFrame", CallingConvention = CallingConvention.Cdecl)]
        bool IsWebRemoteFrame(IntPtr webView, IntPtr frameId);

        /// <summary>
        /// 获取主frame
        /// </summary>
        /// <param name="webView"></param>
        /// <returns></returns>
        [Import(EntryPoint = "wkeWebFrameGetMainFrame", CallingConvention = CallingConvention.Cdecl)]
        IntPtr WebFrameGetMainFrame(IntPtr webView);

        #endregion

        #region JsExec

        /// <summary>
        /// 获取页面指定frame的jsExecState
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="frameId"></param>
        /// <returns></returns>
        [Import(EntryPoint = "wkeGetGlobalExecByFrame", CallingConvention = CallingConvention.Cdecl)]
        IntPtr GetGlobalExecByFrame(IntPtr webView, IntPtr frameId);

        /// <summary>
        /// 执行一段js代码，代码会在MB内部自动被包裹在一个function(){}中，所以使用的变量会作为局部变量以避免同页面js代码中的其他变量重名导致运行出错，要获取返回值，请加return。
        /// </summary>
        /// <param name="es"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        [Import(EntryPoint = "jsEval", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        long JsEval(IntPtr es, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))]
            string str);

        [Import(EntryPoint = "jsEvalW", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        long JsEvalW(IntPtr es, string str);

        /// <summary>
        /// 同jsEvalW，isInClosure设定是否闭包运行js。
        /// </summary>
        /// <param name="es">GetGlobalExecByFrame返回的jsExecState</param>
        /// <param name="str"></param>
        /// <param name="isInClosure"></param>
        /// <returns></returns>
        [Import(EntryPoint = "jsEvalExW", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        long JsEvalExW(IntPtr es, string str, [MarshalAs(UnmanagedType.I1)] bool isInClosure);

        /// <summary>
        /// 执行页面中的js函数，
        /// </summary>
        /// <param name="es"></param>
        /// <param name="func">要执行的函数</param>
        /// <param name="thisObject">如果此js函数是成员函数，则需要填thisValue，否则可以传jsUndefined，</param>
        /// <param name="args">func的参数数组</param>
        /// <param name="argCount">参数个数</param>
        /// <returns></returns>
        [Import(EntryPoint = "jsCall", CallingConvention = CallingConvention.Cdecl)]
        long JsCall(IntPtr es, long func, long thisObject, [MarshalAs(UnmanagedType.LPArray)] long[] args,
            int argCount);

        [Import(EntryPoint = "wkeJsBindFunction", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        void JsBindFunction(string name, wkeJsNativeFunction fn, IntPtr param, uint argCount);

        [Import(EntryPoint = "wkeJsBindGetter", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        void JsBindGetter(string name, wkeJsNativeFunction fn, IntPtr param);

        [Import(EntryPoint = "wkeJsBindSetter", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        void JsBindSetter(string name, wkeJsNativeFunction fn, IntPtr param);

        [Import(EntryPoint = "jsArgCount", CallingConvention = CallingConvention.Cdecl)]
        int JsArgCount(IntPtr es);

        [Import(EntryPoint = "jsArgType", CallingConvention = CallingConvention.Cdecl)]
        jsType JsArgType(IntPtr es, int argIdx);

        [Import(EntryPoint = "jsArg", CallingConvention = CallingConvention.Cdecl)]
        long JsArg(IntPtr es, int argIdx);

        [Import(EntryPoint = "jsTypeOf", CallingConvention = CallingConvention.Cdecl)]
        jsType JsTypeOf(long v);

        [Import(EntryPoint = "jsIsNumber", CallingConvention = CallingConvention.Cdecl)]
        bool JsIsNumber(long v);

        [Import(EntryPoint = "jsIsString", CallingConvention = CallingConvention.Cdecl)]
        bool JsIsString(long v);

        [Import(EntryPoint = "jsIsBoolean", CallingConvention = CallingConvention.Cdecl)]
        bool JsIsBoolean(long v);

        [Import(EntryPoint = "jsIsObject", CallingConvention = CallingConvention.Cdecl)]
        bool JsIsObject(long v);

        [Import(EntryPoint = "jsIsFunction", CallingConvention = CallingConvention.Cdecl)]
        bool JsIsFunction(long v);

        [Import(EntryPoint = "jsIsUndefined", CallingConvention = CallingConvention.Cdecl)]
        bool JsIsUndefined(long v);

        [Import(EntryPoint = "jsIsNull", CallingConvention = CallingConvention.Cdecl)]
        bool JsIsNull(long v);

        [Import(EntryPoint = "jsIsArray", CallingConvention = CallingConvention.Cdecl)]
        bool JsIsArray(long v);

        [Import(EntryPoint = "jsIsTrue", CallingConvention = CallingConvention.Cdecl)]
        bool JsIsTrue(long v);

        [Import(EntryPoint = "jsIsFalse", CallingConvention = CallingConvention.Cdecl)]
        bool JsIsFalse(long v);

        [Import(EntryPoint = "jsToInt", CallingConvention = CallingConvention.Cdecl)]
        int JsToInt(IntPtr es, long v);

        [Import(EntryPoint = "jsToFloat", CallingConvention = CallingConvention.Cdecl)]
        float JsToFloat(IntPtr es, long v);

        [Import(EntryPoint = "jsToDouble", CallingConvention = CallingConvention.Cdecl)]
        double JsToDouble(IntPtr es, long v);

        [Import(EntryPoint = "jsToBoolean", CallingConvention = CallingConvention.Cdecl)]
        byte JsToBoolean(IntPtr es, long v);

        [Import(EntryPoint = "jsToTempString", CallingConvention = CallingConvention.Cdecl,
            MarshalTypeRef = typeof(Utf8Marshaler))]
        string JsToTempString(IntPtr es, long v);

        [Import(EntryPoint = "jsToTempStringW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        string JsToTempStringW(IntPtr es, long v);

        [Import(EntryPoint = "jsInt", CallingConvention = CallingConvention.Cdecl)]
        long JsInt(int n);

        [Import(EntryPoint = "jsFloat", CallingConvention = CallingConvention.Cdecl)]
        long JsFloat(float f);

        [Import(EntryPoint = "jsDouble", CallingConvention = CallingConvention.Cdecl)]
        long jsDouble(double d);

        [Import(EntryPoint = "jsBoolean", CallingConvention = CallingConvention.Cdecl)]
        long jsBoolean(bool b);

        [Import(EntryPoint = "jsUndefined", CallingConvention = CallingConvention.Cdecl)]
        long jsUndefined();

        [Import(EntryPoint = "jsNull", CallingConvention = CallingConvention.Cdecl)]
        long jsNull();

        [Import(EntryPoint = "jsTrue", CallingConvention = CallingConvention.Cdecl)]
        long jsTrue();

        [Import(EntryPoint = "jsFalse", CallingConvention = CallingConvention.Cdecl)]
        long jsFalse();

        [Import(EntryPoint = "jsStringW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        long jsStringW(IntPtr es, string str);

        [Import(EntryPoint = "jsEmptyObject", CallingConvention = CallingConvention.Cdecl)]
        long jsEmptyObject(IntPtr es);

        [Import(EntryPoint = "jsEmptyArray", CallingConvention = CallingConvention.Cdecl)]
        long jsEmptyArray(IntPtr es);

        [Import(EntryPoint = "jsArrayBuffer", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        long jsArrayBuffer(IntPtr es, StringBuilder buffer, int size);

        [Import(EntryPoint = "jsObject", CallingConvention = CallingConvention.Cdecl)]
        long jsObject(IntPtr es, IntPtr obj);

        [Import(EntryPoint = "jsFunction", CallingConvention = CallingConvention.Cdecl)]
        long jsFunction(IntPtr es, IntPtr obj);

        [Import(EntryPoint = "jsGetData", CallingConvention = CallingConvention.Cdecl)]
        IntPtr jsGetData(IntPtr es, long jsValue);

        [Import(EntryPoint = "jsGet", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        long jsGet(IntPtr es, long jsValue, string prop);

        [Import(EntryPoint = "jsSet", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        void jsSet(IntPtr es, long jsValue, string prop, long v);

        [Import(EntryPoint = "jsGetAt", CallingConvention = CallingConvention.Cdecl)]
        long jsGetAt(IntPtr es, long jsValue, int index);

        [Import(EntryPoint = "jsSetAt", CallingConvention = CallingConvention.Cdecl)]
        void jsSetAt(IntPtr es, long jsValue, int index, long v);

        [Import(EntryPoint = "jsGetLength", CallingConvention = CallingConvention.Cdecl)]
        int jsGetLength(IntPtr es, long jsValue);

        [Import(EntryPoint = "jsSetLength", CallingConvention = CallingConvention.Cdecl)]
        void jsSetLength(IntPtr es, long jsValue, int length);

        [Import(EntryPoint = "jsGlobalObject", CallingConvention = CallingConvention.Cdecl)]
        long jsGlobalObject(IntPtr es);

        [Import(EntryPoint = "jsGetWebView", CallingConvention = CallingConvention.Cdecl)]
        IntPtr jsGetWebView(IntPtr es);


        [Import(EntryPoint = "jsCallGlobal", CallingConvention = CallingConvention.Cdecl)]
        long jsCallGlobal(IntPtr es, long func, [MarshalAs(UnmanagedType.LPArray)] Int64[] args, int argCount);

        [Import(EntryPoint = "jsGetGlobal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        long jsGetGlobal(IntPtr es, string prop);

        [Import(EntryPoint = "jsSetGlobal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        void jsSetGlobal(IntPtr es, string prop, long jsValue);

        [Import(EntryPoint = "jsIsJsValueValid", CallingConvention = CallingConvention.Cdecl)]
        byte jsIsJsValueValid(IntPtr es, long jsValue);

        [Import(EntryPoint = "jsIsValidExecState", CallingConvention = CallingConvention.Cdecl)]
        byte jsIsValidExecState(IntPtr es);

        [Import(EntryPoint = "jsDeleteObjectProp", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        void jsDeleteObjectProp(IntPtr es, long jsValue, string prop);

        [Import(EntryPoint = "jsGetArrayBuffer", CallingConvention = CallingConvention.Cdecl)]
        IntPtr jsGetArrayBuffer(IntPtr es, long jsValue);

        [Import(EntryPoint = "jsGetLastErrorIfException", CallingConvention = CallingConvention.Cdecl)]
        IntPtr jsGetLastErrorIfException(IntPtr es);

        [Import(EntryPoint = "jsThrowException", CallingConvention = CallingConvention.Cdecl)]
        long jsThrowException(IntPtr es, [MarshalAs(UnmanagedType.LPArray)] byte[] utf8exception);

        [Import(EntryPoint = "jsGetKeys", CallingConvention = CallingConvention.Cdecl)]
        IntPtr jsGetKeys(IntPtr es, long jsValue);

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

        #region resources

        /// <summary>
        /// 在指定frame中插入一段css。
        /// </summary>
        /// <param name="WebView"></param>
        /// <param name="frameId"></param>
        /// <param name="cssText"> const utf8* cssText </param>
        [Import(EntryPoint = "wkeInsertCSSByFrame", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        void InsertCSSByFrame(IntPtr WebView, IntPtr frameId, string cssText);

        [Import(EntryPoint = "wkeSetResourceGc", CallingConvention = CallingConvention.Cdecl)]
        void SetResourceGc(IntPtr WebView, int intervalSec);

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
            // [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))]
            string userAgent);

        /// <summary>
        /// 获取webview的UA。
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        [Import(EntryPoint = "wkeGetUserAgent", CallingConvention = CallingConvention.Cdecl)]
        string GetUserAgent(IntPtr webView);

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

        [Import(EntryPoint = "wkeLoadURL", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode, Parameter = Const.WebViewHandle)]
        void LoadURL([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshaler))]
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
        /// 重新设置页面的像素宽高，如果webView是带窗口模式的，会设置实际窗口的宽高。
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        [Import(EntryPoint = "wkeResize", CallingConvention = CallingConvention.Cdecl)]
        void Resize(IntPtr webView, int w, int h);

        /// <summary>
        /// 获取webView宽度像素
        /// </summary>
        /// <param name="webView"></param>
        /// <returns></returns>
        [Import(EntryPoint = "wkeGetWidth", CallingConvention = CallingConvention.Cdecl)]
        int GetWidth(IntPtr webView);

        /// <summary>
        /// 获取webView高度像素
        /// </summary>
        /// <param name="webView"></param>
        /// <returns></returns>
        [Import(EntryPoint = "wkeGetHeight", CallingConvention = CallingConvention.Cdecl)]
        int GetHeight(IntPtr webView);

        /// <summary>
        /// 获取webView排版出来的内容区域宽度像素。
        /// </summary>
        /// <param name="webView"></param>
        /// <returns></returns>
        [Import(EntryPoint = "wkeGetContentWidth", CallingConvention = CallingConvention.Cdecl)]
        int GetContentWidth(IntPtr webView);

        [Import(EntryPoint = "wkeGetContentHeight", CallingConvention = CallingConvention.Cdecl)]
        int GetContentHeight(IntPtr webView);


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

        /// <summary>
        /// 通过访问器visitor访问所有cookie
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="usetData"></param>
        /// <param name="visitor"></param>
        [Import(EntryPoint = "wkeVisitAllCookie", CallingConvention = CallingConvention.Cdecl)]
        void VisitAllCookie(IntPtr webView, IntPtr userData, wkeCookieVisitor visitor);

        #endregion

        #region 字符串相关

        [Import(EntryPoint = "wkeGetString", CallingConvention = CallingConvention.Cdecl,
            MarshalTypeRef = typeof(Utf8Marshaler))]
        string GetString(IntPtr wkeString);

        #endregion

        #region Events

        /// <summary>
        /// url改变时触发此回调
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="param"></param>
        [Import(EntryPoint = "wkeOnURLChanged", CallingConvention = CallingConvention.Cdecl)]
        void OnURLChanged(IntPtr webView, wkeURLChangedCallback callback, IntPtr param);

        /// <summary>
        /// url改变时触发此回调
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="param"></param>
        [Import(EntryPoint = "wkeOnURLChanged2", CallingConvention = CallingConvention.Cdecl)]
        void OnURLChanged(IntPtr webView, wkeURLChangedCallback2 callback, IntPtr param);

        /// <summary>
        /// 标题被改变
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="callbackParam"></param>
        [Import(EntryPoint = "wkeOnTitleChanged", CallingConvention = CallingConvention.Cdecl)]
        void OnTitleChanged(IntPtr webView, wkeTitleChangedCallback callback, IntPtr callbackParam);

        /// <summary>
        /// 新建webView时触发此回调，一般配合wkeSetNavigationToNewWindowEnable开关使用
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="param"></param>
        [Import(EntryPoint = "wkeOnCreateView", CallingConvention = CallingConvention.Cdecl)]
        void OnCreateView(IntPtr webView, wkeCreateViewCallback callback, IntPtr param);

        /// <summary>
        /// 鼠标滑过url时触发此回调
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="callbackParam"></param>
        [Import(EntryPoint = "wkeOnMouseOverUrlChanged", CallingConvention = CallingConvention.Cdecl)]
        void OnMouseOverUrlChanged(IntPtr webView, wkeTitleChangedCallback callback, IntPtr callbackParam);

        /// <summary>
        /// 页面任何区域刷新时触发此回调
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="param"></param>
        [Import(EntryPoint = "wkeOnPaintUpdated", CallingConvention = CallingConvention.Cdecl)]
        void OnPaintUpdated(IntPtr webView, wkePaintUpdatedCallback callback, IntPtr param);


        /// <summary>
        /// 功能同wkeOnPaintUpdated，不同的是回调过来的是填充好像素的buffer，而不是DC。方便嵌入到游戏中做离屏渲染
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="param"></param>
        [Import(EntryPoint = "wkeOnPaintBitUpdated", CallingConvention = CallingConvention.Cdecl)]
        void OnPaintBitUpdated(IntPtr webView, wkePaintBitUpdatedCallback callback, IntPtr param);

        /// <summary>
        /// 网页弹出alert时触发此回调
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="param"></param>
        [Import(EntryPoint = "wkeOnAlertBox", CallingConvention = CallingConvention.Cdecl)]
        void OnAlertBox(IntPtr webView, wkeAlertBoxCallback callback, IntPtr param);

        /// <summary>
        /// 网页弹出Confirm时触发此回调
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="param"></param>
        [Import(EntryPoint = "wkeOnConfirmBox", CallingConvention = CallingConvention.Cdecl)]
        void OnConfirmBox(IntPtr webView, wkeConfirmBoxCallback callback, IntPtr param);

        /// <summary>
        /// 网页弹出Prompt时触发此回调
        /// </summary>
        /// <param name="webView"></param>
        /// <param name="callback"></param>
        /// <param name="param"></param>
        [Import(EntryPoint = "wkeOnPromptBox", CallingConvention = CallingConvention.Cdecl)]
        void OnPromptBox(IntPtr webView, wkePromptBoxCallback callback, IntPtr param);

        [Import(EntryPoint = "wkeFireMouseWheelEvent", CallingConvention = CallingConvention.Cdecl)]
        bool FireMouseWheelEvent(IntPtr webView, int x, int y, int delta, uint flags);

        [Import(EntryPoint = "wkeFireMouseEvent", CallingConvention = CallingConvention.Cdecl)]
        bool FireMouseEvent(IntPtr webView, uint message, int x, int y, uint flags);

        [Import(EntryPoint = "wkeFireKeyPressEvent", CallingConvention = CallingConvention.Cdecl)]
        bool FireKeyPressEvent(IntPtr webView, int charCode, uint flags,
            [MarshalAs(UnmanagedType.I1)] bool systemKey);

        [Import(EntryPoint = "wkeFireKeyDownEvent", CallingConvention = CallingConvention.Cdecl)]
        bool FireKeyDownEvent(IntPtr webView, int virtualKeyCode, uint flags,
            [MarshalAs(UnmanagedType.I1)] bool systemKey);

        [Import(EntryPoint = "wkeFireKeyUpEvent", CallingConvention = CallingConvention.Cdecl)]
        bool FireKeyUpEvent(IntPtr webView, int virtualKeyCode, uint flags,
            [MarshalAs(UnmanagedType.I1)] bool systemKey);

        [Import(EntryPoint = "wkeOnNavigation", CallingConvention = CallingConvention.Cdecl)]
        void OnNavigation(IntPtr webView, wkeNavigationCallback callback, IntPtr param);

        #endregion
    }
}