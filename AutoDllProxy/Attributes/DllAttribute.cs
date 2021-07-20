using System;

namespace AutoDllProxy.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class DllAttribute
    {
        /// <summary>
        /// DLL文件名
        /// </summary>
        public string Name { get; }

        public DllAttribute(string name)
        {
            this.Name = name;
        }
    }
}