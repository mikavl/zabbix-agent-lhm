using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace ZabbixAgentLHM
{
    public class Template
    {
        public IList<IGroup> Groups { get; } = new List<IGroup>();

        public IList<IItem> Items { get; } = new List<IItem>();

        public string Uuid { get; } = Guid.NewGuid().ToString().Replace("-", "");

        [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
        public string Name { get; }

        [YamlMember(Alias = "Template", ScalarStyle = ScalarStyle.SingleQuoted)]
        public string TemplateName { get; }

        public Template(string name)
        {
            this.Name = name;
            this.TemplateName = name;
        }

        public void SetGroup(IGroup group)
        {
            this.Groups.Clear();
            this.Groups.Add(group);
        }

        public void AddItem(IItem item)
        {
            this.Items.Add(item);
        }
    }
}
