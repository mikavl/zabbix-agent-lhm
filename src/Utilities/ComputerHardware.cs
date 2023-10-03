namespace ZabbixAgentLHM.Utilities;

// Represent the computer-related bools from:
// https://github.com/LibreHardwareMonitor/LibreHardwareMonitor/blob/master/LibreHardwareMonitorLib/Hardware/Computer.cs
// The existing enums don't really seem to correspond to those, so use this.
public enum ComputerHardware {
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
