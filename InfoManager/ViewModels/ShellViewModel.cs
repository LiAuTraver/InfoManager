using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InfoManager.Contracts.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;

namespace InfoManager.ViewModels;

public partial class ShellViewModel : ObservableRecipient
{
    [ObservableProperty] private bool _isBackEnabled;

    public ShellViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;

        MenuFileExitCommand = new RelayCommand(OnMenuFileExit);
        MenuSettingsCommand = new RelayCommand(OnMenuSettings);
        MenuViewsDataCommand = new RelayCommand(OnMenuViewsData);
        MenuViewsMainCommand = new RelayCommand(OnMenuViewsMain);
    }

    public ICommand MenuFileExitCommand
    {
        get;
    }

    public ICommand MenuSettingsCommand
    {
        get;
    }

    public ICommand MenuViewsDataCommand
    {
        get;
    }

    public ICommand MenuViewsMainCommand
    {
        get;
    }

    public INavigationService NavigationService
    {
        get;
    }

    private void OnNavigated(object sender, NavigationEventArgs e) => IsBackEnabled = NavigationService.CanGoBack;

    private void OnMenuFileExit() => Application.Current.Exit();

    private void OnMenuSettings() => NavigationService.NavigateTo(typeof(SettingsViewModel).FullName!);

    private void OnMenuViewsData() => NavigationService.NavigateTo(typeof(DataViewModel).FullName!);

    private void OnMenuViewsMain() => NavigationService.NavigateTo(typeof(MainViewModel).FullName!);
}