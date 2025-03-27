<h1 align="center">
🔥 DamnScript 🔥
</h1>

**DamnScript** — высокопроизводительный язык сценариев, вдохновлённый Ren'Py и Bash.  
Разработан для написания логики игрового поведения с глубокой интеграцией в C# и Unity.

---

## 🚀 Особенности

- ⚡ Молниеносная скорость выполнения и загрузки
- 💾 Минимальное потребление памяти и ресурсов
- 🛠 JIT и AOT компиляции
- 🧱 Без управляемых аллокаций (*при использовании неуправляемых типов и соответствующего API)
- 🛡 Высокий уровень безопасности
- 🔗 Полная поддержка вызова C# методов (включая async)
- 🔁 Управляющие конструкции: `if`, `else`, `for`, `while`
- ✍️ Работа со всеми типами данных C#
- ✉️ Поддержка строк и их интернирования
- 💾 Сериализация и десериализация состояния
- 🎮 Поддержка Unity 2021.1.0f1+ (на версиях ниже 2021.1.0f1 не тестировалось)
- 🧬 Совместим с .NET 7.0+ (на версиях ниже 7.0 не тестировалось)

---

## ⚡ Быстрый старт

### Для .NET проекта

1. Скачай проект с [GitHub](https://github.com/Rietmon/DamnScript)
2. Импортируй папку `DamnScript/` (включая `Runtimes/` и `Parsing/`) в свой .NET 7+ проект
3. Пользуйся на здоровье

### Для Unity (как сборка)

1. Скачай проект с [GitHub](https://github.com/Rietmon/DamnScript)
2. Собери `DamnScript` в библиотеку с использованием .NET 7+, настроив директивы
3. Подключи `DamnScript.dll` в Unity
4. Готово!

### Для Unity (как исходный код)

1. Скачай проект с [GitHub](https://github.com/Rietmon/DamnScript)
2. Импортируй `DamnScript/`, включая `Runtimes/` и `Parsing/` в Unity
3. Собери DamnScript под нужную платформу
4. Подключи `Antlr4.Runtime.dll` и сгенерированные файлы из `gen`, после окончания сборки
5. Profit ✅

> 🛠 В будущем будет доступна сборка с релизами — всё включено, минимум возни.

> 🔧 Возможные директивы:
> - ARCHITECTURES `DAMN_SCRIPT_ENBALE_MONO` — переключает режим на поддержку Mono (по умолчанию применяется к Unity)
> - CONFIGS `DAMN_SCRIPT_ENBALE_TARGET_32BIT` — включает поддержку 32-битных платформ
> - CONFIGS `DAMN_SCRIPT_SCRIPT_VALUE_SIZE_12` — уменьшает размер `ScriptValue` до 12 байт (не рекомендуется без чёткого
    понимания последствий)
> - CONFIGS `DAMN_SCRIPT_STACK_SIZE_16` / `DAMN_SCRIPT_STACK_SIZE_64` — задаёт размер стека для скриптов (по умолчанию —
    32 элемента)
> - CONFIGS `DAMN_SCRIPT_ENABLE_16_BIT_OPCODES` / `DAMN_SCRIPT_ENABLE_32_BIT_OPCODES` /
    `DAMN_SCRIPT_ENABLE_64_BIT_OPCODES` — устанавливает размер опкодов (по умолчанию — 8 бит)
> - CONFIGS `DAMN_SCRIPT_ENABLE_ADDITIONAL_CHECKS` — включает дополнительные проверки безопасности памяти (по умолчанию
    отключены)
> - CONFIGS `DAMN_SCRIPT_DISABLE_BUILTIN_METHODS` — отключает встроенные методы
> - CONFIGS `DAMN_SCRIPT_DISABLE_ASYNC_PINNING` — разрешает использование неприкреплённых ссылок в асинхронных методах в
    .NET (не применяется к Unity, там всегда разрешено)
> - DEBUGS `DAMN_SCRIPT_ENABLE_MEMORY_DEBUG` — включает отладку памяти
> - DEBUGS `DAMN_SCRIPT_ENABLE_ASSEMBLER_DEBUG` — включает отладочные сообщения ассемблера
> - DEBUGS `DAMN_SCRIPT_PINNING_DEBUG` — включает отладку pinning'а

---

## 🧠 Как это работает

- **Скрипты компилируются в байткод**, исполняемый виртуальной машиной
- Поддерживается JIT и AOT: можно интерпретировать `.ds` или запускать заранее скомпилированные `.dsc`
- Псевдо-многопоточная VM: каждый псевдопоток исполняет свой регион независимо
- Используется **стековая модель выполнения** + 4 регистра
- Безопасная структура `ScriptValuePtr` принимает любые данные (в том числе указатели и ссылки)
- Вызываемые C# методы работают с `ScriptValuePtr`, включая асинхронные `Task<ScriptValuePtr>`
- Ассемблероподобный байткод позволяет легко отлаживать и оптимизировать код

---

## 📜 Пример скрипта

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

## 🧩 Подключение к C#

```csharp
public static void Log(ScriptValuePtr value) 
{
    Console.WriteLine(value.IntValue.ToString()); // Этот метод будет вызван из скрипта
}

public static void TestRun() 
{
    ScriptEngine.RegisterNativeMethod((Action<ScriptValuePtr>)Log); // Регистрируем нативный метод
    
    var fileStream = File.Open("Test1.ds", FileMode.Open); // Открытие стрима для чтения скрипта
    var scriptData = ScriptEngine.LoadScript(fileStream, "Test1"); // Загрузка скрипта в память (в данном случае JIT компиляция)
    var thread = ScriptEngine.RunThread(scriptData, "Main"); // Запуск потока и выполнение региона Main

    // Отправляем машине команду исполнять КАЖДЫЙ поток, пока есть что или пока потом не встанет в ожидание
    while (ScriptEngine.ExecuteNext()) 
    {
        // При попадании потока в сотояние ожидания (при асинхронных вызовах) для примера ждем 15 мс. 
        // Затем попробовать снова получить результат и продолжить выполнение
        Thread.Sleep(15); 
    }
    
    ScriptEngine.UnloadScript(scriptData); // Освобождаем память, занятую скриптом, если ни один другой поток его не использует
}
```

---

## 🤝 Как помочь проекту

- ✍️ Пиши скрипты на DamnScript и делись ими
- 📚 Делай туториалы и гайды
- 🐛 Заводи Issue с багами и идеями
- 🔧 Делай Pull Request’ы с улучшениями

---

## 🔭 Планы (Roadmap)

- ✅ Базовый функционал
- ✅ Поддержка Unity и C# вызовов
- ✅ Условия, циклы, сериализация
- ✅ Проверки памяти
- ✅ Ручные сейвы
- 🔧 Unit Tests
- ⏳ Автосейвы
- ⏳ Выгрузка неактивных метаданных
- ⏳ Миграция при изменении байткода
- ⏳ Hot-reload
- 💡 И многое другое…