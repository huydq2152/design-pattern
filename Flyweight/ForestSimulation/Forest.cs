namespace ForestSimulation;

public class Forest
{
    private List<Tree> _trees = new();

    public void PlantTree(int x, int y, string name, string color, byte[] texture)
    {
        var type = TreeFactory.GetTreeType(name, color, texture);
        var tree = new Tree(x, y, type);
        _trees.Add(tree);
    }

    public void Draw()
    {
        foreach (var tree in _trees) tree.Draw();
    }
}