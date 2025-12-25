// The Facade pattern provides a simplified interface to a complex subsystem.
// This conceptual example demonstrates the core structure and interaction of the pattern.

/// <summary>
/// The Facade class provides a simple interface to the complex logic of one or more subsystems.
///
/// KEY RESPONSIBILITIES:
/// - Provides a simplified, high-level interface to complex subsystem operations
/// - Delegates client requests to appropriate subsystem objects
/// - Coordinates subsystem interactions to perform complex workflows
/// - Shields clients from subsystem complexity and implementation details
///
/// DESIGN DECISIONS:
/// - Uses composition to hold references to subsystem instances
/// - Constructor injection allows for flexibility and testability
/// - Orchestrates multiple subsystem calls in a specific order
/// - Returns aggregated results from multiple subsystems
///
/// REAL-WORLD EXAMPLES:
/// - E-commerce checkout: coordinates cart, payment, shipping, inventory subsystems
/// - Video converter: manages codec, audio, video, file format subsystems
/// - Database connection pool: simplifies connection, transaction, query subsystems
///
/// CHARACTERISTICS:
/// - Does not encapsulate subsystems (clients can still access them directly if needed)
/// - Can have multiple facades for different client needs
/// - Promotes loose coupling between clients and complex subsystems
/// </summary>
public class Facade
{
    private readonly Subsystem1 _subsystem1;
    private readonly Subsystem2 _subsystem2;

    /// <summary>
    /// Initializes the Facade with references to subsystem instances.
    /// Constructor injection allows for dependency injection and easier testing.
    /// </summary>
    /// <param name="subsystem1">First subsystem to coordinate</param>
    /// <param name="subsystem2">Second subsystem to coordinate</param>
    public Facade(Subsystem1 subsystem1, Subsystem2 subsystem2)
    {
        _subsystem1 = subsystem1;
        _subsystem2 = subsystem2;
    }

    /// <summary>
    /// A high-level operation that coordinates multiple subsystems.
    ///
    /// This method demonstrates the core purpose of the Facade pattern:
    /// - Hides the complexity of multiple subsystem interactions
    /// - Provides a single, simple method call for a complex workflow
    /// - Manages the correct order of operations across subsystems
    /// - Aggregates results from multiple subsystems
    ///
    /// In real applications, this could be:
    /// - ProcessOrder() coordinating cart, payment, shipping, inventory
    /// - ConvertVideo() managing codec, audio, video, format subsystems
    /// - GenerateReport() combining data, formatting, export subsystems
    /// </summary>
    /// <returns>Aggregated result from all subsystem operations</returns>
    public string Operation()
    {
        string result = "Facade initializes subsystems:\n";

        // Phase 1: Initialize subsystems
        // The facade knows which subsystems to initialize and in what order
        result += _subsystem1.Operation1();
        result += _subsystem2.Operation1();

        result += "Facade orders subsystems to perform the action:\n";

        // Phase 2: Execute main operations
        // The facade coordinates the workflow between subsystems
        result += _subsystem1.OperationN();
        result += _subsystem2.OperationZ();

        return result;
    }
}

/// <summary>
/// Subsystem1 represents a component of the complex system.
///
/// CHARACTERISTICS:
/// - Contains its own complex logic and operations
/// - Can be used independently or through the facade
/// - May have many methods, only some of which are used by the facade
/// - Has no knowledge of the facade's existence
///
/// DESIGN PRINCIPLES:
/// - Single Responsibility: focused on its specific domain logic
/// - Can evolve independently of the facade
/// - Multiple facades can use the same subsystem differently
///
/// REAL-WORLD ANALOGS:
/// - Payment processing service in e-commerce
/// - Audio codec in video conversion
/// - Database connection manager in data access layer
/// </summary>
public class Subsystem1
{
    /// <summary>
    /// Initialization operation for Subsystem1.
    /// This might involve resource allocation, configuration loading, etc.
    /// </summary>
    public string Operation1()
    {
        return "Subsystem1: Ready!\n";
    }

    /// <summary>
    /// A specific operation that Subsystem1 can perform.
    /// The facade calls this as part of its coordinated workflow.
    /// </summary>
    public string OperationN()
    {
        return "Subsystem1: Go!\n";
    }
}

/// <summary>
/// Subsystem2 represents another component of the complex system.
///
/// CHARACTERISTICS:
/// - Independent of Subsystem1 but may be used together via the facade
/// - Has its own interface and implementation details
/// - Can be modified without affecting clients that use the facade
///
/// COORDINATION:
/// - The facade knows how to coordinate this with Subsystem1
/// - Clients don't need to understand the relationship between subsystems
/// - The facade ensures operations are called in the correct order
///
/// REAL-WORLD ANALOGS:
/// - Shipping service in e-commerce (works with payment subsystem)
/// - Video codec in video conversion (works with audio codec)
/// - Query builder in data access layer (works with connection manager)
/// </summary>
public class Subsystem2
{
    /// <summary>
    /// Initialization operation for Subsystem2.
    /// May have different initialization requirements than Subsystem1.
    /// </summary>
    public string Operation1()
    {
        return "Subsystem2: Get ready!\n";
    }

