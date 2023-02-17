public static class Utilities
{
    public const string MasterItemName = "LibreHardwareMonitor";
    public const string MasterItemSubkey = "gather";
    public const string MasterItemValueType = "TEXT";
    public const string MasterItemType = "ZABBIX_ACTIVE";

    // Return a Zabbix-style UUID v4 with dashes removed
    public static string NewUuid()
    {
        Guid uuid = Guid.NewGuid();
        string uuidString = uuid.ToString().Replace("-", "");
        return uuidString;
    }

    // Return the master item
    public static Item NewMasterItem(string prefix)
    {
        Item item = new Item();
        item.Name = MasterItemName;
        item.Key = $"{prefix}.{MasterItemSubkey}";
        item.History = 0;
        item.Trends = 0;
        item.ValueType = MasterItemValueType;
        item.Type = MasterItemType;
        return item;
    }

}
