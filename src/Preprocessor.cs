using YamlDotNet.Core;
using YamlDotNet.Serialization;

public class Preprocessor
{
    public string? Type { get; set; }

    public List<string> Parameters { get; }

    public Preprocessor()
    {
        this.Parameters = new List<string>();
    }

    public void AddParameter(string parameter)
    {
        this.Parameters.Add(parameter);
    }
}
