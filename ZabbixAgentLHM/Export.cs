using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace ZabbixAgentLHM;

public class Export
{
    // Quote this to prevent "a character string is expected" error on import
    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public string Date { get; }

    public IList<Group> Groups { get; }

    public IList<Template> Templates { get; }

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public string Version { get; }

    public Export()
    {
        this.Date = Utilities.DateTimeUtcNow();
        this.Groups = new List<Group>();
        this.Templates = new List<Template>();
        this.Version = "6.0";
    }

    public void SetGroupByName(
        string groupName)
    {
        var group = new Group(groupName);
        group.Uuid = Utilities.NewUuid();

        this.Groups.Clear();
        this.Groups.Add(group);
    }

    public Template GetTemplate()
    {
        try
        {
            return this.Templates.First();
        }
        catch (System.InvalidOperationException)
        {
            this.SetTemplate(new Template());
        }

        return this.Templates.First();
    }

    public void SetTemplate(
        Template template)
    {
        this.Templates.Clear();
        this.Templates.Add(template);
    }
}

