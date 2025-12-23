using Conceptual.Strategies;

namespace Conceptual.Context;

// Context class maintains a reference to one of the concrete strategies
// and delegates the algorithm execution to the strategy object
// This allows changing algorithms at runtime without modifying the context
public class Context
{
    // Private field holding the current strategy
    // Strategy can be changed at runtime through SetStrategy method
    private IStrategy _strategy;

    // Default constructor - strategy must be set later
    public Context()
    {
    }

    // Constructor with initial strategy injection
    // Allows setting strategy at object creation time
    public Context(IStrategy strategy)
    {
        _strategy = strategy;
    }

    // Allows changing strategy at runtime (Strategy pattern's key feature)
    // This enables dynamic algorithm switching without object recreation
    public void SetStrategy(IStrategy strategy)
    {
        _strategy = strategy;
    }

    // Business logic method that delegates algorithm execution to the strategy
    // Context doesn't know how the algorithm works, it just calls the strategy
    public void DoSomeBusinessLogic()
    {
        Console.WriteLine("Context: Sorting data using the strategy (not sure how it'll do it)");
        
        // Delegate algorithm execution to the current strategy
        var result = _strategy.DoAlgorithm(new List<string> { "a", "b", "c", "d", "e" });

        // Format and display results
        var resultStr = string.Empty;
        foreach (var element in (result as List<string>)!)
        {
            resultStr += element + ",";
        }

        Console.WriteLine(resultStr);
    }
}