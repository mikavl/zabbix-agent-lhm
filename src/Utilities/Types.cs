namespace ZabbixAgentLHM.Utilities;

public static class Types
{
    public static T[] FromString<T>(string s) where T : struct, System.Enum
    {
        T type;
        var types = new List<T>();

        foreach (var typeString in s.Split(","))
        {
            if (typeString.ToLower().Equals("all"))
            {
                foreach (T t in Enum.GetValues(typeof(T)))
                {
                    types.Add(t);
                }
            }
            else if (Enum.TryParse<T>(typeString, true, out type))
            {
                types.Add(type);
            }
            else
            {
                throw new System.Exception($"Unknown type {typeString}");
            }
        }

        return types.ToArray();
    }
}
