using Cosmic3.data.models;

namespace Cosmic3.util;

public static class Stringify
{
    public static string Time(long seconds)
    {
        var h = (seconds / 3600) % 12;
        var m = (seconds / 60) % 60;
        var s = seconds % 60;
        
        var hh = h > 0 ? $"{h:D2}:" : "";
        var mm = m > 0 ? $"{m:D2}:" : "";
        var ss = s > 0 ? $"{s:D2}" : "";

        return $"{hh}{mm}{ss}";
    }

    public static string Item(Item item)
    {
        return item.Count == 1 ? item.Name : $"{item.Name} (x{item.Count})";
    }

    public static string Inventory(Inventory inventory)
    {
        List<string> result = [];
        
        foreach (var item in inventory.Items)
        {
            result.Add(Item(item));
        }

        return result.Count > 0 ? string.Join(", ", result) : "(none)";
    }

    public static string Balance(long amount)
    {
        return $"{amount} star bits";
    }
}