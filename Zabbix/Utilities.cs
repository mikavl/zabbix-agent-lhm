using System.Text.RegularExpressions;

namespace Zabbix
{
    public static class Utilities
    {
        public static string DateTimeUtcNow()
        {
            return $"{DateTime.UtcNow.ToString("s")}Z";
        }

        public static string NewUuid()
        {
            var uuid = Guid.NewGuid();
            return uuid.ToString().Replace("-", "");
        }
    }
}
