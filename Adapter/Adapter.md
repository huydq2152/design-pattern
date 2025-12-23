# Adapter Pattern
## 1. Definition
The Adapter Pattern converts the interface of a class into another interface that clients expect. It allows classes with incompatible interfaces to work together by wrapping the incompatible class with a compatible interface. This pattern acts as a bridge between two incompatible interfaces, making it possible for existing classes to work with others without modifying their source code.

### Manual Implementation
1. Identify the Target interface that the client expects
2. Identify the Adaptee class with an incompatible interface that needs to be adapted
3. Create an Adapter class that implements the Target interface
4. The Adapter contains an instance of the Adaptee (composition)
5. The Adapter translates calls from the Target interface to the Adaptee's interface
6. Client code works with the Target interface, unaware of the adaptation happening behind the scenes

## 2. Pros
- **Open/Closed Principle**: Introduce new adapters without modifying existing client code
- **Single Responsibility**: Separates interface conversion logic from business logic
- **Reusability**: Allows reusing existing classes even when their interfaces don't match requirements
- **Third-Party Integration**: Enables integration with external libraries without modifying their code
- **Legacy Code Support**: Works with legacy systems by adapting old interfaces to new requirements
- **Decoupling**: Client code remains decoupled from concrete implementations

## 3. Cons
- **Increased Complexity**: Adds extra classes and indirection layers
- **Performance Overhead**: Additional method calls through the adapter can impact performance
- **Multiple Adapters Needed**: May require creating many adapter classes for different incompatibilities
- **Harder to Debug**: Extra abstraction layers can make debugging more difficult
- **Code Maintenance**: More classes to maintain and keep synchronized with changes

## 4. Real-world Use Cases in C# & .NET
- **Legacy System Integration**: Adapting old database APIs to modern repository patterns
```csharp
// Adapt legacy database connection to modern IDbConnection interface
public class LegacyDatabaseAdapter : IDbConnection
{
    private readonly LegacyDatabaseClient _legacyClient;

    public LegacyDatabaseAdapter(LegacyDatabaseClient legacyClient)
    {
        _legacyClient = legacyClient;
    }

    public void Open()
    {
        _legacyClient.Connect(); // Adapt Connect() to Open()
    }
}
```
- **Third-Party Library Integration**: Adapting external payment gateways, logging libraries, or cloud services
- **Data Format Conversion**: XML/JSON readers adapted to common data reader interface
- **Stream Adapters**: `StreamReader` and `StreamWriter` adapt Stream to text-based interface
- **Collections**: `IEnumerator<T>` adapts various collection types to unified iteration interface
- **ASP.NET Core Middleware**: Adapting OWIN middleware to ASP.NET Core pipeline
- **Database Providers**: ADO.NET providers adapt different databases (SQL Server, MySQL, PostgreSQL) to common `DbConnection` interface

## 5. Modern Approach: Dependency Injection with Adapters
```csharp
// Target interface expected by client
public interface IPaymentProcessor
{
    Task<PaymentResult> ProcessPaymentAsync(decimal amount, string currency);
}

// Third-party service with incompatible interface (Adaptee)
public class StripePaymentService
{
    public async Task<StripeResponse> ChargeAsync(int amountInCents, string currencyCode)
    {
        // Stripe-specific implementation
        await Task.Delay(100);
        return new StripeResponse { Success = true };
    }
}

// Adapter translates IPaymentProcessor to StripePaymentService
public class StripePaymentAdapter : IPaymentProcessor
{
    private readonly StripePaymentService _stripeService;
    private readonly ILogger<StripePaymentAdapter> _logger;

    public StripePaymentAdapter(
        StripePaymentService stripeService,
        ILogger<StripePaymentAdapter> logger)
    {
        _stripeService = stripeService;
        _logger = logger;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(decimal amount, string currency)
    {
        // Convert from dollars to cents (Stripe requires cents)
        var amountInCents = (int)(amount * 100);

        _logger.LogInformation("Processing payment via Stripe: {Amount} {Currency}",
            amount, currency);

        var response = await _stripeService.ChargeAsync(amountInCents, currency);

        return new PaymentResult
        {
            Success = response.Success,
            Provider = "Stripe"
        };
    }
}

// Register in Program.cs
builder.Services.AddScoped<StripePaymentService>();
builder.Services.AddScoped<IPaymentProcessor, StripePaymentAdapter>();

// Client code works with IPaymentProcessor interface
public class CheckoutService
{
    private readonly IPaymentProcessor _paymentProcessor;

    public CheckoutService(IPaymentProcessor paymentProcessor)
    {
        _paymentProcessor = paymentProcessor; // Unaware it's an adapter
    }

    public async Task CompleteCheckout(decimal amount)
    {
        await _paymentProcessor.ProcessPaymentAsync(amount, "USD");
    }
}
```

**Benefits over traditional Adapter:**
- Automatic dependency injection of adapted services
- Easy to swap adapters by changing DI registration
- Better testability with mock adapters
- Integration with logging, configuration, and other services
- Lifetime management handled by DI container

## 6. Expert Advice: When to Use vs When Not to Use

### ✅ When to Use Adapter:
- **Third-Party Library Integration**: Incompatible external library needs to work with your application
- **Legacy Code Modernization**: Old code with outdated interfaces needs to work with new systems
- **Interface Mismatch**: Two classes should work together but have incompatible interfaces
- **Multiple Data Sources**: Different data sources (XML, JSON, CSV) need unified access interface
- **API Versioning**: Adapting old API versions to new interface contracts
- **Testing**: Creating test adapters to simulate external dependencies
- **Platform Abstraction**: Adapting platform-specific APIs to cross-platform interfaces

### ❌ When NOT to Use Adapter:
- **You Control Both Interfaces**: Refactor interfaces directly instead of adapting
- **Simple Wrapper Needed**: Use delegation or simple wrapper classes
- **Performance Critical**: The adapter overhead is unacceptable
- **Over-Engineering**: When direct usage is simpler and sufficient
- **One-Time Integration**: For single-use scenarios, direct implementation may be clearer
- **Extensive Interface Mismatch**: When interfaces are too different, consider Facade or Bridge instead

### 🎯 Expert Recommendations:
- **Prefer Composition over Inheritance**: Use object adapter (composition) instead of class adapter (inheritance)
```csharp
// Good: Object Adapter (composition)
public class Adapter : ITarget
{
    private readonly Adaptee _adaptee; // Composition

    public Adapter(Adaptee adaptee)
    {
        _adaptee = adaptee;
    }
}

// Avoid: Class Adapter (multiple inheritance - not supported in C#)
// C# doesn't support multiple inheritance, so object adapter is preferred
```
- **Keep Adapters Thin**: Adapters should only translate interfaces, not add business logic
- **Document Adaptations**: Clearly comment what conversions/transformations the adapter performs
- **Use Extension Methods for Simple Adaptations**: For simple interface additions, consider extension methods
```csharp
public static class LegacyServiceExtensions
{
    public static Task<Result> ProcessAsync(this LegacyService service, Request request)
    {
        // Adapt synchronous legacy method to async
        return Task.FromResult(service.Process(request));
    }
}
```
- **Consider Two-Way Adapters**: When bidirectional communication is needed
- **Handle Null/Error Cases**: Adapters should handle edge cases during translation
- **Version Compatibility**: Design adapters to handle version differences gracefully
- **Use Adapter per Incompatibility**: Create separate adapters for different interface mismatches
- **Leverage AutoMapper**: For complex data transformations between incompatible models
