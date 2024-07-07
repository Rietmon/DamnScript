using DamnScript.Runtimes.Natives;
using SV = DamnScript.Runtimes.Natives.ScriptValue;

namespace DamnScript.Runtimes.VirtualMachines.Threads;

public static unsafe class VirtualMachineInvokeHelper
{
    public static SV Invoke(NativeMethod method, SV* arguments, out Task task)
    {
        task = null;
        if (!method.isAsync)
        {
            if (method.hasReturnValue)
                return InvokeValue<SV>(method, arguments);
            
            InvokeVoid(method, arguments);
        }
        else
        {
            task = method.hasReturnValue
                ? InvokeValue<Task<SV>>(method, arguments)
                : InvokeValue<Task>(method, arguments);
        }

        return default;
    }
    
    public static void InvokeVoid(NativeMethod method, SV* a)
    {
        var argumentsCount = method.argumentsCount;
        var methodPointer = method.methodPointer;
        switch (argumentsCount)
        {
            case 0: ((delegate*<void>)methodPointer)
                (); break;
            case 1: ((delegate*<SV, void>)methodPointer)
                (a[0]); break;
            case 2: ((delegate*<SV, SV, void>)methodPointer)
                (a[0], a[1]); break;
            case 3: ((delegate*<SV, SV, SV, void>)methodPointer)
                (a[0], a[1], a[2]); break;
            case 4: ((delegate*<SV, SV, SV, SV, void>)methodPointer)
                (a[0], a[1], a[2], a[3]); break;
            case 5: ((delegate*<SV, SV, SV, SV, SV, void>)methodPointer)
                (a[0], a[1], a[2], a[3], a[4]); break;
            case 6: ((delegate*<SV, SV, SV, SV, SV, SV, void>)methodPointer)
                (a[0], a[1], a[2], a[3], a[4], a[5]); break;
            case 7: ((delegate*<SV, SV, SV, SV, SV, SV, SV, void>)methodPointer)
                (a[0], a[1], a[2], a[3], a[4], a[5], a[6]); break;
            case 8: ((delegate*<SV, SV, SV, SV, SV, SV, SV, SV, void>)methodPointer)
                (a[0], a[1], a[2], a[3], a[4], a[5], a[6], a[7]); break;
            case 9: ((delegate*<SV, SV, SV, SV, SV, SV, SV, SV, SV, void>)methodPointer)
                (a[0], a[1], a[2], a[3], a[4], a[5], a[6], a[7], a[8]); break;
            case 10: ((delegate*<SV, SV, SV, SV, SV, SV, SV, SV, SV, SV, void>)methodPointer)
                (a[0], a[1], a[2], a[3], a[4], a[5], a[6], a[7], a[8], a[9]); break;
            default: throw new Exception("Invalid arguments count! It must be between 0 and 10.");
        }
    }
    
    public static T InvokeValue<T>(NativeMethod method, SV* a)
    {
        var argumentsCount = method.argumentsCount;
        var methodPointer = method.methodPointer;
        return argumentsCount switch
        {
            0 => ((delegate*<T>)methodPointer)
                (),
            1 => ((delegate*<SV, T>)methodPointer)
                (a[0]),
            2 => ((delegate*<SV, SV, T>)methodPointer)
                (a[0], a[1]),
            3 => ((delegate*<SV, SV, SV, T>)methodPointer)
                (a[0], a[1], a[2]),
            4 => ((delegate*<SV, SV, SV, SV, T>)methodPointer)
                (a[0], a[1], a[2], a[3]),
            5 => ((delegate*<SV, SV, SV, SV, SV, T>)methodPointer)
                (a[0], a[1], a[2], a[3], a[4]),
            6 => ((delegate*<SV, SV, SV, SV, SV, SV, T>)methodPointer)
                (a[0], a[1], a[2], a[3], a[4], a[5]),
            7 => ((delegate*<SV, SV, SV, SV, SV, SV, SV, T>)methodPointer)
                (a[0], a[1], a[2], a[3], a[4], a[5], a[6]),
            8 => ((delegate*<SV, SV, SV, SV, SV, SV, SV, SV, T>)methodPointer)
                (a[0], a[1], a[2], a[3], a[4], a[5], a[6], a[7]),
            9 => ((delegate*<SV, SV, SV, SV, SV, SV, SV, SV, SV, T>)methodPointer)
                (a[0], a[1], a[2], a[3], a[4], a[5], a[6], a[7], a[8]),
            10 => ((delegate*<SV, SV, SV, SV, SV, SV, SV, SV, SV, SV, T>)methodPointer)
                (a[0], a[1], a[2], a[3], a[4], a[5], a[6], a[7], a[8], a[9]),
            _ => throw new Exception("Invalid arguments count! It must be between 0 and 10.")
        };
    }
}