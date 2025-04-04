namespace DamnScript.Runtimes.Cores.Pins
{
	public readonly struct PinHandle
	{
#if DAMN_SCRIPT_PINNING_DEBUG
		public const int HashOffsetAsIntPtr = 2;
#else
		public const int HashOffsetAsIntPtr = 1;
#endif
		
		public readonly object target;
#if DAMN_SCRIPT_PINNING_DEBUG
		public readonly string stack;
#endif
		public readonly long hash;

		public PinHandle(int hash, object target)
		{
			this.hash = hash;
#if DAMN_SCRIPT_PINNING_DEBUG
			this.stack = Environment.StackTrace;
#endif
			this.target = target;
		}
	}

	public unsafe struct PinHandleUnmanaged
	{
		public void* target;
#if DAMN_SCRIPT_PINNING_DEBUG
		public void* stack;
#endif
		public long hash;
	}
}