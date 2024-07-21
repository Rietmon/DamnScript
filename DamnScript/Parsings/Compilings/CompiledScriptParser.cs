using DamnScript.Parsings.Serializations;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Types;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Metadatas;

namespace DamnScript.Parsings.Compilings
{
    public static unsafe class CompiledScriptParser
    {
        public static void ParseCompiledScript(byte* scriptCode, int length, StringWrapper scriptName,
            ScriptData* scriptData)
        {
            scriptData->name = scriptName.ToString32();

            var regions = new NativeList<RegionData>(16);

            var stream = new SerializationStream(scriptCode, length);

            var version = stream.Read<int>();
            if (version != ScriptCompiler.Version)
            {
                Debugging.LogError($"[{nameof(CompiledScriptParser)}] ({nameof(ParseCompiledScript)}) " +
                                   $"Invalid script version: {version}");
                return;
            }

            var regionsCount = stream.Read<int>();
            for (var i = 0; i < regionsCount; i++)
            {
                var regionName = stream.Read<String32>();

                var byteCode = stream.Read<ByteCodeData>();
                regions.Add(new RegionData(regionName, byteCode));
            }

            var constantStringsLength = stream.Read<int>();
            var constantStrings = new NativeArray<UnsafeStringPair>(constantStringsLength);
            for (var i = 0; i < constantStringsLength; i++)
            {
                var constantStringLength = stream.Read<int>();
                var constantString = UnsafeString.Alloc(constantStringLength);
                stream.CustomRead(constantString, constantStringLength);
                var hash = constantString->GetHashCode();
                constantStrings.Begin[i] = new UnsafeStringPair(hash, constantString);
            }

            scriptData->regions = regions.ToArrayAlloc();
            regions.Dispose();

            scriptData->metadata = new ScriptMetadata(new ConstantsData(constantStrings));
            constantStrings.Dispose();
        }
    }
}