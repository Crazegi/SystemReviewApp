using System.Management;
using System.Runtime.InteropServices;
using SystemReview.Models;

namespace SystemReview.Services;

public static class WmiService
{
    public static Task<List<CpuInfo>> GetCpuInfoAsync() => Task.Run(() =>
    {
        var list = new List<CpuInfo>();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            foreach (var obj in searcher.Get())
            {
                list.Add(new CpuInfo(
                    Name: obj["Name"]?.ToString()?.Trim() ?? "Unknown",
                    Cores: Convert.ToInt32(obj["NumberOfCores"]),
                    LogicalProcessors: Convert.ToInt32(obj["NumberOfLogicalProcessors"]),
                    MaxSpeed: $"{obj["MaxClockSpeed"]} MHz",
                    Architecture: GetArchitecture(Convert.ToInt32(obj["Architecture"]))
                ));
            }
        }
        catch (Exception ex) { list.Add(new CpuInfo($"Error: {ex.Message}", 0, 0, "", "")); }
        return list;
    });

    public static Task<(string TotalRam, string AvailableRam, int UsagePercent)> GetRamInfoAsync() => Task.Run(() =>
    {
        try
        {
            var memStatus = new MEMORYSTATUSEX { dwLength = (uint)Marshal.SizeOf<MEMORYSTATUSEX>() };
            GlobalMemoryStatusEx(ref memStatus);
            return (
                FormatBytes(memStatus.ullTotalPhys),
                FormatBytes(memStatus.ullAvailPhys),
                (int)memStatus.dwMemoryLoad
            );
        }
        catch (Exception ex) { return ($"Error: {ex.Message}", "", 0); }
    });

    public static Task<List<GpuInfo>> GetGpuInfoAsync() => Task.Run(() =>
    {
        var list = new List<GpuInfo>();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            foreach (var obj in searcher.Get())
            {
                var ram = obj["AdapterRAM"];
                list.Add(new GpuInfo(
                    Name: obj["Name"]?.ToString() ?? "Unknown",
                    DriverVersion: obj["DriverVersion"]?.ToString() ?? "N/A",
                    AdapterRam: ram != null ? FormatBytes(Convert.ToUInt64(ram)) : "N/A",
                    VideoProcessor: obj["VideoProcessor"]?.ToString() ?? "N/A"
                ));
            }
        }
        catch (Exception ex) { list.Add(new GpuInfo($"Error: {ex.Message}", "", "", "")); }
        return list;
    });

    public static Task<List<DriveInfoModel>> GetDriveInfoAsync() => Task.Run(() =>
    {
        var list = new List<DriveInfoModel>();
        try
        {
            foreach (var d in DriveInfo.GetDrives().Where(d => d.IsReady))
            {
                var used = (double)(d.TotalSize - d.TotalFreeSpace) / d.TotalSize * 100;
                list.Add(new DriveInfoModel(
                    Name: d.Name,
                    Label: d.VolumeLabel,
                    DriveType: d.DriveType.ToString(),
                    FileSystem: d.DriveFormat,
                    TotalSize: FormatBytes((ulong)d.TotalSize),
                    FreeSpace: FormatBytes((ulong)d.TotalFreeSpace),
                    UsedPercent: Math.Round(used, 1)
                ));
            }
        }
        catch (Exception ex) { list.Add(new DriveInfoModel($"Error: {ex.Message}", "", "", "", "", "", 0)); }
        return list;
    });

    public static Task<Dictionary<string, string>> GetMotherboardInfoAsync() => Task.Run(() =>
    {
        var info = new Dictionary<string, string>();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            foreach (var obj in searcher.Get())
            {
                info["Manufacturer"] = obj["Manufacturer"]?.ToString() ?? "N/A";
                info["Product"] = obj["Product"]?.ToString() ?? "N/A";
                info["SerialNumber"] = obj["SerialNumber"]?.ToString() ?? "N/A";
            }
            using var bios = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
            foreach (var obj in bios.Get())
            {
                info["BIOS Version"] = obj["SMBIOSBIOSVersion"]?.ToString() ?? "N/A";
                info["BIOS Manufacturer"] = obj["Manufacturer"]?.ToString() ?? "N/A";
                info["BIOS Release Date"] = obj["ReleaseDate"]?.ToString() ?? "N/A";
            }
        }
        catch (Exception ex) { info["Error"] = ex.Message; }
        return info;
    });

    public static Task<Dictionary<string, string>> GetOsInfoAsync() => Task.Run(() =>
    {
        var info = new Dictionary<string, string>();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            foreach (var obj in searcher.Get())
            {
                info["OS Name"] = obj["Caption"]?.ToString() ?? "N/A";
                info["Version"] = obj["Version"]?.ToString() ?? "N/A";
                info["Build Number"] = obj["BuildNumber"]?.ToString() ?? "N/A";
                info["Architecture"] = obj["OSArchitecture"]?.ToString() ?? "N/A";
                info["Install Date"] = obj["InstallDate"]?.ToString() ?? "N/A";

                if (obj["LastBootUpTime"]?.ToString() is string bootTime)
                {
                    var boot = ManagementDateTimeConverter.ToDateTime(bootTime);
                    var uptime = DateTime.Now - boot;
                    info["Uptime"] = $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m";
                    info["Last Boot"] = boot.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            info["Machine Name"] = Environment.MachineName;
            info["User Name"] = Environment.UserName;
        }
        catch (Exception ex) { info["Error"] = ex.Message; }
        return info;
    });

    public static Task<List<Dictionary<string, string>>> GetDisplayInfoAsync() => Task.Run(() =>
    {
        var list = new List<Dictionary<string, string>>();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DesktopMonitor");
            foreach (var obj in searcher.Get())
            {
                var d = new Dictionary<string, string>
                {
                    ["Name"] = obj["Name"]?.ToString() ?? "N/A",
                    ["ScreenWidth"] = obj["ScreenWidth"]?.ToString() ?? "N/A",
                    ["ScreenHeight"] = obj["ScreenHeight"]?.ToString() ?? "N/A",
                    ["MonitorType"] = obj["MonitorType"]?.ToString() ?? "N/A"
                };
                list.Add(d);
            }
            // Fallback to VideoController for resolution info
            if (list.Count == 0)
            {
                using var vc = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
                foreach (var obj in vc.Get())
                {
                    var d = new Dictionary<string, string>
                    {
                        ["Name"] = obj["Name"]?.ToString() ?? "N/A",
                        ["ScreenWidth"] = obj["CurrentHorizontalResolution"]?.ToString() ?? "N/A",
                        ["ScreenHeight"] = obj["CurrentVerticalResolution"]?.ToString() ?? "N/A",
                        ["RefreshRate"] = $"{obj["CurrentRefreshRate"]} Hz"
                    };
                    list.Add(d);
                }
            }
        }
        catch (Exception ex) { list.Add(new Dictionary<string, string> { ["Error"] = ex.Message }); }
        return list;
    });

    // --- Native Memory ---
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

    public static string FormatBytes(ulong bytes)
    {
        string[] suffixes = ["B", "KB", "MB", "GB", "TB"];
        int i = 0;
        double d = bytes;
        while (d >= 1024 && i < suffixes.Length - 1) { d /= 1024; i++; }
        return $"{d:F2} {suffixes[i]}";
    }

    private static string GetArchitecture(int arch) => arch switch
    {
        0 => "x86",
        5 => "ARM",
        9 => "x64",
        12 => "ARM64",
        _ => $"Unknown ({arch})"
    };

        public static Task<Dictionary<string, string>> GetBatteryInfoAsync() => Task.Run(() =>
    {
        var info = new Dictionary<string, string>();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Battery");
            var results = searcher.Get();
            if (results.Count == 0)
            {
                info["Status"] = "No battery detected (Desktop PC)";
                return info;
            }
            foreach (var obj in results)
            {
                info["Name"] = obj["Name"]?.ToString() ?? "N/A";
                info["Status"] = obj["Status"]?.ToString() ?? "N/A";
                info["Charge"] = $"{obj["EstimatedChargeRemaining"]}%";
                var runTime = obj["EstimatedRunTime"];
                info["Est. Runtime"] = runTime != null && Convert.ToInt64(runTime) < 71582788
                    ? $"{Convert.ToInt64(runTime)} min"
                    : "Charging / AC Power";
                info["Chemistry"] = (Convert.ToInt32(obj["Chemistry"] ?? 0)) switch
                {
                    1 => "Other",
                    2 => "Unknown",
                    3 => "Lead Acid",
                    4 => "Nickel Cadmium",
                    5 => "Nickel Metal Hydride",
                    6 => "Lithium-ion",
                    7 => "Zinc Air",
                    8 => "Lithium Polymer",
                    _ => "N/A"
                };
                info["Design Voltage"] = $"{obj["DesignVoltage"]} mV";
            }
        }
        catch (Exception ex) { info["Error"] = ex.Message; }
        return info;
    });

    public static Task<List<KeyValuePair<string, string>>> GetInstalledSoftwareAsync() => Task.Run(() =>
    {
        var list = new List<KeyValuePair<string, string>>();
        try
        {
            using var searcher = new ManagementObjectSearcher(
                "SELECT Name, Version FROM Win32_Product");
            var results = searcher.Get()
                .Cast<ManagementObject>()
                .Where(o => o["Name"] != null)
                .OrderBy(o => o["Name"]?.ToString())
                .Take(20);

            foreach (var obj in results)
            {
                var name = obj["Name"]?.ToString() ?? "Unknown";
                var version = obj["Version"]?.ToString() ?? "";
                list.Add(new(name, version));
            }

            if (list.Count == 0)
            {
                // Fallback: read from registry via WMI
                using var regSearcher = new ManagementObjectSearcher(
                    @"SELECT * FROM Win32_InstalledWin32Program");
                foreach (var obj in regSearcher.Get().Cast<ManagementObject>()
                    .Where(o => o["Name"] != null)
                    .OrderBy(o => o["Name"]?.ToString())
                    .Take(20))
                {
                    list.Add(new(
                        obj["Name"]?.ToString() ?? "Unknown",
                        obj["Version"]?.ToString() ?? ""
                    ));
                }
            }
        }
        catch (Exception ex) { list.Add(new("Error loading software", ex.Message)); }
        return list;
    });


}
