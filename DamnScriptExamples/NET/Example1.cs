using System.Text;
using DamnScript.Runtimes;

namespace DamnScriptExamples.NET
{
    public static class Example1
    {
        private const string Code = @"
        region Main
        {
            Log(""Hello from DamnScript!"");
        }
";
    
        public static void Run()
        {
            // Open the stream with script code.
            // It can be any another stream
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Code));
            
            // Load the script data from the stream and set it the name "Example1".
            // This name will be used for cache and can be got after just by name without loading the script again
            var scriptData = ScriptEngine.LoadScript(memoryStream, "Example1");
            
            // Just for example, print the output bytecode
            Shared.PrintDisassembly(scriptData);
            
            // Run the script thread, which is going to start to execute a region with the name "Main".
            // Returned a handler for thread.
            // There is no need to control it manually, but you can get info about it and kill it
            var thread = ScriptEngine.RunThread(scriptData, "Main");
        
            Console.Write("\n");
            
            // Execute the scheduler until the thread is finished,
            // ScriptEngine.ExecuteVirtualMachineNext() should be placed in the game loop or in the main loop of the application
            // Do not use Thread.Sleep in Unity!
            // Scheduler works differently in Unity and .NET!
            while (ScriptEngine.ExecuteVirtualMachineNext())
                Thread.Sleep(15);
            Console.Write("\n");
        
            // Unload the script data, because we don't need it anymore
            // In your application, you can unload the script data when YOU don't need it anymore, not when it finishes.
            ScriptEngine.UnloadScript(scriptData);
        
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}