    /// <summary>
    /// A specific operation that Subsystem2 can perform.
    /// Works in coordination with Subsystem1 operations via the facade.
    /// </summary>
    public string OperationZ()
    {
        return "Subsystem2: Fire!\n";
    }
}

/// <summary>
/// The Client class demonstrates how code interacts with the facade.
///
/// CLIENT BENEFITS:
/// - Works with a simple interface instead of complex subsystems
/// - Doesn't need to know about subsystem dependencies or interaction order
/// - Protected from changes in subsystem implementation
/// - Cleaner, more maintainable code
///
/// FLEXIBILITY:
/// - Client can still access subsystems directly if needed (facade doesn't prevent this)
/// - Can use multiple facades for different operations
/// - Facade is optional - it's an additional layer, not a mandatory one
/// </summary>
public static class Client
{
    /// <summary>
    /// Client code works with complex subsystems through a simple facade interface.
    /// The client doesn't need to know:
    /// - How many subsystems exist
    /// - How subsystems interact
    /// - The correct order of operations
    /// - Implementation details of any subsystem
    /// </summary>
    public static void ClientCode(Facade facade)
    {
        // Single, simple call replaces complex subsystem orchestration
        Console.Write(facade.Operation());
    }
}

/// <summary>
/// Program demonstrates the Facade pattern in action.
/// Shows how the facade simplifies client code and subsystem coordination.
/// </summary>
public static class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Facade Pattern Demonstration ===\n");

        Console.WriteLine("--- Example 1: Using Facade to Simplify Complex Operations ---");

        // Create subsystem instances
        // In real applications, these might be complex objects with many dependencies
        Subsystem1 subsystem1 = new Subsystem1();
        Subsystem2 subsystem2 = new Subsystem2();

        // Create facade with subsystems
        // The facade knows how to coordinate these subsystems
        Facade facade = new Facade(subsystem1, subsystem2);

        // Client uses the simple facade interface
        // Instead of: subsystem1.Operation1(), subsystem2.Operation1(), subsystem1.OperationN(), subsystem2.OperationZ()
        // Client just calls: facade.Operation()
        Client.ClientCode(facade);

        Console.WriteLine("\n--- Example 2: Direct Subsystem Access (Still Possible) ---");

        // The facade doesn't prevent direct subsystem access
        // Clients can still use subsystems directly if needed for special cases
        Console.WriteLine("Direct call to Subsystem1:");
        Console.Write(subsystem1.Operation1());
        Console.Write(subsystem1.OperationN());

        Console.WriteLine("\n--- Example 3: Multiple Facades for Different Workflows ---");

        // You can have multiple facades for different operations on the same subsystems
        Console.WriteLine("In real applications, you might have:");
        Console.WriteLine("- CheckoutFacade: coordinates payment, shipping, inventory");
        Console.WriteLine("- OrderTrackingFacade: coordinates shipping, notifications");
        Console.WriteLine("- RefundFacade: coordinates payment, inventory, notifications");
    }
}

/*
 * KEY TAKEAWAYS:
 *
 * PATTERN COMPONENTS:
 * 1. Facade - Provides simplified interface to complex subsystems
 * 2. Subsystems - Complex components with their own logic and interfaces
 * 3. Client - Uses facade instead of working directly with subsystems
 *
 * WHEN TO USE:
 * - You need to provide a simple interface to a complex subsystem
 * - You want to decouple clients from subsystem implementation details
 * - You need to layer your subsystems (facade can be a layer)
 * - You want to simplify and unify a complex or poorly designed API
 * - Multiple subsystems need to be coordinated in a specific workflow
 *
 * BENEFITS:
 * 1. Simplified Interface: Clients work with simple methods instead of complex subsystems
 * 2. Loose Coupling: Clients depend on facade, not on subsystem implementation
 * 3. Flexibility: Subsystems can change without affecting clients
 * 4. Layering: Facades can create architectural layers in your application
 * 5. Optional Usage: Clients can still access subsystems directly if needed
 *
 * REAL-WORLD EXAMPLES:
 * - E-commerce checkout process (cart, payment, shipping, inventory, notifications)
 * - Video/audio conversion (codec, format, quality, metadata subsystems)
 * - Compiler (lexer, parser, optimizer, code generator subsystems)
 * - Database frameworks (connection, transaction, query, caching subsystems)
 * - .NET framework: DbProviderFactory, HttpClient, Entity Framework DbContext
 *
 * TRADE-OFFS:
 * - Facade can become a "god object" if it tries to do too much
 * - May hide useful functionality if not designed carefully
 * - Additional layer adds slight complexity to the architecture
 * - Need to balance between simplicity and flexibility
 *
 * COMPARISON WITH OTHER PATTERNS:
 * - Adapter: Changes interface of one object; Facade: Simplifies interface of many objects
 * - Mediator: Two-way communication; Facade: One-way (client to subsystems)
 * - Proxy: Same interface as real object; Facade: New simplified interface
 *
 * MODERN .NET USAGE:
 * - ASP.NET Core middleware pipeline (facade over complex request processing)
 * - Entity Framework DbContext (facade over database operations)
 * - IServiceProvider in DI (facade over service resolution)
 * - HttpClient (facade over HTTP operations)
 */
