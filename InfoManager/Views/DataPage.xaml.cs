using System.ComponentModel;
using System.Runtime.InteropServices;
using CommunityToolkit.WinUI.UI.Controls;
using InfoManager.Models;
using InfoManager.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace InfoManager.Views;

public sealed partial class DataPage : INotifyPropertyChanged
{
    private object? _cellDataTemp;

    private bool _isAscending = true;

    private bool _isEditing;

    private bool _isModified;

    private string _toggleButtonContent = "Ascending";

    public DataPage()
    {
        ViewModel = App.GetService<DataViewModel>();
        IsAscending = true;
        IsEditing = false;
        IsModified = false;
        InitializeComponent();
        NavigationCacheMode = NavigationCacheMode.Enabled;
    }

    public bool IsEditing
    {
        get => _isEditing;
        set
        {
            if (_isEditing == value)
            {
                return;
            }

            _isEditing = value;
            OnPropertyChanged(nameof(IsEditing));
        }
    }

    public bool IsAscending
    {
        get => _isAscending;
        set
        {
            if (_isAscending == value)
            {
                return;
            }

            _isAscending = value;
            OnPropertyChanged(nameof(IsAscending));
            ToggleButtonContent = _isAscending ? "Ascending" : "Descending";
        }
    }

    public bool IsModified
    {
        get => _isModified;
        private set
        {
            if (_isModified == value)
            {
                return;
            }

            _isModified = value;
            OnPropertyChanged(nameof(IsModified));
        }
    }

    public string ToggleButtonContent
    {
        get => _toggleButtonContent;
        set
        {
            if (_toggleButtonContent == value)
            {
                return;
            }

            _toggleButtonContent = value;
            OnPropertyChanged(nameof(ToggleButtonContent));
        }
    }

