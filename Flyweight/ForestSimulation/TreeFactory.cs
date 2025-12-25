namespace ForestSimulation;

public class TreeFactory
{
    private static Dictionary<string, TreeType> _treeTypes = new();
    
    public static TreeType GetTreeType(string name, string color, byte[] texture)
    {
        string key = $"{name}_{color}";
        if (!_treeTypes.ContainsKey(key))
        {
            _treeTypes[key] = new TreeType(name, color, texture);
            Console.WriteLine($"--- Created NEW TreeType: {key} ---");
        }
        return _treeTypes[key];
    }
}