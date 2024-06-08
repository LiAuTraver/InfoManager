using Microsoft.UI.Xaml.Data;

namespace InfoManager.Services;

public class BoolToSortOrderConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isAscending)
        {
            return isAscending ? "Ascending" : "Descending";
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is string sortOrder)
        {
            return sortOrder == "Ascending";
        }

        return false;
    }
}