using Conceptual.Factory;
using Conceptual.Models;

// Client code demonstrating the Flyweight pattern
// Shows how to minimize memory usage by sharing common state across objects
//
// Scenario: Police car database
// - Many cars share the same Company, Model, and Color (intrinsic state)
// - Each car has unique Owner and Number (extrinsic state)
// - Without Flyweight: Store full Car object for each entry
// - With Flyweight: Share car type objects, store only unique data per entry
public static class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Flyweight Pattern Demonstration ===\n");

        // Pre-populate factory with common car types
        // These are the flyweights that will be shared across multiple car instances
        // In a real application, these might be loaded from a database
        Console.WriteLine("Initializing factory with common car types...");
        var factory = new FlyweightFactory(
            new Car { Company = "Chevrolet", Model = "Camaro2018", Color = "pink" },
            new Car { Company = "Mercedes Benz", Model = "C300", Color = "black" },
            new Car { Company = "Mercedes Benz", Model = "C500", Color = "red" },
            new Car { Company = "BMW", Model = "M5", Color = "red" },
            new Car { Company = "BMW", Model = "X6", Color = "white" }
        );

        // Display initial flyweights
        factory.ListFlyweights();

        // Example 1: Add a car that matches existing flyweight
        // The factory will reuse the "BMW M5 red" flyweight
        Console.WriteLine("\n--- Example 1: Adding Car with Existing Type ---");
        AddCarToPoliceDatabase(factory, new Car
        {
            Number = "CL234IR",
            Owner = "James Doe",
            Company = "BMW",
            Model = "M5",
            Color = "red"
        });

        // Example 2: Add a car with a new type
        // The factory will create a new flyweight for "BMW X1 red"
        Console.WriteLine("\n--- Example 2: Adding Car with New Type ---");
        AddCarToPoliceDatabase(factory, new Car
        {
            Number = "CL456XY",
            Owner = "Jane Smith",
            Company = "BMW",
            Model = "X1",
            Color = "red"
        });

        // Example 3: Add another car matching Example 1
        // Demonstrates flyweight reuse - no new flyweight created
        Console.WriteLine("\n--- Example 3: Adding Another Car with Existing Type ---");
        AddCarToPoliceDatabase(factory, new Car
        {
            Number = "AB789ZZ",
            Owner = "Bob Johnson",
            Company = "BMW",
            Model = "M5",
            Color = "red"
        });

        // Display final state
        // Notice: Only 6 flyweights despite adding 3 cars
        // Two of the added cars share the "BMW M5 red" flyweight
        factory.ListFlyweights();

        // Memory savings illustration
        Console.WriteLine("\n=== Memory Savings Analysis ===");
        Console.WriteLine("Without Flyweight:");
        Console.WriteLine("  - 8 cars (5 initial + 3 added) × full Car object = 8 full objects");
        Console.WriteLine("With Flyweight:");
        Console.WriteLine("  - 6 unique car types × Flyweight object = 6 flyweights");
        Console.WriteLine("  - 8 cars × lightweight reference + extrinsic data");
        Console.WriteLine("  - Shared state (Company, Model, Color) stored only 6 times");
        Console.WriteLine("  - Unique state (Owner, Number) stored 8 times");
    }

    // Helper method to add a car to the police database
    // This demonstrates the typical usage pattern of Flyweight:
    // 1. Extract intrinsic state (car type)
    // 2. Get or create flyweight for that state
    // 3. Use flyweight with extrinsic state (owner, number)
    private static void AddCarToPoliceDatabase(FlyweightFactory factory, Car car)
    {
        Console.WriteLine($"Client: Adding car to database (Owner: {car.Owner}, Number: {car.Number})");

        // Step 1: Extract intrinsic state (shared data)
        // Create a Car object containing only the type information
        var carType = new Car
        {
            Color = car.Color,
            Model = car.Model,
            Company = car.Company
            // Note: Owner and Number are NOT included (extrinsic state)
        };

        // Step 2: Get flyweight for this car type
        // If a flyweight for "BMW M5 red" exists, it's reused
        // Otherwise, a new flyweight is created
        var flyweight = factory.GetFlyweight(carType);

        // Step 3: Use the flyweight with extrinsic state
        // The flyweight's Operation method receives the full car data
        // It combines its stored intrinsic state with the passed extrinsic state
        flyweight.Operation(car);
    }
}

/*
 * KEY TAKEAWAYS:
 *
 * FLYWEIGHT PATTERN COMPONENTS:
 * 1. Flyweight: Stores intrinsic (shared) state, receives extrinsic (unique) state as parameters
 * 2. FlyweightFactory: Manages pool of flyweights, ensures sharing
 * 3. Client: Stores extrinsic state, uses flyweights for operations
 *
 * INTRINSIC vs EXTRINSIC STATE:
 * - Intrinsic state: Shared, immutable, stored in flyweight (Company, Model, Color)
 * - Extrinsic state: Unique, context-dependent, passed to flyweight methods (Owner, Number)
 *
 * HOW FLYWEIGHT WORKS:
 * 1. Client needs to work with an object (e.g., a car)
 * 2. Client extracts intrinsic state (car type) from full object
 * 3. Client asks factory for flyweight matching that intrinsic state
 * 4. Factory returns existing flyweight or creates new one
 * 5. Client uses flyweight, passing extrinsic state as parameters
 *
 * MEMORY SAVINGS:
 * - Traditional approach: N objects × full object size
 * - Flyweight approach: M flyweights × intrinsic size + N × extrinsic size
 * - Where M << N (many objects share same intrinsic state)
 *
 * Example with 1000 cars:
 * - 1000 unique owners/numbers (extrinsic)
 * - 50 unique car types (intrinsic)
 * - Without Flyweight: 1000 full objects
 * - With Flyweight: 50 flyweights + 1000 lightweight references
 *
 * WHEN TO USE FLYWEIGHT:
 * - Large number of similar objects consuming excessive memory
 * - Objects can be split into intrinsic (shared) and extrinsic (unique) state
 * - Application becomes slow due to memory pressure or GC
 * - Intrinsic state is immutable (can be safely shared)
 *
 * KEY DESIGN DECISIONS:
 * 1. Identify what state is intrinsic (shared) vs extrinsic (unique)
 * 2. Make intrinsic state immutable (readonly fields)
 * 3. Use consistent key generation for flyweight lookup
 * 4. Factory manages flyweight lifecycle and sharing
 * 5. Client passes extrinsic state to flyweight methods
 *
 * REAL-WORLD EXAMPLES:
 * - Text editors: Character glyphs (intrinsic: font, size; extrinsic: position)
 * - Games: Particles, bullets, trees (intrinsic: texture, model; extrinsic: position, velocity)
 * - String interning: .NET shares identical string literals
 * - Database connections: Connection pooling shares connections
 *
 * TRADE-OFFS:
 * - Pro: Dramatic memory savings for large numbers of similar objects
 * - Pro: Reduced GC pressure, improved performance
 * - Con: Added complexity (separating intrinsic/extrinsic state)
 * - Con: CPU overhead (factory lookups, passing extrinsic state)
 * - Con: Client must manage extrinsic state
 *
 * FACTORY PATTERN:
 * - FlyweightFactory uses caching to ensure object sharing
 * - Key generation must be consistent and efficient
 * - Consider using Dictionary<TKey, TValue> for O(1) lookups
 * - Pre-populate factory with common flyweights for better performance
 *
 * IMMUTABILITY:
 * - Intrinsic state MUST be immutable (readonly)
 * - Prevents bugs from shared state modification
 * - Makes flyweights thread-safe by default
 * - Use readonly fields or init-only properties
 */
