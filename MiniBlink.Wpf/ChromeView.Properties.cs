using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MiniBlink.Wpf
{
    public sealed partial class ChromeView
    {
        #region properties

        public WriteableBitmap ImageSource
        {
            get => (WriteableBitmap) this.GetValue(ImageSourceProperty);
            private set => this.SetValue(ImageSourcePropertyKey, value);
        }

        // 只读属性的定义与注册
        private static readonly DependencyPropertyKey ImageSourcePropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(ImageSource),
            typeof(WriteableBitmap), typeof(ChromeView), new PropertyMetadata(null));

        public static readonly DependencyProperty ImageSourceProperty = ImageSourcePropertyKey.DependencyProperty;

        public bool WebViewIsInitialize
        {
            get => (bool) this.GetValue(WebViewIsInitializeProperty);
            private set => this.SetValue(WebViewIsInitializePropertyKey, value);
        }

        // 只读属性的定义与注册
        private static readonly DependencyPropertyKey WebViewIsInitializePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(WebViewIsInitialize),
                typeof(bool), typeof(ChromeView), new PropertyMetadata(false));

        public static readonly DependencyProperty WebViewIsInitializeProperty =
            WebViewIsInitializePropertyKey.DependencyProperty;

        public string Title
        {
            get => (string) this.GetValue(TitleProperty);
            private set => this.SetValue(TitlePropertyKey, value);
        }

        // 只读属性的定义与注册
        private static readonly DependencyPropertyKey TitlePropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(Title),
            typeof(string), typeof(ChromeView), new PropertyMetadata(default));

        public static readonly DependencyProperty TitleProperty = TitlePropertyKey.DependencyProperty;

        #endregion

        #region event

        /// <summary>
        /// 标题被改变事件
        /// </summary>
        public static readonly RoutedEvent TitleChangedEvent =
            EventManager.RegisterRoutedEvent("TitleChanged", RoutingStrategy.Bubble,
                typeof(EventHandler<TitleChangedEventArgs>), typeof(ChromeView));

        public event RoutedEventHandler TitleChanged
        {
            add { this.AddHandler(TitleChangedEvent, value); }
            remove { this.RemoveHandler(TitleChangedEvent, value); }
        }

        #endregion
    }

    //用于承载时间消息的事件参数
    public class TitleChangedEventArgs : RoutedEventArgs
    {
        public TitleChangedEventArgs(RoutedEvent routedEvent, object source, string title)
            : base(routedEvent, source)
        {
            this.Title = title;
        }

        public string Title { get; }
    }
}