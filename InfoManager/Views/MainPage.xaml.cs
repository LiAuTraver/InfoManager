using InfoManager.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace InfoManager.Views;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }

    public MainViewModel ViewModel
    {
        get;
    }

    private async void OpenInfoFile(object sender, RoutedEventArgs e)
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
        InitializeWithWindow.Initialize(openPicker,
            WindowNative.GetWindowHandle(App.MainWindow));

        // Open the picker for the user to pick a file
        var infoFile = await openPicker.PickSingleFileAsync();
        // convert StorageFile to ScreenReader
        var filePath = infoFile != null ? infoFile.Path : @"-1";
        Frame.Navigate(typeof(DataPage), filePath);
    }
}