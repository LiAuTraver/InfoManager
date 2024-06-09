using InfoManager.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Pickers;

namespace InfoManager.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }

    private async void OpenInfoFile(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Clear previous returned file name, if it exists, between iterations of this scenario
        var openPicker = new FileOpenPicker
        {
            ViewMode = PickerViewMode.Thumbnail,
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            // currently do not support csv
            // we forbid txt file too
            FileTypeFilter = { ".json" }
        };
        // Retrieve the window handle (HWND) of the current WinUI 3 window.
        // Initialize the file picker with the window handle.
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker,
            WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow));

        // Open the picker for the user to pick a file
        var infoFile = await openPicker.PickSingleFileAsync();
        // convert StorageFile to ScreenReader
        var filePath = infoFile.Path ?? @"-1";
        Frame.Navigate(typeof(DataPage), filePath);
    }
}