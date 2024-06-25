namespace DamnScript.Debugs;

public static unsafe class Dumper
{
    public static string DumpMemory(void* start, int length)
    {
        var memory = (byte*)start;
        var dump = string.Empty;
        for (var i = 0; i < length; i++)
        {
            if (i % 16 == 0)
                dump += "\n";
            dump += $"{memory[i]:X2} ";
        }
        return dump;
    }
}