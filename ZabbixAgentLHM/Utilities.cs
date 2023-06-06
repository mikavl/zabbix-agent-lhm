using LibreHardwareMonitor.Hardware;
using System.Text.RegularExpressions;

namespace ZabbixAgentLHM;

//
// Represent the computer-related bools from:
// https://github.com/LibreHardwareMonitor/LibreHardwareMonitor/blob/master/LibreHardwareMonitorLib/Hardware/Computer.cs
// The existing enums don't really seem to correspond to those, so use this.
//
// Special value "All" for enabling everything.
//
public enum ComputerHardwareType {
    Battery,
    Controller,
    Cpu,
    Gpu,
    Memory,
    Motherboard,
    Network,
    Psu,
    Storage
};

public static class Utilities
{
    public const string DefaultPrefix = "lhm";

    public const string MasterItemName = "LibreHardwareMonitor";
    public const string MasterItemSubkey = "gather";
    public const string MasterItemValueType = "TEXT";
    public const string MasterItemType = "ZABBIX_ACTIVE";

    //
    // UUID for Zabbix template
    //
    public static string NewUuid()
    {
        var uuid = Guid.NewGuid();
        return uuid.ToString().Replace("-", "");
    }
}
