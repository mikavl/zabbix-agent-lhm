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
    public IList<Preprocessor> Preprocessors { get; }

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public int? Trends { get; set; }

    public string Type { get; set; }

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public string? Units { get; set; }

    public string Uuid { get; }

    [YamlIgnore]
    public float? Value { get; set; }

    public string? ValueType { get; set; }

    public IList<Tag> Tags { get; }

    public Item()
    {
        this.MasterItem = new Dictionary<string, string>();
        this.Preprocessors = new List<Preprocessor>();
        this.Uuid = Utilities.NewUuid();
        this.Tags = new List<Tag>();

        // All items depend on the single master item
        this.Type = "DEPENDENT";

        // The values are all floats except for the master item
        this.ValueType = "FLOAT";
    }

    public void SetMasterItem(string key)
    {
        this.MasterItem.Add("key", key);
    }
}
