using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace ZabbixAgentLHM
{
    public class MasterItem : IItem
    {
        public string Key { get; } = "lhm.gather";

        public string Name { get; } = "LibreHardwareMonitor";

        public string Type { get; } = "ZABBIX_ACTIVE";

        public string Uuid { get; } = Guid.NewGuid().ToString().Replace("-", "");

        public string ValueType { get; } = "TEXT";

        [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
        public int? Delay { get; } = 60;

        [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
        public int? History { get; } = 0;

        [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
        public int? Trends { get; } = 0;

        public string? Units { get; } = null;

        public float? Value { get; } = null;
    }
}
