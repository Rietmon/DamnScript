using DamnScript.Parsings.Serializations;

namespace DamnScript.Parsings.Compilings;

public static unsafe class ScriptCompiler
{
    public const int Version = 1;

    public static void CompileScriptToFile(string input, string output)
    {
        var fileName = Path.GetFileNameWithoutExtension(output);
        var scriptCode = File.ReadAllText(input);
        var scriptData = ScriptsDataManager.LoadScriptFromCode(scriptCode, fileName);
        
        var stream = new SerializationStream(1024);
        
        stream.Write(Version);
        stream.Write(scriptData.value->regions.Length);
        {
            var begin = scriptData.value->regions.Begin;
            var end = scriptData.value->regions.End;
            while (begin < end)
            {
                stream.Write(begin->name);
                stream.Write(begin->name);
                stream.Write(begin->byteCode);
                begin++;
            }
        }
    }
}