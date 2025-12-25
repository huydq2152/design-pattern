# Facade Pattern

## 1. Definition

The Facade pattern provides a simplified, unified interface to a complex subsystem or set of interfaces. It defines a higher-level interface that makes the subsystem easier to use by hiding the complexity and coordinating the interactions between multiple subsystem components.

**How it works:**
- **Create a Facade class** that provides simple methods for common operations
- **The Facade delegates** client requests to appropriate subsystem objects
- **Subsystems remain accessible** - clients can still use them directly if needed (the facade is optional)
- **Coordinates complex workflows** by orchestrating multiple subsystem calls in the correct order

The Facade pattern doesn't encapsulate the subsystems; it just provides a convenient interface to them. Clients can still access subsystem classes directly when they need fine-grained control.

## 2. Pros

- **Simplified Interface**: Reduces complexity by providing a simple interface to complex subsystems
- **Loose Coupling**: Isolates clients from subsystem components, reducing dependencies
- **Layered Architecture**: Helps structure your application into layers, promoting better organization
- **Easier to Use**: Clients work with one simple interface instead of multiple complex ones
- **Flexibility**: Subsystems can evolve independently without affecting clients who use the facade
- **Single Entry Point**: Provides a clear entry point for subsystem functionality
- **Reduces Learning Curve**: New developers can use the facade without understanding all subsystem details
- **Optional**: Clients can bypass the facade and use subsystems directly when needed

## 3. Cons

