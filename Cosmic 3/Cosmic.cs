using Cosmic3.command;

namespace Cosmic3;

public static class Cosmic
{
    public static void Initialize()
    {
        CommandHandler.Populate();
    }
    
    public static string? RunCommand(string message)
    {
        return CommandHandler.HandleCommand(message);
    }
}