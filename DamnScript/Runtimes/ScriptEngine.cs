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
    
    public static ScriptDataPtr LoadScript(Stream input, SafeString name) => 
        ScriptsDataManager.LoadScript(input, name);
    
    public static ScriptDataPtr LoadCompiledScript(Stream input, SafeString name) => 
        ScriptsDataManager.LoadCompiledScript(input, name);

    public static VirtualMachineThreadPtr CreateThread(ScriptDataPtr scriptData, string regionName)
    {
        var regionData = scriptData.value->GetRegionData(new String32(regionName));
        if (regionData == null)
        {
            Debugging.LogError($"[{nameof(ScriptEngine)}] ({CreateThread}) " +
                               $"Region \"{regionName}\" not found in script \"{scriptData.value->name}\"");
            return default;
        }
        var thread = new VirtualMachineThread(regionData, &scriptData.value->metadata);
        var ptr = _mainScheduler.Register(thread);
        return ptr;
    }
    
    public static void UnloadScript(ScriptDataPtr scriptData) => 
        ScriptsDataManager.UnloadScript(scriptData);
        
    public static void RegisterNativeMethod(Delegate d) => 
        VirtualMachineData.RegisterNativeMethod(d);
    public static void RegisterNativeMethod(MethodInfo method) => 
        VirtualMachineData.RegisterNativeMethod(method);
}