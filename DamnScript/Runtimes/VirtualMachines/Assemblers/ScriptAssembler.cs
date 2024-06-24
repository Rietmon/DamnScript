using System.Runtime.InteropServices;
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
    }
    
    public ScriptAssembler PushToStack(ScriptValue value)
    {
        var pushToStack = new PushToStack(value.longValue);
        Add(pushToStack);
        return this;
    }
    
    public ScriptAssembler NativeCall(string name, int argumentsCount)
    {
        var nativeCall = new NativeCall(name, argumentsCount);
        Add(nativeCall);
        return this;
    }
    
    public ScriptAssembler ExpressionCall(ExpressionCall.ExpressionCallType type)
    {
        var expressionCall = new ExpressionCall(type);
        Add(expressionCall);
        return this;
    }
    
    public ScriptAssembler SetSavePoint()
    {
        var setSavePoint = new SetSavePoint();
        Add(setSavePoint);
        return this;
    }
    
    public ScriptAssembler JumpNotEquals(int jumpOffset)
    {
        var jumpNotEquals = new JumpNotEquals(jumpOffset);
        Add(jumpNotEquals);
        return this;
    }
    
    public ScriptAssembler SetThreadParameters(SetThreadParameters.ThreadParameters parameters)
    {
        var setThreadParameters = new SetThreadParameters(parameters);
        Add(setThreadParameters);
        return this;
    }
    
    public void Add<T>(T value) where T : unmanaged
    {
        var ptr = byteCode + offset;
        *(T*)ptr = value;
        offset += sizeof(T);
    }
    
    public ByteCodeData Finish() => new(byteCode, offset);
}