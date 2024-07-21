# Damn Script
Damn Script is a programming language inspired by .bat/.sh, designed for writing scripts and game logic for individual
objects. The code is divided into "regions," each executed concurrently, allowing the processing of many independent
objects in a single frame. Additionally, the language supports state saving for implementing save and load functionality.
It is an untyped language that compiles to bytecode for a virtual machine. It can compile in real-time when parsing
files or compile to a file containing bytecode, significantly speeding up loading and saving memory usage.

## Theory
### Bytecode
When a script is compiled, it is converted into low-level bytecode, which is then executed by the virtual machine.
Bytecode consists of a sequence of instructions, each performing an action (opcodes). Each opcode has the following format:
```
[4 bytes - instruction identifier] ?[x bytes - data for the instruction] | [4 bytes - instruction identifier]...
```
The identifier determines the type of opcode and has a fixed size of 4 bytes. Data may follow the opcode or the next
opcode may start immediately. The size is determined by the opcode's structure, which can be found in the source code
`Runtimes/VirtualMachines/OpCodes`. Opcodes allow for jumping to a specific section of code, loading data onto the
stack, or calling a function.

### ScriptValue
The ScriptValue structure is used to store data during script execution. It is a 12-byte structure with the following format:
```
[4 bytes - data type] [8 bytes - data]
```
Operations must be conducted with data not exceeding 8 bytes. Otherwise, data will be truncated. To avoid this, 
operations on structures larger than 8 bytes must either be split into several operations or use pointers.
Permissible data types:
- Invalid - indicates that the value was initialized incorrectly and cannot be processed.
- Primitive - a primitive data type such as bool, int, float, double, char, etc. Their size will still be 8 bytes.
- Pointer - a pointer to data, where the value can be located anywhere in memory with an unfixed size.
- ReferenceUnsafePointer - a pointer to a reference type. Not recommended in .NET if the operation spans multiple 
frames, as GC may move the object and invalidate the pointer. However, its use is justified but should be carefully controlled.
- ReferenceSafePointer - a pointer to a reference type that uses a mechanism to pin the object in memory, recommended for safe usage.

`ScriptValue` also includes built-in methods for mathematical operations, comparisons, and bitwise operations. 
Be cautious, as these do not check data types and can cause errors if `ScriptValue` contains a pointer!

### Thread
In DamnScript, a thread is an independent unit that executes a specific code region. After executing the last opcode, 
it stops and is removed from the thread list. The `ExecuteNext()` call will process as many opcodes as possible until all
are executed or the thread enters a waiting state due to an asynchronous method call. The call will check if the 
operation is complete and resume the thread if it is. Each thread has 128 bytes allocated for its stack.

### Registers
The virtual machine has 4 registers used for storing data during script execution. Currently, their use is permitted 
only for loops (for), as this requires storing the indexer for an extended period. Registers are not part of 
`ScriptValue`; their size is fixed at 8 bytes. When a variable is loaded into a register, it will be taken from the 
stack, and when unloaded, it will be put back onto the stack. The registers operate independently of each other, 
and modifying one will not explicitly affect another.

### Stack
For correct code execution, values and data must be saved during runtime. Each thread has its own stack, fixed at
128 bytes. Stack overflow triggers an exception. The stack stores local data and passes arguments to C# methods. 
Ensure the stack is kept clean, as it does not automatically clear. If a C# method puts something on the stack, 
another method must use these data to avoid clutter and potential exceptions!

### Calling C# Methods
DamnScript can call C# methods during script execution. A method can take up to 10 arguments (9 for non-static methods, 
as one argument is used for the object pointer). The method must also operate on `ScriptValue`. Return values are 
supported and will be loaded onto the stack of the thread from which the call originated. 
Asynchronous methods using `Task<ScriptValue>` are also supported.
Example method:
```csharp
public static ScriptValue AddAndPrint(ScriptValue first, ScriptValue second)
{
    var a = first.intValue;
    var b = second.intValue;
    var result = a + b;
    Console.WriteLine(result);
    return new ScriptValue(result);
}
```
This method takes two arguments, adds them, prints the result, and returns the sum, which will be stored on the stack.

### Strings
DamnScript supports strings. However, they are not stored directly in `ScriptValue` or bytecode. During compilation, 
when the compiler encounters a string, it calculates its hash and saves it in a string table. In bytecode, 
instead of the string, the hash will be stored, which will be used to get a pointer to the string during execution.
When using a string inside a DamnScript script, as shown in the example:
```
PlaySound("OnWaypoint1");
```
The UnsafeString structure will be used. There is nothing dangerous about it; it resides in unmanaged memory and is 
not subject to garbage collection. It is fully prepared for use and can be easily converted to `System.String` 
(though in this case, a `System.String` allocation will occur, as a copy of the `UnsafeString`) for use in high-level code.
Additionally, when passing a string from a C# method to a script, it must be wrapped in a `ScriptValue` and either 
pinned or a pointer to it must be obtained, since it is a reference type!
Furthermore, if a duplicate string is encountered (within the current script only), it will not be copied.