    public DataViewModel ViewModel
    {
        get;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private async void SaveData(object sender, RoutedEventArgs e)
    {
        var saveDialog = new ContentDialog
        {
            Title = "Save Changes?",
            Content = "Your data may be lost if you leave without saving.",
            PrimaryButtonText = "Save",
            SecondaryButtonText = "Discard",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = ((FrameworkElement)sender).XamlRoot
        };
        var result = await saveDialog.ShowAsync();
        switch (result)
        {
            case ContentDialogResult.None:
            return;
            case ContentDialogResult.Primary:
            await ViewModel.SaveDataToFileAsync(true);
            IsModified = false;
            return;
            case ContentDialogResult.Secondary:
            await ViewModel.SaveDataToFileAsync(false);
            IsModified = false;
            return;
            default:
            await SimpleDialog("Oops! Something went wrong. You shouldn't be here.", "Error", sender);
            return;
        }
    }

    private async void SwitchSortMode(object sender, RoutedEventArgs e)
    {
        // content is `Sort By $(propertyName)`, we drop the `Sort By ` part
        if ((sender as RadioButton)?.Content is not string content)
        {
            await SimpleDialog(@"Oops! Something went wrong. You shouldn't be here.", "Error", sender);
        }
        else
        {
            const string prefix = "Sort by ";
            if (!content.StartsWith(prefix))
            {
                await SimpleDialog("Oops! Something went wrong. You shouldn't be here.", "Error", sender);
                return;
            }

            var propertyName = content[prefix.Length..];
            UpdateSort(propertyName);
        }
    }

    private async void UpdateSort(string propertyName = "")
    {
        ViewModel.SortData(propertyName, IsAscending);
        await Task.CompletedTask; // placeholder for future async operations (?)
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        switch (e.Parameter)
        {
            case null:
            return;
            case string newFilePath:
            if (newFilePath != @"-1")
            {
                ViewModel.OnNavigatedTo(newFilePath);
                return;
            }

            goto default;
            default:
            _ = SimpleDialog("File path is invalid.", "Error", null);
            return;
        }
    }

    private void ToggleAscending(object sender, RoutedEventArgs e)
    {
        IsAscending = !IsAscending;
        ToggleButtonContent = IsAscending ? "Ascending" : "Descending";
        UpdateSort();
    }

    private void ToggleEditing(object sender, RoutedEventArgs e) => IsEditing = !IsEditing;

    private async void AddData(object sender, RoutedEventArgs e) => await AddDataDialog(sender);

    private async Task AddDataDialog(object sender)
    {
        var idBox = new TextBox { Header = "ID" };
        var nameBox = new TextBox { Header = "Name" };
        var gradeStringBox = new TextBox { Header = "Grade" };
        switch (await new ContentDialog
        {
            Title = "Add",
            Content = new StackPanel
            {
                Children =
                        {
                            idBox,
                            nameBox,
                            gradeStringBox
                        }
            },
            CloseButtonText = "Cancel",
            PrimaryButtonText = "Add",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = (sender as FrameworkElement)?.XamlRoot
        }.ShowAsync())
        {
            case ContentDialogResult.None:
            return;
            case ContentDialogResult.Primary:
            // Use the Text property of the TextBoxes to get the input values
            if (idBox.Text is not { } id)
            {
                await SimpleDialog("ID cannot be empty.", "Warning", sender);
                return;
            }

            if (nameBox.Text is not { } name)
            {
                await SimpleDialog("Name cannot be empty.", "Warning", sender);
                return;
            }

            if (gradeStringBox.Text is not { } gradeString)
            {
                await SimpleDialog("Grade cannot be empty.", "Warning", this);
                return;
            }

            if (await ViewModel.AddDataAsync(id, name, gradeString) is not true)
            {
                await SimpleDialog("Student with ID \"" + id + "\" already exists.", "Error", this);
                return;
            }

            // successfully added student
            IsModified = true;
            return;
            case ContentDialogResult.Secondary:
            default:
            await SimpleDialog("Oops! Something went wrong. You shouldn't be here.", "Error", sender);
            return;
        }
    }

    private static async Task SimpleDialog(string message, string warningLevel, object? sender)
    {
        try
        {
            await new ContentDialog
            {
                Title = warningLevel,
                Content = message,
                CloseButtonText = "Ok",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = (sender as FrameworkElement)?.XamlRoot
            }.ShowAsync();
        } catch (NullReferenceException e)
        {
            Console.WriteLine(@"Caught an NullReferenceException: " + e.Message);
        } catch (COMException e)
        {
            Console.WriteLine(@"Caught an COMException: " + e.Message);
        } catch (Exception e)
        {
            Console.WriteLine(@"Caught an Exception: " + e.Message);
        }
    }

    private async void DeleteStudent(object sender, RoutedEventArgs e) => await DeleteStudentDialog(sender);

    private async Task DeleteStudentDialog(object sender)
    {
        // Create a new TextBox instance each time the dialog is shown
        var textBox = new TextBox();

        // Create a new ContentDialog instance each time the dialog is shown
        var dialog = new ContentDialog
        {
            Title = "Input Student ID:",
            Content = textBox,
            CloseButtonText = "Cancel",
            PrimaryButtonText = "Confirm",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = ((FrameworkElement)sender).XamlRoot
            //XamlRoot = XamlRoot // Set the XamlRoot to ensure it's displayed correctly
            // that is IMPORTANT otherwise exception would be thrown,
            // because a dialog could only be bind to exactly one window (XamlRoot denotes that window)
        };

        var result = await dialog.ShowAsync(); // Show the dialog

        if (result != ContentDialogResult.Primary)
        {
            return;
        }

        if (textBox.Text is null)
        {
            await SimpleDialog("Student ID cannot be empty.", "Warning", sender);
            return;
        }

        if (await ViewModel.DeleteDataAsync(textBox.Text) is not true)
        {
            // show error message: Student not found
            await SimpleDialog("Student with ID \"" + textBox.Text + "\" not found.", "Error", sender);
            return;
        }

        // successfully deleted student
        await SimpleDialog("Successfully deleted Student with ID \"" + textBox.Text + "\".", "Info", sender);
        IsModified = true;
    }

    private string RestoreData(string header)
        => header switch
        {
            "ID" => (_cellDataTemp as Student)?.Id ?? string.Empty,
            "Name" => (_cellDataTemp as Student)?.Name ?? string.Empty,
            "Grades" => (_cellDataTemp as Student)?.GradesString ?? string.Empty,
            _ => string.Empty
        };

    private void CellDataOriginal(object? sender, DataGridBeginningEditEventArgs e) =>
        _cellDataTemp = e.Row.DataContext as Student;

    // this event execute before the binding data is updated,
    // so we can get the original data and handle the exception at UI level.
    // `CellEditEnded` event is executed after the binding data is updated,
    // use converter can't handle the exception at UI level either.
    private void CellDataModified(object? sender, DataGridCellEditEndingEventArgs e)
    {
        try
        {
            var dataString = (e.EditingElement as TextBox)?.Text;
            var header = e.Column.Header?.ToString();
            switch (header)
            {
                case "Grades":
                _ = dataString
                    ?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => double.Parse(s.Trim()))
                    .ToList();
                IsModified = true; // if parse failed, this line won't be executed
                break;
                case "ID":
                _ = double.Parse(dataString?.Trim() ?? string.Empty);
                IsModified = true;
                break;
            }
        } catch (Exception exception)
        {
            // e.Cancel = true;

            // CANNOT use `await`:
            // THIS event handler cannot be async,(some can, see `SaveData` function above)
            // so `await` will throw an exception: `InteropServices.COMException`,
            // which means multiple dialog boxes are shown at the same time (why?? dunno)
            //
            // also here use a `_` to ignore the return value to suppress IDE and compiler warnings
            _ = SimpleDialog(exception.Message, "Error", sender);
            (e.EditingElement as TextBox)!.Text = RestoreData(e.Column.Header?.ToString() ?? string.Empty);
            // but I need to use that value to determine the result of the dialog
            // NO WAY! failed. same error as above
            // switch (dialog.ShowAsync().GetResults())
            // {
            //     case ContentDialogResult.Primary:
            //         // Restore the original data
            //         (e.EditingElement as TextBox)!.Text = RestoreData(e.Column.Header?.ToString() ?? string.Empty);
            //         e.Cancel = false;
            //         break;
            //     case ContentDialogResult.None:
            //     // goto default;
            //     case ContentDialogResult.Secondary:
            //     // goto default;
            //     default:
            //         e.Cancel = true;
            //         break;
            // }
        }
    }
}

// private async void CellDataChanged(object? sender, DataGridCellEditEndedEventArgs e)
// {
//     if(e.Row.DataContext is not Student student)
//     {
//         await SimpleContentDialog("Oops! Something went wrong. You shouldn't be here.", "Error", sender);
//         return;
//     }
//     // find that student in the list
//     var isDataChanged = ViewModel.IsDataChanged(student);
//     if (isDataChanged is null)
//     {
//         // parse error or something else
//         await SimpleContentDialog("The format of the data isn't correct.", "Error", sender);
//         return;
//     }
//
//     // fixme: temporarily do it: shouldn't use the whole list to update the UI
//     UpdateSort();
//     if(isDataChanged is not true)
//     {
//         // do not change `IsModified` here; maybe the data is modified before or not modified at all
//
//
//         await SimpleContentDialog("Data not changed.", "Info", sender);
//         return;
//     }
//     // successfully modified student; no need to prompt a dialog
//     IsModified = true;
// }