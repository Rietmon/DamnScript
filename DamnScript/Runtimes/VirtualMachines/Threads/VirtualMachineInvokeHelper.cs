using System;
using System.Threading.Tasks;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.VirtualMachines.Methods;
using SV = DamnScript.Runtimes.VirtualMachines.ScriptValues.ScriptValue;

namespace DamnScript.Runtimes.VirtualMachines.Threads
{
    public static unsafe class VirtualMachineInvokeHelper
    {
        public static SV Invoke(NativeMethod method, SV* arguments, out Task task)
        {
            task = null;
            if (method.IsStatic)
            {
                if (!method.IsAsync)
                {
                    if (method.HasReturnValue)
                        return InvokeStaticValue(method, arguments);

                    InvokeStaticVoid(method, arguments);
                }
                else
                {
                    var taskPtr = InvokeStaticValue(method, arguments);
                    task = UnsafeUtilities.PointerToReference<Task>(taskPtr);
                }
            }
            else
            {
                if (!method.IsAsync)
                {
                    if (method.HasReturnValue)
                    {
                        var resultPtr = (SV*)InvokeValue(method, arguments);
                        return *resultPtr;
                    }

                    InvokeVoid(method, arguments);
                }
                else
                {
                    var taskPtr = InvokeValue(method, arguments);
                    task = UnsafeUtilities.PointerToReference<Task>(taskPtr);
                }
            }

            return default;
        }
    
        public static void InvokeStaticVoid(NativeMethod method, SV* a)
        {
            var argumentsCount = method.argumentsCount;
            var methodPointer = method.methodPointer;
            switch (argumentsCount)
            {
                case 0: ((delegate*<void>)methodPointer)
                    (); break;
                case 1: ((delegate*<SV*, void>)methodPointer)
                    (a); break;
                case 2: ((delegate*<SV*, SV*, void>)methodPointer)
                    (a, a + 1); break;
                case 3: ((delegate*<SV*, SV*, SV*, void>)methodPointer)
                    (a, a + 1, a + 2); break;
                case 4: ((delegate*<SV*, SV*, SV*, SV*, void>)methodPointer)
                    (a, a + 1, a + 2, a + 3); break;
                case 5: ((delegate*<SV*, SV*, SV*, SV*, SV*, void>)methodPointer)
                    (a, a + 1, a + 2, a + 3, a + 4); break;
                case 6: ((delegate*<SV*, SV*, SV*, SV*, SV*, SV*, void>)methodPointer)
                    (a, a + 1, a + 2, a + 3, a + 4, a + 5); break;
                case 7: ((delegate*<SV*, SV*, SV*, SV*, SV*, SV*, SV*, void>)methodPointer)
                    (a, a + 1, a + 2, a + 3, a + 4, a + 5, a + 6); break;
                case 8: ((delegate*<SV*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, void>)methodPointer)
                    (a, a + 1, a + 2, a + 3, a + 4, a + 5, a + 6, a + 7); break;
                case 9: ((delegate*<SV*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, void>)methodPointer)
                    (a, a + 1, a + 2, a + 3, a + 4, a + 5, a + 6, a + 7, a + 8); break;
                case 10: ((delegate*<SV*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, void>)methodPointer)
                    (a, a + 1, a + 2, a + 3, a + 4, a + 5, a + 6, a + 7, a + 8, a + 9); break;
                default: throw new Exception("Invalid arguments count! It must be between 0 and 10.");
            }
        }
    
        public static void* InvokeStaticValue(NativeMethod method, SV* a)
        {
            var argumentsCount = method.argumentsCount;
            var methodPointer = method.methodPointer;
            var returnValue = argumentsCount switch
            {
                0 => ((delegate*<void*>)methodPointer)
                    (),
                1 => ((delegate*<SV*, void*>)methodPointer)
                    (a),
                2 => ((delegate*<SV*, SV*, void*>)methodPointer)
                    (a, a + 1),
                3 => ((delegate*<SV*, SV*, SV*, void*>)methodPointer)
                    (a, a + 1, a + 2),
                4 => ((delegate*<SV*, SV*, SV*, SV*, void*>)methodPointer)
                    (a, a + 1, a + 2, a + 3),
                5 => ((delegate*<SV*, SV*, SV*, SV*, SV*, void*>)methodPointer)
                    (a, a + 1, a + 2, a + 3, a + 4),
                6 => ((delegate*<SV*, SV*, SV*, SV*, SV*, SV*, void*>)methodPointer)
                    (a, a + 1, a + 2, a + 3, a + 4, a + 5),
                7 => ((delegate*<SV*, SV*, SV*, SV*, SV*, SV*, SV*, void*>)methodPointer)
                    (a, a + 1, a + 2, a + 3, a + 4, a + 5, a + 6),
                8 => ((delegate*<SV*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, void*>)methodPointer)
                    (a, a + 1, a + 2, a + 3, a + 4, a + 5, a + 6, a + 7),
                9 => ((delegate*<SV*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, void*>)methodPointer)
                    (a, a + 1, a + 2, a + 3, a + 4, a + 5, a + 6, a + 7, a + 8),
                10 => ((delegate*<SV*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, void*>)methodPointer)
                    (a, a + 1, a + 2, a + 3, a + 4, a + 5, a + 6, a + 7, a + 8, a + 9),
                _ => throw new Exception("Invalid arguments count! It must be between 0 and 10.")
            };
            return returnValue;
        }
    
