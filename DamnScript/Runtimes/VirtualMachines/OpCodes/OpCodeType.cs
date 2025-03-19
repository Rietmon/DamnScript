namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
	public enum OpCodeType
#if DAMN_SCRIPT_ENABLE_64_BIT_OPCODES
		: ulong
#elif DAMN_SCRIPT_ENABLE_32_BIT_OPCODES
		: uint
#elif DAMN_SCRIPT_ENABLE_16_BIT_OPCODES
		: ushort
#else
		: byte
#endif
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