




namespace SmartBook.ConsoleUI.Helpers;
public class ConsoleHelper
{
    public static void ClearConsole()
    {
        Console.Clear();
        Console.WriteLine($"""
         _______________________________
        |  SmartBook - Library System   |
        |_______________________________|
        """);
    }

    internal static void WriteActionTitle(string actionTitle, string additionalText)
    {
        Console.WriteLine($"""
            {actionTitle}

            {additionalText}
            """);
    }

    public static void HandleReturn()
    {
        Console.WriteLine("Press any key to return to the menu...");
        Console.ReadKey(true);
    }

    public static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error: {message.ToUpperInvariant()}");
        Console.ResetColor();
    }

    public static void PrintWarning(string warningText)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(warningText);
        Console.ResetColor();
    }
}
