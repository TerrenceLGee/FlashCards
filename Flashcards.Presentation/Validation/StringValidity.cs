using System.Globalization;

namespace Flashcards.Presentation.Validation;

public static class StringValidity
{
    public static bool IsValidString(string input)
    {
        return !string.IsNullOrWhiteSpace(input);
    }

    public static bool IsValidDateString(string dateString, string dateFormat, CultureInfo info)
    {
        return DateTime.TryParseExact(dateString, dateFormat, info, DateTimeStyles.None, out _);
    }
}