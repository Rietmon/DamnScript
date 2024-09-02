using System;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Types;
using DamnScript.Runtimes.Metadatas;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.OpCodes;

namespace DamnScript.Runtimes.VirtualMachines.Assemblers
{
    public unsafe struct ScriptAssembler : IDisposable
    {
        public const int DefaultSize = 1024;
    
        public byte* byteCode;
        public int size;
        public int offset;
    
        public ScriptAssembler(int _)
        {
            byteCode = (byte*)UnsafeUtilities.Alloc(DefaultSize);
            UnsafeUtilities.Memset(byteCode, 0, DefaultSize);
            size = DefaultSize;
            offset = 0;
        }
    
        public ScriptAssembler PushToStack(ScriptValue value) =>
            Add(new PushToStack(value.longValue));
    
        public ScriptAssembler NativeCall(int methodIndex, int argumentsCount) =>
            Add(new NativeCall(methodIndex, argumentsCount));
    
        public ScriptAssembler ExpressionCall(ExpressionCall.ExpressionCallType type) =>
            Add(new ExpressionCall(type));
    
        public ScriptAssembler SetSavePoint() =>
            Add(new SetSavePoint(0));
    
        public ScriptAssembler JumpNotEquals(int jumpOffset) =>
            Add(new JumpNotEquals(jumpOffset));
    
        public ScriptAssembler JumpIfEquals(int jumpOffset) =>
            Add(new JumpIfEquals(jumpOffset));
    
        public ScriptAssembler Jump(int jumpOffset) =>
            Add(new Jump(jumpOffset));
    
        public ScriptAssembler SetThreadParameters(SetThreadParameters.ThreadParameters parameters) =>
            Add(new SetThreadParameters(parameters));
    
        public ScriptAssembler PushStringToStack(int index) =>
            Add(new PushStringToStack(index));
        
        public ScriptAssembler StoreToRegister(int register) =>
            Add(new StoreToRegister(register));
        
        public ScriptAssembler LoadFromRegister(int register) =>
            Add(new LoadFromRegister(register));
        
        public ScriptAssembler DuplicateStack() =>
            Add(new DuplicateStack(0));

        public ScriptAssembler Add<T>(T value) where T : unmanaged
        {
            var length = sizeof(T);
            if (offset + length > size)
            {
                size *= 2;
                var newByteCode = (byte*)UnsafeUtilities.ReAlloc(byteCode, size);
                byteCode = newByteCode;
            }
        
#if DAMN_SCRIPT_ENABLE_ASSEMBLER_DEBUG
            Debugging.Log($"Add {typeof(T).Name} at {offset} with value {value} (length: {length})");
#endif
        
            var ptr = byteCode + offset;
            *(T*)ptr = value;
            offset += length;
            return this;
        }
    
        public ByteCodeData FinishAlloc()
        {
            var code = UnsafeUtilities.Alloc(offset);
            UnsafeUtilities.Memcpy(byteCode, code, offset);
        
            return new ByteCodeData((byte*)code, offset);
        }
    
        public void Dispose()
        {
            UnsafeUtilities.Free(byteCode);
            this = default;
        }
    }
}