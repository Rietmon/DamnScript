using DamnScript.Parsings.Serializations;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Types;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Metadatas;

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
            var constantStrings = new NativeArray<UnsafeStringPtr>(constantStringsLength);
            for (var i = 0; i < constantStringsLength; i++)
            {
                var constantStringLength = stream.Read<int>();
                var constantString = UnsafeString.Alloc(constantStringLength);
                stream.CustomRead(constantString, constantStringLength);
                constantStrings.Begin[i] = new UnsafeStringPtr(constantString);
            }
            
            var methodNamesLength = stream.Read<int>();
            var methodNames = new NativeArray<UnsafeStringPtr>(methodNamesLength);
            for (var i = 0; i < methodNamesLength; i++)
            {
                var methodNameLength = stream.Read<int>();
                var methodName = UnsafeString.Alloc(methodNameLength);
                stream.CustomRead(methodName, methodNameLength);
                methodNames.Begin[i] = new UnsafeStringPtr(methodName);
            }

            scriptData->regions = regions.ToArrayAlloc();
            regions.Dispose();

            scriptData->metadata = new ScriptMetadata(new ConstantsData(constantStrings, methodNames));
            constantStrings.Dispose();
        }
    }
}