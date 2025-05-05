using System;
using DamnScript.Parsings.Serializations;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Collections;
using DamnScript.Runtimes.Cores.Strings;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.VirtualMachines;
using DamnScript.Runtimes.VirtualMachines.Scripts;

namespace DamnScript.Parsings.Compilings
{
    public static unsafe class CompiledScriptParser
    {
        public static void ParseCompiledScript(byte* scriptCode, int length, String32 scriptName,
            ScriptData* scriptData)
        {
            scriptData->name = scriptName;

            var regions = new NativeList<RegionData>(16);

            var stream = new SerializationStream(scriptCode, length);

            var version = stream.Read<int>();
            if (version != VirtualMachine.Version)
                throw new Exception($"Invalid script version: {version}. Expected: {VirtualMachine.Version}");

            var regionsCount = stream.Read<int>();
            for (var i = 0; i < regionsCount; i++)
            {
                var regionName = stream.Read<String32>();

                var size = stream.Read<int>();
                var bytes = (byte*)UnsafeUtilities.Alloc(size);
                stream.CustomRead(bytes, size);
                regions.Add(new RegionData(regionName, new ByteCodeData(bytes, size)));
            }

            var constantStrings = stream.ReadNativeStringArray();
            var methodNames = stream.ReadNativeStringArray();

            scriptData->regions = regions.ToArrayAlloc();
            regions.Dispose();

            scriptData->metadata = new ScriptMetadata(new ConstantsData(constantStrings, methodNames));
        }
    }
}