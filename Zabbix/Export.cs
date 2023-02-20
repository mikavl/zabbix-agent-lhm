using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Zabbix
{
    public class Export
    {
        // Quote this to prevent "a character string is expected" error on import
        [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
        public string Date { get; } = Utilities.DateTimeUtcNow();

        public IList<Group> Groups { get; } = new List<Group>();

        public IList<Template> Templates { get; } = new List<Template>();

        [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
        public string Version { get; } = "6.0";
    }
}
