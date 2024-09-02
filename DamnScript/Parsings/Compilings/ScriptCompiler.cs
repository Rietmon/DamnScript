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
            
            var constants = scriptData.value->metadata.constants;
            stream.Write(constants.strings.Length);
            {
                var begin = constants.strings.Begin;
                var end = constants.strings.End;
                while (begin < end)
                {
                    stream.Write(begin->value->length);
                    stream.CustomWrite(begin->value->data, begin->value->length);
                    begin++;
                }
            }
            
            stream.Write(constants.methods.Length);
            {
                var begin = constants.methods.Begin;
                var end = constants.methods.End;
                while (begin < end)
                {
                    stream.Write(begin->value->length);
                    stream.CustomWrite(begin->value->data, begin->value->length);
                    begin++;
                }
            }

            var span = new ReadOnlySpan<byte>(stream.start, stream.length);
            output.Write(span);
            stream.Dispose();
        }
    }
}