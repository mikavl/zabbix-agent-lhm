using YamlDotNet.Core;
using YamlDotNet.Serialization;

public class Preprocessor
{
    public string? Type { get; set; }

    public List<string> Parameters { get; set; }

    public Preprocessor()
    {
        this.Parameters = new List<string>();
    }
}
