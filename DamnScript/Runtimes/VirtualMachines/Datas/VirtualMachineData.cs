using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
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
        private static readonly Type scriptValuePtrType = typeof(ScriptValuePtr);
        private static readonly Type taskType = typeof(Task);
        private static readonly Type taskScriptValuePtrType = typeof(Task<ScriptValuePtr>);

        private static readonly Type asyncStateMachineAttributeType = typeof(AsyncStateMachineAttribute);
        
        private static readonly Dictionary<NativeMethodId, NativeMethod> methods = new();
    
        public static void RegisterNativeMethod(Delegate d, String32 name) => 
            RegisterNativeMethod(d.Method, name);
        
        public static void RegisterNativeMethod(MethodInfo method, String32 name)
        {
#if DAMN_SCRIPT_ENABLE_ADDITIONAL_CHECKS
            if (method is DynamicMethod)
                throw new Exception("Dynamic methods are not supported for native method registration. Please use Delegate instead!");
#endif
            
            var parameters = MethodInfoResolver.GetParameters(method);
            var argumentsCount = parameters!.Length;
            foreach (var parameter in parameters)
            {
                if (parameter.ParameterType == scriptValuePtrType) 
                    continue;
            
                throw new Exception("Invalid parameter type for native method. Only ScriptValuePtr is supported.");
            }
        
            var returnType = method.ReturnType;
            if (returnType!= voidType && returnType != scriptValuePtrType && 
                returnType != taskType && returnType != taskScriptValuePtrType)
            {
                throw new Exception("Invalid return type for native method. " +
                                    "Only void, ScriptValuePtr, Task and Task<ScriptValuePtr> are supported.");
            }
            
            var methodPointer = method.MethodHandle.GetFunctionPointer().ToPointer();
            var isAsync = method.GetCustomAttribute(asyncStateMachineAttributeType) != null;
            var isStatic = method.IsStatic;
            if (!isStatic)
                argumentsCount++;
            
            var hasReturnValue = method.ReturnType != voidType || method.ReturnType.IsGenericType;
            if (argumentsCount > 10)
            {
                throw new Exception("The maximum number of arguments is 10 for native method. " +
                                    "Probably it is 10 but you are using a non-static method which is add one more argument for object pointer.");
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
        
            throw new Exception($"Method with the name \"{methodName}\" and with {argumentsCount.ToString()} not found!");
        }
    }
}