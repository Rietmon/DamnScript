#if UNITY_EDITOR
using System;
using System.Globalization;
using DamnScript.Runtimes.VirtualMachines.ScriptValues;
using UnityEditor;
using UnityEngine;

namespace DamnScript.Extensions.Unity.Editings.Inspectors
{
	public unsafe partial class DSInspectorWindow : EditorWindow
	{
		private static GUIStyle _listButtonStyle;
		private static GUIStyle _richTextStyle;

		private Tabs _currentTab;

		private void OnGUI()
		{
			CreateStyles();
			var listPartWidth = Mathf.Min(position.width / 3, 100);
			GUILayout.BeginHorizontal();
			{
				GUILayout.BeginVertical(GUILayout.Width(listPartWidth));
				{
					if (DrawListButton(Tabs.Threads))
					{
						_currentTab = Tabs.Threads;
					}
					else if (DrawListButton(Tabs.Scripts))
					{
						_currentTab = Tabs.Scripts;
					}
					else if (DrawListButton(Tabs.Methods))
					{
						_currentTab = Tabs.Methods;
					}
					else if (DrawListButton(Tabs.Pins))
					{
						_currentTab = Tabs.Pins;
					}
				}
				GUILayout.EndVertical();
				var rightPartWidth = position.width - listPartWidth;
				GUILayout.BeginHorizontal(GUILayout.Width(rightPartWidth));
				{
					switch (_currentTab)
					{
						case Tabs.Threads:
							OnGUIThreads();
							break;
						case Tabs.Scripts:
							OnGUIScripts();
							break;
						case Tabs.Methods:
							OnGUIMethods();
							break;
						case Tabs.Pins:
							OnGUIPins();
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndHorizontal();
		}

		private bool DrawListButton(Tabs tab)
		{
			var previousColor = GUI.color;
			var isSelected = _currentTab == tab;
			GUI.color = isSelected ? Color.gray : Color.white;
			var button = GUILayout.Button(tab.ToString(), _listButtonStyle, GUILayout.ExpandWidth(true));
			GUI.color = previousColor;
			return button;
		}
		
		private void DrawInfo(string name, string value)
		{
			GUILayout.Label($"<b>{name}</b>: {value}", _richTextStyle);
		}

		private void DrawScriptValues(string name, ScriptValue* values, int size, ref bool foldout)
		{
			if (BeginDrawAsFoldout(name, ref foldout))
			{
				for (var i = 0; i < size; i++)
					DrawScriptValue(values + i);
			}
			EndDrawAsFoldout(ref foldout);
		}

		private bool BeginDrawAsFoldout(string name, ref bool foldout)
		{
			foldout = EditorGUILayout.Foldout(foldout, name);
			if (!foldout)
				return foldout;

			GUILayout.BeginHorizontal();
			GUILayout.Space(25);
			GUILayout.BeginVertical();
			return foldout;
		}

		private void EndDrawAsFoldout(ref bool foldout)
		{
			if (!foldout)
				return;
			
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}

		private void DrawScriptValue(ScriptValue* ptr)
		{
			switch (ptr->type)
			{
				case ScriptValue.ValueType.Invalid:
					GUILayout.Label("Invalid");
					break;
				case ScriptValue.ValueType.Integer:
					GUILayout.Label($"Int ({ptr->rawLong.ToString()})");
					break;
				case ScriptValue.ValueType.Float32:
					GUILayout.Label($"Float32 ({ptr->rawFloat.ToString(CultureInfo.InvariantCulture)})");
					break;
				case ScriptValue.ValueType.Float64:
					GUILayout.Label($"Double64 ({ptr->rawDouble.ToString(CultureInfo.InvariantCulture)})");
					break;
				case ScriptValue.ValueType.Pointer:
					GUILayout.Label($"Ptr: ({new IntPtr(ptr->rawPointerValue).ToString("X")})");
					break;
				case ScriptValue.ValueType.FreedPointer:
					GUILayout.Label($"FREED Ptr: ({new IntPtr(ptr->rawPointerValue).ToString("X")})");
					break;
				case ScriptValue.ValueType.NativeStringPointer:
					GUILayout.Label($"NativeStrPtr: ({ptr->ToString()})");
					break;
				case ScriptValue.ValueType.ReferenceUnsafePointer:
					GUILayout.Label($"UnsafeRef: ({ptr->GetReference<object>()})");
					break;
				case ScriptValue.ValueType.ReferenceSafePointer:
					GUILayout.Label($"SafeRef: ({ptr->GetReference<object>()})");
					break;
				case ScriptValue.ValueType.ReferencePersistentSafePointer:
					GUILayout.Label($"PERSISTENT SafeRef: ({ptr->GetReference<object>()})");
					break;
				case ScriptValue.ValueType.ReferenceUnpinnedSafePointer:
					GUILayout.Label($"UNPINNED SafeRef: ({ptr->GetReference<object>()})");
					break;
				default:
					GUILayout.Label("Unknown");
					break;
			}
		}

		private static void CreateStyles()
		{
			_listButtonStyle ??= new GUIStyle(EditorStyles.toolbarButton)
			{
				alignment = TextAnchor.MiddleLeft,
				fixedHeight = 25
			};
			_richTextStyle ??= new GUIStyle(EditorStyles.label)
			{
				richText = true,
			};
		}

		[MenuItem("Window/DamnScript/DSInspector")]
		public static void OpenWindow()
		{
			var window = GetWindow<DSInspectorWindow>();
			window.titleContent = new GUIContent("DSInspector");
			window.minSize = new Vector2(500, 200);
			window.Show();
		}

		public enum Tabs
		{
			Threads,
			Scripts,
			Methods,
			Pins
		}
	}
}
#endif