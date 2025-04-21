using System;
using System.Runtime.CompilerServices;

namespace DamnScript.Runtimes.VirtualMachines.Threads
{
    public unsafe struct VirtualMachineRegisters
    {
        public long this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => index switch
            {
                0 => registers[0],
                1 => registers[1],
                2 => registers[2],
                3 => registers[3],
                _ => throw new IndexOutOfRangeException()
            };
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                switch (index)
                {
                    case 0: registers[0] = value; break;
                    case 1: registers[1] = value; break;
                    case 2: registers[2] = value; break;
                    case 3: registers[3] = value; break;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }
        
        public fixed long registers[4];
    }
}