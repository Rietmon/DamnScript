using System;
using DamnScript.Runtimes.Natives;

namespace DamnScript.Runtimes.VirtualMachines.Threads
{
    public unsafe struct VirtualMachineThreadStack
    {
        private static readonly int sizeOfScriptValue = sizeof(ScriptValue);
    
#if DAMN_SCRIPT_STACK_SIZE_16
        private const int StackSize = 16 * ScriptValue.Size;
#elif DAMN_SCRIPT_STACK_SIZE_64
        private const int StackSize = 64 * ScriptValue.Size;
#else
        private const int StackSize = 32 * ScriptValue.Size;
#endif
        
        private fixed byte _stack[StackSize];
        private int _stackOffset;
    
        public void Push(ScriptValue value)
        {
            if (_stackOffset + sizeOfScriptValue > StackSize)
                throw new InvalidOperationException("VirtualMachineThreadStack overflow!");

            fixed (byte* pStack = _stack)
            {
                *(ScriptValue*)(pStack + _stackOffset) = value;
                _stackOffset += sizeOfScriptValue;
            }
        }
    
        public ScriptValue Pop()
        {
            if (_stackOffset - sizeOfScriptValue < 0)
                throw new InvalidOperationException("VirtualMachineThreadStack underflow!");
        
            fixed (byte* pStack = _stack)
            {
                _stackOffset -= sizeOfScriptValue;
                return *(ScriptValue*)(pStack + _stackOffset);
            }
        }
    }
}