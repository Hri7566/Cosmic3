using Cosmic3;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Cosmic.Initialize();

while (true)
{
    Console.Write("c> ");
    string? text = Console.ReadLine();
    if (text == null) continue;
    var output = Cosmic.RunCommand(text);
    Console.WriteLine(output);
}
