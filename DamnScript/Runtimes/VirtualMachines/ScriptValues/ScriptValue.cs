using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Pins;
using DamnScript.Runtimes.Cores.Strings;
using DamnScript.Runtimes.VirtualMachines.Threads;

namespace DamnScript.Runtimes.VirtualMachines.ScriptValues
{
    /// <summary>
    /// This struct is a wrapper to handle any type of value in the DamnScript.
    /// It has a fixed size and can be used in the virtual machine.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = Size)]
    public unsafe partial struct ScriptValue : IEquatable<ScriptValue>
    {
        public const int TypeSize = UnsafeUtilities.PointerSize;
        public const int Size = TypeSize + UnsafeUtilities.PointerSize;
        
        public static ScriptValue* ReturnValuePtr { get; set; }

        public bool IsRefOrPtr
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => type is not ValueType.Invalid and not ValueType.Integer and not ValueType.Float32 and not ValueType.Float64;
        }

        public float SafeFloatValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => type switch
            {
                ValueType.Integer => longValue,
                ValueType.Float32 => floatValue,
                ValueType.Float64 => (float)doubleValue,
                _ => throw new NotSupportedException("ScriptValue is not a number type!")
            };
        }

        public double SafeDoubleValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => type switch
            {
                ValueType.Integer => longValue,
                ValueType.Float32 => floatValue,
                ValueType.Float64 => doubleValue,
                _ => throw new NotSupportedException("ScriptValue is not a number type!")
            };
        }

        public long SafeIntegerValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => type switch
            {
                ValueType.Integer => longValue,
                ValueType.Float32 => (long)floatValue,
                ValueType.Float64 => (long)doubleValue,
                _ => throw new NotSupportedException("ScriptValue is not a number type!")
            };
        }
        
        [FieldOffset(0)] public ValueType type;
        [FieldOffset(TypeSize)] public bool boolValue;
        [FieldOffset(TypeSize)] public byte byteValue;
        [FieldOffset(TypeSize)] public sbyte sbyteValue;
        [FieldOffset(TypeSize)] public short shortValue;
        [FieldOffset(TypeSize)] public ushort ushortValue;
        [FieldOffset(TypeSize)] public int intValue;
        [FieldOffset(TypeSize)] public uint uintValue;
        [FieldOffset(TypeSize)] public long longValue;
        [FieldOffset(TypeSize)] public ulong ulongValue;
        [FieldOffset(TypeSize)] public float floatValue;
        [FieldOffset(TypeSize)] public double doubleValue;
        [FieldOffset(TypeSize)] public char charValue;
        [FieldOffset(TypeSize)] public void* pointerValue;
        [FieldOffset(TypeSize)] public PinHandle safeValue;
        [FieldOffset(TypeSize)] public NativeStringPtr nativeStringPtrValue;

        /// <summary>
        /// Convert value to an internal constant pointer and use it as a return value.
        /// </summary>
        /// <returns>Pointer to returned value (use only in VM thread call)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValuePtr Return()
        {
#if DAMN_SCRIPT_ENABLE_ADDITIONAL_CHECKS
            if (ScriptEngine.CurrentThreadPtr.value == null || ReturnValuePtr != &ScriptEngine.CurrentThreadPtr.value->returnValue)
                throw new Exception("For async calls please use ReturnAsync method!");
            
            if (ReturnValuePtr == null)
                throw new Exception("Call return only from VM thread call!");
#endif
            
            *ReturnValuePtr = this;
            return new ScriptValuePtr(ReturnValuePtr);
        }

        /// <summary>
        /// Convert value to an internal constant pointer and use it as a return value.
        /// </summary>
        /// <returns>Pointer to returned value (use only in VM thread call)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValuePtr ReturnAsync(VirtualMachineThreadHandle handle)
        {
#if DAMN_SCRIPT_ENABLE_ADDITIONAL_CHECKS
            if (handle.threadId == -1)
                throw new Exception("Please cache your VM thread handle on method start!");
#endif

            var toReturn = &handle.Ptr.value->returnValue;
            *toReturn = this;
            return new ScriptValuePtr(toReturn);
        }
        
        /// <summary>
        /// Unpin safe pointer if a value type is it.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnpinManagedPointer()
        {
            if (type != ValueType.ReferenceSafePointer)
                throw new NotSupportedException("For UnpinManagedPointer use only " +
                                                $"{nameof(ValueType.ReferenceSafePointer)}!");
            
            safeValue.Free();
            type = ValueType.ReferenceUnpinnedSafePointer;
        }
        
        /// <summary>
        /// Free unmanaged pointer if a value type is it.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FreePointer()
        {
            if (type != ValueType.Pointer)
                throw new NotSupportedException("For FreeUnmanagedPointer use only " +
                                                $"{nameof(ValueType.Pointer)}!");
            
            UnsafeUtilities.Free(pointerValue);
            type = ValueType.FreedPointer;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ScriptValue other) => Equal(this, other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) => obj is ScriptValue other && Equals(other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => HashCode.Combine((int)type, longValue);
    }
}