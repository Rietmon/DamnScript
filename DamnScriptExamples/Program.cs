using System.Text;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Types;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.Threads;

namespace DamnScriptExamples
{
    public static class Program
    {
        public static void Print(ScriptValuePtr value)
        {
            Console.WriteLine($"DAMN SCRIPT: {value.RefValue.ToString()}");
        }
        
        public static ScriptValuePtr MakeString(ScriptValuePtr value)
        {
            return ScriptValue.FromReferenceUnsafe("Hello, " + value.RefValue).Return();
        }
        
        public static async Task<ScriptValuePtr> MakeStringAsync(ScriptValuePtr value)
        {
            var result = "HELLO, " + value.RefValue;
            await Task.Delay(1000);
            result += "!!!";
            return ScriptValue.FromReferenceUnsafe(result).Return();
        }
    
        public static void Main()
        {
            ScriptEngine.RegisterNativeMethod(Print);
            ScriptEngine.RegisterNativeMethod(MakeString);
            ScriptEngine.RegisterNativeMethod(MakeStringAsync);

            var code = @"
                        region Main {
                            Print(MakeString(""WORLD""));
                            Print(MakeStringAsync(""WORLD""));
                        }
                        ";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
            var scriptData = ScriptEngine.LoadScript(stream, "Main");
            var thread = ScriptEngine.RunThread(scriptData, "Main");
            while (ScriptEngine.ExecuteVirtualMachineNext())
                Thread.Sleep(10);
            ScriptEngine.UnloadScript(scriptData);
            return;

            Begin();
        }

        public static void Begin()
        {
            while (true)
            {
                Console.WriteLine("---------------------------------");
                Console.WriteLine("Hello! Choose a test:");
                Console.WriteLine("1: Print string from");
                Console.WriteLine("2: Print dynamic value");
                Console.WriteLine("3: Print with delay from DamnScript + async/await + difficulty expression");
                Console.WriteLine("4: Print non static method from object instance");
                Console.WriteLine("5: Print from \"for\" loop with indexing");
                Console.WriteLine("6: Print from \"while\" loop with counter");
                Console.WriteLine("7: If-elseif-else statement");
                Console.WriteLine("8: Save point");
                Console.WriteLine("Q: Exit");
                Console.WriteLine("---------------------------------");
            
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1": Example1.Run(); break;
                    case "2": Example2.Run(); break;
                    case "3": Example3.Run(); break;
                    case "4": Example4.Run(); break;
                    case "5": Example5.Run(); break;
                    case "6": Example6.Run(); break;
                    case "7": Example7.Run(); break;
                    case "8": Example8.Run(); break;
                    case "Q": return;
                }
            }
        }
    }
}