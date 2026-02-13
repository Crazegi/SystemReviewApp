<div align="center">

# 🖥️ SystemReview

### A Modern Windows Desktop App for System Diagnostics & Network Analysis

[![WinUI 3](https://img.shields.io/badge/WinUI-3-blue?logo=microsoft)](https://microsoft.github.io/microsoft-ui-xaml/)
[![.NET](https://img.shields.io/badge/.NET-10.0-purple?logo=dotnet)](https://dotnet.microsoft.com/)
[![Windows App SDK](https://img.shields.io/badge/Windows%20App%20SDK-1.6-0078D4?logo=windows)](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%2F11-0078D4?logo=windows)](https://www.microsoft.com/windows)

**SystemReview** is a feature-rich, production-ready WinUI 3 desktop application that provides comprehensive system hardware inspection, real-time networking diagnostics, performance monitoring, and exportable reports — all wrapped in a modern Fluent Design UI.

[Features](#-features) · [Screenshots](#-screenshots) · [Installation](#-installation) · [Building from Source](#-building-from-source) · [Usage Guide](#-usage-guide) · [Architecture](#-architecture) · [Contributing](#-contributing)

---

</div>

## 📋 Table of Contents

- [Features](#-features)
- [Screenshots](#-screenshots)
- [System Requirements](#-system-requirements)
- [Installation](#-installation)
- [Building from Source](#-building-from-source)
- [Usage Guide](#-usage-guide)
  - [System Specs Tab](#-system-specs-tab)
  - [Networking Tab](#-networking-tab)
  - [Diagnostics Tab](#-diagnostics-tab)
  - [Exporting Data](#-exporting-data)
- [Architecture](#-architecture)
  - [Project Structure](#project-structure)
  - [Design Patterns](#design-patterns)
  - [Technology Stack](#technology-stack)
  - [API Reference](#apis--data-sources)
- [Configuration](#-configuration)
- [Troubleshooting](#-troubleshooting)
- [Known Limitations](#-known-limitations)
- [Roadmap](#-roadmap)
- [Contributing](#-contributing)
- [License](#-license)
- [Acknowledgments](#-acknowledgments)

---

## ✨ Features

### 🖥️ System Specifications
| Feature | Details |
|---------|---------|
| **CPU Information** | Processor name, physical cores, logical processors, max clock speed, architecture (x86/x64/ARM64) |
| **Memory (RAM)** | Total installed RAM, available memory, usage percentage with visual progress bar |
| **Graphics (GPU)** | GPU name, driver version, dedicated VRAM, video processor details; supports multiple GPUs |
| **Storage Drives** | All mounted drives with volume label, file system type, total/free space, visual usage bar |
| **Motherboard & BIOS** | Manufacturer, product name, serial number, BIOS version, BIOS manufacturer, release date |
| **Operating System** | OS name, version, build number, architecture, install date, uptime, last boot time, machine/user name |
| **Display** | Monitor names, resolutions (width × height), refresh rates, monitor types |
| **Battery** | Battery name, charge percentage, estimated runtime, chemistry type, design voltage (laptops) |
| **Installed Software** | Top 20 installed programs with version numbers |

### 🌐 Networking Tools
| Feature | Details |
|---------|---------|
| **IP Configuration** | Full IP config for all active adapters — IPv4, IPv6, gateway, DNS suffix, link speed |
| **Network Adapters** | Complete adapter list with name, description, status, speed, MAC address, adapter type |
| **Ping Test** | Async ICMP ping with 4 sequential pings, round-trip time, TTL, timestamps |
| **Traceroute** | Real-time traceroute using Windows `tracert` — hop-by-hop with live UI updates |
| **Port Scanner** | Lists all active TCP connections, TCP listeners, and UDP listeners with addresses, ports, and states |
| **DNS Lookup** | Resolves hostnames to IP addresses showing address family (IPv4/IPv6) |
| **Network Statistics** | Bytes sent/received, packets sent/received, error counts, link speed per adapter |
| **Real-time Monitoring** | Automatic detection of network availability changes and address changes with auto-refresh |

### 🩺 Diagnostics
| Feature | Details |
|---------|---------|
| **Performance Counters** | Live CPU usage %, available RAM (MB), disk activity %, app memory, thread count, process count |
| **Real-time Monitor** | Toggle-able 2-second refresh loop for continuous performance tracking |
| **Event Logs** | Last 10 errors/warnings from the Windows System event log with source, level, time, and message |
| **Network Services** | Status of 17 critical network services (DHCP, DNS Cache, WLAN, Netlogon, etc.) with start type |
| **Full Diagnostics** | One-click diagnostic that tests: Internet connectivity, DNS resolution, firewall status, network adapters, and gateway reachability |

### 📤 Export Capabilities
| Format | Details |
|--------|---------|
| **JSON** | Structured, indented JSON with all collected data — ideal for programmatic consumption |
| **Plain Text** | Human-readable formatted report with ASCII borders and aligned columns |
| **Per-Tab Export** | Each tab (Specs, Networking, Diagnostics) has independent export buttons |
| **Auto-naming** | Files are automatically named with timestamps: `SystemReview_Specs_20260213_143022.json` |
| **Desktop Output** | All exports save directly to your Desktop for easy access |

---

## 📸 Screenshots

> After launching, click **🔄 Refresh All** on each tab to populate data.

### System Specs Tab
The main overview tab showing hardware, OS, and software information in organized cards with visual indicators:

```
┌──────────────────────────────────────────────────┐
│  [🔄 Refresh All]  [📋 Export JSON]  [📝 Export] │
│                                                    │
│  ┌─ 🖥️ Processor (CPU) ────────────────────┐     │
│  │  Name:          AMD Ryzen 9 7950X        │     │
│  │  Cores:         16                        │     │
│  │  Logical Procs: 32                        │     │
│  │  Max Speed:     5881 MHz                  │     │
│  │  Architecture:  x64                       │     │
│  └──────────────────────────────────────────┘     │
│                                                    │
│  ┌─ 🧠 Memory (RAM) ──────────────────────┐      │
│  │  [████████████░░░░░░░░] 62%             │      │
│  │  Total RAM:     32.00 GB                │      │
│  │  Available RAM: 12.14 GB                │      │
│  └─────────────────────────────────────────┘      │
│                                                    │
│  ┌─ 💾 Storage Drives ────────────────────┐       │
│  │  C:\ [System] NTFS                     │       │
│  │  [███████████��██░░░░░] 71%             │       │
│  │  Total: 953.87 GB  Free: 276.42 GB     │       │
│  └─────────────────────────────────────────┘      │
└──────────────────────────────────────────────────┘
```

### Networking Tab
Full network diagnostics with interactive tools:

```
┌──────────────────────────────────────────────────┐
│  [🔄 Refresh]  [📋 JSON]  [📝 Text]              │
│                                                    │
│  🌐 IP Configuration                              │
│  ┌─────────────────────────────────────────┐      │
│  │  Wi-Fi - IPv4:    192.168.1.42          │      │
│  │  Wi-Fi - Gateway: 192.168.1.1           │      │
│  │  Wi-Fi - Speed:   867 Mbps              │      │
│  └─────────────────────────────────────────┘      │
│                                                    │
│  📡 Ping Test                                      │
│  [ 8.8.8.8          ] [Ping]                       │
│  14:30:22.451  8.8.8.8 → Success  12ms  TTL:118  │
│  14:30:22.963  8.8.8.8 → Success  11ms  TTL:118  │
│                                                    │
│  🛤️ Traceroute                                     │
│  [ 8.8.8.8          ] [Trace]                      │
│  Hop 1: 192.168.1.1     1ms    OK                 │
│  Hop 2: 10.0.0.1        5ms    OK                 │
│  Hop 3: 172.16.0.1      12ms   OK                 │
└──────────────────────────────────────────────────┘
```

### Diagnostics Tab
Performance monitoring and system health checks:

```
┌──────────────────────────────────────────────────┐
│  [🔄 Refresh] [⏱️ Start Monitor] [🩺 Diagnose]   │
│                                                    │
│  📈 Performance Counters                           │
│  ┌─────────────────────────────────────────┐      │
│  │  CPU Usage:        23.4%                │      │
│  │  Available RAM:    12045 MB             │      │
│  │  Disk Activity:    5.2%                 │      │
│  │  System Processes: 312                  │      │
│  └─────────────────────────────────────────┘      │
│                                                    │
│  🩺 Diagnostic Log                                 │
│  === System Diagnostics Report ===                 │
│  [1/5] Testing Internet Connectivity...            │
│    ✅ Internet: OK (12ms)                          │
│  [2/5] Testing DNS Resolution...                   │
│    ✅ DNS: OK (resolved to 13.107.246.13)          │
│  [3/5] Checking Firewall Status...                 │
│    🔥 State: ON                                    │
│  [4/5] Checking Network Adapters...                │
│    🔌 Wi-Fi: Wireless80211 - 867 Mbps             │
│  [5/5] Testing Gateway...                          │
│    ✅ Gateway 192.168.1.1: OK (1ms)               │
│  === Diagnostics Complete ===                      ��
└──────────────────────────────────────────────────┘
```

---

## 💻 System Requirements

| Requirement | Minimum |
|-------------|---------|
| **OS** | Windows 10 version 1903 (build 19041) or later |
| **Architecture** | x64 (AMD64 / Intel 64-bit) |
| **.NET Runtime** | .NET 10.0 (bundled in self-contained builds) |
| **Windows App SDK** | 1.6 Runtime (bundled in self-contained builds) |
| **RAM** | 4 GB minimum, 8 GB recommended |
| **Disk Space** | ~200 MB for self-contained build |
| **Permissions** | Administrator recommended (for WMI, Event Logs, Performance Counters) |

---

## 📦 Installation

### Option 1: Download Pre-built Release (Recommended)

1. Go to the [Releases](../../releases) page
2. Download `SystemReview-Portable-x64.zip`
3. Extract to any folder
4. Run `SystemReview.exe`

> No additional installations needed — the self-contained build includes everything.

### Option 2: Install Runtime + Lightweight Build

If you prefer a smaller download:

1. Install the Windows App SDK 1.6 Runtime:
   ```powershell
   winget install -e --id Microsoft.WindowsAppRuntime.1.6
   ```
2. Install .NET 10 Runtime:
   ```powershell
   winget install -e --id Microsoft.DotNet.DesktopRuntime.10
   ```
3. Download the framework-dependent build from [Releases](../../releases)
4. Extract and run `SystemReview.exe`

---

## 🔨 Building from Source

### Prerequisites

| Tool | Install Command |
|------|----------------|
| **.NET 10 SDK** | `winget install -e --id Microsoft.DotNet.SDK.10` |
| **Windows App SDK 1.6 Runtime** | `winget install -e --id Microsoft.WindowsAppRuntime.1.6` |
| **Git** | `winget install -e --id Git.Git` |

Verify your setup:
```powershell
dotnet --version   # Should show 10.x.x
```

### Clone & Build

```powershell
# Clone the repository
git clone https://github.com/YOUR_USERNAME/SystemReview.git
cd SystemReview

# Restore NuGet packages
dotnet restore SystemReview.csproj

# Build in Debug mode
dotnet build SystemReview.csproj

# Run the app
.\bin\Debug\net10.0-windows10.0.22621.0\win-x64\SystemReview.exe
```

### Build for Release (Self-Contained)

This creates a fully portable build with no external dependencies:

```powershell
dotnet publish SystemReview.csproj -c Release -r win-x64 --self-contained true
```

The output will be in:
```
bin\Release\net10.0-windows10.0.22621.0\win-x64\publish\
```

### Create a Distributable ZIP

```powershell
Compress-Archive `
  -Path ".\bin\Release\net10.0-windows10.0.22621.0\win-x64\publish\*" `
  -DestinationPath "$HOME\Desktop\SystemReview-Portable-x64.zip"
```

### Build for ARM64

```powershell
# Edit .csproj: change <RuntimeIdentifier>win-x64</RuntimeIdentifier>
# to <RuntimeIdentifier>win-arm64</RuntimeIdentifier>

dotnet publish SystemReview.csproj -c Release -r win-arm64 --self-contained true
```

---

## 📖 Usage Guide

### 🖥️ System Specs Tab

This is the primary hardware and software inventory tab.

1. Click **🔄 Refresh All** to scan your system
2. All sections load in parallel for speed
3. Scroll down to see all cards:

| Card | What It Shows |
|------|--------------|
| **Processor** | CPU model, core count, thread count, clock speed, instruction set architecture |
| **Memory** | Total/available RAM with a color-coded usage bar |
| **Graphics** | Each GPU listed separately with driver and VRAM info |
| **Storage** | Per-drive cards with usage bars — quickly spot drives running low |
| **Operating System** | Windows version, build, uptime, install date, current user |
| **Motherboard / BIOS** | Board manufacturer/model, BIOS version and vendor |
| **Display** | Connected monitors with resolution and refresh rate |
| **Battery** | Charge level, estimated runtime, battery chemistry (laptops only) |
| **Installed Software** | Top 20 installed programs sorted alphabetically with versions |

### 🌐 Networking Tab

Interactive network analysis tools with real-time updates.

1. Click **🔄 Refresh** to load IP configuration, adapters, and network stats
2. The app automatically detects network changes and refreshes

#### Available Tools:

**Ping Test**
- Enter any IP address or hostname (default: `8.8.8.8`)
- Click **Ping** — sends 4 ICMP echo requests
- Shows round-trip time, TTL, and timestamps for each reply

**Traceroute**
- Enter a destination host
- Click **Trace** — hops appear in real-time as `tracert` runs
- Each hop shows address, latency, and status (OK/Timeout)

**Port Scanner**
- Click **Scan Ports**
- Lists all active TCP connections (with state: ESTABLISHED, TIME_WAIT, etc.)
- Lists all TCP and UDP listeners
- Shows local/remote addresses and ports

**DNS Lookup**
- Enter a hostname (default: `www.microsoft.com`)
- Click **Lookup**
- Shows all resolved IP addresses with address family (IPv4/IPv6)

**Network Statistics**
- Automatically loaded on refresh
- Shows per-adapter: bytes sent/received, packets, errors, link speed

### 🩺 Diagnostics Tab

System health monitoring and automated diagnostics.

**Performance Counters** — Click **🔄 Refresh** to get a snapshot:
- CPU usage percentage
- Available RAM in MB
- Disk activity percentage
- Current app memory usage
- System thread and process counts

**Real-time Monitor** — Click **⏱️ Start Monitor**:
- Performance counters refresh every 2 seconds
- Click **⏹️ Stop Monitor** to pause
- Useful for watching resource usage during tasks

**Run Diagnostics** — Click **🩺 Run Diagnostics**:
Performs a 5-step automated health check:
1. **Internet Connectivity** — Pings 8.8.8.8 (Google DNS)
2. **DNS Resolution** — Resolves www.microsoft.com
3. **Firewall Status** — Checks all Windows Firewall profiles
4. **Network Adapters** — Lists active adapters with speeds
5. **Gateway Test** — Pings all detected gateways

Results appear in real-time in the diagnostic log with ✅/❌ indicators.

**Event Logs** — Shows the 10 most recent errors and warnings from the Windows System log.

**Network Services** — Status of critical Windows networking services:
- DHCP Client, DNS Client, Netlogon, WLAN AutoConfig
- Network Location Awareness, Network Connections
- IP Helper, Windows Time, and more

### 📤 Exporting Data

Every tab has **Export JSON** and **Export Text** buttons:

| Export Type | Format | Best For |
|-------------|--------|----------|
| **JSON** | Structured, indented JSON | Automation, scripts, APIs, programmatic analysis |
| **Text** | Formatted plain text with ASCII art | Sharing, printing, quick reading, documentation |

**Export location:** Files are saved to your **Desktop** with automatic timestamped names.

Example filenames:
```
SystemReview_Specs_20260213_143022.json
SystemReview_Network_20260213_143145.txt
SystemReview_Diagnostics_20260213_143312.json
```

---

## 🏗️ Architecture

### Project Structure

```
SystemReview/
│
├── SystemReview.csproj          # Project configuration, NuGet references
├── app.manifest                 # (optional) Application manifest for elevation
│
├── App.xaml                     # Application entry point (XAML resources)
├── App.xaml.cs                  # Application startup logic
│
├── MainWindow.xaml              # Main window with NavigationView (tab bar)
├── MainWindow.xaml.cs           # Navigation logic between tabs
│
├── Helpers/
│   ├── ObservableObject.cs      # INotifyPropertyChanged base class
│   └── RelayCommand.cs          # ICommand implementations (sync + async)
│
├── Models/
│   └── AllModels.cs             # All data records (CpuInfo, GpuInfo, etc.)
│
├── Services/
│   ├── WmiService.cs            # WMI queries (CPU, RAM, GPU, drives, etc.)
│   ├── NetworkService.cs        # Network tools (ping, traceroute, DNS, ports)
│   └── DiagnosticsService.cs    # Perf counters, event logs, services, diagnostics
│
├── ViewModels/
│   ├── SystemSpecsViewModel.cs  # Logic + data for System Specs tab
│   ├── NetworkingViewModel.cs   # Logic + data for Networking tab
│   └── DiagnosticsViewModel.cs  # Logic + data for Diagnostics tab
│
└── Views/
    ├── SystemSpecsPage.xaml     # UI for System Specs tab
    ├── SystemSpecsPage.xaml.cs  # Code-behind for System Specs
    ├── NetworkingPage.xaml      # UI for Networking tab
    ├── NetworkingPage.xaml.cs   # Code-behind for Networking
    ├── DiagnosticsPage.xaml     # UI for Diagnostics tab
    └── DiagnosticsPage.xaml.cs  # Code-behind for Diagnostics
```

### Design Patterns

| Pattern | Implementation |
|---------|---------------|
| **MVVM** | ViewModels contain all logic; Views only handle UI binding and events |
| **Observable Pattern** | `ObservableObject` base class with `INotifyPropertyChanged` for reactive UI |
| **Command Pattern** | `RelayCommand` and `AsyncRelayCommand` for button bindings |
| **Service Layer** | Static service classes (`WmiService`, `NetworkService`, `DiagnosticsService`) encapsulate all data access |
| **Async/Await** | All I/O-bound and CPU-bound operations are async with `Task.Run` for WMI queries |
| **Parallel Loading** | `Task.WhenAll` loads all sections simultaneously for fast refresh |

### Technology Stack

| Component | Technology |
|-----------|-----------|
| **UI Framework** | WinUI 3 (Microsoft.UI.Xaml) |
| **App Platform** | Windows App SDK 1.6 |
| **Language** | C# 12 |
| **Runtime** | .NET 10.0 |
| **Hardware Queries** | WMI via `System.Management` (Win32_Processor, Win32_VideoController, etc.) |
| **Memory Info** | Native P/Invoke `GlobalMemoryStatusEx` |
| **Networking** | `System.Net.NetworkInformation`, `System.Net.Dns`, `System.Diagnostics.Process` |
| **Performance** | `System.Diagnostics.PerformanceCounter` |
| **Event Logs** | `System.Diagnostics.EventLog` |
| **Services** | `System.ServiceProcess.ServiceController` |
| **Serialization** | `System.Text.Json` |

### APIs & Data Sources

#### WMI Classes Used

| WMI Class | Data Retrieved |
|-----------|---------------|
| `Win32_Processor` | CPU name, cores, logical processors, speed, architecture |
| `Win32_VideoController` | GPU name, driver, VRAM, resolution, refresh rate |
| `Win32_BaseBoard` | Motherboard manufacturer, product, serial |
| `Win32_BIOS` | BIOS version, manufacturer, release date |
| `Win32_OperatingSystem` | OS name, version, build, uptime, install date |
| `Win32_DesktopMonitor` | Monitor names, resolutions, types |
| `Win32_Battery` | Charge level, runtime, chemistry, voltage |
| `Win32_Product` | Installed software names and versions |

#### .NET APIs Used

| API | Purpose |
|-----|---------|
| `GlobalMemoryStatusEx` (P/Invoke) | Total/available physical memory |
| `DriveInfo.GetDrives()` | Storage drive enumeration |
| `NetworkInterface.GetAllNetworkInterfaces()` | Network adapter details |
| `IPGlobalProperties` | Active TCP/UDP connections and listeners |
| `Ping.SendPingAsync()` | ICMP echo requests |
| `Dns.GetHostAddressesAsync()` | DNS resolution |
| `Process.Start("tracert")` | Traceroute execution |
| `PerformanceCounter` | CPU%, available memory, disk activity |
| `EventLog` | Windows System event log entries |
| `ServiceController` | Windows service status |

---

## ⚙️ Configuration

### Running as Administrator

Some features provide richer data when run with elevated privileges:

| Feature | Without Admin | With Admin |
|---------|:------------:|:----------:|
| CPU, GPU, Drives | ✅ Full | ✅ Full |
| RAM | ✅ Full | ✅ Full |
| Network IP/Adapters | ✅ Full | ✅ Full |
| Ping, DNS, Traceroute | ✅ Full | ✅ Full |
| Performance Counters | ⚠️ May be limited | ✅ Full |
| Event Logs | ⚠️ May be limited | ✅ Full |
| Installed Software (WMI) | ⚠️ May be slow | ✅ Full |
| Firewall Status | ❌ Access denied | ✅ Full |

To run as administrator:
```powershell
# Right-click PowerShell → Run as Administrator, then:
cd C:\path\to\SystemReview
.\SystemReview.exe
```

### Customizing Network Services List

The monitored network services are defined in `Services/DiagnosticsService.cs`. You can modify the `networkServices` array:

```csharp
string[] networkServices = [
    "Dhcp", "Dnscache", "LanmanServer", "LanmanWorkstation",
    "Netlogon", "NlaSvc", "WlanSvc", "Netman", "RemoteAccess",
    "SharedAccess", "iphlpsvc", "Winmgmt", "W32Time",
    "WinHttpAutoProxySvc", "dot3svc", "SSDPSRV", "upnphost"
];
```

---

## ❓ Troubleshooting

### Common Issues

<details>
<summary><strong>"Side-by-side configuration is incorrect"</strong></summary>

The Windows App SDK Runtime is not installed or doesn't match the build version.

**Fix:**
```powershell
winget install -e --id Microsoft.WindowsAppRuntime.1.6
```
Or use a self-contained build:
```powershell
dotnet publish SystemReview.csproj -c Release -r win-x64 --self-contained true
```
</details>

<details>
<summary><strong>"Program does not contain a static 'Main' method"</strong></summary>

The XAML files (`App.xaml`, `MainWindow.xaml`, Pages) are missing or misconfigured. Ensure all `.xaml` and `.xaml.cs` files exist and have matching `x:Class` attributes.
</details>

<details>
<summary><strong>"Access Denied" errors for WMI or Event Logs</strong></summary>

Run the application as Administrator:
```powershell
Start-Process .\SystemReview.exe -Verb RunAs
```
</details>

<details>
<summary><strong>Installed Software section is slow or empty</strong></summary>

The `Win32_Product` WMI class is notoriously slow (it validates every MSI package). This can take 30-60 seconds on systems with many installed programs. The app handles this gracefully — other sections load independently.
</details>

<details>
<summary><strong>Battery section shows "No battery detected"</strong></summary>

This is normal for desktop PCs. The battery card only shows data on laptops/tablets with a battery.
</details>

<details>
<summary><strong>Performance counters show errors</strong></summary>

Performance counters may need to be rebuilt:
```powershell
# Run as Administrator
lodctr /R
```
Then restart the app.
</details>

<details>
<summary><strong>NETSDK1206 warning during build</strong></summary>

This warning appears when using .NET 10 SDK. It's harmless and doesn't affect functionality. Adding `<WindowsAppSDKSelfContained>true</WindowsAppSDKSelfContained>` to the `.csproj` resolves the underlying native DLL copying issue.
</details>

<details>
<summary><strong>XAML editor shows red squiggles in VS Code</strong></summary>

This is normal — the XAML IntelliSense in VS Code doesn't fully support WinUI 3 data binding expressions. If `dotnet build` succeeds, the app is fine. These are design-time warnings only.
</details>

---

## ⚠️ Known Limitations

| Limitation | Reason | Workaround |
|-----------|--------|------------|
| **WiFi network scanning** | WinUI 3 unpackaged apps cannot access `WiFiAdapter.FindAllAdaptersAsync()` without package identity | Use `netsh wlan show networks` manually |
| **Installed Software is slow** | `Win32_Product` WMI class validates each MSI entry | Wait ~30s; other sections load independently |
| **Single architecture build** | Self-contained builds target one architecture | Build separately for x64 and ARM64 |
| **No auto-update** | No built-in update mechanism | Check GitHub Releases periodically |
| **Windows only** | WMI, PerformanceCounter, and WinUI 3 are Windows-specific | No cross-platform support planned |
| **Export to Desktop only** | Hardcoded export path | Modify `ExportAsync` methods for custom paths |

---

## 🗺️ Roadmap

### Planned Features

- [ ] **Dark/Light theme toggle** — Manual theme switching with system theme detection
- [ ] **Hardware temperature monitoring** — CPU/GPU temps via OpenHardwareMonitor lib
- [ ] **Network speed test** — Built-in download/upload speed measurement
- [ ] **WiFi signal strength** — Wireless network signal quality display
- [ ] **Process manager** — Top resource-consuming processes list
- [ ] **Startup programs** — List and manage auto-start applications
- [ ] **System health score** — Aggregate score based on all diagnostics
- [ ] **Custom export paths** — File picker dialog for export location
- [ ] **Scheduled reports** — Automatic periodic system reports
- [ ] **Historical data** — Track system metrics over time with charts
- [ ] **Localization** — Multi-language support
- [ ] **MSIX packaging** — Microsoft Store distribution

---

## 🤝 Contributing

Contributions are welcome! Here's how to get started:

### Setting Up Development Environment

1. Install prerequisites:
   ```powershell
   winget install -e --id Microsoft.DotNet.SDK.10
   winget install -e --id Microsoft.WindowsAppRuntime.1.6
   winget install -e --id Microsoft.VisualStudioCode
   ```

2. Install VS Code extensions:
   - **C# Dev Kit** (Microsoft)
   - **XAML** (optional, for syntax highlighting)

3. Fork and clone:
   ```powershell
   git clone https://github.com/YOUR_USERNAME/SystemReview.git
   cd SystemReview
   dotnet restore SystemReview.csproj
   dotnet build SystemReview.csproj
   ```

### Contributing Guidelines

1. **Fork** the repository
2. **Create a feature branch:** `git checkout -b feature/amazing-feature`
3. **Make your changes** following the existing code style:
   - MVVM pattern (logic in ViewModels, not code-behind)
   - Async/await for all I/O operations
   - Try-catch with user-friendly error messages
   - XML documentation for public APIs
4. **Test** your changes thoroughly
5. **Commit:** `git commit -m "feat: add amazing feature"`
6. **Push:** `git push origin feature/amazing-feature`
7. **Open a Pull Request** with a clear description

### Commit Convention

| Prefix | Usage |
|--------|-------|
| `feat:` | New feature |
| `fix:` | Bug fix |
| `docs:` | Documentation changes |
| `style:` | UI/formatting changes (no logic change) |
| `refactor:` | Code refactoring |
| `perf:` | Performance improvement |
| `test:` | Adding or fixing tests |

---

## 📄 License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

```
MIT License

Copyright (c) 2026 SystemReview Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
```

---

## 🙏 Acknowledgments

- **[Microsoft WinUI 3](https://github.com/microsoft/microsoft-ui-xaml)** — The modern native UI framework
- **[Windows App SDK](https://github.com/microsoft/WindowsAppSDK)** — Unified Windows development platform
- **[Fluent Design System](https://fluent2.microsoft.design/)** — Design language and components
- **[.NET](https://dotnet.microsoft.com/)** — Runtime and SDK

---

<div align="center">

**Made with ❤️ for Windows power users and IT professionals**

⭐ Star this repo if you find it useful!

</div>
