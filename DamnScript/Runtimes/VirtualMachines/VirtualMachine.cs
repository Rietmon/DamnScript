using System;
using System.Threading.Tasks;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Types;
using DamnScript.Runtimes.Metadatas;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.Serializations;
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

    public unsafe partial struct VirtualMachine
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
        
        public VirtualMachineThreadPtr RunThread(ScriptDataPtr scriptData, String32 regionName)
        {
            var regionData = scriptData.value->GetRegionData(regionName);
            if (regionData == null)
                throw new Exception($"Region with name {regionName} not found in script \"{scriptData.value->name}\"!");
            
            var thread = new VirtualMachineThread(&scriptData.value->name, regionData, &scriptData.value->metadata);
        
            threads.Add(thread);
            var ptr = threads.End - 1;
            return new VirtualMachineThreadPtr(ptr);
        }
        
        public VirtualMachineThreadPtr RunThreadFromSerialized(ScriptDataPtr scriptData, VirtualMachineSerializedThreadPtr serializedThread)
        {
            var data = serializedThread.value;
            var regionData = scriptData.value->GetRegionData(data->regionName);
            if (regionData == null)
                throw new Exception($"Region with name {data->regionName} not found in script \"{scriptData.value->name}\"!");
            
            var thread = new VirtualMachineThread(&scriptData.value->name, regionData, &scriptData.value->metadata)
            {
                offset = data->savePoint,
                savePoint = data->savePoint,
                stack = data->stack,
                registers = data->registers
            };
        
            threads.Add(thread);
            var ptr = threads.End - 1;
            return new VirtualMachineThreadPtr(ptr);
        }
    }
}