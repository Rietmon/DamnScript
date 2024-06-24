namespace DamnScript.Runtimes.Debugs;

public static class Debugging
{
    public static void Log(string message) => WriteToConsole(message, ConsoleColor.White);
    public static void LogWarning(string message) => WriteToConsole(message, ConsoleColor.Yellow);
    public static void LogError(string message) => WriteToConsole(message, ConsoleColor.Red);
    
    private static void WriteToConsole(string message, ConsoleColor color)
    {
        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = previousColor;
    }
}