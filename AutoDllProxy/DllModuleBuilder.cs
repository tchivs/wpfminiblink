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
            module.CreateGlobalFunctions();

            var proxyType = builder.CreateType();
            //    var m=  proxyType.GetMethod("wkeInitialize");
            //  var rrr=  m.Invoke(null, null);
            // var succe= proxyType.GetMethod("wkeIsInitialize").Invoke(null,null);
            return (TModule) Activator.CreateInstance(proxyType);
        }

        private void BuildMethods(TypeBuilder builder)
        {
            const MethodAttributes implementAttribute = MethodAttributes.Public | MethodAttributes.Virtual |
                                                        MethodAttributes.Final | MethodAttributes.NewSlot |
                                                        MethodAttributes.HideBySig;
            const MethodAttributes pInvokeAttribute = MethodAttributes.Family | MethodAttributes.FamANDAssem |
                                                      MethodAttributes.Final | MethodAttributes.Public |
                                                      MethodAttributes.PinvokeImpl |
                                                      MethodAttributes.Static |
                                                      MethodAttributes.HideBySig;
            var actionMethods = this.ApiMethods;
            for (int i = 0; i < actionMethods.Length; i++)
            {
                var actionMethod = actionMethods[i];
                var importAttribute = actionMethod.GetCustomAttribute<ImportAttribute>();
                var actionParameters = actionMethod.GetParameters();
                var parameterTypes = actionParameters.Select(p => p.ParameterType).ToArray();
                var dll = Is64() ? this.DllX64Name : DllName;
                var name = importAttribute.ExactSpelling ? actionMethod.Name : importAttribute.EntryPoint;
                var pInvokeMethod = builder.DefinePInvokeMethod(name, dll,
                    pInvokeAttribute,
                    CallingConventions.Standard,
                    actionMethod.ReturnType, parameterTypes, importAttribute.CallingConvention,
                    importAttribute.CharSet);
                //  pInvokeMethod.SetImplementationFlags(MethodImplAttributes.PreserveSig);
                pInvokeMethod.SetImplementationFlags(
                    pInvokeMethod.GetMethodImplementationFlags() | MethodImplAttributes.PreserveSig);

                //创建代理方法
                var proxyMethod = builder.DefineMethod($"{actionMethod.Name}", implementAttribute,
                    CallingConventions.Standard,
                    actionMethod.ReturnType, parameterTypes);
                var iL = proxyMethod.GetILGenerator();
                
                iL.Emit(OpCodes.Call, pInvokeMethod);
                 
                // if (actionMethod.ReturnType != typeof(void))
                // {
                //     //设置返回值
                //     iL.Emit(OpCodes.Castclass, actionMethod.ReturnType);
                // }
                // else
                // {
                //     //   iL.Emit(OpCodes.Pop);
                // }

                iL.Emit(OpCodes.Ret);


                //
                // var arguments = iL.DeclareLocal(typeof(object[]));
                // iL.Emit(OpCodes.Ldc_I4, actionParameters.Length);
                // iL.Emit(OpCodes.Newarr, typeof(object));
                // iL.Emit(OpCodes.Stloc, arguments);
                //
                // for (var j = 0; j < actionParameters.Length; j++)
                // {
                //     iL.Emit(OpCodes.Ldloc, arguments);
                //     iL.Emit(OpCodes.Ldc_I4, j);
                //     iL.Emit(OpCodes.Ldarg, j + 1);
                //
                //     var parameterType = parameterTypes[j];
                //     if (parameterType.IsValueType || parameterType.IsGenericParameter)
                //     {
                //         iL.Emit(OpCodes.Box, parameterType);
                //     }
                //
                //     iL.Emit(OpCodes.Stelem_Ref);
                // }
                //
                // // 加载arguments参数
                // iL.Emit(OpCodes.Ldloc, arguments);
                // iL.Emit(OpCodes.Callvirt, pInvokeMethod);
                //
                // if (actionMethod.ReturnType == typeof(void))
                // {
                //     iL.Emit(OpCodes.Pop);
                // }
                // iL.Emit(OpCodes.Castclass, actionMethod.ReturnType);
                // iL.Emit(OpCodes.Ret);

                builder.DefineMethodOverride(proxyMethod, actionMethod);
                // methodBuilder.SetImplementationFlags(
                //     methodBuilder.GetMethodImplementationFlags() | MethodImplAttributes.PreserveSig);
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