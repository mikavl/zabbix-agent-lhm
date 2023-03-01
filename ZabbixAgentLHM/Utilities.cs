using LibreHardwareMonitor.Hardware;
using System.Text.RegularExpressions;

namespace ZabbixAgentLHM;

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

    public static Zabbix.Preprocessor NewDefaultPreprocessor(string key)
    {
        var preProcessor = new Zabbix.Preprocessor();
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

    public static string ItemKey(string prefix, string hwName, string sensorName)
    {
        var special = new Regex("[^a-zA-Z0-9]");
        var underscores = new Regex("_+");

        var lowerHwKey = special.Replace(hwName, "_").ToLower();
        var lowerSensorKey = special.Replace(sensorName, "_").ToLower();

        var hwKey = underscores.Replace(lowerHwKey, "_").Trim('_');
        var sensorKey = underscores.Replace(lowerSensorKey, "_").Trim('_');

        return $"{prefix}.{hwKey}.{sensorKey}";
    }

    public static string ItemName(string hwName, string sensorName)
    {
        return $"{hwName}: {sensorName}";
    }
}
