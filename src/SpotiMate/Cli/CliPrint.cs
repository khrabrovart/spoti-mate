namespace SpotiMate.Cli;

public static class CliPrint
{
    public static void PrintInfo(string message) => ColorPrint(message, ConsoleColor.Black);
    
    public static void PrintSuccess(string message) => ColorPrint(message, ConsoleColor.Green);
    
    public static void PrintWarning(string message) => ColorPrint(message, ConsoleColor.Yellow);
    
    public static void PrintError(string message) => ColorPrint(message, ConsoleColor.Red);
    
    private static void ColorPrint(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}