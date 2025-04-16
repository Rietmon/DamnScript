using System;
using System.IO;
using DamnScript.Parsings.Serializations;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Cores.Strings;

namespace DamnScript.Parsings.Compilings
{
    public static unsafe class ScriptCompiler
    {
        public const int Version = 1;
        
        public static void Compile(Stream input, String32 name, Stream output)
        {
            var scriptData = ScriptsStorage.LoadScript(input, name);
        
            var stream = new SerializationStream(1024);
        
            stream.Write(Version);
            stream.Write(scriptData.value->regions.Length);
            {
                var begin = scriptData.value->regions.Begin;
                var end = scriptData.value->regions.End;
                while (begin < end)
                {
                    stream.Write(begin->name);
                    
                    stream.Write(begin->byteCode.length);
                    stream.CustomWrite(begin->byteCode.start, begin->byteCode.length);
                    begin++;
                }
            }
            
            var constants = scriptData.value->metadata.constants;
            
            stream.WriteNativeStringArray(constants.strings);
            
            stream.WriteNativeStringArray(constants.methods);

            var span = new ReadOnlySpan<byte>(stream.start, stream.length);
            output.Write(span);
            output.Flush();
            output.Dispose();
            stream.Dispose();
        }
    }
}