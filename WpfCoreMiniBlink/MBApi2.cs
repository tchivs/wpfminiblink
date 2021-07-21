using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Miniblink
{
   
    public class MBApi2
    {

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void TitleChangedCallback(IntPtr webView, IntPtr param, IntPtr title);

        private const string DLL_x86 = "node_x86.dll";
        private const string DLL_x64 = "node_x64.dll";
        private static bool is64()
        {
            return IntPtr.Size == 8;
        }

        [DllImport(DLL_x86, EntryPoint = "wkeGetString", CallingConvention = CallingConvention.Cdecl)]
        private static extern string wkeGetString_x86(IntPtr wkeString);
        [DllImport(DLL_x64, EntryPoint = "wkeGetString", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8Marshal))]
        private static extern string wkeGetString_x64(IntPtr wkeString);

        public static string wkeGetString(IntPtr wkeString)
        {
            if (is64())
            {
                return wkeGetString_x64(wkeString);
            }

            return wkeGetString_x86(wkeString);
        }
        [DllImport(DLL_x86, EntryPoint = "wkeIsInitialize", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool wkeIsInitialize_x86();
        [DllImport(DLL_x64, EntryPoint = "wkeIsInitialize", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool wkeIsInitialize_x64();

        public static bool wkeIsInitialize()
        {
           

            return wkeIsInitialize_x64();
        }
        [DllImport(DLL_x86, EntryPoint = "wkeOnLoadUrlBegin", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnLoadUrlBegin_x86(IntPtr webView, wkeLoadUrlBeginCallback callback, IntPtr param);
        [DllImport(DLL_x64, EntryPoint = "wkeOnLoadUrlBegin", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeOnLoadUrlBegin_x64(IntPtr webView, wkeLoadUrlBeginCallback callback, IntPtr param);

        public static void wkeOnLoadUrlBegin(IntPtr webView, wkeLoadUrlBeginCallback callback, IntPtr param)
        {
            if (is64())
            {
                wkeOnLoadUrlBegin_x64(webView, callback, param);
            }
            else
            {
                wkeOnLoadUrlBegin_x86(webView, callback, param);
            }
        }
        [DllImport(DLL_x86, EntryPoint = "wkeInitialize", CallingConvention = CallingConvention.Cdecl)]
        public static extern void wkeInitialize_x86();
        [DllImport(DLL_x64, EntryPoint = "wkeInitialize", CallingConvention = CallingConvention.Cdecl)]
        public static extern void wkeInitialize_x64();

        public static void wkeInitialize()
        {
            var mb = typeof(MBApi2).GetMethods();
            wkeInitialize_x64();
            
        }

        //[DllImport(DLL_x86, EntryPoint = "wkeInitializeEx", CallingConvention = CallingConvention.Cdecl)]
        //private static extern void wkeInitializeEx_x86(WKESettings settings);
        //[DllImport(DLL_x64, EntryPoint = "wkeInitializeEx", CallingConvention = CallingConvention.Cdecl)]
        //private static extern void wkeInitializeEx_x64(WKESettings settings);

        //public static void wkeInitializeEx(WKESettings settings)
        //{
        //    if (is64())
        //    {
        //        wkeInitializeEx_x64(settings);
        //    }
        //    else
        //    {
        //        wkeInitializeEx_x86(settings);
        //    }
        //}

        //[DllImport(DLL_x86, EntryPoint = "wkeFinalize", CallingConvention = CallingConvention.Cdecl)]
        //public static extern void wkeFinalize();

        //[DllImport(DLL_x86, EntryPoint = "wkeConfigure", CallingConvention = CallingConvention.Cdecl)]
        //public static extern void wkeConfigure(WKESettings settings);

        //[DllImport(DLL_x86, EntryPoint = "wkeSetDebugConfig", CallingConvention = CallingConvention.Cdecl,
        //    CharSet = CharSet.Unicode)]
        //public static extern void wkeSetDebugConfig(IntPtr webView, string debugString, string param);

        [DllImport(DLL_x86, EntryPoint = "wkeSetTouchEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetTouchEnabled_x86(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool enable);
        [DllImport(DLL_x64, EntryPoint = "wkeSetTouchEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetTouchEnabled_x64(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool enable);

        public static void wkeSetTouchEnabled(IntPtr webView, bool enable)
        {
            
                wkeSetTouchEnabled_x86(webView, enable);
            
        }

        [DllImport(DLL_x86, EntryPoint = "wkeSetMouseEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetMouseEnabled_x86(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool enable);
        [DllImport(DLL_x64, EntryPoint = "wkeSetMouseEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetMouseEnabled_x64(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool enable);

        public static void wkeSetMouseEnabled(IntPtr webView, bool enable)
        {
           
                wkeSetMouseEnabled_x64(webView, enable);
            
        }

        [DllImport(DLL_x86, EntryPoint = "wkeSetDeviceParameter", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern void wkeSetDeviceParameter_x86(IntPtr webView, string device, string s, int i, float f);
        [DllImport(DLL_x64, EntryPoint = "wkeSetDeviceParameter", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern void wkeSetDeviceParameter_x64(IntPtr webView, string device, string s, int i, float f);

        public static void wkeSetDeviceParameter(IntPtr webView, string type, string s, int i, float f)
        {
            
                wkeSetDeviceParameter_x86(webView, type, s, i, f);
            
        }

        //[DllImport(DLL_x86, EntryPoint = "wkeGetVersion", CallingConvention = CallingConvention.Cdecl)]
        //public static extern uint wkeGetVersion();

        //[DllImport(DLL_x86, EntryPoint = "wkeGetVersionString", CallingConvention = CallingConvention.Cdecl)]
        //public static extern IntPtr wkeGetVersionString();

        [DllImport(DLL_x86, EntryPoint = "wkeCreateWebView", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeCreateWebView_x86();
        [DllImport(DLL_x64, EntryPoint = "wkeCreateWebView", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wkeCreateWebView_x64();

        public static IntPtr wkeCreateWebView()
        {
          

            return wkeCreateWebView_x64();
        }

        //[DllImport(DLL_x86, EntryPoint = "wkeGetWebView", CallingConvention = CallingConvention.Cdecl,
        //    CharSet = CharSet.Ansi)]
        //public static extern IntPtr wkeGetWebView(string name);

        //[DllImport(DLL_x86, EntryPoint = "wkeDestroyWebView", CallingConvention = CallingConvention.Cdecl)]
        //public static extern void wkeDestroyWebView(IntPtr webView);

        //[DllImport(DLL_x86, EntryPoint = "wkeSetMemoryCacheEnable", CallingConvention = CallingConvention.Cdecl)]
        //public static extern void wkeSetMemoryCacheEnable(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);


        [DllImport(DLL_x86, EntryPoint = "wkeOnTitleChanged", CallingConvention = CallingConvention.Cdecl)]
        public static extern void wkeOnTitleChanged_x86(IntPtr webView, TitleChangedCallback callback, IntPtr callbackParam);
        [DllImport(DLL_x64, EntryPoint = "wkeOnTitleChanged", CallingConvention = CallingConvention.Cdecl)]
        public static extern void wkeOnTitleChanged_x64(IntPtr webView, TitleChangedCallback callback, IntPtr callbackParam);

        public static void wkeOnTitleChanged(IntPtr webView, TitleChangedCallback callback, IntPtr callbackParam)
        {
            
                wkeOnTitleChanged_x64(webView, callback, callbackParam);
            
        }


        [DllImport(DLL_x86, EntryPoint = "wkeOnMouseOverUrlChanged", CallingConvention = CallingConvention.Cdecl)]
        public static extern void wkeOnMouseOverUrlChanged_x86(IntPtr webView, TitleChangedCallback callback, IntPtr callbackParam);
        [DllImport(DLL_x64, EntryPoint = "wkeOnMouseOverUrlChanged", CallingConvention = CallingConvention.Cdecl)]
        public static extern void wkeOnMouseOverUrlChanged_x64(IntPtr webView, TitleChangedCallback callback, IntPtr callbackParam);

        public static void wkeOnMouseOverUrlChanged(IntPtr webView, TitleChangedCallback callback, IntPtr callbackParam)
        {
           
                wkeOnMouseOverUrlChanged_x86(webView, callback, callbackParam);
            
        }


        [DllImport(DLL_x86, EntryPoint = "wkeSetNavigationToNewWindowEnable", CallingConvention = CallingConvention.Cdecl)]
        public static extern void wkeSetNavigationToNewWindowEnable_x86(IntPtr webView,
            [MarshalAs(UnmanagedType.I1)] bool b);

        [DllImport(DLL_x64, EntryPoint = "wkeSetNavigationToNewWindowEnable", CallingConvention = CallingConvention.Cdecl)]
        public static extern void wkeSetNavigationToNewWindowEnable_x64(IntPtr webView,
            [MarshalAs(UnmanagedType.I1)] bool b);

        public static void wkeSetNavigationToNewWindowEnable(IntPtr webView, bool enable)
        {
            
                wkeSetNavigationToNewWindowEnable_x86(webView, enable);
            
        }

        [DllImport(DLL_x86, EntryPoint = "wkeSetNpapiPluginsEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetNpapiPluginsEnabled_x86(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);
        [DllImport(DLL_x64, EntryPoint = "wkeSetNpapiPluginsEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetNpapiPluginsEnabled_x64(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        public static void wkeSetNpapiPluginsEnabled(IntPtr webView, bool enable)
        {
            
                wkeSetNpapiPluginsEnabled_x86(webView, enable);
            
        }

        [DllImport(DLL_x86, EntryPoint = "wkeSetHeadlessEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetHeadlessEnabled_x86(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);
        [DllImport(DLL_x64, EntryPoint = "wkeSetHeadlessEnabled", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetHeadlessEnabled_x64(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        public static void wkeSetHeadlessEnabled(IntPtr webView, bool enable)
        {
           
                wkeSetHeadlessEnabled_x86(webView, enable);
            
        }

        [DllImport(DLL_x86, EntryPoint = "wkeSetCspCheckEnable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetCspCheckEnable_x86(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);
        [DllImport(DLL_x64, EntryPoint = "wkeSetCspCheckEnable", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeSetCspCheckEnable_x64(IntPtr webView, [MarshalAs(UnmanagedType.I1)] bool b);

        public static void wkeSetCspCheckEnable(IntPtr webView, bool enable)
        {
            
                wkeSetCspCheckEnable_x86(webView, enable);
            
        }

        //[DllImport(DLL_x86, EntryPoint = "wkeSetProxy", CallingConvention = CallingConvention.Cdecl)]
        //private static extern void wkeSetProxy_x86(WKEProxy proxy);
        //[DllImport(DLL_x64, EntryPoint = "wkeSetProxy", CallingConvention = CallingConvention.Cdecl)]
        //private static extern void wkeSetProxy_x64(WKEProxy proxy);

        //public static void wkeSetProxy(WKEProxy proxy)
        //{
        //    if (is64())
        //    {
        //        wkeSetProxy_x64(proxy);
        //    }
        //    else
        //    {
        //        wkeSetProxy_x86(proxy);
        //    }
        //}

       

        [DllImport(DLL_x86, EntryPoint = "wkeEditorCut", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeEditorCut_x86(IntPtr webView);
        [DllImport(DLL_x64, EntryPoint = "wkeEditorCut", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeEditorCut_x64(IntPtr webView);

        public static void wkeEditorCut(IntPtr webView)
        {
      
                wkeEditorCut_x86(webView);
            
        }
         

        [DllImport(DLL_x86, EntryPoint = "jsUndefined", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsUndefined_x86();
        [DllImport(DLL_x64, EntryPoint = "jsUndefined", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsUndefined_x64();

        public static long jsUndefined()
        {
            return  jsUndefined_x86();
        }

        //[DllImport(DLL_x86, EntryPoint = "jsNull", CallingConvention = CallingConvention.Cdecl)]
        //public static extern long jsNull();

        //[DllImport(DLL_x86, EntryPoint = "jsTrue", CallingConvention = CallingConvention.Cdecl)]
        //public static extern long jsTrue();

        //[DllImport(DLL_x86, EntryPoint = "jsFalse", CallingConvention = CallingConvention.Cdecl)]
        //public static extern long jsFalse();

        [DllImport(DLL_x86, EntryPoint = "jsString", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        private static extern long jsString_x86(IntPtr es, string str);

        [DllImport(DLL_x64, EntryPoint = "jsString", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        private static extern long jsString_x64(IntPtr es, string str);

        public static long jsString(IntPtr es, string str)
        {
             

            return jsString_x86(es, str);
        }
 

        [DllImport(DLL_x86, EntryPoint = "jsSetLength", CallingConvention = CallingConvention.Cdecl)]
        private static extern void jsSetLength_x86(IntPtr es, long jsValue, int length);
        [DllImport(DLL_x64, EntryPoint = "jsSetLength", CallingConvention = CallingConvention.Cdecl)]
        private static extern void jsSetLength_x64(IntPtr es, long jsValue, int length);

        public static void jsSetLength(IntPtr es, long jsValue, int length)
        {
            
                jsSetLength_x86(es, jsValue, length);
             
        }

        //[DllImport(DLL_x86, EntryPoint = "jsGlobalObject", CallingConvention = CallingConvention.Cdecl)]
        //public static extern long jsGlobalObject(IntPtr es);

        [DllImport(DLL_x86, EntryPoint = "jsGetWebView", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr jsGetWebView_x86(IntPtr es);
        [DllImport(DLL_x64, EntryPoint = "jsGetWebView", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr jsGetWebView_x64(IntPtr es);

        public static IntPtr jsGetWebView(IntPtr es)
        {
            return   jsGetWebView_x86(es);
        }

        //[DllImport(DLL_x86, EntryPoint = "jsEvalW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        //public static extern long jsEvalW(IntPtr es, string str);

        [DllImport(DLL_x86, EntryPoint = "jsEvalExW", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        private static extern long jsEvalExW_x86(IntPtr es, string str, [MarshalAs(UnmanagedType.I1)] bool isInClosure);

        [DllImport(DLL_x64, EntryPoint = "jsEvalExW", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        private static extern long jsEvalExW_x64(IntPtr es, string str, [MarshalAs(UnmanagedType.I1)] bool isInClosure);

        public static long jsEvalExW(IntPtr es, string str, bool isInClosure)
        {
            return  jsEvalExW_x86(es, str, isInClosure);
        }

        [DllImport(DLL_x86, EntryPoint = "jsCall", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsCall_x86(IntPtr es, long func, long thisObject,
            [MarshalAs(UnmanagedType.LPArray)] long[] args, int argCount);

        [DllImport(DLL_x64, EntryPoint = "jsCall", CallingConvention = CallingConvention.Cdecl)]
        private static extern long jsCall_x64(IntPtr es, long func, long thisObject,
            [MarshalAs(UnmanagedType.LPArray)] long[] args, int argCount);

        public static long jsCall(IntPtr es, long func, long thisObject, long[] args, int argCount)
        {
            return is64()
                ? jsCall_x64(es, func, thisObject, args, argCount)
                : jsCall_x86(es, func, thisObject, args, argCount);
        }

        //[DllImport(DLL_x86, EntryPoint = "jsCallGlobal", CallingConvention = CallingConvention.Cdecl)]
        //public static extern long jsCallGlobal(IntPtr es, long func, [MarshalAs(UnmanagedType.LPArray)] long[] args,
        //    int argCount);

        [DllImport(DLL_x86, EntryPoint = "jsGetGlobal", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern long jsGetGlobal_x86(IntPtr es, string prop);

        [DllImport(DLL_x64, EntryPoint = "jsGetGlobal", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern long jsGetGlobal_x64(IntPtr es, string prop);

        public static long jsGetGlobal(IntPtr es, string prop)
        {
            return   jsGetGlobal_x86(es, prop);
        }

        [DllImport(DLL_x86, EntryPoint = "jsSetGlobal", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern void jsSetGlobal_x86(IntPtr es, string prop, long jsValue);

        [DllImport(DLL_x64, EntryPoint = "jsSetGlobal", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern void jsSetGlobal_x64(IntPtr es, string prop, long jsValue);

        public static void jsSetGlobal(IntPtr es, string prop, long jsValue)
        {
            if (is64())
            {
                jsSetGlobal_x64(es, prop, jsValue);
            }
            else
            {
                jsSetGlobal_x86(es, prop, jsValue);
            }
        }

        //[DllImport(DLL_x86, EntryPoint = "jsGC", CallingConvention = CallingConvention.Cdecl)]
        //public static extern void jsGC();

        [DllImport(DLL_x86, EntryPoint = "wkeShowDevtools", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        private static extern void wkeShowDevtools_x86(IntPtr webView, string path, wkeOnShowDevtoolsCallback callback,
            IntPtr param);
        [DllImport(DLL_x64, EntryPoint = "wkeShowDevtools", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        private static extern void wkeShowDevtools_x64(IntPtr webView, string path, wkeOnShowDevtoolsCallback callback,
            IntPtr param);

        public static void wkeShowDevtools(IntPtr webView, string path, wkeOnShowDevtoolsCallback callback,
            IntPtr param)
        {
            
                wkeShowDevtools_x86(webView, path, callback, param);
            
        }

        [DllImport(DLL_x86, EntryPoint = "wkeNetGetHTTPHeaderFieldFromResponse",
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern IntPtr wkeNetGetHTTPHeaderFieldFromResponse_x86(IntPtr job, string key);

        [DllImport(DLL_x64, EntryPoint = "wkeNetGetHTTPHeaderFieldFromResponse",
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern IntPtr wkeNetGetHTTPHeaderFieldFromResponse_x64(IntPtr job, string key);

        public static IntPtr wkeNetGetHTTPHeaderFieldFromResponse(IntPtr job, string key)
        {
            return is64()
                ? wkeNetGetHTTPHeaderFieldFromResponse_x64(job, key)
                : wkeNetGetHTTPHeaderFieldFromResponse_x86(job, key);
        }

        [DllImport(DLL_x86, EntryPoint = "wkeNetGetUrlByJob", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        private static extern IntPtr wkeNetGetUrlByJob_x86(IntPtr job);

        [DllImport(DLL_x64, EntryPoint = "wkeNetGetUrlByJob", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        private static extern IntPtr wkeNetGetUrlByJob_x64(IntPtr job);

        public static IntPtr wkeNetGetUrlByJob(IntPtr job)
        {
            return is64() ? wkeNetGetUrlByJob_x64(job) : wkeNetGetUrlByJob_x86(job);
        }

        [DllImport(DLL_x86, EntryPoint = "wkeNetGetMIMEType", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern IntPtr wkeNetGetMIMEType_x86(IntPtr job, IntPtr mime);

        [DllImport(DLL_x64, EntryPoint = "wkeNetGetMIMEType", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi)]
        private static extern IntPtr wkeNetGetMIMEType_x64(IntPtr job, IntPtr mime);

        public static IntPtr wkeNetGetMIMEType(IntPtr job)
        {
            return is64() ? wkeNetGetMIMEType_x64(job, IntPtr.Zero) : wkeNetGetMIMEType_x86(job, IntPtr.Zero);
        }

        [DllImport(DLL_x86, EntryPoint = "wkeNetSetHTTPHeaderField", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        private static extern void wkeNetSetHTTPHeaderField_x86(IntPtr job, string key, string value, [MarshalAs(UnmanagedType.I1)] bool response);

        [DllImport(DLL_x64, EntryPoint = "wkeNetSetHTTPHeaderField", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        private static extern void wkeNetSetHTTPHeaderField_x64(IntPtr job, string key, string value, [MarshalAs(UnmanagedType.I1)] bool response);

        public static void wkeNetSetHTTPHeaderField(IntPtr job, string key, string value)
        {
            if (is64())
            {
                wkeNetSetHTTPHeaderField_x64(job, key, value, false);
            }
            else
            {
                wkeNetSetHTTPHeaderField_x86(job, key, value, false);
            }
        }

        [DllImport(DLL_x86, EntryPoint = "wkeLoadURL", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        private static extern void wkeLoadURL_x86(IntPtr webView, string url);

        [DllImport(DLL_x64, EntryPoint = "wkeLoadURL", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        private static extern void wkeLoadURL_x64(IntPtr webView, string url);

        public static void wkeLoadURL(IntPtr webView, string url)
        {
            if (is64())
            {
                wkeLoadURL_x64(webView, url);
            }
            else
            {
                wkeLoadURL_x86(webView, url);
            }
        }

        [DllImport(DLL_x86, EntryPoint = "wkeGetUserAgent", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        private static extern IntPtr wkeGetUserAgent_x86(IntPtr webView);

        [DllImport(DLL_x64, EntryPoint = "wkeGetUserAgent", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        private static extern IntPtr wkeGetUserAgent_x64(IntPtr webView);

        public static IntPtr wkeGetUserAgent(IntPtr webView)
        {
            return is64() ? wkeGetUserAgent_x64(webView) : wkeGetUserAgent_x86(webView);
        }
        [DllImport(DLL_x86, EntryPoint = "wkeNetCancelRequest", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeNetCancelRequest_x86(IntPtr job);
        [DllImport(DLL_x64, EntryPoint = "wkeNetCancelRequest", CallingConvention = CallingConvention.Cdecl)]
        private static extern void wkeNetCancelRequest_x64(IntPtr job);

        public static void wkeNetCancelRequest(IntPtr job)
        {
            if (is64())
            {
                wkeNetCancelRequest_x64(job);
            }
            else
            {
                wkeNetCancelRequest_x86(job);
            }
        }

        [DllImport(DLL_x86, EntryPoint = "wkeInsertCSSByFrame", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        private static extern void wkeInsertCSSByFrame_x86(IntPtr webView, IntPtr frameId, string cssText);
        [DllImport(DLL_x64, EntryPoint = "wkeInsertCSSByFrame", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode)]
        private static extern void wkeInsertCSSByFrame_x64(IntPtr webView, IntPtr frameId, string cssText);

        public static void wkeInsertCSSByFrame(IntPtr webView, IntPtr frameId, string cssText)
        {
            if (is64())
            {
                wkeInsertCSSByFrame_x64(webView, frameId, cssText);
            }
            else
            {
                wkeInsertCSSByFrame_x86(webView, frameId, cssText);
            }
        }

        public static string Utf8IntptrToString(IntPtr ptr)
        {
            var data = new System.Collections.Generic.List<byte>();
            var off = 0;
            while (true)
            {
                var ch = Marshal.ReadByte(ptr, off++);
                if (ch == 0)
                {
                    break;
                }
                data.Add(ch);
            }
            return System.Text.Encoding.UTF8.GetString(data.ToArray());
        }
    }

    internal class Utf8Marshal: ICustomMarshaler
    {
        public static Utf8Marshal Instance = new Utf8Marshal(); public static ICustomMarshaler GetInstance(string cookie)
        {
            return Instance;
        }
        public void CleanUpManagedData(object ManagedObj)
        {
         }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            //对于const utf8应交由miniblink内部回收
        }

        public int GetNativeDataSize()
        {
            return -1;
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            if (ManagedObj is string str)
            {
                var data = Encoding.UTF8.GetBytes(str);
                IntPtr handle = Marshal.AllocHGlobal(data.Length + 1);
                Marshal.Copy(data, 0, handle, data.Length);
                Marshal.WriteByte(handle, data.Length, 0);
                return handle;
            }
            throw new InvalidOperationException();
        }

        public object MarshalNativeToManaged(IntPtr ptr)
        {
            var data = new List<byte>();
            var off = 0;
            while (true)
            {
                var ch = Marshal.ReadByte(ptr, off++);
                if (ch == 0)
                {
                    break;
                }
                data.Add(ch);
            }
            return Encoding.UTF8.GetString(data.ToArray());
        }
    }
}