namespace ZabbixAgentLHM
{
    public class DefaultPreprocessor : IPreprocessor
    {
        public List<string> Parameters { get; } = new List<string>();

        public string Type { get; } = "JAVASCRIPT";

        public DefaultPreprocessor(string key)
        {
            this.Parameters.Add($"return JSON.parse(value)['{key}'];");
        }
    }
}
