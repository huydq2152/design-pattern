using Conceptual.Abstractions;
using Conceptual.Implementations;

// Client code demonstrating the Bridge pattern
// Shows how abstractions and implementations can vary independently
// The same abstraction can work with different implementations, and vice versa
public static class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Bridge Pattern Demonstration ===\n");

        // The Bridge pattern allows you to combine any abstraction with any implementation
        // This prevents a combinatorial explosion of classes
        // Without Bridge: Need classes for every combination (4 classes: BaseA, BaseB, ExtendedA, ExtendedB)
        // With Bridge: Need abstraction classes + implementation classes (2 + 2 = 4 total)

        Console.WriteLine("--- Example 1: Base Abstraction with Implementation A ---");
        // Combine base abstraction with implementation A
        // The abstraction provides high-level logic, implementation A provides platform-specific behavior
        Abstraction abstraction = new Abstraction(new ConcreteImplementationA());
        ClientCode(abstraction);

        Console.WriteLine("\n--- Example 2: Base Abstraction with Implementation B ---");
        // Same abstraction, different implementation
        // Demonstrates that abstractions can work with any implementation
        abstraction = new Abstraction(new ConcreteImplementationB());
        ClientCode(abstraction);

        Console.WriteLine("\n--- Example 3: Extended Abstraction with Implementation A ---");
        // Different abstraction, same implementation A
        // Demonstrates that implementations can work with any abstraction
        abstraction = new ExtendedAbstraction(new ConcreteImplementationA());
        ClientCode(abstraction);

        Console.WriteLine("\n--- Example 4: Extended Abstraction with Implementation B ---");
        // Extended abstraction with implementation B
        // Shows complete flexibility: any abstraction with any implementation
        abstraction = new ExtendedAbstraction(new ConcreteImplementationB());
        ClientCode(abstraction);

        Console.WriteLine("\n=== Key Benefit: Independence ===");
        Console.WriteLine("- Abstractions can be extended without modifying implementations");
        Console.WriteLine("- Implementations can be added without modifying abstractions");
        Console.WriteLine("- Any abstraction works with any implementation");
    }

    // Client method that works with the Abstraction
    // This method doesn't know or care which specific implementation is being used
    // It works exclusively through the abstraction interface
    //
    // This demonstrates the decoupling achieved by the Bridge pattern:
    // - Client code is independent of implementation details
    // - Can easily switch implementations without changing client code
    private static void ClientCode(Abstraction abstraction)
    {
        // Client calls high-level operation on the abstraction
        // The abstraction delegates to the implementation through the bridge
        // This produces different results based on which implementation is connected
        Console.Write(abstraction.Operation());
    }
}

/*
 * KEY TAKEAWAYS:
 *
 * BRIDGE PATTERN COMPONENTS:
 * 1. Implementation (IImplementation): Interface for low-level operations
 * 2. Concrete Implementations (A, B): Platform-specific or variant-specific behavior
 * 3. Abstraction: High-level interface that uses Implementation through a bridge reference
 * 4. Refined Abstraction (Extended): Extended high-level interface built on same bridge
 *
 * THE BRIDGE CONCEPT:
 * - The "bridge" is the reference from Abstraction to Implementation
 * - This composition-based connection allows both hierarchies to vary independently
 * - Changes in one hierarchy don't affect the other
 *
 * WHEN TO USE BRIDGE PATTERN:
 * - Two dimensions that vary independently (abstraction types × implementation types)
 * - Avoiding class explosion (N abstractions × M implementations = N×M classes)
 * - Platform independence (UI controls that work across Windows, Linux, macOS)
 * - Runtime implementation switching (change platform without changing abstraction)
 * - Sharing implementations across abstractions
 *
 * BRIDGE vs ADAPTER:
 * - Bridge: Designed upfront to separate abstraction from implementation
 * - Adapter: Applied after to make incompatible interfaces work together
 * - Bridge: Both sides are designed to work together through the bridge
 * - Adapter: Adapts existing incompatible code
 *
 * REAL-WORLD EXAMPLES:
 * - RemoteControl (abstraction) + Device (implementation)
 * - Shape (abstraction) + Renderer (implementation)
 * - Notification (abstraction) + Channel (implementation)
 * - Repository (abstraction) + Database (implementation)
 *
 * BENEFITS:
 * - Reduces number of classes (prevents combinatorial explosion)
 * - Open/Closed Principle: Extend abstractions and implementations separately
 * - Single Responsibility: Abstractions focus on high-level logic, implementations on platform details
 * - Runtime flexibility: Can change implementation dynamically
 * - Platform independence: Write abstraction once, run on any platform
 */
