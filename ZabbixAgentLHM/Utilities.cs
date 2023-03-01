using LibreHardwareMonitor.Hardware;
using System.Text.RegularExpressions;

namespace ZabbixAgentLHM;

//
// Represent the computer-related bools from:
// https://github.com/LibreHardwareMonitor/LibreHardwareMonitor/blob/master/LibreHardwareMonitorLib/Hardware/Computer.cs
// The existing enums don't really seem to correspond to those, so use this.
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

    public static List<SensorType> ParseSensorTypes(string sensorTypesString)
    {
        SensorType sensorTypeEnum;
        var sensorTypes = new List<SensorType>();

        foreach (var sensorType in sensorTypesString.Split(",")) {
            if (Enum.TryParse<SensorType>(sensorType, true, out sensorTypeEnum)) {
                sensorTypes.Add(sensorTypeEnum);
            } else {
                throw new System.Exception($"Unknown sensor type {sensorType}");
            }
        }

        return sensorTypes;
    }

    public static Preprocessor NewDefaultPreprocessor(string key)
    {
        var preProcessor = new Preprocessor();
        preProcessor.Parameters.Add($"return JSON.parse(value)['{key}'];");
        return preProcessor;
    }

    public static List<ZabbixAgentLHM.ComputerHardwareType> ParseHardwareTypes(string hardwareTypesString)
    {
        ZabbixAgentLHM.ComputerHardwareType hardwareTypeEnum;
        var hardwareTypes = new List<ZabbixAgentLHM.ComputerHardwareType>();

        foreach (var hardwareType in hardwareTypesString.Split(",")) {
            if (Enum.TryParse<ZabbixAgentLHM.ComputerHardwareType>(hardwareType, true, out hardwareTypeEnum)) {
                hardwareTypes.Add(hardwareTypeEnum);
            } else {
                throw new System.Exception($"Unknown hardware type {hardwareType}");
            }
        }

        return hardwareTypes;
    }

    //
    // Return a value for the Component tag corresponding to the ComputerHardwareType.
    // Mostly just does capitalization as I prefer "CPU" to "Cpu".
    // See:
    // https://github.com/LibreHardwareMonitor/LibreHardwareMonitor/blob/master/LibreHardwareMonitorLib/Hardware/HardwareType.cs
    //
    public static string ComponentName(HardwareType ht)
    {
        switch (ht)
        {

            case HardwareType.Motherboard:
                return "Motherboard";
            case HardwareType.SuperIO:
                // I'll just use motherboard also here for my purposes. See:
                // https://github.com/LibreHardwareMonitor/LibreHardwareMonitor/blob/master/LibreHardwareMonitorLib/Hardware/Motherboard/SuperIOHardware.cs
                return "Motherboard";
            case HardwareType.Cpu:
                return "CPU";
            case HardwareType.Memory:
                return "Memory";
            case HardwareType.GpuNvidia:
                return "GPU";
            case HardwareType.GpuAmd:
                return "GPU";
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
                throw new System.Exception($"No component tag value specified for {ht.ToString()}");
        }
    }

    //
    // For a complete list of LHM sensors and their units, see:
    // https://github.com/LibreHardwareMonitor/LibreHardwareMonitor/blob/master/LibreHardwareMonitorLib/Hardware/ISensor.cs
    //
    public static string Units(SensorType sensorType)
    {
        switch (sensorType)
        {
            case SensorType.Voltage:
                return "V";
            case SensorType.Current:
                return "A";
            case SensorType.Power:
                return "W";
            case SensorType.Clock:
                return "MHz";
            case SensorType.Temperature:
                return "°C";
            case SensorType.Load:
                return "%";
            case SensorType.Frequency:
                return "Hz";
            case SensorType.Fan:
                return "RPM";
            case SensorType.Flow:
                return "L/h";
            case SensorType.Control:
                return "%";
            case SensorType.Level:
                return "%";
            case SensorType.Factor:
                // Maybe use "x" for factor, if this works like "2.2 x"
                return "x";
            case SensorType.Data:
                // 2^30 Bytes
                return "GB";
            case SensorType.SmallData:
                // 2^20 Bytes
                return "MB";
            case SensorType.Throughput:
                return "B/s";
            case SensorType.TimeSpan:
                // Seconds
                return "s";
            case SensorType.Energy:
                // Milliwatt-hour
                return "mWh";
            // "Noise" is not in 0.9.1 yet
            //case SensorType.Noise:
            //    return "dBA";
            default:
                throw new System.Exception($"No units specified for {sensorType.ToString()}");
        }
    }

    //
    // Format a Zabbix key from the sensor name
    //
    public static string ItemKey(string prefix, string hwName, string sensorName)
    {
        // Used to remove all special characters from the names
        var special = new Regex("[^a-zA-Z0-9]");
        // Used to remove consecutive underscores from the names (single underscores are fine)
        var underscores = new Regex("_+");

        // Replace special characters with underscores
        var lowerHwKey = special.Replace(hwName, "_").ToLower();
        var lowerSensorKey = special.Replace(sensorName, "_").ToLower();

        // Replace consecutive underscores with a single underscore
        var hwKey = underscores.Replace(lowerHwKey, "_").Trim('_');
        var sensorKey = underscores.Replace(lowerSensorKey, "_").Trim('_');

        return $"{prefix}.{hwKey}.{sensorKey}";
    }

    public static string ItemName(string hwName, string sensorName)
    {
        return $"{hwName}: {sensorName}";
    }

    //
    // Time format for Zabbix template
    //
    public static string DateTimeUtcNow()
    {
        return $"{DateTime.UtcNow.ToString("s")}Z";
    }

    //
    // UUID for Zabbix template
    //
    public static string NewUuid()
    {
        var uuid = Guid.NewGuid();
        return uuid.ToString().Replace("-", "");
    }
}