using LibreHardwareMonitor.Hardware;
using System.Text.RegularExpressions;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace ZabbixAgentLHM;

public class Item : IItem
{
    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public int? Delay { get; } = 0;

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public int? History { get; } = null;

    public string Key { get; set; } = ""; // TODO

    public IDictionary<string, string> MasterItem { get; } = new Dictionary<string, string>();

    public string Name { get; set; } = ""; // TODO

    public IList<IPreprocessor> Preprocessing { get; } = new List<IPreprocessor>();

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public int? Trends { get; set; }

    // All items depend on the single master item
    public string Type { get; set; } = "DEPENDENT";

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public string? Units { get; set; }

    public string Uuid { get; } = Utilities.NewUuid();

    [YamlIgnore]
    public float? Value { get; set; }

    // The values are all floats except for the master item
    public string ValueType { get; set; } = "FLOAT";

    public IList<ITag> Tags { get; } = new List<ITag>();

    public Item(IHardware hardware, ISensor sensor)
    {
        this.Name = $"{hardware.Name}: {sensor.Name}";
        this.Value = sensor.Value;

        this.SetKey(sensor.Identifier);
        this.SetUnits(sensor.SensorType);
        this.AddPreprocessor(new DefaultPreprocessor(this.Key));
        this.AddTag(new ComponentTag(hardware.HardwareType));
    }

    public void SetKey(Identifier identifier)
    {
        // Remove all special characters from the names. Allow slash as that's
        // the separator. See:
        // https://github.com/LibreHardwareMonitor/LibreHardwareMonitor/blob/master/LibreHardwareMonitorLib/Hardware/Identifier.cs
        var special = new Regex("[^/a-zA-Z0-9]");

        // Replace slashes and any underscores before or after them
        var slashes = new Regex("_*/_*");

        var identifierKey = special.Replace(identifier.ToString(), "_");
        var identifierDots = slashes.Replace(identifierKey, ".");

        this.Key = $"lhm{identifierDots.ToLower()}";
    }

    public void SetMasterItem(IItem item)
    {
        this.MasterItem.Clear();
        this.MasterItem.Add("key", item.Key);
    }

    public void AddTag(ITag tag)
    {
        this.Tags.Add(tag);
    }

    public void AddPreprocessor(IPreprocessor preprocessor)
    {
        this.Preprocessing.Add(preprocessor);
    }

    // For a complete list of LHM sensors and their units, see:
    // https://github.com/LibreHardwareMonitor/LibreHardwareMonitor/blob/master/LibreHardwareMonitorLib/Hardware/ISensor.cs
    public void SetUnits(SensorType sensorType)
    {
        switch (sensorType)
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
            // "Noise" is not in 0.9.1 yet
            //case SensorType.Noise:
            //  this.Units = "dBA";
            //  break;
            default:
                throw new System.Exception($"No units specified for {sensorType.ToString()}");
        }
    }
}
