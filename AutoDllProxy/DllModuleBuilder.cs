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
            var typeName = $"{interfaceType.Namespace}.{interfaceType.Name.Substring(1)}";
            var apiDllBuilder = module.DefineType($"{interfaceType.Namespace}.PInvoke", TypeAttributes.Class);
            // apiDllBuilder.DefineConstructor(MethodAttributes.Static, CallingConventions.Standard, null);
            //定义类型
            var implBuilder = module.DefineType(typeName, TypeAttributes.Class);
            //获取方法与描述信息
            //实现接口
            implBuilder.AddInterfaceImplementation(interfaceType);
            BuildMethods(apiDllBuilder, implBuilder);
            module.CreateGlobalFunctions();
            apiDllBuilder.CreateType();
            var proxyType = implBuilder.CreateType();
#if NET472
            asm.Save(asmFileName);
#endif

            //    var m=  proxyType.GetMethod("wkeInitialize");
            //  var rrr=  m.Invoke(null, null);
            // var succe= proxyType.GetMethod("wkeIsInitialize").Invoke(null,null);
            return (TModule) Activator.CreateInstance(proxyType);
        }

        private void BuildMethods(TypeBuilder dllBuilder, TypeBuilder proxyBuilder)
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
            Dictionary<string, FieldBuilder> proxyFields = new Dictionary<string, FieldBuilder>();
            Dictionary<string, PropertyBuilder> proxyProperties = new Dictionary<string, PropertyBuilder>();
            for (int i = 0; i < actionMethods.Length; i++)
            {
                var actionMethod = actionMethods[i];

                var importAttribute = actionMethod.GetCustomAttribute<ImportAttribute>();
                var fieldNames = importAttribute.Parameter?.Split(',');
                var actionParameters = actionMethod.GetParameters();
                var parameterTypes = actionParameters.Select(p => p.ParameterType).ToArray();
                var dll = Is64() ? this.DllX64Name : DllName;
                var name = importAttribute.ExactSpelling ? actionMethod.Name : importAttribute.EntryPoint;
                var pInvokeReturnValue = actionMethod.ReturnType;
                var invokeparameterTypes = new List<Type>();
                if (fieldNames != null)
                {
                    foreach (var fieldName in fieldNames)
                    {
                        invokeparameterTypes.Add(proxyFields[fieldName].FieldType);
                    }
                }

                if (importAttribute.MarshalTypeRef != null)
                {
                    var getSetAttr =
                        MethodAttributes.Public | MethodAttributes.SpecialName |
                        MethodAttributes.HideBySig;
                    pInvokeReturnValue = typeof(IntPtr);
                    //this.xxxMarshaler = CustonMarshalType.Instance
                    var property = proxyBuilder.DefineProperty(importAttribute.MarshalTypeRef.Name,
                        PropertyAttributes.HasDefault,
                        importAttribute.MarshalTypeRef, null);
                    var field = proxyBuilder.DefineField(importAttribute.MarshalTypeRef.Name.ToLower(),
                        importAttribute.MarshalTypeRef, FieldAttributes.Private);
                    var custNameGetPropMthdBldr =
                        dllBuilder.DefineMethod($"get_{property.Name}",
                            getSetAttr,
                            property.PropertyType,
                            Type.EmptyTypes);

                    var custNameGetIL = custNameGetPropMthdBldr.GetILGenerator();
                    custNameGetIL.Emit(OpCodes.Ldsfld, field);
                    custNameGetIL.Emit(OpCodes.Ret);
                    property.SetGetMethod(custNameGetPropMthdBldr);
                    var marsh = importAttribute.MarshalTypeRef.GetField("Instance").GetValue(null);
                     
                }

                invokeparameterTypes.AddRange(parameterTypes);
                //生成DLL的静态导出类
                var pInvokeMethod = dllBuilder.DefinePInvokeMethod(name, dll,
                    pInvokeAttribute,
                    CallingConventions.Standard,
                    pInvokeReturnValue, invokeparameterTypes.ToArray(), importAttribute.CallingConvention,
                    importAttribute.CharSet);


                pInvokeMethod.SetImplementationFlags(
                    pInvokeMethod.GetMethodImplementationFlags() | MethodImplAttributes.PreserveSig);


                //创建代理方法
                var proxyMethod = proxyBuilder.DefineMethod($"{actionMethod.Name}", implementAttribute,
                    CallingConventions.Standard,
                    actionMethod.ReturnType, parameterTypes);
                var iL = proxyMethod.GetILGenerator();

                if (importAttribute.ReturnValue != null)
                {
                    if (actionMethod.ReturnType == typeof(void))
                    {
                        throw new NullReferenceException("方法返回值为空，不能设置为ReturnValue!");
                    }

                    //在类中创建一个字段
                    /*
                     * this.field = PInvoke.wkeCreateWebView();
			            return this.field;
                     */
                    var field = proxyBuilder.DefineField(importAttribute.ReturnValue, pInvokeMethod.ReturnType,
                        FieldAttributes.Private);
                    proxyFields.Add(field.Name, field);
                    iL.Emit(OpCodes.Ldarg_0);
                    iL.Emit(OpCodes.Call, pInvokeMethod);
                    iL.Emit(OpCodes.Stfld, field);
                    iL.Emit(OpCodes.Ldarg_0);
                    iL.Emit(OpCodes.Ldfld, field);
                }
                else
                {
                    if (importAttribute.Parameter != null)
                    {
                        InjectFieldToParameter(iL, fieldNames, proxyFields, actionParameters);
                    }
                    else
                    {
                        //注入参数
                        InjectParameter(iL, actionParameters);
                    }

                    iL.Emit(OpCodes.Call, pInvokeMethod);
                }


                iL.Emit(OpCodes.Ret);
                proxyBuilder.DefineMethodOverride(proxyMethod, actionMethod);
            }
        }

        private void InjectParameter(ILGenerator iL, ParameterInfo[] actionParameters)
        {
            for (var j = 0; j < actionParameters.Length; j++)
            {
                // iL.Emit(OpCodes.Ldloc, arguments);
                // iL.Emit(OpCodes.Ldc_I4, j);
                iL.Emit(OpCodes.Ldarg, j + 1);

                // var parameterType = actionParameters[j];
                // if (parameterType.IsValueType || parameterType.IsGenericParameter)
                // {
                //     iL.Emit(OpCodes.Box, parameterType);
                // }
                // iL.Emit(OpCodes.Stelem_Ref);
            }
        }

        /// <summary>
        /// 把字段注入方法参数中
        /// </summary>
        /// <param name="iL"></param>
        /// <param name="fieldNames"></param>
        /// <param name="proxyFields"></param>
        /// <param name="actionParameters"></param>
        private void InjectFieldToParameter(ILGenerator iL, string[] fieldNames,
            Dictionary<string, FieldBuilder> proxyFields, ParameterInfo[] actionParameters)
        {
            for (int i = 0; i < fieldNames.Length; i++)
            {
                var fieldName = fieldNames[i];
                var field = proxyFields[fieldName];
                iL.Emit(OpCodes.Ldarg, i);
                iL.Emit(OpCodes.Ldfld, field);
            }

            for (int i = fieldNames.Length; i < actionParameters.Length + fieldNames.Length; i++)
            {
                iL.Emit(OpCodes.Ldarg, i);
            }

            iL.Emit(OpCodes.Nop);
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