using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Types;
using DamnScript.Runtimes.Metadatas;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.Datas;
using DamnScript.Runtimes.VirtualMachines.OpCodes;

namespace DamnScript.Runtimes.VirtualMachines.Threads
{
    public readonly unsafe struct VirtualMachineThreadPtr
    {
        public readonly VirtualMachineThread* value; 
    
        public ref VirtualMachineThread RefValue => ref *value;
    
        public VirtualMachineThreadPtr(VirtualMachineThread* value) => this.value = value;

        public static implicit operator VirtualMachineThreadPtr(VirtualMachineThread* value) => new(value);
    
        public static implicit operator VirtualMachineThread*(VirtualMachineThreadPtr ptr) => ptr.value;
    }

    public unsafe struct VirtualMachineThread : IDisposable
    {
        private byte* ByteCode => regionData->byteCode.start + offset;

        public readonly String32* scriptName;
        public readonly RegionData* regionData;
        public readonly ScriptMetadata* metadata;

        public VirtualMachineThreadStack stack;
        public VirtualMachineRegisters registers;
    
        public int offset;
        public int savePoint;

        public bool isDisposed;

        public VirtualMachineThread(String32* scriptName, RegionData* regionData, ScriptMetadata* metadata) : this()
        {
            this.scriptName = scriptName;
            this.regionData = regionData;
            this.metadata = metadata;
            stack = new VirtualMachineThreadStack();
            registers = new VirtualMachineRegisters();
        }
    
