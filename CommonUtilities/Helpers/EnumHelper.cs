using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CommonUtilities.Helpers;

/*
 * @author: LoveDoLove
 * @description:
 * This enum helper class is used to convert an enum to specific types that can be used in the view.
 */
public static class EnumHelper
{
    public static List<T> ToList<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T))
            .Cast<T>()
            .ToList();
    }

    public static SelectList ToSelectList<T>(string? selectedValue = null) where T : Enum
    {
        var enumList = Enum.GetValues(typeof(T))
            .Cast<T>()
            .Select(e => new { Value = e.ToString(), Text = GetDisplayName(e) })
            .ToList();

        return new SelectList(enumList, "Value", "Text", selectedValue);
    }

    public static SelectList ToSelectListWithAllowed<T>(string? selectedValue = null, T[]? allowedValue = default)
        where T : Enum
    {
        var enumList = Enum.GetValues(typeof(T))
            .Cast<T>()
            .Where(e => allowedValue != null && allowedValue.Contains(e))
            .Select(e => new { Value = e.ToString(), Text = GetDisplayName(e) })
            .ToList();
        return new SelectList(enumList, "Value", "Text", selectedValue);
    }

    public static SelectList ToSelectListWithExcluded<T>(string? selectedValue = null, T? valueToExclude = default)
        where T : Enum
    {
        var enumList = Enum.GetValues(typeof(T))
            .Cast<T>()
            .Where(e => !e.Equals(valueToExclude))
            .Select(e => new { Value = e.ToString(), Text = GetDisplayName(e) })
            .ToList();
        return new SelectList(enumList, "Value", "Text", selectedValue);
    }

    public static SelectList ToSelectListWithExcluded<T>(string? selectedValue = null, T[]? valuesToExclude = default)
        where T : Enum
    {
        var enumList = Enum.GetValues(typeof(T))
            .Cast<T>()
            .Where(e => valuesToExclude != null && !valuesToExclude.Contains(e))
            .Select(e => new { Value = e.ToString(), Text = GetDisplayName(e) })
            .ToList();
        return new SelectList(enumList, "Value", "Text", selectedValue);
    }

    public static string GetDisplayName<T>(T enumValue) where T : Enum
    {
        var fieldInfo = typeof(T).GetField(enumValue.ToString());
        var displayAttributes = fieldInfo?.GetCustomAttributes(typeof(DisplayAttribute), false)
            as DisplayAttribute[];

        return displayAttributes?.FirstOrDefault()?.Name ?? enumValue.ToString();
    }

    public static bool IsAllowValue<T>(T currentValue, T[]? allowedValue = default) where T : struct, Enum
    {
        return allowedValue != null && allowedValue.Contains(currentValue);
    }
}