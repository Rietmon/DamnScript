using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DamnScript.Runtimes.Cores.Collections
{
	public unsafe struct NativeMap<TValue> : IDisposable
		where TValue : unmanaged
	{
		private NativeArray<Entry> _entries;
		private int _count;
		private int _capacity;

		public bool IsValid
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _entries.IsValid;
		}

		public NativeMap(int capacity)
		{
			if (capacity <= 0)
				throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than 0.");

			_entries = new NativeArray<Entry>(capacity);
			_count = 0;
			_capacity = capacity;
		}

		public void Add(int key, TValue value)
		{
			AssertIfNotInitialized();
			
			if (_count >= _capacity * 0.75)
			{
				_capacity *= 2;
				NativeArray<Entry>.ReAlloc(UnsafeUtilities.AsPointer(ref _entries), _capacity);
			}

			var index = Hash(key);
			var probes = 0;
			
			ref var entry = ref _entries[index];
			while (entry.used && entry.key != key)
			{
				if (probes++ > _capacity)
					throw new InvalidOperationException("Infinite loop detected in hash map probing.");

				index = (index + 1) % _capacity;
				entry = ref _entries[index];
			}

			if (!entry.used)
			{
				entry.key = key;
				entry.value = value;
				entry.used = true;
				_count++;
			}
			else
			{
				entry.value = value;
			}
		}
		
		public bool TryGetValue(int key, out TValue value)
		{
			var index = Hash(key);
			var start = index;

			while (_entries[index].used)
			{
				if (_entries[index].key == key)
				{
					value = _entries[index].value;
					return true;
				}

				index = (index + 1) % _capacity;
				if (index == start)
					break;
			}

			value = default;
			return false;
		}

		private int Hash(int key) => key % _capacity;
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AssertIfNotInitialized()
		{
			if (!IsValid)
				throw new NullReferenceException("NativeHashMap is not initialized.");
		}
    
		public void Dispose()
		{
			AssertIfNotInitialized();
        
			_entries.Dispose();
			this = default;
		}
		
		[DebuggerDisplay("{value}:{key}:{used}")]
		public struct Entry
		{
			public TValue value;
			public int key;
			public bool used;
		}
	}
}