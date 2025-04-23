using System;
using System.Reflection;
using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores;

namespace DamnScript.Runtimes.VirtualMachines.Methods
{
	public static unsafe class MethodInfoUtilities
	{
#if !DAMN_SCRIPT_DISABLE_RAW_METHOD_INFO
		private static readonly MethodInfo getParametersNoCopyMethodInfo;
		
		static MethodInfoUtilities()
		{
			var runtimeMethodInfoType = Type.GetType("System.Reflection.RuntimeMethodInfo");
			if (runtimeMethodInfoType == null)
				throw new Exception("Type \"System.Reflection.RuntimeMethodInfo\" not found!");
            
			getParametersNoCopyMethodInfo = runtimeMethodInfoType.GetMethod("GetParametersNoCopy", BindingFlags.NonPublic | BindingFlags.Instance);
			if (getParametersNoCopyMethodInfo == null)
				throw new Exception("Method \"GetParametersNoCopy\" not found!");
		}
#endif

		public static ParameterInfo[] GetParameters(MethodInfo method)
		{
#if !DAMN_SCRIPT_DISABLE_RAW_METHOD_INFO
			return GetParametersNoCopy(method);
#else
			return GetParametersCopy(method);
#endif
		}
		
#if !DAMN_SCRIPT_DISABLE_RAW_METHOD_INFO
		public static ParameterInfo[] GetParametersNoCopy(MethodInfo method) => 
			(ParameterInfo[])getParametersNoCopyMethodInfo.Invoke(method, null);
#endif

#if DAMN_SCRIPT_DISABLE_RAW_METHOD_INFO
		public static ParameterInfo[] GetParametersCopy(MethodInfo method) =>
			method.GetParameters();
#endif

#if DAMN_SCRIPT_ENABLE_IL2CPP
		[StructLayout(LayoutKind.Sequential)]
		private struct UnmanagedMonoMethodInfo
		{
			public UnsafeUtilities.UnmanagedClassHeader header;
			public void* methodPointer;
		}
#endif

		public static void* GetFunctionPointer(MethodInfo method)
		{
#if DAMN_SCRIPT_ENABLE_IL2CPP
			var info = (UnmanagedMonoMethodInfo*)UnsafeUtilities.ReferenceToPointer(method);
			return info->methodPointer;
#else
			return method.MethodHandle.GetFunctionPointer().ToPointer();
#endif
		}
	}
}