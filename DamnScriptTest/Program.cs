using DamnScript.Parsings;
using DamnScript.Runtimes.VirtualMachines;
using DamnScript.Runtimes.VirtualMachines.Assemblers;
using DamnScript.Runtimes.VirtualMachines.Threads;

public static class Program
{
    public static unsafe void Main()
    {
        var file = File.ReadAllText("C:/Projects/DamnScript/DamnScript/Parsings/G4/ds.ds");
        var scriptData = ScriptParser.ParseScript(file);

        Console.ReadKey();
    }
    
    public static void Print()
    {
        Console.WriteLine("Print");
    }
    
    public static async Task Wait()
    {
        await Task.Delay(1000);
    }
}