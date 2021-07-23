using System;
using AutoDllProxy;

namespace MiniBlink.Share
{
    public struct MiniBlink
    {
        IntPtr _handle;
        public static   IMiniBlinkProxy Proxy;

        public bool Equals(MiniBlink other)
            => other._handle == _handle;

        public static implicit operator MiniBlink(IntPtr ptr)
            => new MiniBlink() {_handle = ptr};

        public static implicit operator IntPtr(MiniBlink value)
            => value._handle;

        public bool HaveValue => _handle != IntPtr.Zero;

   

        public static bool IsGlobalInitialization = false;
        public static object GlobalRoot = new object();

        public static void Init(Platform platform)
        {

            if (!IsGlobalInitialization)
                lock (GlobalRoot)
                    if (!IsGlobalInitialization)
                    {
                        Proxy = AutoDllProxy.DllModuleBuilder.Create(platform).Build<IMiniBlinkProxy>();
                        Proxy.Initialize();
                        IsGlobalInitialization = Proxy.IsInitialize();
                    }
        }

        public static MiniBlink Create()
        {
            if (IsGlobalInitialization)
            {
                MiniBlink p = Proxy.CreateWebView();
                return p;
            }

            throw new ApplicationException($"{nameof(MiniBlink)}初始化失败");
        }
    }
}