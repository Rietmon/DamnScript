using System;
using System.Threading.Tasks;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Collections;
using DamnScript.Runtimes.Cores.Strings;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Serializations;
using DamnScript.Runtimes.VirtualMachines.Scripts;
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
#if DAMN_SCRIPT_THREADS_CAPACITY_8
        public const int DefaultThreadsCapacity = 8;
#elif DAMN_SCRIPT_THREADS_CAPACITY_16
        public const int DefaultThreadsCapacity = 16;
#elif DAMN_SCRIPT_THREADS_CAPACITY_32
        public const int DefaultThreadsCapacity = 32;
#elif DAMN_SCRIPT_THREADS_CAPACITY_64
        public const int DefaultThreadsCapacity = 64;
#else
        public const int DefaultThreadsCapacity = 4;
#endif
        
        public const int Version = 1;
        
        public bool IsAlive => threads.Begin != null;

        public NativeArray<VirtualMachineThread> threads;

        public VirtualMachineThread* currentThread;

        public VirtualMachine(int capacity)
        {
            threads = new NativeArray<VirtualMachineThread>(capacity, true);
            currentThread = null;
        }
        
        public VirtualMachineThreadHandle RunThread(ScriptDataPtr scriptData, String32 regionName)
        {
            var regionData = scriptData.value->GetRegionData(regionName);
            if (regionData == null)
                throw new Exception($"Region with name {regionName} not found in script \"{scriptData.value->name}\"!");
            
            var thread = new VirtualMachineThread(scriptData.value, regionData, &scriptData.value->metadata);
            var slot = GetEmptySlotOrReAlloc();
            threads[slot] = thread;
            return new VirtualMachineThreadHandle(slot, new VirtualMachinePtr(ref this));
        }
        
        public VirtualMachineThreadHandle RunThreadFromSerialized(ScriptDataPtr scriptData, VirtualMachineSerializedThreadPtr serializedThread)
        {
            var data = serializedThread.value;
            var regionData = scriptData.value->GetRegionData(data->regionName);
            if (regionData == null)
                throw new Exception($"Region with name {data->regionName} not found in script \"{scriptData.value->name}\"!");
            
            var thread = new VirtualMachineThread(scriptData.value, regionData, &scriptData.value->metadata)
            {
                offset = data->savePoint,
                savePoint = data->savePoint,
                stack = data->stack,
                threadRegisters = data->threadRegisters
            };
            var slot = GetEmptySlotOrReAlloc();
            threads[slot] = thread;
            return new VirtualMachineThreadHandle(slot, new VirtualMachinePtr(ref this));
        }

        public int GetEmptySlotOrReAlloc()
        {
            static int FindEmptySlot(VirtualMachine vm)
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
            
            var index = FindEmptySlot(this);
            if (index != -1)
                return index;
            
            var newThreads = new NativeArray<VirtualMachineThread>(threads.Length * 2, true);
            UnsafeUtilities.Memcpy(threads.Begin, newThreads.Begin, sizeof(VirtualMachineThread) * threads.Length);
            threads.Dispose();
            threads = newThreads;
            
            index = FindEmptySlot(this);
            
#if DAMN_SCRIPT_ENABLE_ADDITIONAL_CHECKS
            if (index == -1)
                throw new Exception($"Failed to find empty slot in threads array even after realloc!");
#endif
            
            return index;
        }

        /// <summary>
        /// Dispose current virtual machine and stops all threads.
        /// </summary>
        public void Dispose()
        {
            var begin = threads.Begin;
            var end = threads.End;
            while (begin < end)
            {
                if (begin->isAlive)
                    begin->Dispose();

                begin++;
            }

            threads.Dispose();
            this = default;
        }
        
        public static VirtualMachine* Alloc(int capacity = DefaultThreadsCapacity)
        {
            var vm = UnsafeUtilities.Alloc<VirtualMachine>();
            *vm = new VirtualMachine(capacity);
            return vm;
        }
    }
}