        public static void InvokeVoid(NativeMethod method, SV* a)
        {
            var argumentsCount = method.argumentsCount;
            var methodPointer = method.methodPointer;
            
            var objectInstance = a->GetReferencePointer();
            switch (argumentsCount)
            {
                case 0: ((delegate*<void>)methodPointer)
                    (); break;
                case 1: ((delegate*<void*, void>)methodPointer)
                    (objectInstance); break;
                case 2: ((delegate*<void*, SV*, void>)methodPointer)
                    (objectInstance, a); break;
                case 3: ((delegate*<void*, SV*, SV*, void>)methodPointer)
                    (objectInstance, a + 1, a + 2); break;
                case 4: ((delegate*<void*, SV*, SV*, SV*, void>)methodPointer)
                    (objectInstance, a + 1, a + 2, a + 3); break;
                case 5: ((delegate*<void*, SV*, SV*, SV*, SV*, void>)methodPointer)
                    (objectInstance, a + 1, a + 2, a + 3, a + 4); break;
                case 6: ((delegate*<void*, SV*, SV*, SV*, SV*, SV*, void>)methodPointer)
                    (objectInstance, a + 1, a + 2, a + 3, a + 4, a + 5); break;
                case 7: ((delegate*<void*, SV*, SV*, SV*, SV*, SV*, SV*, void>)methodPointer)
                    (objectInstance, a + 1, a + 2, a + 3, a + 4, a + 5, a + 6); break;
                case 8: ((delegate*<void*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, void>)methodPointer)
                    (objectInstance, a + 1, a + 2, a + 3, a + 4, a + 5, a + 6, a + 7); break;
                case 9: ((delegate*<void*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, void>)methodPointer)
                    (objectInstance, a + 1, a + 2, a + 3, a + 4, a + 5, a + 6, a + 7, a + 8); break;
                case 10: ((delegate*<void*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, void>)methodPointer)
                    (objectInstance, a + 1, a + 2, a + 3, a + 4, a + 5, a + 6, a + 7, a + 8, a + 9); break;
                default: throw new Exception("Invalid arguments count! It must be between 0 and 10.");
            }
        }
    
        public static void* InvokeValue(NativeMethod method, SV* a)
        {
            var argumentsCount = method.argumentsCount;
            var methodPointer = method.methodPointer;

            var objectInstance = a->GetReferencePointer();
            var returnValue = argumentsCount switch
            {
                1 => ((delegate*<void*, void*>)methodPointer)
                    (objectInstance),
                2 => ((delegate*<void*, SV*, void*>)methodPointer)
                    (objectInstance, a + 1),
                3 => ((delegate*<void*, SV*, SV*, void*>)methodPointer)
                    (objectInstance, a + 1, a + 2),
                4 => ((delegate*<void*, SV*, SV*, SV*, void*>)methodPointer)
                    (objectInstance, a + 1, a + 2, a + 3),
                5 => ((delegate*<void*, SV*, SV*, SV*, SV*, void*>)methodPointer)
                    (objectInstance, a + 1, a + 2, a + 3, a + 4),
                6 => ((delegate*<void*, SV*, SV*, SV*, SV*, SV*, void*>)methodPointer)
                    (objectInstance, a + 1, a + 2, a + 3, a + 4, a + 5),
                7 => ((delegate*<void*, SV*, SV*, SV*, SV*, SV*, SV*, void*>)methodPointer)
                    (objectInstance, a + 1, a + 2, a + 3, a + 4, a + 5, a + 6),
                8 => ((delegate*<void*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, void*>)methodPointer)
                    (objectInstance, a + 1, a + 2, a + 3, a + 4, a + 5, a + 6, a + 7),
                9 => ((delegate*<void*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, void*>)methodPointer)
                    (objectInstance, a + 1, a + 2, a + 3, a + 4, a + 5, a + 6, a + 7, a + 8),
                10 => ((delegate*<void*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, SV*, void*>)methodPointer)
                    (objectInstance, a + 1, a + 2, a + 3, a + 4, a + 5, a + 6, a + 7, a + 8, a + 9),
                _ => throw new Exception("Invalid arguments count! It must be between 1 and 10.")
            };
            return returnValue;
        }
    }
}