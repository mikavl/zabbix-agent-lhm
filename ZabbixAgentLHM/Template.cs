using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace ZabbixAgentLHM;

public class Template
{
    public IList<Group> Groups { get; }

    public IList<Item> Items { get; set; }

    public string Uuid { get; }

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public string? Name { get; set; }

    [YamlMember(Alias = "template", ScalarStyle = ScalarStyle.SingleQuoted)]
    public string? TemplateName { get; set; }

    public Template()
    {
        this.Groups = new List<Group>();
        this.Items = new List<Item>();
        this.Uuid = Utilities.NewUuid();
    }

    public void SetGroupByName(
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
