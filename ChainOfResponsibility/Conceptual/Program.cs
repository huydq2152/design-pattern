// The Chain of Responsibility pattern passes requests along a chain of handlers.
// Each handler decides either to process the request or to pass it to the next handler in the chain.
// This conceptual example demonstrates the core structure and behavior of the pattern.

/// <summary>
/// The Handler interface declares a method for building the chain of handlers.
/// It also declares a method for executing a request.
///
/// KEY PRINCIPLES:
/// - Defines both SetNext (for chain building) and Handle (for processing)
/// - Allows handler composition through fluent interface
/// - All handlers implement the same interface for uniform treatment
///
/// DESIGN BENEFITS:
/// - Decouples sender and receiver of a request
/// - Multiple handlers can process the request
/// - Handlers can be added or removed dynamically
///
/// REAL-WORLD EXAMPLES:
/// - HTTP middleware pipeline (request/response processing)
/// - Exception handling frameworks (try-catch chain)
/// - Authentication/authorization chains
/// - Validation pipelines (form validation, business rules)
/// </summary>
public interface IHandler
{
    /// <summary>
    /// Sets the next handler in the chain.
    /// Returns the next handler to enable fluent chain building.
    /// Example: handler1.SetNext(handler2).SetNext(handler3)
    /// </summary>
    IHandler SetNext(IHandler handler);

    /// <summary>
    /// Handles the request or passes it to the next handler.
    /// Returns the result if handled, or null if no handler processed it.
    /// </summary>
    object Handle(object request);
}

/// <summary>
/// The base AbstractHandler class provides default chain behavior.
///
/// KEY RESPONSIBILITIES:
/// - Stores reference to the next handler in the chain
/// - Implements SetNext with fluent interface support
/// - Provides default Handle implementation that forwards to next handler
///
/// DESIGN PATTERN:
/// - Template Method: Provides default behavior that subclasses can override
/// - Fluent Interface: SetNext returns the handler for method chaining
///
/// CHARACTERISTICS:
/// - Handlers don't need to know about the full chain
/// - Each handler only knows about the next handler
/// - Subclasses override Handle to add custom processing logic
/// - Default forwarding behavior reduces boilerplate in concrete handlers
///
/// REAL-WORLD ANALOGS:
/// - Middleware base class in ASP.NET Core
/// - Filter base class in logging frameworks
/// - Handler base class in message processing systems
/// </summary>
public abstract class AbstractHandler : IHandler
{
    private IHandler? _nextHandler;

    /// <summary>
    /// Sets the next handler in the chain.
    ///
    /// FLUENT INTERFACE:
    /// Returns the next handler (not this) to enable chaining:
    /// handler1.SetNext(handler2).SetNext(handler3)
    ///
    /// This creates: handler1 -> handler2 -> handler3
    ///
    /// ALTERNATIVE DESIGN:
    /// Could return 'this' instead: handler1.SetNext(handler2).SetNext(handler3)
    /// Would create: handler3 -> handler2 -> handler1 (reversed)
    /// Current design is more intuitive for linear chain construction.
    /// </summary>
    public IHandler SetNext(IHandler handler)
    {
        _nextHandler = handler;

        // Return the next handler (not 'this') to allow fluent chaining
        return handler;
    }

    /// <summary>
    /// Default handling behavior: forward to the next handler.
    ///
    /// TEMPLATE METHOD PATTERN:
    /// Provides base behavior that concrete handlers can override.
    /// Subclasses typically:
    /// 1. Check if they can handle the request
    /// 2. If yes: process and return result
    /// 3. If no: call base.Handle(request) to forward
    ///
    /// DESIGN CHOICES:
    /// - Virtual (not abstract): Allows handlers that just forward
    /// - Returns null when chain ends: Signals no handler processed request
    /// - Could throw exception instead: More strict, fails fast
    /// - Could return default value: More lenient, always succeeds
    ///
    /// CHAIN TERMINATION:
    /// - If _nextHandler is null, we've reached the end
    /// - Returning null signals "request not handled"
    /// - Client can decide what to do with unhandled requests
    /// </summary>
    public virtual object? Handle(object request)
    {
        if (_nextHandler != null)
        {
            // Forward request to next handler in chain
            return _nextHandler.Handle(request);
        }
        else
        {
            // End of chain reached, request not handled
            return null;
        }
    }
}

