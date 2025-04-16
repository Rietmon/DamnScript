using System;
using System.Runtime.CompilerServices;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.VirtualMachines.OpCodes;
using DamnScript.Runtimes.VirtualMachines.Scripts;
using DamnScript.Runtimes.VirtualMachines.ScriptValues;

namespace DamnScript.Runtimes.VirtualMachines.Assemblers
{
    public unsafe struct ScriptAssembler : IDisposable
    {
        public const int DefaultSize = 1024;
    
        public byte* byteCode;
        public int size;
        public int offset;

        public int currentHash;
    
        public ScriptAssembler(int capacity)
        {
            byteCode = (byte*)UnsafeUtilities.Alloc(capacity <= 0 ? DefaultSize : capacity);
            size = DefaultSize;
            offset = 0;
            
            currentHash = 0;
        }
    
        public ScriptAssembler PushToStack(ScriptValue value) =>
            Add(new PushToStack(value.longValue));
    
        public ScriptAssembler NativeCall(int methodIndex, int argumentsCount) =>
            Add(new NativeCall(methodIndex, argumentsCount));
    
        public ScriptAssembler ExpressionCall(ExpressionCall.ExpressionCallType type) =>
            Add(new ExpressionCall(type));

        public ScriptAssembler SetSavePoint()
        {
            Add(new SetSavePoint(currentHash));
            currentHash = 0;
            return this;
        }
    
        public ScriptAssembler JumpNotEquals(int jumpOffset) =>
            Add(new JumpNotEquals(jumpOffset));
    
        public ScriptAssembler JumpIfEquals(int jumpOffset) =>
            Add(new JumpEquals(jumpOffset));
    
        public ScriptAssembler Jump(int jumpOffset) =>
            Add(new Jump(jumpOffset));
    
        public ScriptAssembler PushStringToStack(int index) =>
            Add(new PushStringToStack(index));
        
        public ScriptAssembler StoreToRegister(int register) =>
            Add(new StoreToRegister(register));
        
        public ScriptAssembler LoadFromRegister(int register) =>
            Add(new LoadFromRegister(register));
        
        public ScriptAssembler DuplicateStack() =>
            Add(new DuplicateStack(0));

        public ScriptAssembler Add<T>(T value) where T : unmanaged, IOpCode
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

            var opcodeHash = value.CalculateHash();
            
            if (currentHash == 0)
                currentHash = opcodeHash;
            else
                currentHash *= opcodeHash;
            
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