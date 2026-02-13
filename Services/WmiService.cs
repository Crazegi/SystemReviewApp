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
            using var searcher = new ManagementObjectSearcher("SELECT Name, Version FROM Win32_Product");
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
                using var regSearcher = new ManagementObjectSearcher(@"SELECT * FROM Win32_InstalledWin32Program");
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

    // ═══════════════════════════════════════════════════
    // DISK HEALTH - S.M.A.R.T. DATA
    // ═══════════════════════════════════════════════════

    private static readonly Dictionary<int, string> SmartAttributeNames = new()
    {
        [1] = "Read Error Rate",
        [2] = "Throughput Performance",
        [3] = "Spin-Up Time",
        [4] = "Start/Stop Count",
        [5] = "Reallocated Sectors Count",
        [7] = "Seek Error Rate",
        [8] = "Seek Time Performance",
        [9] = "Power-On Hours",
        [10] = "Spin Retry Count",
        [11] = "Recalibration Retries",
        [12] = "Power Cycle Count",
        [170] = "Available Reserved Space",
        [171] = "Program Fail Count",
        [172] = "Erase Fail Count",
        [173] = "Wear Leveling Count",
        [174] = "Unexpected Power Loss",
        [175] = "Power Loss Protection Failure",
        [176] = "Erase Fail Count (Chip)",
        [177] = "Wear Range Delta",
        [178] = "Used Reserved Block Count (Chip)",
        [179] = "Used Reserved Block Count (Total)",
        [180] = "Unused Reserved Block Count (Total)",
        [181] = "Program Fail Count (Total)",
        [182] = "Erase Fail Count (Total)",
        [183] = "Runtime Bad Block",
        [184] = "End-to-End Error",
        [187] = "Reported Uncorrectable Errors",
        [188] = "Command Timeout",
        [189] = "High Fly Writes",
        [190] = "Airflow Temperature",
        [191] = "G-Sense Error Rate",
        [192] = "Power-Off Retract Count",
        [193] = "Load Cycle Count",
        [194] = "Temperature",
        [195] = "Hardware ECC Recovered",
        [196] = "Reallocation Event Count",
        [197] = "Current Pending Sector Count",
        [198] = "Offline Uncorrectable Sector Count",
        [199] = "UltraDMA CRC Error Count",
        [200] = "Multi-Zone Error Rate",
        [201] = "Soft Read Error Rate",
        [220] = "Disk Shift",
        [222] = "Loaded Hours",
        [223] = "Load Retry Count",
        [224] = "Load Friction",
        [225] = "Load Cycle Count",
        [226] = "Load-in Time",
        [230] = "Drive Life Protection Status",
        [231] = "SSD Life Left",
        [232] = "Endurance Remaining",
        [233] = "Media Wearout Indicator",
        [234] = "Average/Max Erase Count",
        [235] = "Good/System Block Count",
        [240] = "Head Flying Hours",
        [241] = "Total LBAs Written",
        [242] = "Total LBAs Read",
        [243] = "Total LBAs Written Expanded",
        [244] = "Total LBAs Read Expanded",
        [249] = "NAND Writes (1GiB)",
        [250] = "Read Error Retry Rate",
        [251] = "Minimum Spares Remaining",
        [252] = "Newly Added Bad Flash Block",
        [254] = "Free Fall Protection"
    };

    public static Task<List<DiskHealthModel>> GetDiskHealthAsync() => Task.Run(() =>
    {
        var disks = new List<DiskHealthModel>();
        try
        {
            var diskInfoMap = new Dictionary<string, (string Model, string Serial, string Firmware, string Interface, string MediaType, ulong Size, string Status)>();

            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive"))
            {
                foreach (var obj in searcher.Get())
                {
                    var deviceId = obj["DeviceID"]?.ToString() ?? "";
                    var model = obj["Model"]?.ToString()?.Trim() ?? "Unknown";
                    var serial = obj["SerialNumber"]?.ToString()?.Trim() ?? "N/A";
                    var firmware = obj["FirmwareRevision"]?.ToString()?.Trim() ?? "N/A";
                    var iface = obj["InterfaceType"]?.ToString() ?? "N/A";
                    var mediaType = obj["MediaType"]?.ToString() ?? "Unknown";
                    var size = Convert.ToUInt64(obj["Size"] ?? 0);
                    var status = obj["Status"]?.ToString() ?? "Unknown";

                    if (mediaType.Contains("Fixed", StringComparison.OrdinalIgnoreCase))
                        mediaType = DetectMediaType(model, deviceId);

                    diskInfoMap[deviceId] = (model, serial, firmware, iface, mediaType, size, status);
                }
            }

            var smartDataMap = new Dictionary<int, List<SmartAttribute>>();
            try
            {
                var scope = new ManagementScope(@"\\.\root\WMI");
                scope.Connect();

                using var smartSearcher = new ManagementObjectSearcher(scope,
                    new ObjectQuery("SELECT * FROM MSStorageDriver_ATAPISmartData"));

                int diskIndex = 0;
                foreach (var obj in smartSearcher.Get())
                {
                    var attributes = new List<SmartAttribute>();
                    var vendorSpecific = (byte[])obj["VendorSpecific"];

                    if (vendorSpecific != null && vendorSpecific.Length >= 30)
                    {
                        for (int i = 2; i + 12 <= vendorSpecific.Length; i += 12)
                        {
                            int attrId = vendorSpecific[i];
                            if (attrId == 0) continue;

                            int current = vendorSpecific[i + 3];
                            int worst = vendorSpecific[i + 4];

                            long rawValue = 0;
                            for (int j = 0; j < 6 && (i + 5 + j) < vendorSpecific.Length; j++)
                                rawValue |= (long)vendorSpecific[i + 5 + j] << (j * 8);

                            var name = SmartAttributeNames.GetValueOrDefault(attrId, $"Attribute {attrId}");

                            attributes.Add(new SmartAttribute(
                                Id: attrId, Name: name, Current: current, Worst: worst,
                                Threshold: 0, RawValue: rawValue, Status: current > 0 ? "OK" : "Warning"
                            ));
                        }
                    }
                    smartDataMap[diskIndex] = attributes;
                    diskIndex++;
                }

                using var threshSearcher = new ManagementObjectSearcher(scope,
                    new ObjectQuery("SELECT * FROM MSStorageDriver_FailurePredictThresholds"));

                diskIndex = 0;
                foreach (var obj in threshSearcher.Get())
                {
                    var thresholds = (byte[])obj["VendorSpecific"];
                    if (thresholds != null && smartDataMap.ContainsKey(diskIndex))
                    {
                        var threshMap = new Dictionary<int, int>();
                        for (int i = 2; i + 12 <= thresholds.Length; i += 12)
                        {
                            int attrId = thresholds[i];
                            if (attrId == 0) continue;
                            int threshold = thresholds[i + 1];
                            threshMap[attrId] = threshold;
                        }

                        var updated = smartDataMap[diskIndex].Select(a =>
                        {
                            var thresh = threshMap.GetValueOrDefault(a.Id, 0);
                            var status = a.Current > thresh ? "OK" : (a.Current > 0 ? "Warning" : "Critical");
                            return a with { Threshold = thresh, Status = status };
                        }).ToList();

                        smartDataMap[diskIndex] = updated;
                    }
                    diskIndex++;
                }
            }
            catch { /* S.M.A.R.T. not available */ }

            int idx = 0;
            foreach (var kvp in diskInfoMap)
            {
                var (model, serial, firmware, iface, mediaType, size, status) = kvp.Value;
                var attrs = smartDataMap.GetValueOrDefault(idx, []);

                long powerOnHoursRaw = attrs.FirstOrDefault(a => a.Id == 9)?.RawValue ?? -1;
                long powerCycleRaw = attrs.FirstOrDefault(a => a.Id == 12)?.RawValue ?? -1;
                long tempRaw = attrs.FirstOrDefault(a => a.Id == 194)?.RawValue ?? attrs.FirstOrDefault(a => a.Id == 190)?.RawValue ?? -1;
                long reallocRaw = attrs.FirstOrDefault(a => a.Id == 5)?.RawValue ?? -1;
                long pendingRaw = attrs.FirstOrDefault(a => a.Id == 197)?.RawValue ?? -1;
                long uncorrRaw = attrs.FirstOrDefault(a => a.Id == 198)?.RawValue ?? -1;
                long readErrRaw = attrs.FirstOrDefault(a => a.Id == 1)?.RawValue ?? -1;
                long spinRetryRaw = attrs.FirstOrDefault(a => a.Id == 10)?.RawValue ?? -1;

                string health = "✅ Healthy";
                if (reallocRaw > 100 || pendingRaw > 10 || uncorrRaw > 10)
                    health = "❌ Critical — Replace Soon!";
                else if (reallocRaw > 0 || pendingRaw > 0 || uncorrRaw > 0)
                    health = "⚠️ Warning — Monitor Closely";

                string pohStr = powerOnHoursRaw >= 0 ? $"{powerOnHoursRaw:N0} hours" : "N/A (run as Admin)";
                string podStr = powerOnHoursRaw >= 0 ? $"{powerOnHoursRaw / 24.0:F1} days ({powerOnHoursRaw / 8760.0:F2} years)" : "N/A";

                disks.Add(new DiskHealthModel(
                    DeviceId: kvp.Key, Model: model, SerialNumber: serial,
                    FirmwareRevision: firmware, InterfaceType: iface, MediaType: mediaType,
                    SizeFormatted: FormatBytes(size), Status: status,
                    PowerOnHours: pohStr, PowerOnDays: podStr,
                    PowerCycleCount: powerCycleRaw >= 0 ? $"{powerCycleRaw:N0}" : "N/A",
                    Temperature: tempRaw >= 0 ? $"{tempRaw & 0xFF}°C / {(tempRaw & 0xFF) * 9 / 5 + 32}°F" : "N/A",
                    ReallocatedSectors: reallocRaw >= 0 ? $"{reallocRaw}" : "N/A",
                    PendingSectors: pendingRaw >= 0 ? $"{pendingRaw}" : "N/A",
                    UncorrectableSectors: uncorrRaw >= 0 ? $"{uncorrRaw}" : "N/A",
                    ReadErrorRate: readErrRaw >= 0 ? $"{readErrRaw}" : "N/A",
                    SpinRetryCount: spinRetryRaw >= 0 ? $"{spinRetryRaw}" : "N/A",
                    HealthStatus: health, AllAttributes: attrs
                ));
                idx++;
            }
        }
        catch (Exception ex)
        {
            disks.Add(new DiskHealthModel(
                $"Error: {ex.Message}", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "❌ Error", []
            ));
        }
        return disks;
    });

    private static string DetectMediaType(string model, string deviceId)
    {
        try
        {
            var scope = new ManagementScope(@"\\.\root\Microsoft\Windows\Storage");
            scope.Connect();
            using var searcher = new ManagementObjectSearcher(scope,
                new ObjectQuery("SELECT MediaType, SpindleSpeed FROM MSFT_PhysicalDisk"));
            foreach (var obj in searcher.Get())
            {
                var mt = Convert.ToInt32(obj["MediaType"] ?? 0);
                var spindle = Convert.ToInt32(obj["SpindleSpeed"] ?? -1);
                if (mt == 4 || spindle == 0) return "SSD (Solid State)";
                if (mt == 3) return "HDD (Mechanical)";
            }
        }
        catch { }

        var m = model.ToUpperInvariant();
        if (m.Contains("SSD") || m.Contains("NVME") || m.Contains("SOLID")) return "SSD (Solid State)";
        if (m.Contains("HDD") || m.Contains("BARRACUDA") || m.Contains("CAVIAR")) return "HDD (Mechanical)";
        return "Fixed Disk";
    }

    // ═══════════════════════════════════════════════════
    // MONITOR INFO - EDID PARSING
    // ═══════════════════════════════════════════════════

    private static readonly Dictionary<string, string> PnpManufacturers = new()
    {
        ["ACI"] = "ASUS (ASUSTeK)", ["ACR"] = "Acer", ["AOC"] = "AOC",
        ["AUO"] = "AU Optronics", ["BNQ"] = "BenQ", ["BOE"] = "BOE Technology",
        ["CMN"] = "Chi Mei Innolux (Innolux)", ["DEL"] = "Dell",
        ["ENC"] = "Eizo Nanao", ["GSM"] = "LG Electronics (GoldStar)",
        ["HPN"] = "HP (Hewlett-Packard)", ["HWP"] = "HP (Hewlett-Packard)",
        ["IVM"] = "Iiyama", ["LEN"] = "Lenovo", ["LGD"] = "LG Display",
        ["MEI"] = "Panasonic", ["MSI"] = "MSI (Micro-Star International)",
        ["NEC"] = "NEC", ["PHL"] = "Philips", ["SAM"] = "Samsung",
        ["SDC"] = "Samsung Display", ["SEC"] = "Samsung (Seiko Epson)",
        ["SHP"] = "Sharp", ["SNY"] = "Sony", ["VSC"] = "ViewSonic",
        ["FUS"] = "Fujitsu Siemens", ["APP"] = "Apple", ["CPQ"] = "Compaq",
        ["IBM"] = "IBM", ["MED"] = "Medion", ["UNK"] = "Unknown"
    };

    private static readonly (int Width, int Height)[] EstablishedTimings =
    [
        (720, 400), (720, 400), (640, 480), (640, 480), (640, 480), (640, 480), (800, 600), (800, 600),
        (800, 600), (800, 600), (832, 624), (1024, 768), (1024, 768), (1024, 768), (1280, 1024), (1152, 870)
    ];

    public static Task<List<MonitorDetailModel>> GetMonitorDetailsAsync() => Task.Run(() =>
    {
        var monitors = new List<MonitorDetailModel>();
        try
        {
            var scope = new ManagementScope(@"\\.\root\WMI");
            scope.Connect();

            var activeMonitors = new Dictionary<string, bool>();
            var connectionTypes = new Dictionary<string, string>();

            try
            {
                using var activeSearcher = new ManagementObjectSearcher(scope,
                    new ObjectQuery("SELECT InstanceName, Active FROM WmiMonitorConnectionParams"));
                foreach (var obj in activeSearcher.Get())
                {
                    var instance = obj["InstanceName"]?.ToString() ?? "";
                    var active = Convert.ToBoolean(obj["Active"] ?? false);
                    activeMonitors[NormalizeInstance(instance)] = active;
                }
            }
            catch { }

            try
            {
                using var connSearcher = new ManagementObjectSearcher(scope,
                    new ObjectQuery("SELECT InstanceName, VideoOutputTechnology FROM WmiMonitorConnectionParams"));
                foreach (var obj in connSearcher.Get())
                {
                    var instance = NormalizeInstance(obj["InstanceName"]?.ToString() ?? "");
                    var connType = Convert.ToInt32(obj["VideoOutputTechnology"] ?? -1) switch
                    {
                        0 => "VGA (HD15)", 1 => "S-Video", 2 => "Composite",
                        3 => "Component", 4 => "DVI", 5 => "HDMI",
                        6 => "LVDS (Internal)", 9 => "SDI",
                        10 => "DisplayPort (External)", 11 => "DisplayPort (Internal/eDP)",
                        12 => "UDI (External)", 13 => "UDI (Internal)",
                        14 => "SDTV Dongle", 15 => "Miracast",
                        -1 => "Unknown", _ => "Other"
                    };
                    connectionTypes[instance] = connType;
                }
            }
            catch { }

            using var edidSearcher = new ManagementObjectSearcher(scope,
                new ObjectQuery("SELECT InstanceName, WmiMonitorRawEEdidV1Block FROM WmiMonitorRawEEdidV1Block WHERE BlockType=0"));

            foreach (var obj in edidSearcher.Get())
            {
                try
                {
                    var instanceName = obj["InstanceName"]?.ToString() ?? "Unknown";
                    var edidBytes = (byte[])obj["WmiMonitorRawEEdidV1Block"];

                    if (edidBytes == null || edidBytes.Length < 128) continue;

                    int mfgCode = (edidBytes[8] << 8) | edidBytes[9];
                    char c1 = (char)(((mfgCode >> 10) & 0x1F) + 'A' - 1);
                    char c2 = (char)(((mfgCode >> 5) & 0x1F) + 'A' - 1);
                    char c3 = (char)((mfgCode & 0x1F) + 'A' - 1);
                    string mfgStr = $"{c1}{c2}{c3}";
                    string manufacturer = PnpManufacturers.GetValueOrDefault(mfgStr, mfgStr);

                    int productCode = edidBytes[10] | (edidBytes[11] << 8);
                    uint serialNum = (uint)(edidBytes[12] | (edidBytes[13] << 8) | (edidBytes[14] << 16) | (edidBytes[15] << 24));

                    int mfgWeek = edidBytes[16];
                    int mfgYear = edidBytes[17] + 1990;

                    string edidVersion = $"{edidBytes[18]}.{edidBytes[19]}";

                    bool isDigital = (edidBytes[20] & 0x80) != 0;
                    string displayType;
                    string colorDepth = "N/A";

                    if (isDigital && edidBytes[18] >= 1 && edidBytes[19] >= 4)
                    {
                        int depth = (edidBytes[20] >> 4) & 0x07;
                        colorDepth = depth switch
                        {
                            1 => "6-bit", 2 => "8-bit", 3 => "10-bit",
                            4 => "12-bit", 5 => "14-bit", 6 => "16-bit",
                            _ => "Undefined"
                        };
                        int iface2 = edidBytes[20] & 0x0F;
                        displayType = iface2 switch
                        {
                            1 => "Digital (DVI)", 2 => "Digital (HDMI-a)",
                            3 => "Digital (HDMI-b)", 4 => "Digital (MDDI)",
                            5 => "Digital (DisplayPort)", _ => "Digital"
                        };
                    }
                    else if (isDigital) { displayType = "Digital (DFP 1.x)"; }
                    else { displayType = "Analog (VGA)"; }

                    int hSizeCm = edidBytes[21];
                    int vSizeCm = edidBytes[22];
                    string screenSize = (hSizeCm > 0 && vSizeCm > 0)
                        ? $"{hSizeCm} cm x {vSizeCm} cm ({hSizeCm / 2.54:F1}\" x {vSizeCm / 2.54:F1}\")"
                        : "N/A";

                    double diagInches = 0;
                    if (hSizeCm > 0 && vSizeCm > 0)
                        diagInches = Math.Sqrt(hSizeCm * hSizeCm + vSizeCm * vSizeCm) / 2.54;
                    string diagStr = diagInches > 0 ? $"{diagInches:F1}\"" : "N/A";

                    double gamma = (edidBytes[23] + 100) / 100.0;
                    string gammaStr = edidBytes[23] != 0xFF ? $"{gamma:F2}" : "Defined in DI-EXT";

                    var dpmsFeatures = new List<string>();
                    if ((edidBytes[24] & 0x80) != 0) dpmsFeatures.Add("Standby");
                    if ((edidBytes[24] & 0x40) != 0) dpmsFeatures.Add("Suspend");
                    if ((edidBytes[24] & 0x20) != 0) dpmsFeatures.Add("Active-Off");
                    string dpms = dpmsFeatures.Count > 0 ? string.Join(", ", dpmsFeatures) : "None";

                    var resolutions = new List<string>();
                    for (int bit = 0; bit < 16 && bit < EstablishedTimings.Length; bit++)
                    {
                        int byteIdx = 35 + (bit / 8);
                        int bitIdx = 7 - (bit % 8);
                        if (byteIdx < edidBytes.Length && (edidBytes[byteIdx] & (1 << bitIdx)) != 0)
                        {
                            var (w, h) = EstablishedTimings[bit];
                            resolutions.Add($"{w}x{h}");
                        }
                    }

                    string maxRes = "N/A";
                    if (edidBytes.Length >= 72)
                    {
                        int pixelClock = edidBytes[54] | (edidBytes[55] << 8);
                        if (pixelClock > 0)
                        {
                            int hActive = edidBytes[56] | ((edidBytes[58] & 0xF0) << 4);
                            int vActive = edidBytes[59] | ((edidBytes[61] & 0xF0) << 4);
                            if (hActive > 0 && vActive > 0)
                            {
                                maxRes = $"{hActive} x {vActive}";
                                if (!resolutions.Contains($"{hActive}x{vActive}"))
                                    resolutions.Insert(0, $"{hActive}x{vActive} (native)");
                            }
                        }
                    }

                    string monitorName = "Unknown Monitor";
                    string serialString = serialNum > 0 ? $"{serialNum}" : "N/A";

                    for (int block = 0; block < 4; block++)
                    {
                        int offset = 54 + (block * 18);
                        if (offset + 18 > edidBytes.Length) break;

                        if (edidBytes[offset] == 0 && edidBytes[offset + 1] == 0)
                        {
                            int tag = edidBytes[offset + 3];
                            if (tag == 0xFC)
                            {
                                monitorName = System.Text.Encoding.ASCII
                                    .GetString(edidBytes, offset + 5, 13)
                                    .Trim('\n', '\r', ' ', '\0');
                            }
                            else if (tag == 0xFF)
                            {
                                serialString = System.Text.Encoding.ASCII
                                    .GetString(edidBytes, offset + 5, 13)
                                    .Trim('\n', '\r', ' ', '\0');
                            }
                        }
                    }

                    var mfgDate = new DateTime(mfgYear, Math.Max(1, Math.Min(12, (mfgWeek * 12) / 52 + 1)), 1);
                    var age = DateTime.Now - mfgDate;
                    string estimatedUsage = $"Manufactured ~{age.Days / 365.25:F1} years ago";
                    if (age.Days > 0)
                        estimatedUsage += $" (est. {age.Days * 8:N0} hrs if ~8h/day)";

                    string connType = connectionTypes.GetValueOrDefault(NormalizeInstance(instanceName), "Unknown");

                    monitors.Add(new MonitorDetailModel(
                        Name: monitorName, Manufacturer: manufacturer, ManufacturerCode: mfgStr,
                        ProductCode: $"0x{productCode:X4}", SerialNumber: serialString,
                        ManufactureDate: $"Week {mfgWeek}, {mfgYear}", EdidVersion: edidVersion,
                        MaxResolution: maxRes, ScreenSize: screenSize, DiagonalInches: diagStr,
                        DisplayType: displayType, GammaValue: gammaStr, DpmsSupport: dpms,
                        ColorBitDepth: colorDepth, YearOfManufacture: mfgYear.ToString(),
                        WeekOfManufacture: mfgWeek.ToString(), EstimatedUsage: estimatedUsage,
                        SupportedResolutions: resolutions, ConnectionType: connType,
                        DriverStatus: activeMonitors.GetValueOrDefault(NormalizeInstance(instanceName), false) ? "Active" : "Inactive"
                    ));
                }
                catch { continue; }
            }

            if (monitors.Count == 0)
            {
                using var fallback = new ManagementObjectSearcher("SELECT * FROM Win32_DesktopMonitor");
                foreach (var obj in fallback.Get())
                {
                    monitors.Add(new MonitorDetailModel(
                        Name: obj["Name"]?.ToString() ?? "Unknown",
                        Manufacturer: obj["MonitorManufacturer"]?.ToString() ?? "N/A",
                        ManufacturerCode: "", ProductCode: "", SerialNumber: "N/A",
                        ManufactureDate: "N/A", EdidVersion: "N/A",
                        MaxResolution: $"{obj["ScreenWidth"]}x{obj["ScreenHeight"]}",
                        ScreenSize: "N/A", DiagonalInches: "N/A",
                        DisplayType: obj["MonitorType"]?.ToString() ?? "N/A",
                        GammaValue: "N/A", DpmsSupport: "N/A", ColorBitDepth: "N/A",
                        YearOfManufacture: "N/A", WeekOfManufacture: "N/A",
                        EstimatedUsage: "N/A", SupportedResolutions: [],
                        ConnectionType: "N/A", DriverStatus: "N/A"
                    ));
                }
            }
        }
        catch (Exception ex)
        {
            monitors.Add(new MonitorDetailModel(
                $"Error: {ex.Message}", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", [], "", ""
            ));
        }
        return monitors;
    });

    private static string NormalizeInstance(string instance) =>
        instance.Contains('_') ? instance[..instance.LastIndexOf('_')] : instance;

    // ═══════════════════════════════════════════════════
    // NATIVE MEMORY
    // ═══════════════════════════════════════════════════

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
        0 => "x86", 5 => "ARM", 9 => "x64", 12 => "ARM64",
        _ => $"Unknown ({arch})"
    };
}
