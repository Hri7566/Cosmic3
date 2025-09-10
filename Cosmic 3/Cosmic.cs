using Cosmic3.command;

namespace Cosmic3;

public static class Cosmic
{
    public static readonly long StartTime = DateTimeOffset.Now.ToUnixTimeSeconds();
    
    public static void Initialize()
    {
        CommandHandler.Populate();
    }
    
    public static string? RunCommand(string message)
    {
        return CommandHandler.HandleCommand(message);
    }
}