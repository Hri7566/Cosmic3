using Cosmic3.data;
using Cosmic3.data.models;
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
    /// <param name="p">User's participant data</param>
    /// <param name="text">User's chat message to parse</param>
    /// <returns>Output of the command</returns>
    public static async Task<string?> HandleCommand(Participant p, string text)
    {
        var platformId = Cosmic.platform + "." + p.Id;
        var args = text.Split(' ');
        
        // Find prefix that was used
        var usedPrefix = (from pre in Prefixes
                where args[0].StartsWith(pre.Text)
                select pre).FirstOrDefault();
        
        if (usedPrefix == null) return null;

        string usedAlias;
        
        if (!usedPrefix.Spaced)
        {
            // If prefix is not spaced, strip prefix off first arg to get command to potentially run
            usedAlias = args[0].Substring(usedPrefix.Text.Length, args[0].Length - usedPrefix.Text.Length).Trim();
        }
        else
        {
            // Otherwise, skip the first argument entirely
            args = args.Skip(1).ToArray();
            usedAlias = args[0];
        }

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

        // Get/create user data
        var user = await User.Read(platformId);

        if (user == null)
        {
            user = new User()
            {
                Name = p.Name,
                PlatformId = platformId,
                Color = p.Color
            };
            
            await User.Create(user);
        }

        user.Name = p.Name;
        user.Color = p.Color;
        
        // Create context object to be passed to command
        var ctx = new CommandContext(args, usedAlias, usedPrefix, user);

        try
        {
            // Run the command
            var output = await foundCommand.Callback(ctx);
            await User.Update(user);
            return output;
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
            async ctx =>
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
            async ctx => "🌌 This bot was made by `@hri7566`. 💾 Source code: https://github.com/Hri7566/Cosmic3"
        ));
        
        CommandGroups.Add(general);

        CommandGroup items = new CommandGroup("items", "🎁 Items");
        
        CommandGroups.Add(items);
        
        items.Add(new Command(
            ["inventory", "inv", "items", "item", "i", "bal"],
            "View your inventory",
            "inventory",
            async ctx =>
            {
                var inventory = await Inventory.Pick(ctx.user);
                return inventory == null ? "No inventory available." : $"Balance: {Stringify.Balance(inventory.Balance)} | Items: {Stringify.Inventory(inventory)}";
            }));

        CommandGroup baking = new CommandGroup("baking", "🍰 Baking");
        
        baking.Add(new Command(
            ["bake", "startbake", "startbaking"],
            "Start baking a cake",
            "bake",
            async ctx =>
            {
                Oven.StartBaking();
                return "WIP no oven yet";
            }));
        
        baking.Add(new Command(
            ["stopbaking", "stopbake", "stop"],
            "Start baking a cake",
            "bake",
            async ctx =>
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
            async ctx =>
            {
                var uptime = DateTimeOffset.Now.ToUnixTimeSeconds() - Cosmic.StartTime;
                return $"Uptime: {Stringify.Time(uptime)}";
            }
        ));
        
        util.Add(new Command(
            ["data"],
            "Get your user data",
            "data",
            async ctx => "User data: " +
                   "Username: " + ctx.user.Name + " | " +
                   "User ID: " + ctx.user.UserId + " | " +
                   "Platform ID: " + ctx.user.PlatformId + " | " +
                   "Color: " + ctx.user.Color + " | " +
                   "Inventory ID: " + ctx.user.Inventory.InventoryId,
            false
        ));
        
        CommandGroups.Add(util);
    }
}