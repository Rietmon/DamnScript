using System.Diagnostics;
using DamnScript.Parsings;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines;
using DamnScript.Runtimes.VirtualMachines.Threads;

public static unsafe class Program
{
    public static void Main()
    {
        VirtualMachine.RegisterNativeMethod(Print);
        
        var file = File.ReadAllText("C:\\Projects\\DamnScript\\DamnScript\\Parsings\\G4\\ds.ds");
        var scriptData = ScriptParser.ParseScript(file);
        
        var thread = new VirtualMachineThread(&scriptData.regions.Data[0], &scriptData.metadata);
        var scheduler = new VirtualMachineScheduler();
        scheduler.Register((IntPtr)(&thread));
        
        while (scheduler.HasThreads)
            scheduler.ExecuteNext();
    }

    public static void Print(ScriptValue value)
    {
        Console.WriteLine(value.GetUnsafeString()->ToString());
    }
}