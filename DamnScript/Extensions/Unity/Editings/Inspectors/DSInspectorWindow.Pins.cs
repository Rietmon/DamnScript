#if UNITY_EDITOR
using System;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Cores.Pins;
using DamnScript.Runtimes.VirtualMachines;
using DamnScript.Runtimes.VirtualMachines.ScriptValues;
using DamnScript.Runtimes.VirtualMachines.Threads;
using UnityEditor;
using UnityEngine;

namespace DamnScript.Extensions.Unity.Editings.Inspectors
{
	public unsafe partial class DSInspectorWindow
	{
		private Vector2 _pinsScrollPosition;

		private void OnGUIPins()
		{
			_pinsScrollPosition = GUILayout.BeginScrollView(_pinsScrollPosition);

			GUILayout.Label($"Pins: {PinHelper.PinsCount.ToString()}", EditorStyles.boldLabel);
			
			GUILayout.BeginVertical();
			{
				for (var i = 0; i < PinHelper.Buckets.Length; i++)
				{
					var bucket = PinHelper.Buckets[i];
					for (var j = 0; j < bucket.pinnedObjects.Length; j++)
					{
						var obj = bucket.pinnedObjects[j];
						if (obj.hash == 0)
							continue;
						
						var result = $"<b>{obj.target}</b> | <color=green>{obj.hash.ToString()}</color>";
						GUILayout.Label(result, _richTextStyle);
						GUILayout.Space(5);
					}
				}
			}
			GUILayout.EndVertical();
			
			GUILayout.EndScrollView();
		}
	}
}
#endif