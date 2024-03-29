using LibreHardwareMonitor.Hardware;
using System.Text.RegularExpressions;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace ZabbixAgentLHM.Zabbix;

public class Item : IItem
{
    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public int? Delay { get; } = 0;

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public int? History { get; } = null;

    public string Key { get; set; }

    public IDictionary<string, string> MasterItem { get; } = new Dictionary<string, string>();

    public string Name { get; set; }

    public IList<Zabbix.IPreprocessor> Preprocessing { get; } = new List<Zabbix.IPreprocessor>();

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public int? Trends { get; set; }

    // All items depend on the single master item
    public string Type { get; set; } = "DEPENDENT";

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public string? Units { get; set; }

    public string Uuid { get; } = Guid.NewGuid().ToString().Replace("-", "");

    [YamlIgnore]
    public float? Value { get; set; }

    // The values are all floats except for the master item
    public string ValueType { get; set; } = "FLOAT";

    public IList<Zabbix.Tag> Tags { get; } = new List<Zabbix.Tag>();

    public Item(IHardware hardware, ISensor sensor)
    {
        this.Name = $"{hardware.Name}: {sensor.Name}";
        this.Value = sensor.Value;

        // Remove all special characters from the names. Allow slash as that's
        // the separator. See:
        // https://github.com/LibreHardwareMonitor/LibreHardwareMonitor/blob/master/LibreHardwareMonitorLib/Hardware/Identifier.cs
        var special = new Regex("[^/a-zA-Z0-9]");

        // Replace slashes and any underscores before or after them
        var slashes = new Regex("_*/_*");

        var identifierKey = special.Replace(sensor.Identifier.ToString(), "_");
        var identifierDots = slashes.Replace(identifierKey, ".");

        // Ensure unique keys
        var underscores = new Regex("_[_]+");
        var sensorKey = special.Replace(sensor.Name.ToString(), "_");
        var sensorUnderscores = underscores.Replace(sensorKey, "_");
        var sensorFinal = slashes.Replace(sensorUnderscores, "_");

        this.Key = $"lhm{identifierDots.ToLower()}.{sensorFinal.ToLower()}";

        this.AddPreprocessor(new Zabbix.DefaultPreprocessor(this.Key));
        this.AddTag(new Zabbix.Tag("Component", Utilities.Component.Name(hardware.HardwareType)));

        // For a complete list of LHM sensors and their units, see:
        // https://github.com/LibreHardwareMonitor/LibreHardwareMonitor/blob/master/LibreHardwareMonitorLib/Hardware/ISensor.cs
        switch (sensor.SensorType)
        {
            case SensorType.Voltage:
                this.Units = "V";
                break;
            case SensorType.Current:
                this.Units = "A";
                break;
            case SensorType.Power:
                this.Units = "W";
                break;
            case SensorType.Clock:
                this.Units = "MHz";
                break;
            case SensorType.Temperature:
                this.Units = "°C";
                break;
            case SensorType.Load:
                this.Units = "%";
                break;
            case SensorType.Frequency:
                this.Units = "Hz";
                break;
            case SensorType.Fan:
                this.Units = "RPM";
                break;
            case SensorType.Flow:
                this.Units = "L/h";
                break;
            case SensorType.Control:
                this.Units = "%";
                break;
            case SensorType.Level:
                this.Units = "%";
                break;
            case SensorType.Factor:
                // Maybe use "x" for factor, if this works like "2.2 x"
                this.Units = "x";
                break;
            case SensorType.Data:
                // 2^30 Bytes
                this.Units = "GB";
                break;
            case SensorType.SmallData:
                // 2^20 Bytes
                this.Units = "MB";
                break;
            case SensorType.Throughput:
                this.Units = "B/s";
                break;
            case SensorType.TimeSpan:
                // Seconds
                this.Units = "s";
                break;
            case SensorType.Energy:
                // Milliwatt-hour
                this.Units = "mWh";
                break;
            case SensorType.Noise:
                this.Units = "dBA";
                break;
            default:
                throw new System.Exception($"No units specified for {sensor.SensorType.ToString()}");
        }
    }

    public void SetMasterItem(IItem item)
    {
        this.MasterItem.Clear();
        this.MasterItem.Add("key", item.Key);
    }

    public void AddTag(Zabbix.Tag tag)
    {
        this.Tags.Add(tag);
    }

    public void AddPreprocessor(Zabbix.IPreprocessor preprocessor)
    {
        this.Preprocessing.Add(preprocessor);
    }

}
