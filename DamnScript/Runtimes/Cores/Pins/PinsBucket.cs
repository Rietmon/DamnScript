using System;
using System.Runtime.CompilerServices;

namespace DamnScript.Runtimes.Cores.Pins
{
	public unsafe struct PinsBucket
	{
		public long freeSlots;
		public readonly ObjectPin[] pinnedObjects;
		
		public PinsBucket(int size)	
		{
			freeSlots = size;
			pinnedObjects = new ObjectPin[size];
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public short FastFind()
		{
			const int hashStride = ObjectPin.HashOffsetAsIntPtr + 1;

			ref var first = ref pinnedObjects[0];
			var begin = &((PinHandleUnmanaged*)UnsafeUtilities.ToPointer(ref first))->hash;

			if (*begin == 0) return (short)((begin - begin) / hashStride);
			if (*(begin + hashStride * 1) == 0) return (short)((begin + hashStride * 1 - begin) / hashStride);
			if (*(begin + hashStride * 2) == 0) return (short)((begin + hashStride * 2 - begin) / hashStride);
			if (*(begin + hashStride * 3) == 0) return (short)((begin + hashStride * 3 - begin) / hashStride);
			if (*(begin + hashStride * 4) == 0) return (short)((begin + hashStride * 4 - begin) / hashStride);
			if (*(begin + hashStride * 5) == 0) return (short)((begin + hashStride * 5 - begin) / hashStride);
			if (*(begin + hashStride * 6) == 0) return (short)((begin + hashStride * 6 - begin) / hashStride);
			if (*(begin + hashStride * 7) == 0) return (short)((begin + hashStride * 7 - begin) / hashStride);
			if (*(begin + hashStride * 8) == 0) return (short)((begin + hashStride * 8 - begin) / hashStride);
			if (*(begin + hashStride * 9) == 0) return (short)((begin + hashStride * 9 - begin) / hashStride);
			if (*(begin + hashStride * 10) == 0) return (short)((begin + hashStride * 10 - begin) / hashStride);
			if (*(begin + hashStride * 11) == 0) return (short)((begin + hashStride * 11 - begin) / hashStride);
			if (*(begin + hashStride * 12) == 0) return (short)((begin + hashStride * 12 - begin) / hashStride);
			if (*(begin + hashStride * 13) == 0) return (short)((begin + hashStride * 13 - begin) / hashStride);
			if (*(begin + hashStride * 14) == 0) return (short)((begin + hashStride * 14 - begin) / hashStride);
			if (*(begin + hashStride * 15) == 0) return (short)((begin + hashStride * 15 - begin) / hashStride);
			if (*(begin + hashStride * 16) == 0) return (short)((begin + hashStride * 16 - begin) / hashStride);
			if (*(begin + hashStride * 17) == 0) return (short)((begin + hashStride * 17 - begin) / hashStride);
			if (*(begin + hashStride * 18) == 0) return (short)((begin + hashStride * 18 - begin) / hashStride);
			if (*(begin + hashStride * 19) == 0) return (short)((begin + hashStride * 19 - begin) / hashStride);
			if (*(begin + hashStride * 20) == 0) return (short)((begin + hashStride * 20 - begin) / hashStride);
			if (*(begin + hashStride * 21) == 0) return (short)((begin + hashStride * 21 - begin) / hashStride);
			if (*(begin + hashStride * 22) == 0) return (short)((begin + hashStride * 22 - begin) / hashStride);
			if (*(begin + hashStride * 23) == 0) return (short)((begin + hashStride * 23 - begin) / hashStride);
			if (*(begin + hashStride * 24) == 0) return (short)((begin + hashStride * 24 - begin) / hashStride);
			if (*(begin + hashStride * 25) == 0) return (short)((begin + hashStride * 25 - begin) / hashStride);
			if (*(begin + hashStride * 26) == 0) return (short)((begin + hashStride * 26 - begin) / hashStride);
			if (*(begin + hashStride * 27) == 0) return (short)((begin + hashStride * 27 - begin) / hashStride);
			if (*(begin + hashStride * 28) == 0) return (short)((begin + hashStride * 28 - begin) / hashStride);
			if (*(begin + hashStride * 29) == 0) return (short)((begin + hashStride * 29 - begin) / hashStride);
			if (*(begin + hashStride * 30) == 0) return (short)((begin + hashStride * 30 - begin) / hashStride);
			if (*(begin + hashStride * 31) == 0) return (short)((begin + hashStride * 31 - begin) / hashStride);
			
#if DAMN_SCRIPT_PINSBUCKET_SIZE_64
			if (*(begin + hashStride * 32) == 0) return (short)((begin + hashStride * 32 - begin) / hashStride);
			if (*(begin + hashStride * 33) == 0) return (short)((begin + hashStride * 33 - begin) / hashStride);
			if (*(begin + hashStride * 34) == 0) return (short)((begin + hashStride * 34 - begin) / hashStride);
			if (*(begin + hashStride * 35) == 0) return (short)((begin + hashStride * 35 - begin) / hashStride);
			if (*(begin + hashStride * 36) == 0) return (short)((begin + hashStride * 36 - begin) / hashStride);
			if (*(begin + hashStride * 37) == 0) return (short)((begin + hashStride * 37 - begin) / hashStride);
			if (*(begin + hashStride * 38) == 0) return (short)((begin + hashStride * 38 - begin) / hashStride);
			if (*(begin + hashStride * 39) == 0) return (short)((begin + hashStride * 39 - begin) / hashStride);
			if (*(begin + hashStride * 40) == 0) return (short)((begin + hashStride * 40 - begin) / hashStride);
			if (*(begin + hashStride * 41) == 0) return (short)((begin + hashStride * 41 - begin) / hashStride);
			if (*(begin + hashStride * 42) == 0) return (short)((begin + hashStride * 42 - begin) / hashStride);
			if (*(begin + hashStride * 43) == 0) return (short)((begin + hashStride * 43 - begin) / hashStride);
			if (*(begin + hashStride * 44) == 0) return (short)((begin + hashStride * 44 - begin) / hashStride);
			if (*(begin + hashStride * 45) == 0) return (short)((begin + hashStride * 45 - begin) / hashStride);
			if (*(begin + hashStride * 46) == 0) return (short)((begin + hashStride * 46 - begin) / hashStride);
			if (*(begin + hashStride * 47) == 0) return (short)((begin + hashStride * 47 - begin) / hashStride);
			if (*(begin + hashStride * 48) == 0) return (short)((begin + hashStride * 48 - begin) / hashStride);
			if (*(begin + hashStride * 49) == 0) return (short)((begin + hashStride * 49 - begin) / hashStride);
			if (*(begin + hashStride * 50) == 0) return (short)((begin + hashStride * 50 - begin) / hashStride);
			if (*(begin + hashStride * 51) == 0) return (short)((begin + hashStride * 51 - begin) / hashStride);
			if (*(begin + hashStride * 52) == 0) return (short)((begin + hashStride * 52 - begin) / hashStride);
			if (*(begin + hashStride * 53) == 0) return (short)((begin + hashStride * 53 - begin) / hashStride);
			if (*(begin + hashStride * 54) == 0) return (short)((begin + hashStride * 54 - begin) / hashStride);
			if (*(begin + hashStride * 55) == 0) return (short)((begin + hashStride * 55 - begin) / hashStride);
			if (*(begin + hashStride * 56) == 0) return (short)((begin + hashStride * 56 - begin) / hashStride);
			if (*(begin + hashStride * 57) == 0) return (short)((begin + hashStride * 57 - begin) / hashStride);
			if (*(begin + hashStride * 58) == 0) return (short)((begin + hashStride * 58 - begin) / hashStride);
			if (*(begin + hashStride * 59) == 0) return (short)((begin + hashStride * 59 - begin) / hashStride);
			if (*(begin + hashStride * 60) == 0) return (short)((begin + hashStride * 60 - begin) / hashStride);
			if (*(begin + hashStride * 61) == 0) return (short)((begin + hashStride * 61 - begin) / hashStride);
			if (*(begin + hashStride * 62) == 0) return (short)((begin + hashStride * 62 - begin) / hashStride);
			if (*(begin + hashStride * 63) == 0) return (short)((begin + hashStride * 63 - begin) / hashStride);
#endif

			return -1;
		}
	}
}