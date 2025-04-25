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

        public float FloatValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => type switch
            {
                ValueType.Integer => rawLong,
                ValueType.Float32 => rawFloat,
                ValueType.Float64 => (float)rawDouble,
                _ => throw new NotSupportedException("ScriptValue is not a number type!")
            };
        }

        public double DoubleValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => type switch
            {
                ValueType.Integer => rawLong,
                ValueType.Float32 => rawFloat,
                ValueType.Float64 => rawDouble,
                _ => throw new NotSupportedException("ScriptValue is not a number type!")
            };
        }

        public long IntegerValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => type switch
            {
                ValueType.Integer => rawLong,
                ValueType.Float32 => (long)rawFloat,
                ValueType.Float64 => (long)rawDouble,
                _ => throw new NotSupportedException("ScriptValue is not a number type!")
            };
        }
        
        [FieldOffset(0)] public ValueType type;
        [FieldOffset(TypeSize)] public bool rawBool;
        [FieldOffset(TypeSize)] public byte rawByte;
        [FieldOffset(TypeSize)] public sbyte rawSByte;
        [FieldOffset(TypeSize)] public short rawShort;
        [FieldOffset(TypeSize)] public ushort rawUShort;
        [FieldOffset(TypeSize)] public int rawInt;
        [FieldOffset(TypeSize)] public uint rawUInt;
        [FieldOffset(TypeSize)] public long rawLong;
        [FieldOffset(TypeSize)] public ulong rawULong;
        [FieldOffset(TypeSize)] public float rawFloat;
        [FieldOffset(TypeSize)] public double rawDouble;
        [FieldOffset(TypeSize)] public char rawChar;
        [FieldOffset(TypeSize)] public void* rawPointerValue;
        [FieldOffset(TypeSize)] public PinHandle rawSafeValue;
        [FieldOffset(TypeSize)] public NativeStringPtr rawNativeStringPointerValue;

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
        public void UnpinSafePointer()
        {
            if (type is not (ValueType.ReferenceSafePointer or ValueType.ReferencePersistentSafePointer))
                throw new NotSupportedException("For UnpinSafePointer use only " +
                                                $"{nameof(ValueType.ReferencePersistentSafePointer)}!" +
                                                $"{nameof(ValueType.ReferenceSafePointer)}!");
            
            rawSafeValue.Free();
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
            
            UnsafeUtilities.Free(rawPointerValue);
            type = ValueType.FreedPointer;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ScriptValue other) => Equal(this, other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) => obj is ScriptValue other && Equals(other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => HashCode.Combine((int)type, rawLong);
    }
}