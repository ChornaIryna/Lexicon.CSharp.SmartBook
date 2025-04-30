using System.Text;

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

    public static void PrintActionTitle(string actionTitle, string additionalText)
    {
        Console.WriteLine($"""
            {actionTitle}

            {additionalText}
            """);
    }

    public static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
        HandleReturn();
    }

    public static void PrintInfo(string message, bool showReturnPrompt = true)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(message);
        Console.ResetColor();
        if (showReturnPrompt)
            HandleReturn();
    }

    public static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"An error occurred: {message.ToUpperInvariant()}");
        Console.ResetColor();
        HandleReturn();
    }

    public static void PrintWarning(string warningText, bool showReturnPrompt = true)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(warningText);
        Console.ResetColor();
        if (showReturnPrompt)
            HandleReturn();
    }

    public static void PrintInfoList(IEnumerable<object>? items, bool showReturnPrompt = true)
    {
        if (items == null || !items.Any())
        {
            PrintWarning("No items to display.");
            return;
        }

        StringBuilder stringBuilder = new();
        foreach (var item in items)
            stringBuilder.AppendLine(item?.ToString() ?? "Unknown item");

        PrintInfo(stringBuilder.ToString(), showReturnPrompt);
    }

    public static string GetValidatedInput(string prompt)
    {
        while (true)
        {
            string? input = GetInput(prompt);
            if (!string.IsNullOrWhiteSpace(input))
                return input;
            PrintWarning("Input cannot be empty. Please try again.", false);
        }
    }

    private static string GetInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    private static void HandleReturn()
    {
        Console.WriteLine("""
            --------------------------------------------
            Press any key to return to the menu...
            """);
        Console.ReadKey(true);
    }
}
