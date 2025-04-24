using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Cores.Strings;
using DamnScript.Runtimes.VirtualMachines.ScriptValues;

namespace DamnScriptExamples.NET
{
    public static class Program
    {
        public static void test(ScriptValuePtr a, ScriptValuePtr b)
        {
            Console.WriteLine($"a: {a}, b: {b}");
        }
        
        public static void Main()
        {
            var code = @"
            region Main {
            test(1, 2);
}
";
            ScriptEngine.RegisterNativeMethod(test);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
            var script = ScriptEngine.LoadScript(stream, "Main");
            ScriptEngine.RunThread(script, "Main");
            ScriptEngine.ExecuteVirtualMachineNext();
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
                Console.WriteLine("9: Hot reload");
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
                    case "9": Example9.Run(); break;
                    case "Q": return;
                }
            }
        }
    }
}