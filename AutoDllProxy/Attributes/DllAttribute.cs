using System;

namespace AutoDllProxy.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class DllAttribute:Attribute
    {
        /// <summary>
        /// DLL文件名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 64位文件名
        /// </summary>
        public string NameX64 { get; set; }

        public DllAttribute(string name,string nameX64=null)
        {
            this.Name = name;
            this.NameX64 = nameX64;
        }
    }
}