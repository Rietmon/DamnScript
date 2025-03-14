namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
	public enum OpCodes
	{
		Invalid,
		NativeCall,
		PushToStack,
		ExpressionCall,
		SetSavePoint,
		JumpNotEquals,
		JumpEquals,
		SetThreadParameters,
		PushStringToStack,
		Jump,
		StoreToRegister,
		LoadFromRegister,
		DuplicateStack,
	}
}