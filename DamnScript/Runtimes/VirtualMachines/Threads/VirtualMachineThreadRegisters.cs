using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores;

namespace DamnScript.Runtimes.VirtualMachines.Threads
{
	public unsafe struct VirtualMachineThreadRegisters
	{
		public const int RegistersCount = 4;

		public long* Ptr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (long*)UnsafeUtilities.AsPointer(ref registers);
		}

		public long this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
#if DAMN_SCRIPT_ENABLE_ADDITIONAL_CHECKS
				if (index is < 0 or >= RegistersCount)
					throw new IndexOutOfRangeException();
#endif

				return *(Ptr + index);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
#if DAMN_SCRIPT_ENABLE_ADDITIONAL_CHECKS
				if (index is < 0 or >= RegistersCount)
					throw new IndexOutOfRangeException();
#endif

				*(Ptr + index) = value;
			}
		}

		public RegistersBuffer registers;

		[StructLayout(LayoutKind.Sequential, Size = RegistersCount * sizeof(long))]
		public struct RegistersBuffer
		{
		}
	}
}