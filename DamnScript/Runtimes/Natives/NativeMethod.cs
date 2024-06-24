﻿namespace DamnScript.Runtimes.Natives;

public readonly unsafe struct NativeMethod
{
    public readonly void* methodPointer;
    public readonly int argumentsCount;
    public readonly bool isAsync;
    public readonly bool isStatic;
    
    public NativeMethod(void* methodPointer, int argumentsCount, bool isAsync, bool isStatic)
    {
        this.methodPointer = methodPointer;
        this.argumentsCount = argumentsCount + (isStatic ? 0 : 1);
        this.isAsync = isAsync;
        this.isStatic = isStatic;
    }
}