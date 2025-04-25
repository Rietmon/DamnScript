using System;
using DamnScript.Parsings.Serializations;
using DamnScript.Runtimes.Cores.Collections;
using DamnScript.Runtimes.VirtualMachines;

namespace DamnScript.Runtimes.Serializations
{
    public static unsafe class VirtualMachineSerialization
    {
        public static SerializationStream SerializeToSerializationStream(VirtualMachinePtr vmPtr)
        {
            var vm = vmPtr.value;
            
            var stream = new SerializationStream(4096);
            
            stream.Write(VirtualMachine.Version);
            
            stream.Write(-1);
            var threadsLengthPosition = stream.length - sizeof(int);
            
            var threadsCount = 0;
            var threadsBegin = vm->threads.Begin;
            var threadsEnd = vm->threads.End;
            while (threadsBegin < threadsEnd)
            {
                if (!threadsBegin->isAlive)
                {
                    threadsBegin++;
                    continue;
                }
                
                var serializedThread = new VirtualMachineSerializedThread
                {
                    scriptName = threadsBegin->scriptData->name,
                    regionName = threadsBegin->regionData->name,
                    savePoint = threadsBegin->savePoint,
                    stack = threadsBegin->stack,
                    threadRegisters = threadsBegin->threadRegisters
                };
                stream.Write(serializedThread);
                
                threadsBegin++;
                threadsCount++;
            }
            
            var lastPosition = stream.length;
            stream.length = threadsLengthPosition;
            stream.Write(threadsCount);
            stream.length = lastPosition;

            return stream;
        }
        
        public static NativeArray<VirtualMachineSerializedThread> DeserializeFromSerializationStream(SerializationStream stream)
        {
            var version = stream.Read<int>();
            if (version != VirtualMachine.Version)
                throw new Exception("VirtualMachine version mismatch!");
            
            var threadsCount = stream.Read<int>();
            var threads = new NativeArray<VirtualMachineSerializedThread>(threadsCount);
            for (var i = 0; i < threadsCount; i++)
                threads[i] = stream.Read<VirtualMachineSerializedThread>();
            
            return threads;
        }
    }
}