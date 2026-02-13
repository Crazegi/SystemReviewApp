using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using SystemReview.Helpers;
using SystemReview.Models;
using SystemReview.Services;

namespace SystemReview.ViewModels;

public class SystemSpecsViewModel : ObservableObject
{
    private string _statusMessage = "Ready. Click Refresh to load system specs.";
    private bool _isLoading;
    private int _ramUsagePercent;

    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public int RamUsagePercent { get => _ramUsagePercent; set => SetProperty(ref _ramUsagePercent, value); }

    public ObservableCollection<CpuInfo> Cpus { get; } = [];
    public ObservableCollection<GpuInfo> Gpus { get; } = [];
    public ObservableCollection<DriveInfoModel> Drives { get; } = [];
    public ObservableCollection<KeyValuePair<string, string>> RamDetails { get; } = [];
    public ObservableCollection<KeyValuePair<string, string>> OsDetails { get; } = [];
    public ObservableCollection<KeyValuePair<string, string>> MotherboardDetails { get; } = [];
    public ObservableCollection<KeyValuePair<string, string>> DisplayDetails { get; } = [];
    public ObservableCollection<KeyValuePair<string, string>> BatteryDetails { get; } = [];
    public ObservableCollection<KeyValuePair<string, string>> InstalledSoftware { get; } = [];

    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand ExportJsonCommand { get; }
    public AsyncRelayCommand ExportTextCommand { get; }

    public SystemSpecsViewModel()
    {
        RefreshCommand = new AsyncRelayCommand(async _ => await LoadAllAsync());
        ExportJsonCommand = new AsyncRelayCommand(async _ => await ExportAsync(true));
        ExportTextCommand = new AsyncRelayCommand(async _ => await ExportAsync(false));
    }

