// The Proxy pattern provides a surrogate or placeholder for another object to control access to it.
// This conceptual example demonstrates different proxy types: access control and logging.

/// <summary>
/// The Subject interface declares common operations for both RealSubject and Proxy.
///
/// KEY PRINCIPLE:
/// - Both RealSubject and Proxy implement the same interface
/// - This allows proxies to be used anywhere the real subject is expected
/// - Clients work with the interface, unaware if they're using a proxy or real object
///
/// DESIGN BENEFITS:
/// - Enables transparent substitution (Liskov Substitution Principle)
/// - Client code doesn't need to know about proxy existence
/// - Multiple proxy types can be created for different purposes
///
/// REAL-WORLD EXAMPLES:
/// - IRepository interface implemented by both Repository and CachingProxyRepository
/// - IService interface implemented by both Service and LoggingProxyService
/// - IResource interface implemented by both Resource and LazyLoadingProxyResource
/// </summary>
public interface ISubject
{
    /// <summary>
    /// The operation that both RealSubject and Proxy must implement.
    /// Clients call this method without knowing if it's proxied or not.
    /// </summary>
    void Request();
}

/// <summary>
/// The RealSubject contains the actual business logic.
///
/// KEY CHARACTERISTICS:
/// - Contains the core functionality that clients want to use
/// - May be expensive to create, slow to execute, or require special access
/// - Has no knowledge of proxies (follows Single Responsibility Principle)
/// - Can be used directly or through a proxy
///
/// TYPICAL SCENARIOS:
/// - Heavy object that takes time to initialize (images, videos, large datasets)
/// - Remote object that requires network communication
/// - Sensitive object that requires access control
/// - Object whose usage should be logged or monitored
///
/// DESIGN CONSIDERATIONS:
/// - Should focus only on its core business logic
/// - Shouldn't contain proxy concerns (logging, caching, access control)
/// - Can evolve independently of proxy implementations
///
/// REAL-WORLD EXAMPLES:
/// - Database connection that's expensive to create
/// - Large image or video file that's slow to load
/// - Remote service that requires network calls
/// - Admin-only service that needs access control
/// </summary>
class RealSubject : ISubject
{
    /// <summary>
    /// The actual implementation of the business logic.
    /// This is what clients ultimately want to execute.
    /// </summary>
    public void Request()
    {
        Console.WriteLine("RealSubject: Handling Request.");
    }
}

/// <summary>
/// The Proxy maintains a reference to the RealSubject and controls access to it.
///
/// PROXY TYPES DEMONSTRATED:
/// 1. Protection Proxy: CheckAccess() controls who can use the real subject
/// 2. Logging Proxy: LogAccess() records usage of the real subject
///
/// OTHER COMMON PROXY TYPES:
/// 3. Virtual Proxy: Delays creation of expensive objects until needed (lazy loading)
/// 4. Remote Proxy: Represents objects in different address spaces (WCF, web services)
/// 5. Caching Proxy: Caches results to avoid repeated expensive operations
/// 6. Smart Reference: Adds additional behavior like reference counting
///
/// KEY RESPONSIBILITIES:
/// - Implements the same interface as RealSubject for transparent substitution
/// - Controls access to RealSubject (when, how, by whom)
/// - May add behavior before/after delegating to RealSubject
/// - Can manage RealSubject lifecycle (creation, caching, disposal)
///
/// DESIGN PATTERNS RELATIONSHIP:
/// - Decorator: Adds behavior (focuses on enhancement)
/// - Proxy: Controls access (focuses on control/protection/optimization)
/// - Both use composition and implement the same interface
///
/// REAL-WORLD EXAMPLES:
/// - Entity Framework change tracking proxy
/// - WCF client proxy for remote services
/// - Lazy&lt;T&gt; for deferred initialization
/// - Authorization middleware in ASP.NET Core
/// </summary>
class Proxy : ISubject
{
    private readonly RealSubject _realSubject;

