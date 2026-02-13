namespace SystemReview.Models;

public record CpuInfo(string Name, int Cores, int LogicalProcessors, string MaxSpeed, string Architecture);
public record GpuInfo(string Name, string DriverVersion, string AdapterRam, string VideoProcessor);
public record DriveInfoModel(string Name, string Label, string DriveType, string FileSystem, string TotalSize, string FreeSpace, double UsedPercent);
public record NetworkAdapterInfo(string Name, string Description, string Status, string Speed, string MacAddress, string AdapterType);
public record PingResultModel(string Host, string Status, long RoundtripMs, int Ttl, string Timestamp);
public record TracerouteHop(int Hop, string Address, string RoundtripMs, string Status);
public record OpenPortInfo(string Protocol, string LocalAddress, int LocalPort, string RemoteAddress, int RemotePort, string State, int ProcessId);
public record DnsResultModel(string HostName, string AddressFamily, string Address);
public record EventLogEntryModel(string Source, string Level, string Message, DateTime TimeGenerated, long InstanceId);
public record ServiceInfoModel(string Name, string DisplayName, string Status, string StartType);