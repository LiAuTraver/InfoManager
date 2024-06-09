using System.ComponentModel;
using Microsoft.UI.Xaml.Data;

namespace InfoManager.Helpers;

public class SafeStringParseConverter : IValueConverter
{
    public SafeStringParseConverter()
    {
        throw new WarningException("Do not use this converter. No need to use it.");
    }
    // public event EventHandler<ConversionExceptionEventArgs>? ConversionException;

    public object? Convert(object value, Type targetType, object parameter, string language) => value.ToString();

    public object? ConvertBack(object? value, Type targetType, object parameter, string language) =>
        // try
        // {
        //     value
        //         ?.ToString()
        //         ?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
        //         .Select(s => double.Parse(s.Trim()))
        //         .ToList();
        //     // if `ToList()` is successfully executed, return the value.
        //     // the value is still a string, not a list of doubles
        //     // that is because string is immutable in C#, so the `value` is not changed}
        //     return value;
        // }
        // catch(Exception e)
        // {
        //     ConversionException?.Invoke(this, new ConversionExceptionEventArgs { Exception = e });
        //     return null;
        // }
        value;
}

public class ConversionExceptionEventArgs : EventArgs
{
    public required Exception Exception
    {
        get;
        set;
    }
}