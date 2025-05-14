#if UNITY_EDITOR
using System;
using DamnScript.Runtimes;
using DamnScript.Runtimes.VirtualMachines;
using DamnScript.Runtimes.VirtualMachines.ScriptValues;
using DamnScript.Runtimes.VirtualMachines.Threads;
using UnityEditor;
using UnityEngine;

namespace DamnScript.Extensions.Unity.Editings.Inspectors
{
	public unsafe partial class DSInspectorWindow
	{
		private bool[] _threadsFoldout = new bool[VirtualMachine.DefaultThreadsCapacity];
		private bool[] _stacksFoldout = new bool[VirtualMachine.DefaultThreadsCapacity];
		private bool[] _paramsFoldout = new bool[VirtualMachine.DefaultThreadsCapacity];
		private bool[] _registersFoldout = new bool[VirtualMachine.DefaultThreadsCapacity];

		private Vector2 _scrollPosition;

		private void OnGUIThreads()
		{
			var vm = ScriptEngine.mainPtr.value;

			if (_threadsFoldout.Length != vm->threads.Length)
				Array.Resize(ref _threadsFoldout, vm->threads.Length);
			if (_stacksFoldout.Length != vm->threads.Length)
				Array.Resize(ref _stacksFoldout, vm->threads.Length);
			if (_paramsFoldout.Length != vm->threads.Length)
				Array.Resize(ref _paramsFoldout, vm->threads.Length);
			if (_registersFoldout.Length != vm->threads.Length)
				Array.Resize(ref _registersFoldout, vm->threads.Length);

			_scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
			GUILayout.Label($"Threads allocated: {vm->ThreadsAllocated.ToString()}/{vm->threads.Length.ToString()}");
			
			GUILayout.BeginVertical();
			{
				for (var i = 0; i < vm->threads.Length; i++)
				{
					var thread = vm->threads.Begin + i;
					if (!thread->isAlive)
						break;

					_threadsFoldout[i] = EditorGUILayout.Foldout(_threadsFoldout[i], $"Thread {i}");
					if (_threadsFoldout[i])
					{
						GUILayout.BeginHorizontal();
						{
							GUILayout.Space(25);
							DrawThreadGUI(thread, i);
						}
						GUILayout.EndHorizontal();
					}
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
		}

		private void DrawThreadGUI(VirtualMachineThread* ptr, int id)
		{
			GUILayout.BeginVertical();
			{
				GUILayout.Label($"Address: 0x{new IntPtr(ptr).ToString("X")}");
				GUILayout.Label($"Is in await: {(ptr->awaitTaskPin != default).ToString()}");
				GUILayout.Label($"Script/Region: {ptr->scriptData->name.ToString()}/{ptr->regionData->name.ToString()}");
				GUILayout.Label($"Current offset: {ptr->offset.ToString()}");
				GUILayout.Label($"Save point offset: {ptr->savePoint.ToString()}");
				ptr->isFreezed = EditorGUILayout.Toggle("Is freezed", ptr->isFreezed);
				if (ptr->stack.stackOffset > 0)
				{
					DrawScriptValues($"Stack ({ptr->stack.stackOffset})", ptr->stack.Ptr, ptr->stack.stackOffset, ref _stacksFoldout[id]);
				}
				else
				{
					GUILayout.Label($"Stack (0)");
				}
				
				var paramsCount = 0;
				for (var i = 0; i < VirtualMachineThreadParametersStack.MaxParameters; i++)
				{
					if (ptr->parametersStack.BeginPtr[i].type != ScriptValue.ValueType.Invalid)
						paramsCount++;
				}
				DrawScriptValues($"Parameters ({paramsCount.ToString()})", ptr->parametersStack.BeginPtr, paramsCount, ref _paramsFoldout[id]);
				
				_registersFoldout[id] = EditorGUILayout.Foldout(_registersFoldout[id], "Registers");
				if (_registersFoldout[id])
				{
					GUILayout.BeginHorizontal();
					{
						GUILayout.Space(25);
						GUILayout.BeginHorizontal();
						{
							for (var i = 0; i < VirtualMachineThreadRegisters.RegistersCount; i++)
							{
								GUILayout.Label($"R{i.ToString()}: {ptr->threadRegisters[i].ToString()}");
								GUILayout.Space(25);
							}
						}
						GUILayout.EndHorizontal();
					}
					GUILayout.EndHorizontal();
				}
			}
			GUILayout.EndVertical();
		}
	}
}
#endif