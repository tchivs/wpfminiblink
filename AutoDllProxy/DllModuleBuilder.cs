using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using AutoDllProxy.Attributes;

namespace AutoDllProxy
{
    static class Extensions
    {
        public static ParameterInfo[] GetParameters(this MethodInfo method, string[] fields)
        {
            if (fields == null)
            {
                return method.GetParameters();
            }

            return method.GetParameters().Skip(fields.Length - 1).ToArray();
        }
    }

    public class DllModuleBuilder
    {
        /// <summary>
        /// DLL所在路径
        /// </summary>
        public string DllPath { get; set; }

        /// <summary>
        /// dll文件名
        /// </summary>
        public string DllName { get; set; }

        /// <summary>
        /// 64位文件名
        /// </summary>
        public string DllX64Name { get; set; }


        private static bool Is64()
        {
            return IntPtr.Size == 8;
        }

        private string GetDllName => Is64() ? this.DllX64Name : DllName;

        public static DllModuleBuilder Create()
        {
            return new DllModuleBuilder();
        }

        public TModule Build<TModule>()
        {
            //动态生成接口
            //声明一个程序集名称
            var interfaceType = typeof(TModule);
            if (!Types.TryGetValue(interfaceType.FullName, out var type))
            {
                InitDllName(interfaceType);
                var assemblyName = new AssemblyName($"{interfaceType.Namespace}.Impl");
                var moduleName = $"{interfaceType.Namespace}.Impl";

                AssemblyBuilderAccess assemblyBuilderAccess = default(AssemblyBuilderAccess);
#if NET472
                assemblyBuilderAccess = AssemblyBuilderAccess.RunAndSave;
#else
                assemblyBuilderAccess = AssemblyBuilderAccess.Run;
#endif
                var asm = AssemblyBuilder
                    .DefineDynamicAssembly(assemblyName, assemblyBuilderAccess);

                //创建程序集构造器
                ModuleBuilder module;
#if NET472
                var asmFileName = "asm.dll";
                module = asm
                    .DefineDynamicModule(moduleName, asmFileName);
#else
                module = asm
                    .DefineDynamicModule(moduleName);
#endif

                type = new ProxyTypeBuilder(module, interfaceType, GetDllName).Builder();
#if NET472
#if DEBUG

                asm.Save(asmFileName);
#endif
#endif
                Types.Add(interfaceType.FullName, type);
            }

            return (TModule) Activator.CreateInstance(type);
        }

        static readonly Dictionary<string, Type> Types = new Dictionary<string, Type>();

        /// <summary>
        /// get dllname by attribute
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private void InitDllName(Type interfaceType)
        {
            var dllDesc = interfaceType.GetCustomAttribute<DllAttribute>();
            if (dllDesc == null)
            {
                if (Is64())
                {
                    if (string.IsNullOrEmpty(this.DllX64Name))
                    {
                        throw new ArgumentNullException(nameof(DllX64Name));
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(this.DllName))
                    {
                        throw new ArgumentNullException(nameof(DllName));
                    }
                }
            }
            else
            {
                this.DllName = dllDesc.Name;
                this.DllX64Name = dllDesc.NameX64;
            }
        }
    }
}