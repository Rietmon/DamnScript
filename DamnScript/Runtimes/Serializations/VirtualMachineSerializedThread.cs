using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores.Types;
using DamnScript.Runtimes.VirtualMachines.Threads;

namespace DamnScript.Runtimes.Serializations
{
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