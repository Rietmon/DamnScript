using DamnScript.Parsings;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines;
using DamnScript.Runtimes.VirtualMachines.Assemblers;
using DamnScript.Runtimes.VirtualMachines.Threads;

public static class Program
{
    public static unsafe void Main()
    {
        VirtualMachine.RegisterNativeMethod(Print);
        
        var file = File.ReadAllText("C:/Projects/DamnScript/DamnScript/Parsings/G4/ds.ds");
        var scriptData = ScriptParser.ParseScript(file);
        
        var scheduler = new VirtualMachineScheduler();
        var thread = new VirtualMachineThread(scriptData.regions.byteCode);
        var bytes = new byte[scriptData.regions.byteCode.length];
        for (var i = 0; i < scriptData.regions.byteCode.length; i++)
        {
            bytes[i] = scriptData.regions.byteCode.start[i];
        }
        File.WriteAllBytes("output.dsc", bytes);
        scheduler.Register((IntPtr)(&thread));
        while (scheduler.HasThreads)
            scheduler.ExecuteNext();
    }
    
    public static void Print(ScriptValue value)
    {
        Console.WriteLine(value.longValue);
    }
}