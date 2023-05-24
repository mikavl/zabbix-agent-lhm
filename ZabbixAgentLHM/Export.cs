using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace ZabbixAgentLHM;

public class Export
{
    // Quote this to prevent "a character string is expected" error on import
    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public string Date { get; } = Utilities.DateTimeUtcNow();

    public IList<Group> Groups { get; } = new List<Group>();

    public IList<Template> Templates { get; } = new List<Template>();

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public string Version { get; } = "6.0";

    public Export()
    {
        this.Templates.Add(new Template());
    }

    public void SetGroup(
        string groupName)
    {
        var group = new Group(groupName);
        group.Uuid = Utilities.NewUuid();

        this.Groups.Clear();
        this.Groups.Add(group);
    }

    public void SetTemplate(
        Template template)
    {
        this.Templates.Clear();
        this.Templates.Add(template);
    }
}

