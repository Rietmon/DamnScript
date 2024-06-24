﻿using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes;

[StructLayout(LayoutKind.Sequential)]
public struct JumpIfEquals
{
    public const int OpCode = 0x6;
    public readonly int opCode = OpCode;
    public int jumpOffset;
    
    public JumpIfEquals(int jumpOffset)
    {
        this.jumpOffset = jumpOffset;
    }
}