    /// <summary>
    /// Constructor accepts a RealSubject instance.
    ///
    /// DESIGN VARIATIONS:
    /// - Accept existing instance (as shown): Allows reuse of RealSubject
    /// - Create instance internally: Provides full control over RealSubject lifecycle
    /// - Lazy initialization: Create RealSubject only when first needed (Virtual Proxy)
    ///
    /// TRADE-OFFS:
    /// - Accepting instance: More flexible, easier testing, follows Dependency Injection
    /// - Creating internally: Simpler client code, proxy controls lifecycle
    /// - Lazy loading: Optimizes resource usage, delays initialization cost
    /// </summary>
    public Proxy(RealSubject realSubject)
    {
        _realSubject = realSubject;
    }

    /// <summary>
    /// The Proxy's Request implementation demonstrates the core proxy workflow.
    ///
    /// STANDARD PROXY PATTERN:
    /// 1. Pre-processing: Perform checks or setup before delegation
    /// 2. Delegation: Forward request to RealSubject (if allowed)
    /// 3. Post-processing: Perform cleanup or logging after delegation
    ///
    /// This method combines multiple proxy types:
    /// - Protection Proxy: CheckAccess() ensures authorization
    /// - Logging Proxy: LogAccess() records the request
    ///
    /// VARIATIONS:
    /// - Caching Proxy: Check cache before delegation, store result after
    /// - Virtual Proxy: Create RealSubject on first call, reuse thereafter
    /// - Remote Proxy: Serialize request, send over network, deserialize response
    /// </summary>
    public void Request()
    {
        // Pre-processing: Check if access is allowed (Protection Proxy)
        if (CheckAccess())
        {
            // Delegation: Forward to RealSubject if access granted
            _realSubject.Request();

            // Post-processing: Log the access (Logging Proxy)
            LogAccess();
        }
        // If access denied, RealSubject is never called
    }

    /// <summary>
    /// CheckAccess implements the Protection Proxy pattern.
    ///
    /// PURPOSE:
    /// - Controls who can access the RealSubject
    /// - Enforces security policies or business rules
    /// - Can check permissions, quotas, licensing, etc.
    ///
    /// REAL-WORLD IMPLEMENTATIONS:
    /// - User authentication and authorization
    /// - Role-based access control (RBAC)
    /// - API rate limiting
    /// - License validation
    /// - Resource quota checks
    ///
    /// CURRENT IMPLEMENTATION:
    /// - Always returns true (simplified for demonstration)
    /// - In real applications, would check actual permissions
    /// </summary>
    /// <returns>True if access is granted, false otherwise</returns>
    public bool CheckAccess()
    {
        // Some real checks should go here.
        // Examples:
        // - if (!currentUser.HasPermission("AdminAccess")) return false;
        // - if (rateLimiter.IsExceeded()) return false;
        // - if (!licenseValidator.IsValid()) return false;

        Console.WriteLine("Proxy: Checking access prior to firing a real request.");

        return true;
    }

    /// <summary>
    /// LogAccess implements the Logging Proxy pattern.
    ///
    /// PURPOSE:
    /// - Records usage of the RealSubject for auditing or monitoring
    /// - Can track performance metrics
    /// - Helps with debugging and troubleshooting
    ///
    /// REAL-WORLD IMPLEMENTATIONS:
    /// - Audit logging (who accessed what and when)
    /// - Performance monitoring (how long operations take)
    /// - Usage analytics (which features are used most)
    /// - Error tracking (when operations fail)
    /// - Compliance requirements (SOX, HIPAA, GDPR)
    ///
    /// INTEGRATION POINTS:
    /// - Application Insights / Application Performance Monitoring (APM)
    /// - Serilog / NLog structured logging
    /// - Database audit tables
    /// - Event sourcing systems
    /// </summary>
    public void LogAccess()
    {
        Console.WriteLine("Proxy: Logging the time of request.");

        // Real implementation might include:
        // - logger.LogInformation("Request at {Time} by {User}", DateTime.Now, currentUser);
        // - metrics.RecordRequestLatency(stopwatch.Elapsed);
        // - auditRepository.SaveAuditEntry(new AuditEntry { ... });
    }
}

