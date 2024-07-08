using System.Reflection;
using System.Runtime.CompilerServices;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Natives;

namespace DamnScript.Runtimes.VirtualMachines.Datas;

public static unsafe class VirtualMachineData
{
    private static readonly Dictionary<string, NativeMethod> methods = new();
    
    public static void RegisterNativeMethod(Delegate d) => RegisterNativeMethod(d.Method);
    public static void RegisterNativeMethod(Delegate d, string name) => RegisterNativeMethod(d.Method, name);
    public static void RegisterNativeMethod(MethodInfo method) => RegisterNativeMethod(method, method.Name);
    public static void RegisterNativeMethod(MethodInfo method, string name)
    {
        if (methods.ContainsKey(name))
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
        methods.Add(name, nativeMethod);
    }
    
    public static bool TryGetNativeMethod(string methodName, out NativeMethod method)
    {
        if (methods.TryGetValue(methodName, out method)) 
            return true;
        
        Debugging.LogError($"[{nameof(ScriptEngine)}] ({nameof(TryGetNativeMethod)}) " +
                           $"Method not found: {methodName}");
        return false;
    }
}