using InfoManager.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace InfoManager.Views;

public sealed partial class LaunchPage : Page
{
    public LaunchPage()
    {
        ViewModel = App.GetService<LaunchViewModel>();
        InitializeComponent();
    }

    public LaunchViewModel ViewModel
    {
        get;
    }
}