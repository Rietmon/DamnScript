using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct SetSavePoint
    {
        public const OpCodes OpCode = OpCodes.SetSavePoint;
        public static readonly int size = sizeof(SetSavePoint);
        
        public readonly OpCodes opCode;
    
        public SetSavePoint(int _) 
        { 
            opCode = OpCode;
        }
    }
}