using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using SystemReview.Models;
using SystemReview.Services;

namespace SystemReview.Views;

public sealed partial class DiskHealthPage : Page
{
    public DiskHealthPage()
    {
        this.InitializeComponent();
    }

    private async void RefreshBtn_Click(object sender, RoutedEventArgs e)
    {
        LoadingBar.Visibility = Visibility.Visible;
        StatusText.Text = "Reading S.M.A.R.T. data...";
        DiskPanel.Children.Clear();

        try
        {
            var disks = await WmiService.GetDiskHealthAsync();

            foreach (var disk in disks)
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

                // Header
                var header = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };
                header.Children.Add(new FontIcon { Glyph = "\xEDA2", FontSize = 22 });
                header.Children.Add(new TextBlock
                {
                    Text = $"{disk.Model}",
                    Style = (Style)Application.Current.Resources["SubtitleTextBlockStyle"]
                });

                // Health badge
                var healthBorder = new Border
                {
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(12, 4, 12, 4),
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = disk.HealthStatus.Contains("✅")
                        ? new SolidColorBrush(Windows.UI.Color.FromArgb(255, 16, 124, 16))
                        : disk.HealthStatus.Contains("⚠️")
                            ? new SolidColorBrush(Windows.UI.Color.FromArgb(255, 200, 150, 0))
                            : new SolidColorBrush(Windows.UI.Color.FromArgb(255, 196, 43, 28))
                };
                healthBorder.Child = new TextBlock
                {
                    Text = disk.HealthStatus,
                    Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255)),
                    FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                    FontSize = 13
                };
                header.Children.Add(healthBorder);
                stack.Children.Add(header);

                // Info grid
                var infoRows = new (string Label, string Value)[]
                {
                    ("Serial Number", disk.SerialNumber),
                    ("Firmware", disk.FirmwareRevision),
                    ("Interface", disk.InterfaceType),
                    ("Media Type", disk.MediaType),
                    ("Capacity", disk.SizeFormatted),
                    ("Status", disk.Status),
                    ("", ""),
                    ("⏱️ Power-On Time", disk.PowerOnHours),
                    ("📅 Power-On (Days)", disk.PowerOnDays),
                    ("🔄 Power Cycles", disk.PowerCycleCount),
                    ("🌡️ Temperature", disk.Temperature),
                    ("", ""),
                    ("⚠️ Reallocated Sectors", disk.ReallocatedSectors),
                    ("⏳ Pending Sectors", disk.PendingSectors),
                    ("❌ Uncorrectable", disk.UncorrectableSectors),
                    ("📖 Read Error Rate", disk.ReadErrorRate),
                    ("🔁 Spin Retry Count", disk.SpinRetryCount),
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
                    row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(180) });
                    row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    var lbl = new TextBlock { Text = label };
                    lbl.Foreground = (Brush)Application.Current.Resources["SystemControlForegroundBaseMediumBrush"];
                    Grid.SetColumn(lbl, 0);

                    var val = new TextBlock { Text = value, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold, TextWrapping = TextWrapping.Wrap };

                    // Color critical values
                    if (label.Contains("Reallocated") || label.Contains("Pending") || label.Contains("Uncorrectable"))
                    {
                        if (value != "0" && value != "N/A")
                            val.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 196, 43, 28));
                    }

                    Grid.SetColumn(val, 1);
                    row.Children.Add(lbl);
                    row.Children.Add(val);
                    stack.Children.Add(row);
                }

                // S.M.A.R.T. attributes expander
                if (disk.AllAttributes.Count > 0)
                {
                    var expander = new Expander
                    {
                        Header = $"📊 All S.M.A.R.T. Attributes ({disk.AllAttributes.Count})",
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Stretch
                    };

                    var attrStack = new StackPanel { Spacing = 2 };

                    // Header row
                    var hdrRow = new Grid { ColumnSpacing = 8, Padding = new Thickness(0, 4, 0, 8) };
                    hdrRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(35) });
                    hdrRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(220) });
                    hdrRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
                    hdrRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
                    hdrRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
                    hdrRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
                    hdrRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });

                    var headers = new[] { "ID", "Attribute", "Curr", "Worst", "Thresh", "Raw Value", "Status" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var t = new TextBlock { Text = headers[i], FontWeight = Microsoft.UI.Text.FontWeights.Bold, FontSize = 12 };
                        Grid.SetColumn(t, i);
                        hdrRow.Children.Add(t);
                    }
                    attrStack.Children.Add(hdrRow);

                    foreach (var attr in disk.AllAttributes.OrderBy(a => a.Id))
                    {
                        var row = new Grid { ColumnSpacing = 8, Padding = new Thickness(0, 2, 0, 2) };
                        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(35) });
                        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(220) });
                        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
                        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
                        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
                        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
                        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });

                        var vals = new[] { $"{attr.Id}", attr.Name, $"{attr.Current}", $"{attr.Worst}", $"{attr.Threshold}", $"{attr.RawValue}", attr.Status };
                        for (int i = 0; i < vals.Length; i++)
                        {
                            var t = new TextBlock { Text = vals[i], FontSize = 12 };
                            if (i == 6)
                                t.Foreground = attr.Status == "OK"
                                    ? new SolidColorBrush(Windows.UI.Color.FromArgb(255, 16, 124, 16))
                                    : new SolidColorBrush(Windows.UI.Color.FromArgb(255, 196, 43, 28));
                            Grid.SetColumn(t, i);
                            row.Children.Add(t);
                        }
                        attrStack.Children.Add(row);
                    }

                    expander.Content = attrStack;
                    stack.Children.Add(expander);
                }

                card.Child = stack;
                DiskPanel.Children.Add(card);
            }

            StatusText.Text = $"✅ Loaded {disks.Count} disk(s) at {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"❌ Error: {ex.Message}";
        }
        finally
        {
            LoadingBar.Visibility = Visibility.Collapsed;
        }
    }
}