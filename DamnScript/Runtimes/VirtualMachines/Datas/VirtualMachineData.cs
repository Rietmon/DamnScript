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
        private static readonly Type voidType = typeof(void);
        private static readonly Type scriptValueType = typeof(ScriptValue);
        private static readonly Type taskType = typeof(Task);
        private static readonly Type taskScriptValueType = typeof(Task<ScriptValue>);
        
        private static readonly Dictionary<NativeMethodId, NativeMethod> methods = new();
    
        public static void RegisterNativeMethod(Delegate d, String32 name) => 
            RegisterNativeMethod(d.Method, name);
        
        public static void RegisterNativeMethod(MethodInfo method, String32 name)
        {
            var parameters = method.GetParameters();
            var argumentsCount = parameters.Length;
            foreach (var parameter in parameters)
            {
                if (parameter.ParameterType == typeof(ScriptValue)) 
                    continue;
            
                Debugging.LogError($"[{nameof(ScriptEngine)}] ({nameof(RegisterNativeMethod)}) " +
                                   $"Method \"{name}\" has invalid parameter type: {parameter.ParameterType}");
                return;
            }
        
            var returnType = method.ReturnType;
            if (returnType!= voidType && returnType != scriptValueType && 
                returnType != taskType && returnType != taskScriptValueType)
            {
                Debugging.LogError($"[{nameof(ScriptEngine)}] ({nameof(RegisterNativeMethod)}) " +
                                   $"Method \"{name}\" has invalid return type: {returnType}");
                return;
            }
            
            var methodPointer = method.MethodHandle.GetFunctionPointer().ToPointer();
            var isAsync = method.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;
            var isStatic = method.IsStatic;
            if (!isStatic)
                argumentsCount++;
            var hasReturnValue = method.ReturnType != typeof(void) || method.ReturnType.IsGenericType;
            if (argumentsCount > 10)
            {
                Debugging.LogError($"[{nameof(ScriptEngine)}] ({nameof(RegisterNativeMethod)}) " +
                                   $"The maximum number of arguments is 10 for native method. " +
                                   "Probably it is 10 but you are using a non-static method which is add one more argument for object pointer.");
                return;
            }
            
            var id = new NativeMethodId(name, argumentsCount);
            var nativeMethod = new NativeMethod(methodPointer, argumentsCount, isAsync, isStatic, hasReturnValue);

            if (methods.TryAdd(id, nativeMethod)) 
                return;
            
            Debugging.LogWarning($"[{nameof(ScriptEngine)}] ({nameof(RegisterNativeMethod)}) " +
                                 $"Method with the name \"{name}\" and with {argumentsCount.ToString()} arguments is already registered.");
        }
    
        public static bool TryGetNativeMethod(String32 methodName, int argumentsCount, out NativeMethod method)
        {
            var id = new NativeMethodId(methodName, argumentsCount);
            if (methods.TryGetValue(id, out method)) 
                return true;
        
            Debugging.LogError($"[{nameof(ScriptEngine)}] ({nameof(TryGetNativeMethod)}) " +
                               $"Method not found: {methodName}");
            return false;
        }
    }
}