using Microsoft.UI.Xaml.Data;

namespace InfoManager.Helpers;

public class SafeStringParseConverter : IValueConverter
{
    public SafeStringParseConverter()
    {
        throw new NotSupportedException("Stop using this converter. It's not needed.");
    }
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        return value.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object parameter, string language)
    {
        value
            ?.ToString()
            ?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => double.Parse(s.Trim()))
            .ToList();
        // if `ToList()` is successfully executed, return the value.
        // the value is still a string, not a list of doubles
        // that is because string is immutable in C#, so the `value` is not changed
        return value;
    }
}