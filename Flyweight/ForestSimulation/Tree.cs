namespace ForestSimulation;

public class Tree(int x, int y, TreeType type)
{
    public void Draw() => type.Draw(x, y);
}