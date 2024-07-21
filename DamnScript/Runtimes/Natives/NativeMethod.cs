namespace DamnScript.Runtimes.Natives
{
    public readonly unsafe struct NativeMethod
    {
        public readonly void* methodPointer;
        public readonly int argumentsCount;
        public readonly bool isAsync;
        public readonly bool isStatic;
        public readonly bool hasReturnValue;

        public NativeMethod(void* methodPointer, int argumentsCount, bool isAsync, bool isStatic, bool hasReturnValue)
        {
            this.methodPointer = methodPointer;
            this.argumentsCount = argumentsCount + (isStatic ? 0 : 1);
            if (this.argumentsCount > 10)
            {
                throw new System.ArgumentException("The maximum number of arguments is 10 for native method. " +
                                                   "Probably it is 10 but you are using a non-static method which is add one more argument for object pointer.");
            }
            this.isAsync = isAsync;
            this.isStatic = isStatic;
            this.hasReturnValue = hasReturnValue;
        }
    }
}