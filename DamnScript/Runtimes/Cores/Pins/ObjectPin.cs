﻿namespace DamnScript.Runtimes.Cores.Pins
{
    public readonly unsafe struct ObjectPin
    {
        public object Target => PinHelper.GetTarget(this);
        
        public bool IsAllocated => hash != 0;
        
        public readonly long hash;
        
        public ObjectPin(long hash) => this.hash = hash;
        
        public void Free() => PinHelper.Free(this);
        
        public void* GetAddress() => PinHelper.GetAddress(this);
        
        public object FreeAndGetTarget() => PinHelper.FreeAndGetTarget(this);
    }
}