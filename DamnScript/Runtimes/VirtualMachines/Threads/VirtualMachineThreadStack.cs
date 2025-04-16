using System;
using DamnScript.Runtimes.VirtualMachines.ScriptValues;

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
        
        public fixed byte stack[StackSize];
        public int stackOffset;
    
        public void Push(ScriptValue value)
        {
            if (stackOffset + sizeOfScriptValue > StackSize)
                throw new InvalidOperationException("VirtualMachineThreadStack overflow!");

            fixed (byte* pStack = stack)
            {
                *(ScriptValue*)(pStack + stackOffset) = value;
                stackOffset += sizeOfScriptValue;
            }
        }
    
        public ScriptValue Pop()
        {
            if (stackOffset - sizeOfScriptValue < 0)
                throw new InvalidOperationException("VirtualMachineThreadStack underflow!");
        
            fixed (byte* pStack = stack)
            {
                stackOffset -= sizeOfScriptValue;
                return *(ScriptValue*)(pStack + stackOffset);
            }
        }
        
        public ScriptValue Peek()
        {
            if (stackOffset - sizeOfScriptValue < 0)
                throw new InvalidOperationException("VirtualMachineThreadStack underflow!");
        
            fixed (byte* pStack = stack)
            {
                return *(ScriptValue*)(pStack + stackOffset - sizeOfScriptValue);
            }
        }
    }
}