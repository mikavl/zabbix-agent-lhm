using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace ZabbixAgentLHM;

public class Template
{
    public IList<IGroup> Groups { get; }

    public IList<IItem> Items { get; set; }

    public string Uuid { get; }

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public string? Name { get; set; }

    [YamlMember(Alias = "template", ScalarStyle = ScalarStyle.SingleQuoted)]
    public string? TemplateName { get; set; }

    public Template(string name)
    {
        this.Groups = new List<IGroup>();
        this.Items = new List<IItem>();
        this.Name = name;
        this.TemplateName = name;
        this.Uuid = Utilities.NewUuid();
    }

    public void SetGroup(IGroup group)
    {
        this.Groups.Clear();
        this.Groups.Add(group);
    }

    public void AddItem(IItem item)
    {
        this.Items.Add(item);
    }
}
