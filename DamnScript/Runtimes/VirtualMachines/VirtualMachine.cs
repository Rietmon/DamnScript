using System;
using System.Threading.Tasks;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Types;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Metadatas;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.Serializations;
using DamnScript.Runtimes.VirtualMachines.Threads;

namespace DamnScript.Runtimes.VirtualMachines
{
    public readonly unsafe struct VirtualMachinePtr
    {
        public readonly VirtualMachine* value;
    
        public ref VirtualMachine RefValue => ref UnsafeUtilities.AsRef<VirtualMachine>(value);
    
        public VirtualMachinePtr(VirtualMachine* value) => this.value = value;

        public static implicit operator VirtualMachinePtr(VirtualMachine* value) => new(value);
    
        public static implicit operator VirtualMachine*(VirtualMachinePtr ptr) => ptr.value;
    }

    public unsafe partial struct VirtualMachine
    {
        public const int Version = 1;
        public bool HasThreads { get; private set; }

        public NativeArray<VirtualMachineThread> threads;

        public VirtualMachineThread* currentThread;

        public VirtualMachine(int capacity)
        {
            threads = new NativeArray<VirtualMachineThread>(capacity);
            UnsafeUtilities.Memset(threads.Begin, 0, threads.Length * sizeof(VirtualMachineThread));
            currentThread = null;
            HasThreads = false;
        }
        
        public VirtualMachineThreadPtr RunThread(ScriptDataPtr scriptData, String32 regionName)
        {
            var regionData = scriptData.value->GetRegionData(regionName);
            if (regionData == null)
            {
                Debugging.LogError($"[{nameof(VirtualMachine)}] ({nameof(RunThread)}) " +
                                   $"Region with name {regionName} not found in script \"{scriptData.value->name}\"!");
                return default;
            }
            
            var thread = new VirtualMachineThread(&scriptData.value->name, regionData, &scriptData.value->metadata);
        
            var slot = GetEmptySlotOrReAlloc();
            threads[slot] = thread;
            var ptr = threads.Begin + slot;
            HasThreads = true;
            return new VirtualMachineThreadPtr(ptr);
        }
        
        public VirtualMachineThreadPtr RunThreadFromSerialized(ScriptDataPtr scriptData, VirtualMachineSerializedThreadPtr serializedThread)
        {
            var data = serializedThread.value;
            var regionData = scriptData.value->GetRegionData(data->regionName);
            if (regionData == null)
            {
                Debugging.LogError($"[{nameof(VirtualMachine)}] ({nameof(RunThread)}) " +
                                   $"Region with name {data->regionName} not found in script \"{scriptData.value->name}\"!");
                return default;
            }
            
            var thread = new VirtualMachineThread(&scriptData.value->name, regionData, &scriptData.value->metadata)
            {
                offset = data->savePoint,
                savePoint = data->savePoint,
                stack = data->stack,
                registers = data->registers
            };
        
            var slot = GetEmptySlotOrReAlloc();
            threads[slot] = thread;
            var ptr = threads.Begin + slot;
            HasThreads = true;
            return new VirtualMachineThreadPtr(ptr);
        }

        private int GetEmptySlotOrReAlloc()
        {
            static int Find(VirtualMachine vm)
            {
                var begin = vm.threads.Begin;
                var end = vm.threads.End;
                while (begin < end)
                {
                    if (begin->isAlive)
                    {
                        begin++;
                        continue;
                    }

                    var index = (int)(begin - vm.threads.Begin);
                    return index;
                }

                return -1;
            }
            
            var index = Find(this);
            if (index == -1)
                threads = threads.ReAlloc(threads.Length * 2);
            
            index = Find(this);
            if (index == -1)
                throw new Exception($"Failed to find empty slot in threads array even after realloc!");
            
            return index;
        }
    }
}