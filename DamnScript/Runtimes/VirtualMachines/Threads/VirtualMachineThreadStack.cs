using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Debugs;
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

        public ScriptValue* Ptr => (ScriptValue*)UnsafeUtilities.AsPointer(ref stack);
        
        public StackBuffer stack;
        public int stackOffset;
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(ScriptValue value)
        {
            if (stackOffset + 1 > StackSize)
                throw new InvalidOperationException("VirtualMachineThreadStack overflow!");

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

        public ScriptValue* GetPointer(int offset)
        {
            if (stackOffset - offset < 0)
                throw new InvalidOperationException("VirtualMachineThreadStack underflow!");

            return Ptr + stackOffset - offset;
        }
        
        [StructLayout(LayoutKind.Sequential, Size = StackSize * ScriptValue.Size)]
        public struct StackBuffer { }
    }
}