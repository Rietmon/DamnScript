using System;
using System.IO;
using DamnScript.Parsings.Antlrs;
using DamnScript.Parsings.Compilings;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Types;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Metadatas;

namespace DamnScript.Parsings
{
    public static unsafe class ScriptsDataManager
    {
        private const int MaxStackBufferSize = 1024 * 16;
    
        private static NativeList<ScriptDataPtr> _scripts = new(16);

        private static byte* _buffer;
        private static int _bufferSize;
    
        public static ScriptDataPtr GetScriptData(String32 name)
        {
            if (_scripts.Count == 0)
                return default;
        
            var begin = _scripts.Begin;
            var end = _scripts.End;
        
            while (begin < end)
            {
                if (begin->value->name == name)
                    return *begin;
                begin++;
            }
            return default;
        }
    
        public static ScriptDataPtr LoadScript(Stream input, String32 name)
        {
            var loaded = GetScriptData(name);
            if (loaded.value != null)
                return loaded;
        
            var scriptData = UnsafeUtilities.Alloc<ScriptData>();
            ScriptParser.ParseScript(input, name, scriptData);
            var scriptDataPtr = new ScriptDataPtr(scriptData);
            _scripts.Add(scriptDataPtr);
        
            return scriptDataPtr;
        }
    
        public static ScriptDataPtr LoadCompiledScript(Stream input, String32 name)
        {
            var loaded = GetScriptData(name);
            if (loaded.value != null)
                return loaded;

            var length = input.Length;
            switch (length)
            {
                case > int.MaxValue:
                    throw new NotSupportedException("Input stream is large than 32bit value!");
            
                case <= MaxStackBufferSize:
                    return LoadCompiledScriptWithStackAlloc(input, name);
            }

            if (_bufferSize < length)
            {
                _bufferSize = (int)length;
                if (_buffer != null)
                    _buffer = (byte*)UnsafeUtilities.ReAlloc(_buffer, _bufferSize);
                else
                    _buffer = (byte*)UnsafeUtilities.Alloc(_bufferSize);
            }
            var scriptData = UnsafeUtilities.Alloc<ScriptData>();
            CompiledScriptParser.ParseCompiledScript(_buffer, _bufferSize, name, scriptData);
            var scriptDataPtr = new ScriptDataPtr(scriptData);
            _scripts.Add(scriptDataPtr);
        
            return scriptDataPtr;
        }
    
        public static ScriptDataPtr LoadCompiledScriptWithStackAlloc(Stream input, String32 name)
        {
            var length = (int)input.Length;
            if (length > MaxStackBufferSize)
                throw new NotSupportedException($"Input length is too big for stack: {input.Length.ToString()}!");
        
            var scriptData = UnsafeUtilities.Alloc<ScriptData>();
            var buffer = stackalloc byte[length];
            CompiledScriptParser.ParseCompiledScript(buffer, length, name, scriptData);
            var scriptDataPtr = new ScriptDataPtr(scriptData);
            _scripts.Add(scriptDataPtr);
        
            return scriptDataPtr;
        }
    
        public static void UnloadScript(ScriptDataPtr scriptDataPtr)
        {
            if (!_scripts.Remove(scriptDataPtr))
            {
                Debugging.LogError($"[{nameof(ScriptsDataManager)}] ({nameof(UnloadScript)}) " +
                                   $"Attempt to unload script which is not present in cache! Name: {scriptDataPtr.value->name.ToString()}");
                return;
            }
        
            scriptDataPtr.value->Dispose();
            UnsafeUtilities.Free(scriptDataPtr.value);
        }
    }
}