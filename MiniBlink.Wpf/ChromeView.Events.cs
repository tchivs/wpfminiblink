using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MiniBlink.Wpf
{
    public partial class ChromeView
    {
        #region DefineEvents

        /// <summary>
        /// 标题被改变事件
        /// </summary>
        public static readonly RoutedEvent TitleChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(TitleChanged), RoutingStrategy.Bubble,
                typeof(EventHandler<TitleChangedEventArgs>), typeof(ChromeView));

        /// <summary>
        /// 页面导航时触发此回调
        /// </summary>
        public static readonly RoutedEvent OnNavigationEvent =
            EventManager.RegisterRoutedEvent(nameof(OnNavigation), RoutingStrategy.Bubble,
                typeof(EventHandler<OnNavigateEventArgs>), typeof(ChromeView));

        /// <summary>
        /// URL被改变
        /// </summary>
        public static readonly RoutedEvent OnUrlChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(OnUrlChanged), RoutingStrategy.Bubble,
                typeof(EventHandler<OnUrlChangedEventArgs>), typeof(ChromeView));

        #endregion

        #region AddEventHandler

        public event EventHandler<TitleChangedEventArgs> TitleChanged
        {
            add => AddHandler(TitleChangedEvent, value);
            remove => RemoveHandler(TitleChangedEvent, value);
        }

        public event EventHandler<OnNavigateEventArgs> OnNavigation
        {
            add => AddHandler(OnNavigationEvent, value);
            remove => RemoveHandler(OnNavigationEvent, value);
        }

        public event EventHandler<OnUrlChangedEventArgs> OnUrlChanged
        {
            add => AddHandler(OnUrlChangedEvent, value);
            remove => RemoveHandler(OnUrlChangedEvent, value);
        }

        #endregion

        #region RaiseEvents

        #endregion
    }
}