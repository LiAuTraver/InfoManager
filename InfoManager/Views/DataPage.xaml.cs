using System.ComponentModel;
using InfoManager.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace InfoManager.Views;

// func part
public sealed partial class DataPage : INotifyPropertyChanged
{
    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

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
            XamlRoot = XamlRoot
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
            Console.WriteLine(@"Oops! Something went wrong. You shouldn't be here.");
            return;
        }
    }

    private void SwitchSortMode(object sender, RoutedEventArgs e)
    {
        if ((sender as RadioButton)?.Content is string content)
        {
            _selectedSortOption = content;
        }

        UpdateSort();
    }

    private void UpdateSort()
    {
        switch (_selectedSortOption)
        {
            case "Sort by ID":
            ViewModel.SortByIdCommand.Execute(IsAscending);
            return;
            case "Sort by Average":
            ViewModel.SortByAverageCommand.Execute(IsAscending);
            return;
            case "Sort by Name":
            ViewModel.SortByNameCommand.Execute(IsAscending);
            return;
            default:
            // Do nothing, emit a warning
            Console.WriteLine(@"Oops! Something went wrong. You shouldn't be here.");
            return;
        }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is not string newFilePath)
        {
            return;
        }

        ViewModel.OnNavigatedTo(newFilePath);
    }

    private void ToggleAscending(object sender, RoutedEventArgs e)
    {
        IsAscending = !IsAscending;
        ToggleButtonContent = IsAscending ? "Ascending" : "Descending";
        UpdateSort();
    }

    private void ToggleEditing(object sender, RoutedEventArgs e)
    {
        IsEditing = !IsEditing;
    }

    private async void AddStudent(object sender, RoutedEventArgs e)
    {
        await AddStudentDialog(sender);
    }

    private async Task AddStudentDialog(object sender)
    {
        var idBox = new TextBox { Header = "ID" };
        var nameBox = new TextBox { Header = "Name" };
        var gradeStringBox = new TextBox { Header = "Grade" };

        var stackPanel = new StackPanel
        {
            Children =
            {
                idBox,
                nameBox,
                gradeStringBox
            }
        };

        var dialog = new ContentDialog
        {
            Title = "Add",
            Content = stackPanel,
            CloseButtonText = "Cancel",
            PrimaryButtonText = "Add",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = (sender as FrameworkElement)?.XamlRoot
        };

        var result = await dialog.ShowAsync();
        switch (result)
        {
            case ContentDialogResult.None:
            return;
            case ContentDialogResult.Primary:
            // Use the Text property of the TextBoxes to get the input values
            if (idBox.Text is not { } id)
            {
                await SimpleContentDialog("ID cannot be empty.", "Warning", sender);
                return;
            }

            if (nameBox.Text is not { } name)
            {
                await SimpleContentDialog("Name cannot be empty.", "Warning", sender);
                return;
            }

            if (gradeStringBox.Text is not { } gradeString)
            {
                await SimpleContentDialog("Grade cannot be empty.", "Warning", sender);
                return;
            }

            if (await ViewModel.AddDataAsync(id, name, gradeString) is not true)
            {
                await SimpleContentDialog("Student with ID \"" + id + "\" already exists.", "Error", sender);
                return;
            }

            // successfully added student
            IsModified = true;
            return;
            case ContentDialogResult.Secondary:
            Console.WriteLine(@"Oops! Something went wrong. You shouldn't be here.");
            return;
            default:
            Console.WriteLine(@"Oops! Something went wrong. You shouldn't be here.");
            return;
        }
    }

    private static async Task SimpleContentDialog(string message, string warningLevel, object sender)
    {
        var dialog = new ContentDialog
        {
            Title = warningLevel,
            Content = message,
            CloseButtonText = "Ok",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = ((FrameworkElement)sender).XamlRoot
        };
        await dialog.ShowAsync();
    }

    private async void DeleteStudent(object sender, RoutedEventArgs e)
    {
        await DeleteStudentDialog(sender);
    }

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
            await SimpleContentDialog("Student ID cannot be empty.", "Warning", sender);
            return;
        }

        if (await ViewModel.DeleteDataAsync(textBox.Text) is not true)
        {
            // show error message: Student not found
            await SimpleContentDialog("Student with ID \"" + textBox.Text + "\" not found.", "Error", sender);
            return;
        }

        // successfully deleted student
        await SimpleContentDialog("Successfully deleted Student with ID \"" + textBox.Text + "\".", "Info", sender);
        IsModified = true;
    }

    protected async override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (IsModified is not true)
        {
            base.OnNavigatedFrom(e);
        }

        var warningDialog = new ContentDialog
        {
            Title = "Warning",
            Content = "You have unsaved changes. Save or discard before leaving.",
            CloseButtonText = "Ok",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = this.XamlRoot
        };
        await warningDialog.ShowAsync();
    }
}

// member variables and ctor part
public sealed partial class DataPage : INotifyPropertyChanged
{
    public DataPage()
    {
        ViewModel = App.GetService<DataViewModel>();
        IsAscending = true;
        IsEditing = false;
        IsModified = false;
        InitializeComponent();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool _isEditing;

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

    private string _selectedSortOption = "Sort by ID";

    private bool _isAscending = true;

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

    private bool _isModified;

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

    private string _toggleButtonContent = "Ascending";

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
}