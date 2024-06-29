using DamnScript.Parsings.Antlrs;
using DamnScript.Parsings.Compilings;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Metadatas;

namespace DamnScript.Parsings;

public static unsafe class ScriptsDataManager
{
    private static NativeList<ScriptDataPtr> _scripts = new(16);

    public static ScriptDataPtr LoadScriptFromCode(string scriptCode, string scriptName)
    {
        var scriptData = (ScriptData*)UnsafeUtilities.Alloc(sizeof(ScriptData));
        ScriptParser.ParseScript(scriptCode, scriptName, scriptData);
        var scriptDataPtr = new ScriptDataPtr(scriptData);
        _scripts.Add(scriptDataPtr);
        return scriptDataPtr;
    }
    
    public static ScriptDataPtr LoadScriptFromCompiledCode(byte[] bytes, string scriptName)
    {
        var scriptData = (ScriptData*)UnsafeUtilities.Alloc(sizeof(ScriptData));
        fixed (byte* ptr = bytes)
            CompiledScriptParser.ParseCompiledScript(ptr, bytes.Length, scriptName, scriptData);
        var scriptDataPtr = new ScriptDataPtr(scriptData);
        _scripts.Add(scriptDataPtr);
        return scriptDataPtr;
    }
    
    public static void UnloadScript(ScriptDataPtr scriptDataPtr)
    {
        _scripts.Remove(scriptDataPtr);
        scriptDataPtr.value->Dispose();
        UnsafeUtilities.Free(scriptDataPtr.value);
    }
}