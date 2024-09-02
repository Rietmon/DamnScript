using System;
using DamnScript.Runtimes.Natives;

namespace DamnScript.Runtimes.VirtualMachines.Threads
{
    public unsafe struct VirtualMachineThreadStack
    {
        private static readonly int sizeOfScriptValue = sizeof(ScriptValue);
    
        private const int StackSize = 384;
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