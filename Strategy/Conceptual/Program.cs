using Conceptual.Context;
using Conceptual.Strategies;

// Client code demonstrating the Strategy pattern
// Shows how different algorithms can be plugged in at runtime
public static class Program
{
    public static void Main()
    {
        // Example 1: Setting strategy after context creation
        var context1 = new Context();

        Console.WriteLine("Client: Strategy is set to normal sorting.");
        context1.SetStrategy(new ConcreteStrategyA());
        context1.DoSomeBusinessLogic();

        Console.WriteLine();

        // Example 2: Changing strategy at runtime (key feature of Strategy pattern)
        Console.WriteLine("Client: Strategy is set to reverse sorting.");
        context1.SetStrategy(new ConcreteStrategyB());
        context1.DoSomeBusinessLogic();

        // Example 3: Setting strategy at construction time
        Console.WriteLine("Client: Strategy is set to normal sorting.");
        var context2 = new Context(new ConcreteStrategyA());
        context2.DoSomeBusinessLogic();
        
        Console.WriteLine();

        // Example 4: Different context with different strategy
        Console.WriteLine("Client: Strategy is set to reverse sorting.");
        var context3 = new Context(new ConcreteStrategyB());
        context3.DoSomeBusinessLogic();
    }
}