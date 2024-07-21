using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SetSavePoint
    {
        public const int OpCode = 0x4;
        public readonly int opCode;
    
        public SetSavePoint(int _) 
        { 
            opCode = OpCode;
        }
    }
}