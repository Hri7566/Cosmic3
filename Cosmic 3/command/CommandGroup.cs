namespace Cosmic3.command;

public class CommandGroup(string id, string name)
{
    public string Id = id;
    public string Name = name;
    public List<Command> Commands = new List<Command>();

    public void Add(Command command)
    {
        Commands.Add(command);
    }

    public Command? FindCommandByAlias(string alias)
    {
        return (from c in Commands
                where c.Aliases.Contains(alias)
                select c).FirstOrDefault();
    }
}