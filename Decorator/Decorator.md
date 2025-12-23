# Decorator Pattern
## 1. Definition
The Decorator Pattern attaches additional responsibilities to an object dynamically. Decorators provide a flexible alternative to subclassing for extending functionality. By wrapping objects in decorator objects, you can add new behaviors at runtime without modifying the original class, adhering to the Open/Closed Principle.

### Manual Implementation
1. Define a Component interface or abstract class declaring operations
2. Create ConcreteComponent class implementing the base functionality
3. Create an abstract Decorator class that implements Component and contains a reference to a Component
4. Create ConcreteDecorator classes that extend Decorator and add new behaviors
5. Decorators wrap components and can be stacked (decorator wrapping another decorator)
6. Client code creates component and wraps it with desired decorators at runtime

## 2. Pros
- **Open/Closed Principle**: Add new functionality without modifying existing code
- **Single Responsibility**: Each decorator focuses on one additional behavior
- **Runtime Flexibility**: Add/remove behaviors dynamically at runtime
- **Composable Behaviors**: Stack multiple decorators for combined functionality
- **Alternative to Subclassing**: Avoid class explosion from multiple feature combinations
- **Fine-Grained Control**: Choose which behaviors to add to specific instances

## 3. Cons
- **Complexity**: Many small decorator classes can make code harder to understand
- **Order Dependency**: Decorator order matters and can be confusing
- **Identity Issues**: Decorated object differs from original (type checking problems)
- **Harder to Remove**: Difficult to remove specific decorator from wrapper stack
- **Debugging Difficulty**: Multiple wrapper layers make debugging and stack traces more complex
- **Configuration Complexity**: Setting up decorator chains can be verbose

## 4. Real-world Use Cases in C# & .NET
- **I/O Streams**: BufferedStream, GZipStream, CryptoStream wrapping base streams
```csharp
// .NET Stream decorators - classic example
using var fileStream = new FileStream("data.txt", FileMode.Open);
using var bufferedStream = new BufferedStream(fileStream); // Add buffering
using var gzipStream = new GZipStream(bufferedStream, CompressionMode.Decompress); // Add compression
using var reader = new StreamReader(gzipStream); // Add text reading

var content = reader.ReadToEnd();
```
- **ASP.NET Core Middleware**: Middleware components wrap the request pipeline
```csharp
// Middleware decorates the request pipeline
app.UseAuthentication(); // Add authentication
app.UseAuthorization();  // Add authorization
app.UseResponseCompression(); // Add compression
```
- **Caching Decorators**: Add caching behavior to repositories or services
- **Logging Decorators**: Add logging around method calls
- **Validation Decorators**: Add validation before delegating to wrapped component
- **Retry/Circuit Breaker**: Add resilience patterns around services
- **Performance Monitoring**: Add telemetry/metrics collection

## 5. Modern Approach: Decorator with Dependency Injection
```csharp
// Component interface
public interface INotificationService
{
    Task SendAsync(string recipient, string message);
}

// Concrete component - basic implementation
public class EmailNotificationService : INotificationService
{
    private readonly IEmailClient _emailClient;

    public EmailNotificationService(IEmailClient emailClient)
    {
        _emailClient = emailClient;
    }

    public async Task SendAsync(string recipient, string message)
    {
        await _emailClient.SendEmailAsync(recipient, message);
    }
}

// Decorator 1: Add logging
public class LoggingNotificationDecorator : INotificationService
{
    private readonly INotificationService _inner;
    private readonly ILogger<LoggingNotificationDecorator> _logger;

    public LoggingNotificationDecorator(
        INotificationService inner,
        ILogger<LoggingNotificationDecorator> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task SendAsync(string recipient, string message)
    {
        _logger.LogInformation("Sending notification to {Recipient}", recipient);
        try
        {
            await _inner.SendAsync(recipient, message);
            _logger.LogInformation("Notification sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification");
            throw;
        }
    }
}

// Decorator 2: Add retry logic
public class RetryNotificationDecorator : INotificationService
{
    private readonly INotificationService _inner;
    private readonly int _maxRetries;

    public RetryNotificationDecorator(INotificationService inner, int maxRetries = 3)
    {
        _inner = inner;
        _maxRetries = maxRetries;
    }

    public async Task SendAsync(string recipient, string message)
    {
        for (int i = 0; i < _maxRetries; i++)
        {
            try
            {
                await _inner.SendAsync(recipient, message);
                return;
            }
            catch when (i < _maxRetries - 1)
            {
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i))); // Exponential backoff
            }
        }
    }
}

// Register in Program.cs with Scrutor for automatic decoration
builder.Services.AddScoped<INotificationService, EmailNotificationService>();
builder.Services.Decorate<INotificationService, LoggingNotificationDecorator>();
builder.Services.Decorate<INotificationService, RetryNotificationDecorator>();

// Or manual registration
builder.Services.AddScoped<INotificationService>(provider =>
{
    var emailClient = provider.GetRequiredService<IEmailClient>();
    var logger = provider.GetRequiredService<ILogger<LoggingNotificationDecorator>>();

    // Build decorator chain
    INotificationService service = new EmailNotificationService(emailClient);
    service = new LoggingNotificationDecorator(service, logger);
    service = new RetryNotificationDecorator(service);

    return service;
});
```

