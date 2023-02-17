using YamlDotNet.Core;
using YamlDotNet.Serialization;

public class Export
{
    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public string? Version { get; set; } = "6.0";

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public string? Date { get; set; }

    public List<Group> Groups { get; }
    public List<Template> Templates { get; }

    public Export()
    {
        this.Date = $"{DateTime.UtcNow.ToString("s")}Z";
        this.Groups = new List<Group>();
        this.Templates = new List<Template>();
    }

    public void AddGroup(Group group)
    {
        this.Groups.Add(group);
    }

    public void AddTemplate(Template template)
    {
        this.Templates.Add(template);
    }

    // We basically have a single group and template, so make things simple
    public Group GetGroup()
    {
        return this.Groups.First();
    }

    public Template GetTemplate()
    {
        return this.Templates.First();
    }
}
