namespace DamnScript.Runtimes.Cores.Pins
{
    public readonly struct DSObjectPin
    {
        public readonly long hash;
        
        public object Target => PinHelper.GetTarget(this);
        
        public bool IsAllocated => hash != 0;
        
        public DSObjectPin(long hash) => this.hash = hash;
        
        public void Free() => PinHelper.Free(this);
        
        public object FreeAndGetTarget() => PinHelper.FreeAndGetTarget(this);
    }
}