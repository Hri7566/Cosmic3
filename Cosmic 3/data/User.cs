namespace Cosmic3.data;

public class User(string id, string name, string color)
{
    public string Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string Color { get; set; } = color;

    public void Save()
    {
        
    }
}