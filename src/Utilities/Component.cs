using LibreHardwareMonitor.Hardware;

namespace ZabbixAgentLHM.Utilities;

public static class Component
{
    public static string Name(HardwareType hardwareType)
    {
        // https://github.com/LibreHardwareMonitor/LibreHardwareMonitor/blob/master/LibreHardwareMonitorLib/Hardware/HardwareType.cs
        switch (hardwareType)
        {
            case HardwareType.Motherboard:
            case HardwareType.SuperIO:
                // I'll just use motherboard also here for my purposes
                // https://github.com/LibreHardwareMonitor/LibreHardwareMonitor/blob/master/LibreHardwareMonitorLib/Hardware/Motherboard/SuperIOHardware.cs
                return "Motherboard";
            case HardwareType.Cpu:
                return "CPU";
            case HardwareType.Memory:
                return "Memory";
            case HardwareType.GpuNvidia:
            case HardwareType.GpuAmd:
            case HardwareType.GpuIntel:
                return "GPU";
            case HardwareType.Storage:
                return "Storage";
            case HardwareType.Network:
                return "Network";
            case HardwareType.Cooler:
                return "Cooler";
            case HardwareType.EmbeddedController:
                return "Embedded controller";
            case HardwareType.Psu:
                return "PSU";
            case HardwareType.Battery:
                return "Battery";
            default:
                throw new System.Exception($"No component tag value specified for {hardwareType.ToString()}");
        }
    }
}
