using DamnScript.Parsings.Antlrs;
using DamnScript.Parsings.Compilings;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Metadatas;

namespace DamnScript.Parsings;

public static unsafe class ScriptsDataManager
{
    private const int MaxStackBufferSize = 1024 * 16;
    
    private static NativeList<ScriptDataPtr> _scripts = new(16);

    private static byte* _buffer;
    private static int _bufferSize;
    
    public static ScriptDataPtr GetScriptDataPtr(SafeString name)
    {
        if (_scripts.Count == 0)
            return default;
        
        var begin = _scripts.Begin;
        var end = _scripts.End;
        
        var nameStr32 = name.ToString32();
        
        while (begin < end)
        {
            if (begin->value->name == nameStr32)
                return *begin;
            begin++;
        }
        return default;
    }
    
    public static ScriptDataPtr LoadScript(Stream input, SafeString name)
    {
        var loaded = GetScriptDataPtr(name);
        if (loaded.value != null)
            return loaded;
        
        var scriptData = UnsafeUtilities.Alloc<ScriptData>();
        ScriptParser.ParseScript(input, name, scriptData);
        var scriptDataPtr = new ScriptDataPtr(scriptData);
        _scripts.Add(scriptDataPtr);
        
        name.Dispose();
        return scriptDataPtr;
    }
    
    public static ScriptDataPtr LoadCompiledScript(Stream input, SafeString name)
    {
        var loaded = GetScriptDataPtr(name);
        if (loaded.value != null)
            return loaded;

        var length = input.Length;
        switch (length)
        {
            case > int.MaxValue:
                Debugging.LogError($"[{nameof(ScriptsDataManager)}] ({nameof(LoadCompiledScript)}) " +
                                   $"Input stream is large than 32bit value!");
                return default;
            case <= MaxStackBufferSize:
                return LoadCompiledScriptWithStackAlloc(input, name);
        }

        if (_buffer == null || _bufferSize < length)
        {
            if (_buffer != null)
            {
                _buffer = (byte*)UnsafeUtilities.ReAlloc(_buffer, (int)length);
                UnsafeUtilities.Free(_buffer);
            }
            else
            {
                _buffer = (byte*)UnsafeUtilities.Alloc((int)length);
            }
            _bufferSize = (int)length;
        }
        var scriptData = UnsafeUtilities.Alloc<ScriptData>();
        CompiledScriptParser.ParseCompiledScript(_buffer, _bufferSize, name, scriptData);
        var scriptDataPtr = new ScriptDataPtr(scriptData);
        _scripts.Add(scriptDataPtr);
        
        name.Dispose();
        return scriptDataPtr;
    }
    
    private static ScriptDataPtr LoadCompiledScriptWithStackAlloc(Stream input, SafeString name)
    {
        var length = (int)input.Length;
        if (length > MaxStackBufferSize)
        {
            Debugging.LogError($"[{nameof(ScriptsDataManager)}] ({nameof(LoadCompiledScriptWithStackAlloc)}) " +
                               $"Input length is too big for stack: {input.Length}!");
            return default;
        }
        
        var scriptData = UnsafeUtilities.Alloc<ScriptData>();
        var buffer = stackalloc byte[length];
        CompiledScriptParser.ParseCompiledScript(buffer, length, name, scriptData);
        var scriptDataPtr = new ScriptDataPtr(scriptData);
        _scripts.Add(scriptDataPtr);
        
        name.Dispose();
        return scriptDataPtr;
    }
    
    public static void UnloadScript(ScriptDataPtr scriptDataPtr)
    {
        _scripts.Remove(scriptDataPtr);
        scriptDataPtr.value->Dispose();
        UnsafeUtilities.Free(scriptDataPtr.value);
    }
}