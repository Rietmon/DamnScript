using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores.Types;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NativeCall
    {
        public const int OpCode = 0x1;
        public readonly int opCode;
        public String32 name;
        public readonly int argumentsCount;

        public NativeCall(String32 name, int argumentsCount)
        {
            opCode = OpCode;
            this.name = name;
            this.argumentsCount = argumentsCount;
        }
    }
}
