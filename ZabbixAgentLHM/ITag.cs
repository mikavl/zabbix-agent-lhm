namespace ZabbixAgentLHM
{
    public interface ITag
    {
        // Name of the tag. Use "Tag" instead of "Name" to get rid of YamlMember alias.
        public string Tag { get; set; }

        public string Value { get; set; }
    }
}
