using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DamnScript.Debugs;
using DamnScript.Runtimes.Metadatas;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.OpCodes;

namespace DamnScript.Runtimes.VirtualMachines.Assemblers;

public unsafe struct ScriptAssembler
{
    public readonly byte* byteCode;
    public int offset;
    
    public ScriptAssembler(int length)
    {
        byteCode = (byte*)Marshal.AllocHGlobal(length).ToPointer();
        Unsafe.InitBlock(byteCode, 0, (uint)length);
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
    
    public ScriptAssembler Add<T>(T value) where T : unmanaged
    {
        if (value is PushToStack a)
            Console.WriteLine($"{typeof(T).Name} {a.value}");
        if (value is ExpressionCall b)
            Console.WriteLine($"{typeof(T).Name} {b.type}");
        if (value is NativeCall c)
            Console.WriteLine($"{typeof(T).Name} {new string(c.name)}");
        var ptr = byteCode + offset;
        *(T*)ptr = value;
        offset += sizeof(T);
        return this;
    }
    
    public ByteCodeData Finish() => new(byteCode, offset);
}