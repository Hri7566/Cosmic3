using System.Runtime.CompilerServices;
using Cosmic3.command;
using Cosmic3.data;

namespace Cosmic3;

public static class Cosmic
{
    public static readonly long StartTime = DateTimeOffset.Now.ToUnixTimeSeconds();
    public static string platform;
    
    public static void Initialize(string platform)
    {
        CosmicDbContext.EnsureCreated();
        CommandHandler.Populate();
        Cosmic.platform = platform;
    }
    
    public static Task<string?> RunCommand(Participant p, string message)
    {
        return CommandHandler.HandleCommand(p, message);
    }
}
