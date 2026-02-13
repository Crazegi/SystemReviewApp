using System.Diagnostics;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using SystemReview.Models;

namespace SystemReview.Services;

public static class NetworkService
{
    public static Task<Dictionary<string, string>> GetIpConfigAsync() => Task.Run(() =>
    {
        var info = new Dictionary<string, string>();
        try
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up);

            foreach (var ni in interfaces)
            {
                var props = ni.GetIPProperties();
                var ipv4 = props.UnicastAddresses
                    .FirstOrDefault(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                var ipv6 = props.UnicastAddresses
                    .FirstOrDefault(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6);
                var gateway = props.GatewayAddresses.FirstOrDefault();

                var prefix = ni.Name;
                info[$"{prefix} - Type"] = ni.NetworkInterfaceType.ToString();
                info[$"{prefix} - Status"] = ni.OperationalStatus.ToString();
                info[$"{prefix} - IPv4"] = ipv4?.Address.ToString() ?? "N/A";
                info[$"{prefix} - IPv6"] = ipv6?.Address.ToString() ?? "N/A";
                info[$"{prefix} - Gateway"] = gateway?.Address.ToString() ?? "N/A";
                info[$"{prefix} - DNS Suffix"] = props.DnsSuffix ?? "N/A";
                info[$"{prefix} - Speed"] = $"{ni.Speed / 1_000_000} Mbps";
            }
        }
        catch (Exception ex) { info["Error"] = ex.Message; }
        return info;
    });

    public static Task<List<NetworkAdapterInfo>> GetAdaptersAsync() => Task.Run(() =>
    {
        var list = new List<NetworkAdapterInfo>();
        try
        {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                list.Add(new NetworkAdapterInfo(
                    Name: ni.Name,
                    Description: ni.Description,
                    Status: ni.OperationalStatus.ToString(),
                    Speed: ni.OperationalStatus == OperationalStatus.Up ? $"{ni.Speed / 1_000_000} Mbps" : "N/A",
                    MacAddress: FormatMac(ni.GetPhysicalAddress().ToString()),
                    AdapterType: ni.NetworkInterfaceType.ToString()
                ));
            }
        }
        catch (Exception ex) { list.Add(new NetworkAdapterInfo($"Error: {ex.Message}", "", "", "", "", "")); }
        return list;
    });

    public static async Task<PingResultModel> PingHostAsync(string host, int timeout = 3000)
    {
        try
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(host, timeout);
            return new PingResultModel(
                Host: host,
                Status: reply.Status.ToString(),
                RoundtripMs: reply.RoundtripTime,
                Ttl: reply.Options?.Ttl ?? 0,
                Timestamp: DateTime.Now.ToString("HH:mm:ss.fff")
            );
        }
        catch (Exception ex)
        {
            return new PingResultModel(host, $"Error: {ex.Message}", 0, 0, DateTime.Now.ToString("HH:mm:ss.fff"));
        }
    }

    public static async Task<List<TracerouteHop>> TracerouteAsync(string host, Action<TracerouteHop>? onHop = null)
    {
        var hops = new List<TracerouteHop>();
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "tracert",
                Arguments = $"-d -w 1000 -h 30 {host}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var proc = Process.Start(psi);
            if (proc == null) return hops;

            int hopNum = 0;
            while (await proc.StandardOutput.ReadLineAsync() is { } line)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("Tracing") || trimmed.StartsWith("over")) continue;

                hopNum++;
                var parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 1)
                {
                    var hop = new TracerouteHop(
                        Hop: hopNum,
                        Address: parts.LastOrDefault() ?? "*",
                        RoundtripMs: string.Join(" ", parts.Skip(1).Take(parts.Length - 2)),
                        Status: trimmed.Contains('*') ? "Timeout" : "OK"
                    );
                    hops.Add(hop);
                    onHop?.Invoke(hop);
                }
            }

            await proc.WaitForExitAsync();
        }
        catch (Exception ex) { hops.Add(new TracerouteHop(0, "Error", ex.Message, "Error")); }
        return hops;
    }

    public static Task<List<OpenPortInfo>> GetOpenPortsAsync() => Task.Run(() =>
    {
        var list = new List<OpenPortInfo>();
        try
        {
            var ipProps = IPGlobalProperties.GetIPGlobalProperties();

            foreach (var tcp in ipProps.GetActiveTcpConnections())
            {
                list.Add(new OpenPortInfo(
                    "TCP",
                    tcp.LocalEndPoint.Address.ToString(),
                    tcp.LocalEndPoint.Port,
                    tcp.RemoteEndPoint.Address.ToString(),
                    tcp.RemoteEndPoint.Port,
                    tcp.State.ToString(),
                    0
                ));
            }

            foreach (var listener in ipProps.GetActiveTcpListeners())
            {
                list.Add(new OpenPortInfo(
                    "TCP",
                    listener.Address.ToString(),
                    listener.Port,
                    "*",
                    0,
                    "LISTENING",
                    0
                ));
            }

            foreach (var udp in ipProps.GetActiveUdpListeners())
            {
                list.Add(new OpenPortInfo(
                    "UDP",
                    udp.Address.ToString(),
                    udp.Port,
                    "*",
                    0,
                    "LISTENING",
                    0
                ));
            }
        }
        catch (Exception ex) { list.Add(new OpenPortInfo("Error", ex.Message, 0, "", 0, "", 0)); }
        return list;
    });

    public static async Task<List<DnsResultModel>> DnsLookupAsync(string hostname)
    {
        var list = new List<DnsResultModel>();
        try
        {
            var addresses = await Dns.GetHostAddressesAsync(hostname);
            foreach (var addr in addresses)
            {
                list.Add(new DnsResultModel(hostname, addr.AddressFamily.ToString(), addr.ToString()));
            }
        }
        catch (Exception ex) { list.Add(new DnsResultModel(hostname, "Error", ex.Message)); }
        return list;
    }

    public static Task<Dictionary<string, string>> GetNetworkStatsAsync() => Task.Run(() =>
    {
        var stats = new Dictionary<string, string>();
        try
        {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up && n.NetworkInterfaceType != NetworkInterfaceType.Loopback))
            {
                var s = ni.GetIPv4Statistics();
                stats[$"{ni.Name} - Bytes Sent"] = WmiService.FormatBytes((ulong)s.BytesSent);
                stats[$"{ni.Name} - Bytes Received"] = WmiService.FormatBytes((ulong)s.BytesReceived);
                stats[$"{ni.Name} - Packets Sent"] = s.UnicastPacketsSent.ToString("N0");
                stats[$"{ni.Name} - Packets Received"] = s.UnicastPacketsReceived.ToString("N0");
                stats[$"{ni.Name} - Errors Out"] = s.OutgoingPacketsWithErrors.ToString("N0");
                stats[$"{ni.Name} - Errors In"] = s.IncomingPacketsWithErrors.ToString("N0");
                stats[$"{ni.Name} - Speed"] = $"{ni.Speed / 1_000_000} Mbps";
            }
        }
        catch (Exception ex) { stats["Error"] = ex.Message; }
        return stats;
    });

    private static string FormatMac(string mac)
    {
        if (string.IsNullOrEmpty(mac)) return "N/A";
        return string.Join(":", Enumerable.Range(0, mac.Length / 2).Select(i => mac.Substring(i * 2, 2)));
    }
}