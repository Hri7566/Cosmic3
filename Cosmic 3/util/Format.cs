namespace Cosmic3.util;

public static class Format
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
}