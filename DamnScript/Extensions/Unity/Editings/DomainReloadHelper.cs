#if UNITY_EDITOR
using DamnScript.Runtimes;
using DamnScript.Runtimes.Cores;
using UnityEditor;
using UnityEngine;

namespace DamnScript.Extensions.Unity.Editings
{
	[InitializeOnLoad]
	internal static class DomainReloadHelper
	{
		static DomainReloadHelper()
		{
			AssemblyReloadEvents.beforeAssemblyReload += OnBeforeReload;
		}

		public static void OnBeforeReload()
		{
			ScriptEngine.TotalDispose();
			
#if DAMN_SCRIPT_ENABLE_MEMORY_DEBUG
			if (UnsafeUtilities.AllocInfos.Count > 0)
				Debug.LogError($"[{nameof(DomainReloadHelper)}] ({nameof(OnBeforeReload)}) " +
				               $"Memory leak detected! Allocated: {UnsafeUtilities.AllocInfos.Count.ToString()}");
			else
				Debug.Log($"[{nameof(DomainReloadHelper)}] ({nameof(OnBeforeReload)}) " +
				         $"No memory leak detected! Allocated: {UnsafeUtilities.AllocInfos.Count.ToString()}");
#else
			Debug.Log($"[{nameof(DomainReloadHelper)}] ({nameof(OnBeforeReload)}) " +
			          $"Memory cleared before domain reload");
#endif
		}
	}
}
#endif