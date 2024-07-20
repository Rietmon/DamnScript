using DamnScript.Runtimes;
using DamnScript.Runtimes.Natives;

namespace DamnScriptTest
{
    public static class Program
    {
        public static void Print(ScriptValue value)
        {
            var str = value.ToSafeString();
            var safeStr = str.ToString();
            Console.WriteLine($"DAMN SCRIPT: {safeStr}");
        }
    
        public static void Main()
        {
            ScriptEngine.RegisterNativeMethod(Print);

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
                Console.WriteLine("Q: Exit");
                Console.WriteLine("---------------------------------");
            
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1": Test1.Run(); break;
                    case "2": Test2.Run(); break;
                    case "3": Test3.Run(); break;
                    case "4": Test4.Run(); break;
                    case "5": Test5.Run(); break;
                    case "6": Test6.Run(); break;
                    case "Q": return;
                }
            }
        }
    }
}