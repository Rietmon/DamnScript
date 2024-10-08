﻿using System.Runtime.InteropServices;
using System.Text;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.Datas;

namespace DamnScriptExamples
{
    public static class Example5
    {
        private const string Code = @"
        region Main
        {
            for (i in 5) {
                for (j in 5) {
                    Print((i + 1) * (j + 1));
                }
            }
        }
";
    
        public static void Run()
        {
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Code));
            var scriptData = ScriptEngine.LoadScript(memoryStream, "Example5");
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