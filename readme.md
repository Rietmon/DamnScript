<h1 align="center">
🔥 DamnScript 🔥
</h1>

**DamnScript** is a high-performance scripting language inspired by Ren'Py and Bash.  
It is designed for writing game behavior logic with deep integration into C# and Unity.

---

## 🚀 Features

- ⚡ Lightning-fast execution and loading speed
- 💾 Minimal memory and resource consumption
- 🛠 JIT and AOT compilation
- 🧱 No managed allocations (*when using unmanaged types and corresponding API)
- 🛡 High level of security
- 🔗 Full support for calling C# methods (including async)
- 🔁 Control structures: `if`, `else`, `for`, `while`
- ✍️ Working with all C# data types
- ✉️ String support and interning
- 💾 State serialization and deserialization
- 🎮 Unity 2021.1.0f1+ support (not tested on versions below 2021.1.0f1)
- 🧬 Compatible with .NET 7.0+ (not tested on versions below 7.0)

---

## ⚡ Quick Start

### For .NET Project
1. Download the project from [GitHub](https://github.com/Rietmon/DamnScript)
2. Import the `DamnScript/` folder (including `Runtimes/` and `Parsing/`) into your .NET 7+ project
3. Enjoy!

### For Unity (as a build)
1. Download the project from [GitHub](https://github.com/Rietmon/DamnScript)
2. Build `DamnScript` into a library using .NET 7+, configuring the directives
3. Connect `DamnScript.dll` in Unity
4. Done!

### For Unity (as source code)
1. Download the project from [GitHub](https://github.com/Rietmon/DamnScript)
2. Import `DamnScript/`, including `Runtimes/` and `Parsing/` into Unity
3. Build DamnScript for the target platform
4. Connect `Antlr4.Runtime.dll` and generated files from `gen`, after the build is complete
5. Profit ✅

> 🛠 In the future, a build with releases will be available — everything included, minimal hassle.

> 🔧 Possible directives:
> - ARCHITECTURES `DAMN_SCRIPT_ENBALE_MONO` - switches the mode to support Mono (by default, it applies to Unity)
> - CONFIGS `DAMN_SCRIPT_ENBALE_TARGET_32BIT` - switches the mode to support 32-bit platforms
> - CONFIGS `DAMN_SCRIPT_SCRIPT_VALUE_SIZE_12` - reduces ScriptValue size to 12 bytes (not recommended without clear understanding)
> - CONFIGS `DAMN_SCRIPT_STACK_SIZE_16`/`DAMN_SCRIPT_STACK_SIZE_64` - sets the stack size for scripts (default is 32 elements)
> - CONFIGS `DAMN_SCRIPT_ENABLE_16_BIT_OPCODES`/`DAMN_SCRIPT_ENABLE_32_BIT_OPCODES`/`DAMN_SCRIPT_ENABLE_64_BIT_OPCODES` - sets the size of opcodes (default is 8 bits)
> - CONFIGS `DAMN_SCRIPT_ENABLE_ADDITIONAL_CHECKS` - enables additional memory safety checks (disabled by default)
> - CONFIGS `DAMN_SCRIPT_DISABLE_BUILTIN_METHODS` - disables built-in methods
> - CONFIGS `DAMN_SCRIPT_DISABLE_ASYNC_PINNING` - allows using non-pinned reference in async methods in .NET (not applies to Unity, here it is always allowed)
> - DEBUGS `DAMN_SCRIPT_ENABLE_MEMORY_DEBUG` - enables memory debug
> - DEBUGS `DAMN_SCRIPT_ENABLE_ASSEMBLER_DEBUG` - enables assembler debug messages
> - DEBUGS `DAMN_SCRIPT_PINNING_DEBUG` - enables pinning debug

---

## 🧠 How It Works

- **Scripts are compiled into bytecode**, executed by a virtual machine
- JIT and AOT are supported: you can interpret `.ds` or run pre-compiled `.dsc`
- Pseudo-multithreaded VM: each pseudo-thread executes its region independently
- **Stack-based execution model** + 4 registers are used
- Safe `ScriptValuePtr` structure stores any data (including pointers and references)
- Callable C# methods work with `ScriptValuePtr`, including async `Task<ScriptValuePtr>`
- Assembler-like bytecode allows easy debugging and code optimization

---

## 📜 Example Script

```
region Main {
    GoToWaypointAsync(GetActor(), "Way1");
    PlaySound("OnWaypoint1");
    GoToWaypoint(GetActor(), "Way2");
    PlaySound("Shot");
    Die();
    
    if (IsDead(GetActor())) {
        Print("Actor is dead");
    }
    elseif (IsAlive(GetActor())) {
        Print("Actor is alive");
    }
    else {
        Print("HOW?!");
    }
}

region AnythingElse {
    Print("Arrived at Waypoint 1");
}
```

---

## 🧩 Connecting to C#

```csharp
public static void Log(ScriptValuePtr value) 
{
    Console.WriteLine(value.IntValue.ToString()); // This method will be called from the script
}

public static void TestRun() 
{
	ScriptEngine.RegisterNativeMethod((Action<ScriptValuePtr>)Log); // Register a native method
    
    var fileStream = File.Open("Test1.ds", FileMode.Open); // Opening the stream to read the script
    var scriptData = ScriptEngine.LoadScript(fileStream, "Test1"); // Loading the script into memory (JIT compilation in this case)
    var thread = ScriptEngine.RunThread(scriptData, "Main"); // Starting the thread and executing the Main region

    // Sending the machine a command to execute EVERY thread, as long as there is work to do or until it goes into a wait state
    while (ScriptEngine.ExecuteNext()) 
    {
        // When a thread enters a wait state (during asynchronous calls), for example, wait for 15 ms. 
        // Then try to get the result again and continue execution
        Thread.Sleep(15); 
    }
    
    ScriptEngine.UnloadScript(scriptData); // Freeing memory used by the script if no other threads are using it
}
```

---

## 🤝 How to Contribute
- ✍️ Write scripts in DamnScript and share them
- 📚 Create tutorials and guides
- 🐛 Open Issues with bugs and ideas
- 🔧 Make Pull Requests with improvements

---

## 🔭 Plans (Roadmap)

- ✅ Basic functionality
- ✅ Unity and C# call support
- ✅ Conditions, loops, serialization
- ✅ Memory checks
- ✅ Manual saves
- 🔧 Unit Tests
- ⏳ Auto-saves
- ⏳ Unloading inactive metadata
- ⏳ Migration when bytecode changes
- ⏳ Hot-reload
- 💡 And much more...