/// <summary>
/// MonkeyHandler is a concrete handler that handles "Banana" requests.
///
/// HANDLER PATTERN:
/// 1. Check if this handler can process the request
/// 2. If yes: handle it and return result
/// 3. If no: delegate to next handler (base.Handle)
///
/// CHARACTERISTICS:
/// - Single Responsibility: Only handles bananas
/// - Open/Closed: Can add new handlers without modifying existing ones
/// - Liskov Substitution: Can be used wherever IHandler is expected
///
/// REAL-WORLD EXAMPLES:
/// - AuthenticationHandler: Handles authentication requests
/// - CacheHandler: Handles requests that can be cached
/// - LoggingHandler: Handles logging for all requests
/// </summary>
public class MonkeyHandler : AbstractHandler
{
    /// <summary>
    /// Handles "Banana" requests, forwards others to the next handler.
    ///
    /// PROCESSING LOGIC:
    /// - Check request type/content
    /// - If matches criteria: process and return
    /// - If doesn't match: call base.Handle to forward
    ///
    /// ALTERNATIVE APPROACHES:
    /// - Could process AND forward (logging, auditing)
    /// - Could modify request before forwarding (transformation)
    /// - Could short-circuit chain (caching, early validation)
    /// </summary>
    public override object? Handle(object request)
    {
        if ((request as string) == "Banana")
        {
            // This handler can process the request
            return $"Monkey: I'll eat the {request.ToString()}.\n";
        }
        else
        {
            // Can't handle, try next handler in chain
            return base.Handle(request);
        }
    }
}

/// <summary>
/// SquirrelHandler is a concrete handler that handles "Nut" requests.
///
/// DESIGN CONSISTENCY:
/// - Same structure as MonkeyHandler
/// - Different responsibility (different request type)
/// - Demonstrates how handlers are independent and focused
///
/// CHAIN FLEXIBILITY:
/// - Can be used in any position in the chain
/// - Can be part of multiple chains
/// - Can be added/removed without affecting other handlers
/// </summary>
public class SquirrelHandler : AbstractHandler
{
    public override object? Handle(object request)
    {
        if (request.ToString() == "Nut")
        {
            return $"Squirrel: I'll eat the {request.ToString()}.\n";
        }
        else
        {
            return base.Handle(request);
        }
    }
}

/// <summary>
/// DogHandler is a concrete handler that handles "MeatBall" requests.
///
/// CHAIN BEHAVIOR:
/// - Typically last in the chain in the examples
/// - If it can't handle, chain returns null (no handler processed)
/// - Could be anywhere in the chain (order matters for some use cases)
///
/// USE CASE CONSIDERATIONS:
/// - Order matters when multiple handlers can handle same request
/// - First matching handler wins (unless handlers forward after processing)
/// - Can create catch-all handler that handles everything (default handler)
/// </summary>
public class DogHandler : AbstractHandler
{
    public override object? Handle(object request)
    {
        if (request.ToString() == "MeatBall")
        {
            return $"Dog: I'll eat the {request.ToString()}.\n";
        }
        else
        {
            return base.Handle(request);
        }
    }
}

