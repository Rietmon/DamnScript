using System;
using DamnScript.Parsings.Serializations;
using DamnScript.Runtimes.Cores.Types;
using DamnScript.Runtimes.VirtualMachines;

namespace DamnScript.Runtimes.Serializations
{
    public static unsafe class VirtualMachineSerialization
    {
        public static SerializationStream SerializeToSerializationStream(VirtualMachine vm)
        {
            var stream = new SerializationStream(4096);
            
            stream.Write(VirtualMachine.Version);
            
            var threadsCount = vm.threads.Count;
            stream.Write(threadsCount);
            
            var threadsBegin = vm.threads.Begin;
            var threadsEnd = vm.threads.End;
            while (threadsBegin < threadsEnd)
            {
                var serializedThread = new VirtualMachineSerializedThread
                {
                    scriptName = *threadsBegin->scriptName,
                    regionName = threadsBegin->regionData->name,
                    savePoint = threadsBegin->savePoint,
                    stack = threadsBegin->stack,
                    registers = threadsBegin->registers
                };
                stream.Write(serializedThread);
                
                threadsBegin++;
            }

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