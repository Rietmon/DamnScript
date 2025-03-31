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

### 📌 Version Numbering
Format: `x.y.z`
- **x** — Major version
- **y** — Release type (`0` — alpha, `1` — beta, `2` — stable release)
- **z** — Build number

### 🚀 Using in .NET

#### 🔹 As a Binary File
1. **Download** the required version from the [GitHub releases](https://github.com/Rietmon/DamnScript/releases).
2. **Add to your project** the files `DamnScript.dll` and `Antlr4.Runtime.dll`.
3. **Done!** Ready to use.

#### 🔹 As Source Code
1. **Clone** the DamnScript repository.
2. **Import** the `DamnScript/` folder (including `Runtimes` and `Parsing`) into your project.
3. **Add Antlr4** (either as a binary file or a NuGet package).
4. **Done!** You’re ready to go.

### 🎮 Using in Unity
1. **Download** the required version from the [GitHub releases](https://github.com/Rietmon/DamnScript/releases).
2. **Import the files** from the archive into your Unity project.
3. **Done!** Ready to use.

> 🔧 Possible Directives:
> - ARCHITECTURES `DAMN_SCRIPT_ENABLE_MONO` — switches the mode to support Mono (default for Unity).
> - CONFIGS `DAMN_SCRIPT_ENABLE_TARGET_32BIT` — enables support for 32-bit platforms.
> - CONFIGS `DAMN_SCRIPT_STACK_SIZE_16` / `DAMN_SCRIPT_STACK_SIZE_64` — sets the script stack size (default: 32 elements).
> - CONFIGS `DAMN_SCRIPT_ENABLE_16_BIT_OPCODES` / `DAMN_SCRIPT_ENABLE_32_BIT_OPCODES` /  
    `DAMN_SCRIPT_ENABLE_64_BIT_OPCODES` — sets the opcode size (default: 8 bits).  
    Does not disable compiler alignment, so the result may not be as expected.  
    When using this directive, it is recommended to also use `DAMN_SCRIPT_DISABLE_ALIGNMENT_OPCODES`!
> - CONFIGS `DAMN_SCRIPT_DISABLE_ALIGNMENT_OPCODES` — disables opcode alignment (opcode size may be less than 8 bytes) from the compiler (enabled by default).
> - CONFIGS `DAMN_SCRIPT_ENABLE_ADDITIONAL_CHECKS` — enables additional memory safety checks (disabled by default).
> - CONFIGS `DAMN_SCRIPT_DISABLE_BUILTIN_METHODS` — disables built-in methods.
> - CONFIGS `DAMN_SCRIPT_DISABLE_RAW_METHOD_INFO` — disables the use of NoCopy methods for MethodInfo (enabled by default).
> - CONFIGS `DAMN_SCRIPT_DISABLE_ASYNC_PINNING` — allows the use of unpinned references in asynchronous methods in .NET (not applicable to Unity, where it is always allowed).
> - CONFIGS `DAMN_SCRIPT_DISABLE_BUILTIN_METHODS` — disables built-in methods.
> - CONFIGS `DAMN_SCRIPT_DISABLE_ASYNC_PINNING` — allows the use of unpinned references in asynchronous methods in .NET (not applicable to Unity, where it is always allowed).
> - DEBUGS `DAMN_SCRIPT_ENABLE_EXECUTION_LOG` — enables execution debugging.
> - DEBUGS `DAMN_SCRIPT_ENABLE_MEMORY_DEBUG` — enables memory debugging.
> - DEBUGS `DAMN_SCRIPT_ENABLE_ASSEMBLER_DEBUG` — enables assembler debug messages.
> - DEBUGS `DAMN_SCRIPT_PINNING_DEBUG` — enables pinning debugging.

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
public static void Print(ScriptValuePtr value) 
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