/// <summary>
/// The Client class demonstrates how to work with the chain of handlers.
///
/// CLIENT BENEFITS:
/// - Doesn't need to know which handler will process the request
/// - Doesn't need to know the chain structure
/// - Works with the chain through a single entry point (first handler)
/// - Can send different types of requests to the same chain
///
/// DECOUPLING:
/// - Client depends on IHandler interface, not concrete handlers
/// - Chain structure can change without affecting client code
/// - New handlers can be added transparently
///
/// FLEXIBILITY:
/// - Same client code works with different chain configurations
/// - Can have multiple chains for different purposes
/// - Can dynamically reconfigure chains at runtime
/// </summary>
public class Client
{
    /// <summary>
    /// Sends multiple requests through the handler chain.
    ///
    /// PATTERN DEMONSTRATION:
    /// - Client only knows about the first handler (entry point)
    /// - Sends various requests and handles results
    /// - Doesn't know or care about chain internals
    ///
    /// ERROR HANDLING:
    /// - Checks if result is null (request not handled)
    /// - Could throw exception for unhandled requests
    /// - Could have default behavior for unhandled requests
    ///
    /// REAL-WORLD USAGE:
    /// - Web request pipeline: middleware chain processes requests
    /// - Validation chain: multiple validators process input
    /// - Event handling: chain of event handlers
    /// </summary>
    public static void ClientCode(AbstractHandler handler)
    {
        // Send various requests through the chain
        foreach (var food in new List<string> { "Nut", "Banana", "Cup of coffee" })
        {
            Console.WriteLine($"Client: Who wants a {food}?");

            // Pass request to the first handler in the chain
            var result = handler.Handle(food);

            if (result != null)
            {
                // Some handler in the chain processed the request
                Console.Write($"   {result}");
            }
            else
            {
                // No handler in the chain could process the request
                Console.WriteLine($"   {food} was left untouched.");
            }
        }
    }
}

/// <summary>
/// Program demonstrates the Chain of Responsibility pattern.
/// Shows different chain configurations and how requests are processed.
/// </summary>
public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Chain of Responsibility Pattern Demonstration ===\n");

        Console.WriteLine("--- Example 1: Full Chain (Monkey -> Squirrel -> Dog) ---\n");

        // Create handler instances
        var monkey = new MonkeyHandler();
        var squirrel = new SquirrelHandler();
        var dog = new DogHandler();

        // Build the chain using fluent interface
        // This creates: monkey -> squirrel -> dog
        monkey.SetNext(squirrel).SetNext(dog);

        Console.WriteLine("Chain: Monkey -> Squirrel -> Dog\n");
        Client.ClientCode(monkey);
        // Banana -> handled by Monkey
        // Nut -> forwarded to Squirrel, handled by Squirrel
        // Cup of coffee -> forwarded through entire chain, not handled

        Console.WriteLine();

        Console.WriteLine("--- Example 2: Partial Chain (Squirrel -> Dog) ---\n");

        // Client can use any handler as entry point
        // This creates a shorter chain: squirrel -> dog
        Console.WriteLine("Subchain: Squirrel -> Dog\n");
        Client.ClientCode(squirrel);
        // Banana -> forwarded through chain, not handled (Monkey not in chain)
        // Nut -> handled by Squirrel
        // Cup of coffee -> forwarded to Dog, not handled

        Console.WriteLine();

        Console.WriteLine("--- Example 3: Single Handler ---\n");

        // Chain can consist of a single handler
        Console.WriteLine("Chain: Dog only\n");
        Client.ClientCode(dog);
        // Only MeatBall would be handled, others return null

        Console.WriteLine();

        Console.WriteLine("--- Example 4: Different Chain Order ---\n");

        // Create new instances to demonstrate different order
        var monkey2 = new MonkeyHandler();
        var squirrel2 = new SquirrelHandler();
        var dog2 = new DogHandler();

        // Build a different chain: dog -> monkey -> squirrel
        dog2.SetNext(monkey2).SetNext(squirrel2);

        Console.WriteLine("Chain: Dog -> Monkey -> Squirrel\n");
        var customRequests = new List<string> { "MeatBall", "Banana", "Nut" };
        foreach (var food in customRequests)
        {
            Console.WriteLine($"Client: Who wants a {food}?");
            var result = dog2.Handle(food);
            Console.Write(result != null ? $"   {result}" : $"   {food} was left untouched.\n");
        }
        // Order doesn't matter here since each handler handles different requests
        // But order matters when multiple handlers can handle the same request
    }
}

