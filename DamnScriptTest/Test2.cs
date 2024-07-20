using System.Text;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Natives;

namespace DamnScriptTest
{
    public static class Test2
    {
        private const string Code = @"
        region Main
        {
            Print(GetString());
        }
";

        // Every method should operate ONLY with ScriptValue.
        // This is a wrapper for each one of the types that can be passed to/from the script.
        // You can pass managed reference, pointer, string, number, boolean etc.
        // Return type also should be a ScriptValue.
        // Because of string is a reference type, we should Pin it to prevent the garbage collector from collecting it.
        // ScriptValue.ToSafeString() is a safe way to get the string from the ScriptValue.
        // If it pinned will auto unpin it.
        // P.S. Every value which is used from stack will be unpinned automatically.
        public static ScriptValue GetString()
        {
            return ScriptValue.FromReferencePin("Hello from C#!");
        }
    
        public static void Run()
        {
            // Register the native method GetString
            ScriptEngine.RegisterNativeMethod(GetString);
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Code));
            var scriptData = ScriptEngine.LoadScript(memoryStream, "Test2");
            var thread = ScriptEngine.RunThread(scriptData, "Main");
        
            Console.Write("\n");
            ScriptEngine.ExecuteScheduler();
            Console.Write("\n");
        
            ScriptEngine.UnloadScript(scriptData);
        
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}