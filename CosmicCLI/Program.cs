using Cosmic3;
using System.Text;
using Cosmic3.data;
using Cosmic3.data.models;

Console.OutputEncoding = Encoding.UTF8;
Cosmic.Initialize("cli");

while (true)
{
    Console.Write("c> ");
    string? text = Console.ReadLine();
    if (text == null) continue;

    Participant p = new()
    {
        Id = "cli",
        Name = "CLI",
        Color = "#8d3f50"
    };

    var output = Cosmic.RunCommand(p, text);
    Console.WriteLine(output);
}
