using System;
using DamnScript.Runtimes.Cores.Strings;

namespace DamnScript.Runtimes.VirtualMachines.Threads
{
	public readonly struct VirtualMachineThreadNativeCallInfo : IEquatable<VirtualMachineThreadNativeCallInfo>
	{
		public static readonly VirtualMachineThreadNativeCallInfo invalid = new(-1);
		
		public readonly int argumentsCount;

		public VirtualMachineThreadNativeCallInfo(int argumentsCount) : this()
		{
			this.argumentsCount = argumentsCount;
		}

		public bool Equals(VirtualMachineThreadNativeCallInfo other)
		{
			return argumentsCount == other.argumentsCount;
		}

		public override bool Equals(object obj)
		{
			return obj is VirtualMachineThreadNativeCallInfo other && Equals(other);
		}

		public override int GetHashCode()
		{
			return argumentsCount;
		}
		
		public static bool operator ==(VirtualMachineThreadNativeCallInfo left, VirtualMachineThreadNativeCallInfo right)
		{
			return left.argumentsCount == right.argumentsCount;
		}
		
		public static bool operator !=(VirtualMachineThreadNativeCallInfo left, VirtualMachineThreadNativeCallInfo right)
		{
			return !(left == right);
		}
	}
}