using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace ZabbixAgentLHM.Zabbix;

public class Template
{
    private const string templateName = "Template App LibreHardwareMonitor";

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public string Name = Template.templateName;

    [YamlMember(Alias = "Template", ScalarStyle = ScalarStyle.SingleQuoted)]
    public string TemplateName = Template.templateName;

    public IList<IGroup> Groups { get; }

    public IList<IItem> Items { get; } = new List<IItem>();

    public string Uuid { get; } = Guid.NewGuid().ToString().Replace("-", "");

    public Template(string groupName)
    {
      this.Groups = new List<IGroup>() { new TemplateGroup(groupName) };
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

