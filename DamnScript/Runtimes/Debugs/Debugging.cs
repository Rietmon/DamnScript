using System;
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace DamnScript.Runtimes.Debugs
{
    public static class Debugging
    {
        public static void Log(string message) =>
#if UNITY_5_3_OR_NEWER
            Debug.Log(message);
#else
            WriteToConsole(message, ConsoleColor.White);
#endif
        public static void LogWarning(string message) => 
#if UNITY_5_3_OR_NEWER
            Debug.LogWarning(message);
#else
            WriteToConsole(message, ConsoleColor.Yellow);
#endif
        public static void LogError(string message) => 
#if UNITY_5_3_OR_NEWER
            Debug.LogError(message);
#else
            WriteToConsole(message, ConsoleColor.Red);
#endif
    
        private static void WriteToConsole(string message, ConsoleColor color)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = previousColor;
        }
    }
}