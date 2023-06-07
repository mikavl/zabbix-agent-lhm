namespace ZabbixAgentLHM
{
    public class MasterItem : IItem
    {
        public string Key { get; } = "lhm.gather";

        public string Name { get; } = "LibreHardwareMonitor";

        public string Type { get; } = "ZABBIX_ACTIVE";

        public string Uuid { get; } = Utilities.NewUuid();

        public string ValueType { get; } = "TEXT";

        public int? Delay { get; } = 0;

        public int? History { get; } = 0;

        public int? Trends { get; } = 0;

        public string? Units { get; } = null;

        public float? Value { get; } = null;
    }
}
