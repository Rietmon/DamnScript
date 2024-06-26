using System.Reflection;
using System.Runtime.CompilerServices;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Metadatas;
using DamnScript.Runtimes.Natives;

namespace DamnScript.Runtimes.VirtualMachines;

public static unsafe class VirtualMachine
{
    public static Dictionary<string, NativeMethod> methods = new();
    
    public static ConstantsData constants;
    
    public static void RegisterNativeMethod(Delegate method)
    {
        var methodName = method.Method.Name;
        var methodPointer = method.Method.MethodHandle.GetFunctionPointer().ToPointer();
        var argumentsCount = method.Method.GetParameters().Length;
        var isAsync = IsAsyncMethod(method.Method, typeof(AsyncStateMachineAttribute));
        var isStatic = method.Method.IsStatic;
        var nativeMethod = new NativeMethod(methodPointer, argumentsCount, isAsync, isStatic);
        methods.Add(methodName, nativeMethod);
    }
    
    public static void RegisterNativeMethod(MethodInfo method)
    {
        var methodName = method.Name;
        var methodPointer = method.MethodHandle.GetFunctionPointer().ToPointer();
        var isAsync = IsAsyncMethod(method, typeof(AsyncStateMachineAttribute));
        var argumentsCount = method.GetParameters().Length;
        var isStatic = method.IsStatic;
        var nativeMethod = new NativeMethod(methodPointer, argumentsCount, isAsync, isStatic);
        methods.Add(methodName, nativeMethod);
    }
    
    public static bool TryGetNativeMethod(string methodName, out NativeMethod method)
    {
        if (methods.TryGetValue(methodName, out method)) 
            return true;
        
        Debugging.LogError($"[{nameof(VirtualMachine)}] ({nameof(TryGetNativeMethod)}) " +
                           $"Method not found: {methodName}");
        return false;
    }
    
    private static bool IsAsyncMethod(MethodInfo method, Type attType) => method.GetCustomAttribute(attType) != null;
}