**Benefits over traditional Decorator:**
- Automatic dependency injection into decorators
- Easy to add/remove decorators by changing registration
- Better testability with mock decorators
- Integration with logging, configuration, and other services
- Clean separation of concerns

## 6. Expert Advice: When to Use vs When Not to Use

### ✅ When to Use Decorator:
- **Dynamic Behavior Addition**: Need to add responsibilities at runtime
- **Avoiding Subclass Explosion**: Too many feature combinations would require many subclasses
- **Optional Features**: Features that should be selectively added to instances
- **Composable Behaviors**: Behaviors that can be combined in various ways
- **Cross-Cutting Concerns**: Logging, caching, validation, retry logic, monitoring
- **Extending Third-Party Classes**: Can't modify the original class
- **Reversible Extensions**: Need to add/remove behaviors dynamically

### ❌ When NOT to Use Decorator:
- **Simple Inheritance Sufficient**: When simple subclassing works fine
- **Static Behavior**: Behaviors known at compile-time that don't change
- **Type Checking Required**: When code relies on checking concrete types
- **Performance Critical**: Wrapper layers add overhead
- **Single Behavior**: Only one additional behavior needed
- **Complex Dependencies**: Decorators have complex initialization requirements

### 🎯 Expert Recommendations:
- **Keep Decorators Focused**: Each decorator should add one clear responsibility
```csharp
// Good: Single responsibility
public class CachingDecorator : IRepository
{
    private readonly IRepository _inner;
    private readonly ICache _cache;

    public async Task<User> GetUserAsync(int id)
    {
        var cacheKey = $"user:{id}";
        if (_cache.TryGet(cacheKey, out User? cached))
            return cached!;

        var user = await _inner.GetUserAsync(id);
        _cache.Set(cacheKey, user);
        return user;
    }
}

// Avoid: Multiple responsibilities
// Don't combine caching + logging + validation in one decorator
```
- **Document Decorator Order**: Order matters, document the correct sequence
```csharp
// Correct order: Logging -> Retry -> Cache -> Service
// This logs retries and caches after successful retry
IService service = new ConcreteService();
service = new CachingDecorator(service);
service = new RetryDecorator(service);
service = new LoggingDecorator(service);
```
- **Use Interfaces, Not Abstract Classes**: Prefer composition over inheritance
- **Make Decorators Transparent**: Decorator should preserve the component's interface completely
- **Consider Scrutor Package**: For automatic decorator registration in ASP.NET Core
```csharp
builder.Services.AddScoped<INotificationService, EmailNotificationService>();
builder.Services.Decorate<INotificationService, LoggingNotificationDecorator>();
```
- **Implement IDisposable Carefully**: Decorators should dispose wrapped components
```csharp
public class DisposableDecorator : IService, IDisposable
{
    private readonly IService _inner;

    public void Dispose()
    {
        (_inner as IDisposable)?.Dispose();
    }
}
```
- **Avoid Deep Nesting**: Too many decorators hurt performance and debugging
- **Use Decorator for Cross-Cutting**: Perfect for logging, caching, validation, retry logic
- **Consider Alternatives**:
  - **Proxy Pattern**: When you need lazy initialization or access control
  - **Chain of Responsibility**: When handlers can pass requests to the next handler
  - **Aspect-Oriented Programming (AOP)**: For very generic cross-cutting concerns
- **Test Each Decorator Independently**: Unit test decorators with mock inner components
- **Use Factory for Complex Chains**: Encapsulate decorator chain creation
```csharp
public class NotificationServiceFactory
{
    public INotificationService CreateWithLoggingAndRetry(IEmailClient emailClient, ILogger logger)
    {
        INotificationService service = new EmailNotificationService(emailClient);
        service = new LoggingNotificationDecorator(service, logger);
        service = new RetryNotificationDecorator(service, maxRetries: 3);
        return service;
    }
}
```
