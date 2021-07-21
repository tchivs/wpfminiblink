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
}