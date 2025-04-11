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
		
		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
		public short FastFind()
		{
			const int hashOffset = ObjectPin.HashOffsetAsIntPtr + 1;

			ref var first = ref pinnedObjects[0];
			var begin = &((PinHandleUnmanaged*)UnsafeUtilities.ToPointer(ref first))->hash;

			if (*begin == 0) return (short)((begin - begin) / hashOffset);
			if (*(begin + hashOffset * 1) == 0) return (short)((begin + hashOffset * 1 - begin) / hashOffset);
			if (*(begin + hashOffset * 2) == 0) return (short)((begin + hashOffset * 2 - begin) / hashOffset);
			if (*(begin + hashOffset * 3) == 0) return (short)((begin + hashOffset * 3 - begin) / hashOffset);
			if (*(begin + hashOffset * 4) == 0) return (short)((begin + hashOffset * 4 - begin) / hashOffset);
			if (*(begin + hashOffset * 5) == 0) return (short)((begin + hashOffset * 5 - begin) / hashOffset);
			if (*(begin + hashOffset * 6) == 0) return (short)((begin + hashOffset * 6 - begin) / hashOffset);
			if (*(begin + hashOffset * 7) == 0) return (short)((begin + hashOffset * 7 - begin) / hashOffset);
			if (*(begin + hashOffset * 8) == 0) return (short)((begin + hashOffset * 8 - begin) / hashOffset);
			if (*(begin + hashOffset * 9) == 0) return (short)((begin + hashOffset * 9 - begin) / hashOffset);
			if (*(begin + hashOffset * 10) == 0) return (short)((begin + hashOffset * 10 - begin) / hashOffset);
			if (*(begin + hashOffset * 11) == 0) return (short)((begin + hashOffset * 11 - begin) / hashOffset);
			if (*(begin + hashOffset * 12) == 0) return (short)((begin + hashOffset * 12 - begin) / hashOffset);
			if (*(begin + hashOffset * 13) == 0) return (short)((begin + hashOffset * 13 - begin) / hashOffset);
			if (*(begin + hashOffset * 14) == 0) return (short)((begin + hashOffset * 14 - begin) / hashOffset);
			if (*(begin + hashOffset * 15) == 0) return (short)((begin + hashOffset * 15 - begin) / hashOffset);
			if (*(begin + hashOffset * 16) == 0) return (short)((begin + hashOffset * 16 - begin) / hashOffset);
			if (*(begin + hashOffset * 17) == 0) return (short)((begin + hashOffset * 17 - begin) / hashOffset);
			if (*(begin + hashOffset * 18) == 0) return (short)((begin + hashOffset * 18 - begin) / hashOffset);
			if (*(begin + hashOffset * 19) == 0) return (short)((begin + hashOffset * 19 - begin) / hashOffset);
			if (*(begin + hashOffset * 20) == 0) return (short)((begin + hashOffset * 20 - begin) / hashOffset);
			if (*(begin + hashOffset * 21) == 0) return (short)((begin + hashOffset * 21 - begin) / hashOffset);
			if (*(begin + hashOffset * 22) == 0) return (short)((begin + hashOffset * 22 - begin) / hashOffset);
			if (*(begin + hashOffset * 23) == 0) return (short)((begin + hashOffset * 23 - begin) / hashOffset);
			if (*(begin + hashOffset * 24) == 0) return (short)((begin + hashOffset * 24 - begin) / hashOffset);
			if (*(begin + hashOffset * 25) == 0) return (short)((begin + hashOffset * 25 - begin) / hashOffset);
			if (*(begin + hashOffset * 26) == 0) return (short)((begin + hashOffset * 26 - begin) / hashOffset);
			if (*(begin + hashOffset * 27) == 0) return (short)((begin + hashOffset * 27 - begin) / hashOffset);
			if (*(begin + hashOffset * 28) == 0) return (short)((begin + hashOffset * 28 - begin) / hashOffset);
			if (*(begin + hashOffset * 29) == 0) return (short)((begin + hashOffset * 29 - begin) / hashOffset);
			if (*(begin + hashOffset * 30) == 0) return (short)((begin + hashOffset * 30 - begin) / hashOffset);
			if (*(begin + hashOffset * 31) == 0) return (short)((begin + hashOffset * 31 - begin) / hashOffset);
			
#if DAMN_SCRIPT_PINSBUCKET_SIZE_64
			if (*(begin + hashOffset * 32) == 0) return (short)((begin + hashOffset * 32 - begin) / hashOffset);
			if (*(begin + hashOffset * 33) == 0) return (short)((begin + hashOffset * 33 - begin) / hashOffset);
			if (*(begin + hashOffset * 34) == 0) return (short)((begin + hashOffset * 34 - begin) / hashOffset);
			if (*(begin + hashOffset * 35) == 0) return (short)((begin + hashOffset * 35 - begin) / hashOffset);
			if (*(begin + hashOffset * 36) == 0) return (short)((begin + hashOffset * 36 - begin) / hashOffset);
			if (*(begin + hashOffset * 37) == 0) return (short)((begin + hashOffset * 37 - begin) / hashOffset);
			if (*(begin + hashOffset * 38) == 0) return (short)((begin + hashOffset * 38 - begin) / hashOffset);
			if (*(begin + hashOffset * 39) == 0) return (short)((begin + hashOffset * 39 - begin) / hashOffset);
			if (*(begin + hashOffset * 40) == 0) return (short)((begin + hashOffset * 40 - begin) / hashOffset);
			if (*(begin + hashOffset * 41) == 0) return (short)((begin + hashOffset * 41 - begin) / hashOffset);
			if (*(begin + hashOffset * 42) == 0) return (short)((begin + hashOffset * 42 - begin) / hashOffset);
			if (*(begin + hashOffset * 43) == 0) return (short)((begin + hashOffset * 43 - begin) / hashOffset);
			if (*(begin + hashOffset * 44) == 0) return (short)((begin + hashOffset * 44 - begin) / hashOffset);
			if (*(begin + hashOffset * 45) == 0) return (short)((begin + hashOffset * 45 - begin) / hashOffset);
			if (*(begin + hashOffset * 46) == 0) return (short)((begin + hashOffset * 46 - begin) / hashOffset);
			if (*(begin + hashOffset * 47) == 0) return (short)((begin + hashOffset * 47 - begin) / hashOffset);
			if (*(begin + hashOffset * 48) == 0) return (short)((begin + hashOffset * 48 - begin) / hashOffset);
			if (*(begin + hashOffset * 49) == 0) return (short)((begin + hashOffset * 49 - begin) / hashOffset);
			if (*(begin + hashOffset * 50) == 0) return (short)((begin + hashOffset * 50 - begin) / hashOffset);
			if (*(begin + hashOffset * 51) == 0) return (short)((begin + hashOffset * 51 - begin) / hashOffset);
			if (*(begin + hashOffset * 52) == 0) return (short)((begin + hashOffset * 52 - begin) / hashOffset);
			if (*(begin + hashOffset * 53) == 0) return (short)((begin + hashOffset * 53 - begin) / hashOffset);
			if (*(begin + hashOffset * 54) == 0) return (short)((begin + hashOffset * 54 - begin) / hashOffset);
			if (*(begin + hashOffset * 55) == 0) return (short)((begin + hashOffset * 55 - begin) / hashOffset);
			if (*(begin + hashOffset * 56) == 0) return (short)((begin + hashOffset * 56 - begin) / hashOffset);
			if (*(begin + hashOffset * 57) == 0) return (short)((begin + hashOffset * 57 - begin) / hashOffset);
			if (*(begin + hashOffset * 58) == 0) return (short)((begin + hashOffset * 58 - begin) / hashOffset);
			if (*(begin + hashOffset * 59) == 0) return (short)((begin + hashOffset * 59 - begin) / hashOffset);
			if (*(begin + hashOffset * 60) == 0) return (short)((begin + hashOffset * 60 - begin) / hashOffset);
			if (*(begin + hashOffset * 61) == 0) return (short)((begin + hashOffset * 61 - begin) / hashOffset);
			if (*(begin + hashOffset * 62) == 0) return (short)((begin + hashOffset * 62 - begin) / hashOffset);
			if (*(begin + hashOffset * 63) == 0) return (short)((begin + hashOffset * 63 - begin) / hashOffset);
#endif

			return -1;
		}
	}
}