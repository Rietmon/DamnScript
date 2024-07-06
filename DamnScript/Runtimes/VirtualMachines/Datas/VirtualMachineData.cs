using System.Reflection;
using System.Runtime.CompilerServices;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Natives;

namespace DamnScript.Runtimes.VirtualMachines.Datas;

public static unsafe class VirtualMachineData
{
    private static readonly Dictionary<string, NativeMethod> methods = new();
    
    public static void RegisterNativeMethod(Delegate d) => 
        RegisterNativeMethod(d.Method);
    
    public static void RegisterNativeMethod(MethodInfo method)
    {
        var methodName = method.Name;
        var methodPointer = method.MethodHandle.GetFunctionPointer().ToPointer();
        var isAsync = method.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;
        var argumentsCount = method.GetParameters().Length;
        var isStatic = method.IsStatic;
        var nativeMethod = new NativeMethod(methodPointer, argumentsCount, isAsync, isStatic);
        methods.Add(methodName, nativeMethod);
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