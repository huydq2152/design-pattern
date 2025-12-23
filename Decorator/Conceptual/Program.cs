using Conceptual.Components;
using Conceptual.Decorators;

// Client code demonstrating the Decorator pattern
// Shows how to add responsibilities dynamically by wrapping objects
public static class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Decorator Pattern Demonstration ===\n");

        // Example 1: Simple component without decoration
        // Demonstrates the base functionality before any enhancement
        Console.WriteLine("--- Example 1: Simple Component ---");
        var simple = new ConcreteComponent();
        Console.WriteLine("Client: Working with a simple component:");
        ClientCode(simple);

        // Example 2: Single decorator
        // Demonstrates wrapping with one decorator
        Console.WriteLine("\n--- Example 2: Single Decorator ---");
        var decoratedOnce = new ConcreteDecoratorA(simple);
        Console.WriteLine("Client: Component decorated with DecoratorA:");
        ClientCode(decoratedOnce);

        // Example 3: Multiple decorators (decorator stacking)
        // Demonstrates that decorators can wrap other decorators
        // This is the power of the pattern: unlimited behavior composition
        Console.WriteLine("\n--- Example 3: Stacked Decorators ---");

        // Build decorator chain: DecoratorB wraps DecoratorA wraps ConcreteComponent
        // Execution flow: DecoratorB -> DecoratorA -> ConcreteComponent
        // Result: "DecoratorB(DecoratorA(ConcreteComponent))"
        var decorator1 = new ConcreteDecoratorA(simple);
        var decorator2 = new ConcreteDecoratorB(decorator1);
        Console.WriteLine("Client: Component wrapped with DecoratorA and DecoratorB:");
        ClientCode(decorator2);

        // Example 4: Different decorator order
        // Demonstrates that order matters in decorator chains
        Console.WriteLine("\n--- Example 4: Different Decorator Order ---");

        // Reverse order: DecoratorA wraps DecoratorB wraps ConcreteComponent
        // Execution flow: DecoratorA -> DecoratorB -> ConcreteComponent
        // Result: "DecoratorA(DecoratorB(ConcreteComponent))"
        var decoratorB = new ConcreteDecoratorB(simple);
        var decoratorA = new ConcreteDecoratorA(decoratorB);
        Console.WriteLine("Client: Component wrapped with DecoratorB and DecoratorA (reversed):");
        ClientCode(decoratorA);

        // Example 5: Multiple instances of same decorator
        // Demonstrates that the same decorator type can be applied multiple times
        Console.WriteLine("\n--- Example 5: Multiple Instances of Same Decorator ---");

        // Apply DecoratorA twice
        var doubleA1 = new ConcreteDecoratorA(simple);
        var doubleA2 = new ConcreteDecoratorA(doubleA1);
        Console.WriteLine("Client: Component with DecoratorA applied twice:");
        ClientCode(doubleA2);
    }

    // Client method that works with Component interface
    // Key point: Client doesn't know or care if it's working with:
    // - A simple ConcreteComponent
    // - A decorated component
    // - A chain of decorators
    //
    // This demonstrates the transparency of the Decorator pattern
    // All components are treated uniformly through the Component interface
    private static void ClientCode(Component component)
    {
        // Simply call Operation() - the component handles everything
        // - If it's ConcreteComponent: returns base result
        // - If it's a Decorator: adds behavior and delegates to wrapped component
        // - If it's multiple Decorators: each adds its behavior in sequence
        Console.WriteLine($"RESULT: {component.Operation()}\n");
    }
}

/*
 * KEY TAKEAWAYS:
 *
 * DECORATOR PATTERN COMPONENTS:
 * 1. Component: Interface/abstract class for both concrete components and decorators
 * 2. ConcreteComponent: Object being decorated, provides base functionality
 * 3. Decorator: Base class that wraps a Component and delegates to it
 * 4. ConcreteDecorators (A, B): Add specific behaviors by wrapping components
 *
 * HOW DECORATOR WORKS:
 * - Decorators implement the same interface as the component they wrap
 * - Decorators contain a reference to a Component (composition)
 * - Decorators add behavior before/after delegating to wrapped component
 * - Multiple decorators can be stacked (decorator wrapping decorator)
 *
 * DECORATOR STACKING:
 * - DecoratorB(DecoratorA(Component)) creates a chain
 * - When Operation() is called on DecoratorB:
 *   1. DecoratorB adds its behavior
 *   2. Calls wrapped component (DecoratorA)
 *   3. DecoratorA adds its behavior
 *   4. Calls wrapped component (ConcreteComponent)
 *   5. ConcreteComponent returns base result
 *   6. Results bubble up through the chain
 *
 * WHEN TO USE DECORATOR PATTERN:
 * - Adding responsibilities dynamically at runtime
 * - Avoiding subclass explosion (too many feature combinations)
 * - Optional features that can be selectively added
 * - Cross-cutting concerns (logging, caching, validation, retry)
 * - Extending functionality of third-party classes
 *
 * DECORATOR vs INHERITANCE:
 * - Inheritance: Static, decided at compile-time, creates rigid hierarchies
 * - Decorator: Dynamic, decided at runtime, flexible composition
 * - Example: Instead of creating classes for every combination
 *   (BasicCoffee, CoffeeWithMilk, CoffeeWithSugar, CoffeeWithMilkAndSugar)
 *   Use: Coffee + MilkDecorator + SugarDecorator (composable)
 *
 * ORDER MATTERS:
 * - Decorator order affects behavior
 * - Example: Logging + Caching
 *   - Logging(Caching(Service)): Logs all calls including cache hits
 *   - Caching(Logging(Service)): Only logs cache misses
 * - Document expected decorator order
 *
 * REAL-WORLD EXAMPLES:
 * - .NET Streams: BufferedStream, GZipStream, CryptoStream wrap base Stream
 * - ASP.NET Middleware: Each middleware decorates the request pipeline
 * - Repository pattern: CachingRepository, LoggingRepository wrap base Repository
 * - Services: RetryService, CircuitBreakerService wrap base Service
 *
 * BENEFITS:
 * - Open/Closed Principle: Add new decorators without modifying existing code
 * - Single Responsibility: Each decorator has one clear purpose
 * - Flexible composition: Mix and match decorators as needed
 * - Runtime configuration: Choose decorators at runtime based on config
 *
 * TRANSPARENCY:
 * - Client treats all components uniformly (doesn't check types)
 * - Decorators are invisible to clients (substitutable for components)
 * - Allows unlimited decorator stacking
 *
 * TRADE-OFFS:
 * - Complexity: Many small decorator classes
 * - Order dependency: Must understand correct decorator order
 * - Debugging: Stack traces become longer with multiple decorators
 * - Identity: Decorated object != original object (type checking issues)
 */
