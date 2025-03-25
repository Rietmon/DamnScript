using System;

namespace DamnScript.Runtimes.Natives
{
    public readonly unsafe struct NativeMethod
    {
        public bool IsAsync => (flags & NativeMethodFlags.IsAsync) != 0;
        public bool IsStatic => (flags & NativeMethodFlags.IsStatic) != 0;
        public bool HasReturnValue => (flags & NativeMethodFlags.HasReturnValue) != 0;
        
        public readonly void* methodPointer;
        public readonly int argumentsCount;
        public readonly NativeMethodFlags flags;

        public NativeMethod(void* methodPointer, int argumentsCount, bool isAsync, bool isStatic, bool hasReturnValue)
        {
            this.methodPointer = methodPointer;
            this.argumentsCount = argumentsCount;
            flags = NativeMethodFlags.None;
            flags |= isStatic ? NativeMethodFlags.IsStatic : NativeMethodFlags.None;
            flags |= isAsync ? NativeMethodFlags.IsAsync : NativeMethodFlags.None;
            flags |= hasReturnValue ? NativeMethodFlags.HasReturnValue : NativeMethodFlags.None;
        }
    }
    
    [Flags]
    public enum NativeMethodFlags
    {
        None = 0,
        IsStatic = 1 << 0,
        IsAsync = 1 << 1,
        HasReturnValue = 1 << 2,
    }
}