using System.Text;
using DamnScript.Runtimes;
using DamnScript.Runtimes.VirtualMachines.ScriptValues;

namespace DamnScriptExamples.NET
{
    public static class Example2
    {
        private const string Code = @"
        region Main
        {
            Log(GetString());
        }
";

        // Every method should operate ONLY with ScriptValuePtr as a parameter or return value.
        // Except: Task and Task<ScriptValuePtr> - for async methods.
        // This is a wrapper for each one of the types that can be passed to/from the script.
        // You can pass managed reference, pointer, string, number, etc.
        // A return type also should be a ScriptValuePtr.
        // Because of return value is a pointer, you can create structure from ScriptValue and call "Return()" method.
        // If you are about to return any managed value (string, class reference) more safely to use "Pin".
        // Especially if you are using async and ESPECIALLY if you are using .NET
        // If it is pinned will auto unpin it after using as argument in the next method.
        // You should never return value and not use it in the next method!
        public static ScriptValuePtr GetString()
        {
            return ScriptValue.FromReferencePin("Hello from C#!").Return();
        }
    
        public static void Run()
        {
            // Register the native method GetString
            ScriptEngine.RegisterNativeMethod(GetString);
            
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Code));
            var scriptData = ScriptEngine.LoadScript(memoryStream, "Example2");
            Shared.PrintDisassembly(scriptData);
            var thread = ScriptEngine.RunThread(scriptData, "Main");
        
            Console.Write("\n");
            // In previous example, we used Thread.Sleep(15) and while loop.
            // This is not necessary while code doesn't use async methods.
            // In the wait loop, we are waiting for the async method finished.
            // In other ways engine will execute thread until it finished.
            ScriptEngine.ExecuteVirtualMachineNext();
            Console.Write("\n");
        
            ScriptEngine.UnloadScript(scriptData);
        
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}