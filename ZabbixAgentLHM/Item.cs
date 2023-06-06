using LibreHardwareMonitor.Hardware;
using System.Text.RegularExpressions;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace ZabbixAgentLHM;

public class Item
{
    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public int? Delay { get; set; }

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public int? History { get; set; }

    public string? Key { get; set; }

    public IDictionary<string, string> MasterItem { get; }

    public string? Name { get; set; }

    [YamlMember(Alias = "preprocessing")]
    public IList<IPreprocessor> Preprocessors { get; }

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public int? Trends { get; set; }

    public string Type { get; set; }

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public string? Units { get; set; }

    public string Uuid { get; }

    [YamlIgnore]
    public float? Value { get; set; }

    public string? ValueType { get; set; }

    public IList<ITag> Tags { get; }

    public Item()
    {
        this.MasterItem = new Dictionary<string, string>();
        this.Preprocessors = new List<IPreprocessor>();
        this.Uuid = Utilities.NewUuid();
        this.Tags = new List<ITag>();

        // All items depend on the single master item
        this.Type = "DEPENDENT";

        // The values are all floats except for the master item
        this.ValueType = "FLOAT";
    }

    public void SetName(string hardwareName, string sensorName)
    {
        this.Name = $"{hardwareName}: {sensorName}";
    }

    public void SetKey(string prefix, Identifier identifier)
    {
        // Remove all special characters from the names. Allow slash as that's
        // the separator. See:
        // https://github.com/LibreHardwareMonitor/LibreHardwareMonitor/blob/master/LibreHardwareMonitorLib/Hardware/Identifier.cs
        var special = new Regex("[^/a-zA-Z0-9]");

        // Replace slashes and any underscores before or after them
        var slashes = new Regex("_*/_*");

        var identifierKey = special.Replace(identifier.ToString(), "_");
        var identifierDots = slashes.Replace(identifierKey, ".");

        this.Key = $"{prefix}{identifierDots.ToLower()}";
    }

    public void SetMasterItem(Item item)
    {
        if (item.Key is string key)
        {
            this.MasterItem.Clear();
            this.MasterItem.Add("key", key);
        }
        else
        {
            throw new System.Exception("Master item candidate key was null, this should not happen");
        }
    }

    public void AddTag(ITag tag)
    {
        this.Tags.Add(tag);
    }

    public void AddPreprocessor(IPreprocessor preprocessor)
    {
        this.Preprocessors.Add(preprocessor);
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
