using Conceptual.Builders;
using Conceptual.Director;

// Client code demonstrating the Builder pattern
// Shows three ways to use builders:
// 1. Through Director for common configurations
// 2. Through Director for full-featured products
// 3. Direct builder usage for custom configurations (without Director)
public static class Program
{
    public static void Main()
    {
        // Create Director to orchestrate the build process
        var director = new Director();

        // Create a concrete builder instance
        // The builder will construct products according to the build steps called
        var builder = new ConcreteBuilder1();

        // Assign the builder to the director
        // The director will use this builder to execute construction sequences
        director.Builder = builder;

        // Example 1: Build minimal product using Director
        // Director calls only BuildPartA(), creating a simple product variant
        Console.WriteLine("Minimal product (only Part A):");
        director.BuildMinimalProduct();
        Console.WriteLine(builder.GetProduct().GetParts());

        // Example 2: Build full-featured product using Director
        // Director calls BuildPartA(), BuildPartB(), and BuildPartC() in sequence
        // This demonstrates how Director encapsulates common construction logic
        Console.WriteLine("Full-featured product (all parts):");
        director.BuildFullFeaturedProduct();
        Console.WriteLine(builder.GetProduct().GetParts());

        // Example 3: Custom product without Director
        // Client code directly controls which parts to build
        // This demonstrates the flexibility of the Builder pattern
        // The Director is optional - clients can bypass it when they need custom configurations
        Console.WriteLine("Custom product (Parts A and C only):");
        builder.BuildPartA();
        builder.BuildPartC();
        Console.WriteLine(builder.GetProduct().GetParts());
    }
}

/*
 * KEY TAKEAWAYS:
 *
 * BUILDER PATTERN COMPONENTS:
 * 1. Product: The complex object being constructed (Product class)
 * 2. Builder Interface: Declares construction steps (IBuilder)
 * 3. Concrete Builder: Implements construction steps and tracks the product (ConcreteBuilder1)
 * 4. Director: Defines the order of construction steps for common configurations (Director class)
 * 5. Client: Uses Builder directly or through Director to construct products
 *
 * WHEN TO USE BUILDER PATTERN:
 * - Objects with many constructor parameters (4+)
 * - Step-by-step construction required
 * - Same construction process creates different representations
 * - Avoiding telescoping constructor anti-pattern
 * - Building immutable objects that need all properties set before creation
 *
 * DIRECTOR VS DIRECT BUILDER USAGE:
 * - Use Director: When you have common construction sequences to reuse
 * - Use Builder Directly: When you need fine-grained control or custom configurations
 * - Director is OPTIONAL: Clients can always bypass Director and use Builder directly
 *
 * BENEFITS OVER CONSTRUCTORS:
 * - Readable, self-documenting code (especially with fluent interfaces)
 * - Fine-grained control over construction process
 * - Can construct objects step by step, deferring expensive operations
 * - Easy to add new construction steps without breaking existing code
 */
