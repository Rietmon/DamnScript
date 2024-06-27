using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct NativeCall
{
    public const int OpCode = 0x1;
    public readonly int opCode = OpCode;
    public fixed char name[32];
    public readonly int argumentsCount;

    public NativeCall(string name, int argumentsCount)
    {
        for (var i = 0; i < name.Length; i++)
            this.name[i] = name[i];
        this.argumentsCount = argumentsCount;
    }
}
