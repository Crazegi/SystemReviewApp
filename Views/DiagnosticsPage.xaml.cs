using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
using SystemReview.ViewModels;
using System.ComponentModel;

namespace SystemReview.Views;

public sealed partial class DiagnosticsPage : Page
{
    private readonly DiagnosticsViewModel _vm = new();
    private readonly PropertyChangedEventHandler _propertyChangedHandler;

    public DiagnosticsPage()
    {
        this.InitializeComponent();
        _vm.SetDispatcher(DispatcherQueue.GetForCurrentThread());
        PerfList.ItemsSource = _vm.PerfCounters;
        EventLogList.ItemsSource = _vm.EventLogs;
        ServiceList.ItemsSource = _vm.Services;
        DiagLogList.ItemsSource = _vm.DiagnosticLog;
        _propertyChangedHandler = (_, e) =>
        {
            if (e.PropertyName == nameof(_vm.StatusMessage))
                StatusText.Text = _vm.StatusMessage;
            if (e.PropertyName == nameof(_vm.IsLoading))
                LoadingBar.Visibility = _vm.IsLoading ? Visibility.Visible : Visibility.Collapsed;
        };
        _vm.PropertyChanged += _propertyChangedHandler;

        this.Unloaded += (_, _) =>
        {
            _vm.PropertyChanged -= _propertyChangedHandler;
            _vm.Dispose();
        };
    }

    private async void RefreshBtn_Click(object sender, RoutedEventArgs e) => await _vm.LoadAllAsync();
    private void MonitorBtn_Click(object sender, RoutedEventArgs e)
    {
        _vm.ToggleMonitorCommand.Execute(null);
        MonitorBtn.Content = _vm.IsMonitoring ? "⏹️ Stop Monitor" : "⏱️ Start Monitor";
    }
    private void DiagBtn_Click(object sender, RoutedEventArgs e) => _vm.RunDiagnosticsCommand.Execute(null);
    private void ExportJsonBtn_Click(object sender, RoutedEventArgs e) => _vm.ExportJsonCommand.Execute(null);
    private void ExportTextBtn_Click(object sender, RoutedEventArgs e) => _vm.ExportTextCommand.Execute(null);
}