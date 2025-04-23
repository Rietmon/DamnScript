using System;
using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Collections;
using DamnScript.Runtimes.Cores.Strings;
using DamnScript.Runtimes.VirtualMachines.Assemblers;

namespace DamnScript.Parsings.Antlrs
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ScriptParserContext
	{
		public const int RegistersCount = 4;

		public String32* RegistersPtr
		{
			get
			{
				fixed (String32* ptr = &registerId0)
					return ptr;
			}
		}

		public String32 name;

		public NativeList<NativeStringPtr>* strings;
		public NativeList<NativeStringPtr>* methods;
		public ScriptAssembler* assembler;

		public String32 registerId0;
		public String32 registerId1;
		public String32 registerId2;
		public String32 registerId3;

		public bool isError;

		public int ReserveIdentifier(String32 identifier)
		{
			var begin = RegistersPtr;
			var end = begin + RegistersCount;
			while (begin < end)
			{
				if (*begin == default)
				{
					*begin = identifier;
					return (int)(begin - RegistersPtr);
				}

				begin++;
			}

			throw new Exception("Out of 4 registers");
		}

		public int GetRegisterIndex(String32 identifier)
		{
			var begin = RegistersPtr;
			var end = begin + RegistersCount;
			while (begin < end)
			{
				if (*begin == identifier)
					return (int)(begin - RegistersPtr);
				begin++;
			}

			throw new Exception($"Not found register with identifier {identifier.ToString()}");
		}

		public void FreeRegister(int index)
		{
			if (index is < 0 or >= RegistersCount)
				throw new IndexOutOfRangeException();

			fixed (String32* ptr = &registerId0)
				*(ptr + index) = default;
		}
	}
}