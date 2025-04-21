using System;
using System.Runtime.CompilerServices;
using System.Text;
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace DamnScript.Runtimes.Debugs
{
    public static unsafe class Debugging
    {
	    public static Action<(LogType type, string message)> OnLog { get; set; }

	    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	    public static void Log(string message)
	    {
#if UNITY_5_3_OR_NEWER
            Debug.Log(message);
#else
		    WriteToConsole(message, ConsoleColor.White);
#endif
		    
		    OnLog?.Invoke((LogType.Log, message));
	    }
	    
	    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	    public static void LogWarning(string message)
	    {
#if UNITY_5_3_OR_NEWER
            Debug.LogWarning(message);
#else
		    WriteToConsole(message, ConsoleColor.Yellow);
#endif
		    
		    OnLog?.Invoke((LogType.Warning, message));
	    }
	    
	    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	    public static void LogError(string message)
	    {
#if UNITY_5_3_OR_NEWER
            Debug.LogError(message);
#else
		    WriteToConsole(message, ConsoleColor.Red);
#endif
		    
		    OnLog?.Invoke((LogType.Error, message));
	    }
	    
	    public static string DumpMemory(void* start, int length)
	    {
		    var memory = (byte*)start;
		    var dump = new StringBuilder();
		    for (var i = 0; i < length; i++)
		    {
			    if (i % 16 == 0)
				    dump.Append('\n');
			    dump.Append($"{memory[i]:X2} ");
		    }
		    return dump.ToString();
	    }
    
        private static void WriteToConsole(string message, ConsoleColor color)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = previousColor;
        }

        public enum LogType
        {
	        Log,
	        Warning,
	        Error
        }
    }
}