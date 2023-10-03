namespace ZabbixAgentLHM.Zabbix;

public class TemplateGroup : IGroup
{
    public string Name { get; }

    public string? Uuid { get; } = null;

    public TemplateGroup(string name)
    {
        this.Name = name;
    }
}

