using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SetSavePoint
    {
        public const OpCodes OpCode = OpCodes.SetSavePoint;
        public readonly OpCodes opCode;
    
        public SetSavePoint(int _) 
        { 
            opCode = OpCode;
        }
    }
}