namespace PenBox;

public class Box(string name) : IInventoryItem
{
    public string Name { get; set; } = name;

    private readonly List<IInventoryItem> _inventoryItems = new();

    public void Add(IInventoryItem inventoryItem)
    {
        _inventoryItems.Add(inventoryItem);
    }

    public void PrintDetails(int currentDepth)
    {
        foreach (var inventoryItem in _inventoryItems)
        {
            if(inventoryItem is Box box)
            {
                Console.WriteLine($"Box Name: {box.Name}, Depth Level: {currentDepth}");
            }
            inventoryItem.PrintDetails(currentDepth + 1);
        }
    }
}