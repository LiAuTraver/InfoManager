# 学生信息管理系统

采用了[Fluent UI2](https://fluent2.microsoft.design/)的设计，使用的是[Win UI3](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/)系列以及MMVM设计模式;

第一次采用UI设计，花费了很多时间，也走了很多弯路，才做出来最基本的功能。

源码请`git clone --recursive git@github.com:LiAuTraver/InfoManager.git`!

## 源码安排

```powershell
root
│   app.manifest 	# Packaged App的Config
│   App.xaml		# App 主程序的UI部分
│   App.xaml.cs		# App 主程序背后的C#源码
│   appsettings.json	
│   InfoManager.csproj		# C#项目的MS-Build文件
│   InfoManager.sln			# Visual Studio 文件 
│   MainWindow.xaml
│   MainWindow.xaml.cs
│   Package.appinstaller	# Package 文件
│   Package.appxmanifest
│   README.md
│   Usings.cs				# C# Golbal Usings 文件
│   
├───Activation
│       ActivationHandler.cs
│       AppNotificationActivationHandler.cs
│       DefaultActivationHandler.cs
│       IActivationHandler.cs
│       
├───Assets		# 图标相关，暂时没时间弄，后续会更新，现在还是默认
│       LockScreenLogo.scale-200.png
│       SplashScreen.scale-200.png
│       Square150x150Logo.scale-200.png
│       Square44x44Logo.scale-200.png
│       Square44x44Logo.targetsize-24_altform-unplated.png
│       StoreLogo.png
│       Wide310x150Logo.scale-200.png
│       WindowIcon.ico
│       
├───Contracts			# 这些Interface是Win UI3项目自带的，非我所写
│   ├───Services		# 但是我需要继承这些Interface做事
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
│       data.backup.json		# 学生数据（备份）
│       data.json				# 学生数据
│       
├───Helpers
│       BooleanNegationConvertor.cs		# UI Binding所需的转换器，
│       BoolToSortOrderConverter.cs		# 会用在XAML的Binding当中
│       EnumToBooleanConverter.cs		# 
│       FrameExtensions.cs				# Frame出了点问题，有空再修
│       Json.cs							# 学生数据的Serialization
│       ResourceExtensions.cs			# 和Deserialization所需
│       RuntimeHelper.cs
│       SafeStringParseConverter.cs
│       SettingsStorageExtensions.cs
│       TitleBarHelper.cs				# （Win UI3自带）
│       
├───Models
│       LocalSettingsOptions.cs # 设置相关Config
│       Student.cs				# 单个学生
│       
├───Properties		# Package相关文件
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
│       FileService.cs	# 文件操作相关
│       LocalSettingsService.cs
│       NavigationService.cs
│       PageService.cs
│       SortService.cs			# Generic 排序分类
│       StudentService.cs		# 学生们
│       StudentSortService.cs	# 分类偏特化
│       ThemeSelectorService.cs	# App主题颜色转换
│       
├───Strings			# Localization 相关文件，由于时间限制，我并没有做
│   └───en-us
│           Resources.resw
│           
├───Styles			# UI 字体相关配置
│       FontSizes.xaml
│       TextBlock.xaml
│       Thickness.xaml
│       
├───ViewModels
│       DataViewModel.cs		# 数据在UI上排布的设置
│       MainViewModel.cs		# 主页面
│       SettingsViewModel.cs	# 关于页面
│       ShellViewModel.cs		# 菜单栏
│       
└───Views		# 见后
        DataPage.xaml
        DataPage.xaml.cs
        MainPage.xaml
        MainPage.xaml.cs
        SettingsPage.xaml
        SettingsPage.xaml.cs
        ShellPage.xaml
        ShellPage.xaml.cs
```

### 获得源码

```powershell
# 所需依赖:	 
# Microsoft .dot Net 环境
# NuGet 包管理工具
# .NET运行环境

# 源码放在了GitHub上
git clone --recursive git@github.com:LiAuTraver/InfoManager.git
```

### 直接安装

下载Zip然后点击`msix`为后缀名的文件。需要安装.NET环境，可能还要安装证书（我自己的证书，因为要获得正式的证书需要在微软商城注册开发者，需要99$）



## 简单介绍

Microsoft Win UI3其实能用C++和C#开发，甚至二者结合。原先我打算用C++写，后来发现关于.NET的教学资源C#比较多，因此就转用C#开发。实现的功能见视频。

## 感想

开发过程中我发现最难的部分是UI与程序之间的连接。由于UI部分是以XAML形式编辑的，所以相应Class的所有变量需要与XAML绑定才能在UI中显示。用户在UI上面的任何操作必须在XAML里面创建事件，然后再相应的代码里面取实现。由于XAML的限制，需要使用Converter来转换信息。比方说在我的代码里`IsEditing`为`True`时，不能使用`Sort`。在UI中变量与UI按钮的Binding不能直接用逻辑符号 `!`表示，此时需要转换器。要是用户能直接在表格上编辑，必须将显示的数据和真实数据进行类似于“量子纠缠”的绑定。UI应用常常是Multi-Thread和Coroutine的，也感觉比较麻烦。

还有比较令人头疼的是文件的分类。Win UI的代码规范比较严格，也就是说，即使某种类别的处理比较少，我也得单独弄一个Class。这导致用户如果点击一个`Sort`选项，在代码里要实现4个Relay才能对真正的数据进行更改，因为在设计理念上，UI Class应该只管UI的事，对数据的处理要交给`Service`Class和`Converter`Class，对用户的事件处理应该在`View`Class里面完成，对数据的排布模式布局应该在`ViewModel` Class里面完成。所以我的项目中很多Class只有几个function，Interface甚至是空的（Win UI3自己生成的，必须继承它才能被内部的UI认识）。

另外Exception Handling也非常的棘手。如果数据格式出错，必须在UI上给予反馈；然而Student Class并非UI层级，所以如果抛出异常得隔好几层来catch，这样会增加代码风险。

# Bugs

异常抛出没有得到完美解决

非核心部分还待修缮