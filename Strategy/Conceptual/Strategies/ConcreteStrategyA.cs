namespace Conceptual.Strategies;

// Concrete strategy implementing ascending sort algorithm
// This strategy sorts data in normal (ascending) order
public class ConcreteStrategyA : IStrategy
{
    public object DoAlgorithm(object data)
    {
        // Cast input data to List<string> and apply ascending sort
        var list = data as List<string>;
        list.Sort(); // Built-in ascending sort

        return list;
    }
}