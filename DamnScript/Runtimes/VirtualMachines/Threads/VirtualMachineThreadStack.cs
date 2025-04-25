using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.VirtualMachines.ScriptValues;

namespace DamnScript.Runtimes.VirtualMachines.Threads
{
    public unsafe struct VirtualMachineThreadStack
    {
#if DAMN_SCRIPT_STACK_SIZE_32
        private const int StackSize = 32;
#elif DAMN_SCRIPT_STACK_SIZE_64
        private const int StackSize = 64;
#else
        private const int StackSize = 16;
#endif

        public ScriptValue* Ptr => (ScriptValue*)UnsafeUtilities.AsPointer(ref stack);
        
        public StackBuffer stack;
        public int stackOffset;
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(ScriptValue value)
        {
            if (stackOffset + 1 > StackSize)
                throw new InvalidOperationException("VirtualMachineThreadStack overflow! Try to use DAMN_SCRIPT_STACK_SIZE_32 or DAMN_SCRIPT_STACK_SIZE_64.");

            *(Ptr + stackOffset) = value;
            stackOffset++;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue Pop()
        {
            if (stackOffset - 1 < 0)
                throw new InvalidOperationException("VirtualMachineThreadStack underflow!");
            
            stackOffset--;
            return *(Ptr + stackOffset);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue Peek()
        {
            if (stackOffset - 1 < 0)
                throw new InvalidOperationException("VirtualMachineThreadStack underflow!");
        
            return *(Ptr + stackOffset - 1);
        }
        
        [StructLayout(LayoutKind.Sequential, Size = StackSize * ScriptValue.Size)]
        public struct StackBuffer { }
    }
}