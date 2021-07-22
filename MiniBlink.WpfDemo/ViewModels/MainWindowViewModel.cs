using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using AutoDllProxy;
using MiniBlink.Share;
using MiniBlink.WpfDemo.Annotations;

namespace MiniBlink.WpfDemo.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string CookieFileName { get; set; }
        public string CurrentPath { get; set; }
        public string CacheDir { get; set; }
        
        public MainWindowViewModel()
        {
            if (IsDesignMode())
            {
                return;
            }

            // _proxy = DllModuleBuilder.Create().Build<IMiniBlinkProxy>();
            // _proxy.Initialize();
            // IsInit = _proxy.IsInitialize();
            // if (IsInit)
            // {
            //     Version = _proxy.GetVersionString();
            //     this.ViewHandle = _proxy.CreateWebView();
            // }
            //
            // CurrentPath = AppDomain.CurrentDomain.BaseDirectory;
            // CacheDir = Path.Combine(CurrentPath, "Cache");
            // if (!System.IO.Directory.Exists(CacheDir))
            // {
            //     System.IO.Directory.CreateDirectory(CacheDir);
            // }
            //
            // _proxy.OnTitleChanged(this.ViewHandle, OnTitleChanged, IntPtr.Zero);
            // _proxy.OnCreateView(this.ViewHandle, OnCreateView, IntPtr.Zero);
            // _proxy.OnPaintBitUpdated(this.ViewHandle,OnPaintBitUpdated,IntPtr.Zero);
            // _proxy.SetDragEnable(ViewHandle, false);
            // _proxy.SetDragDropEnable(ViewHandle, false);
            // CookieFileName = CacheDir + "\\cookies.dat";
            // _proxy.SetCookieJarFullPath(this.ViewHandle, CookieFileName);
            // _proxy.SetLocalStorageFullPath(this.ViewHandle, CacheDir);
            // _proxy.SetNavigationToNewWindowEnable(this.ViewHandle, true);
            // _proxy.LoadURL(this.ViewHandle, "http://www.baidu.com");
            // var code = _proxy.GetSource(this.ViewHandle);
            // if (!IsDesignMode() && ViewHandle != IntPtr.Zero)
            // {
            //     _proxy.Resize(ViewHandle, (int)200, (int)200);
            // }
        }

    
        private void OnPaintBitUpdated(IntPtr webview, IntPtr param, IntPtr hdc, ref wkeRect r, int width, int height)
        {
             
        }

        private IntPtr OnCreateView(IntPtr webview, IntPtr param, wkeNavigationType navigationtype, IntPtr url, IntPtr windowfeatures)
        {
            
            return IntPtr.Zero;
        }

         

        public static bool IsDesignMode()
        {
            var returnFlag = false;

#if DEBUG
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                returnFlag = true;
            }
            else if (Process.GetCurrentProcess().ProcessName == "devenv")
            {
                returnFlag = true;
            }
#endif

            return returnFlag;
        }

        public IntPtr ViewHandle { get; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}