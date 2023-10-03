namespace ZabbixAgentLHM.Zabbix;

public interface IPreprocessor
{
    public List<string> Parameters { get; }

    public string Type { get; }
}

