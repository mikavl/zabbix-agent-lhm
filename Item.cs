using LibreHardwareMonitor.Hardware;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

public class Item
{
    public string? Uuid { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public string? Key { get; set; }

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public int? Delay { get; set; }

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public string? Units { get; set; }

    public string? Params { get; set; }

    public string? ValueType { get; set; }

    public List<Preprocessor> Preprocessing { get; set; }

    public IDictionary<string, string> MasterItem { get; set; }

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public int? History { get; set; }

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public int? Trends { get; set; }

    [YamlIgnore]
    public float? Value { get; set; }

    public Item()
    {
        this.MasterItem = new Dictionary<string, string>();
        this.Preprocessing = new List<Preprocessor>();
    }

    public void SetName(string hardware, string sensor)
    {
        this.Name = $"{hardware}: {sensor}";
    }

    public void SetKey(string prefix, string hardware, string sensor)
    {
        Regex special = new Regex("[^a-zA-Z0-9]");

        var hardwareKey = special.Replace(hardware, "_").ToLower();
        var sensorKey = special.Replace(sensor, "_").ToLower();

        Regex underscores = new Regex("_+");

        hardwareKey = underscores.Replace(hardwareKey, "_").Trim('_');
        sensorKey = underscores.Replace(sensorKey, "_").Trim('_');

        this.Key = $"{prefix}.{hardwareKey}.{sensorKey}";
    }

    public void SetUnits(SensorType sensorType)
    {
        switch (sensorType)
        {
            case SensorType.Power:
                this.Units = "W";
                break;
            case SensorType.Temperature:
                this.Units = "°C";
                break;
            case SensorType.Fan:
                this.Units = "RPM";
                break;
            default:
                throw new System.Exception($"No units specified for {sensorType.ToString()}");
        }
    }

    public void AddDefaultPreprocessor()
    {
        Preprocessor pp = new Preprocessor();
        pp.Type = "JAVASCRIPT";
        pp.Parameters.Add("var obj = JSON.parse(value);");
        pp.Parameters.Add($"return obj['{this.Key}'];");
        this.Preprocessing.Add(pp);
    }

    public void SetMasterItem(string key)
    {
        this.MasterItem.Add("key", key);
    }
}
