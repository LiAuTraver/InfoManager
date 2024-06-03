using System.ComponentModel;
using InfoManager.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace InfoManager.Views;

public sealed partial class DataPage : /* Default Page Interface */
    Page, /* for that binding of IsAscending and corresponding button*/INotifyPropertyChanged
{
    public bool IsBusy
    {
        get;
        set;
    }

    public bool IsEditing
    {
        get;
        set;
    }

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
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public DataViewModel ViewModel
    {
        get;
    }

    public DataPage()
    {
        ViewModel = App.GetService<DataViewModel>();
        IsAscending = true;
        InitializeComponent();
    }

    public async void SaveData(object sender, RoutedEventArgs e)
    {
        // await ViewModel.SaveDataToFileAsync(_filePath);
        await Task.CompletedTask;
    }

    private void SwitchSortMode(object sender, RoutedEventArgs e)
    {
        switch ((sender as RadioButton)?.Content.ToString())
        {
            case "Sort by ID":
                ViewModel.SortByIdCommand.Execute(null);
                break;
            case "Sort by Average":
                ViewModel.SortByAverageCommand.Execute(null);
                break;
            case "Sort by Name":
                ViewModel.SortByNameCommand.Execute(null);
                break;
            default:
                // Do nothing, emit a warning
                Console.WriteLine(@"Oops! Something went wrong. You shouldn't be here.");
                break;
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

}