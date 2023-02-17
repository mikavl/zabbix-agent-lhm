public class Group
{
    public string? Uuid { get; set; }
    public string? Name { get; set; }

    public Group(string name, bool uuid = true)
    {
        this.Name = name;
        if (uuid == true) {
            this.Uuid = Utilities.NewUuid();
        }
    }
}
