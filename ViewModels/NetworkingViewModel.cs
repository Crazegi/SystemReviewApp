using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using SystemReview.Helpers;
using SystemReview.Models;
using SystemReview.Services;
using Microsoft.UI.Dispatching;

namespace SystemReview.ViewModels;

public class NetworkingViewModel : ObservableObject, IDisposable
{
    private string _statusMessage = "Ready.";
    private bool _isLoading;
    private string _pingHost = "8.8.8.8";
    private string _tracerouteHost = "8.8.8.8";
    private string _dnsHost = "www.microsoft.com";
    private DispatcherQueue? _dispatcherQueue;
    private readonly NetworkAvailabilityChangedEventHandler _networkAvailabilityChangedHandler;
    private readonly NetworkAddressChangedEventHandler _networkAddressChangedHandler;
    private bool _disposed;

    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public string PingHost { get => _pingHost; set => SetProperty(ref _pingHost, value); }
    public string TracerouteHost { get => _tracerouteHost; set => SetProperty(ref _tracerouteHost, value); }
    public string DnsHost { get => _dnsHost; set => SetProperty(ref _dnsHost, value); }

    public ObservableCollection<KeyValuePair<string, string>> IpConfig { get; } = [];
    public ObservableCollection<NetworkAdapterInfo> Adapters { get; } = [];
    public ObservableCollection<PingResultModel> PingResults { get; } = [];
    public ObservableCollection<TracerouteHop> TracerouteResults { get; } = [];
    public ObservableCollection<OpenPortInfo> OpenPorts { get; } = [];
    public ObservableCollection<DnsResultModel> DnsResults { get; } = [];
    public ObservableCollection<KeyValuePair<string, string>> NetStats { get; } = [];

    public AsyncRelayCommand LoadAllCommand { get; }
    public AsyncRelayCommand PingCommand { get; }
    public AsyncRelayCommand TracerouteCommand { get; }
    public AsyncRelayCommand ScanPortsCommand { get; }
    public AsyncRelayCommand DnsLookupCommand { get; }
    public AsyncRelayCommand ExportJsonCommand { get; }
    public AsyncRelayCommand ExportTextCommand { get; }

    public NetworkingViewModel()
    {
        LoadAllCommand = new AsyncRelayCommand(async _ => await LoadAllAsync());
        PingCommand = new AsyncRelayCommand(async _ => await RunPingAsync());
        TracerouteCommand = new AsyncRelayCommand(async _ => await RunTracerouteAsync());
        ScanPortsCommand = new AsyncRelayCommand(async _ => await ScanPortsAsync());
        DnsLookupCommand = new AsyncRelayCommand(async _ => await RunDnsLookupAsync());
        ExportJsonCommand = new AsyncRelayCommand(async _ => await ExportAsync(true));
        ExportTextCommand = new AsyncRelayCommand(async _ => await ExportAsync(false));

        _networkAvailabilityChangedHandler = (_, e) =>
        {
            _dispatcherQueue?.TryEnqueue(() =>
            {
                StatusMessage = e.IsAvailable ? "🟢 Network available" : "🔴 Network unavailable";
                _ = LoadAllAsync();
            });
        };

        _networkAddressChangedHandler = (_, _) =>
        {
            _dispatcherQueue?.TryEnqueue(() =>
            {
                StatusMessage = "🔄 Network address changed, refreshing...";
                _ = LoadAllAsync();
            });
        };

        NetworkChange.NetworkAvailabilityChanged += _networkAvailabilityChangedHandler;
        NetworkChange.NetworkAddressChanged += _networkAddressChangedHandler;
    }

