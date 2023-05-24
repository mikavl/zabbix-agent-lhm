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

    [YamlIgnore]
    public Item? MasterItem { get; set; }

    public Template()
    {
        this.Groups = new List<Group>();
        this.Items = new List<Item>();
        this.Uuid = Utilities.NewUuid();
    }

    public void SetGroupByName(
        string groupName)
    {
        // Template groups should not have an UUID, so don't set one here
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

    public void SetMasterItemByNameAndKey(string name, string key)
    {
        var item = new Item();
        item.Name = name;
        item.Key = key;

        // Don't keep history or trends, as they are all in the dependent items
        item.History = 0;
        item.Trends = 0;
        item.ValueType = "TEXT";
        item.Type = "ZABBIX_ACTIVE";

        if (this.MasterItem is Item masterItem)
        {
            this.Items.Remove(masterItem);
        }

        this.MasterItem = item;
        this.AddItem(item);
    }

    public void AddItem(Item item)
    {
        this.Items.Add(item);
    }
}
