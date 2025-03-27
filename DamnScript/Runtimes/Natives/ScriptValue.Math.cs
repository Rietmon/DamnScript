using System;
using System.Runtime.CompilerServices;

namespace DamnScript.Runtimes.Natives
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
        public static ScriptValue operator >>(ScriptValue left, int right) => BitwiseRightShift(left, right);
    
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptValue operator ++(ScriptValue value) => Increment(value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptValue operator --(ScriptValue value) => Decrement(value);
		
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool Equal(ScriptValue l, ScriptValue r)
		{
			if (l.type != r.type)
				return l.SafeDoubleValue == r.SafeDoubleValue;

			return l.type switch
			{
				ValueType.NumberFloat32 => l.floatValue == r.floatValue,
				ValueType.NumberFloat64 => l.doubleValue == r.doubleValue,
				_ => l.longValue == r.longValue
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool Greater(ScriptValue l, ScriptValue r)
		{
			if (l.type != r.type)
				return l.SafeDoubleValue > r.SafeDoubleValue;

			return l.type switch
			{
				ValueType.NumberFloat32 => l.floatValue > r.floatValue,
				ValueType.NumberFloat64 => l.doubleValue > r.doubleValue,
				_ => l.longValue > r.longValue
			};
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool Less(ScriptValue l, ScriptValue r)
		{
			if (l.type != r.type)
				return l.SafeDoubleValue < r.SafeDoubleValue;

			return l.type switch
			{
				ValueType.NumberFloat32 => l.floatValue < r.floatValue,
				ValueType.NumberFloat64 => l.doubleValue < r.doubleValue,
				_ => l.longValue < r.longValue
			};
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool GreaterOrEqual(ScriptValue l, ScriptValue r)
		{
			if (l.type != r.type)
				return l.SafeDoubleValue >= r.SafeDoubleValue;

			return l.type switch
			{
				ValueType.NumberFloat32 => l.floatValue >= r.floatValue,
				ValueType.NumberFloat64 => l.doubleValue >= r.doubleValue,
				_ => l.longValue >= r.longValue
			};
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool LessOrEqual(ScriptValue l, ScriptValue r)
		{
			if (l.type != r.type)
				return l.SafeDoubleValue <= r.SafeDoubleValue;

			return l.type switch
			{
				ValueType.NumberFloat32 => l.floatValue <= r.floatValue,
				ValueType.NumberFloat64 => l.doubleValue <= r.doubleValue,
				_ => l.longValue <= r.longValue
			};
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue Add(ScriptValue l, ScriptValue r)
		{
			if (l.type != r.type)
				return new ScriptValue(l.SafeDoubleValue + r.SafeDoubleValue);
			
			return l.type switch
			{
				ValueType.NumberFloat32 => new ScriptValue(l.floatValue + r.floatValue),
				ValueType.NumberFloat64 => new ScriptValue(l.doubleValue + r.doubleValue),
				_ => new ScriptValue(l.longValue + r.longValue)
			};
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue Subtract(ScriptValue l, ScriptValue r)
		{
			if (l.type != r.type)
				return new ScriptValue(l.SafeDoubleValue - r.SafeDoubleValue);
			
			return l.type switch
			{
				ValueType.NumberFloat32 => new ScriptValue(l.floatValue - r.floatValue),
				ValueType.NumberFloat64 => new ScriptValue(l.doubleValue - r.doubleValue),
				_ => new ScriptValue(l.longValue - r.longValue)
			};
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue Multiply(ScriptValue l, ScriptValue r)
		{
			if (l.type != r.type)
				return new ScriptValue(l.SafeDoubleValue * r.SafeDoubleValue);
			
			return l.type switch
			{
				ValueType.NumberFloat32 => new ScriptValue(l.floatValue * r.floatValue),
				ValueType.NumberFloat64 => new ScriptValue(l.doubleValue * r.doubleValue),
				_ => new ScriptValue(l.longValue * r.longValue)
			};
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue Divide(ScriptValue l, ScriptValue r)
		{
			if (l.type != r.type)
				return new ScriptValue(l.SafeDoubleValue / r.SafeDoubleValue);
			
			return l.type switch
			{
				ValueType.NumberFloat32 => new ScriptValue(l.floatValue / r.floatValue),
				ValueType.NumberFloat64 => new ScriptValue(l.doubleValue / r.doubleValue),
				_ => new ScriptValue(l.longValue / r.longValue)
			};
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue Modulo(ScriptValue l, ScriptValue r)
		{
			if (l.type != r.type)
				return new ScriptValue(l.SafeDoubleValue % r.SafeDoubleValue);
			
			return l.type switch
			{
				ValueType.NumberFloat32 => new ScriptValue(l.floatValue % r.floatValue),
				ValueType.NumberFloat64 => new ScriptValue(l.doubleValue % r.doubleValue),
				_ => new ScriptValue(l.longValue % r.longValue)
			};
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue Negate(ScriptValue l)
		{
			return l.type switch
			{
				ValueType.NumberFloat32 => new ScriptValue(-l.floatValue),
				ValueType.NumberFloat64 => new ScriptValue(-l.doubleValue),
				_ => new ScriptValue(-l.longValue)
			};
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue Positive(ScriptValue l)
		{
			return l.type switch
			{
				ValueType.NumberFloat32 => new ScriptValue(+l.floatValue),
				ValueType.NumberFloat64 => new ScriptValue(+l.doubleValue),
				_ => new ScriptValue(+l.longValue)
			};
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue And(ScriptValue l, ScriptValue r)
		{
			return l.type switch
			{
				ValueType.NumberInteger => new ScriptValue(l.longValue & r.longValue),
				_ => throw new Exception($"Cannot use AND operator on {l.type} and {r.type}.")
			};
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue Or(ScriptValue l, ScriptValue r)
		{
			return l.type switch
			{
				ValueType.NumberInteger => new ScriptValue(l.longValue | r.longValue),
				_ => throw new Exception($"Cannot use OR operator on {l.type} and {r.type}.")
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue Xor(ScriptValue l, ScriptValue r)
		{
			return l.type switch
			{
				ValueType.NumberInteger => new ScriptValue(l.longValue ^ r.longValue),
				_ => throw new Exception($"Cannot use XOR operator on {l.type} and {r.type}.")
			};
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue BitwiseLeftShift(ScriptValue l, ScriptValue r)
		{
			return l.type switch
			{
				ValueType.NumberInteger => new ScriptValue(l.longValue << (int)r.longValue),
				_ => throw new Exception($"Cannot use Bitwise Left Shift operator on {l.type} and {r.type}.")
			};
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue BitwiseRightShift(ScriptValue l, ScriptValue r)
		{
			return l.type switch
			{
				ValueType.NumberInteger => new ScriptValue(l.longValue >> (int)r.longValue),
				_ => throw new Exception($"Cannot use Bitwise Right Shift operator on {l.type} and {r.type}.")
			};
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue BitwiseNot(ScriptValue l)
		{
			return l.type switch
			{
				ValueType.NumberInteger => new ScriptValue(~l.longValue),
				_ => throw new Exception($"Cannot use Bitwise Not operator on {l.type}.")
			};
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue Increment(ScriptValue l)
		{
			return l.type switch
			{
				ValueType.NumberInteger => new ScriptValue(l.longValue + 1),
				ValueType.NumberFloat32 => new ScriptValue(l.floatValue + 1),
				ValueType.NumberFloat64 => new ScriptValue(l.doubleValue + 1),
				_ => throw new Exception($"Cannot use Increment operator on {l.type}.")
			};
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ScriptValue Decrement(ScriptValue l)
		{
			return l.type switch
			{
				ValueType.NumberInteger => new ScriptValue(l.longValue - 1),
				ValueType.NumberFloat32 => new ScriptValue(l.floatValue - 1),
				ValueType.NumberFloat64 => new ScriptValue(l.doubleValue - 1),
				_ => throw new Exception($"Cannot use Decrement operator on {l.type}.")
			};
		}
	}
}