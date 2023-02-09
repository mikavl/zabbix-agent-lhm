using YamlDotNet.Core;
using YamlDotNet.Serialization;

public class ZabbixExport
{
    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public string? Version { get; set; }

    [YamlMember(ScalarStyle = ScalarStyle.SingleQuoted)]
    public string? Date { get; set; }

    public List<Group> Groups { get; set; }

    public List<Template> Templates { get; set; }

    public ZabbixExport()
    {
        this.Groups = new List<Group>();
        this.Templates = new List<Template>();
    }
}
