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

    public IDictionary<string, string> MasterItem { get; } = new Dictionary<string, string>();

    public string? Name { get; set; }

    public IList<Preprocessor> Preprocessing { get; } = new List<Preprocessor>();

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public int? Trends { get; set; }

    public string Type { get; set; } = "DEPENDENT";

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public string? Units { get; set; }

    public string Uuid { get; } = Utilities.NewUuid();

    [YamlIgnore]
    public float? Value { get; set; }

    public string? ValueType { get; set; } = "FLOAT";

    public void SetMasterItem(string key)
    {
        this.MasterItem.Add("key", key);
    }
}
