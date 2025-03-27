using System.Runtime.CompilerServices;
using DamnScript.Runtimes.Cores.Pins;
using DamnScript.Runtimes.Cores.Types;

namespace DamnScript.Runtimes.Natives
{
	public unsafe partial struct ScriptValue
	{
		#region Constructors
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(ValueType type, long value) : this()
        {
            this.type = type;
            longValue = value;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(bool value) : this() => (type, boolValue) = (ValueType.NumberInteger, value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(byte value) : this() => (type, ulongValue) = (ValueType.NumberInteger, value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(sbyte value) : this() => (type, longValue) = (ValueType.NumberInteger, value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(short value) : this() => (type, longValue) = (ValueType.NumberInteger, value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(ushort value) : this() => (type, ulongValue) = (ValueType.NumberInteger, value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(int value) : this() => (type, longValue) = (ValueType.NumberInteger, value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(uint value) : this() => (type, ulongValue) = (ValueType.NumberInteger, value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(long value) : this() => (type, longValue) = (ValueType.NumberInteger, value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(ulong value) : this() => (type, ulongValue) = (ValueType.NumberInteger, value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(float value) : this() => (type, floatValue) = (ValueType.NumberFloat32, value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(double value) : this() => (type, doubleValue) = (ValueType.NumberFloat64, value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(char value) : this() => (type, charValue) = (ValueType.NumberInteger, value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(void* value, ValueType type) : this()
        {
            this.type = type;
            pointerValue = value;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(ObjectPin value) : this() => (type, safeValue) = (ValueType.ReferenceSafePointer, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(NativeString* value) : this()
        {
            nativeStringValue = value;
            type = ValueType.NativeStringPointer;
        }

        #endregion
        
        #region Implicit operators
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(bool value) => new(value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(byte value) => new(value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(sbyte value) => new(value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(short value) => new(value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(ushort value) => new(value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(int value) => new(value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(uint value) => new(value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(long value) => new(value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(ulong value) => new(value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(float value) => new(value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(double value) => new(value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(char value) => new(value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(void* value) => new(value, ValueType.Pointer);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(NativeString* value) => new(value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(ObjectPin value) => new(value);
        
        #endregion
	}
}