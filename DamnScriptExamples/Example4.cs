using System.Runtime.InteropServices;
using System.Text;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.Datas;

namespace DamnScriptExamples
{
    public static class Example4
    {
        private const string Code = @"
        region Main
        {
            TestManagedPrint(GetObject(0));
            TestManagedPrint(GetObject(1));
        }
";

        public class TestOop
        {
            public string name = "test name";
            public int age = 20;
            public TestOop parent;
        
            public void TestManagedPrint()
            {
                Console.WriteLine($"Hello! I'm {name}, {age} y.o.");
                if (parent != null)
                    Console.WriteLine($"My parent is {parent.name}");
            }
        }

        public static TestOop[] objects = new TestOop[2];
    
        // As I said before the safest way to pass the object to the script is to pass it as a pinned reference.
        // But you can pass it as a pointer, but you should be careful with it especially for .NET Core.
        public static ScriptValue GetObject(ScriptValue index)
        {
            return ScriptValue.FromReferencePin(objects[index.longValue]);
        }
    
        public static void Run()
        {
            ScriptEngine.RegisterNativeMethod(GetObject);
            // In C# every method can be present as a static and called from anywhere.
            // But you should pass the object as a first parameter. It can be any pointer or "Object"
            // Registering as present in the example below will allow us to invoke method for different objects.
            // If we pass as a delegate from exact object, it will be allocated one more special method.
            // For this method you don't need to pass object pointer, it did C# for you.
            ScriptEngine.RegisterNativeMethod(typeof(TestOop).GetMethod(nameof(TestOop.TestManagedPrint)));

            objects[0] = new TestOop
            {
                name = "John",
                age = 25
            };
            objects[1] = new TestOop
            {
                name = "Jane",
                age = 2,
                parent = objects[0]
            };
        
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Code));
            var scriptData = ScriptEngine.LoadScript(memoryStream, "Example4");
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