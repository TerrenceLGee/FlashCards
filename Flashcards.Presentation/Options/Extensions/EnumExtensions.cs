using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Flashcards.Presentation.Options.Extensions;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        FieldInfo? fieldInfo = enumValue
            .GetType()
            .GetField(enumValue.ToString());

        if (fieldInfo?.GetCustomAttributes(typeof(DisplayAttribute),
                false) is DisplayAttribute[] descriptionAttributes && descriptionAttributes.Length > 0)
        {
            return descriptionAttributes[0].Name!;
        }

        return enumValue.ToString();
    }
}