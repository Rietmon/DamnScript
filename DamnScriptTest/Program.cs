using DamnScript.Runtimes;
using DamnScript.Runtimes.Natives;

namespace DamnScriptTest;

public static class Program
{
    public static void Print(ScriptValue value)
    {
        var str = value.GetSafeString();
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
            Console.WriteLine("1: Print string from DamnScript");
            Console.WriteLine("2: Print dynamic value from DamnScript");
            Console.WriteLine("Q: Exit");
            Console.WriteLine("---------------------------------");
            
            var input = Console.ReadLine();

            switch (input)
            {
                case "1": Test1.Run(); break;
                case "2": Test2.Run(); break;
                case "Q": return;
            }
        }
    }
}