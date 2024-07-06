using System.Text;

namespace DamnScript.Runtimes.Debugs;

public static unsafe class Dumper
{
    public static string DumpMemory(void* start, int length)
    {
        var memory = (byte*)start;
        var dump = new StringBuilder();
        for (var i = 0; i < length; i++)
        {
            if (i % 16 == 0)
                dump.Append('\n');
            dump.Append($"{memory[i]:X2} ");
        }
        return dump.ToString();
    }
}