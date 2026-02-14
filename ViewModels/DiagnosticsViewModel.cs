using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using SystemReview.Helpers;
using SystemReview.Models;
using SystemReview.Services;
using Microsoft.UI.Dispatching;

namespace SystemReview.ViewModels;

public class DiagnosticsViewModel : ObservableObject, IDisposable
{
    private string _statusMessage = "Ready.";
    private bool _isLoading;
    private bool _isMonitoring;
    private CancellationTokenSource? _monitorCts;
    private DispatcherQueue? _dispatcherQueue;
    private bool _disposed;

    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public bool IsMonitoring { get => _isMonitoring; set => SetProperty(ref _isMonitoring, value); }

    public ObservableCollection<KeyValuePair<string, string>> PerfCounters { get; } = [];
    public ObservableCollection<EventLogEntryModel> EventLogs { get; } = [];
    public ObservableCollection<ServiceInfoModel> Services { get; } = [];
    public ObservableCollection<string> DiagnosticLog { get; } = [];

    public AsyncRelayCommand LoadAllCommand { get; }
    public RelayCommand ToggleMonitorCommand { get; }
    public AsyncRelayCommand RunDiagnosticsCommand { get; }
    public AsyncRelayCommand ExportJsonCommand { get; }
    public AsyncRelayCommand ExportTextCommand { get; }

    public DiagnosticsViewModel()
    {
        LoadAllCommand = new AsyncRelayCommand(async _ => await LoadAllAsync());
        ToggleMonitorCommand = new RelayCommand(_ => ToggleMonitoring());
        RunDiagnosticsCommand = new AsyncRelayCommand(async _ => await RunDiagnosticsAsync());
        ExportJsonCommand = new AsyncRelayCommand(async _ => await ExportAsync(true));
        ExportTextCommand = new AsyncRelayCommand(async _ => await ExportAsync(false));

        DiagnosticsService.InitializeCounters();
    }

    public void SetDispatcher(DispatcherQueue dq) => _dispatcherQueue = dq;

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _monitorCts?.Cancel();
        _monitorCts?.Dispose();
        _monitorCts = null;
        IsMonitoring = false;
    }

    public async Task LoadAllAsync()
    {
        IsLoading = true;
        StatusMessage = "Loading diagnostics...";
        try
        {
            var perfTask = DiagnosticsService.GetPerformanceDataAsync();
            var logTask = DiagnosticsService.GetRecentEventLogsAsync();
            var svcTask = DiagnosticsService.GetNetworkServicesAsync();
            await Task.WhenAll(perfTask, logTask, svcTask);

            PerfCounters.Clear();
            foreach (var kv in perfTask.Result) PerfCounters.Add(kv);

            EventLogs.Clear();
            foreach (var e in logTask.Result) EventLogs.Add(e);

            Services.Clear();
            foreach (var s in svcTask.Result) Services.Add(s);

            StatusMessage = $"Loaded at {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private void ToggleMonitoring()
    {
        if (IsMonitoring)
        {
            _monitorCts?.Cancel();
            _monitorCts?.Dispose();
            _monitorCts = null;
            IsMonitoring = false;
            StatusMessage = "Monitoring stopped.";
        }
        else
        {
            _monitorCts = new CancellationTokenSource();
            IsMonitoring = true;
            StatusMessage = "Monitoring started (updates every 2s)...";
            _ = MonitorLoopAsync(_monitorCts.Token);
        }
    }

    private async Task MonitorLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var data = await DiagnosticsService.GetPerformanceDataAsync();
                _dispatcherQueue?.TryEnqueue(() =>
                {
                    PerfCounters.Clear();
                    foreach (var kv in data) PerfCounters.Add(kv);
                });
                await Task.Delay(2000, ct);
            }
            catch (OperationCanceledException) { break; }
            catch { /* Swallow transient errors during monitoring */ }
        }
    }

    private async Task RunDiagnosticsAsync()
    {
        IsLoading = true;
        DiagnosticLog.Clear();
        StatusMessage = "Running diagnostics...";

        await DiagnosticsService.RunDiagnosticsAsync(msg =>
        {
            _dispatcherQueue?.TryEnqueue(() => DiagnosticLog.Add(msg));
        });

        StatusMessage = "Diagnostics complete.";
        IsLoading = false;
    }

    private async Task ExportAsync(bool asJson)
    {
        try
        {
            var data = new Dictionary<string, object>
            {
                ["PerformanceCounters"] = ToSafeDictionary(PerfCounters),
                ["EventLogs"] = EventLogs.ToList(),
                ["Services"] = Services.ToList(),
                ["DiagnosticLog"] = DiagnosticLog.ToList()
            };

            var filename = $"SystemReview_Diagnostics_{DateTime.Now:yyyyMMdd_HHmmss}";
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (asJson)
            {
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                var path = Path.Combine(desktop, $"{filename}.json");
                await File.WriteAllTextAsync(path, json);
                StatusMessage = $"Exported to {path}";
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine("╔══════════════════════════════════════════╗");
                sb.AppendLine("║     SYSTEM REVIEW - DIAGNOSTICS          ║");
                sb.AppendLine($"║  Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}            ║");
                sb.AppendLine("╚══════════════════════════════════════════╝");
                sb.AppendLine();

                sb.AppendLine("── Performance Counters ─────────");
                foreach (var kv in PerfCounters)
                    sb.AppendLine($"  {kv.Key}: {kv.Value}");

                sb.AppendLine("\n── Event Logs (Recent Errors/Warnings) ──");
                foreach (var e in EventLogs)
                    sb.AppendLine($"  [{e.TimeGenerated:yyyy-MM-dd HH:mm}] {e.Level} | {e.Source} | {e.Message}");

                sb.AppendLine("\n── Network Services ─────────────");
                foreach (var s in Services)
                    sb.AppendLine($"  {s.DisplayName} ({s.Name}) — {s.Status} [{s.StartType}]");

                sb.AppendLine("\n── Diagnostic Log ───────────────");
                foreach (var line in DiagnosticLog)
                    sb.AppendLine($"  {line}");

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