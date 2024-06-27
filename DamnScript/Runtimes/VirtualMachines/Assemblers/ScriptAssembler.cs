using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Metadatas;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.OpCodes;

namespace DamnScript.Runtimes.VirtualMachines.Assemblers;

public unsafe struct ScriptAssembler : IDisposable
{
    public const int DefaultSize = 1024;
    
    public byte* byteCode;
    public int size;
    public int offset;

    public NativeList<UnsafeStringPair> constantStrings;
    
    public ScriptAssembler()
    {
        byteCode = (byte*)UnsafeUtilities.Alloc(DefaultSize);
        Unsafe.InitBlock(byteCode, 0, DefaultSize);
        size = DefaultSize;
        constantStrings = new NativeList<UnsafeStringPair>(16);
    }
    
    public ScriptAssembler PushToStack(ScriptValue value) =>
        Add(new PushToStack(value.longValue));
    
    public ScriptAssembler NativeCall(string name, int argumentsCount) =>
        Add(new NativeCall(name, argumentsCount));
    
    public ScriptAssembler ExpressionCall(ExpressionCall.ExpressionCallType type) =>
        Add(new ExpressionCall(type));
    
    public ScriptAssembler SetSavePoint() =>
        Add(new SetSavePoint());
    
    public ScriptAssembler JumpNotEquals(int jumpOffset) =>
        Add(new JumpNotEquals(jumpOffset));
    
    public ScriptAssembler SetThreadParameters(SetThreadParameters.ThreadParameters parameters) =>
        Add(new SetThreadParameters(parameters));
    
    public ScriptAssembler PushStringToStack(int hash) =>
        Add(new PushStringToStack(hash));
    
    public ScriptAssembler Add<T>(T value) where T : unmanaged
    {
        var length = sizeof(T);
        if (offset + length > size)
        {
            size *= 2;
            var newByteCode = (byte*)UnsafeUtilities.Realloc(byteCode, size);
            if (newByteCode == null)
                throw new OutOfMemoryException("Failed to reallocate memory for ScriptAssembler.");
            byteCode = newByteCode;
        }
        
#if DAMN_SCRIPT_ENABLE_ASSEMBLER_DEBUG
        Debugging.Log($"Add {typeof(T).Name} at {offset} with value {value}.");
#endif
        
        var ptr = byteCode + offset;
        *(T*)ptr = value;
        offset += length;
        return this;
    }
    
    public ByteCodeData FinishAlloc()
    {
        var code = UnsafeUtilities.Alloc(offset);
        Unsafe.CopyBlock(code, byteCode, (uint)offset);
        
        return new ByteCodeData((byte*)code, offset);
    }
    
    public void Dispose()
    {
        UnsafeUtilities.Free(byteCode);
        constantStrings.Dispose();
        this = default;
    }
}