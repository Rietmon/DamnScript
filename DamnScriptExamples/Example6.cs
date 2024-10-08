﻿using System.Runtime.InteropServices;
using System.Text;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.Datas;

namespace DamnScriptExamples
{
    public static class Example6
    {
        private const string Code = @"
        region Main
        {
            while (CanHandle()) {
                Print(GetCounter());
            }
        }
";

        private static int counter;
        public static ScriptValue CanHandle() => ++counter < 10;
        public static ScriptValue GetCounter() => counter;
    
        public static void Run()
        {
            ScriptEngine.RegisterNativeMethod(CanHandle);
            ScriptEngine.RegisterNativeMethod(GetCounter);
            
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Code));
            var scriptData = ScriptEngine.LoadScript(memoryStream, "Example6");
            Shared.PrintDisassembly(scriptData);
            var thread = ScriptEngine.RunThread(scriptData, "Main");
        
            Console.Write("\n");
            while (ScriptEngine.ExecuteScheduler())
                Thread.Sleep(15);
            Console.Write("\n");
        
            ScriptEngine.UnloadScript(scriptData);
        
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}