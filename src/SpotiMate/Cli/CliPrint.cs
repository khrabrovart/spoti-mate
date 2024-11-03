namespace SpotiMate.Cli;

public static class CliPrint
{
    public static void Info(string message, bool writeLine = true)
    {
        PrintInColor(message, ConsoleColor.White, writeLine);
    }

    public static void Success(string message, bool writeLine = true)
    {
        PrintInColor(message, ConsoleColor.Green, writeLine);
    }

    public static void Warning(string message, bool writeLine = true)
    {
        PrintInColor(message, ConsoleColor.Yellow, writeLine);
    }

    public static void Error(string message, bool writeLine = true)
    {
        PrintInColor(message, ConsoleColor.Red, writeLine);
    }

    public static void EmptyLine(int lines = 1)
    {
        for (var i = 0; i < lines; i++)
        {
            Console.WriteLine();
        }
    }

    private static void PrintInColor(string message, ConsoleColor color, bool writeLine)
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
