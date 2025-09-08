namespace Cosmic3.command;

public struct CommandContext(string[] args, string usedAlias, Prefix usedPrefix)
{
    public readonly string[] Args = args;
    public readonly string UsedAlias = usedAlias;
    public readonly Prefix UsedPrefix = usedPrefix;
}