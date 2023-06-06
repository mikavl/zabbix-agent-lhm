namespace ZabbixAgentLHM
{
    public class DefaultPreprocessor : IPreprocessor
    {
        public List<string> Parameters { get; }

        public string Type { get; set; }

        public DefaultPreprocessor(string key)
        {
            this.Parameters = new List<string>();
            this.Type = "JAVASCRIPT";

            this.AddParameter($"return JSON.parse(value)['{key}'];");
        }

        public void AddParameter(string parameter)
        {
            this.Parameters.Add(parameter);
        }
    }
}
