namespace ZabbixAgentLHM;

public class Preprocessor
{
    public List<string> Parameters { get; }

    public string Type { get; set; }

    public Preprocessor()
    {
        this.Parameters = new List<string>();

        // All the items use the Javascript preprocessor
        this.Type = "JAVASCRIPT";
    }
}