- **God Object Risk**: Facade can become a "god object" that knows too much if not designed carefully
- **Limited Functionality**: May hide useful subsystem features if the facade is too simple
- **Additional Layer**: Adds another level of indirection, slightly increasing complexity
- **Maintenance Overhead**: Changes in subsystems might require updating the facade
- **Not Always Necessary**: For simple systems, a facade might be overkill
- **Multiple Facades**: Complex systems might need multiple facades, adding to the overall design
- **Tight Coupling to Subsystems**: The facade itself is coupled to many subsystems (though clients aren't)

## 4. Real-world Use Cases in C# & .NET

### E-Commerce Order Processing
```csharp
// Without Facade - Client must coordinate multiple subsystems
public class OrderController
{
    public void ProcessOrder(Order order)
    {
        // Client must know about and coordinate all these subsystems
        var cartService = new ShoppingCartService();
        var inventoryService = new InventoryService();
        var paymentService = new PaymentService();
        var shippingService = new ShippingService();
        var notificationService = new NotificationService();

        // Client must know the correct order of operations
        cartService.ValidateCart(order.CartId);
        inventoryService.ReserveItems(order.Items);
        var paymentResult = paymentService.ProcessPayment(order.Payment);
        if (paymentResult.Success)
        {
            inventoryService.DeductItems(order.Items);
            shippingService.CreateShipment(order);
            notificationService.SendOrderConfirmation(order);
        }
        else
        {
            inventoryService.ReleaseReservation(order.Items);
        }
    }
}

// With Facade - Simple, clean interface
public class OrderProcessingFacade
{
    private readonly ShoppingCartService _cartService;
    private readonly InventoryService _inventoryService;
    private readonly PaymentService _paymentService;
    private readonly ShippingService _shippingService;
    private readonly NotificationService _notificationService;

    public OrderProcessingFacade(/* inject services */) { /* ... */ }

    public async Task<OrderResult> ProcessOrder(Order order)
    {
        // Facade coordinates all the complex subsystem interactions
        await _cartService.ValidateCart(order.CartId);
        await _inventoryService.ReserveItems(order.Items);

        var paymentResult = await _paymentService.ProcessPayment(order.Payment);

        if (paymentResult.Success)
        {
            await _inventoryService.DeductItems(order.Items);
            await _shippingService.CreateShipment(order);
            await _notificationService.SendOrderConfirmation(order);
            return OrderResult.Success(order.Id);
        }

        await _inventoryService.ReleaseReservation(order.Items);
        return OrderResult.Failure(paymentResult.Error);
    }
}

// Clean client code
public class OrderController
{
    private readonly OrderProcessingFacade _facade;

    public OrderController(OrderProcessingFacade facade)
    {
        _facade = facade;
    }

    public async Task<IActionResult> PlaceOrder(Order order)
    {
        // Single, simple call instead of complex coordination
        var result = await _facade.ProcessOrder(order);
        return result.Success ? Ok(result) : BadRequest(result.Error);
    }
}
```

### .NET Framework Examples
The .NET ecosystem uses the Facade pattern extensively:

**DbContext in Entity Framework**
```csharp
// DbContext is a facade over complex database operations
public class ApplicationDbContext : DbContext
{
    // Facade simplifies: connection management, transaction handling,
    // query translation, change tracking, and more
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
}

// Client code is simple
using var context = new ApplicationDbContext();
var customers = context.Customers.Where(c => c.IsActive).ToList();
// Behind the scenes: connection management, SQL generation,
// parameter handling, object materialization, change tracking
```

**HttpClient in .NET**
```csharp
// HttpClient is a facade over complex HTTP operations
// Simplifies: connection pooling, header management, content serialization,
// request/response handling, timeout management
var client = new HttpClient();
var response = await client.GetStringAsync("https://api.example.com/data");
```

**IServiceProvider in Dependency Injection**
```csharp
// IServiceProvider is a facade over service resolution
// Hides complexity of: lifetime management, dependency graphs,
// disposal, scope management
var service = serviceProvider.GetRequiredService<IMyService>();
```

### Video Conversion Application
```csharp
public class VideoConverterFacade
{
    private readonly VideoCodec _videoCodec;
    private readonly AudioCodec _audioCodec;
    private readonly BitrateOptimizer _optimizer;
    private readonly FormatConverter _formatConverter;

    public VideoConverterFacade()
    {
        _videoCodec = new VideoCodec();
        _audioCodec = new AudioCodec();
        _optimizer = new BitrateOptimizer();
        _formatConverter = new FormatConverter();
    }

    public File ConvertVideo(string filename, string format)
    {
        // Simple interface hides complex conversion workflow
        var file = new VideoFile(filename);
        var sourceCodec = CodecFactory.Extract(file);

        File destinationFile;
        if (format == "mp4")
        {
            destinationFile = _videoCodec.Decode(file, sourceCodec);
            destinationFile = _audioCodec.Decode(destinationFile);
            destinationFile = _optimizer.Optimize(destinationFile);
            destinationFile = _formatConverter.Convert(destinationFile, "mp4");
        }
        else
        {
            destinationFile = _formatConverter.Convert(file, format);
        }

        return destinationFile;
    }
}

// Clean client usage
var converter = new VideoConverterFacade();
var convertedFile = converter.ConvertVideo("video.avi", "mp4");
```

## 5. Modern Approach: Dependency Injection

In modern C# and ASP.NET Core applications, facades are typically integrated with the dependency injection container for better testability and flexibility.

```csharp
// Define the facade interface (optional but recommended for testing)
public interface IOrderProcessingFacade
{
    Task<OrderResult> ProcessOrder(Order order);
    Task<OrderResult> CancelOrder(int orderId);
    Task<RefundResult> ProcessRefund(int orderId);
}

// Implement the facade with dependency injection
public class OrderProcessingFacade : IOrderProcessingFacade
{
    private readonly IShoppingCartService _cartService;
    private readonly IInventoryService _inventoryService;
    private readonly IPaymentService _paymentService;
    private readonly IShippingService _shippingService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<OrderProcessingFacade> _logger;

    // All subsystems injected via constructor
    public OrderProcessingFacade(
        IShoppingCartService cartService,
        IInventoryService inventoryService,
        IPaymentService paymentService,
        IShippingService shippingService,
        INotificationService notificationService,
        ILogger<OrderProcessingFacade> logger)
    {
        _cartService = cartService;
        _inventoryService = inventoryService;
        _paymentService = paymentService;
        _shippingService = shippingService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<OrderResult> ProcessOrder(Order order)
    {
        _logger.LogInformation("Processing order {OrderId}", order.Id);

        try
        {
            // Coordinate subsystems with proper error handling
            await _cartService.ValidateCart(order.CartId);
            await _inventoryService.ReserveItems(order.Items);

            var paymentResult = await _paymentService.ProcessPayment(order.Payment);

            if (!paymentResult.Success)
            {
                await _inventoryService.ReleaseReservation(order.Items);
                return OrderResult.Failure(paymentResult.Error);
            }

            await _inventoryService.DeductItems(order.Items);
            await _shippingService.CreateShipment(order);
            await _notificationService.SendOrderConfirmation(order);

            _logger.LogInformation("Order {OrderId} processed successfully", order.Id);
            return OrderResult.Success(order.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order {OrderId}", order.Id);
            throw;
        }
    }

    public async Task<OrderResult> CancelOrder(int orderId)
    {
        // Another high-level operation coordinating subsystems
        // ...
    }

    public async Task<RefundResult> ProcessRefund(int orderId)
    {
        // Yet another workflow coordination
        // ...
    }
}

// Register in Program.cs (ASP.NET Core)
var builder = WebApplication.CreateBuilder(args);

// Register all subsystem services
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IShippingService, ShippingService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Register the facade
builder.Services.AddScoped<IOrderProcessingFacade, OrderProcessingFacade>();

var app = builder.Build();

// Clean controller code
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderProcessingFacade _orderFacade;

    public OrdersController(IOrderProcessingFacade orderFacade)
    {
        _orderFacade = orderFacade;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] Order order)
    {
        var result = await _orderFacade.ProcessOrder(order);
        return result.Success ? Ok(result) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var result = await _orderFacade.CancelOrder(id);
        return result.Success ? Ok(result) : BadRequest(result.Error);
    }
}
```

**Benefits of DI Integration:**
- **Testability**: Easy to mock the facade or its dependencies in unit tests
- **Flexibility**: Can swap implementations without changing client code
- **Lifetime Management**: Container manages object creation and disposal
- **Configuration**: Can configure subsystems through appsettings.json
- **Separation of Concerns**: Clear dependency graph and responsibility boundaries

## 6. Expert Advice: When to Use vs When Not to Use

### ✅ When to Use Facade:

- **Complex Subsystem with Multiple Classes**: You have a library or subsystem with many interdependent classes that are difficult to use together
  ```csharp
  // Good: Complex video processing with many codecs, formats, optimizers
  var facade = new VideoConverterFacade();
  facade.ConvertVideo("input.avi", "mp4");
  ```

- **Need to Layer Your System**: You want to define entry points to different layers in your architecture
  ```csharp
  // Good: Data access facade separates business logic from data layer
  public class DataAccessFacade
  {
      private readonly IRepository<Customer> _customerRepo;
      private readonly IRepository<Order> _orderRepo;
      private readonly IUnitOfWork _unitOfWork;
      // Provides simple CRUD operations hiding repository complexity
  }
  ```

- **Third-Party Library with Poor API**: You're working with a complex third-party library and want to simplify its usage
  ```csharp
  // Good: Wrap complex XML parsing library with simple facade
  public class XmlParserFacade
  {
      public T ParseXml<T>(string xml) { /* hide complexity */ }
  }
  ```

- **Multiple Client Types**: Different clients need different levels of abstraction over the same subsystems
  ```csharp
  // Good: AdminFacade and CustomerFacade over same subsystems
  public class AdminOrderFacade { /* admin operations */ }
  public class CustomerOrderFacade { /* customer operations */ }
  ```

- **Reducing Dependencies**: You want to reduce the number of classes client code depends on

### ❌ When NOT to Use Facade:

- **Simple Subsystem**: Your subsystem is already simple and easy to use
  ```csharp
  // Bad: Unnecessary facade for simple service
  public class UserServiceFacade
  {
      private readonly UserService _userService;

      // Just wrapping a single simple call - no value added
      public User GetUser(int id) => _userService.GetUser(id);
  }
  ```

- **One-to-One Wrapper**: You're just wrapping a single class without coordinating multiple subsystems
  ```csharp
  // Bad: This is just a wrapper, not a facade
  public class LoggerFacade
  {
      private ILogger _logger;
      public void Log(string message) => _logger.LogInformation(message);
  }
  // Better: Use the logger directly or use an adapter if changing interface
  ```

- **Frequently Changing Requirements**: The facade would need constant updates due to changing requirements

- **Need Full Control**: Clients frequently need fine-grained control that a simplified interface can't provide

- **Performance Critical Paths**: The extra indirection matters (though usually negligible)

### 🎯 Expert Recommendations:

**1. Keep Facades Focused**
```csharp
// Good: Focused facades for different workflows
public class OrderCheckoutFacade { /* checkout workflow */ }
public class OrderRefundFacade { /* refund workflow */ }
public class OrderTrackingFacade { /* tracking workflow */ }

// Bad: God object facade doing too much
public class OrderFacade
{
    // Checkout, refund, tracking, reporting, analytics, etc.
    // Too many responsibilities - violates SRP
}
```

**2. Don't Hide Everything**
```csharp
// Good: Facade for complex workflows, direct access still possible
public class VideoConverterFacade
{
    public File ConvertToMp4(string filename) { /* high-level */ }

    // Expose subsystems for advanced users
    public VideoCodec VideoCodec { get; }
    public AudioCodec AudioCodec { get; }
}

// Clients can use simple interface or access subsystems directly
var simple = facade.ConvertToMp4("video.avi");
var advanced = facade.VideoCodec.Decode(file, customCodec);
```

**3. Use Interfaces for Facades**
```csharp
// Good: Interface makes testing and mocking easier
public interface IOrderProcessingFacade
{
    Task<OrderResult> ProcessOrder(Order order);
}

// Can easily mock in tests
var mockFacade = new Mock<IOrderProcessingFacade>();
```

**4. Consider Multiple Facades**
```csharp
// Good: Different facades for different user roles or use cases
public class AdminReportingFacade { /* admin reporting */ }
public class CustomerReportingFacade { /* customer reporting */ }

// Better than one complex facade trying to serve all roles
```

**5. Document What the Facade Hides**
```csharp
/// <summary>
/// Facade for order processing.
/// Coordinates: cart validation, inventory reservation, payment processing,
/// shipping creation, and notification sending.
/// </summary>
public class OrderProcessingFacade
{
    // Clear documentation helps users understand what's happening behind the scenes
}
```

**6. Combine with Other Patterns**
- **Factory + Facade**: Factory creates the facade with all its dependencies
- **Singleton + Facade**: Single facade instance for global subsystem access (use sparingly)
- **Strategy + Facade**: Facade coordinates different strategies for workflows
- **Template Method + Facade**: Facade uses template method for common workflow steps

**7. Layering Strategy**
```csharp
// Presentation Layer
[Controller] -> [IOrderFacade]

// Business Layer (Facade)
[OrderFacade] -> [CartService], [PaymentService], [ShippingService]

// Data Layer
[Services] -> [Repositories] -> [DbContext]

// Each layer can use facades to simplify the layer below
```

**Real-World Decision Flow:**
1. Do you have 3+ subsystems that need coordination? → Consider Facade
2. Do clients repeat the same complex subsystem interaction pattern? → Use Facade
3. Does the workflow involve specific ordering of subsystem calls? → Use Facade
4. Do you want to create an architectural layer? → Use Facade
5. Is it just wrapping one simple class? → Don't use Facade

The Facade pattern excels at simplifying complex subsystems and coordinating workflows. Use it when you want to provide a clean, simple interface to complex interactions, but avoid using it as just a pass-through wrapper for simple services.
