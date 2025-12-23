using Conceptual.Target;
using AdapteeClass = Conceptual.Adaptee.Adaptee;
using AdapterClass = Conceptual.Adapter.Adapter;

// Client code demonstrating the Adapter pattern
// Shows how the Adapter makes the incompatible Adaptee work with the ITarget interface
public static class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Adapter Pattern Demonstration ===\n");

        // Scenario: The client needs to work with ITarget interface
        // Problem: The Adaptee class has useful functionality but incompatible interface
        // Solution: Use an Adapter to bridge the incompatibility

        // Create the Adaptee instance
        // This represents an existing class that cannot be modified
        // (e.g., third-party library, legacy code, external service)
        var adaptee = new AdapteeClass();

        Console.WriteLine("Client: The Adaptee class has an incompatible interface:");
        Console.WriteLine($"Adaptee.GetSpecificRequest() returns: {adaptee.GetSpecificRequest()}");
        Console.WriteLine("Client cannot use this directly because it expects ITarget interface.\n");

        // Create the Adapter wrapping the Adaptee
        // The Adapter implements ITarget, making it compatible with client code
        ITarget target = new AdapterClass(adaptee);

        Console.WriteLine("Client: But with the Adapter, I can work with it via ITarget interface:");
        Console.WriteLine($"ITarget.GetRequest() returns: {target.GetRequest()}");
        Console.WriteLine("\nThe Adapter translated the incompatible interface!");

        // Demonstrate polymorphism
        Console.WriteLine("\n=== Client Code Using ITarget ===");
        ClientCode(target);
    }

    // Client method that works exclusively with ITarget interface
    // This method is unaware of the Adaptee or Adapter implementation details
    // It demonstrates that the client code remains clean and decoupled
    private static void ClientCode(ITarget target)
    {
        // Client calls the standard ITarget method
        // It doesn't know whether it's working with:
        // - A native ITarget implementation
        // - An Adapter wrapping an Adaptee
        // - A mock for testing
        Console.WriteLine($"Client: Working with ITarget interface: {target.GetRequest()}");
    }
}

/*
 * KEY TAKEAWAYS:
 *
 * ADAPTER PATTERN COMPONENTS:
 * 1. Target (ITarget): Interface expected by the client
 * 2. Adaptee (Adaptee): Existing class with incompatible interface
 * 3. Adapter (Adapter): Implements Target and wraps Adaptee to make them compatible
 * 4. Client: Works with Target interface, unaware of adaptation
 *
 * WHEN TO USE ADAPTER PATTERN:
 * - Third-party libraries with incompatible interfaces
 * - Legacy code that cannot be modified
 * - Interface mismatch between components
 * - Multiple data sources needing unified interface
 * - API versioning and compatibility
 *
 * OBJECT ADAPTER vs CLASS ADAPTER:
 * - Object Adapter (used here): Uses composition, more flexible, preferred in C#
 * - Class Adapter: Uses multiple inheritance, not supported in C#
 *
 * BENEFITS:
 * - Client code remains unchanged when adapting new classes
 * - Reuses existing classes without modification (Open/Closed Principle)
 * - Separates interface conversion from business logic (Single Responsibility)
 * - Enables integration with third-party code
 *
 * REAL-WORLD EXAMPLES:
 * - Adapting XML/JSON readers to common IDataReader interface
 * - Adapting different payment gateways to IPaymentProcessor
 * - Adapting legacy database clients to modern repository pattern
 * - Adapting third-party logging libraries to ILogger interface
 */
