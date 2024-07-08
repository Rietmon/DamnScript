using System.Text;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.Datas;

namespace DamnScriptTest;

public static class Test4
{
    private const string Code = @"
        region Main
        {
            TestOopPrint(GetObject(Test(0)));
            TestOopPrint(GetObject(Test(1)));
        }
";

    public class TestOop
    {
        public string name = "test name";
        public int age = 20;
    }

    public static TestOop[] objects = new TestOop[2];
    
    public static ScriptValue GetObject(ScriptValue index)
    {
        return ScriptValue.FromReference(objects[index.longValue]);
    }
    
    public static ScriptValue Test(ScriptValue value)
    {
        return value;
    }
        
    public static void TestOopPrint(ScriptValue value)
    {
        var obj = value.GetReference<TestOop>();
        Console.WriteLine($"Hello! I'm {obj.name}, {obj.age} y.o.");
    }
    
    public static void Run()
    {
        VirtualMachineData.RegisterNativeMethod(GetObject);
        VirtualMachineData.RegisterNativeMethod(TestOopPrint);
        VirtualMachineData.RegisterNativeMethod(Test);

        objects[0] = new TestOop
        {
            name = "John",
            age = 25
        };
        objects[1] = new TestOop
        {
            name = "Jane",
            age = 22
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