/// <summary>
/// The Client class demonstrates working with the ISubject interface.
///
/// KEY PRINCIPLE - DEPENDENCY ON ABSTRACTION:
/// - Client depends on ISubject interface, not concrete implementations
/// - Can work with RealSubject or Proxy interchangeably
/// - No code changes needed when introducing or removing proxies
///
/// BENEFITS:
/// - Loose coupling: Client doesn't know or care about proxy existence
/// - Flexibility: Proxies can be added/removed without changing client code
/// - Testability: Easy to inject mocks or test doubles
/// - Open/Closed Principle: Client is closed for modification, open for extension
///
/// TRANSPARENCY:
/// - Client code is identical whether using RealSubject or Proxy
/// - This is the "transparent" aspect of the Proxy pattern
/// - Proxy concerns (security, logging, caching) are invisible to client
/// </summary>
public class Client
{
    /// <summary>
    /// Client code works with subjects through the ISubject interface.
    ///
    /// DESIGN ADVANTAGE:
    /// - This method accepts any ISubject implementation
    /// - Works equally well with RealSubject, Proxy, or any other implementation
    /// - Client is completely unaware of proxy-specific behavior
    ///
    /// REAL-WORLD SCENARIOS:
    /// - Service layer doesn't know if repository is cached or not
    /// - Controller doesn't know if service calls are logged or not
    /// - Business logic doesn't know if data access is proxied or direct
    /// </summary>
    /// <param name="subject">Any object implementing ISubject (RealSubject or Proxy)</param>
    public void ClientCode(ISubject subject)
    {
        // Same code works for both RealSubject and Proxy
        // Client has no idea whether this is proxied or not
        subject.Request();
    }
}

/// <summary>
/// Program demonstrates the Proxy pattern with different usage scenarios.
/// Shows how proxies add behavior transparently without changing client code.
/// </summary>
public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Proxy Pattern Demonstration ===\n");

        Client client = new Client();

        Console.WriteLine("--- Example 1: Client Using RealSubject Directly ---");
        Console.WriteLine("Client: Executing the client code with a real subject:");

        // Client can use RealSubject directly when no proxy is needed
        RealSubject realSubject = new RealSubject();
        client.ClientCode(realSubject);

        Console.WriteLine();

        Console.WriteLine("--- Example 2: Client Using Proxy (Transparent) ---");
        Console.WriteLine("Client: Executing the same client code with a proxy:");

        // Same client code, but now with added proxy behavior (access control + logging)
        // Client code is identical - this is the transparency of the Proxy pattern
        Proxy proxy = new Proxy(realSubject);
        client.ClientCode(proxy);
        // Output shows access check and logging happened without client knowing

        Console.WriteLine();

        Console.WriteLine("--- Example 3: Multiple Proxy Types (Conceptual) ---");
        Console.WriteLine("In real applications, you might use:");
        Console.WriteLine("- VirtualProxy: Lazy loading of expensive objects");
        Console.WriteLine("- CachingProxy: Caching results to avoid repeated work");
        Console.WriteLine("- RemoteProxy: Representing remote objects locally");
        Console.WriteLine("- SmartReferenceProxy: Reference counting, locking");

        Console.WriteLine();

        Console.WriteLine("--- Example 4: Chaining Proxies (Advanced) ---");
        Console.WriteLine("Proxies can be chained: Client -> LoggingProxy -> CachingProxy -> AuthProxy -> RealSubject");
        Console.WriteLine("Each proxy adds its behavior and forwards to the next");
        Console.WriteLine("This is similar to middleware pipeline in ASP.NET Core");
    }
}

