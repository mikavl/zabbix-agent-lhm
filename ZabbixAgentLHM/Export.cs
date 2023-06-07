using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace ZabbixAgentLHM
{
    public class Export
    {
        // Quote this to prevent "a character string is expected" error on import
        [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
        public string Date { get; } = $"{DateTime.UtcNow.ToString("s")}Z";

        public IList<IGroup> Groups { get; } = new List<IGroup>();

        public IList<Template> Templates { get; } = new List<Template>();

        [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
        public string Version { get; } = "6.0";

        public void AddItem(IItem item)
        {
            this.GetTemplate().AddItem(item);
        }

        public void SetGroup(IGroup group)
        {
            this.Groups.Clear();
            this.Groups.Add(group);
        }

        public Template GetTemplate()
        {
            // Will throw if the list is empty, but it won't be *knocks wood*
            return this.Templates.First();
        }

        public void SetTemplate(Template template)
        {
            this.Templates.Clear();
            this.Templates.Add(template);
        }
    }
}
