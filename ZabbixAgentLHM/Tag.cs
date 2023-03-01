using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace ZabbixAgentLHM;

public class Tag
{
    [YamlMember(Alias = "tag")]
    public string Name { get; set; }

    public string Value { get; set; }

    public Tag(string name, string value)
    {
        this.Name = name;
        this.Value = value;
    }
}
