using YamlDotNet.Core;
using YamlDotNet.Serialization;

public class Template
{
    public string? Uuid { get; set; }

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public string? Name { get; set; }

    [YamlMember(Alias = "template", ScalarStyle = ScalarStyle.SingleQuoted)]
    public string? TemplateName { get; set; }

    public List<Group> Groups { get; }
    public List<Item> Items { get; set; }

    public Template()
    {
        this.Groups = new List<Group>();
        this.Items = new List<Item>();
    }

    public Template(string name)
    {
        this.Groups = new List<Group>();
        this.Items = new List<Item>();

        this.Name = name;
        this.TemplateName = name;
        this.Uuid = Utilities.NewUuid();
    }

    public void AddGroup(Group group)
    {
        this.Groups.Add(group);
    }

    public void AddItem(Item item)
    {
        this.Items.Add(item);
    }
}
