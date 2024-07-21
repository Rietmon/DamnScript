using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SetSavePoint
    {
        public const int OpCode = 0x4;
        public readonly int opCode;
        
#if !DAMN_SCRIPT_UNITY
        public SetSavePoint() => throw new Exception("Don't use default constructor.");
#endif
    
        public SetSavePoint(int _) 
        { 
            opCode = OpCode;
        }
    }
}