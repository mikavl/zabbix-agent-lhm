using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace ZabbixAgentLHM.Zabbix;

public class Tag
{
    [YamlMember(Alias = "Tag")]
    public string Name { get; }

    public string Value { get; }

    public Tag(string name, string value)
    {
        this.Name = name;
        this.Value = value;
    }
}