### Assembler
Any DamnScript code compiles to bytecode, which can be represented as text (assembler). An example:
```
PUSH 1
PUSH 2
CALL AddAndPrint 2
```
Each line is one instruction. The first word is the instruction identifier, followed by data. 
This example code will:
- Push the number 1 onto the stack
- Push the number 2 onto the stack
- Call the `AddAndPrint` method with two arguments

In reality, bytecode can be much more complex. Supported opcodes:
- CALL x, y (Native call) - call a C# method. x - method name, y - number of arguments.
- PUSH x (Push to stack) - push a constant value onto the stack. x - value.
- EXPCALL x (Expression call) - call a built-in expression, such as addition, subtraction, etc. x - identifier 
(listed in `ExpressionCall.ExpressionCallType`)
- SAVE (Save) - creates a save point for later loading.
- JNE x (Jump not equal) - jump to address if the last two values on the stack are not equal. x - absolute 
offset in bytecode from the beginning.
- JEQ x (Jump equal) - jump to address if the last two values on the stack are equal. x - absolute offset in 
bytecode from the beginning.
- JMP x (Jump) - jump to address. x - absolute offset in bytecode from the beginning.
- PUSHSTR x (Push string) - push a string onto the stack. x - string hash in the string table.
## Language Syntax
```
region Main {
    GoToWaypointAsync(GetActor(), "Way1");
    PlaySound("OnWaypoint1");
    GoToWaypoint(GetActor(), "Way2");
    PlaySound("Shot");
    Die();
    
    if (IsDead(GetActor()) {
        Print("Actor is dead");
    }
    elseif (IsAlive(GetActor()) {
        Print("Actor is alive");
    }
    else {
        Print("HOW?!");
    }
}
```
Let's break down this example:
- region - keyword for declaring a region. Code is divided into regions.
- Main - the region's name, which is the main region executed by default.
- { } - code block. Everything inside braces is executed within the region.
- GetActor() - C# method call without arguments.
- "Way1" - string.
- GoToWaypointAsync - C# method call, an asynchronous method that pauses the thread during execution. 
This method is called last in the line, as other methods are arguments.
- PlaySound - C# method call, executed after the actor reaches "Way1".
- GoToWaypoint - C# method call, executed after "OnWaypoint1" sound is played.
- PlaySound - C# method call, executed after the actor reaches "Way2".
- Die - C# method call, executed after the "Shot" sound is played.
- Conditional check in C-style. It checks if the actor is alive and executes the appropriate code block. 
Conditions support operators `&&,` `||`, `==`, `!=`, `>`, `<`, `>=`, `<=`, and C# method calls as conditions.

Each expression must end with a semicolon (;) (except conditions and loops). A new region can be declared after the 
previous one without restrictions. Each region must have a unique name not exceeding 32 characters!

## API
99% of functionality is implemented without GC allocations, using pointers (safe mode with ref keyword is available). 
Strings can be `System.String` or `UnsafeString`, but high-level code should prefer `System.String` due to method 
completeness and manual memory management in `UnsafeString`.

### Initialization
DamnScript does not require special initialization. To start, load the script into memory and create a thread for it:
```csharp
var fileStream = File.Open("Test1.ds", FileMode.Open);
var scriptData = ScriptEngine.LoadScript(fileStream, "Test1");
var thread = ScriptEngine.RunThread(scriptData);
```
Step-by-step:
1. Open a stream for the file containing the script. It can be any stream inheriting from `Stream`.
2. Load the script into memory. The first argument is the stream, the second is the script's name, which must be 
unique and under 32 characters.
3. Create a thread for the script on the virtual machine, that will start execution of the Main region, unless otherwise specified.


After a thread is created, it does NOT start automatically. Each game frame (or at any other moment), you need to call
the `ScriptEngine.ExecuteNext()` method. This method will process each thread that was created and has not 
yet completed its work. It should work like this:
```csharp
while (ScriptEngine.ExecuteNext())
    Thread.Sleep(15);
```
`ScriptEngine.ExecuteNext()` returns `true` if there are threads that have not yet finished their work. Otherwise, it 
returns `false`. For our program, this means it is time to finish its work, but in your case, the logic might be more complex.
If you need to stop a thread's execution, you can do so by calling the `Dispose()` method on the thread.
If, after executing a script, your program needs to continue working, you might need to unload the script from memory.
In this case, you can use the `ScriptEngine.UnloadScript()` method. Important! Do not unload a script if a thread with 
this script is still running! This can lead to unpredictable results.

## Roadmap
- [x] Basic functionality
- [x] Ability to call C# methods
- [x] Conditions (if, elseif, else)
- [x] Loops (for, while)
- [x] Unity support
- [ ] Additional security measures
- [ ] State saving
- [ ] State loading
- [ ] Unloading unused metadata
- [ ] Additional safe memory management checks
- [ ] Soft migration when changing bytecode
- [ ] Hot-reload

... And much more. :)

## Support
You can also support the project by buying me a coffee, so I can continue developing this project through the night.  
[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/rietmon)