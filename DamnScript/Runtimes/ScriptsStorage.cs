using System;
using System.IO;
using DamnScript.Parsings.Antlrs;
using DamnScript.Parsings.Compilings;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Collections;
using DamnScript.Runtimes.Cores.Strings;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.VirtualMachines.Scripts;

namespace DamnScript.Runtimes
{
    public static unsafe class ScriptsStorage
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
        
            var scriptData = ScriptData.Alloc();
            ScriptParser.ParseScript(input, name, scriptData);
            var scriptDataPtr = new ScriptDataPtr(scriptData);
            _scripts.Add(scriptDataPtr);
        
            return scriptDataPtr;
        }

        public static ScriptDataPtr ReloadScript(Stream input, String32 name)
        {
            var loadedScriptData = GetScriptData(name).value;
            if (loadedScriptData == null)
                throw new Exception("Unable to reload script because no script was loaded.");

            var newScriptData = new ScriptData();
            ScriptParser.ParseScript(input, name, &newScriptData);
            
            if (loadedScriptData->regions.Length != newScriptData.regions.Length)
                throw new NotSupportedException("Not support different number of regions");

            var loadedConsts = loadedScriptData->metadata.constants;
            var newConsts = newScriptData.metadata.constants;
            
            if (loadedConsts.strings.Length < newConsts.strings.Length)
                NativeArray<NativeStringPtr>.ReAlloc(&loadedConsts.strings, newConsts.strings.Length);
            if (loadedConsts.methods.Length < newConsts.methods.Length)
                NativeArray<NativeStringPtr>.ReAlloc(&loadedConsts.methods, newConsts.methods.Length);

            loadedScriptData->metadata.constants = new ConstantsData(loadedConsts.strings, loadedConsts.methods);
            
            for (var i = 0; i < newConsts.strings.Length; i++)
                *(loadedConsts.strings.Begin + i) = *(newConsts.strings.Begin + i);
            for (var i = 0; i < newConsts.methods.Length; i++)
                *(loadedConsts.methods.Begin + i) = *(newConsts.methods.Begin + i);
            
            newConsts.strings.Dispose();
            newConsts.methods.Dispose();
            
            for (var i = 0; i < newScriptData.regions.Length; i++)
            {
                var newRegion = newScriptData.regions.Begin + i;
                var loadedRegion = loadedScriptData->regions.Begin + i;
                if (loadedRegion->byteCode.length < newRegion->byteCode.length)
                {
                    UnsafeUtilities.ReAlloc(loadedRegion->byteCode.start, newRegion->byteCode.length);
                    loadedRegion->byteCode.length = newRegion->byteCode.length;
                }
                
                UnsafeUtilities.Memcpy(newRegion->byteCode.start, loadedRegion->byteCode.start, newRegion->byteCode.length);
            }
            
            return loadedScriptData;
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
            if (input.Read(new Span<byte>(_buffer, _bufferSize)) != _bufferSize)
                throw new IOException("Failed to read input stream!");
                
            input.Dispose();
            var scriptData = ScriptData.Alloc();
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

            var scriptData = ScriptData.Alloc();
            var buffer = stackalloc byte[length];
            if (input.Read(new Span<byte>(buffer, length)) != length)
                throw new IOException("Failed to read input stream!");
            
            input.Dispose();
            CompiledScriptParser.ParseCompiledScript(buffer, length, name, scriptData);
            var scriptDataPtr = new ScriptDataPtr(scriptData);
            _scripts.Add(scriptDataPtr);
        
            return scriptDataPtr;
        }
    
        public static void UnloadScript(ScriptDataPtr scriptDataPtr)
        {
            if (!_scripts.Remove(scriptDataPtr))
            {
                Debugging.LogError($"[{nameof(ScriptsStorage)}] ({nameof(UnloadScript)}) " +
                                   $"Attempt to unload script which is not present in cache! Name: {scriptDataPtr.value->name.ToString()}");
                return;
            }
        
            scriptDataPtr.value->Dispose();
            UnsafeUtilities.Free(scriptDataPtr.value);
        }
    }
}