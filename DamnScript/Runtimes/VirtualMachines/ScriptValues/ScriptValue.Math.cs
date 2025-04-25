using System;
using System.Runtime.CompilerServices;

namespace DamnScript.Runtimes.VirtualMachines.ScriptValues
{
	public partial struct ScriptValue
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(ScriptValue left, ScriptValue right) => Equal(left, right);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(ScriptValue left, ScriptValue right) => !Equal(left, right);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >(ScriptValue left, ScriptValue right) => Greater(left, right);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <(ScriptValue left, ScriptValue right) => Less(left, right);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >=(ScriptValue left, ScriptValue right) => GreaterOrEqual(left, right);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <=(ScriptValue left, ScriptValue right) => LessOrEqual(left, right);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValue operator +(ScriptValue left, ScriptValue right) => Add(left, right);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValue operator -(ScriptValue left, ScriptValue right) => Subtract(left, right);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValue operator *(ScriptValue left, ScriptValue right) => Multiply(left, right);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValue operator /(ScriptValue left, ScriptValue right) => Divide(left, right);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValue operator %(ScriptValue left, ScriptValue right) => Modulo(left, right);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValue operator -(ScriptValue value) => Negate(value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValue operator +(ScriptValue value) => Positive(value);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValue operator &(ScriptValue left, ScriptValue right) => And(left, right);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValue operator |(ScriptValue left, ScriptValue right) => Or(left, right);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValue operator ^(ScriptValue left, ScriptValue right) => Xor(left, right);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValue operator ~(ScriptValue value) => BitwiseNot(value);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValue operator <<(ScriptValue left, int right) => BitwiseLeftShift(left, right);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValue operator >> (ScriptValue left, int right) => BitwiseRightShift(left, right);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValue operator ++(ScriptValue value) => Increment(value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValue operator --(ScriptValue value) => Decrement(value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool Equal(ScriptValue l, ScriptValue r)
		{
			if (l.type != r.type)
				return l.DoubleValue == r.DoubleValue;

			return l.type switch
			{
				ValueType.Float32 => l.rawFloat == r.rawFloat,
				ValueType.Float64 => l.rawDouble == r.rawDouble,
				_ => l.rawLong == r.rawLong
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool Greater(ScriptValue l, ScriptValue r)
		{
			if (l.type != r.type)
				return l.DoubleValue > r.DoubleValue;

			return l.type switch
			{
				ValueType.Float32 => l.rawFloat > r.rawFloat,
				ValueType.Float64 => l.rawDouble > r.rawDouble,
				_ => l.rawLong > r.rawLong
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool Less(ScriptValue l, ScriptValue r)
		{
			if (l.type != r.type)
				return l.DoubleValue < r.DoubleValue;

			return l.type switch
			{
				ValueType.Float32 => l.rawFloat < r.rawFloat,
				ValueType.Float64 => l.rawDouble < r.rawDouble,
				_ => l.rawLong < r.rawLong
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool GreaterOrEqual(ScriptValue l, ScriptValue r)
		{
			if (l.type != r.type)
				return l.DoubleValue >= r.DoubleValue;

			return l.type switch
			{
				ValueType.Float32 => l.rawFloat >= r.rawFloat,
				ValueType.Float64 => l.rawDouble >= r.rawDouble,
				_ => l.rawLong >= r.rawLong
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool LessOrEqual(ScriptValue l, ScriptValue r)
		{
			if (l.type != r.type)
				return l.DoubleValue <= r.DoubleValue;

			return l.type switch
			{
				ValueType.Float32 => l.rawFloat <= r.rawFloat,
				ValueType.Float64 => l.rawDouble <= r.rawDouble,
				_ => l.rawLong <= r.rawLong
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue Add(ScriptValue l, ScriptValue r)
		{
			if (l.type != r.type)
				return new ScriptValue(l.DoubleValue + r.DoubleValue);

			AssertIfBothAreRefOrPtr(l, r);

			return l.type switch
			{
				ValueType.Float32 => new ScriptValue(l.rawFloat + r.rawFloat),
				ValueType.Float64 => new ScriptValue(l.rawDouble + r.rawDouble),
				_ => new ScriptValue(l.rawLong + r.rawLong)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue Subtract(ScriptValue l, ScriptValue r)
		{
			if (l.type != r.type)
				return new ScriptValue(l.DoubleValue - r.DoubleValue);

			AssertIfBothAreRefOrPtr(l, r);

			return l.type switch
			{
				ValueType.Float32 => new ScriptValue(l.rawFloat - r.rawFloat),
				ValueType.Float64 => new ScriptValue(l.rawDouble - r.rawDouble),
				_ => new ScriptValue(l.rawLong - r.rawLong)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue Multiply(ScriptValue l, ScriptValue r)
		{
			if (l.type != r.type)
				return new ScriptValue(l.DoubleValue * r.DoubleValue);

			AssertIfBothAreRefOrPtr(l, r);

			return l.type switch
			{
				ValueType.Float32 => new ScriptValue(l.rawFloat * r.rawFloat),
				ValueType.Float64 => new ScriptValue(l.rawDouble * r.rawDouble),
				_ => new ScriptValue(l.rawLong * r.rawLong)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue Divide(ScriptValue l, ScriptValue r)
		{
			if (l.type != r.type)
				return new ScriptValue(l.DoubleValue / r.DoubleValue);

			AssertIfBothAreRefOrPtr(l, r);

			return l.type switch
			{
				ValueType.Float32 => new ScriptValue(l.rawFloat / r.rawFloat),
				ValueType.Float64 => new ScriptValue(l.rawDouble / r.rawDouble),
				_ => new ScriptValue(l.rawLong / r.rawLong)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue Modulo(ScriptValue l, ScriptValue r)
		{
			if (l.type != r.type)
				return new ScriptValue(l.DoubleValue % r.DoubleValue);

			AssertIfBothAreRefOrPtr(l, r);

			return l.type switch
			{
				ValueType.Float32 => new ScriptValue(l.rawFloat % r.rawFloat),
				ValueType.Float64 => new ScriptValue(l.rawDouble % r.rawDouble),
				_ => new ScriptValue(l.rawLong % r.rawLong)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue Negate(ScriptValue l)
		{
			AssertIfIsRefOrPtr(l);

			return l.type switch
			{
				ValueType.Float32 => new ScriptValue(-l.rawFloat),
				ValueType.Float64 => new ScriptValue(-l.rawDouble),
				_ => new ScriptValue(-l.rawLong)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue Positive(ScriptValue l)
		{
			AssertIfIsRefOrPtr(l);

			return l.type switch
			{
				ValueType.Float32 => new ScriptValue(+l.rawFloat),
				ValueType.Float64 => new ScriptValue(+l.rawDouble),
				_ => new ScriptValue(+l.rawLong)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue And(ScriptValue l, ScriptValue r)
		{
			return l.type switch
			{
				ValueType.Integer => new ScriptValue(l.rawLong & r.rawLong),
				_ => throw new Exception($"Cannot use AND operator on {l.type} and {r.type}.")
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue Or(ScriptValue l, ScriptValue r)
		{
			return l.type switch
			{
				ValueType.Integer => new ScriptValue(l.rawLong | r.rawLong),
				_ => throw new Exception($"Cannot use OR operator on {l.type} and {r.type}.")
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue Xor(ScriptValue l, ScriptValue r)
		{
			return l.type switch
			{
				ValueType.Integer => new ScriptValue(l.rawLong ^ r.rawLong),
				_ => throw new Exception($"Cannot use XOR operator on {l.type} and {r.type}.")
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue BitwiseLeftShift(ScriptValue l, ScriptValue r)
		{
			return l.type switch
			{
				ValueType.Integer => new ScriptValue(l.rawLong << (int)r.rawLong),
				_ => throw new Exception($"Cannot use Bitwise Left Shift operator on {l.type} and {r.type}.")
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue BitwiseRightShift(ScriptValue l, ScriptValue r)
		{
			return l.type switch
			{
				ValueType.Integer => new ScriptValue(l.rawLong >> (int)r.rawLong),
				_ => throw new Exception($"Cannot use Bitwise Right Shift operator on {l.type} and {r.type}.")
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue BitwiseNot(ScriptValue l)
		{
			return l.type switch
			{
				ValueType.Integer => new ScriptValue(~l.rawLong),
				_ => throw new Exception($"Cannot use Bitwise Not operator on {l.type}.")
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue Increment(ScriptValue l)
		{
			return l.type switch
			{
				ValueType.Integer => new ScriptValue(l.rawLong + 1),
				ValueType.Float32 => new ScriptValue(l.rawFloat + 1),
				ValueType.Float64 => new ScriptValue(l.rawDouble + 1),
				_ => throw new Exception($"Cannot use Increment operator on {l.type}.")
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue Decrement(ScriptValue l)
		{
			return l.type switch
			{
				ValueType.Integer => new ScriptValue(l.rawLong - 1),
				ValueType.Float32 => new ScriptValue(l.rawFloat - 1),
				ValueType.Float64 => new ScriptValue(l.rawDouble - 1),
				_ => throw new Exception($"Cannot use Decrement operator on {l.type}.")
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void AssertIfBothAreRefOrPtr(ScriptValue l, ScriptValue r)
		{
			if (!l.IsRefOrPtr || !r.IsRefOrPtr) return;

			throw new Exception("You are about to use some math between two pointers/refs. This will cause an undefined behavior.");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void AssertIfIsRefOrPtr(ScriptValue l)
		{
			if (!l.IsRefOrPtr) return;

			throw new Exception("You are about to use some math on a pointer/ref. This will cause an undefined behavior.");
		}
	}
}