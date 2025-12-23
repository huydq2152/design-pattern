namespace Conceptual.Strategies;

// Concrete strategy implementing descending sort algorithm  
// This strategy sorts data in reverse (descending) order
public class ConcreteStrategyB : IStrategy
{
    public object DoAlgorithm(object data)
    {
        // Cast input data to List<string> and apply descending sort
        var list = data as List<string>;
        list.Sort();        // First sort in ascending order
        list.Reverse();     // Then reverse to get descending order

        return list;
    }
}