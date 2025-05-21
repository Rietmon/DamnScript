#if UNITY_EDITOR
using System;
using System.Reflection;
using System.Text;
using DamnScript.Runtimes;
using DamnScript.Runtimes.BuiltIns;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.VirtualMachines;
using DamnScript.Runtimes.VirtualMachines.ScriptValues;
using DamnScript.Runtimes.VirtualMachines.Threads;
using UnityEditor;
using UnityEngine;

namespace DamnScript.Extensions.Unity.Editings.Inspectors
{
	public unsafe partial class DSInspectorWindow
	{
		private Vector2 _methodsScrollPosition;

		private void OnGUIMethods()
		{
			_methodsScrollPosition = GUILayout.BeginScrollView(_methodsScrollPosition);

			var methods = MethodsStorage.Methods;
			GUILayout.Label($"Methods registered: {methods.Count.ToString()}", EditorStyles.boldLabel);
			
			GUILayout.BeginVertical();
			{
				foreach (var method in methods)
				{
					var methodId = method.Key;
					var methodValue = method.Value;
					
					var methodName = methodId.name.ToString();
					var methodPointer = methodValue.methodPointer;

					var result =
						$"<b>{methodName}</b> | (0x{new IntPtr(methodPointer).ToString("X")}) | ({methodValue.argumentsCount.ToString()}) \n" +
						$"      IsAsync: {methodValue.IsAsync.ToString()} | IsStatic: {methodValue.IsStatic.ToString()} " +
						$"| HasReturnValue: {methodValue.HasReturnValue.ToString()}";
					
					if (BuiltInMethods.IsBuiltInMethod(methodName))
						result += " | <color=green>BuiltIn</color>";
					
					GUILayout.Label(result, _richTextStyle);
					GUILayout.Space(5);
				}
			}
			GUILayout.EndVertical();
			
			GUILayout.EndScrollView();
		}
	}
}
#endif