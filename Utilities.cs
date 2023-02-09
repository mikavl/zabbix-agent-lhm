public static class Utilities
{
    // Return a Zabbix-style UUID v4 with dashes removed
    public static string NewUuid()
    {
        Guid uuid = Guid.NewGuid();
        string uuidString = uuid.ToString().Replace("-", "");
        return uuidString;
    }
}
