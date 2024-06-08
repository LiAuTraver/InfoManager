using InfoManager.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace InfoManager.Views;

public sealed partial class LaunchPage : Page
{
    public LaunchViewModel ViewModel
    {
        get;
    }

    public LaunchPage()
    {
        ViewModel = App.GetService<LaunchViewModel>();
        InitializeComponent();
    }
}
