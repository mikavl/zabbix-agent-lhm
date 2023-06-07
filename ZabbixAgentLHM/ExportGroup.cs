namespace ZabbixAgentLHM
{
    public class ExportGroup : IGroup
    {
        public string Name { get; }

        public string? Uuid { get; } = Utilities.NewUuid();

        public ExportGroup(string name)
        {
            this.Name = name;
        }
    }
}
