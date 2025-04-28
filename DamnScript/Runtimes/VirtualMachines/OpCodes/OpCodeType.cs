namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
	public enum OpCodeType
#if DAMN_SCRIPT_ENABLE_64_BIT_OPCODES
		: ulong
#elif DAMN_SCRIPT_ENABLE_16_BIT_OPCODES
		: ushort
#elif DAMN_SCRIPT_ENABLE_8_BIT_OPCODES
		: byte
#else
		: uint
#endif
	{
		Invalid,
		NativeCall,
		PushToStack,
		ExpressionCall,
		SetSavePoint,
		JumpNotEquals,
		JumpEquals,
		PushStringToStack,
		Jump,
		StoreToRegister,
		LoadFromRegister,
		DuplicateStack,
		PushNullToStack
	}
}