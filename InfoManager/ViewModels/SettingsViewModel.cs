using System.Reflection;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InfoManager.Contracts.Services;
using InfoManager.Helpers;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel;

namespace InfoManager.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private const string Uri = "https://github.com/liautraver";
    private const string AppSourceCodeUri = "https://github.com/liautraver/InfoManager";
    private readonly IThemeSelectorService _themeSelectorService;
    public readonly Uri MyUri = new(Uri);
    public readonly Uri MyAppSourceCodeUri = new(AppSourceCodeUri);
    [ObservableProperty] private ElementTheme _elementTheme;

    [ObservableProperty] private string _versionDescription;

    public SettingsViewModel(IThemeSelectorService themeSelectorService)
    {
        _themeSelectorService = themeSelectorService;
        _elementTheme = _themeSelectorService.Theme;
        _versionDescription = GetVersionDescription();

        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async param =>
            {
                if (ElementTheme == param)
                {
                    return;
                }

                ElementTheme = param;
                await _themeSelectorService.SetThemeAsync(param);
            });
    }

    public ICommand SwitchThemeCommand
    {
        get;
    }

    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build,
                packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return
            $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
}