        /// <summary>
        /// Begin handle thread. Will work until the end of the bytecode, or until it invokes async method.
        /// In the second case, it will return "out Task".
        /// </summary>
        /// <param name="result">Is invoked async method?</param>
        /// <returns>Is the end of the bytecode or thread disposed?</returns>
        /// <exception cref="Exception">Invalid opcode</exception>
        public bool ExecuteNext(out Task result)
        {
            result = null;
        
            if (isDisposed)
                return false;
        
            if (!regionData->byteCode.IsInRange(offset))
                return false;
        
            var byteCode = ByteCode;
            var opCode = *(int*)byteCode;
            switch (opCode)
            {
                case NativeCall.OpCode:
                    ExecuteNativeCall(*(NativeCall*)byteCode, out result);
                    offset += sizeof(NativeCall);
                    break;
                case PushToStack.OpCode:
                    ExecutePushToStack(*(PushToStack*)byteCode);
                    offset += sizeof(PushToStack);
                    break;
                case ExpressionCall.OpCode:
                    ExecuteExpressionCall(*(ExpressionCall*)byteCode);
                    offset += sizeof(ExpressionCall);
                    break;
                case SetSavePoint.OpCode:
                    ExecuteSetSavePoint();
                    offset += sizeof(SetSavePoint);
                    break;
                case JumpNotEquals.OpCode:
                    if (ExecuteJumpNotEquals(*(JumpNotEquals*)byteCode))
                        offset += sizeof(JumpNotEquals);
                    break;
                case JumpIfEquals.OpCode:
                    if (ExecuteJumpIfEquals(*(JumpIfEquals*)byteCode))
                        offset += sizeof(JumpIfEquals);
                    break;
                case Jump.OpCode:
                    if (ExecuteJump(*(Jump*)byteCode))
                        offset += sizeof(Jump);
                    break;
                case PushStringToStack.OpCode:
                    ExecutePushStringToStack(*(PushStringToStack*)byteCode);
                    offset += sizeof(JumpIfEquals);
                    break;
                case SetThreadParameters.OpCode:
                    ExecuteSetThreadParameters(*(SetThreadParameters*)byteCode);
                    offset += sizeof(SetThreadParameters);
                    break;
                case StoreToRegister.OpCode:
                    ExecuteStoreToRegister(*(StoreToRegister*)byteCode);
                    offset += sizeof(StoreToRegister);
                    break;
                case LoadFromRegister.OpCode:
                    ExecuteLoadFromRegister(*(LoadFromRegister*)byteCode);
                    offset += sizeof(LoadFromRegister);
                    break;
                case DuplicateStack.OpCode:
                    ExecuteDuplicateStack(*(DuplicateStack*)byteCode);
                    offset += sizeof(DuplicateStack);
                    break;
                default:
                    throw new Exception($"Invalid OpCode: {opCode}");
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteNativeCall(NativeCall nativeCall, out Task result)
        {
            result = null;
            var methodName = nativeCall.name;
            var argumentsCount = nativeCall.argumentsCount;
            if (!VirtualMachineData.TryGetNativeMethod(methodName, argumentsCount, out var method))
                return false;
        
            var argumentsStack = stackalloc ScriptValue[method.argumentsCount];
            for (var i = method.argumentsCount - 1; i >= 0; i--)
                argumentsStack[i] = StackPop();
        
            var returnValue = VirtualMachineInvokeHelper.Invoke(method, argumentsStack, out result);
        
            for (var i = 0; i < method.argumentsCount; i++)
            {
                var argument = argumentsStack[i];
                if (argument.type == ScriptValue.ValueType.ReferenceSafePointer)
                    argument.UnpinManagedPointer();
            }
        
            if (method.hasReturnValue)
                StackPush(returnValue);
        
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecutePushToStack(PushToStack pushToStack)
        {
            StackPush(pushToStack.value);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteExpressionCall(ExpressionCall expressionCall)
        {
            switch (expressionCall.type)
            {
                case ExpressionCall.ExpressionCallType.Add:
                {
                    var right = StackPop();
                    var left = StackPop();
                    StackPush(left + right);
                    break;
                }
                case ExpressionCall.ExpressionCallType.Subtract:
                {
                    var right = StackPop();
                    var left = StackPop();
                    StackPush(left - right);
                    break;
                }
                case ExpressionCall.ExpressionCallType.Multiply:
                {
                    var right = StackPop();
                    var left = StackPop();
                    StackPush(left * right);
                    break;
                }
                case ExpressionCall.ExpressionCallType.Divide:
                {
                    var right = StackPop();
                    var left = StackPop();
                    StackPush(left / right);
                    break;
                }
                case ExpressionCall.ExpressionCallType.Modulo:
                {
                    var right = StackPop();
                    var left = StackPop();
                    StackPush(left % right);
                    break;
                }
                case ExpressionCall.ExpressionCallType.Negate: StackPush(-StackPop()); break;
                case ExpressionCall.ExpressionCallType.Equal: StackPush(StackPop() == StackPop() ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.NotEqual: StackPush(StackPop() != StackPop() ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.Greater:
                {
                    var right = StackPop();
                    var left = StackPop();
                    StackPush(left > right);
                    break;
                }
                case ExpressionCall.ExpressionCallType.GreaterOrEqual:
                {
                    var right = StackPop();
                    var left = StackPop();
                    StackPush(left >= right);
                    break;
                }
                case ExpressionCall.ExpressionCallType.Less:
                {
                    var right = StackPop();
                    var left = StackPop();
                    StackPush(left < right);
                    break;
                }
                case ExpressionCall.ExpressionCallType.LessOrEqual:
                {
                    var right = StackPop();
                    var left = StackPop();
                    StackPush(left <= right);
                    break;
                }
                case ExpressionCall.ExpressionCallType.And: StackPush(StackPop() != 0 && StackPop() != 0 ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.Or: StackPush(StackPop() != 0 || StackPop() != 0 ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.Not: StackPush(StackPop() == 0 ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.Test: StackPush(StackPop() != 0 ? 1 : 0); break;
                default: throw new ArgumentOutOfRangeException($"{nameof(expressionCall)} == {expressionCall.type}");
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteSetSavePoint()
        {
            savePoint = offset;
            return true;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteJumpNotEquals(JumpNotEquals jumpNotEquals)
        {
            if (StackPop() == StackPop()) 
                return true;
        
            offset = jumpNotEquals.jumpOffset;
            return false;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteJumpIfEquals(JumpIfEquals jumpIfEquals)
        {
            if (StackPop() != StackPop())
                return true;
        
            offset = jumpIfEquals.jumpOffset;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ExecuteJump(Jump jump)
        {
            offset = jump.jumpOffset;
            return false;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecutePushStringToStack(PushStringToStack pushStringToStack)
        {
            var hash = pushStringToStack.hash;
            var str = metadata->GetUnsafeString(hash);
            if (str == null)
                throw new Exception($"String not found with hash: {hash}");
        
            StackPush(str);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteSetThreadParameters(SetThreadParameters setThreadParameters)
        {
            throw new NotImplementedException();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteStoreToRegister(StoreToRegister storeToRegister)
        {
            var registerIndex = storeToRegister.register;
            registers[registerIndex] = StackPop().longValue;
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteLoadFromRegister(LoadFromRegister loadFromRegister)
        {
            var registerIndex = loadFromRegister.register;
            StackPush(registers[registerIndex]);
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteDuplicateStack(DuplicateStack duplicateStack)
        {
            var value = StackPop();
            StackPush(value);
            StackPush(value);

            return true;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void StackPush(ScriptValue value) => stack.Push(value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue StackPop() => stack.Pop();

        public void Dispose()
        {
            isDisposed = true;
        }
    }
}