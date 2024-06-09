using Windows.ApplicationModel.Core;
using InfoManager.Activation;
using InfoManager.Contracts.Services;
using InfoManager.Core.Contracts.Services;
using InfoManager.Core.Services;
using InfoManager.Helpers;
using InfoManager.Models;
using InfoManager.Services;
using InfoManager.ViewModels;
using InfoManager.Views;

using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace InfoManager;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public static UIElement? AppTitlebar
    {
        get; set;
    }

    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers
            services.AddTransient<IActivationHandler, AppNotificationActivationHandler>();

            // Services
            services.AddSingleton<IAppNotificationService, AppNotificationService>();
            services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // Core Services
            services.AddSingleton<IFileService, FileService>();

            // Views and ViewModels
            services.AddTransient<LaunchViewModel>();
            services.AddTransient<LaunchPage>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<DataViewModel>();
            services.AddTransient<DataPage>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainPage>();
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();

            // Configuration
            services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
        }).
        Build();

        App.GetService<IAppNotificationService>().Initialize();

        // UnhandledException += AppUnhandledException;
        //CoreApplication.Exiting += OnExiting;
    }

    //private async void OnExiting(object sender, object e)
    //{
    //    if (MainWindow.Content is Frame { Content: Page { DataContext: DataViewModel viewModel } })
    //    {
    //        var testDialog = new ContentDialog
    //        {
    //            Title = "Unsaved changes",
    //            Content = "You have unsaved changes. Do you want to save them?",
    //            PrimaryButtonText = "Save",
    //            CloseButtonText = "Don't Save"
    //        };
    //        var result = await testDialog.ShowAsync();
    //        if (result == ContentDialogResult.Primary)
    //        {
    //            // await viewModel.SaveDataToFileAsync(true);

    //        }

    //    }
    //}
    private void AppUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // TODO: Log and handle exceptions as appropriate.
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);
        App.GetService<IAppNotificationService>().Show(string.Format("AppNotificationSamplePayload".GetLocalized(), AppContext.BaseDirectory));

        await App.GetService<IActivationService>().ActivateAsync(args);
        // BUG: not working


        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active

        // avoid naming conflicts, because there are also `Frame` and `Window` classes in the `ABI. ...` namespace


        //var rootFrame = new Microsoft.UI.Xaml.Controls.Frame();
        ////if (Window.Current is null)
        ////{
        ////    // Create a Frame to act as the navigation context and navigate to the first page
        ////    if (args.UWPLaunchActivatedEventArgs.PreviousExecutionState == ApplicationExecutionState.Terminated)
        ////    {
        ////        //TODO: Load state from previously suspended application
        ////    }
        ////    // Place the frame in the current Window
        ////    Window.Current = new Window();
        ////    Window.Current.Content = rootFrame;
        ////}

        //if (rootFrame.Content is null)
        //{

        //    // When the navigation stack isn't restored, navigate to the first page
        //    // and configure the new page by passing required information as a navigation parameter
        //    await Task.Delay(3000);
        //    rootFrame.Navigate(typeof(LaunchPage), args.Arguments);
        //    // sleep 3s
        //    await Task.Delay(3000);
        //    // Ensure the current window is active
        //    //Window.Current.Activate();
        //}
    }

    public void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
    }
}
