# Bridge Pattern
## 1. Definition
The Bridge Pattern decouples an abstraction from its implementation so that the two can vary independently. It separates the interface (abstraction) from the implementation by creating a bridge between them through composition. This pattern is particularly useful when both the abstractions and their implementations should be extensible through subclassing, preventing a combinatorial explosion of classes.

### Manual Implementation
1. Identify two orthogonal dimensions that can vary independently (e.g., shape and color, device and remote control)
2. Define an Implementation interface that declares low-level operations
3. Create Concrete Implementation classes that implement the interface
4. Define an Abstraction class that contains a reference to an Implementation object
5. Create Refined Abstraction classes that extend the Abstraction with higher-level operations
6. Client code works with the Abstraction and can combine any abstraction with any implementation

## 2. Pros
- **Decoupling**: Separates interface from implementation, allowing independent development
- **Reduced Class Explosion**: Avoids creating classes for every combination of abstraction and implementation
- **Open/Closed Principle**: Can extend abstractions and implementations independently without affecting each other
- **Single Responsibility**: Abstractions focus on high-level logic, implementations focus on platform-specific details
- **Runtime Binding**: Can switch implementations at runtime through composition
- **Platform Independence**: Abstractions work across different platforms by swapping implementations

## 3. Cons
- **Increased Complexity**: Introduces additional layers of abstraction and indirection
- **Design Overhead**: More complex than simple inheritance hierarchies for simple scenarios
- **Harder to Understand**: The separation between abstraction and implementation can be confusing
- **More Classes**: Requires creating separate hierarchies for abstractions and implementations
- **Potential Over-Engineering**: May be unnecessary when only one implementation exists

## 4. Real-world Use Cases in C# & .NET
- **Cross-Platform UI**: Separating UI controls (abstraction) from platform-specific rendering (implementation)
```csharp
// Abstraction: Button control
public abstract class Button
{
    protected IRenderer _renderer; // Bridge to implementation

    public Button(IRenderer renderer)
    {
        _renderer = renderer;
    }

    public abstract void Render();
}

// Implementations: Different rendering engines
public interface IRenderer
{
    void RenderButton(string text);
}

public class WindowsRenderer : IRenderer
{
    public void RenderButton(string text)
    {
        Console.WriteLine($"[Windows] Rendering button: {text}");
    }
}

public class LinuxRenderer : IRenderer
{
    public void RenderButton(string text)
    {
        Console.WriteLine($"[Linux] Rendering button: {text}");
    }
}

// Can now have any button type work with any renderer
var button = new RoundButton(new WindowsRenderer());
```
- **Database Abstraction**: Repository pattern (abstraction) with different database providers (implementation)
- **Messaging Systems**: Message sender (abstraction) with different channels (email, SMS, push notification)
- **Graphics API**: Shape (abstraction) with different rendering APIs (DirectX, OpenGL, Vulkan)
- **Remote Controls and Devices**: Remote control types (abstraction) controlling different devices (implementation)
- **Logging Frameworks**: Logger (abstraction) with different log targets (file, console, database)
- **Payment Processing**: Payment workflow (abstraction) with different payment gateways (implementation)

