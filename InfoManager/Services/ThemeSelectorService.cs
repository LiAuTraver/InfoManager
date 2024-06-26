﻿using InfoManager.Contracts.Services;
using InfoManager.Helpers;
using Microsoft.UI.Xaml;

namespace InfoManager.Services;

public class ThemeSelectorService(ILocalSettingsService localSettingsService) : IThemeSelectorService
{
    private const string SettingsKey = "AppBackgroundRequestedTheme";

    public ElementTheme Theme
    {
        get;
        set;
    } = ElementTheme.Default;

    public async Task InitializeAsync()
    {
        Theme = await LoadThemeFromSettingsAsync();
        await Task.CompletedTask;
    }

    public async Task SetThemeAsync(ElementTheme theme)
    {
        Theme = theme;

        await SetRequestedThemeAsync();
        await SaveThemeInSettingsAsync(Theme);
    }

    public async Task SetRequestedThemeAsync()
    {
        if (App.MainWindow.Content is FrameworkElement rootElement)
        {
            rootElement.RequestedTheme = Theme;

            TitleBarHelper.UpdateTitleBar(Theme);
        }

        await Task.CompletedTask;
    }

    private async Task<ElementTheme> LoadThemeFromSettingsAsync()
    {
        var themeName = await localSettingsService.ReadSettingAsync<string>(SettingsKey);

        return Enum.TryParse(themeName, out ElementTheme cacheTheme) ? cacheTheme : ElementTheme.Default;
    }

    private async Task SaveThemeInSettingsAsync(ElementTheme theme) =>
        await localSettingsService.SaveSettingAsync(SettingsKey, theme.ToString());
}