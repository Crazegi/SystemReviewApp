using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace SystemReview.Views;

public sealed partial class SettingsPage : Page
{
    public static bool AutoLoadEnabled { get; set; } = true;

    public SettingsPage()
    {
        this.InitializeComponent();
        AutoLoadToggle.IsOn = AutoLoadEnabled;
        ThemeRadio.SelectedIndex = 0; // Default = system
    }

    private void ThemeRadio_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ThemeRadio.SelectedItem is RadioButton rb && rb.Tag is string tag)
        {
            var theme = tag switch
            {
                "Light" => ElementTheme.Light,
                "Dark" => ElementTheme.Dark,
                _ => ElementTheme.Default
            };
            App.MainWindowInstance?.SetTheme(theme);
        }
    }

    private void AutoLoadToggle_Toggled(object sender, RoutedEventArgs e)
    {
        AutoLoadEnabled = AutoLoadToggle.IsOn;
    }
}