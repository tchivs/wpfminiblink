using System;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Text;

namespace MiniBlink.Share
{
   
    public static class Ext
    {
        public static object ToValue(this long value, IntPtr es)
        {
            if (value == 0) return null;

            jsType type = MiniBlink.Proxy.JsTypeOf(value);
            switch (type)
            {
                case jsType.NULL:
                case jsType.UNDEFINED:
                    return null;
                case jsType.NUMBER:
                    return MiniBlink.Proxy.JsToDouble(es, value);
                case jsType.BOOLEAN:
                    return MiniBlink.Proxy.JsToBoolean(es, value);
                case jsType.STRING:
                    return MiniBlink.Proxy.JsToTempStringW(es, value);
                case jsType.FUNCTION:
                    // return new JsFunc(new JsFuncWapper(value, es).Call);
                    return null;
                case jsType.ARRAY:
                    var len = MiniBlink.Proxy.jsGetLength(es, value);
                    var array = new object[len];
                    for (var i = 0; i < array.Length; i++)
                    {
                        array[i] = MiniBlink.Proxy.jsGetAt(es, value, i).ToValue(es);
                    }

                    return array;
                case jsType.OBJECT:
                    var ptr = MiniBlink.Proxy.jsGetKeys(es, value);
                    var jskeys = (jsKeys)Marshal.PtrToStructure(ptr, typeof(jsKeys));
                    // var keys = Utils.PtrToStringArray(jskeys.keys, jskeys.length);
                    // var exp = new ExpandoObject();
                    // var map = (IDictionary<string, object>)exp;
                    // foreach (var k in keys)
                    // {
                    //     map.Add(k, MiniBlink.Proxy.jsGet(es, value, k).ToValue(es));
                    // }

                    return jskeys;
                default:
                    throw new NotSupportedException();
            }
        }
    }
    public class FrameContext
    {
        public IntPtr Id { get; }
        public bool IsMain { get; }
        public string Url { get; }
        public bool IsRemote { get; }
        private IntPtr _mb;
        private static IMiniBlinkProxy p => MiniBlink.Proxy;
        internal FrameContext(IntPtr webView, IntPtr frameId)
        {
          
            _mb = webView;
            Id = frameId;
            IsMain = p.IsMainFrame(_mb, frameId);
            Url = p.GetFrameUrl(_mb, frameId);
            IsRemote = p.IsWebRemoteFrame(_mb, frameId);
        }

        public object RunJs(string script)
        {
            var es = p.GetGlobalExecByFrame(_mb, Id);
           return p.JsEvalExW(es, script, true).ToValue(es);
 
        }
        
        public void InsertCss(string cssText)
        {
            p.InsertCSSByFrame(_mb, Id, cssText);
        }
    }
}