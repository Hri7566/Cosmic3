using Cosmic3.oven;
using Cosmic3.util;

namespace Cosmic3.command;

public static class CommandHandler
{
    public static List<CommandGroup> CommandGroups = [];
    private static readonly List<Prefix> Prefixes = [];

    /// <summary>
    /// Find the correct chat command and run its callback, then return its resulting chat message as a string.
    /// </summary>
    /// <param name="text">User's chat message to parse</param>
    /// <returns>Output of the command</returns>
    public static string? HandleCommand(string text)
    {
        var args = text.Split(' ');
        
        // Find prefix that was used
        var usedPrefix = (from p in Prefixes
                where args[0].StartsWith(p.Text)
                select p).FirstOrDefault();
        
        if (usedPrefix == null) return null;

        // Strip prefix off first arg to get command to potentially run
        var usedAlias = args[0].Substring(usedPrefix.Text.Length,  args[0].Length - usedPrefix.Text.Length).Trim();
        Command? foundCommand = null;

        // Find the supposed command
        foreach (var commandGroup in CommandGroups)
        {
            foundCommand = (from c in commandGroup.Commands
                where c.Aliases.Contains(usedAlias)
                select c).FirstOrDefault();

            if (foundCommand != null) break;
        }

        if (foundCommand == null) return null;
        
        var ctx = new CommandContext(args, usedAlias, usedPrefix);

        try
        {
            // Run the command
            return foundCommand.Callback(ctx);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return "An error has occurred: " + e.Message + "\n" + e.StackTrace;
        }
    }

    /// <summary>
    /// Populate the command handler with initial data
    /// </summary>
    public static void Populate()
    {
        Prefixes.Add(new Prefix("*", false));
        Prefixes.Add(new Prefix("cosmic", true));
        
        CommandGroup general = new CommandGroup("general", "ℹ️ General");
        
        general.Add(new Command(
            ["help", "h", "commands", "cmds", "cmd"], 
            "List or get info about commands", 
            "help [command]",
            ctx =>
            {
                var prefixText = ctx.UsedPrefix.Text;
                if (ctx.UsedPrefix.Spaced) prefixText += " ";
                
                if (ctx.Args.Length == 1)
                {
                    // List all commands
                    List<string> commandGroupList = [];

                    foreach (var commandGroup in CommandGroups)
                    {
                        List<string> commandList = [];
                        commandList.AddRange(
                            from command in commandGroup.Commands
                                where command.Visible
                                select $"`{prefixText}{command.Aliases[0]}`"
                            );
                        
                        var list = string.Join(" | ", commandList);
                        if (list == "") list = "(none)";

                        commandGroupList.Add(commandGroup.Name + ": " + list);
                    }

                    return string.Join("\n", commandGroupList);
                }
                
                // Find command and send help info
                Command? foundCommand = null;

                foreach (var commandGroup in CommandGroups)
                {
                    foundCommand = commandGroup.FindCommandByAlias(ctx.Args[1]);
                    if (foundCommand != null) break;
                }

                if (foundCommand == null) return "Command \"" + ctx.Args[1] + "\" not found.";
                return "ℹ️ Description: " + foundCommand.Description + " | ⚙️ Usage: `" + prefixText + foundCommand.Usage + "`";
            }
        ));

        general.Add(new Command(
            ["about", "info"],
            "Show information about the bot",
            "about",
            ctx => "🌌 This bot was made by `@hri7566`. 💾 Source code: https://github.com/Hri7566/Cosmic3"
        ));
        
        CommandGroups.Add(general);

        CommandGroup items = new CommandGroup("items", "🎁 Items");
        
        CommandGroups.Add(items);

        CommandGroup baking = new CommandGroup("baking", "🍰 Baking");
        
        baking.Add(new Command(
            ["bake", "startbake", "startbaking"],
            "Start baking a cake",
            "bake",
            ctx =>
            {
                Oven.StartBaking();
                return "WIP no oven yet";
            }));
        
        baking.Add(new Command(
            ["stopbaking", "stopbake", "stop"],
            "Start baking a cake",
            "bake",
            ctx =>
            {
                Oven.StopBaking();
                return "WIP no oven yet";
            }));
        
        CommandGroups.Add(baking);

        var util = new CommandGroup("util", "⚙️ Utility");
        
        util.Add(new Command(
            ["uptime"],
            "Get the bot's uptime",
            "uptime",
            ctx =>
            {
                var uptime = DateTimeOffset.Now.ToUnixTimeSeconds() - Cosmic.StartTime;
                return $"Uptime: {Format.Time(uptime)}";
            }
        ));
        
        util.Add(new Command(
            ["data"],
            "Get your user data",
            "data",
            ctx =>
            {
                return "potato";
            },
            false
        ));
        
        CommandGroups.Add(util);
    }
}