using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SystemReview.ViewModels;
using System.ComponentModel;

namespace SystemReview.Views;

public sealed partial class SystemSpecsPage : Page
{
    private readonly SystemSpecsViewModel _vm = new();
    private bool _loaded;
    private readonly PropertyChangedEventHandler _propertyChangedHandler;

    public SystemSpecsPage()
    {
        this.InitializeComponent();
        CpuList.ItemsSource = _vm.Cpus;
        RamList.ItemsSource = _vm.RamDetails;
        GpuList.ItemsSource = _vm.Gpus;
        DriveList.ItemsSource = _vm.Drives;
        OsList.ItemsSource = _vm.OsDetails;
        MbList.ItemsSource = _vm.MotherboardDetails;
        DisplayList.ItemsSource = _vm.DisplayDetails;
        BatteryList.ItemsSource = _vm.BatteryDetails;
        SoftwareList.ItemsSource = _vm.InstalledSoftware;

        _propertyChangedHandler = (_, e) =>
        {
            if (e.PropertyName == nameof(_vm.StatusMessage))
                StatusText.Text = _vm.StatusMessage;
            if (e.PropertyName == nameof(_vm.IsLoading))
                LoadingBar.Visibility = _vm.IsLoading ? Visibility.Visible : Visibility.Collapsed;
            if (e.PropertyName == nameof(_vm.RamUsagePercent))
            {
                RamBarGrid.Visibility = Visibility.Visible;
                RamUsageBar.Value = _vm.RamUsagePercent;
                RamUsageText.Text = $"{_vm.RamUsagePercent}% used";
            }
        };
        _vm.PropertyChanged += _propertyChangedHandler;

        this.Loaded += async (_, _) =>
        {
            if (!_loaded && SettingsPage.AutoLoadEnabled)
            {
                _loaded = true;
                await _vm.LoadAllAsync();
            }
        };

        this.Unloaded += (_, _) =>
        {
            _vm.PropertyChanged -= _propertyChangedHandler;
        };
    }

    private async void RefreshBtn_Click(object sender, RoutedEventArgs e) => await _vm.LoadAllAsync();
    private void ExportJsonBtn_Click(object sender, RoutedEventArgs e) => _vm.ExportJsonCommand.Execute(null);
    private void ExportTextBtn_Click(object sender, RoutedEventArgs e) => _vm.ExportTextCommand.Execute(null);
}