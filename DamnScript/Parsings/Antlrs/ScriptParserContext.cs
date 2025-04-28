using System;
using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Collections;
using DamnScript.Runtimes.Cores.Strings;
using DamnScript.Runtimes.VirtualMachines.Assemblers;
using DamnScript.Runtimes.VirtualMachines.Threads;

namespace DamnScript.Parsings.Antlrs
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ScriptParserContext
	{
		public const int RegistersCount = VirtualMachineThreadRegisters.RegistersCount;

		public String32* RegistersNamesPtr => (String32*)UnsafeUtilities.AsPointer(ref registersNames);

		public String32 name;

		public NativeList<NativeStringPtr>* strings;
		public NativeList<NativeStringPtr>* methods;
		public ScriptAssembler* assembler;

		public RegistersNamesBuffer registersNames;

		public bool isObjectCall;
		
		public bool isError;

		public int ReserveIdentifier(String32 identifier)
		{
			var begin = RegistersNamesPtr;
			var end = begin + RegistersCount;
			while (begin < end)
			{
				if (*begin == default)
				{
					*begin = identifier;
					return (int)(begin - RegistersNamesPtr);
				}

				begin++;
			}

			throw new Exception("Out of 4 registers");
		}

		public int GetRegisterIndex(String32 identifier)
		{
			var begin = RegistersNamesPtr;
			var end = begin + RegistersCount;
			while (begin < end)
			{
				if (*begin == identifier)
					return (int)(begin - RegistersNamesPtr);
				begin++;
			}

			throw new Exception($"Not found register with identifier {identifier.ToString()}");
		}

		public void FreeRegister(int index)
		{
			if (index is < 0 or >= RegistersCount)
				throw new IndexOutOfRangeException();

			*(RegistersNamesPtr + index) = default;
		}

		[StructLayout(LayoutKind.Sequential, Size = String32.Size * RegistersCount)]
		public struct RegistersNamesBuffer { }
	}
}