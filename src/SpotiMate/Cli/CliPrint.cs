namespace SpotiMate.Cli;

public static class CliPrint
{
    public static void PrintInfo(string message, bool writeLine = true) =>
        ColorPrint(message, ConsoleColor.Gray, writeLine);

    public static void PrintSuccess(string message, bool writeLine = true) =>
        ColorPrint(message, ConsoleColor.Green, writeLine);

    public static void PrintWarning(string message, bool writeLine = true) =>
        ColorPrint(message, ConsoleColor.Yellow, writeLine);

    public static void PrintError(string message, bool writeLine = true) =>
        ColorPrint(message, ConsoleColor.Red, writeLine);

    private static void ColorPrint(string message, ConsoleColor color, bool writeLine)
    {
        Console.ForegroundColor = color;

        if (writeLine)
        {
            Console.WriteLine(message);
        }
        else
        {
            Console.Write(message);
        }

        Console.ResetColor();
    }
}