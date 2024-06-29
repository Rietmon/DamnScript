using DamnScript.Parsings.Serializations;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Metadatas;

namespace DamnScript.Parsings.Compilings;

public static unsafe class CompiledScriptParser
{
    public const int Version = 1;
    
    public static void ParseCompiledScript(byte* scriptCode, int length, string scriptName, ScriptData* scriptData)
    {
        scriptData->name = new String32(scriptName);
        
        var regions = new NativeList<RegionData>(16);

        var stream = new SerializationStream(scriptCode, length);

        var version = stream.Read<int>();
        if (version != Version)
        {
            Debugging.LogError($"[{nameof(CompiledScriptParser)}] ({nameof(ParseCompiledScript)}) " +
                               $"Invalid script version: {version}");
            return;
        }

        var regionsCount = stream.Read<int>();
        for (var i = 0; i < regionsCount; i++)
        {
            var regionNameLength = stream.Read<int>();
            var regionName = new String32();
            stream.CustomRead(&regionName, regionNameLength);
            
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
            constantStrings.Data[i] = new UnsafeStringPair(hash, constantString);
        }
        
        scriptData->regions = regions.ToArrayAlloc();
        regions.Dispose();
        
        scriptData->metadata = new ScriptMetadata(new ConstantsData(constantStrings));
        constantStrings.Dispose();
    }
}