using System.Runtime.CompilerServices;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Pins;
using DamnScript.Runtimes.Cores.Strings;
using DamnScript.Runtimes.VirtualMachines.Threads;

namespace DamnScript.Runtimes.VirtualMachines.ScriptValues
{
	/// <summary>
	/// Using as an argument for the native methods be passed by reference.
	/// It is a pointer to the ScriptValue.
	/// </summary>
	public readonly unsafe struct ScriptValuePtr
	{
		public bool IsRefOrPtr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => value->IsRefOrPtr;
		}

		public ScriptValue.ValueType Type
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => value->type;
		}

		public bool BoolValue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => value->boolValue;
		}

		public byte ByteValue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => value->byteValue;
		}

		public sbyte SByteValue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => value->sbyteValue;
		}

		public short ShortValue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => value->shortValue;
		}

		public ushort UShortValue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => value->ushortValue;
		}

		public int IntValue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => value->intValue;
		}

		public uint UIntValue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => value->uintValue;
		}

		public long LongValue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => value->longValue;
		}

		public ulong ULongValue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => value->ulongValue;
		}

		public float FloatValue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => value->floatValue;
		}

		public double DoubleValue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => value->doubleValue;
		}

		public char CharValue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => value->charValue;
		}

		public void* PointerValue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => value->pointerValue;
		}

		public PinHandle SafeValue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => value->safeValue;
		}

		public ref ScriptValue RefValue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref UnsafeUtilities.AsRef<ScriptValue>(value);
		}

		public readonly ScriptValue* value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ScriptValuePtr(ScriptValue* value) => this.value = value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ScriptValuePtr(ref ScriptValue value) => this.value = UnsafeUtilities.AsPointer(ref value);

		/// <summary>
		/// <inheritdoc cref="ScriptValue.GetReferencePin{T}"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T GetReferencePin<T>() where T : class => value->GetReferencePin<T>();


#if DAMN_SCRIPT_ENABLE_UNSAFE_SCRIPT_VALUE
		/// <summary>
		/// <inheritdoc cref="ScriptValue.GetReferenceUnsafe{T}"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T GetReferenceUnsafe<T>() where T : class => value->GetReferenceUnsafe<T>();
#endif

		/// <summary>
		/// <inheritdoc cref="ScriptValue.GetReference{T}"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T GetReference<T>() where T : class => value->GetReference<T>();


#if DAMN_SCRIPT_ENABLE_UNSAFE_SCRIPT_VALUE
		/// <summary>
		/// <inheritdoc cref="ScriptValue.GetStruct{T}"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T GetStruct<T>() where T : unmanaged => value->GetStruct<T>();
#endif

		/// <summary>
		/// <inheritdoc cref="ScriptValue.GetStringWrapper"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public StringWrapper GetStringWrapper() => value->GetStringWrapper();

		/// <summary>
		/// <inheritdoc cref="ScriptValue.GetReferencePointer"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void* GetReferencePointer() => value->GetReferencePointer();

		/// <summary>
		/// <inheritdoc cref="ScriptValue.UnpinManagedPointer"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void UnpinManagedPointer() => value->UnpinManagedPointer();

		/// <summary>
		/// <inheritdoc cref="ScriptValue.ToString"/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString() => value->ToString();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ScriptValuePtr(ScriptValue* value) => new(value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator ScriptValue*(ScriptValuePtr ptr) => ptr.value;
	}

#if !DAMN_SCRIPT_DISABLE_RETURN_EXTENSIONS
	public static class ScriptValuePtrExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr Return(this bool value) => new ScriptValue(value).Return();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr Return(this byte value) => new ScriptValue(value).Return();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr Return(this sbyte value) => new ScriptValue(value).Return();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr Return(this short value) => new ScriptValue(value).Return();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr Return(this ushort value) => new ScriptValue(value).Return();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr Return(this int value) => new ScriptValue(value).Return();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr Return(this uint value) => new ScriptValue(value).Return();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr Return(this long value) => new ScriptValue(value).Return();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr Return(this ulong value) => new ScriptValue(value).Return();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr Return(this float value) => new ScriptValue(value).Return();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr Return(this double value) => new ScriptValue(value).Return();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr Return(this char value) => new ScriptValue(value).Return();
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr Return(this PinHandle value) => new ScriptValue(value).Return();
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr ReturnAsync(this bool value, VirtualMachineThreadHandle handle) =>
			new ScriptValue(value).ReturnAsync(handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr ReturnAsync(this byte value, VirtualMachineThreadHandle handle) =>
			new ScriptValue(value).ReturnAsync(handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr ReturnAsync(this sbyte value, VirtualMachineThreadHandle handle) =>
			new ScriptValue(value).ReturnAsync(handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr ReturnAsync(this short value, VirtualMachineThreadHandle handle) =>
			new ScriptValue(value).ReturnAsync(handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr ReturnAsync(this ushort value, VirtualMachineThreadHandle handle) =>
			new ScriptValue(value).ReturnAsync(handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr ReturnAsync(this int value, VirtualMachineThreadHandle handle) =>
			new ScriptValue(value).ReturnAsync(handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr ReturnAsync(this uint value, VirtualMachineThreadHandle handle) =>
			new ScriptValue(value).ReturnAsync(handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr ReturnAsync(this long value, VirtualMachineThreadHandle handle) =>
			new ScriptValue(value).ReturnAsync(handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr ReturnAsync(this ulong value, VirtualMachineThreadHandle handle) =>
			new ScriptValue(value).ReturnAsync(handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr ReturnAsync(this float value, VirtualMachineThreadHandle handle) =>
			new ScriptValue(value).ReturnAsync(handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr ReturnAsync(this double value, VirtualMachineThreadHandle handle) =>
			new ScriptValue(value).ReturnAsync(handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr ReturnAsync(this char value, VirtualMachineThreadHandle handle) =>
			new ScriptValue(value).ReturnAsync(handle);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValuePtr ReturnAsync(this PinHandle value, VirtualMachineThreadHandle handle) =>
			new ScriptValue(value).ReturnAsync(handle);
	}
#endif
}