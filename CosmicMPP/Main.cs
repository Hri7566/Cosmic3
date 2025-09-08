// See https://aka.ms/new-console-template for more information

using System.Text;
using Cosmic3;
using CosmicMPP.mpp;
using Newtonsoft.Json.Linq;

Console.OutputEncoding = Encoding.UTF8;
Cosmic.Initialize();

var cl = new Client(new Uri("wss://mppclone.com"), Environment.GetEnvironmentVariable("MPPNET_TOKEN"));

var user = new JObject()
{
    ["name"] = "Cosmic",
    ["color"] = "#1d0054"
};

cl.On("hi", async void (msg) =>
{
    try
    {
        Console.WriteLine("Connected to server");

        if (msg["u"]?["name"] != user["name"] || msg["u"]?["color"] != user["color"])
        {
            await cl.SendArray([new JObject()
            {
                ["m"] = "userset",
                ["set"] = user
            }]).ConfigureAwait(false);
        }

        await cl.SetChannel("cheez").ConfigureAwait(false);
    }
    catch (Exception e)
    {
        Console.WriteLine("Error in hi message: " + e);
    }
});

cl.On("a", async void (msg) =>
{
    try
    {
        var message = msg["a"]?.ToString();
        var id = msg["p"]?["id"]?.ToString();
        var name =  msg["p"]?["name"]?.ToString();
        
        if (message == null) return;
        if (id == null) return;
        if  (name == null) return;
        
        Console.WriteLine($"{id[..6]} {name} {message}");
        
        var output = Cosmic.RunCommand(message);
        if (output == null) return;
        await cl.SendChat(output).ConfigureAwait(false);
    }
    catch (Exception e)
    {
        Console.WriteLine("Error in chat message handler: " + e);
    }
});

cl.On("ch", async void (msg) =>
{
    try
    {
        var id = msg["ch"]?["_id"]?.ToString();
        if (id == null) return;
        
        Console.WriteLine("Connected to channel " + id);
        //await cl.SendChat("test").ConfigureAwait(false);
    }
    catch (Exception e)
    {
        Console.WriteLine("Error in channel event handler: " + e);
    }
});

await Task.Run(() => cl.Start()).ConfigureAwait(false);
