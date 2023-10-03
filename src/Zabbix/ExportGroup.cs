namespace ZabbixAgentLHM.Zabbix;

public class ExportGroup : IGroup
{
    public string Name { get; }

    public string? Uuid { get; } = Guid.NewGuid().ToString().Replace("-", "");

    public ExportGroup(string name)
    {
        this.Name = name;
    }
}

