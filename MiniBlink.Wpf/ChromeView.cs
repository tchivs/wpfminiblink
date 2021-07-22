using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MiniBlink.Share;

namespace MiniBlink.Wpf
{
    public sealed partial class ChromeView : HeaderedContentControl, IDisposable
    {
        #region fields

        private int _width, _heigth;

        private readonly IntPtr MiniblinkHandle;

        #endregion

        #region static

        private static readonly IMiniBlinkProxy Api;

        static ChromeView()
        {
            Api = Share.MiniBlink.Create();
        }

        #endregion

        #region ctor

        public ChromeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChromeView),
                new FrameworkPropertyMetadata(typeof(ChromeView)));
            if (!Api.IsInitialize())
            {
                throw new ExternalException(nameof(IMiniBlinkProxy));
            }

            WebViewIsInitialize = Api.IsInitialize();
            if (WebViewIsInitialize)
            {
                MiniblinkHandle = Api.CreateWebView();
                this.SynchronizationEvent(MiniblinkHandle);
            }

            this.Loaded += OnLoaded;
        }

        #endregion


        #region eventHandle

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Api.LoadURL(this.MiniblinkHandle, "http://www.baidu.com");
        }

        #region webViewEvent

        private void RaiseTitleChanged(IntPtr webview, IntPtr param, IntPtr title)
        {
            this.Title = Api.GetString(title);
            TitleChangedEventArgs args = new TitleChangedEventArgs(TitleChangedEvent, this, Title);
            this.RaiseEvent(args);
        }

        System.Drawing.Size oldSize;

        /// <summary>
        /// 触发渲染
        /// </summary>
        /// <param name="webview"></param>
        /// <param name="param"></param>
        /// <param name="hdc"></param>
        /// <param name="r"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void RaisePaintBitUpdated(IntPtr webview, IntPtr param, IntPtr hdc, ref wkeRect r, int width,
            int height)
        {
            if (width > 0 && height > 0)
            {
                _width = (int) ActualWidth;
                try
                {
                    ImageSource.WritePixels(
                        new System.Windows.Int32Rect(0, 0, ImageSource.PixelWidth, ImageSource.PixelHeight),
                        hdc, width * height * 4, width * 4);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                // _heigth = (int)ActualHeight;
                // if (oldSize.Width == (int) width && oldSize.Height == height && this.ImageSource != null)
                // {
                //     ImageSource.WritePixels(
                //         new System.Windows.Int32Rect(0, 0, ImageSource.PixelWidth, ImageSource.PixelHeight),
                //         hdc, width * height * 4, width * 4);
                // }
                // else
                // {
                //     // BitmapPalette palette = new BitmapPalette(new List<System.Windows.Media.Color>()
                //     //     {Colors.Blue, Colors.Green, Colors.Red});
                //     BitmapSource src = BitmapSource.Create((int) width, height, 96, 96, PixelFormats.Bgra32, null,
                //         hdc, (int) width * (int) height * 4, (int) width * 4);
                //     ImageSource = new WriteableBitmap(src);
                //     oldSize = new System.Drawing.Size((int) width, height);
                // }
            }
        }

        #endregion

        #endregion

        #region private Methods

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void SynchronizationEvent(IntPtr ptr)
        {
            Api.OnTitleChanged(ptr, RaiseTitleChanged, IntPtr.Zero);
            Api.OnPaintBitUpdated(ptr, RaisePaintBitUpdated, IntPtr.Zero);
        }


        /// <summary>
        /// 卸载事件
        /// </summary>
        /// <param name="ptr"></param>
        private void UnSynchronizationEvent(IntPtr ptr)
        {
            Api.OnTitleChanged(ptr, null, IntPtr.Zero);
        }

        #endregion

        #region methods

        #endregion

        #region overloaded

        /// <summary>
        /// 大小被改变时同步变化数据
        /// </summary>
        /// <param name="sizeInfo"></param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            _width = (int) ActualWidth;
            _heigth = (int) ActualHeight;
            if (MiniblinkHandle != IntPtr.Zero)
            {
                try
                {
                    this.ImageSource = new WriteableBitmap(_width, _heigth, 96, 96, PixelFormats.Bgra32, null);
                    Api.Resize(MiniblinkHandle, _width, _heigth);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        #endregion


        #region Disposable

        private bool _disposedValue;

        private void Dispose(bool disposing)
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

        #endregion
    }
}