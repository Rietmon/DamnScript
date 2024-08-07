using System;
using System.Threading.Tasks;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Types;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.Threads;

namespace DamnScript.Runtimes.VirtualMachines
{
    public readonly unsafe struct VirtualMachinePtr
    {
        public readonly VirtualMachine* value;
    
        public ref VirtualMachine RefValue => ref *value;
    
        public VirtualMachinePtr(VirtualMachine* value) => this.value = value;

        public static implicit operator VirtualMachinePtr(VirtualMachine* value) => new(value);
    
        public static implicit operator VirtualMachine*(VirtualMachinePtr ptr) => ptr.value;
    }

    public unsafe struct VirtualMachine
    {
        public const int Version = 1;
        public bool HasThreads => threads.Count > 0;
        public bool HasThreadsAwaiting => threadsAreAwait.Count > 0;

        public NativeList<VirtualMachineThread> threads;
        public NativeList<(IntPtr result, VirtualMachineThreadPtr pointer)> threadsAreAwait;

        public VirtualMachineThread* currentThread;

        public VirtualMachine(int capacity)
        {
            threads = new NativeList<VirtualMachineThread>(capacity);
            threadsAreAwait = new NativeList<(IntPtr, VirtualMachineThreadPtr)>(capacity);
            currentThread = null;
        }
    
        public bool ExecuteNext()
        {
            ExecuteAwaitsThreads();
            ExecuteThreads();
        
            return HasThreads || HasThreadsAwaiting;
        }
    
        private void ExecuteAwaitsThreads()
        {
            var begin = threadsAreAwait.Begin;
            var end = threadsAreAwait.End;
            while (begin < end)
            {
                currentThread = begin->pointer;
                var result = UnsafeUtilities.PointerToReference<IAsyncResult>(begin->result.ToPointer());
                if (result.IsCompleted)
                {
                    RemoveFromAwait(begin->pointer);
                    if (result is Task<ScriptValue> task)
                        begin->pointer.value->StackPush(task.Result.longValue);
                }
            
                begin++;
            }
        }
    
        private void ExecuteThreads()
        {
            var begin = threads.Begin;
            var end = threads.End;
            while (begin < end)
            {
                currentThread = begin;
                if (!IsInAwait(begin))
                {
                    Task result;
                    while (begin->ExecuteNext(out result))
                    {
                        if (result == null)
                            continue;

                        AddToAwait(result, begin);
                        break;
                    }

                    if (result == null)
                        Unregister(*begin);
                }
            
                begin++;
            }
        }

        public void AddToAwait(Task result, VirtualMachineThreadPtr pointer) => 
            threadsAreAwait.Add(((IntPtr)UnsafeUtilities.ReferenceToPointer(result), pointer));
    
        public bool IsInAwait(VirtualMachineThreadPtr virtualMachineThreadPointer)
        {
            var begin = threadsAreAwait.Begin;
            var end = threadsAreAwait.End;
            while (begin < end)
            {
                if (begin->pointer.value == virtualMachineThreadPointer.value)
                    return true;
            
                begin++;
            }

            return false;
        }

        public void RemoveFromAwait(VirtualMachineThreadPtr virtualMachineThreadPointer)
        {
            var begin = threadsAreAwait.Begin;
            var end = threadsAreAwait.End;
            var i = 0;
            while (begin < end)
            {
                if (begin->pointer.value == virtualMachineThreadPointer.value)
                {
                    threadsAreAwait.RemoveAt(i);
                    return;
                }
            
                begin++;
                i++;
            }
        }

        public VirtualMachineThreadPtr Register(VirtualMachineThread thread)
        {
            threads.Add(thread);
            var ptr = threads.End - 1;
            return new VirtualMachineThreadPtr(ptr);
        }

        public void Unregister(VirtualMachineThread thread) => 
            threads.Remove(thread);
    }
}