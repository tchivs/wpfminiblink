using System;
using AutoDllProxy.Attributes;

namespace MiniBlink.Share.Events
{
    public class TitleChangeEventArgs : EventArgs
    {
        public TitleChangeEventArgs()
        {
        }
        [Map(Name = "title")]
        public string Title { get; set; }
    }
    public class MiniblinkEventArgs : EventArgs
    {
        internal MiniblinkEventArgs()
        {
        }
    }
    public enum NavigateType
    {
        /// <summary>
        /// 链接
        /// </summary>
        LinkClick,
        /// <summary>
        /// 表单提交submit
        /// </summary>
        Submit,
        /// <summary>
        /// 前进或后退
        /// </summary>
        BackForward,
        /// <summary>
        /// 重新载入
        /// </summary>
        ReLoad,
        /// <summary>
        /// 表单重新提交resubmit
        /// </summary>
        ReSubmit,
        /// <summary>
        /// 其他
        /// </summary>
        Other,
        /// <summary>
        /// window.open引发
        /// </summary>
        WindowOpen,
        /// <summary>
        /// 拥有target=blank属性的a链接引发
        /// </summary>
        BlankLink
    }

    public class NavigateEventArgs : MiniblinkEventArgs
    {
        public string Url { get; }
        public NavigateType Type { get; set; }
        public bool Cancel { get; set; }
        public NavigateEventArgs(string url)
        {
            this.Url = url;
           
        }
        public NavigateEventArgs(string url, NavigateType type)
        {
            this.Url = url;
            this.Type = type;
        }     public NavigateEventArgs(string url, wkeNavigationType type)
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