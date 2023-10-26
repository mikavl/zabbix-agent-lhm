namespace ZabbixAgentLHM.Zabbix;

public class DefaultPreprocessor : IPreprocessor
{
    public List<string> Parameters { get; }

    public string Type { get; } = "JSONPATH";

    public DefaultPreprocessor(string key)
    {
        this.Parameters = new List<string>() { $"$.[\"{key}\"]" };
    }
}
