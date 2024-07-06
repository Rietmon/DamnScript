using DamnScript.Runtimes;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines;
using DamnScript.Runtimes.VirtualMachines.Threads;

namespace DamnScriptTest;

public static unsafe class Program
{
    public static void Main()
    {
        ScriptEngine.RegisterNativeMethod(Print);
        var file = File.ReadAllText(@"C:\Projects\DamnScript\_Binaries\Debug\net7.0\script.ds");
        var script = ScriptEngine.LoadScriptFromCode(file, "script");
        var str = Disassembler.DisassembleToString(script.value->regions.Begin->byteCode, script.value->metadata);
        Console.WriteLine(str);
        var thread = ScriptEngine.CreateThread(script, "Main");
        ScriptEngine.ExecuteScheduler();
        Console.ReadKey();
    }

    public static void Print(ScriptValue value)
    {
        var str = value.GetUnsafeString();
        var safeStr = str->ToString();
        Console.WriteLine(safeStr);
    }
}