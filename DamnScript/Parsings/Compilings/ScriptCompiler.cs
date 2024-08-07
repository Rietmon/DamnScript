using System;
using System.IO;
using DamnScript.Parsings.Serializations;
using DamnScript.Runtimes.Cores.Types;

namespace DamnScript.Parsings.Compilings
{
    public static unsafe class ScriptCompiler
    {
        public const int Version = 1;

        public static void Compile(Stream input, StringWrapper name, Stream output) =>
            Compile(input, name.ToString32(), output);
        
        public static void Compile(Stream input, String32 name, Stream output)
        {
            var scriptData = ScriptsDataManager.LoadScript(input, name);
        
            var stream = new SerializationStream(1024);
        
            stream.Write(Version);
            stream.Write(scriptData.value->regions.Length);
            {
                var begin = scriptData.value->regions.Begin;
                var end = scriptData.value->regions.End;
                while (begin < end)
                {
                    stream.Write(begin->name);
                    stream.Write(begin->byteCode);
                    begin++;
                }
            }

            var span = new ReadOnlySpan<byte>(stream.start, stream.length);
            output.Write(span);
            stream.Dispose();
        }
    }
}