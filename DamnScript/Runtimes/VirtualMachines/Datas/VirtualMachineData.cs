using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DamnScript.Runtimes.Cores.Types;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Natives;

namespace DamnScript.Runtimes.VirtualMachines.Datas
{
    public static unsafe class VirtualMachineData
    {
        private static readonly Dictionary<String32, NativeMethod> methods = new();
    
        public static void RegisterNativeMethod(Delegate d) => RegisterNativeMethod(d.Method);
        public static void RegisterNativeMethod(Delegate d, ConstString name) => RegisterNativeMethod(d.Method, name);
        public static void RegisterNativeMethod(MethodInfo method) => RegisterNativeMethod(method, method.Name);
        public static void RegisterNativeMethod(MethodInfo method, ConstString name)
        {
            var str32 = name.ToString32();
            if (methods.ContainsKey(str32))
            {
                Debugging.LogWarning($"[{nameof(ScriptEngine)}] ({nameof(RegisterNativeMethod)}) " +
                                     $"Method already registered: \"{name}\"");
                return;
            }
        
            foreach (var parameter in method.GetParameters())
            {
                if (parameter.ParameterType == typeof(ScriptValue)) 
                    continue;
            
                Debugging.LogError($"[{nameof(ScriptEngine)}] ({nameof(RegisterNativeMethod)}) " +
                                   $"Method \"{name}\" has invalid parameter type: {parameter.ParameterType}");
                return;
            }
        
            if (!(method.ReturnType == typeof(void) || method.ReturnType == typeof(ScriptValue) ||
                  method.ReturnType == typeof(Task) || method.ReturnType == typeof(Task<ScriptValue>)))
            {
                Debugging.LogError($"[{nameof(ScriptEngine)}] ({nameof(RegisterNativeMethod)}) " +
                                   $"Method \"{name}\" has invalid return type: {method.ReturnType}");
                return;
            }
        
            var methodPointer = method.MethodHandle.GetFunctionPointer().ToPointer();
            var isAsync = method.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;
            var argumentsCount = method.GetParameters().Length;
            var isStatic = method.IsStatic;
            var hasReturnValue = method.ReturnType != typeof(void) || method.ReturnType.IsGenericType;
            var nativeMethod = new NativeMethod(methodPointer, argumentsCount, isAsync, isStatic, hasReturnValue);
            methods.Add(str32, nativeMethod);
        }
    
        public static bool TryGetNativeMethod(String32 methodName, out NativeMethod method)
        {
            if (methods.TryGetValue(methodName, out method)) 
                return true;
        
            Debugging.LogError($"[{nameof(ScriptEngine)}] ({nameof(TryGetNativeMethod)}) " +
                               $"Method not found: {methodName}");
            return false;
        }
    }
}