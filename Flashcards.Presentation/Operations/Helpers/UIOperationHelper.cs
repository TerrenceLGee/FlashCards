using System.Globalization;
using Spectre.Console;
using Flashcards.Core.Results;
using Flashcards.Presentation.Validation;

namespace Flashcards.Presentation.Operations.Helpers;

public static class UIOperationHelper
{
    public static void WriteResult(Result result)
    {
        if (result.IsSuccess)
        {
            AnsiConsole.MarkupLine("[green]Success[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Error: [/]{result.ErrorMessage}");
        }
    }

    public static void WriteResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            AnsiConsole.MarkupLine($"[green]Success:[/] {result.Value}");
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] {result.ErrorMessage}");
        }
    }

    public static DateTime GetValidDateTime(string dateString, string dateFormat, CultureInfo info)
    {
        return DateTime.ParseExact(dateString, dateFormat, info);
    }

    public static string GetValidDateString(string format, CultureInfo info)
    {
        string dateString = GetValidInput($"Enter date in format: '{format}' ", "blue");

        while (!StringValidity.IsValidDateString(dateString, format, info))
        {
            dateString = GetValidInput($"Enter date in format: '{format}'", "blue");
        }

        return dateString;
    }

    public static string GetValidInput(string message, string color = "yellow")
    {
        string validString = string.Empty;

        while (!StringValidity.IsValidString(validString))
        {
            validString = AnsiConsole.Ask<string>($"[{color}]{message}[/]");
        }

        return validString;
    }

    public static void DisplayMessage(string message, string color = "red")
    {
        AnsiConsole.MarkupLine($"[{color}]{message}[/]");
    }

    public static int GetValidNumber(string message, string color = "yellow")
    {
        return AnsiConsole.Ask<int>($"[{color}]{message}[/]");
    }

    public static void ClearTheScreen(string color = "yellow")
    {
        AnsiConsole.MarkupLine($"[{color}]Press any key to continue[/]");
        Console.ReadKey();
        Console.Clear();
    }
}