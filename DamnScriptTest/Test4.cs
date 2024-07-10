using System.Runtime.InteropServices;
using System.Text;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.Datas;

namespace DamnScriptTest
{
    public static class Test4
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
    
        public static ScriptValue GetObject(ScriptValue index)
        {
            return ScriptValue.FromReferencePin(objects[index.longValue]);
        }
    
        public static void Run()
        {
            ScriptEngine.RegisterNativeMethod(GetObject);
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
            var scriptData = ScriptEngine.LoadScript(memoryStream, "Test4");
            var disassembler = Disassembler.DisassembleToString(scriptData.RefValue.regions[0].byteCode, scriptData.RefValue.metadata);
            Console.WriteLine(disassembler);
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