/*
 * KEY TAKEAWAYS:
 *
 * PATTERN COMPONENTS:
 * 1. IHandler - Interface defining Handle and SetNext methods
 * 2. AbstractHandler - Base class with default chain traversal logic
 * 3. Concrete Handlers - Specific handlers (MonkeyHandler, SquirrelHandler, DogHandler)
 * 4. Client - Sends requests to the chain through the first handler
 *
 * WHEN TO USE:
 * - Multiple objects can handle a request, but you don't know which one in advance
 * - You want to issue a request without specifying the receiver explicitly
 * - The set of handlers should be dynamic and configurable
 * - You want to decouple request sender from request handlers
 * - You need to process a request through multiple stages (middleware, filters)
 *
 * BENEFITS:
 * 1. Decoupling: Client doesn't need to know about handler hierarchy
 * 2. Flexibility: Can add, remove, or reorder handlers dynamically
 * 3. Single Responsibility: Each handler has one specific responsibility
 * 4. Open/Closed: Can add new handlers without modifying existing code
 * 5. Dynamic Chain: Chain can be built and modified at runtime
 * 6. Responsibility Distribution: Processing is distributed across handlers
 *
 * REAL-WORLD EXAMPLES:
 * - ASP.NET Core Middleware Pipeline: Request/response processing chain
 *   app.UseAuthentication().UseAuthorization().UseEndpoints(...)
 *
 * - Exception Handling: try-catch blocks form a chain
 *   Multiple catch blocks handle different exception types
 *
 * - Event Bubbling in UI: Events propagate up the component tree
 *   Each component can handle or forward the event
 *
 * - Logging Frameworks: Chain of loggers (console, file, database)
 *   Log message passes through chain, each logger decides whether to log
 *
 * - Approval Workflows: Purchase approval chain based on amount
 *   Manager -> Director -> VP -> CEO
 *
 * - Form Validation: Chain of validators
 *   Required -> Format -> Business Rule -> Database Check
 *
 * - HTTP Client Handlers: DelegatingHandler chain in HttpClient
 *   Logging -> Retry -> Cache -> Authentication -> Actual Request
 *
 * TRADE-OFFS:
 * - Request may not be handled: Need to handle null results
 * - Debugging difficulty: Hard to trace which handler processed request
 * - Performance: Request traverses multiple handlers
 * - No guarantee of handling: Chain might not have appropriate handler
 * - Chain configuration: Must ensure chain is configured correctly
 *
 * VARIATIONS:
 * 1. Process and Forward: Handler processes AND forwards to next
 *    Use case: Logging, auditing (all handlers participate)
 *
 * 2. Process or Forward: Handler processes OR forwards to next (current example)
 *    Use case: Request routing (only one handler processes)
 *
 * 3. Transform and Forward: Handler modifies request before forwarding
 *    Use case: Request transformation pipeline
 *
 * 4. Catch-all Handler: Final handler that handles everything
 *    Use case: Default behavior, error handling
 *
 * IMPLEMENTATION CONSIDERATIONS:
 * - Handler Order Matters: When multiple handlers can handle same request
 * - Chain Termination: Decide what happens when no handler processes request
 * - Thread Safety: Consider if chain will be used in multi-threaded scenarios
 * - Handler State: Handlers should typically be stateless or thread-safe
 * - Error Handling: Decide how to handle exceptions in handlers
 *
 * MODERN .NET USAGE:
 * - ASP.NET Core Middleware: app.Use(), app.UseAuthentication(), app.UseAuthorization()
 * - HttpClient DelegatingHandler: Custom message handlers for HTTP requests
 * - Serilog Sink Chain: Multiple logging sinks processing log events
 * - MediatR Pipeline Behaviors: Pre/post-processing behaviors for commands
 * - FluentValidation: Chain of validation rules
 *
 * COMPARISON WITH OTHER PATTERNS:
 * - Command: Encapsulates request as object; CoR passes request through chain
 * - Decorator: Similar structure but focuses on adding behavior, not handling requests
 * - Composite: Similar tree structure but focuses on part-whole hierarchies
 * - Strategy: Chooses one algorithm; CoR tries multiple handlers in sequence
 *
 * BEST PRACTICES:
 * 1. Keep handlers focused: Each handler should have one responsibility
 * 2. Make chains configurable: Allow dynamic chain building
 * 3. Document chain order: When order matters, document expected sequence
 * 4. Handle null results: Client should handle case when no handler processes request
 * 5. Consider catch-all: Add default handler at end of chain if appropriate
 * 6. Avoid long chains: Too many handlers can impact performance and debuggability
 * 7. Use dependency injection: Register handlers in DI container for better testability
 * 8. Keep handlers stateless: Makes chain thread-safe and reusable
 */
