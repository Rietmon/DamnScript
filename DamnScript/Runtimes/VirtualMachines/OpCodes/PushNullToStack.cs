using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
#if DAMN_SCRIPT_DISABLE_ALIGNMENT_OPCODES
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
#else 
	[StructLayout(LayoutKind.Sequential)]
#endif
	public readonly unsafe struct PushNullToStack : IOpCode
	{
		public const OpCodeType OpCode = OpCodeType.PushNullToStack;
		public static readonly int size = sizeof(PushNullToStack);
        
		public readonly OpCodeType opCode;

		public PushNullToStack(int _)
		{
			opCode = OpCode;
		}

		public int CalculateHash()
		{
			return HashCode.Combine(opCode);
		}

		public string GetAssemblerDebugInfo() => string.Empty;
	}
}