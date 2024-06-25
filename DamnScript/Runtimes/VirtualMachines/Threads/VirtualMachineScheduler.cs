namespace DamnScript.Runtimes.VirtualMachines.Threads;

public unsafe struct VirtualMachineScheduler
{
    public bool HasThreads => threads.Count > 0;
    
    public List<IntPtr> threads;

    public HashSet<IntPtr> asyncThreadsInUse;
    public List<(IAsyncResult result, IntPtr pointer)> asyncThreads;

    public VirtualMachineScheduler()
    {
        this = default;
        threads = new List<IntPtr>();
        asyncThreadsInUse = new HashSet<IntPtr>();
        asyncThreads = new List<(IAsyncResult, IntPtr)>();
    }
    
    public void ExecuteNext()
    {
        for (var i = 0; i < asyncThreads.Count; i++)
        {
            var (result, pointer) = asyncThreads[i];
            if (!result.IsCompleted) 
                continue;
            
            UnregisterForAsync(pointer);
        }
        
        for (var i = 0; i < threads.Count; i++)
        {
            var thread = (VirtualMachineThread*)threads[i];
            if (asyncThreadsInUse.Contains(threads[i]))
                continue;

            IAsyncResult result;
            while (thread->ExecuteNext(out result))
            {
                if (result == null) 
                    continue;
                
                RegisterForAsync(result, threads[i]);
                break;
            }
            
            if (result == null) 
                Unregister(threads[i]);
        }
    }
    
    public void Register(IntPtr virtualMachineThreadPointer) => 
        threads.Add(virtualMachineThreadPointer);

    public void RegisterForAsync(IAsyncResult result, IntPtr virtualMachineThreadPointer)
    {
        asyncThreadsInUse.Add(virtualMachineThreadPointer);
        asyncThreads.Add((result, virtualMachineThreadPointer));
    }
    
    public void Unregister(IntPtr virtualMachineThreadPointer) => 
        threads.Remove(virtualMachineThreadPointer);

    public void UnregisterForAsync(IntPtr virtualMachineThreadPointer)
    {
        asyncThreadsInUse.Remove(virtualMachineThreadPointer);
        for (var i = 0; i < asyncThreads.Count; i++)
        {
            if (asyncThreads[i].pointer != virtualMachineThreadPointer) 
                continue;
            
            asyncThreads.RemoveAt(i);
            return;
        }
    }
}