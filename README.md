<div align="center">

# 🖥️ SystemReview

### A Modern Windows Desktop App for System Diagnostics, Hardware Health & Network Analysis

[![WinUI 3](https://img.shields.io/badge/WinUI-3-blue?logo=microsoft)](https://microsoft.github.io/microsoft-ui-xaml/)
[![.NET](https://img.shields.io/badge/.NET-10.0-purple?logo=dotnet)](https://dotnet.microsoft.com/)
[![Windows App SDK](https://img.shields.io/badge/Windows%20App%20SDK-1.6-0078D4?logo=windows)](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%2F11-0078D4?logo=windows)](https://www.microsoft.com/windows)

**SystemReview** is a feature-rich WinUI 3 desktop application that provides comprehensive system hardware inspection, **S.M.A.R.T. disk health analysis**, **EDID monitor details**, real-time networking diagnostics, performance monitoring, theme switching, and exportable reports — all in a modern Fluent Design UI.

[Features](#-features) · [Screenshots](#-screenshots) · [Installation](#-installation) · [Building from Source](#-building-from-source) · [Usage Guide](#-usage-guide) · [Architecture](#%EF%B8%8F-architecture) · [Contributing](#-contributing)

---

</div>

## 📋 Table of Contents

- [Features](#-features)
- [Screenshots](#-screenshots)
- [System Requirements](#-system-requirements)
- [Installation](#-installation)
- [Building from Source](#-building-from-source)
- [Usage Guide](#-usage-guide)
  - [System Specs Tab](#%EF%B8%8F-system-specs-tab)
  - [Disk Health Tab](#-disk-health-tab)
  - [Monitor Info Tab](#%EF%B8%8F-monitor-info-tab)
  - [Networking Tab](#-networking-tab)
  - [Diagnostics Tab](#-diagnostics-tab)
  - [Settings](#%EF%B8%8F-settings)
  - [Exporting Data](#-exporting-data)
- [Architecture](#%EF%B8%8F-architecture)
  - [Project Structure](#project-structure)
  - [Design Patterns](#design-patterns)
  - [Technology Stack](#technology-stack)
  - [API Reference](#apis--data-sources)
- [Configuration](#%EF%B8%8F-configuration)
- [Troubleshooting](#-troubleshooting)
- [Known Limitations](#%EF%B8%8F-known-limitations)
- [Roadmap](#%EF%B8%8F-roadmap)
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
| **Graphics (GPU)** | GPU name, **accurate VRAM (supports >4 GB via registry)**, resolution, refresh rate, color depth, driver version & date |
| **Storage Drives** | All mounted drives with volume label, file system type, total/free space, visual usage bar |
| **Motherboard & BIOS** | Manufacturer, product name, serial number, BIOS version, BIOS manufacturer, release date |
| **Operating System** | OS name, version, build number, architecture, install date, uptime, last boot time, machine/user name |
| **Display** | GPU-reported resolution, refresh rate, color depth per output |
| **Battery** | Battery name, charge percentage, estimated runtime, chemistry type, design voltage (laptops) |
| **Installed Software** | Top 30 installed programs with version numbers (fast registry-based scanning) |

### 💾 Disk Health (S.M.A.R.T.)
| Feature | Details |
|---------|---------|
| **S.M.A.R.T. Data** | Raw attribute parsing from `MSStorageDriver_ATAPISmartData` WMI |
| **Power-On Hours** | Total lifetime the disk has been powered on (hours, days, years) |
| **Power Cycle Count** | Number of times the disk was turned on/off |
| **Temperature** | Current drive temperature in °C and °F |
| **Health Status** | ✅ Healthy / ⚠️ Warning / ❌ Critical — based on reallocated, pending, and uncorrectable sectors |
| **SSD vs HDD Detection** | Automatic media type detection via `MSFT_PhysicalDisk` |
| **All Attributes Table** | Expandable table with ID, name, current, worst, threshold, raw value, and status for every S.M.A.R.T. attribute |
| **Threshold Checking** | Reads thresholds from `MSStorageDriver_FailurePredictThresholds` and flags degraded attributes |

### 🖥️ Monitor Info (EDID)
| Feature | Details |
|---------|---------|
| **EDID Parsing** | Reads raw Extended Display Identification Data from hardware registers |
| **Manufacturer** | Decoded from PnP codes (Samsung, Dell, LG, ASUS, BenQ, Acer, etc.) |
| **Serial Number** | Hardware serial from EDID descriptor blocks |
| **Screen Size** | Physical dimensions in cm and inches with diagonal calculation |
| **Native Resolution** | Parsed from EDID detailed timing blocks |
| **Color Bit Depth** | 6-bit, 8-bit, 10-bit, 12-bit, etc. |
| **Display Type** | Digital (HDMI/DisplayPort/DVI) or Analog (VGA) |
| **Connection Type** | HDMI, DisplayPort, DVI, VGA, eDP, etc. via `WmiMonitorConnectionParams` |
| **Manufacture Date** | Week and year of manufacture with estimated lifetime usage |
| **DPMS Support** | Standby, Suspend, Active-Off power management capabilities |
| **Supported Resolutions** | Established timings list from EDID |
| **Fallback Detection** | Uses `WmiMonitorID` when EDID raw blocks are unavailable |

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

### ⚙️ Settings & Theming
| Feature | Details |
|---------|---------|
| **Dark / Light / System Theme** | Switch between dark mode, light mode, or follow system setting |
| **Auto-load Toggle** | Enable/disable automatic data loading when switching tabs |
| **About Section** | App version, description, and GitHub link |

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

> Data loads **automatically** when you switch tabs (configurable in Settings).

### System Specs Tab
```
┌──────────────────────────────────────────────────┐
│  [🔄 Refresh All]  [📋 Export JSON]  [📝 Export] │
│                                                    │
│  ┌─ 🖥️ Processor (CPU) ────────────────────┐     │
│  │  Name:          AMD Ryzen 9 7950X        │     │
│  │  Cores:         16                        │     │
│  │  Logical Procs: 32                        │     │
│  └──────────────────────────────────────────┘     │
│                                                    │
│  ┌─ 🎮 Graphics (GPU) ────────────────────┐      │
│  │  Name:       NVIDIA GeForce RTX 4070    │      │
│  │  VRAM:       8.00 GB  ← accurate!       │      │
│  │  Resolution: 2560 x 1440                │      │
│  │  Refresh:    165 Hz                     │      │
│  │  Driver:     32.0.15.6590 (2026-01-15)  │      │
│  └──────────────────────────────────────────┘     │
│                                                    │
│  ┌─ 🧠 Memory (RAM) ──────────────────────┐      │
│  │  [████████████░░░░░░░░] 62%             │      │
│  │  Total: 32.00 GB  Available: 12.14 GB   │      │
│  └─────────────────────────────────────────┘      │
└──────────────────────────────────────────────────┘
```

### Disk Health Tab (S.M.A.R.T.)
```
┌──────────────────────────────────────────────────┐
│  [🔄 Refresh Disk Health]                         │
│                                                    │
│  ┌─ 💾 Samsung SSD 980 PRO 1TB ─ ✅ Healthy ┐   │
│  │  Media Type:     SSD (Solid State)         │   │
│  │  Interface:      SCSI (NVMe)               │   │
│  │  Power-On Time:  8,742 hours               │   │
│  │  Power Cycles:   1,247                     │   │
│  │  Temperature:    38°C / 100°F              │   │
│  │  ────────────────────────────────────────  │   │
│  │  Reallocated:    0                         │   │
│  │  Pending:        0                         │   │
│  │  Uncorrectable:  0                         │   │
│  │                                             │   │
│  │  ▸ All S.M.A.R.T. Attributes (14)         │   │
│  │    ID  Attribute            Curr  Raw  Sts │   │
│  │    1   Read Error Rate      100   0    OK  │   │
│  │    5   Reallocated Sectors  100   0    OK  │   │
│  │    9   Power-On Hours       98    8742 OK  │   │
│  │    194 Temperature          68    38   OK  │   │
│  └─────────────────��───────────────────────────┘  │
└──────────────────────────────────────────────────┘
```

### Monitor Info Tab (EDID)
```
┌──────────────────────────────────────────────────┐
│  [🔄 Refresh Monitor Info]                        │
│                                                    │
│  ┌─ 🖥️ Monitor 1: DELL S2722DGM ─ 🟢 Active ┐  │
│  │  Manufacturer:   Dell (DEL)                 │  │
│  │  Serial:         F8KH3N3                    │  │
│  │  Connection:     DisplayPort (External)     │  │
│  │  ────────────────────────────────────────   │  │
│  │  Display Type:   Digital (DisplayPort)      │  │
│  │  Color Depth:    8-bit                      │  │
│  │  Gamma:          2.20                       │  │
│  │  ────────────────────────────────────────   │  │
│  │  Native Res:     2560 x 1440               │  │
│  │  Screen Size:    60 cm x 34 cm             │  │
│  │  Diagonal:       27.2"                      │  │
│  │  ────────────────────────────────────────   │  │
│  │  Manufactured:   Week 22, 2023              │  │
│  │  Est. Usage:     ~2.7 yrs (est. 7,884 hrs) │  │
│  │                                              │  │
│  │  ▸ Supported Resolutions (8)                │  │
│  └──────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────┘
```

### Networking Tab
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
│  === Diagnostics Complete ===                      │
└──────────────────────────────────────────────────┘
```

### Settings
```
┌──────────────────────────────────────────────────┐
│  Settings                                          │
│                                                    │
│  ┌─ 🎨 Appearance ────────────────────────┐       │
│  │  App Theme:                             │       │
│  │  ○ Use system setting                   │       │
│  │  ○ Light                                │       │
│  │  ● Dark                                 │       │
│  └─────────────────────────────────────────┘       │
│                                                    │
│  ┌─ 🔄 Behavior ──────────────────────────┐       │
│  │  Auto-load data when switching tabs [✓] │       │
│  └─────────────────────────────────────────┘       │
│                                                    │
│  ┌─ ℹ️ About ──────────────────────────────┐      │
│  │  SystemReview v1.2.0                     │      │
│  │  Made by Crazegi                         │      │
│  │  [View on GitHub]                        │      │
│  └──────────────────────────────────────────┘      │
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
| **Permissions** | Administrator recommended (required for S.M.A.R.T. data, Event Logs, Performance Counters) |

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
git clone https://github.com/Crazegi/SystemReviewApp.git
cd SystemReviewApp

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

The primary hardware and software inventory tab. **Data loads automatically** when you open the tab.

| Card | What It Shows |
|------|--------------|
| **Processor** | CPU model, core count, thread count, clock speed, architecture |
| **Memory** | Total/available RAM with a color-coded usage bar |
| **Graphics** | Each GPU with **accurate VRAM** (even >4 GB), resolution, refresh rate, color depth, driver version & date |
| **Storage** | Per-drive cards with usage bars — quickly spot drives running low |
| **Operating System** | Windows version, build, uptime, install date, current user |
| **Motherboard / BIOS** | Board manufacturer/model, BIOS version and vendor |
| **Display** | Resolution, refresh rate, color depth per GPU output |
| **Battery** | Charge level, estimated runtime, battery chemistry (laptops only) |
| **Installed Software** | Top 30 installed programs (fast registry scan, not slow Win32_Product) |

### 💾 Disk Health Tab

Deep storage health analysis using S.M.A.R.T. data parsed from raw hardware registers.

> ⚠️ **Run as Administrator** for full S.M.A.R.T. data! Without admin, basic disk info is still shown.

- **Power-On Hours** — how long the disk has been running in its entire lifetime
- **Power Cycle Count** — total on/off cycles
- **Temperature** — current drive temperature
- **Sector Health** — reallocated, pending, and uncorrectable sectors with color-coded warnings
- **Health Badge** — instant visual: ✅ Healthy / ⚠️ Warning / ❌ Critical
- **SSD vs HDD** — automatic media type detection
- **Full Attribute Table** — expandable list of all S.M.A.R.T. attributes with thresholds

### 🖥️ Monitor Info Tab

Detailed monitor specifications parsed from EDID (Extended Display Identification Data).

- **Manufacturer** — decoded from PnP 3-letter codes (supports 30+ brands)
- **Screen dimensions** — physical size in cm/inches with diagonal
- **Native resolution** — from detailed timing blocks
- **Color depth & gamma** — bit depth and gamma curve
- **Connection type** — HDMI, DisplayPort, DVI, VGA, eDP
- **Age & estimated usage** — manufacture date with estimated lifetime hours
- **Fallback** — uses `WmiMonitorID` when raw EDID is unavailable

### 🌐 Networking Tab

Interactive network analysis tools with real-time updates.

| Tool | How To Use |
|------|-----------|
| **IP Config** | Auto-loads on tab open — shows all adapter IPs, gateways, DNS |
| **Ping** | Enter host, click Ping — 4 ICMP requests with latency |
| **Traceroute** | Enter host, click Trace — live hop-by-hop display |
| **Port Scanner** | Click Scan — all TCP/UDP connections and listeners |
| **DNS Lookup** | Enter hostname, click Lookup — resolves to all IPs |
| **Net Stats** | Auto-loads — bytes/packets sent/received per adapter |

The app automatically detects network changes and refreshes.

### 🩺 Diagnostics Tab

System health monitoring and automated diagnostics.

- **Performance Counters** — CPU %, available RAM, disk activity, process count
- **Real-time Monitor** — 2-second auto-refresh for continuous tracking
- **Run Diagnostics** — 5-step automated health check (internet, DNS, firewall, adapters, gateway)
- **Event Logs** — recent Windows System errors/warnings
- **Network Services** — status of 17 critical Windows services

### ⚙️ Settings

Click the **⚙️ Settings** gear icon in the navigation bar:

- **Theme** — Switch between Dark, Light, or System default
- **Auto-load** — Toggle automatic data loading when switching tabs
- **About** — Version info and GitHub link

### 📤 Exporting Data

Every tab (Specs, Networking, Diagnostics) has **Export JSON** and **Export Text** buttons.

| Export Type | Format | Best For |
|-------------|--------|----------|
| **JSON** | Structured, indented JSON | Automation, scripts, APIs |
| **Text** | Formatted plain text with ASCII art | Sharing, printing, documentation |

Files save to your **Desktop** with timestamped names:
```
SystemReview_Specs_20260213_143022.json
SystemReview_Network_20260213_143145.txt
SystemReview_Diagnostics_20260213_143312.json
```

---

## 🏗️ Architecture

### Project Structure

```
SystemReviewApp/
│
├── SystemReview.csproj           # Project configuration, NuGet references
├── WinApp.sln                    # Visual Studio solution file
├── LICENSE                       # GNU GPL v3 license
├── .gitignore                    # Git ignore rules (bin/, obj/, etc.)
│
├── App.xaml                      # Application entry point (XAML resources)
├── App.xaml.cs                   # Application startup, theme support
│
├── MainWindows.xaml              # Main window with NavigationView (5 tabs + Settings)
├── MainWindows.xaml.cs           # Navigation logic, theme switching
│
├── Helpers/
│   ├── ObservableObject.cs       # INotifyPropertyChanged base class
│   └── RelayCommand.cs           # ICommand implementations (sync + async)
│
├── Models/
│   └── AllModels.cs              # All data records (CpuInfo, GpuInfo, DiskHealthModel,
│                                 #   MonitorDetailModel, SmartAttribute, etc.)
│
├── Services/
│   ├── WmiService.cs             # WMI queries, S.M.A.R.T. parsing, EDID parsing,
│   │                             #   GPU VRAM from registry, monitor detection
│   ├── NetworkService.cs         # Network tools (ping, traceroute, DNS, ports)
│   └── DiagnosticsService.cs     # Perf counters, event logs, services, diagnostics
│
├── ViewModels/
│   ├── SystemSpecsViewModel.cs   # Logic + data for System Specs tab
│   ├── NetworkingViewModel.cs    # Logic + data for Networking tab
│   └── DiagnosticsViewModel.cs   # Logic + data for Diagnostics tab
│
└── Views/
    ├── SystemSpecsPage.xaml/.cs  # System Specs tab (auto-loads)
    ├── DiskHealthPage.xaml/.cs   # Disk Health tab — S.M.A.R.T. data (auto-loads)
    ├── MonitorInfoPage.xaml/.cs  # Monitor Info tab — EDID data (auto-loads)
    ├── NetworkingPage.xaml/.cs   # Networking tab
    ├── DiagnosticsPage.xaml/.cs  # Diagnostics tab
    └── SettingsPage.xaml/.cs     # Settings — theme, auto-load, about
```

### Design Patterns

| Pattern | Implementation |
|---------|---------------|
| **MVVM** | ViewModels contain all logic; Views only handle UI binding and events |
| **Observable Pattern** | `ObservableObject` base class with `INotifyPropertyChanged` for reactive UI |
| **Command Pattern** | `RelayCommand` and `AsyncRelayCommand` for button bindings |
| **Service Layer** | Static service classes encapsulate all data access |
| **Async/Await** | All I/O-bound and CPU-bound operations are async with `Task.Run` |
| **Parallel Loading** | `Task.WhenAll` loads all sections simultaneously for fast refresh |
| **Auto-load** | Pages load data on `Loaded` event (configurable via Settings) |

### Technology Stack

| Component | Technology |
|-----------|-----------|
| **UI Framework** | WinUI 3 (Microsoft.UI.Xaml) |
| **App Platform** | Windows App SDK 1.6 |
| **Language** | C# 12 |
| **Runtime** | .NET 10.0 |
| **Hardware Queries** | WMI via `System.Management` |
| **GPU VRAM** | Windows Registry (`HardwareInformation.qwMemorySize`) |
| **Disk Health** | WMI `MSStorageDriver_ATAPISmartData` + `FailurePredictThresholds` |
| **Monitor Info** | WMI `WmiMonitorRawEEdidV1Block` + `WmiMonitorID` + `WmiMonitorConnectionParams` |
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
| `Win32_VideoController` | GPU name, driver, resolution, refresh rate, color depth |
| `Win32_DiskDrive` | Physical disk model, serial, firmware, interface, size |
| `Win32_BaseBoard` | Motherboard manufacturer, product, serial |
| `Win32_BIOS` | BIOS version, manufacturer, release date |
| `Win32_OperatingSystem` | OS name, version, build, uptime, install date |
| `Win32_Battery` | Charge level, runtime, chemistry, voltage |
| `MSFT_PhysicalDisk` | SSD vs HDD detection (media type, spindle speed) |
| `MSStorageDriver_ATAPISmartData` | Raw S.M.A.R.T. attribute data (power-on hours, temp, sectors) |
| `MSStorageDriver_FailurePredictThresholds` | S.M.A.R.T. threshold values per attribute |
| `WmiMonitorRawEEdidV1Block` | Raw EDID bytes for monitor identification |
| `WmiMonitorID` | Monitor manufacturer, serial, name (fallback) |
| `WmiMonitorConnectionParams` | Connection type (HDMI/DP/DVI/VGA) and active status |

#### Registry Keys Used

| Key | Purpose |
|-----|---------|
| `HKLM\SYSTEM\ControlSet001\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}` | Accurate GPU VRAM (`qwMemorySize`) for cards >4 GB |
| `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall` | Fast installed software enumeration |
| `HKLM\SOFTWARE\WOW6432Node\...\Uninstall` | 32-bit software on 64-bit systems |

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

Some features require elevated privileges for full data:

| Feature | Without Admin | With Admin |
|---------|:------------:|:----------:|
| CPU, GPU, Drives | ✅ Full | ✅ Full |
| RAM | ✅ Full | ✅ Full |
| GPU VRAM (>4 GB) | ✅ Full (registry) | ✅ Full |
| **S.M.A.R.T. Disk Health** | ⚠️ Basic info only | ✅ **Full S.M.A.R.T. data** |
| Monitor EDID | ✅ Full | ✅ Full |
| Network IP/Adapters | ✅ Full | ✅ Full |
| Ping, DNS, Traceroute | ✅ Full | ✅ Full |
| Performance Counters | ⚠️ May be limited | ✅ Full |
| Event Logs | ⚠️ May be limited | ✅ Full |
| Firewall Status | ❌ Access denied | ✅ Full |

To run as administrator:
```powershell
Start-Process .\SystemReview.exe -Verb RunAs
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
<summary><strong>"Access Denied" errors for WMI or Event Logs</strong></summary>

Run the application as Administrator:
```powershell
Start-Process .\SystemReview.exe -Verb RunAs
```
</details>

<details>
<summary><strong>S.M.A.R.T. data shows "N/A (run as Admin)"</strong></summary>

The S.M.A.R.T. WMI classes (`MSStorageDriver_ATAPISmartData`) require administrator privileges. Run the app as admin to see power-on hours, temperature, and sector health.
</details>

<details>
<summary><strong>GPU VRAM shows 4 GB instead of actual amount</strong></summary>

This was fixed in v1.2.0. The app now reads VRAM from the Windows registry (`qwMemorySize`) which supports values >4 GB. If you still see incorrect values, ensure you're running the latest version.
</details>

<details>
<summary><strong>Monitor Info tab shows no data</strong></summary>

Some monitors don't expose EDID data through WMI. The app has multiple fallbacks:
1. Raw EDID blocks (`WmiMonitorRawEEdidV1Block`)
2. Monitor ID (`WmiMonitorID`) — manufacturer, serial, name
3. Desktop Monitor (`Win32_DesktopMonitor`) — basic info

If none work, your monitor's driver may not support WMI queries.
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
| **NVMe S.M.A.R.T.** | Some NVMe drives don't expose data via `MSStorageDriver_ATAPISmartData` | Use manufacturer's tool (Samsung Magician, etc.) |
| **Monitor EDID** | Some monitors/drivers don't expose raw EDID via WMI | App falls back to `WmiMonitorID` |
| **WiFi network scanning** | WinUI 3 unpackaged apps can't access `WiFiAdapter` | Use `netsh wlan show networks` manually |
| **Single architecture** | Self-contained builds target one architecture | Build separately for x64 and ARM64 |
| **Windows only** | WMI, PerformanceCounter, and WinUI 3 are Windows-specific | No cross-platform support planned |
| **Export to Desktop only** | Hardcoded export path | Modify `ExportAsync` methods for custom paths |

---

## 🗺️ Roadmap

### Completed ✅
- [x] **Dark/Light theme toggle** — Manual theme switching with system theme detection
- [x] **S.M.A.R.T. disk health** — Full disk lifetime and health analysis
- [x] **Monitor EDID parsing** — Detailed monitor specs from hardware data
- [x] **Accurate GPU VRAM** — Registry-based reading for >4 GB cards
- [x] **Auto-load pages** — Data loads when switching tabs
- [x] **Settings page** — Theme, behavior, and about info
- [x] **Fast software scanning** — Registry-based instead of slow Win32_Product

### Planned Features
- [ ] **Hardware temperature monitoring** — CPU/GPU temps via OpenHardwareMonitor lib
- [ ] **Network speed test** — Built-in download/upload speed measurement
- [ ] **WiFi signal strength** — Wireless network signal quality display
- [ ] **Process manager** — Top resource-consuming processes list
- [ ] **Startup programs** — List and manage auto-start applications
- [ ] **System health score** — Aggregate score based on all diagnostics
- [ ] **Custom export paths** — File picker dialog for export location
- [ ] **Historical data** — Track system metrics over time with charts
- [ ] **MSIX packaging** — Microsoft Store distribution

---

## 🤝 Contributing

Contributions are welcome! Here's how to get started:

### Setting Up Development Environment

1. Install prerequisites:
   ```powershell
   winget install -e --id Microsoft.DotNet.SDK.10
   winget install -e --id Microsoft.WindowsAppRuntime.1.6
   ```

2. Fork and clone:
   ```powershell
   git clone https://github.com/YOUR_USERNAME/SystemReviewApp.git
   cd SystemReviewApp
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

---

## 📄 License

This project is licensed under the **GNU General Public License v3.0** — see the [LICENSE](LICENSE) file for details.

### What this means:

- ✅ **Use** — You can use this software for any purpose
- ✅ **Modify** — You can modify the source code
- ✅ **Distribute** — You can share and distribute copies
- ✅ **Patent Use** — Contributors provide an express grant of patent rights
- ⚠️ **Same License** — Any modified version must also be distributed under GPL v3
- ⚠️ **Disclose Source** — You must make your source code available when distributing
- ⚠️ **State Changes** — You must document changes you make to the code
- ❌ **No Sublicensing** — You cannot relicense under different terms

> **TL;DR:** You can do anything with this code, but if you distribute a modified version, you must keep it open source under the same GPL v3 license.

For the full license text, see: https://www.gnu.org/licenses/gpl-3.0.en.html

---

## 🙏 Acknowledgments

- **[Microsoft WinUI 3](https://github.com/microsoft/microsoft-ui-xaml)** — The modern native UI framework
- **[Windows App SDK](https://github.com/microsoft/WindowsAppSDK)** — Unified Windows development platform
- **[Fluent Design System](https://fluent2.microsoft.design/)** — Design language and components
- **[.NET](https://dotnet.microsoft.com/)** — Runtime and SDK

---

<div align="center">

**Made with ❤️ by [Crazegi](https://github.com/Crazegi) for Windows power users and IT professionals**

⭐ Star this repo if you find it useful!

</div>
