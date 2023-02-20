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

    public static string Units(SensorType sensorType)
    {
        switch (sensorType)
        {
            case SensorType.Fan:
                return "RPM";
            case SensorType.Power:
                return "W";
            case SensorType.Temperature:
                return "°C";
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
