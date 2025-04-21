using System;
using System.Runtime.CompilerServices;
using DamnScript.Runtimes.VirtualMachines.ScriptValues;

namespace DamnScript.Runtimes.VirtualMachines.Threads
{
    public unsafe struct VirtualMachineThreadStack
    {
#if DAMN_SCRIPT_STACK_SIZE_16
        private const int StackSize = 16 * ScriptValue.Size;
#elif DAMN_SCRIPT_STACK_SIZE_64
        private const int StackSize = 64 * ScriptValue.Size;
#else
        private const int StackSize = 32 * ScriptValue.Size;
#endif
        
        public fixed byte stack[StackSize];
        public int stackOffset;
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(ScriptValue value)
        {
            if (stackOffset + ScriptValue.Size > StackSize)
                throw new InvalidOperationException("VirtualMachineThreadStack overflow!");

            fixed (byte* pStack = stack)
            {
                *(ScriptValue*)(pStack + stackOffset) = value;
                stackOffset += ScriptValue.Size;
            }
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue Pop()
        {
            if (stackOffset - ScriptValue.Size < 0)
                throw new InvalidOperationException("VirtualMachineThreadStack underflow!");
        
            fixed (byte* pStack = stack)
            {
                stackOffset -= ScriptValue.Size;
                return *(ScriptValue*)(pStack + stackOffset);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue Peek()
        {
            if (stackOffset - ScriptValue.Size < 0)
                throw new InvalidOperationException("VirtualMachineThreadStack underflow!");
        
            fixed (byte* pStack = stack)
            {
                return *(ScriptValue*)(pStack + stackOffset - ScriptValue.Size);
            }
        }
    }
}