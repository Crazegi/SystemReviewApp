using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
using SystemReview.ViewModels;

namespace SystemReview.Views;

public sealed partial class NetworkingPage : Page
{
    private readonly NetworkingViewModel _vm = new();

    public NetworkingPage()
    {
        this.InitializeComponent();
        _vm.SetDispatcher(DispatcherQueue.GetForCurrentThread());
        IpConfigList.ItemsSource = _vm.IpConfig;
        AdapterList.ItemsSource = _vm.Adapters;
        PingList.ItemsSource = _vm.PingResults;
        TraceList.ItemsSource = _vm.TracerouteResults;
        PortList.ItemsSource = _vm.OpenPorts;
        DnsList.ItemsSource = _vm.DnsResults;
        NetStatsList.ItemsSource = _vm.NetStats;
        _vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(_vm.StatusMessage))
                StatusText.Text = _vm.StatusMessage;
            if (e.PropertyName == nameof(_vm.IsLoading))
                LoadingBar.Visibility = _vm.IsLoading ? Visibility.Visible : Visibility.Collapsed;
        };
    }

    private async void RefreshBtn_Click(object sender, RoutedEventArgs e) => await _vm.LoadAllAsync();
    private void PingBtn_Click(object sender, RoutedEventArgs e) { _vm.PingHost = PingInput.Text; _vm.PingCommand.Execute(null); }
    private void TraceBtn_Click(object sender, RoutedEventArgs e) { _vm.TracerouteHost = TraceInput.Text; _vm.TracerouteCommand.Execute(null); }
    private void ScanPortsBtn_Click(object sender, RoutedEventArgs e) => _vm.ScanPortsCommand.Execute(null);
    private void DnsBtn_Click(object sender, RoutedEventArgs e) { _vm.DnsHost = DnsInput.Text; _vm.DnsLookupCommand.Execute(null); }
    private void ExportJsonBtn_Click(object sender, RoutedEventArgs e) => _vm.ExportJsonCommand.Execute(null);
    private void ExportTextBtn_Click(object sender, RoutedEventArgs e) => _vm.ExportTextCommand.Execute(null);
}