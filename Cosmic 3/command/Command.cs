namespace Cosmic3.command;

public class Command(string[] aliases, string description, string usage, Func<CommandContext, string> callback)
{
    public readonly string[] Aliases = aliases;
    public readonly string Description = description;
    public readonly string Usage = usage;
    public readonly Func<CommandContext, string> Callback = callback;
    public bool Visible = true;
}