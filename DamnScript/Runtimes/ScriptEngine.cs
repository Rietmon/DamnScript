using System;
using System.IO;
using System.Reflection;
using DamnScript.Parsings;
using DamnScript.Parsings.Serializations;
using DamnScript.Runtimes.Cores.Types;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Metadatas;
using DamnScript.Runtimes.Serializations;
using DamnScript.Runtimes.VirtualMachines;
using DamnScript.Runtimes.VirtualMachines.Datas;
using DamnScript.Runtimes.VirtualMachines.Threads;

namespace DamnScript.Runtimes
{
    public static unsafe class ScriptEngine
    {
        private const string DefaultRegionName = "Main";
        
        private static readonly String32 defaultRegionName32 = new(DefaultRegionName);
        
        private static VirtualMachine _main = new(16);

        /// <summary>
        /// Will register native method in the virtual machine.
        /// It supports methods with return value and async methods.
        /// Also, you can use OOP methods, but in this case, you should pass an instance of the object as the first argument.
        /// </summary>
        /// <param name="d">Delegate to method</param>
        /// <param name="name">Override method name</param>
        public static void RegisterNativeMethod(Delegate d, StringWrapper name = default) => 
            VirtualMachineData.RegisterNativeMethod(d, name);

        /// <summary>
        /// Will register native method in the virtual machine.
        /// It supports methods with return value and async methods.
        /// Also, you can use OOP methods, but in this case, you should pass an instance of the object as the first argument.
        /// </summary>
        /// <param name="method">Method info</param>
        /// <param name="name">Override method name</param>
        public static void RegisterNativeMethod(MethodInfo method, StringWrapper name = default) => 
            VirtualMachineData.RegisterNativeMethod(method, name);
    
        /// <summary>
        /// Load script from provided stream.
        /// Every single script SHOULD have a unique name because every script is loaded will be pushed into cache.
        /// It will be present in the dictionary until it is implicitly unloaded.
        /// </summary>
        /// <param name="input">Stream with script code. Should be a text code!</param>
        /// <param name="name">Name of the script. Should be unique!</param>
        /// <returns>Pointer to script data</returns>
        public static ScriptDataPtr LoadScript(Stream input, StringWrapper name) => 
            ScriptsDataManager.LoadScript(input, name);
    
        /// <summary>
        /// Load compiled script from provided stream.
        /// Every single script SHOULD have a unique name because every script is loaded will be pushed into cache.
        /// It will be present in the dictionary until it is implicitly unloaded.
        /// </summary>
        /// <param name="input">Stream with compiled script code. Should be a byte code!</param>
        /// <param name="name">Name of the script. Should be unique!</param>
        /// <returns>Pointer to script data</returns>
        public static ScriptDataPtr LoadCompiledScript(Stream input, StringWrapper name) => 
            ScriptsDataManager.LoadCompiledScript(input, name);

        /// <summary>
        /// Run thread with provided region name from script data.
        /// It automatically is registered in the main scheduler.
        /// End of thread guarantee that thread will be removed from the scheduler, but not be unloaded from the cache.
        /// </summary>
        /// <param name="scriptData">Pointer to script data</param>
        /// <param name="regionName">Region which should be run. By default, it's "Main" region</param>
        /// <returns>Pointer to thread</returns>
        public static VirtualMachineThreadPtr RunThread(ScriptDataPtr scriptData, StringWrapper regionName = default)
        {
            var name = regionName.type == StringWrapper.ConstStringType.Invalid 
                ? defaultRegionName32
                : regionName.ToString32();
        
            var regionData = scriptData.value->GetRegionData(name);
            if (regionData == null)
                throw new Exception($"Region with name {regionName} not found in script \"{name}\"!");
            
            var thread = new VirtualMachineThread(&scriptData.value->name, regionData, &scriptData.value->metadata);
            var ptr = _main.Register(thread);
            return ptr;
        }

        /// <summary>
        /// Execute all threads that are present in the main scheduler.
        /// Each call of this method will start executing code until it catches an async method.
        /// Because of it, this method should be called in application life cycle loop.
        /// </summary>
        /// <returns>Does scheduler have other threads?</returns>
        public static bool ExecuteScheduler() =>
            _main.ExecuteNext();
    
        /// <summary>
        /// Return script data from cache by provided name if it's present.
        /// If it's not present, it will return default.
        /// </summary>
        /// <param name="scriptName">Name of the script</param>
        /// <returns>Pointer to script data</returns>
        public static ScriptDataPtr GetScriptDataFromCache(StringWrapper scriptName) => 
            ScriptsDataManager.GetScriptData(scriptName);
    
        /// <summary>
        /// Return script data from cache by provided name if it's present.
        /// If it's not present, it will return default.
        /// </summary>
        /// <param name="scriptName">Name of the script</param>
        /// <returns>Pointer to script data</returns>
        public static ScriptDataPtr GetScriptDataFromCache(String32 scriptName) => 
            ScriptsDataManager.GetScriptData(scriptName);
    
        /// <summary>
        /// Unload script from cache by provided pointer.
        /// If a script is not present in cache, it will log an error. And do nothing.
        /// </summary>
        /// <param name="scriptData">Name of the script</param>
        public static void UnloadScript(ScriptDataPtr scriptData) => 
            ScriptsDataManager.UnloadScript(scriptData);
        
        public static VirtualMachineThreadPtr GetCurrentThread() => 
            _main.currentThread;

        public static SerializationStream SerializeToSerializationStream() => 
            VirtualMachineSerialization.SerializeToSerializationStream(_main);

        public static void DeserializeFromSerializationStream(SerializationStream stream)
        {
            var threads = VirtualMachineSerialization.DeserializeFromSerializationStream(stream);
            var begin = threads.Begin;
            var end = threads.End;
            while (begin < end)
            {
                var scriptData = GetScriptDataFromCache(begin->scriptName);
                if (scriptData.value == null)
                    throw new Exception($"Script data with hash {begin->scriptName} not found in cache!");
                
                var thread = new VirtualMachineThread(
                    &scriptData.value->name, 
                    scriptData.value->GetRegionData(begin->regionName),
                    &scriptData.value->metadata)
                {
                    offset = begin->savePoint,
                    savePoint = begin->savePoint,
                    stack = begin->stack,
                    registers = begin->registers
                };
                
                _main.Register(thread);
                begin++;
            }
            
            threads.Dispose();
        }
    }
}