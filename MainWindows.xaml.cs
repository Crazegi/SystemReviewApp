using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SystemReview.Views;

namespace SystemReview;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        this.Title = "SystemReview";
        this.ExtendsContentIntoTitleBar = false;

        NavView.SelectedItem = NavView.MenuItems[0];
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationViewItem item && item.Tag is string tag)
        {
            ContentFrame.Navigate(tag switch
            {
                "specs" => typeof(SystemSpecsPage),
                "diskhealth" => typeof(DiskHealthPage),
                "monitor" => typeof(MonitorInfoPage),
                "network" => typeof(NetworkingPage),
                "diagnostics" => typeof(DiagnosticsPage),
                _ => typeof(SystemSpecsPage)
            });
        }
    }
}