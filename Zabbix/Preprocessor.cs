namespace Zabbix
{
    public class Preprocessor
    {
        public List<string> Parameters { get; } = new List<string>();

        public string Type { get; set; } = "JAVASCRIPT";
    }
}
