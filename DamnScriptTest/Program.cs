using DamnScript.Runtimes;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Natives;

namespace DamnScriptTest;

public static unsafe class Program
{
    public static void Main()
    {
        ScriptEngine.RegisterNativeMethod(Print);
        var file = File.Open(@"C:\Projects\DamnScript\_Binaries\Debug\net7.0\script.ds", FileMode.Open);
        var script = ScriptEngine.LoadScript(file, "script");
        var str = Disassembler.DisassembleToString(script.value->regions.Begin->byteCode, script.value->metadata);
        Console.WriteLine(str);
        var thread = ScriptEngine.CreateThread(script, "Main");
        ScriptEngine.ExecuteScheduler();
        thread.RefValue.Dispose();
    }

    public static void Print(ScriptValue value)
    {
        var str = value.GetUnsafeString();
        var safeStr = str->ToString();
        Console.WriteLine(safeStr);
    }
}