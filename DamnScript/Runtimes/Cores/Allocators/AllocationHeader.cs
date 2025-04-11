namespace DamnScript.Runtimes.Cores.Allocators
{
	public struct AllocationHeader
	{
		public const int Size = 8;

		public bool isFree;
		public int size;
	}
}