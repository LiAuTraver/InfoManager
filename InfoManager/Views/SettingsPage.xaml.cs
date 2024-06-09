using InfoManager.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace InfoManager.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        ViewModel = App.GetService<SettingsViewModel>();
        InitializeComponent();
    }

    public SettingsViewModel ViewModel
    {
        get;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e) => base.OnNavigatedTo(e);
}