## 5. Modern Approach: Dependency Injection with Bridge
```csharp
// Implementation interface - the bridge interface
public interface INotificationChannel
{
    Task SendAsync(string recipient, string message);
}

// Concrete implementations
public class EmailChannel : INotificationChannel
{
    private readonly IEmailService _emailService;

    public EmailChannel(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task SendAsync(string recipient, string message)
    {
        await _emailService.SendEmailAsync(recipient, "Notification", message);
    }
}

public class SmsChannel : INotificationChannel
{
    private readonly ISmsService _smsService;

    public SmsChannel(ISmsService smsService)
    {
        _smsService = smsService;
    }

    public async Task SendAsync(string recipient, string message)
    {
        await _smsService.SendSmsAsync(recipient, message);
    }
}

// Abstraction - high-level notification logic
public abstract class Notification
{
    protected readonly INotificationChannel _channel;

    protected Notification(INotificationChannel channel)
    {
        _channel = channel;
    }

    public abstract Task SendAsync(string recipient);
}

// Refined abstractions
public class UrgentNotification : Notification
{
    public UrgentNotification(INotificationChannel channel) : base(channel)
    {
    }

    public override async Task SendAsync(string recipient)
    {
        // Add urgent-specific logic
        await _channel.SendAsync(recipient, "[URGENT] This requires immediate attention!");
    }
}

public class RegularNotification : Notification
{
    public RegularNotification(INotificationChannel channel) : base(channel)
    {
    }

    public override async Task SendAsync(string recipient)
    {
        await _channel.SendAsync(recipient, "This is a regular notification.");
    }
}

// Register in Program.cs
builder.Services.AddScoped<INotificationChannel, EmailChannel>();
builder.Services.AddScoped<INotificationChannel, SmsChannel>();

// Factory to select channel
public class NotificationFactory
{
    private readonly IServiceProvider _serviceProvider;

    public NotificationFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Notification CreateNotification(NotificationType type, ChannelType channel)
    {
        var channelImpl = channel switch
        {
            ChannelType.Email => _serviceProvider.GetRequiredService<EmailChannel>(),
            ChannelType.Sms => _serviceProvider.GetRequiredService<SmsChannel>(),
            _ => throw new ArgumentException("Unknown channel")
        };

        return type switch
        {
            NotificationType.Urgent => new UrgentNotification(channelImpl),
            NotificationType.Regular => new RegularNotification(channelImpl),
            _ => throw new ArgumentException("Unknown type")
        };
    }
}

// Client usage
var notification = factory.CreateNotification(NotificationType.Urgent, ChannelType.Email);
await notification.SendAsync("user@example.com");
```

**Benefits over traditional Bridge:**
- Automatic dependency injection for implementations
- Easy to add new implementations or abstractions
- Better testability with mock channels
- Integration with logging, configuration, and other services
- Lifetime management handled by DI container

## 6. Expert Advice: When to Use vs When Not to Use

### ✅ When to Use Bridge:
- **Multiple Dimensions of Variation**: Two or more aspects that can change independently (UI control types × platforms)
- **Avoiding Class Explosion**: Prevent creating classes for every combination (5 controls × 3 platforms = 15 classes vs 5 + 3 = 8 classes)
- **Platform Abstraction**: Supporting multiple platforms, databases, or external services
- **Runtime Implementation Switching**: Need to change implementation dynamically
- **Share Implementation Across Abstractions**: Multiple abstractions use the same implementations
- **Independent Extension**: Both abstractions and implementations need to be extended separately
- **Legacy System Integration**: New abstraction layer over legacy implementations

### ❌ When NOT to Use Bridge:
- **Single Implementation**: Only one implementation exists and is unlikely to change
- **Single Abstraction**: Only one abstraction hierarchy exists
- **Simple Inheritance Sufficient**: When inheritance alone handles the variation adequately
- **Performance Critical**: The extra indirection layer is unacceptable
- **Over-Engineering**: Adds unnecessary complexity for straightforward scenarios
- **Tight Coupling Required**: When abstraction and implementation must be tightly coupled

### 🎯 Expert Recommendations:
- **Identify Orthogonal Variations**: Look for two independent dimensions of change
```csharp
// Good use of Bridge: Shape and Color are independent
// Without Bridge: RedCircle, BlueCircle, RedSquare, BlueSquare (4 classes)
// With Bridge: Circle, Square (2) + Red, Blue (2) = 4 classes total

public abstract class Shape
{
    protected IColor _color; // Bridge

    public Shape(IColor color)
    {
        _color = color;
    }
}
```
- **Favor Composition over Inheritance**: Bridge uses composition to achieve flexibility
- **Use Abstract Class for Abstraction**: Allows sharing common code across refined abstractions
- **Keep Bridge Interface Focused**: Implementation interface should contain low-level primitive operations
- **Document the Bridge**: Clearly indicate which reference is the bridge to implementation
- **Consider Strategy Pattern**: If you only need to vary the algorithm, Strategy might be simpler
- **Bridge vs Adapter**:
  - Bridge: Designed upfront to separate abstraction and implementation
  - Adapter: Applied to make incompatible interfaces work together
- **Thread Safety**: Consider thread safety when implementations are shared
- **Combine with Factory**: Use Factory or Abstract Factory to create abstraction-implementation pairs
- **Validate Combinations**: Ensure all abstraction-implementation combinations make sense
