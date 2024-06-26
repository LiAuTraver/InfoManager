﻿using System.Collections.Specialized;
using System.Web;
using InfoManager.Contracts.Services;
using Microsoft.Windows.AppNotifications;

namespace InfoManager.Services;

public class AppNotificationService(INavigationService navigationService) : IAppNotificationService
{
    private readonly INavigationService _navigationService = navigationService;

    public void Initialize()
    {
        AppNotificationManager.Default.NotificationInvoked += OnNotificationInvoked;

        AppNotificationManager.Default.Register();
    }

    public bool Show(string payload)
    {
        var appNotification = new AppNotification(payload);

        AppNotificationManager.Default.Show(appNotification);

        return appNotification.Id != 0;
    }

    public NameValueCollection ParseArguments(string arguments) => HttpUtility.ParseQueryString(arguments);

    public void Unregister() => AppNotificationManager.Default.Unregister();

    ~AppNotificationService()
    {
        Unregister();
    }

    public void OnNotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args) =>
        // TODO: Handle notification invocations when your app is already running.
        //// // Navigate to a specific page based on the notification arguments.
        //// if (ParseArguments(args.Argument)["action"] == "Settings")
        //// {
        ////    App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        ////    {
        ////        _navigationService.NavigateTo(typeof(SettingsViewModel).FullName!);
        ////    });
        //// }
        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            App.MainWindow.ShowMessageDialogAsync(
                "Congratulations! You have successfully installed the app.",
                "Welcome to Lyc's Student InfoManager App"
                );

            App.MainWindow.BringToFront();
        });
}