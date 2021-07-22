using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using AutoDllProxy.Attributes;

namespace AutoDllProxy
{
    public class ProxyTypeBuilder
    {
        private readonly string _dllName;
        public ModuleBuilder ModuleBuilder { get; }
        public Type InterfaceType { get; }
        public string TypeName { get; }
        private string pInvokeName;

        const MethodAttributes IMPLEMENT_ATTRIBUTES = MethodAttributes.Public | MethodAttributes.Virtual |
                                                      MethodAttributes.Final | MethodAttributes.NewSlot |
                                                      MethodAttributes.HideBySig;

        const MethodAttributes P_INVOKE_ATTRIBUTE = MethodAttributes.Family | MethodAttributes.FamANDAssem |
                                                    MethodAttributes.Final | MethodAttributes.Public |
                                                    MethodAttributes.PinvokeImpl |
                                                    MethodAttributes.Static |
                                                    MethodAttributes.HideBySig;

        public ProxyTypeBuilder(ModuleBuilder moduleBuilder, Type interfaceType, string dllName)
        {
            _dllName = dllName;
            ModuleBuilder = moduleBuilder;
            this.InterfaceType = interfaceType;
            TypeName = $"{moduleBuilder.ScopeName}.{interfaceType.Name.Substring(1)}";
            this.TypeBuilder = moduleBuilder.DefineType(TypeName, TypeAttributes.Class);
            TypeBuilder.AddInterfaceImplementation(InterfaceType);
            pInvokeName = $"{TypeName}PInvoke";
            this.PInvokeBuilder = moduleBuilder.DefineType(pInvokeName, TypeAttributes.Class | TypeAttributes.Public);
        }

        public Dictionary<string, FieldInfo> FieldInfos { get; } = new Dictionary<string, FieldInfo>();
        public Dictionary<string, MethodBuilder> PInvokeMethods { get; } = new Dictionary<string, MethodBuilder>();
        public TypeBuilder TypeBuilder { get; }
        public TypeBuilder PInvokeBuilder { get; }
        private ILGenerator publicCtorIl;

        IEnumerable<KeyValuePair<ImportAttribute, MethodInfo>> GetMethods()
        {
            var methods = DllProxy.FindApiMethods(this.InterfaceType);
            var dictionary = new Dictionary<ImportAttribute, MethodInfo>();
            foreach (var method in methods)
            {
                var import = method.GetCustomAttribute<ImportAttribute>();
                if (import == null)
                {
                    continue;
                }

                dictionary.Add(import, method);
            }

            var d = dictionary.OrderByDescending(pair => pair.Key.ReturnValue);
            foreach (var pair in d)
            {
                yield return pair;
            }
        }

        public Type Builder()
        {
            //定义一个空的构造函数
            ConstructorBuilder publicCtor = TypeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                Type.EmptyTypes);
            this.publicCtorIl = publicCtor.GetILGenerator();
            publicCtorIl.Emit(OpCodes.Ldarg_0);
            publicCtorIl.Emit(OpCodes.Call,
                typeof(object).GetConstructor(Type.EmptyTypes));
            foreach (var pair in GetMethods())
            {
                var import = pair.Key;
                var methodInfo = pair.Value;

                var actionParameters = methodInfo.GetParameters();
                var parameterTypes = actionParameters.Select(p => p.ParameterType).ToArray();
                var pInvoke = CreatePInvokeMethod(
                    import,
                    methodInfo.ReturnType,
                    parameterTypes, methodInfo.Name);

                if (import.ReturnValue == null)
                {
                    if (import.Parameter != null)
                    {
                        //把字段注入到代理类中执行
                        CreateMethod(import, parameterTypes, methodInfo, pInvoke);
                    }
                    else
                    {
                        //默认实现
                        CreateMethod(import, parameterTypes, methodInfo, pInvoke);
                    }
                }
                else
                {
                    CreateMethod(import, parameterTypes, methodInfo, pInvoke);
                }
            }

