using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using MiniBlink.Share;
using MiniBlink.Share.Events;
using MiniBlink.Wpf.Ime;

namespace MiniBlink.Wpf
{
    public sealed partial class ChromeView
    {
        #region properties

        // public string Url
        // {
        //     get => (string) this.GetValue(UrlProperty);
        //       set => this.SetValue(UrlProperty, value);
        // }
        //
        // private static readonly DependencyProperty UrlProperty = DependencyProperty.Register(
        //     nameof(Url),
        //     typeof(string), typeof(ChromeView), new PropertyMetadata(null, OnUrlPropertyChanged));


        /// <summary>
        /// 获取或设置Url的值
        /// </summary>
        public string Url
        {
            get => (string) GetValue(UrlProperty);
            set => SetValue(UrlProperty, value);
        }

        /// <summary>
        /// 标识 Url 依赖属性。
        /// </summary>
        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.Register(nameof(Url), typeof(string), typeof(ChromeView),
                new PropertyMetadata(default(string), OnUrlChangedCallback));

        private static void OnUrlChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var oldValue = (string) args.OldValue;
            var newValue = (string) args.NewValue;
            if (oldValue == newValue)
                return;
            if (obj is ChromeView target)
            {
                var e = new OnUrlChangedEventArgs(OnUrlChangedEvent, target, newValue);
                target.RaiseUrlChangedEvent(e);
            }
        }


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

    public class OnUrlChangedEventArgs : RoutedEventArgs
    {
        public string Url { get; set; }

        public OnUrlChangedEventArgs(RoutedEvent routedEvent, object source, string url) : base(
            routedEvent, source)
        {
            this.Url = url;
        }
    }

    public class OnNavigateEventArgs : RoutedEventArgs
    {
        public string Url { get; }
        public NavigateType Type { get; }
        public bool Cancel { get; set; }

        public OnNavigateEventArgs(RoutedEvent routedEvent, object source, string url, NavigateType type) : base(
            routedEvent, source)
        {
            this.Url = url;
            this.Type = type;
        }

        public OnNavigateEventArgs(RoutedEvent routedEvent, object source, string url, wkeNavigationType type) : base(
            routedEvent, source)
        {
            this.Url = url;
            switch (type)
            {
                case wkeNavigationType.BackForward:
                    Type = NavigateType.BackForward;
                    break;
                case wkeNavigationType.FormReSubmit:
                    Type = NavigateType.ReSubmit;
                    break;
                case wkeNavigationType.FormSubmit:
                    Type = NavigateType.Submit;
                    break;
                case wkeNavigationType.LinkClick:
                    Type = NavigateType.LinkClick;
                    break;
                case wkeNavigationType.ReLoad:
                    Type = NavigateType.ReLoad;
                    break;
                case wkeNavigationType.Other:
                    Type = NavigateType.Other;
                    break;
                default:
                    throw new Exception("未知的重定向类型：" + type);
            }
        }
    }
}