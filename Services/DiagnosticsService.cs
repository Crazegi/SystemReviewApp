using System.Diagnostics;
using System.ServiceProcess;
using SystemReview.Models;

namespace SystemReview.Services;

public static class DiagnosticsService
{
    private static PerformanceCounter? _cpuCounter;
    private static PerformanceCounter? _ramCounter;
    private static PerformanceCounter? _diskCounter;

    public static void InitializeCounters()
    {
        try
        {
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            _diskCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");
            // Prime the counters
            _cpuCounter.NextValue();
            _ramCounter.NextValue();
            _diskCounter.NextValue();
        }
        catch { /* Counters may not be available */ }
    }

    public static Task<Dictionary<string, string>> GetPerformanceDataAsync() => Task.Run(() =>
    {
        var data = new Dictionary<string, string>();
        try
        {
            if (_cpuCounter != null) data["CPU Usage"] = $"{_cpuCounter.NextValue():F1}%";
            if (_ramCounter != null) data["Available RAM"] = $"{_ramCounter.NextValue():F0} MB";
            if (_diskCounter != null) data["Disk Activity"] = $"{_diskCounter.NextValue():F1}%";

            // Additional via WMI fallback
            var proc = Process.GetCurrentProcess();
            data["App Memory"] = WmiService.FormatBytes((ulong)proc.WorkingSet64);
            data["App Threads"] = proc.Threads.Count.ToString();
            data["System Processes"] = Process.GetProcesses().Length.ToString();
        }
        catch (Exception ex) { data["Error"] = ex.Message; }
        return data;
    });

    public static Task<List<EventLogEntryModel>> GetRecentEventLogsAsync(int count = 10) => Task.Run(() =>
    {
        var list = new List<EventLogEntryModel>();
        try
        {
            using var log = new EventLog("System");
            var entries = log.Entries.Cast<EventLogEntry>()
                .Where(e => e.EntryType is EventLogEntryType.Error or EventLogEntryType.Warning)
                .OrderByDescending(e => e.TimeGenerated)
                .Take(count);

            foreach (var entry in entries)
            {
                list.Add(new EventLogEntryModel(
                    Source: entry.Source,
                    Level: entry.EntryType.ToString(),
                    Message: entry.Message.Length > 200 ? entry.Message[..200] + "..." : entry.Message,
                    TimeGenerated: entry.TimeGenerated,
                    InstanceId: entry.InstanceId
                ));
            }
        }
        catch (Exception ex) { list.Add(new EventLogEntryModel("Error", "Error", ex.Message, DateTime.Now, 0)); }
        return list;
    });

    public static Task<List<ServiceInfoModel>> GetNetworkServicesAsync() => Task.Run(() =>
    {
        var list = new List<ServiceInfoModel>();
        string[] networkServices = [
            "Dhcp", "Dnscache", "LanmanServer", "LanmanWorkstation",
            "Netlogon", "NlaSvc", "WlanSvc", "Netman", "RemoteAccess",
            "SharedAccess", "iphlpsvc", "Winmgmt", "W32Time",
            "WinHttpAutoProxySvc", "dot3svc", "SSDPSRV", "upnphost"
        ];

        try
        {
            foreach (var svcName in networkServices)
            {
                try
                {
                    using var sc = new ServiceController(svcName);
                    list.Add(new ServiceInfoModel(
                        Name: sc.ServiceName,
                        DisplayName: sc.DisplayName,
                        Status: sc.Status.ToString(),
                        StartType: sc.StartType.ToString()
                    ));
                }
                catch { /* Service may not exist on this machine */ }
            }
        }
        catch (Exception ex) { list.Add(new ServiceInfoModel("Error", ex.Message, "", "")); }
        return list;
    });

    public static async Task<List<string>> RunDiagnosticsAsync(Action<string>? onProgress = null)
    {
        var results = new List<string>();

        void Log(string msg) { results.Add(msg); onProgress?.Invoke(msg); }

        Log("=== System Diagnostics Report ===");
        Log($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        Log("");

        // Internet connectivity
        Log("[1/5] Testing Internet Connectivity...");
        try
        {
            using var ping = new System.Net.NetworkInformation.Ping();
            var reply = await ping.SendPingAsync("8.8.8.8", 3000);
            Log(reply.Status == System.Net.NetworkInformation.IPStatus.Success
                ? $"  ✅ Internet: OK ({reply.RoundtripTime}ms)"
                : $"  ❌ Internet: {reply.Status}");
        }
        catch (Exception ex) { Log($"  ❌ Internet: {ex.Message}"); }

        // DNS resolution
        Log("[2/5] Testing DNS Resolution...");
        try
        {
            var addresses = await System.Net.Dns.GetHostAddressesAsync("www.microsoft.com");
            Log($"  ✅ DNS: OK (resolved to {addresses.First()})");
        }
        catch (Exception ex) { Log($"  ❌ DNS: {ex.Message}"); }

        // Firewall
        Log("[3/5] Checking Firewall Status...");
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "netsh",
                Arguments = "advfirewall show allprofiles state",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var proc = Process.Start(psi);
            if (proc != null)
            {
                var output = await proc.StandardOutput.ReadToEndAsync();
                await proc.WaitForExitAsync();
                var lines = output.Split('\n').Where(l => l.Contains("State")).Select(l => l.Trim());
                foreach (var line in lines) Log($"  🔥 {line}");
            }
        }
        catch (Exception ex) { Log($"  ⚠️ Firewall: {ex.Message}"); }

        // Network adapters
        Log("[4/5] Checking Network Adapters...");
        try
        {
            var adapters = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up);
            foreach (var a in adapters)
                Log($"  🔌 {a.Name}: {a.NetworkInterfaceType} - {a.Speed / 1_000_000} Mbps");
        }
        catch (Exception ex) { Log($"  ❌ Adapters: {ex.Message}"); }

        // Gateway
        Log("[5/5] Testing Gateway...");
        try
        {
            var gateways = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
                .SelectMany(n => n.GetIPProperties().GatewayAddresses)
                .Where(g => g.Address.ToString() != "0.0.0.0");
            foreach (var gw in gateways)
            {
                using var ping = new System.Net.NetworkInformation.Ping();
                var reply = await ping.SendPingAsync(gw.Address, 2000);
                Log(reply.Status == System.Net.NetworkInformation.IPStatus.Success
                    ? $"  ✅ Gateway {gw.Address}: OK ({reply.RoundtripTime}ms)"
                    : $"  ❌ Gateway {gw.Address}: {reply.Status}");
            }
        }
        catch (Exception ex) { Log($"  ❌ Gateway: {ex.Message}"); }

        Log("");
        Log("=== Diagnostics Complete ===");
        return results;
    }
}