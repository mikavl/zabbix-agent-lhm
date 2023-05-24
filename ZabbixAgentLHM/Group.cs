namespace ZabbixAgentLHM;

public class Group
{
    public string? Name { get; set; }

    public string? Uuid { get; set; }

    public Group(string name)
    {
        this.Name = name;
    }
}
