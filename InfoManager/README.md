# Structure *( tree command, may not be up-to-date )*

```pwsh
Folder PATH listing for volume Dev Drive
Volume serial number is 8678-97F0
InfoManager
│   app.manifest
│   App.xaml
│   App.xaml.cs
│   appsettings.json
│   InfoManager.csproj
│   InfoManager.sln
│   InfoManager.sln.DotSettings
│   InfoManager.sln.DotSettings.user
│   MainWindow.xaml
│   MainWindow.xaml.cs
│   Package.appinstaller
│   Package.appxmanifest
│   README.md
│   README_from_template_studio.md
│   structure.txt
│   TemplateStudio.xml
│   Usings.cs
│   
├───Activation
│       ActivationHandler.cs
│       AppNotificationActivationHandler.cs
│       DefaultActivationHandler.cs
│       IActivationHandler.cs
│       
├───Assets
│       LockScreenLogo.scale-200.png
│       SplashScreen.scale-200.png
│       Square150x150Logo.scale-200.png
│       Square44x44Logo.scale-200.png
│       Square44x44Logo.targetsize-24_altform-unplated.png
│       StoreLogo.png
│       Wide310x150Logo.scale-200.png
│       WindowIcon.ico
│       
├───Contracts
│   ├───Services
│   │       IActivationService.cs
│   │       IAppNotificationService.cs
│   │       IFileService.cs
│   │       ILocalSettingsService.cs
│   │       INavigationService.cs
│   │       IPageService.cs
│   │       IThemeSelectorService.cs
│   │       
│   └───ViewModels
│           INavigationAware.cs
│           
├───Data
│       data.backup.json
│       data.json
│       
├───Helpers
│       BooleanNegationConvertor.cs
│       BoolToSortOrderConverter.cs
│       EnumToBooleanConverter.cs
│       FrameExtensions.cs
│       Json.cs
│       ResourceExtensions.cs
│       RuntimeHelper.cs
│       SafeStringParseConverter.cs
│       SettingsStorageExtensions.cs
│       TitleBarHelper.cs
│       
├───Models
│       LocalSettingsOptions.cs
│       Student.cs
│       
├───Properties
│   │   launchsettings.json
│   │   Resources.Designer.cs
│   │   Resources.resx
│   │   
│   └───PublishProfiles
│           win10-arm64.pubxml
│           win10-x64.pubxml
│           win10-x86.pubxml
│           
├───Services
│       ActivationService.cs
│       AppNotificationService.cs
│       FileService.cs
│       LocalSettingsService.cs
│       NavigationService.cs
│       PageService.cs
│       SortService.cs
│       StudentService.cs
│       StudentSortService.cs
│       ThemeSelectorService.cs
│       
├───Strings
│   └───en-us
│           Resources.resw
│           
├───Styles
│       FontSizes.xaml
│       TextBlock.xaml
│       Thickness.xaml
│       
├───ViewModels
│       DataViewModel.cs
│       MainViewModel.cs
│       SettingsViewModel.cs
│       ShellViewModel.cs
│       
└───Views
        DataPage.xaml
        DataPage.xaml.cs
        MainPage.xaml
        MainPage.xaml.cs
        SettingsPage.xaml
        SettingsPage.xaml.cs
        ShellPage.xaml
        ShellPage.xaml.cs
        
```