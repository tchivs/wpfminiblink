using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AutoDllProxy;
using MiniBlink.Share;
using MiniBlink.Share.Events;
using MiniBlink.Wpf.Ime;

namespace MiniBlink.Wpf
{
    public sealed partial class ChromeView : HeaderedContentControl, IDisposable
    {
        #region fields

        System.Drawing.Size oldSize;

        // private int _width;
        // private int _height;
        Window _sourceWindow;
        readonly IWpfKeyboardHandler _wpfKeyboardHandler;

        public Share.MiniBlink Handle { get; private set; }

        #endregion

        #region static

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

        private static IMiniBlinkProxy Api;


        public static bool IsExtendedKey(Key key)
        {
            switch (key)
            {
                case Key.Insert:
                case Key.Delete:
                case Key.Home:
                case Key.End:
                case Key.Prior:
                case Key.Next:
                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                    return true;
                default:
                    return false;
            }
        }

        private static uint GetMouseFlags(MouseEventArgs e)
        {
            uint flags = 0;


            if (e.LeftButton == MouseButtonState.Pressed)
            {
                flags = flags | (uint) wkeMouseFlags.WKE_LBUTTON;
            }
            else if (e.MiddleButton == MouseButtonState.Pressed)
            {
                flags = flags | (uint) wkeMouseFlags.WKE_MBUTTON;
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                flags = flags | (uint) wkeMouseFlags.WKE_RBUTTON;
            }

            //判断键盘按键
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                flags = flags | (uint) wkeMouseFlags.WKE_CONTROL;
            }

            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                flags = flags | (uint) wkeMouseFlags.WKE_SHIFT;
            }

