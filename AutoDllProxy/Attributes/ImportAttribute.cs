using System;
using System.Runtime.InteropServices;

namespace AutoDllProxy.Attributes
{
    /// <summary>
    /// 导入的方法描述 类似DllImport
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class ImportAttribute : Attribute
    {
        /// <summary>
        /// 给出 dll 中入口点的名称。如果未指定 EntryPoint，则使用方法本身的名称
        /// </summary>
        public string EntryPoint;

        /// <summary>
        /// 指示入口点的调用约定。如果未指定 CallingConvention，则使用默认值 CallingConvention.Winapi。
        /// </summary>
        public CallingConvention CallingConvention;

        /// <summary>
        /// 指示 EntryPoint 是否必须与指示的入口点的拼写完全匹配。如果未指定 ExactSpelling，则使用默认值 false。    
        /// </summary>
        public bool ExactSpelling;

        /// <summary>
        ///  指示用在入口点中的字符集。如果未指定 CharSet，则使用默认值 CharSet.Auto。   
        /// </summary>
        public CharSet CharSet;

        /// <summary>
        /// 参数引用指定保存的字段，按顺序使用，多个参数使用,分割 如name，则在内部将name参数传递到该方法中
        /// </summary>
        public string Parameter { get; set; }

        /// <summary>
        /// 把返回的数据保存成一个字段 如：name
        /// </summary>
        public string ReturnValue { get; set; }

        public Type MarshalTypeRef
        {
            get;
            set;
        }
    }
}