using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DaveTheMonitor.Core.Helpers
{
    /// <summary>
    /// Contains helpers for creating fast method invokers that don't perform any boxing unless required by differences in invoker and method signatures.
    /// </summary>
    public static class MethodHelper
    {
        /// <summary>
        /// Creates an invoker that calls <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <remarks>
        /// <para>Harmony's <see cref="MethodInvoker"/> can create invokers by emitting dynamic methods, but these methods take <see cref="object"/>[] as a parameter, meaning they require an array allocation and boxing for value types.
        /// The invokers emitted by <see cref="CreateInvoker{T}(MethodInfo)"/> match the signature of <typeparamref name="T"/>, so they don't require an array or boxing unless the invoker's signature takes an object where the target method takes a value type or vice versa.</para>
        /// <para>This method does not currently support byref paramaters (ref, in, out)</para>
        /// </remarks>
        public static T CreateInvoker<T>(this MethodInfo method) where T : Delegate
        {
#if DEBUG
            CorePlugin.Log($"Building invoker for {method.Name}");
#endif
            Type del = typeof(T);
            MethodInfo delInvoke = del.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance);
            ParameterInfo[] delParams = delInvoke.GetParameters();
            Type[] delParamTypes = new Type[delParams.Length];
            for (int i = 0; i < delParams.Length; i++)
            {
                delParamTypes[i] = delParams[i].ParameterType;
            }
            Type delReturn = delInvoke.ReturnType;
            Type[] methodParams = GetParamTypes(method);

            VerifyInvoker(delReturn, delParamTypes, method.ReturnType, methodParams);

            DynamicMethod invoker = BuildInvoker(delReturn, delParamTypes, method, methodParams);
#if DEBUG
            CorePlugin.Log($"Built invoker for {method.Name}");
#endif
            return (T)invoker.CreateDelegate(del);
        }

        private static Type[] GetParamTypes(MethodInfo method)
        {
            Type[] targetParams;
            if (method.IsStatic)
            {
                ParameterInfo[] @params = method.GetParameters();
                targetParams = new Type[@params.Length];
                for (int i = 0; i < @params.Length; i++)
                {
                    targetParams[i] = @params[i].ParameterType;
                }
            }
            else
            {
                ParameterInfo[] @params = method.GetParameters();
                targetParams = new Type[@params.Length + 1];
                targetParams[0] = method.DeclaringType;
                for (int i = 0; i < @params.Length; i++)
                {
                    targetParams[i + 1] = @params[i].ParameterType;
                }
            }

            return targetParams;
        }

        private static void VerifyInvoker(Type invokerReturn, Type[] invokerParams, Type methodReturn, Type[] methodParams)
        {
            if (invokerParams.Length != methodParams.Length)
            {
                throw new InvalidOperationException("Delegate params and method param lengths must match");
            }

            VerifyType(invokerReturn);
            VerifyType(methodReturn);
            VerifyConversion(methodReturn, invokerReturn);
            for (int i = 0; i < invokerParams.Length; i++)
            {
                Type invokerType = invokerParams[i];
                Type methodType = methodParams[i];
                VerifyType(invokerType);
                VerifyType(methodType);
                VerifyConversion(invokerType, methodType);
            }
        }

        private static void VerifyType(Type type)
        {
            if (type.IsByRef)
            {
                throw new InvalidOperationException("MethodInvoker does not support by ref, if the arguments are known at compile time, use MethodInfo.CreateDelegate, otherwise use a Harmony reverse patch.");
            }
        }

        private static void VerifyConversion(Type from, Type to)
        {
            if (from == typeof(object) || to == typeof(object))
            {
                return;
            }
            else if (from.IsInterface && to.GetInterfaces().Contains(from))
            {
                return;
            }
            else if (from.IsAssignableTo(to))
            {
                return;
            }
            else
            {
                throw new InvalidOperationException($"Invoker type {from} cannot be converted to target {to}");
            }
        }

        private static DynamicMethod BuildInvoker(Type invokerReturn, Type[] invokerParams, MethodInfo method, Type[] methodParams)
        {
            Type[] targetParams = methodParams;
            Type targetReturn = method.ReturnType;

            DynamicMethod invoker = new DynamicMethod($"{method.Name}_Invoker", invokerReturn, invokerParams, method.DeclaringType);
            ILGenerator il = invoker.GetILGenerator();
            for (int i = 0; i < invokerParams.Length; i++)
            {
                EmitArg(il, i, invokerParams[i], targetParams[i]);
            }
            EmitCall(il, method);
            if (invokerReturn != typeof(void))
            {
                EmitConvert(il, targetReturn, invokerReturn);
            }
            Emit(il, OpCodes.Ret);
            return invoker;
        }

        private static void EmitArg(ILGenerator il, int arg, Type invokerType, Type methodType)
        {
            switch (arg)
            {
                case 0: Emit(il, OpCodes.Ldarg_0); break;
                case 1: Emit(il, OpCodes.Ldarg_1); break;
                case 2: Emit(il, OpCodes.Ldarg_2); break;
                case 3: Emit(il, OpCodes.Ldarg_3); break;
                default:
                {
                    if (arg <= 255)
                    {
                        Emit(il, OpCodes.Ldarg_S, (byte)arg);
                    }
                    else
                    {
                        Emit(il, OpCodes.Ldarg, arg);
                    }
                    break;
                }
            }

            EmitConvert(il, invokerType, methodType);
        }

        private static void EmitConvert(ILGenerator il, Type from, Type to)
        {
            Type obj = typeof(object);
            if (to != obj && from.IsAssignableTo(to))
            {
                return;
            }

            if (to == obj && from.IsValueType)
            {
                Emit(il, OpCodes.Box, from);
                return;
            }
            else if (to == obj)
            {
                return;
            }

            if (from == obj && to.IsValueType)
            {
                Emit(il, OpCodes.Unbox_Any, to);
                return;
            }
            else if (from == obj || from.IsInterface)
            {
                Emit(il, OpCodes.Castclass, to);
                return;
            }

            throw new InvalidOperationException($"Could not convert type {from} to {to}");
        }

        private static void EmitCall(ILGenerator il, MethodInfo method)
        {
            if (method.IsStatic)
            {
                Emit(il, OpCodes.Call, method);
            }
            else
            {
                Emit(il, OpCodes.Callvirt, method);
            }
        }

        private static void Emit(ILGenerator il, OpCode opCode)
        {
            il.Emit(opCode);
#if DEBUG
            CorePlugin.Log(opCode.Name);
#endif
        }

        private static void Emit(ILGenerator il, OpCode opCode, byte arg)
        {
            il.Emit(opCode, arg);
#if DEBUG
            CorePlugin.Log($"{opCode.Name} {arg}");
#endif
        }

        private static void Emit(ILGenerator il, OpCode opCode, int arg)
        {
            il.Emit(opCode, arg);
#if DEBUG
            CorePlugin.Log($"{opCode.Name} {arg}");
#endif
        }

        private static void Emit(ILGenerator il, OpCode opCode, MethodInfo method)
        {
            il.Emit(opCode, method);
#if DEBUG
            CorePlugin.Log($"{opCode.Name} {method}");
#endif
        }

        private static void Emit(ILGenerator il, OpCode opCode, Type type)
        {
            il.Emit(opCode, type);
#if DEBUG
            CorePlugin.Log($"{opCode.Name} {type}");
#endif
        }
    }
}