    public void SetDispatcher(DispatcherQueue dq) => _dispatcherQueue = dq;

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        NetworkChange.NetworkAvailabilityChanged -= _networkAvailabilityChangedHandler;
        NetworkChange.NetworkAddressChanged -= _networkAddressChangedHandler;
    }

    public async Task LoadAllAsync()
    {
        IsLoading = true;
        StatusMessage = "Loading network information...";
        try
        {
            var ipTask = NetworkService.GetIpConfigAsync();
            var adTask = NetworkService.GetAdaptersAsync();
            var stTask = NetworkService.GetNetworkStatsAsync();
            await Task.WhenAll(ipTask, adTask, stTask);

            IpConfig.Clear();
            foreach (var kv in ipTask.Result) IpConfig.Add(kv);

            Adapters.Clear();
            foreach (var a in adTask.Result) Adapters.Add(a);

            NetStats.Clear();
            foreach (var kv in stTask.Result) NetStats.Add(kv);

            StatusMessage = $"Loaded at {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task RunPingAsync()
    {
        if (string.IsNullOrWhiteSpace(PingHost)) { StatusMessage = "Enter a host to ping."; return; }
        IsLoading = true;
        StatusMessage = $"Pinging {PingHost}...";
        PingResults.Clear();
        for (int i = 0; i < 4; i++)
        {
            var result = await NetworkService.PingHostAsync(PingHost);
            PingResults.Add(result);
            await Task.Delay(500);
        }
        StatusMessage = "Ping complete.";
        IsLoading = false;
    }

    private async Task RunTracerouteAsync()
    {
        if (string.IsNullOrWhiteSpace(TracerouteHost)) { StatusMessage = "Enter a host for traceroute."; return; }
        IsLoading = true;
        StatusMessage = $"Traceroute to {TracerouteHost}...";
        TracerouteResults.Clear();
        await NetworkService.TracerouteAsync(TracerouteHost, hop =>
        {
            _dispatcherQueue?.TryEnqueue(() => TracerouteResults.Add(hop));
        });
        StatusMessage = "Traceroute complete.";
        IsLoading = false;
    }

    private async Task ScanPortsAsync()
    {
        IsLoading = true;
        StatusMessage = "Scanning open ports...";
        OpenPorts.Clear();
        var ports = await NetworkService.GetOpenPortsAsync();
        foreach (var p in ports) OpenPorts.Add(p);
        StatusMessage = $"Found {ports.Count} connections/listeners.";
        IsLoading = false;
    }

    private async Task RunDnsLookupAsync()
    {
        if (string.IsNullOrWhiteSpace(DnsHost)) { StatusMessage = "Enter a hostname."; return; }
        IsLoading = true;
        StatusMessage = $"Resolving {DnsHost}...";
        DnsResults.Clear();
        var results = await NetworkService.DnsLookupAsync(DnsHost);
        foreach (var r in results) DnsResults.Add(r);
        StatusMessage = $"Resolved {results.Count} address(es).";
        IsLoading = false;
    }

    private async Task ExportAsync(bool asJson)
    {
        try
        {
            var data = new Dictionary<string, object>
            {
                ["IPConfig"] = ToSafeDictionary(IpConfig),
                ["Adapters"] = Adapters.ToList(),
                ["PingResults"] = PingResults.ToList(),
                ["Traceroute"] = TracerouteResults.ToList(),
                ["OpenPorts"] = OpenPorts.ToList(),
                ["DnsResults"] = DnsResults.ToList(),
                ["NetworkStats"] = ToSafeDictionary(NetStats)
            };

            var filename = $"SystemReview_Network_{DateTime.Now:yyyyMMdd_HHmmss}";
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (asJson)
            {
                var path = Path.Combine(desktop, $"{filename}.json");
                await File.WriteAllTextAsync(path, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
                StatusMessage = $"Exported to {path}";
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine("╔══════════════════════════════════════════╗");
                sb.AppendLine("║       SYSTEM REVIEW - NETWORKING         ║");
                sb.AppendLine($"║  Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}            ║");
                sb.AppendLine("╚══════════════════════════════════════════╝\n");

                sb.AppendLine("── IP Configuration ─────────────");
                foreach (var kv in IpConfig) sb.AppendLine($"  {kv.Key}: {kv.Value}");

                sb.AppendLine("\n── Adapters ─────────────────────");
                foreach (var a in Adapters) sb.AppendLine($"  {a.Name} [{a.AdapterType}] Status:{a.Status} Speed:{a.Speed} MAC:{a.MacAddress}");

                sb.AppendLine("\n── Ping Results ─────────────────");
                foreach (var p in PingResults) sb.AppendLine($"  {p.Timestamp} {p.Host} → {p.Status} {p.RoundtripMs}ms TTL:{p.Ttl}");

                sb.AppendLine("\n── Traceroute ───────────────────");
                foreach (var h in TracerouteResults) sb.AppendLine($"  Hop {h.Hop}: {h.Address} {h.RoundtripMs} [{h.Status}]");

                sb.AppendLine("\n── Open Ports ───────────────────");
                foreach (var p in OpenPorts) sb.AppendLine($"  {p.Protocol} {p.LocalAddress}:{p.LocalPort} → {p.RemoteAddress}:{p.RemotePort} [{p.State}]");

                sb.AppendLine("\n── DNS ──────────────────────────");
                foreach (var d in DnsResults) sb.AppendLine($"  {d.HostName} → {d.Address} ({d.AddressFamily})");

                sb.AppendLine("\n── Network Stats ────────────────");
                foreach (var kv in NetStats) sb.AppendLine($"  {kv.Key}: {kv.Value}");

                var path = Path.Combine(desktop, $"{filename}.txt");
                await File.WriteAllTextAsync(path, sb.ToString());
                StatusMessage = $"Exported to {path}";
            }
        }
        catch (Exception ex) { StatusMessage = $"Export error: {ex.Message}"; }
    }

    private static Dictionary<string, string> ToSafeDictionary(IEnumerable<KeyValuePair<string, string>> entries)
    {
        return entries
            .GroupBy(kv => kv.Key)
            .ToDictionary(
                g => g.Key,
                g => string.Join(" | ", g.Select(x => x.Value).Where(v => !string.IsNullOrWhiteSpace(v)).Distinct())
            );
    }
}