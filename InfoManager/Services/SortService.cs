using System.Collections.ObjectModel;
using System.Reflection;

namespace InfoManager.Services;

// reflection-based sorting service
public abstract class SortService<T>
{
    public static List<T> Sort(ObservableCollection<T> source, string propertyName, bool isAscending = true)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            throw new ArgumentNullException(nameof(propertyName), @"Property name cannot be null or empty.");
        }

        var propertyInfo = typeof(T).GetProperty(propertyName,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (propertyInfo == null)
        {
            throw new ArgumentException($@"Invalid property name: {propertyName}", nameof(propertyName));
        }

        var sortedList = isAscending
            ? source.OrderBy(e => GetPropertyValue(e, propertyInfo)).ToList()
            : source.OrderByDescending(e => GetPropertyValue(e, propertyInfo)).ToList();

        return sortedList;
    }

    private static object? GetPropertyValue(T obj, PropertyInfo propertyInfo) => propertyInfo.GetValue(obj, null);
}