            return flags;
        }

        static ChromeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChromeView),
                new FrameworkPropertyMetadata(typeof(ChromeView)));
        }

        #endregion

        #region ctor

        public ChromeView()
        {
            //强制允许控件获取焦点
            Focusable = true;
            //去除选中焦点样式
            FocusVisualStyle = null;

            // this.DefaultStyleKey = typeof(ChromeView);
            if (IsDesignMode())
            {
                return;
            }
            Share.MiniBlink.Init(Platform.Any);
            Handle = Share.MiniBlink.Create()
                .SetNavigationToNewWindowEnable(false)
                .SetDragEnable(false).ShowDevTools()
                .SetDragDropEnable(false);
            Api = Share.MiniBlink.Proxy;
            WebViewIsInitialize = Share.MiniBlink.IsGlobalInitialization;
            _wpfKeyboardHandler = new WpfImeKeyboardHandler(this);
            PresentationSource.AddSourceChangedHandler(this, PresentationSourceChangedHandler);
            if (WebViewIsInitialize)
            {
                this.SynchronizationEvent(Handle);
            }

            this.Loaded += OnLoaded;
        }

        #endregion


        #region eventHandle

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
        }


        private void RaiseTitleChanged(IntPtr webView, IntPtr param, IntPtr title)
        {
            this.Title = Api.GetString(title);
            TitleChangedEventArgs args = new TitleChangedEventArgs(TitleChangedEvent, this, Title);
            this.RaiseEvent(args);
        }

        private void RaiseUrlChangedEvent(OnUrlChangedEventArgs onUrlChangedEventArgs)
        {
            this.RaiseEvent(onUrlChangedEventArgs);
            if (this.WebViewIsInitialize)
            {
                this.Handle.LoadUrl(onUrlChangedEventArgs.Url);
            }
        }

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
                if (oldSize.Width == width && oldSize.Height == height && ImageSource != null)
                {
                    ImageSource.WritePixels(
                        new Int32Rect(0, 0, ImageSource.PixelWidth, ImageSource.PixelHeight), hdc,
                        width * height * 4, width * 4);
                }
                else
                {
                    // BitmapPalette palette = new BitmapPalette(new List<System.Windows.Media.Color>()
                    //     {Colors.Blue, Colors.Green, Colors.Red});
                    BitmapSource src = BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32,
                        null, hdc, width * height * 4, width * 4);
                    ImageSource = new WriteableBitmap(src);
                    // this.ImageSource = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
                    oldSize = new System.Drawing.Size(width, height);
                }
            }

            // if (width > 0 && height > 0)
            // {
            //     ImageSource.WritePixels(
            //         new System.Windows.Int32Rect(0, 0, ImageSource.PixelWidth, ImageSource.PixelHeight),
            //         hdc, width * height * 4, width * 4);
            // }
        }

        #endregion


        #region private Methods

        wkeTitleChangedCallback _titleChangeCallback;
        wkePaintBitUpdatedCallback _paintBitUpdatedCallback;
        private wkeTitleChangedCallback _mouseOverUrlChangedCallback;

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void SynchronizationEvent(IntPtr ptr)
        {
            //事件必须先定义为委托字段，不可以直接注册， 否则会报错
            //标题
            this._titleChangeCallback = RaiseTitleChanged;
            Api.OnTitleChanged(ptr, _titleChangeCallback, IntPtr.Zero);
            //设置鼠标
            _mouseOverUrlChangedCallback = (view, intPtr, title) => { SetCursors(); };
            Api.OnMouseOverUrlChanged(ptr, _mouseOverUrlChangedCallback, IntPtr.Zero);
            //绘图
            _paintBitUpdatedCallback = RaisePaintBitUpdated;
            Api.OnPaintBitUpdated(ptr, _paintBitUpdatedCallback, IntPtr.Zero);
        }

        /// <summary>
        /// 卸载事件
        /// </summary>
        /// <param name="ptr"></param>
        private void UnSynchronizationEvent(IntPtr ptr)
        {
            Api.OnTitleChanged(ptr, null, IntPtr.Zero);
            Api.OnTitleChanged(ptr, null, IntPtr.Zero);
            Api.OnMouseOverUrlChanged(ptr, null, IntPtr.Zero);
        }

        private void PresentationSourceChangedHandler(object sender, System.Windows.SourceChangedEventArgs args)
        {
            if (args.NewSource != null)
            {
                var source = (HwndSource) args.NewSource;

                _wpfKeyboardHandler.Setup(source);
                // var matrix = source.CompositionTarget.TransformToDevice;
                if (source.RootVisual is Window window)
                {
                    _sourceWindow = window;
                }
            }
            else if (args.OldSource != null)
            {
                _wpfKeyboardHandler.Dispose();
                if (args.OldSource.RootVisual is Window window)
                {
                    _sourceWindow = null;
                }
            }
        }

        public void TextInputPress(string text)
        {
            if (Handle != IntPtr.Zero)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    Api.FireKeyPressEvent(Handle, text[i], 0, false);
                }
            }
        }

        internal void MoveImeWindow(IntPtr hwnd, bool systemCaret, int languageCodeId)
        {
            var hIMC = ImeNative.ImmGetContext(hwnd);
            var rc = Api.GetCaretRect(Handle);
            var x = rc.x + rc.w;
            var y = rc.y + rc.h;
            const int kCaretMargin = 1;
            var candidatePosition = new ImeNative.CANDIDATEFORM
            {
                dwIndex = 0,
                dwStyle = (int) ImeNative.CFS_CANDIDATEPOS,
                ptCurrentPos = new ImeNative.POINT(x, y),
                rcArea = new ImeNative.RECT(0, 0, 0, 0)
            };
            ImeNative.ImmSetCandidateWindow(hIMC, ref candidatePosition);
            if (systemCaret)
            {
                ImeNative.SetCaretPos(x, y);
            }

            if (languageCodeId == ImeNative.LANG_KOREAN)
            {
                y += kCaretMargin;
            }

            var excludeRectangle = new ImeNative.CANDIDATEFORM
            {
                dwIndex = 0,
                dwStyle = (int) ImeNative.CFS_EXCLUDE,
                ptCurrentPos = new ImeNative.POINT(x, y),
                rcArea = new ImeNative.RECT(rc.x, rc.y, x, y + kCaretMargin)
            };
            ImeNative.ImmSetCandidateWindow(hIMC, ref excludeRectangle);

            ImeNative.ImmReleaseContext(hwnd, hIMC);
        }

        /// <summary>
        /// 设置鼠标指针
        /// </summary>
        void SetCursors()
        {
            switch (Api.GetCursorInfoType(Handle))
            {
                case wkeCursorInfo.Pointer:
                    Cursor = null;
                    break;
                case wkeCursorInfo.Cross:
                    Cursor = Cursors.Cross;
                    break;
                case wkeCursorInfo.Hand:
                    Cursor = Cursors.Hand;
                    break;
                case wkeCursorInfo.IBeam:
                    Cursor = Cursors.IBeam;
                    break;
                case wkeCursorInfo.Wait:
                    Cursor = Cursors.Wait;
                    break;
                case wkeCursorInfo.Help:
                    Cursor = Cursors.Help;
                    break;
                case wkeCursorInfo.EastResize:
                    Cursor = Cursors.SizeWE;
                    break;
                case wkeCursorInfo.NorthResize:
                    Cursor = Cursors.SizeNS;
                    break;
                case wkeCursorInfo.NorthEastResize:
                    Cursor = Cursors.SizeNESW;
                    break;
                case wkeCursorInfo.NorthWestResize:
                    Cursor = Cursors.SizeNWSE;
                    break;
                case wkeCursorInfo.SouthResize:
                    Cursor = Cursors.SizeWE;
                    break;
                case wkeCursorInfo.SouthEastResize:
                    Cursor = Cursors.SizeNWSE;
                    break;
                case wkeCursorInfo.SouthWestResize:
                    Cursor = Cursors.SizeNESW;
                    break;
                case wkeCursorInfo.WestResize:
                    Cursor = Cursors.SizeWE;
                    break;
                case wkeCursorInfo.NorthSouthResize:
                    Cursor = Cursors.SizeNS;
                    break;
                case wkeCursorInfo.EastWestResize:
                    Cursor = Cursors.SizeWE;
                    break;
                case wkeCursorInfo.NorthEastSouthWestResize:
                    Cursor = Cursors.SizeAll;
                    break;
                case wkeCursorInfo.NorthWestSouthEastResize:
                    Cursor = Cursors.SizeAll;
                    break;
                case wkeCursorInfo.ColumnResize:
                    Cursor = null;
                    break;
                case wkeCursorInfo.RowResize:
                    Cursor = null;
                    break;
                default:
                    Cursor = null;
                    break;
            }
        }

        #endregion

        #region methods

        #endregion

        #region overloaded

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            var key = e.SystemKey == Key.None ? e.Key : e.SystemKey;
            int code = KeyInterop.VirtualKeyFromKey(key);
            var flags = (uint) wkeKeyFlags.WKE_REPEAT;

            if (IsExtendedKey(key))
            {
                flags |= (uint) wkeKeyFlags.WKE_EXTENDED;
            }

            if (Api.FireKeyDownEvent(Handle, code, 0, false))
            {
                e.Handled = false;
            }

            base.OnPreviewKeyDown(e);
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            var key = e.SystemKey == Key.None ? e.Key : e.SystemKey;
            int code = KeyInterop.VirtualKeyFromKey(key);
            var flags = (uint) wkeKeyFlags.WKE_REPEAT;

            if (IsExtendedKey(key))
            {
                flags |= (uint) wkeKeyFlags.WKE_EXTENDED;
            }

            if (Api.FireKeyUpEvent(Handle, code, 0, false))
            {
                e.Handled = false;
            }

            base.OnPreviewKeyUp(e);
        }

        /// <summary>
        /// 大小被改变时同步变化数据
        /// </summary>
        /// <param name="sizeInfo"></param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (Handle != IntPtr.Zero)
            {
                var width = (int) ActualWidth;
                var height = (int) ActualHeight;
                Api.Resize(Handle, width, height);
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (Handle != IntPtr.Zero)
            {
                Api.SetFocus(Handle);
            }
        }

        protected override void OnLostFocus(System.Windows.RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            if (Handle != IntPtr.Zero)
            {
                Api.KillFocus(Handle);
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            var point = e.GetPosition(this);
            if (Handle != IntPtr.Zero)
            {
                uint flags = GetMouseFlags(e);
                Api.FireMouseWheelEvent(Handle, (int) point.X, (int) point.Y, e.Delta, flags);
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (Handle != IntPtr.Zero)
            {
                Focus();
                uint msg = 0;
                if (e.ChangedButton == MouseButton.Left)
                {
                    msg = (uint) WinConst.WM_LBUTTONDOWN;
                }
                else if (e.ChangedButton == MouseButton.Middle)
                {
                    msg = (uint) WinConst.WM_MBUTTONDOWN;
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    msg = (uint) WinConst.WM_RBUTTONDOWN;
                }

                var point = e.GetPosition(this);
                uint flags = GetMouseFlags(e);
                Api.FireMouseEvent(Handle, msg, (int) point.X, (int) point.Y, flags);
            }
        }

        /// <summary>
        /// 同步发送鼠标被按下事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (Handle != IntPtr.Zero)
            {
                uint msg = 0;
                if (e.ChangedButton == MouseButton.Left)
                {
                    msg = (uint) WinConst.WM_LBUTTONUP;
                }
                else if (e.ChangedButton == MouseButton.Middle)
                {
                    msg = (uint) WinConst.WM_MBUTTONUP;
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    msg = (uint) WinConst.WM_RBUTTONUP;
                }

                var point = e.GetPosition(this);

                uint flags = GetMouseFlags(e);
                Api.FireMouseEvent(Handle, msg, (int) point.X, (int) point.Y, flags);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.Handle != IntPtr.Zero)
            {
                uint flags = GetMouseFlags(e);
                var point = e.GetPosition(this);
                Api.FireMouseEvent(this.Handle, 0x200, (int) point.X, (int) point.Y, flags);
            }
        }

        /// <summary>
        /// 文本输入
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);
            if (Handle != IntPtr.Zero)
            {
                for (int i = 0; i < e.Text.Length; i++)
                {
                    Api.FireKeyPressEvent(Handle, e.Text[i], 0, false);
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
                    if (this.Handle != IntPtr.Zero)
                    {
                        Api.DestroyWebView(this.Handle);
                    }
                }

                this.Handle = IntPtr.Zero;

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