    public async Task LoadAllAsync()
    {
        IsLoading = true;
        StatusMessage = "Loading system specifications...";

        try
        {
            var cpuTask = WmiService.GetCpuInfoAsync();
            var ramTask = WmiService.GetRamInfoAsync();
            var gpuTask = WmiService.GetGpuInfoAsync();
            var driveTask = WmiService.GetDriveInfoAsync();
            var mbTask = WmiService.GetMotherboardInfoAsync();
            var osTask = WmiService.GetOsInfoAsync();
            var displayTask = WmiService.GetDisplayInfoAsync();
            var batteryTask = WmiService.GetBatteryInfoAsync();
            var softwareTask = WmiService.GetInstalledSoftwareAsync();

            await Task.WhenAll(cpuTask, ramTask, gpuTask, driveTask, mbTask, osTask, displayTask, batteryTask, softwareTask);

            Cpus.Clear();
            foreach (var c in cpuTask.Result) Cpus.Add(c);

            RamDetails.Clear();
            var (total, avail, pct) = ramTask.Result;
            RamUsagePercent = pct;
            RamDetails.Add(new("Total RAM", total));
            RamDetails.Add(new("Available RAM", avail));
            RamDetails.Add(new("Usage", $"{pct}%"));

            Gpus.Clear();
            foreach (var g in gpuTask.Result) Gpus.Add(g);

            Drives.Clear();
            foreach (var d in driveTask.Result) Drives.Add(d);

            MotherboardDetails.Clear();
            foreach (var kv in mbTask.Result) MotherboardDetails.Add(kv);

            OsDetails.Clear();
            foreach (var kv in osTask.Result) OsDetails.Add(kv);

            DisplayDetails.Clear();
            foreach (var disp in displayTask.Result)
                foreach (var kv in disp) DisplayDetails.Add(kv);

            BatteryDetails.Clear();
            foreach (var kv in batteryTask.Result) BatteryDetails.Add(kv);

            InstalledSoftware.Clear();
            foreach (var kv in softwareTask.Result) InstalledSoftware.Add(kv);

            StatusMessage = $"✅ Loaded at {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ExportAsync(bool asJson)
    {
        try
        {
            var data = new Dictionary<string, object>
            {
                ["CPU"] = Cpus.ToList(),
                ["RAM"] = RamDetails.ToDictionary(kv => kv.Key, kv => kv.Value),
                ["GPU"] = Gpus.ToList(),
                ["Drives"] = Drives.ToList(),
                ["Motherboard"] = MotherboardDetails.ToDictionary(kv => kv.Key, kv => kv.Value),
                ["OS"] = OsDetails.ToDictionary(kv => kv.Key, kv => kv.Value),
                ["Display"] = DisplayDetails.ToDictionary(kv => kv.Key, kv => kv.Value),
                ["Battery"] = BatteryDetails.ToDictionary(kv => kv.Key, kv => kv.Value),
                ["InstalledSoftware"] = InstalledSoftware.ToDictionary(kv => kv.Key, kv => kv.Value)
            };

            var filename = $"SystemReview_Specs_{DateTime.Now:yyyyMMdd_HHmmss}";
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (asJson)
            {
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                var path = Path.Combine(desktop, $"{filename}.json");
                await File.WriteAllTextAsync(path, json);
                StatusMessage = $"✅ Exported to {path}";
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine("╔══════════════════════════════════════════════╗");
                sb.AppendLine("║       SYSTEM REVIEW — SPECIFICATIONS         ║");
                sb.AppendLine($"║  Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}               ║");
                sb.AppendLine("╚══════════════════════════════════════════════╝");
                sb.AppendLine();

                sb.AppendLine("── CPU ─────────────────────────────");
                foreach (var c in Cpus)
                {
                    sb.AppendLine($"  Name:           {c.Name}");
                    sb.AppendLine($"  Cores:          {c.Cores}");
                    sb.AppendLine($"  Logical Procs:  {c.LogicalProcessors}");
                    sb.AppendLine($"  Max Speed:      {c.MaxSpeed}");
                    sb.AppendLine($"  Architecture:   {c.Architecture}");
                }

                sb.AppendLine("\n── RAM ─────────────────────────────");
                foreach (var kv in RamDetails) sb.AppendLine($"  {kv.Key,-18} {kv.Value}");

                sb.AppendLine("\n── GPU ─────────────────────────────");
                foreach (var g in Gpus)
                {
                    sb.AppendLine($"  Name:           {g.Name}");
                    sb.AppendLine($"  Driver:         {g.DriverVersion}");
                    sb.AppendLine($"  VRAM:           {g.AdapterRam}");
                    sb.AppendLine($"  Processor:      {g.VideoProcessor}");
                }

                sb.AppendLine("\n── Storage ─────────────────────────");
                foreach (var d in Drives)
                    sb.AppendLine($"  {d.Name} [{d.Label}] {d.FileSystem} | Total: {d.TotalSize} | Free: {d.FreeSpace} | Used: {d.UsedPercent}%");

                sb.AppendLine("\n── Motherboard / BIOS ──────────────");
                foreach (var kv in MotherboardDetails) sb.AppendLine($"  {kv.Key,-18} {kv.Value}");

                sb.AppendLine("\n── Operating System ────────────────");
                foreach (var kv in OsDetails) sb.AppendLine($"  {kv.Key,-18} {kv.Value}");

                sb.AppendLine("\n── Display ─────────────────────────");
                foreach (var kv in DisplayDetails) sb.AppendLine($"  {kv.Key,-18} {kv.Value}");

                sb.AppendLine("\n── Battery ─────────────────────────");
                foreach (var kv in BatteryDetails) sb.AppendLine($"  {kv.Key,-18} {kv.Value}");

                sb.AppendLine("\n── Installed Software (Top 20) ─────");
                foreach (var kv in InstalledSoftware) sb.AppendLine($"  {kv.Key,-40} {kv.Value}");

                var path = Path.Combine(desktop, $"{filename}.txt");
                await File.WriteAllTextAsync(path, sb.ToString());
                StatusMessage = $"✅ Exported to {path}";
            }
        }
        catch (Exception ex) { StatusMessage = $"❌ Export error: {ex.Message}"; }
    }
}