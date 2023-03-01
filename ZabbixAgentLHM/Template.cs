using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace ZabbixAgentLHM;

public class Template
{
    public IList<Group> Groups { get; } = new List<Group>();

    public IList<Item> Items { get; set; } = new List<Item>();

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public string? Name { get; set; }

    [YamlMember(Alias = "template", ScalarStyle = ScalarStyle.SingleQuoted)]
    public string? TemplateName { get; set; }

    public string? Uuid { get; set; } = Utilities.NewUuid();
}
