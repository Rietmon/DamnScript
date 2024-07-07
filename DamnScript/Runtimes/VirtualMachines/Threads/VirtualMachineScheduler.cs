using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Natives;

namespace DamnScript.Runtimes.VirtualMachines.Threads;

public readonly unsafe struct VirtualMachineSchedulerPtr
{
    public readonly VirtualMachineScheduler* value;
    
    public ref VirtualMachineScheduler RefValue => ref *value;
    
    public VirtualMachineSchedulerPtr(VirtualMachineScheduler* value) => this.value = value;

    public static implicit operator VirtualMachineSchedulerPtr(VirtualMachineScheduler* value) => new(value);
    
    public static implicit operator VirtualMachineScheduler*(VirtualMachineSchedulerPtr ptr) => ptr.value;
}

public unsafe struct VirtualMachineScheduler
{
    public bool HasThreads => _threads.Count > 0;
    public bool HasThreadsAwaiting => _threadsAreAwait.Count > 0;

    private NativeList<VirtualMachineThread> _threads;
    private NativeList<(IntPtr result, VirtualMachineThreadPtr pointer)> _threadsAreAwait;

    public VirtualMachineScheduler(int capacity)
    {
        _threads = new NativeList<VirtualMachineThread>(capacity);
        _threadsAreAwait = new NativeList<(IntPtr, VirtualMachineThreadPtr)>(capacity);
    }
    
    public bool ExecuteNext()
    {
        ExecuteAwaitsThreads();
        ExecuteThreads();
        
        return HasThreads || HasThreadsAwaiting;
    }
    
    private void ExecuteAwaitsThreads()
    {
        var begin = _threadsAreAwait.Begin;
        var end = _threadsAreAwait.End;
        while (begin < end)
        {
            var result = UnsafeUtilities.PointerToReference<IAsyncResult>(begin->result.ToPointer());
            if (result.IsCompleted)
            {
                RemoveFromAwait(begin->pointer);
                if (result is Task<ScriptValue> task)
                    begin->pointer.value->Push(task.Result.longValue);
            }
            
            begin++;
        }
    }
    
    private void ExecuteThreads()
    {
        var begin = _threads.Begin;
        var end = _threads.End;
        while (begin < end)
        {
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
        _threadsAreAwait.Add(((IntPtr)UnsafeUtilities.ReferenceToPointer(result), pointer));
    
    public bool IsInAwait(VirtualMachineThreadPtr virtualMachineThreadPointer)
    {
        var begin = _threadsAreAwait.Begin;
        var end = _threadsAreAwait.End;
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
        var begin = _threadsAreAwait.Begin;
        var end = _threadsAreAwait.End;
        var i = 0;
        while (begin < end)
        {
            if (begin->pointer.value == virtualMachineThreadPointer.value)
            {
                _threadsAreAwait.RemoveAt(i);
                return;
            }
            
            begin++;
            i++;
        }
    }

    public VirtualMachineThreadPtr Register(VirtualMachineThread thread)
    {
        _threads.Add(thread);
        var ptr = _threads.End - 1;
        return new VirtualMachineThreadPtr(ptr);
    }

    public void Unregister(VirtualMachineThread thread) => 
        _threads.Remove(thread);
}