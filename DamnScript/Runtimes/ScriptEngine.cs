using System.Reflection;
using DamnScript.Parsings;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Metadatas;
using DamnScript.Runtimes.VirtualMachines.Datas;
using DamnScript.Runtimes.VirtualMachines.Threads;

namespace DamnScript.Runtimes;

public static unsafe class ScriptEngine
{
    private static VirtualMachineScheduler _mainScheduler = new(16);
    
    public static bool ExecuteScheduler() =>
        _mainScheduler.ExecuteNext();
    
    public static ScriptDataPtr LoadScriptFromCode(string scriptCode, string scriptName) => 
        ScriptsDataManager.LoadScriptFromCode(scriptCode, scriptName);
    public static ScriptDataPtr LoadScriptFromCompiledCode(byte[] bytes, string scriptName) =>
        ScriptsDataManager.LoadScriptFromCompiledCode(bytes, scriptName);

    public static VirtualMachineThreadPtr CreateThread(ScriptDataPtr scriptData, string regionName)
    {
        var regionData = scriptData.value->GetRegionData(new String32(regionName));
        if (regionData == null)
        {
            Debugging.LogError($"[{nameof(ScriptEngine)}] ({CreateThread}) " +
                               $"Region '{regionName}' not found in script '{scriptData.value->name}'");
            return default;
        }
        var thread = new VirtualMachineThread(regionData, &scriptData.value->metadata);
        var ptr = _mainScheduler.Register(thread);
        return ptr;
    }
        
    public static void RegisterNativeMethod(Delegate d) => 
        VirtualMachineData.RegisterNativeMethod(d);
    public static void RegisterNativeMethod(MethodInfo method) => 
        VirtualMachineData.RegisterNativeMethod(method);
}