using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using AutoDllProxy.Attributes;

namespace AutoDllProxy
{
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

        protected MethodInfo[] ApiMethods { get; private set; }

        private static bool Is64()
        {
            return IntPtr.Size == 8;
        }

        public static DllModuleBuilder Create()
        {
            return new DllModuleBuilder();
        }

        public TModule Build<TModule>()
        {
            //动态生成接口
            //声明一个程序集名称
            var interfaceType = typeof(TModule);
            InitDllName(interfaceType);
            this.ApiMethods = DllProxy.FindApiMethods(interfaceType);
            var moduleName = interfaceType.Module.Name;
            var assemblyName = new AssemblyName(Guid.NewGuid().ToString());
            //创建程序集构造器
            var module = AssemblyBuilder
                .DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run)
                .DefineDynamicModule(moduleName);
            var typeName = interfaceType.FullName ?? Guid.NewGuid().ToString();
            //定义类型
            var builder = module.DefineType(typeName, TypeAttributes.Class);
            //获取方法与描述信息
            //实现接口
            builder.AddInterfaceImplementation(interfaceType);
            BuildMethods(builder);

            var proxyType = builder.CreateType();
            return (TModule) Activator.CreateInstance(proxyType);
        }

        private void BuildMethods(TypeBuilder builder)
        {
            var actionMethods = this.ApiMethods;
            for (int i = 0; i < actionMethods.Length; i++)
            {
                var actionMethod = actionMethods[i];
                var importAttribute = actionMethod.GetCustomAttribute<ImportAttribute>();
                var actionParameters = actionMethod.GetParameters();
                var parameterTypes = actionParameters.Select(p => p.ParameterType).ToArray();
                var dll = Is64() ? this.DllX64Name : DllName;
                var name = importAttribute.ExactSpelling ? actionMethod.Name : importAttribute.EntryPoint;
                builder.DefinePInvokeMethod(name, dll, MethodAttributes.Public, CallingConventions.Standard,
                    actionMethod.ReturnType, parameterTypes, importAttribute.CallingConvention,
                    importAttribute.CharSet);
            }
        }


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