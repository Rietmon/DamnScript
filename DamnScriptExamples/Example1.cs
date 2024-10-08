﻿using System.Text;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Debugs;

namespace DamnScriptExamples
{
    public static class Example1
    {
        private const string Code = @"
        region Main
        {
            Print(""Hello from DamnScript!"");
        }
";
    
        public static void Run()
        {
            // Open the stream with script code
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Code));
            // Load the script data from the stream and set it the name "Example1"
            var scriptData = ScriptEngine.LoadScript(memoryStream, "Example1");
            Shared.PrintDisassembly(scriptData);
            // Run the script thread, which is going to start to execute a region with the name "Main"
            var thread = ScriptEngine.RunThread(scriptData, "Main");
        
            Console.Write("\n");
            // Execute the scheduler until the thread is finished,
            // ScriptEngine.ExecuteScheduler() should be placed in the game loop or in the main loop of the application
            while (ScriptEngine.ExecuteScheduler())
                Thread.Sleep(15);
            Console.Write("\n");
        
            // Unload the script data, because we don't need it anymore
            // In your application, you should unload the script data when YOU don't need it anymore, not when it finishes
            ScriptEngine.UnloadScript(scriptData);
        
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}