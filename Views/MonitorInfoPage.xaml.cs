using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using SystemReview.Services;

namespace SystemReview.Views;

public sealed partial class MonitorInfoPage : Page
{
    private bool _loaded;

    public MonitorInfoPage()
    {
        this.InitializeComponent();
        this.Loaded += async (_, _) =>
        {
            if (!_loaded && SettingsPage.AutoLoadEnabled)
            {
                _loaded = true;
                RefreshBtn_Click(this, new RoutedEventArgs());
            }
        };
    }

    private async void RefreshBtn_Click(object sender, RoutedEventArgs e)
    {
        LoadingBar.Visibility = Visibility.Visible;
        StatusText.Text = "Reading monitor data...";
        MonitorPanel.Children.Clear();

        try
        {
            var monitors = await WmiService.GetMonitorDetailsAsync();

            int monIdx = 1;
            foreach (var mon in monitors)
            {
                var card = new Border
                {
                    Background = (Brush)Application.Current.Resources["CardBackgroundFillColorDefaultBrush"],
                    BorderBrush = (Brush)Application.Current.Resources["CardStrokeColorDefaultBrush"],
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(20)
                };

                var stack = new StackPanel { Spacing = 12 };

                var header = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };
                header.Children.Add(new FontIcon { Glyph = "\xE7F4", FontSize = 22 });
                header.Children.Add(new TextBlock
                {
                    Text = $"Monitor {monIdx}: {mon.Name}",
                    Style = (Style)Application.Current.Resources["SubtitleTextBlockStyle"]
                });

                var statusBorder = new Border
                {
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(12, 4, 12, 4),
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = mon.DriverStatus == "Active"
                        ? new SolidColorBrush(Windows.UI.Color.FromArgb(255, 16, 124, 16))
                        : new SolidColorBrush(Windows.UI.Color.FromArgb(255, 128, 128, 128))
                };
                statusBorder.Child = new TextBlock
                {
                    Text = mon.DriverStatus == "Active" ? "Active" : "Inactive",
                    Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255)),
                    FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                    FontSize = 13
                };
                header.Children.Add(statusBorder);
                stack.Children.Add(header);

                var infoRows = new (string Label, string Value)[]
                {
                    ("Manufacturer", $"{mon.Manufacturer} ({mon.ManufacturerCode})"),
                    ("Product Code", mon.ProductCode),
                    ("Serial Number", mon.SerialNumber),
                    ("Connection", mon.ConnectionType),
                    ("", ""),
                    ("EDID Version", mon.EdidVersion),
                    ("Display Type", mon.DisplayType),
                    ("Color Depth", mon.ColorBitDepth),
                    ("Gamma", mon.GammaValue),
                    ("DPMS Support", mon.DpmsSupport),
                    ("", ""),
                    ("Native Resolution", mon.MaxResolution),
                    ("Screen Size", mon.ScreenSize),
                    ("Diagonal", mon.DiagonalInches),
                    ("", ""),
                    ("Manufacture Date", mon.ManufactureDate),
                    ("Year", mon.YearOfManufacture),
                    ("Estimated Usage", mon.EstimatedUsage),
                };

                foreach (var (label, value) in infoRows)
                {
                    if (string.IsNullOrEmpty(label))
                    {
                        stack.Children.Add(new Border
                        {
                            Height = 1,
                            Background = (Brush)Application.Current.Resources["CardStrokeColorDefaultBrush"],
                            Margin = new Thickness(0, 4, 0, 4)
                        });
                        continue;
                    }

                    var row = new Grid { ColumnSpacing = 16 };
                    row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(160) });
                    row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    var lbl = new TextBlock { Text = label };
                    lbl.Foreground = (Brush)Application.Current.Resources["SystemControlForegroundBaseMediumBrush"];
                    Grid.SetColumn(lbl, 0);

                    var val = new TextBlock { Text = value, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold, TextWrapping = TextWrapping.Wrap };
                    Grid.SetColumn(val, 1);

                    row.Children.Add(lbl);
                    row.Children.Add(val);
                    stack.Children.Add(row);
                }

                if (mon.SupportedResolutions.Count > 0)
                {
                    var expander = new Expander
                    {
                        Header = $"Supported Resolutions ({mon.SupportedResolutions.Count})",
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Stretch
                    };

                    var resList = new StackPanel { Spacing = 4 };
                    foreach (var res in mon.SupportedResolutions)
                        resList.Children.Add(new TextBlock { Text = $"  - {res}", FontSize = 13 });
                    expander.Content = resList;
                    stack.Children.Add(expander);
                }

                card.Child = stack;
                MonitorPanel.Children.Add(card);
                monIdx++;
            }

            StatusText.Text = $"Loaded {monitors.Count} monitor(s) at {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Error: {ex.Message}";
        }
        finally
        {
            LoadingBar.Visibility = Visibility.Collapsed;
        }
    }
}