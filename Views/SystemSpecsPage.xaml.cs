using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SystemReview.ViewModels;

namespace SystemReview.Views;

public sealed partial class SystemSpecsPage : Page
{
    private readonly SystemSpecsViewModel _vm = new();

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
        _vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(_vm.StatusMessage))
                StatusText.Text = _vm.StatusMessage;
            if (e.PropertyName == nameof(_vm.IsLoading))
                LoadingBar.Visibility = _vm.IsLoading ? Visibility.Visible : Visibility.Collapsed;
        };
    }

    private async void RefreshBtn_Click(object sender, RoutedEventArgs e) => await _vm.LoadAllAsync();
    private void ExportJsonBtn_Click(object sender, RoutedEventArgs e) => _vm.ExportJsonCommand.Execute(null);
    private void ExportTextBtn_Click(object sender, RoutedEventArgs e) => _vm.ExportTextCommand.Execute(null);
}