/*
 * KEY TAKEAWAYS:
 *
 * PATTERN COMPONENTS:
 * 1. ISubject - Interface that both RealSubject and Proxy implement
 * 2. RealSubject - Contains actual business logic
 * 3. Proxy - Controls access to RealSubject, adds behavior before/after delegation
 * 4. Client - Works with ISubject interface, unaware of proxies
 *
 * WHEN TO USE:
 * - Lazy initialization: Delay expensive object creation until needed (Virtual Proxy)
 * - Access control: Protect objects from unauthorized access (Protection Proxy)
 * - Logging/auditing: Record operations for compliance or debugging (Logging Proxy)
 * - Caching: Avoid repeated expensive operations (Caching Proxy)
 * - Remote access: Local representation of remote objects (Remote Proxy)
 * - Smart references: Add reference counting, locking, etc. (Smart Reference Proxy)
 *
 * BENEFITS:
 * 1. Transparency: Client code unchanged when adding/removing proxies
 * 2. Separation of Concerns: RealSubject focuses on business logic, proxy handles cross-cutting concerns
 * 3. Single Responsibility: Each proxy type handles one concern (logging, caching, etc.)
 * 4. Open/Closed: Add new proxy types without modifying existing code
 * 5. Performance: Can optimize resource usage through lazy loading or caching
 * 6. Security: Can enforce access control policies
 *
 * PROXY TYPES:
 * 1. Virtual Proxy - Lazy initialization of expensive objects (Lazy<T> in .NET)
 * 2. Protection Proxy - Access control and authorization
 * 3. Remote Proxy - Local representative of remote object (WCF, Web Services)
 * 4. Caching Proxy - Cache results to improve performance
 * 5. Logging Proxy - Audit and track usage
 * 6. Smart Reference - Reference counting, copy-on-write, locking
 *
 * REAL-WORLD EXAMPLES IN .NET:
 * - Lazy<T>: Virtual proxy for deferred initialization
 * - Entity Framework proxies: Change tracking and lazy loading
 * - WCF client proxies: Remote service communication
 * - DispatchProxy / RealProxy: Dynamic proxy generation
 * - ASP.NET Core middleware: Chain of proxies for request processing
 * - IMemoryCache with fallback: Caching proxy pattern
 * - Authorization policies: Protection proxy for endpoints
 *
 * COMPARISON WITH SIMILAR PATTERNS:
 * - Decorator: Adds behavior (enhancement focus); Proxy: Controls access (control focus)
 * - Adapter: Changes interface; Proxy: Same interface as real object
 * - Facade: Simplifies complex interface; Proxy: Same interface, controls access
 *
 * TRADE-OFFS:
 * - Adds indirection (slight performance overhead)
 * - More classes in the system (increased complexity)
 * - May complicate debugging (additional layer to trace through)
 * - Need to maintain interface consistency between proxy and real subject
 *
 * MODERN .NET USAGE:
 * - Castle DynamicProxy: Runtime proxy generation for AOP
 * - DispatchProxy: Built-in .NET proxy generation
 * - Decorator pattern in dependency injection (Scrutor library)
 * - Middleware pipeline in ASP.NET Core
 * - IHttpClientFactory with delegating handlers (proxy chain)
 * - Memory cache with distributed cache fallback
 *
 * IMPLEMENTATION PATTERNS:
 * 1. Proxy with existing instance: new Proxy(realSubject)
 * 2. Proxy with lazy creation: Proxy creates RealSubject on first use
 * 3. Proxy factory: Factory creates and returns appropriate proxy
 * 4. Proxy chain: Multiple proxies wrapping each other
 * 5. Dynamic proxy: Generate proxy at runtime using reflection
 *
 * BEST PRACTICES:
 * - Keep proxy focused on one concern (SRP)
 * - Use same interface as RealSubject (LSP)
 * - Consider using DI to inject proxies
 * - Document proxy behavior for maintainability
 * - Test proxy and RealSubject separately
 * - Be aware of performance implications
 */
