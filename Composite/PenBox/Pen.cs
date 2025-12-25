namespace PenBox;

public class Pen(string name, string color) : IInventoryItem
{
    public string Name { get; set; } = name;

    public string Color { get; set; } = color;

    public void PrintDetails(int currentDepth)
    {
        Console.WriteLine($"Pen Name: {Name}, Color: {Color}, Depth Level: {currentDepth}");
    }
}