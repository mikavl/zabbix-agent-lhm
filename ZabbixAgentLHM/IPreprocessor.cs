namespace ZabbixAgentLHM
{
    public interface IPreprocessor
    {
        public List<string> Parameters { get; }

        public string Type { get; set; }

        public void AddParameter(string parameter);
    }
}
