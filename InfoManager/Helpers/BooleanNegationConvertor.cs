using Microsoft.UI.Xaml.Data;

namespace InfoManager.Helpers;

public class BooleanNegationConvertor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) => InvertValue(value);

    public object ConvertBack(object value, Type targetType, object parameter, string language) => InvertValue(value);

    private static object InvertValue(object value)
    {
        if (value is bool booleanValue)
        {
            return !booleanValue;
        }

        return value; // not a boolean, return as is
    }
}