namespace DamnScript.Runtimes.VirtualMachines.Threads;

public unsafe struct VirtualMachineThreadStack
{
    private const int StackSize = 128;
    private fixed byte _stack[StackSize];
    private int _stackOffset;
    
    public void Push(long value)
    {
        const int sizeofLong = sizeof(long);
        if (_stackOffset + sizeofLong > StackSize)
            throw new InvalidOperationException("VirtualThreadStack overflow!");

        fixed (byte* pStack = _stack)
        {
            *(long*)(pStack + _stackOffset) = value;
            _stackOffset += sizeofLong;
        }
    }
    
    public long Pop()
    {
        const int sizeofLong = sizeof(long);
        if (_stackOffset - sizeofLong < 0)
            throw new InvalidOperationException("VirtualThreadStack underflow!");
        
        fixed (byte* pStack = _stack)
        {
            _stackOffset -= sizeofLong;
            return *(long*)(pStack + _stackOffset);
        }
    }
}