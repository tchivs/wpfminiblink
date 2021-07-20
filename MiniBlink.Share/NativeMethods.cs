using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Reflection.Emit;

namespace MiniBlink
{
    internal class PInvokeDescription
    {
        public DllImportAttribute Import { get; }
        public Type ReturnType { get; }
        public Type[] ParamTypes { get; }
        public Type Type { get; set; }
        public PInvokeDescription(DllImportAttribute import, Type returnType, params Type[] paramTypes)
        {
            this.Import = import;
            ReturnType = returnType;
            ParamTypes = paramTypes;
        }

    }
    /// <summary>
    /// 封装API
    /// </summary>
    internal class NativeMethods
    {
        public const string Dll = "node.dll";
        public const string Dll64 = "node_x64.dll";
        public const string Dll86 = "node_x86.dll";

        public static readonly List<PInvokeDescription> DllImportList = new List<PInvokeDescription>(new[]
        {
            Create("wkeInitialize",null,null),
        });
        static PInvokeDescription Create(string entryPoint, Type returnType, params Type[] paramTypes)
        {
            return new PInvokeDescription(new DllImportAttribute(Dll)
            {
                CallingConvention = CallingConvention.Cdecl,
                EntryPoint = entryPoint
            }, returnType, paramTypes);
        }

    }

    public interface IMiniBlink
    {
        /// <summary>
        /// 初始化
        /// </summary>
        bool Initialize();
    }

    /// <summary>
    /// 标准版本接口
    /// </summary>
    public static class MiniBlinkBuilder
    {
        private static bool Is64()
        {
            return IntPtr.Size == 8;
        }

        static TypeBuilder DefineMiniBlink(this TypeBuilder builder,string method, string dll, Type returnType, Type[] paramTypes,
            CharSet charSet = CharSet.None)
        {
            builder.DefinePInvokeMethod("", dll,
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.PinvokeImpl,
                CallingConventions.Standard, returnType, paramTypes, CallingConvention.Cdecl, charSet);
            return builder;
        }

        /// <summary>
        /// 绑定64位程序集
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        static TypeBuilder Define64(this TypeBuilder builder)
        {
            return builder;
        }

        /// <summary>
        /// 绑定32位程序集
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        static TypeBuilder Define32(this TypeBuilder builder)
        {
            return builder;
        }

        public static IMiniBlink Create()
        {
            //动态生成接口
            //声明一个程序集名称
            var interfaceType = typeof(IMiniBlink);
            var moduleName = interfaceType.Module.Name;
            var assemblyName = new AssemblyName(Guid.NewGuid().ToString());
            //创建程序集构造器
            var module = AssemblyBuilder
                .DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run)
                .DefineDynamicModule(moduleName);
            var typeName = interfaceType.FullName ?? Guid.NewGuid().ToString();
            //定义类型
            var builder = module.DefineType(typeName, TypeAttributes.Class);
            //实现接口
            builder.AddInterfaceImplementation(interfaceType);

            if (Is64())
            {
                builder.Define64();
            }
            else
            {
                builder.Define32();
            }

            var proxyType = builder.CreateType();
            return (IMiniBlink)Activator.CreateInstance(proxyType);
        }
    }
}