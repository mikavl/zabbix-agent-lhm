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

    public void SetGroup(
        string groupName)
    {
        // Template groups should have no UUID, so don't set one here
        var group = new Group(groupName);

        this.Groups.Clear();
        this.Groups.Add(group);
    }

    public void SetName(
        string name)
    {
        this.Name = name;
        this.TemplateName = name;
    }
}
