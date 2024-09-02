using System;
using DamnScript.Runtimes.Cores.Types;

namespace DamnScript.Runtimes.Natives
{
    public readonly struct NativeMethodId : IEquatable<NativeMethodId>
    {
        public readonly String32 name;
        public readonly int argumentsCount;
        
        public NativeMethodId(String32 name, int argumentsCount)
        {
            this.name = name;
            this.argumentsCount = argumentsCount;
        }
        
        public bool Equals(NativeMethodId other) => this == other;

        public override bool Equals(object obj) => obj is NativeMethodId other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(name, argumentsCount);
        
        public static bool operator ==(NativeMethodId l, NativeMethodId r) => l.argumentsCount == r.argumentsCount && l.name == r.name;

        public static bool operator !=(NativeMethodId l, NativeMethodId r) => !(l == r);
    }
}