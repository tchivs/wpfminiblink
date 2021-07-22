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
        private bool _isInit;
        private string _version;
        private string _title;

        public string Title
        {
            get => _title;
            set
            {
                this._title = value;
                this.OnPropertyChanged();
            }
        }

        public string Version
        {
            get => _version;
            set
            {
                this._version = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsInit
        {
            get => _isInit;
            set
            {
                _isInit = value;
                this.OnPropertyChanged();
            }
        }

        private IMiniBlinkProxy _proxy;

        public MainWindowViewModel()
        {
            if (IsDesignMode())
            {
                return;
            }

            _proxy = DllModuleBuilder.Create().Build<IMiniBlinkProxy>();
            _proxy.Initialize();
            IsInit = _proxy.IsInitialize();
            if (IsInit)
            {
                Version = _proxy.GetVersionString();
                this.ViewHandle = _proxy.CreateWebView();
            }

            CurrentPath = AppDomain.CurrentDomain.BaseDirectory;
            CacheDir = Path.Combine(CurrentPath, "Cache");
            if (!System.IO.Directory.Exists(CacheDir))
            {
                System.IO.Directory.CreateDirectory(CacheDir);
            }

            _proxy.OnTitleChanged(this.ViewHandle, OnTitleChanged, IntPtr.Zero);
            _proxy.OnCreateView(this.ViewHandle, OnCreateView, IntPtr.Zero);
            CookieFileName = CacheDir + "\\cookies.dat";
            _proxy.SetCookieJarFullPath(this.ViewHandle, CookieFileName);
            _proxy.SetLocalStorageFullPath(this.ViewHandle, CacheDir);
            _proxy.LoadURL(this.ViewHandle, "http://www.baidu.com");
            var code = _proxy.GetSource(this.ViewHandle);
        }

        private IntPtr OnCreateView(IntPtr webview, IntPtr param, wkeNavigationType navigationtype, IntPtr url, IntPtr windowfeatures)
        {
            
            return IntPtr.Zero;
        }

        private void OnTitleChanged(IntPtr webview, IntPtr param, IntPtr title)
        {
            this.Title = _proxy.GetString(title);
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