            publicCtorIl.Emit(OpCodes.Ret);
            ModuleBuilder.CreateGlobalFunctions();
            PInvokeBuilder.CreateType();
            return this.TypeBuilder.CreateType();
        }

        /// <summary>
        /// 创建方法同时将返回值保存为内部字段
        /// </summary>
        /// <param name="import"></param>
        /// <param name="parameterTypes"></param>
        /// <param name="methodInfo"></param>
        /// <param name="pInvoke"></param>
        /// <exception cref="NotImplementedException"></exception>
        private MethodInfo CreateMethod(ImportAttribute import, Type[] parameterTypes, MethodInfo methodInfo,
            MethodInfo pInvoke)
        {
            var proxyMethod = this.TypeBuilder.DefineMethod($"{methodInfo.Name}", IMPLEMENT_ATTRIBUTES,
                CallingConventions.Standard,
                methodInfo.ReturnType, parameterTypes);
            var iL = proxyMethod.GetILGenerator();
            if (import.Parameter == null)
            {
                if (import.ReturnValue != null)
                {
                    if (methodInfo.ReturnType == typeof(void))
                    {
                        throw new NullReferenceException("方法返回值为空，不能设置为ReturnValue!");
                    }

                    //创建方法同时将返回值保存为内部字段
                    var field = this.CreateField(import.ReturnValue, methodInfo.ReturnType, out var _);
                    //this.xxx=pinvoke();
                    //return this.xxx;
                    iL.Emit(OpCodes.Ldarg_0);
                    // InjectParameter(iL, 1, parameterTypes.Length);
                    CallInvoke(import, iL, pInvoke);
                    iL.Emit(OpCodes.Stfld, field);
                    iL.Emit(OpCodes.Ldarg_0);
                    iL.Emit(OpCodes.Ldfld, field);
                }
                else
                {
                    //默认注入方法
                    CallInvoke(import, iL, pInvoke,parameterTypes.Length);
                    
                }
            }
            else
            {
                //this.xxx=pinvoke(this.a,this.b);
                //return this.xxx;
                var fieldNames = import.Parameter.Split(',');
                for (int i = 0; i < fieldNames.Length; i++)
                {
                    var fieldName = fieldNames[i];
                    if (this.FieldInfos.TryGetValue(fieldName, out var fieldInfo))
                    {
                        iL.Emit(OpCodes.Ldarg, i);
                        iL.Emit(OpCodes.Ldfld, fieldInfo);
                    }
                }
            
                CallInvoke(import, iL, pInvoke,parameterTypes.Length);
            }


            iL.Emit(OpCodes.Ret);
            TypeBuilder.DefineMethodOverride(proxyMethod, methodInfo);
            return proxyMethod;
        }

        /// <summary>
        /// 注入参数
        /// </summary>
        /// <param name="iL"></param>
        /// <param name="offSet">如果起始位为0则第一个参数传this，所以一般是以1为起始</param>
        /// <param name="count"></param>
        void InjectParameter(ILGenerator iL, int offSet, int count)
        {
            for (int i = 0; i < count; i++)
            {
                //注入参数
                iL.Emit(OpCodes.Ldarg, i + offSet);
            }
        }

        void CallInvoke(ImportAttribute import, ILGenerator iL, MethodInfo pInvoke,int parameterCount=0)
        {
            if (import.MarshalTypeRef != null)
            {
                //在构造函数中把转换器赋值
                var marshalType = import.MarshalTypeRef;
                var field = CreateField(marshalType.Name, marshalType, out var isExists);
                if (!isExists)
                {
                     this.publicCtorIl.Emit(OpCodes.Ldarg_0);
                    var instance = marshalType.GetField("Instance");
                    this.publicCtorIl.Emit(OpCodes.Ldsfld, instance);
                    this.publicCtorIl.Emit(OpCodes.Stfld, field);
                }

                if (field == null)
                {
                    throw new NullReferenceException($"字段{marshalType.Name}不能为空");
                }

                var method = field.FieldType.GetMethod(nameof(ICustomMarshaler.MarshalNativeToManaged));
                if (method == null)
                {
                    throw new NullReferenceException($"方法{nameof(ICustomMarshaler.MarshalNativeToManaged)}不能为空");
                }

                iL.Emit(OpCodes.Ldarg_0);
                iL.Emit(OpCodes.Ldfld, field);
                this.InjectParameter(iL,1,parameterCount);
                iL.Emit(OpCodes.Call, pInvoke);
                iL.Emit(OpCodes.Callvirt, method);
                iL.Emit(OpCodes.Castclass, typeof(string));
            }
            else
            {
                this.InjectParameter(iL,1,parameterCount);
                iL.Emit(OpCodes.Call, pInvoke);
            }
        }

