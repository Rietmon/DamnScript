namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
	public interface IOpCode
	{
		int CalculateHash();
		
#if DAMN_SCRIPT_ENABLE_ASSEMBLER_DEBUG
		string GetAssemblerDebugInfo();
#endif
	}
}