using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using DamnScript.Parsings;
using DamnScript.Parsings.Serializations;
using DamnScript.Runtimes.BuiltIns;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Strings;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Serializations;
using DamnScript.Runtimes.VirtualMachines;
using DamnScript.Runtimes.VirtualMachines.Scripts;
using DamnScript.Runtimes.VirtualMachines.Threads;

namespace DamnScript.Runtimes
{
    public static unsafe class ScriptEngine
    {
        /// <summary>
        /// Returns the current thread executing right now.
        /// </summary>
        public static VirtualMachineThreadPtr CurrentThreadPtr
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => mainPtr.value->currentThread;
        }

        /// <summary>
        /// Current thread handle which is executing right now.
        /// </summary>
        public static VirtualMachineThreadHandle CurrentThreadHandle
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(CurrentThreadPtr.value == null ? -1 : CurrentThreadPtr - mainPtr.value->threads.Begin, mainPtr);
        }
        
        /// <summary>
        /// Pointer to the main virtual machine which is allocated by default.
        /// </summary>
        public static readonly VirtualMachinePtr mainPtr;
        
        static ScriptEngine()
        {
#if !DAMN_SCRIPT_DISABLE_ALLOC_DEFAULT_VIRTUAL_MACHINE
            mainPtr = VirtualMachine.Alloc();
#endif
            BuiltInMethods.Register();
        }
        
        /// <summary>
        /// Will register native method in the virtual machine.
        /// It supports methods with return value and async methods.
        /// Also, you can use OOP methods, but in this case, you should pass an instance of the object as the first argument.
        /// </summary>
        /// <param name="d">Delegate to method</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RegisterNativeMethod(Delegate d) => 
            MethodsStorage.RegisterNativeMethod(d, d.Method.Name);
        
        /// <summary>
        /// Will register native method in the virtual machine.
        /// It supports methods with return value and async methods.
        /// Also, you can use OOP methods, but in this case, you should pass an instance of the object as the first argument.
        /// </summary>
        /// <param name="method">Method info</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RegisterNativeMethod(MethodInfo method) => 
            MethodsStorage.RegisterNativeMethod(method, method.Name);

        /// <summary>
        /// Will register native method in the virtual machine.
        /// It supports methods with return value and async methods.
        /// Also, you can use OOP methods, but in this case, you should pass an instance of the object as the first argument.
        /// </summary>
        /// <param name="d">Delegate to method</param>
        /// <param name="name">Override method name</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RegisterNativeMethod(Delegate d, String32 name) => 
            MethodsStorage.RegisterNativeMethod(d, name);

        /// <summary>
        /// Will register native method in the virtual machine.
        /// It supports methods with return value and async methods.
        /// Also, you can use OOP methods, but in this case, you should pass an instance of the object as the first argument.
        /// </summary>
        /// <param name="method">Method info</param>
        /// <param name="name">Override method name</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RegisterNativeMethod(MethodInfo method, String32 name) => 
            MethodsStorage.RegisterNativeMethod(method, name);
    
        /// <summary>
        /// Load script from provided stream.
        /// Every single script SHOULD have a unique name because every script is loaded will be pushed into cache.
        /// It will be present in the dictionary until it is implicitly unloaded.
        /// </summary>
        /// <param name="input">Stream with script code. Should be a text code!</param>
        /// <param name="name">Name of the script. Should be unique!</param>
        /// <returns>Pointer to script data</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDataPtr LoadScript(Stream input, String32 name) => 
            ScriptsStorage.LoadScript(input, name);
    
        /// <summary>
        /// Load compiled script from provided stream.
        /// Every single script SHOULD have a unique name because every script is loaded will be pushed into cache.
        /// It will be present in the dictionary until it is implicitly unloaded.
        /// </summary>
        /// <param name="input">Stream with compiled script code. Should be a byte code!</param>
        /// <param name="name">Name of the script. Should be unique!</param>
        /// <returns>Pointer to script data</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDataPtr LoadCompiledScript(Stream input, String32 name) => 
            ScriptsStorage.LoadCompiledScript(input, name);

        /// <summary>
        /// Run thread with provided region name from script data.
        /// It automatically is registered in the main scheduler.
        /// End of thread guarantee that thread will be removed from the scheduler, but not be unloaded from the cache.
        /// </summary>
        /// <param name="scriptData">Pointer to script data</param>
        /// <param name="regionName">Region which should be run. By default, it's "Main" region</param>
        /// <returns>Pointer to thread</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VirtualMachineThreadHandle RunThread(ScriptDataPtr scriptData, String32 regionName) => 
            mainPtr.value->RunThread(scriptData, regionName);

        /// <summary>
        /// Execute all threads that are present in the main scheduler.
        /// Each call of this method will start executing code until it catches an async method.
        /// Because of it, this method should be called in application life cycle loop.
        /// </summary>
        /// <returns>Does scheduler have other threads?</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ExecuteVirtualMachineNext() =>
            mainPtr.value->ExecuteNext();
    
        /// <summary>
        /// Return script data from cache by provided name if it's present.
        /// If it's not present, it will return default.
        /// </summary>
        /// <param name="scriptName">Name of the script</param>
        /// <returns>Pointer to script data</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDataPtr GetScriptDataFromCache(String32 scriptName) => 
            ScriptsStorage.GetScriptData(scriptName);
    
        /// <summary>
        /// Unload script from cache by provided pointer.
        /// If a script is not present in cache, it will log an error. And do nothing.
        /// </summary>
        /// <param name="scriptData">Name of the script</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnloadScript(ScriptDataPtr scriptData) => 
            ScriptsStorage.UnloadScript(scriptData);

        /// <summary>
        /// Unload ALL scripts. Can throw exception if some threads are running and references are not zero.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnloadAllScripts() => 
            ScriptsStorage.UnloadAllScripts();

        /// <summary>
        /// Unload all unused scripts.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnloadAllUnusedScripts() => 
            ScriptsStorage.UnloadAllUnusedScripts();

        /// <summary>
        /// Serialize the Main virtual machine to bytes then return it.
        /// </summary>
        /// <returns>Serialization stream with bytes</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SerializationStream SerializeToSerializationStream() => 
            VirtualMachineSerialization.SerializeToSerializationStream(mainPtr);

        /// <summary>
        /// Deserialize the Main virtual machine from provided stream.
        /// </summary>
        /// <param name="stream">Stream with bytes</param>
        /// <exception cref="Exception">If any, using script is not loaded yet</exception>
        public static void DeserializeFromSerializationStream(SerializationStream stream)
        {
            var threads = VirtualMachineSerialization.DeserializeFromSerializationStream(stream);
            var begin = threads.Begin;
            var end = threads.End;
            while (begin < end)
            {
                var scriptData = GetScriptDataFromCache(begin->scriptName);
                if (scriptData.value == null)
                    throw new Exception($"Attempt to deserialize thread with script ({begin->scriptName.ToString()}) which is not present in cache!");
                
                mainPtr.value->RunThreadFromSerialized(scriptData, begin);
                begin++;
            }
            
            threads.Dispose();
        }
    }
}