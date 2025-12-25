namespace ForestSimulation;

public class TreeType(string name, string color, byte[] texture)
{
    public string Name { get; } = name;
    public string Color { get; } = color;
    public byte[] Texture { get; } = texture; // Assume texture is a byte array representing image data, it is heavy, so we share it

    public void Draw(int x, int y)
    {
        Console.WriteLine($"Drawing {Name} ({Color}) at [{x}, {y}]");
    }
}