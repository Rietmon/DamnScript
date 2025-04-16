using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
#if DAMN_SCRIPT_DISABLE_ALIGNMENT_OPCODES
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
#else 
    [StructLayout(LayoutKind.Sequential)]
#endif
    public readonly unsafe struct SetSavePoint : IOpCode
    {
        public const OpCodeType OpCode = OpCodeType.SetSavePoint;
        public static readonly int size = sizeof(SetSavePoint);
        
        public readonly OpCodeType opCode;
        public readonly int hash;
    
        public SetSavePoint(int hash) 
        { 
            opCode = OpCode;
            this.hash = hash;
        }

        public int CalculateHash()
        {
            return 0;
        }
    }
}