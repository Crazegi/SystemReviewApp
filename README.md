<div align="center">

# 🖥️ SystemReview

### A Modern Windows Desktop App for System Diagnostics, Hardware Health & Network Analysis

[![WinUI 3](https://img.shields.io/badge/WinUI-3-blue?logo=microsoft)](https://microsoft.github.io/microsoft-ui-xaml/)
[![.NET](https://img.shields.io/badge/.NET-10.0-purple?logo=dotnet)](https://dotnet.microsoft.com/)
[![Windows App SDK](https://img.shields.io/badge/Windows%20App%20SDK-1.6-0078D4?logo=windows)](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%2F11-0078D4?logo=windows)](https://www.microsoft.com/windows)

**SystemReview** is a feature-rich WinUI 3 desktop application that provides comprehensive system hardware inspection, **S.M.A.R.T. disk health analysis**, **EDID monitor details**, real-time networking diagnostics, performance monitoring, theme switching, and exportable reports — all in a modern Fluent Design UI.

[Features](#-features) · [Screenshots](#-screenshots) · [Installation](#-installation) · [Building from Source](#-building-from-source) · [Usage Guide](#-usage-guide) · [Architecture](#️-architecture) · [Contributing](#-contributing)

---

</div>

## 📋 Table of Contents

- [Features](#-features)
- [Screenshots](#-screenshots)
- [System Requirements](#-system-requirements)
- [Installation](#-installation)
- [Building from Source](#-building-from-source)
- [Usage Guide](#-usage-guide)
  - [System Specs Tab](#️-system-specs-tab)
  - [Disk Health Tab](#-disk-health-tab)
  - [Monitor Info Tab](#-monitor-info-tab)
  - [Networking Tab](#-networking-tab)
  - [Diagnostics Tab](#-diagnostics-tab)
  - [Settings](#️-settings)
  - [Exporting Data](#-exporting-data)
- [Architecture](#️-architecture)
  - [Project Structure](#project-structure)
  - [Design Patterns](#design-patterns)
  - [Technology Stack](#technology-stack)
  - [API Reference](#apis--data-sources)
- [Configuration](#️-configuration)
- [Troubleshooting](#troubleshooting)
- [Known Limitations](#️-known-limitations)
- [Roadmap](#️-roadmap)
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
| **Graphics (GPU)** | GPU name, **accurate VRAM (supports >4GB via registry)**, resolution, refresh rate, color depth, driver version & date |
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
.inininininininininininininin
```