        /// <summary>
        /// 在代理类中创建字段
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private FieldInfo CreateField(string name, Type type, out bool isExists)
        {
            isExists = FieldInfos.TryGetValue(name, out var field);
            if (!isExists)
            {
                field = this.TypeBuilder.DefineField(name, type, FieldAttributes.Private);
                FieldInfos.Add(name, field);
            }

            return field;
        }
        //
        // private void BuildMethods(TypeBuilder dllBuilder, TypeBuilder proxyBuilder)
        // {
        //     var actionMethods = new List<MethodInfo>();
        //     Dictionary<string, FieldBuilder> proxyFields = new Dictionary<string, FieldBuilder>();
        //     Dictionary<string, PropertyBuilder> proxyProperties = new Dictionary<string, PropertyBuilder>();
        //     for (int i = 0; i < actionMethods.Count; i++)
        //     {
        //         var actionMethod = actionMethods[i];
        //         var importAttribute = actionMethod.GetCustomAttribute<ImportAttribute>();
        //         var fieldNames = importAttribute.Parameter?.Split(',');
        //         var actionParameters = actionMethod.GetParameters();
        //         var parameterTypes = actionParameters.Select(p => p.ParameterType).ToArray();
        //
        //         var pInvokeReturnValue = actionMethod.ReturnType;
        //         var invokeParameterTypes = new List<Type>();
        //         if (fieldNames != null)
        //         {
        //             foreach (var fieldName in fieldNames)
        //             {
        //                 invokeParameterTypes.Add(proxyFields[fieldName].FieldType);
        //             }
        //         }
        //
        //         if (importAttribute.MarshalTypeRef != null)
        //         {
        //             var getSetAttr =
        //                 MethodAttributes.Public | MethodAttributes.SpecialName |
        //                 MethodAttributes.HideBySig;
        //             pInvokeReturnValue = typeof(IntPtr);
        //             //this.xxxMarshaler = CustonMarshalType.Instance
        //             var property = proxyBuilder.DefineProperty(importAttribute.MarshalTypeRef.Name,
        //                 PropertyAttributes.HasDefault,
        //                 importAttribute.MarshalTypeRef, null);
        //             var field = proxyBuilder.DefineField(importAttribute.MarshalTypeRef.Name.ToLower(),
        //                 importAttribute.MarshalTypeRef, FieldAttributes.Private);
        //             var custNameGetPropMthdBldr =
        //                 dllBuilder.DefineMethod($"get_{property.Name}",
        //                     getSetAttr,
        //                     property.PropertyType,
        //                     Type.EmptyTypes);
        //             var custNameGetIL = custNameGetPropMthdBldr.GetILGenerator();
        //             custNameGetIL.Emit(OpCodes.Ldsfld, field);
        //             custNameGetIL.Emit(OpCodes.Ret);
        //             property.SetGetMethod(custNameGetPropMthdBldr);
        //             var marsh = importAttribute.MarshalTypeRef.GetField("Instance").GetValue(null);
        //         }
        //
        //         invokeParameterTypes.AddRange(parameterTypes);
        //         //生成DLL的静态导出类
        //         var pInvokeMethod = this.CreatePInvokeMethod(
        //             importAttribute,
        //             pInvokeReturnValue,
        //             invokeParameterTypes.ToArray());
        //         //创建代理方法
        //         var proxyMethod = proxyBuilder.DefineMethod($"{actionMethod.Name}", IMPLEMENT_ATTRIBUTES,
        //             CallingConventions.Standard,
        //             actionMethod.ReturnType, parameterTypes);
        //         var iL = proxyMethod.GetILGenerator();
        //
        //         if (importAttribute.ReturnValue != null)
        //         {
        //             if (actionMethod.ReturnType == typeof(void))
        //             {
        //                 throw new NullReferenceException("方法返回值为空，不能设置为ReturnValue!");
        //             }
        //
        //             //在类中创建一个字段
        //             /*
        //              * this.field = PInvoke.wkeCreateWebView();
        //        return this.field;
        //              */
        //             var field = proxyBuilder.DefineField(importAttribute.ReturnValue, pInvokeMethod.ReturnType,
        //                 FieldAttributes.Private);
        //             proxyFields.Add(field.Name, field);
        //             iL.Emit(OpCodes.Ldarg_0);
        //
        //             iL.Emit(OpCodes.Call, pInvokeMethod);
        //             iL.Emit(OpCodes.Stfld, field);
        //             iL.Emit(OpCodes.Ldarg_0);
        //             iL.Emit(OpCodes.Ldfld, field);
        //         }
        //         else
        //         {
        //             if (importAttribute.Parameter != null)
        //             {
        //                 InjectFieldToParameter(iL, fieldNames, proxyFields, actionParameters);
        //             }
        //             else
        //             {
        //                 //注入参数
        //                 InjectParameter(iL, actionParameters);
        //             }
        //
        //          
        //         }
        //         iL.Emit(OpCodes.Call, pInvokeMethod);
        //
        //         iL.Emit(OpCodes.Ret);
        //    
        //     }
        // }


