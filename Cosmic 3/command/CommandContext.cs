using Cosmic3.data.models;

namespace Cosmic3.command;

public struct CommandContext(string[] args, string usedAlias, Prefix usedPrefix, User user)
{
    public readonly string[] Args = args;
    public readonly string UsedAlias = usedAlias;
    public readonly Prefix UsedPrefix = usedPrefix;
    public readonly User user = user;
}