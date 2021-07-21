using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using AutoDllProxy;
using MiniBlink.Share;
using MiniBlink.WpfDemo.Annotations;

namespace MiniBlink.WpfDemo
{
    public class ChromeBrowser : Border, INotifyPropertyChanged, IDisposable
    {
        private static readonly IMiniBlinkProxy MiniBlinkProxy;
        private static IntPtr MiniblinkHandle;
        private bool _disposedValue;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        static ChromeBrowser()
        {
            MiniBlinkProxy = DllModuleBuilder.Create().Build<IMiniBlinkProxy>();
        }
          static bool IsDesignMode()
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

        public ChromeBrowser()
        {
            if (!MiniBlinkProxy.IsInitialize()&& !IsDesignMode())
            {//设置基础信息
                MiniBlinkProxy.Initialize();
                if (MiniBlinkProxy.IsInitialize())
                {
                    MiniblinkHandle = MiniBlinkProxy.CreateWebView();
                    if (MiniblinkHandle == IntPtr.Zero)
                    {
                        throw new NullReferenceException(nameof(MiniblinkHandle));
                    }

                    // MiniBlinkProxy.SetHandle(MiniblinkHandle,IntPtr.Zero);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                _disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~ChromeBrowser()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}