        /// <summary>
        /// 创建静态方法
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="attribute"></param>
        /// <param name="returnValue"></param>
        /// <param name="parameterTypes"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        private MethodInfo CreatePInvokeMethod(ImportAttribute attribute, Type returnValue,
            Type[] parameterTypes, string name = null)
        {
            if (attribute.ExactSpelling)
            {
                if (name == null)
                {
                    throw new NullReferenceException($"ExactSpelling为True时必须使用方法名称！");
                }
            }

            name = name == null ? Guid.NewGuid().ToString("N") : attribute.EntryPoint;
            if (name == null)
            {
                throw new NullReferenceException($"{nameof(attribute.EntryPoint)}为空！");
            }

            var sign = $"{name}";
            if (!PInvokeMethods.TryGetValue(sign, out var pInvokeMethod))
            {
                if (attribute.Parameter != null)
                {
                    //把注解中的参数加入到静态类中
                    var ps = attribute.Parameter.Split(',');
                    List<Type> newTypes = new List<Type>(parameterTypes);
                    for (int i = 0; i < ps.Length; i++)
                    {
                        var pName = ps[i];
                        if (this.FieldInfos.TryGetValue(pName, out var field))
                        {
                            newTypes.Insert(i, field.FieldType);
                        }
                        else
                        {
                            throw new NullReferenceException($"字段{pName}未找到！");
                        }
                    }

                    parameterTypes = newTypes.ToArray();
                }

                if (attribute.MarshalTypeRef != null)
                {
                    returnValue = typeof(IntPtr);
                }

                var dll = _dllName;
                pInvokeMethod = this.PInvokeBuilder.DefinePInvokeMethod(name, dll,
                    P_INVOKE_ATTRIBUTE,
                    CallingConventions.Standard,
                    returnValue, parameterTypes, attribute.CallingConvention,
                    attribute.CharSet);
                pInvokeMethod.SetImplementationFlags(
                    pInvokeMethod.GetMethodImplementationFlags() | MethodImplAttributes.PreserveSig);
                PInvokeMethods.Add(sign, pInvokeMethod);
            }

            return pInvokeMethod;
        }
    }
}