using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores.Types;
using DamnScript.Runtimes.VirtualMachines.Threads;

namespace DamnScript.Runtimes.Serializations
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct VirtualMachineSerializedThreadPtr
    {
        public VirtualMachineSerializedThread* value;
        public ref VirtualMachineSerializedThread RefValue => ref *value;
    
        public VirtualMachineSerializedThreadPtr(VirtualMachineSerializedThread* value) => this.value = value;

        public static implicit operator VirtualMachineSerializedThreadPtr(VirtualMachineSerializedThread* value) => new(value);
    
        public static implicit operator VirtualMachineSerializedThread*(VirtualMachineSerializedThreadPtr ptr) => ptr.value;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct VirtualMachineSerializedThread
    {
        public String32 scriptName;
        public String32 regionName;
        
        public int savePoint;
        
        public VirtualMachineThreadStack stack;
        public VirtualMachineRegisters registers;
    }
}