using System;
using System.IO;
using DamnScript.Parsings.Antlrs;
using DamnScript.Parsings.Serializations;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Strings;
using DamnScript.Runtimes.VirtualMachines;
using DamnScript.Runtimes.VirtualMachines.Scripts;

namespace DamnScript.Parsings.Compilings
{
    public static unsafe class ScriptCompiler
    {
        public static void Compile(Stream input, String32 name, Stream output)
        {
            var scriptData = new ScriptData();
            ScriptParser.ParseScript(input, name, &scriptData);
            
            var stream = new SerializationStream(1024);
            stream.Write(VirtualMachine.Version);
            stream.Write(scriptData.regions.Length);
            {
                var begin = scriptData.regions.Begin;
                var end = scriptData.regions.End;
                while (begin < end)
                {
                    stream.Write(begin->name);
                    
                    stream.Write(begin->byteCode.length);
                    stream.CustomWrite(begin->byteCode.start, begin->byteCode.length);
                    begin++;
                }
            }
            
            var constants = scriptData.metadata.constants;
            
            stream.WriteNativeStringArray(constants.strings);
            
            stream.WriteNativeStringArray(constants.methods);

            var span = new ReadOnlySpan<byte>(stream.start, stream.length);
            output.Write(span);
            output.Flush();
            output.Dispose();
            stream.Dispose();
            
            scriptData.Dispose();
        }
    }
}