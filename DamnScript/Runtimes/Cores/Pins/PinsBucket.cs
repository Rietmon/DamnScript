using System;
using System.Runtime.CompilerServices;

namespace DamnScript.Runtimes.Cores.Pins
{
	public unsafe struct PinsBucket
	{
		private const int DefaultPinsBucketSize = PinHelper.DefaultPinsBucketSize;
		
		public long freeSlots;
		public readonly ObjectPin[] pinnedObjects;

		public PinsBucket(int _)
		{
			freeSlots = DefaultPinsBucketSize;
			pinnedObjects = new ObjectPin[DefaultPinsBucketSize];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public short FastFindFreeSlot()
		{
			const int hashStride = ObjectPin.HashOffsetAsIntPtr + 1;

			ref var first = ref pinnedObjects[0];
			var begin = &((PinHandleUnmanaged*)UnsafeUtilities.ToPointer(ref first))->hash;
			
			if (*begin == 0) return 0;
			if (*(begin + hashStride * 1) == 0) return 1;
			if (*(begin + hashStride * 2) == 0) return 2;
			if (*(begin + hashStride * 3) == 0) return 3;
			if (*(begin + hashStride * 4) == 0) return 4;
			if (*(begin + hashStride * 5) == 0) return 5;
			if (*(begin + hashStride * 6) == 0) return 6;
			if (*(begin + hashStride * 7) == 0) return 7;
			if (*(begin + hashStride * 8) == 0) return 8;
			if (*(begin + hashStride * 9) == 0) return 9;
			if (*(begin + hashStride * 10) == 0) return 10;
			if (*(begin + hashStride * 11) == 0) return 11;
			if (*(begin + hashStride * 12) == 0) return 12;
			if (*(begin + hashStride * 13) == 0) return 13;
			if (*(begin + hashStride * 14) == 0) return 14;
			if (*(begin + hashStride * 15) == 0) return 15;
			if (*(begin + hashStride * 16) == 0) return 16;
			if (*(begin + hashStride * 17) == 0) return 17;
			if (*(begin + hashStride * 18) == 0) return 18;
			if (*(begin + hashStride * 19) == 0) return 19;
			if (*(begin + hashStride * 20) == 0) return 20;
			if (*(begin + hashStride * 21) == 0) return 21;
			if (*(begin + hashStride * 22) == 0) return 22;
			if (*(begin + hashStride * 23) == 0) return 23;
			if (*(begin + hashStride * 24) == 0) return 24;
			if (*(begin + hashStride * 25) == 0) return 25;
			if (*(begin + hashStride * 26) == 0) return 26;
			if (*(begin + hashStride * 27) == 0) return 27;
			if (*(begin + hashStride * 28) == 0) return 28;
			if (*(begin + hashStride * 29) == 0) return 29;
			if (*(begin + hashStride * 30) == 0) return 30;
			if (*(begin + hashStride * 31) == 0) return 31;
			
#if DAMN_SCRIPT_PINSBUCKET_SIZE_64
			if (*(begin + hashStride * 32) == 0) return 32;
			if (*(begin + hashStride * 33) == 0) return 33;
			if (*(begin + hashStride * 34) == 0) return 34;
			if (*(begin + hashStride * 35) == 0) return 35;
			if (*(begin + hashStride * 36) == 0) return 36;
			if (*(begin + hashStride * 37) == 0) return 37;
			if (*(begin + hashStride * 38) == 0) return 38;
			if (*(begin + hashStride * 39) == 0) return 39;
			if (*(begin + hashStride * 40) == 0) return 40;
			if (*(begin + hashStride * 41) == 0) return 41;
			if (*(begin + hashStride * 42) == 0) return 42;
			if (*(begin + hashStride * 43) == 0) return 43;
			if (*(begin + hashStride * 44) == 0) return 44;
			if (*(begin + hashStride * 45) == 0) return 45;
			if (*(begin + hashStride * 46) == 0) return 46;
			if (*(begin + hashStride * 47) == 0) return 47;
			if (*(begin + hashStride * 48) == 0) return 48;
			if (*(begin + hashStride * 49) == 0) return 49;
			if (*(begin + hashStride * 50) == 0) return 50;
			if (*(begin + hashStride * 51) == 0) return 51;
			if (*(begin + hashStride * 52) == 0) return 52;
			if (*(begin + hashStride * 53) == 0) return 53;
			if (*(begin + hashStride * 54) == 0) return 54;
			if (*(begin + hashStride * 55) == 0) return 55;
			if (*(begin + hashStride * 56) == 0) return 56;
			if (*(begin + hashStride * 57) == 0) return 57;
			if (*(begin + hashStride * 58) == 0) return 58;
			if (*(begin + hashStride * 59) == 0) return 59;
			if (*(begin + hashStride * 60) == 0) return 60;
			if (*(begin + hashStride * 61) == 0) return 61;
			if (*(begin + hashStride * 62) == 0) return 62;
			if (*(begin + hashStride * 63) == 0) return 63;
#endif

			return -1;
		}
	}
}