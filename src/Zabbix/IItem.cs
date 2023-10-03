namespace ZabbixAgentLHM.Zabbix;

public interface IItem
{
    public string Key { get; }

    public string Name { get; }

    public string Type { get; }

    public string Uuid { get; }

    public string ValueType { get; }

    public int? Delay { get; }

    public int? History { get; }

    public int? Trends { get; }

    public string? Units { get; }

    public float? Value { get; }
}

