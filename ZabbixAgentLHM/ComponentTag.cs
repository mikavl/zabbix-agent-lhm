using LibreHardwareMonitor.Hardware;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace ZabbixAgentLHM
{
    public class ComponentTag : ITag
    {
        [YamlMember(Alias = "tag")]
        public string? Name { get; set; }

        public string? Value { get; set; }

        public ComponentTag(HardwareType hardwareType)
        {
            this.Name = "Component";
            this.SetValue(hardwareType);
        }

        // https://github.com/LibreHardwareMonitor/LibreHardwareMonitor/blob/master/LibreHardwareMonitorLib/Hardware/HardwareType.cs
        public void SetValue(HardwareType hardwareType)
        {
            switch (hardwareType)
            {
                case HardwareType.Motherboard:
                case HardwareType.SuperIO:
                    // I'll just use motherboard also here for my purposes
                    // https://github.com/LibreHardwareMonitor/LibreHardwareMonitor/blob/master/LibreHardwareMonitorLib/Hardware/Motherboard/SuperIOHardware.cs
                    this.Value = "Motherboard";
                    break;
                case HardwareType.Cpu:
                    this.Value = "CPU";
                    break;
                case HardwareType.Memory:
                    this.Value = "Memory";
                    break;
                case HardwareType.GpuNvidia:
                case HardwareType.GpuAmd:
                case HardwareType.GpuIntel:
                    this.Value = "GPU";
                    break;
                case HardwareType.Storage:
                    this.Value = "Storage";
                    break;
                case HardwareType.Network:
                    this.Value = "Network";
                    break;
                case HardwareType.Cooler:
                    this.Value = "Cooler";
                    break;
                case HardwareType.EmbeddedController:
                    this.Value = "Embedded controller";
                    break;
                case HardwareType.Psu:
                    this.Value = "PSU";
                    break;
                case HardwareType.Battery:
                    this.Value = "Battery";
                    break;
                default:
                    throw new System.Exception($"No component tag value specified for {hardwareType.ToString()}");
            }
        }
    }
}
