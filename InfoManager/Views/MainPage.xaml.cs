using InfoManager.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Pickers;

namespace InfoManager.Views;

public sealed partial class MainPage : Page
{
    public string Result;
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
        Result = "Pick a File";
    }

    private async void OpenInfoFile(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        
        // Clear previous returned file name, if it exists, between iterations of this scenario
        Result = "";

        // Create a file picker
        var openPicker = new FileOpenPicker();

        // See the sample code below for how to make the window accessible from the App class.
        var window = App.MainWindow;

        // Retrieve the window handle (HWND) of the current WinUI 3 window.
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

        // Initialize the file picker with the window handle (HWND).
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

        // Set options for your file picker
        openPicker.ViewMode = PickerViewMode.Thumbnail;
        openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
        openPicker.FileTypeFilter.Add(".txt");
        openPicker.FileTypeFilter.Add(".csv");

        // Open the picker for the user to pick a file
        var infoFile = await openPicker.PickSingleFileAsync();
        if (infoFile == null) {
            Result = "Operation cancelled.";
        } else {
            // convert StorageFile to ScreenReader
            var filePath = infoFile.Path;
            Frame.Navigate(typeof(DataPage),filePath);
            Result="File" + infoFile.ToString() + " has been opened.";
        }
    }
}