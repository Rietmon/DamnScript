#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.VirtualMachines.Scripts;
using UnityEditor;
using UnityEngine;

namespace DamnScript.Extensions.Unity.Editings.Inspectors
{
	public unsafe partial class DSInspectorWindow
	{
		private bool[] _scriptsFoldout = Array.Empty<bool>();
		private bool[] _referencesFoldout = Array.Empty<bool>();
		private bool[] _metadatasFoldout = Array.Empty<bool>();
		private bool[] _constantsFoldout = Array.Empty<bool>();
		private bool[] _stringsFoldout = Array.Empty<bool>();
		private bool[] _methodsFoldout = Array.Empty<bool>();
		private bool[] _regionsFoldout = Array.Empty<bool>();
		private bool[][] _regionsDisassemblyFoldout = Array.Empty<bool[]>();
		
		private readonly Dictionary<(int, int), string> _regionsDisassemblyCache = new();

		private Vector2 _scriptsScrollPosition;

		private void OnGUIScripts()
		{
			if (_scriptsFoldout.Length != ScriptsStorage.scriptsStorage.Count)
				Array.Resize(ref _scriptsFoldout, ScriptsStorage.scriptsStorage.Count);
			if (_referencesFoldout.Length != ScriptsStorage.scriptsStorage.Count)
				Array.Resize(ref _referencesFoldout, ScriptsStorage.scriptsStorage.Count);
			if (_metadatasFoldout.Length != ScriptsStorage.scriptsStorage.Count)
				Array.Resize(ref _metadatasFoldout, ScriptsStorage.scriptsStorage.Count);
			if (_constantsFoldout.Length != ScriptsStorage.scriptsStorage.Count)
				Array.Resize(ref _constantsFoldout, ScriptsStorage.scriptsStorage.Count);
			if (_stringsFoldout.Length != ScriptsStorage.scriptsStorage.Count)
				Array.Resize(ref _stringsFoldout, ScriptsStorage.scriptsStorage.Count);
			if (_methodsFoldout.Length != ScriptsStorage.scriptsStorage.Count)
				Array.Resize(ref _methodsFoldout, ScriptsStorage.scriptsStorage.Count);
			if (_regionsFoldout.Length != ScriptsStorage.scriptsStorage.Count)
				Array.Resize(ref _regionsFoldout, ScriptsStorage.scriptsStorage.Count);
			if (_regionsDisassemblyFoldout.Length != ScriptsStorage.scriptsStorage.Count)
				Array.Resize(ref _regionsDisassemblyFoldout, ScriptsStorage.scriptsStorage.Count);

			_scriptsScrollPosition = GUILayout.BeginScrollView(_scriptsScrollPosition);
			GUILayout.Label($"Scripts allocated: {ScriptsStorage.scriptsStorage.Count.ToString()}", EditorStyles.boldLabel);

			GUILayout.BeginVertical();
			{
				for (var i = 0; i < ScriptsStorage.scriptsStorage.Count; i++)
				{
					var script = ScriptsStorage.scriptsStorage[i];
					_scriptsFoldout[i] =
						EditorGUILayout.Foldout(_scriptsFoldout[i], $"Script ({script.value->name.ToString()})");
					if (_scriptsFoldout[i])
					{
						GUILayout.BeginHorizontal();
						{
							GUILayout.Space(25);
							DrawScriptGUI(script.value, i);
						}
						GUILayout.EndHorizontal();
					}
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
		}

		private void DrawScriptGUI(ScriptData* ptr, int index)
		{
			GUILayout.BeginVertical();
			{
				if (BeginDrawAsFoldout($"References ({ptr->referencesCount.ToString()})", ref _referencesFoldout[index]))
				{
					var lastThread = 0;
					for (var i = 0; i < ptr->referencesCount; i++)
					{
						var threads = ScriptEngine.mainPtr.value->threads;
						for (var j = lastThread; j < threads.Length; j++)
						{
							var thread = threads.Begin + j;
							if (thread->isAlive && thread->scriptData == ptr)
							{
								GUILayout.Label($"<b>Ref {i.ToString()}</b>: Thread {j.ToString()}", _richTextStyle);
								lastThread = j + 1;
								break;
							}
						}
					}
				}
				EndDrawAsFoldout(ref _referencesFoldout[index]);
				DrawMetadata(&ptr->metadata, index);
				var regions = ptr->regions;
				if (BeginDrawAsFoldout($"Regions ({regions.Length.ToString()})", ref _regionsFoldout[index]))
				{
					for (var i = 0; i < ptr->regions.Length; i++)
					{
						var region = ptr->regions.Begin + i;
						if (_regionsDisassemblyFoldout[index] == null || _regionsDisassemblyFoldout[index].Length != ptr->regions.Length)
							Array.Resize(ref _regionsDisassemblyFoldout[index], ptr->regions.Length);
						_regionsDisassemblyFoldout[index][i] =
							EditorGUILayout.Foldout(_regionsDisassemblyFoldout[index][i], $"Region ({region->name.ToString()})");
						if (_regionsDisassemblyFoldout[index][i])
						{
							GUILayout.BeginHorizontal();
							{
								GUILayout.Space(25);
								if (_regionsDisassemblyCache.TryGetValue((index, i), out var disassembly))
								{
									GUILayout.Label(disassembly);
								}
								else
								{
									disassembly = ScriptDisassembler.DisassembleRegionToString(region, ptr->metadata);
									GUILayout.Label(disassembly);
									_regionsDisassemblyCache[(index, i)] = disassembly;
								}
							}
							GUILayout.EndHorizontal();
						}
					}
				}
				EndDrawAsFoldout(ref _regionsFoldout[index]);
			}
			GUILayout.EndVertical();
		}

		private void DrawMetadata(ScriptMetadata* ptr, int index)
		{
			if (BeginDrawAsFoldout("Metadata", ref _metadatasFoldout[index]))
			{
				if (BeginDrawAsFoldout("Constants", ref _constantsFoldout[index]))
				{
					if (BeginDrawAsFoldout("Strings", ref _stringsFoldout[index]))
					{
						for (var i = 0; i < ptr->constants.strings.Length; i++)
							GUILayout.Label($"<b>{i}</b>: {(ptr->constants.strings.Begin + i)->value->ToString()}", _richTextStyle);
					}
					EndDrawAsFoldout(ref _stringsFoldout[index]);
					GUILayout.Space(5);
					if (BeginDrawAsFoldout("Methods", ref _methodsFoldout[index]))
					{
						for (var i = 0; i < ptr->constants.methods.Length; i++)
							GUILayout.Label($"<b>{i}</b>: {(ptr->constants.methods.Begin + i)->value->ToString()}", _richTextStyle);
					}
					EndDrawAsFoldout(ref _methodsFoldout[index]);
				}
				EndDrawAsFoldout(ref _constantsFoldout[index]);
			}
			EndDrawAsFoldout(ref _metadatasFoldout[index]);
		}

		private void DrawRegion(RegionData* ptr, ScriptMetadata* metadata, int parentIndex, int index)
		{
			if (BeginDrawAsFoldout("Disassembly", ref _regionsDisassemblyFoldout[parentIndex][index]))
			{
				
			}
			EndDrawAsFoldout(ref _regionsDisassemblyFoldout[parentIndex][index]);
		}
	}
}
#endif