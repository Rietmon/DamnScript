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
    
        public void PushToStack(ScriptValue value) =>
            Add(new PushToStack(value.rawLong));
    
        public void NativeCall(int methodIndex, int argumentsCount) =>
            Add(new NativeCall(methodIndex, argumentsCount));
    
        public void ExpressionCall(ExpressionCall.ExpressionCallType type) =>
            Add(new ExpressionCall(type));

        public ScriptAssembler SetSavePoint()
        {
            Add(new SetSavePoint(currentHash));
            currentHash = 0;
            return this;
        }
    
        public void JumpNotEquals(int jumpOffset) =>
            Add(new JumpNotEquals(jumpOffset));
    
        public void JumpIfEquals(int jumpOffset) =>
            Add(new JumpEquals(jumpOffset));
    
        public void Jump(int jumpOffset) =>
            Add(new Jump(jumpOffset));
    
        public void PushStringToStack(int index) =>
            Add(new PushStringToStack(index));
        
        public void StoreToRegister(int register) =>
            Add(new StoreToRegister(register));
        
        public void LoadFromRegister(int register) =>
            Add(new LoadFromRegister(register));
        
        public void DuplicateStack() =>
            Add(new DuplicateStack(0));
        
        public void PushNullToStack() =>
            Add(new PushNullToStack(0));

        public void Add<T>(T value) where T : unmanaged, IOpCode
        {
            var length = sizeof(T);
            if (offset + length > size)
            {
                size *= 2;
                var newByteCode = (byte*)UnsafeUtilities.ReAlloc(byteCode, size);
                byteCode = newByteCode;
            }
            
#if DAMN_SCRIPT_ENABLE_ASSEMBLER_DEBUG
            var info = value.GetAssemblerDebugInfo();
            var debugInfo = info == null ? "" : $"with info: {info}";
            Debugging.Log($"Add {typeof(T).Name} at {offset} {debugInfo} (length: {length})");
#endif

            var opcodeHash = value.CalculateHash();
            
            if (currentHash == 0)
                currentHash = opcodeHash;
            else
                currentHash *= opcodeHash;
            
            var ptr = byteCode + offset;
            *(T*)ptr = value;
            offset += length;
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