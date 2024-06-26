using System.Diagnostics;
using DamnScript.Parsings;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines;
using DamnScript.Runtimes.VirtualMachines.Threads;

public static class Program
{
    public static unsafe void Main()
    {
    }

    public static void Print(ScriptValue value)
    {
        Console.WriteLine(value.longValue);
    }
}