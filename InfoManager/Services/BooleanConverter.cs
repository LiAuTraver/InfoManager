using Microsoft.UI.Xaml.Data;

namespace InfoManager.Services;

public class BooleanConverter : IValueConverter
{
    public new object Convert(object value, Type targetType, object parameter, string language)
    {
        return value;
    }
    public new object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }
}