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
        public VirtualMachinePtr(ref VirtualMachine value) => this.value = UnsafeUtilities.AsPointer(ref value);

        public static implicit operator VirtualMachinePtr(VirtualMachine* value) => new(value);
    
        public static implicit operator VirtualMachine*(VirtualMachinePtr ptr) => ptr.value;
    }

    public unsafe partial struct VirtualMachine : IDisposable
    {
        public const int Version = 1;
        public bool HasThreads { get; private set; }

        public NativeArray<VirtualMachineThread> threads;

        public VirtualMachineThread* currentThread;

        public bool IsAlive { get; private set; }

        public VirtualMachine(int capacity)
        {
            HasThreads = false;
            threads = new NativeArray<VirtualMachineThread>(capacity, true);
            currentThread = null;
            IsAlive = true;
        }
        
        public VirtualMachineThreadHandle RunThread(ScriptDataPtr scriptData, String32 regionName)
        {
            var regionData = scriptData.value->GetRegionData(regionName);
            if (regionData == null)
                throw new Exception($"Region with name {regionName} not found in script \"{scriptData.value->name}\"!");
            
            var thread = new VirtualMachineThread(&scriptData.value->name, regionData, &scriptData.value->metadata);
        
            var slot = GetEmptySlotOrReAlloc();
            threads[slot] = thread;
            HasThreads = true;
            return new VirtualMachineThreadHandle(slot, new VirtualMachinePtr(ref this));
        }
        
        public VirtualMachineThreadHandle RunThreadFromSerialized(ScriptDataPtr scriptData, VirtualMachineSerializedThreadPtr serializedThread)
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
        
            var slot = GetEmptySlotOrReAlloc();
            threads[slot] = thread;
            HasThreads = true;
            return new VirtualMachineThreadHandle(slot, new VirtualMachinePtr(ref this));
        }

        public int GetEmptySlotOrReAlloc()
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
            if (index != -1)
                return index;
                
            var newThreads = new NativeArray<VirtualMachineThread>(threads.Length * 2, true);
            UnsafeUtilities.Memcpy(threads.Begin, newThreads.Begin, sizeof(VirtualMachineThread) * threads.Length);
            threads.Dispose();
            threads = newThreads;
            
            index = Find(this);
            if (index == -1)
                throw new Exception($"Failed to find empty slot in threads array even after realloc!");
            
            return index;
        }

        /// <summary>
        /// Dispose current virtual machine and stops all threads.
        /// </summary>
        public void Dispose()
        {
            if (HasThreads)
            {
                var begin = threads.Begin;
                var end = threads.End;
                while (begin < end)
                {
                    if (begin->isAlive)
                        begin->Dispose();

                    begin++;
                }
            }

            threads.Dispose();
            this = default;
        }
    }
}