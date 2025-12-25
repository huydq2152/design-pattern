# Chain of Responsibility Pattern

## 1. Definition

The Chain of Responsibility pattern passes requests along a chain of handlers. Each handler in the chain decides either to process the request or to pass it to the next handler in the chain. This pattern decouples the sender of a request from its receivers by giving multiple objects a chance to handle the request.

**How it works:**
- **Create a Handler interface** declaring a method for handling requests and setting the next handler
- **Implement concrete handlers** that either process the request or forward it to the next handler
- **Build a chain** by linking handlers together (handler1 → handler2 → handler3)
- **Client sends request** to the first handler in the chain without knowing which handler will process it

The key principle is **sequential processing**: the request travels through the chain until a handler processes it or the chain ends. Handlers are independent and don't know about the chain structure.

## 2. Pros

- **Decoupling**: Sender doesn't need to know which handler will process the request
- **Flexibility**: Can add, remove, or reorder handlers dynamically at runtime
- **Single Responsibility**: Each handler focuses on one type of request
- **Open/Closed Principle**: Can add new handlers without modifying existing code
- **Dynamic Chain Building**: Chain structure can be configured at runtime
- **Responsibility Distribution**: Processing is distributed across multiple handlers
- **Reduced Coupling**: Handlers don't reference each other directly
- **Easier Testing**: Can test each handler independently

## 3. Cons

- **No Guarantee of Handling**: Request might not be handled if no handler processes it
- **Debugging Difficulty**: Hard to trace which handler processed the request
- **Performance Overhead**: Request traverses multiple handlers even if early handler could process it
- **Chain Configuration**: Must ensure chain is configured correctly
- **Runtime Errors**: Broken chain or infinite loops if not implemented carefully
- **Complex Flow**: Request flow is less obvious than direct method calls
- **Memory Usage**: Each handler holds reference to next handler
- **Potential for Misuse**: Can create overly complex chains

## 4. Real-world Use Cases in C# & .NET

### Expense Approval Workflow

```csharp
// Request object
public class ExpenseRequest
{
    public decimal Amount { get; init; }
    public string Description { get; init; }
    public string EmployeeName { get; init; }
}

// Handler interface
public interface IExpenseHandler
{
    IExpenseHandler SetNext(IExpenseHandler handler);
    string ProcessRequest(ExpenseRequest request);
}

// Base handler with chain logic
public abstract class ExpenseHandlerBase : IExpenseHandler
{
    private IExpenseHandler? _nextHandler;

    public IExpenseHandler SetNext(IExpenseHandler handler)
    {
        _nextHandler = handler;
        return handler; // Fluent interface
    }

    public virtual string ProcessRequest(ExpenseRequest request)
    {
        // Default: forward to next handler
        if (_nextHandler != null)
        {
            return _nextHandler.ProcessRequest(request);
        }

        return "Request not approved - exceeds all approval limits";
    }
}

// Concrete handlers with different approval limits
public class ManagerHandler : ExpenseHandlerBase
{
    private const decimal ApprovalLimit = 1000m;

    public override string ProcessRequest(ExpenseRequest request)
    {
        if (request.Amount <= ApprovalLimit)
        {
            return $"Manager approved: ${request.Amount} for {request.Description}";
        }

        Console.WriteLine($"Manager cannot approve ${request.Amount}, forwarding to Director");
        return base.ProcessRequest(request);
    }
}

public class DirectorHandler : ExpenseHandlerBase
{
    private const decimal ApprovalLimit = 5000m;

    public override string ProcessRequest(ExpenseRequest request)
    {
        if (request.Amount <= ApprovalLimit)
        {
            return $"Director approved: ${request.Amount} for {request.Description}";
        }

        Console.WriteLine($"Director cannot approve ${request.Amount}, forwarding to VP");
        return base.ProcessRequest(request);
    }
}

public class VPHandler : ExpenseHandlerBase
{
    private const decimal ApprovalLimit = 10000m;

    public override string ProcessRequest(ExpenseRequest request)
    {
        if (request.Amount <= ApprovalLimit)
        {
            return $"VP approved: ${request.Amount} for {request.Description}";
        }

        Console.WriteLine($"VP cannot approve ${request.Amount}, forwarding to CEO");
        return base.ProcessRequest(request);
    }
}

public class CEOHandler : ExpenseHandlerBase
{
    public override string ProcessRequest(ExpenseRequest request)
    {
        // CEO can approve any amount
        return $"CEO approved: ${request.Amount} for {request.Description}";
    }
}

// Usage
var manager = new ManagerHandler();
var director = new DirectorHandler();
var vp = new VPHandler();
var ceo = new CEOHandler();

// Build chain: Manager → Director → VP → CEO
manager.SetNext(director).SetNext(vp).SetNext(ceo);

// Send requests through the chain
var request1 = new ExpenseRequest { Amount = 500, Description = "Office supplies" };
Console.WriteLine(manager.ProcessRequest(request1));
// Output: Manager approved: $500 for Office supplies

var request2 = new ExpenseRequest { Amount = 3000, Description = "New computers" };
Console.WriteLine(manager.ProcessRequest(request2));
// Output: Manager cannot approve $3000, forwarding to Director
//         Director approved: $3000 for New computers

var request3 = new ExpenseRequest { Amount = 15000, Description = "Company retreat" };
Console.WriteLine(manager.ProcessRequest(request3));
// Output: Manager → Director → VP → CEO approved
```

### Form Validation Chain

```csharp
public interface IValidator
{
    IValidator SetNext(IValidator validator);
    ValidationResult Validate(UserRegistration registration);
}

public abstract class ValidatorBase : IValidator
{
    private IValidator? _nextValidator;

    public IValidator SetNext(IValidator validator)
    {
        _nextValidator = validator;
        return validator;
    }

    public virtual ValidationResult Validate(UserRegistration registration)
    {
        // Forward to next validator
        if (_nextValidator != null)
        {
            return _nextValidator.Validate(registration);
        }

        return ValidationResult.Success();
    }
}

// Concrete validators
public class RequiredFieldsValidator : ValidatorBase
{
    public override ValidationResult Validate(UserRegistration registration)
    {
        if (string.IsNullOrWhiteSpace(registration.Username))
            return ValidationResult.Failure("Username is required");

        if (string.IsNullOrWhiteSpace(registration.Email))
            return ValidationResult.Failure("Email is required");

        if (string.IsNullOrWhiteSpace(registration.Password))
            return ValidationResult.Failure("Password is required");

        // All required fields present, continue chain
        return base.Validate(registration);
    }
}

public class EmailFormatValidator : ValidatorBase
{
    public override ValidationResult Validate(UserRegistration registration)
    {
        if (!registration.Email.Contains("@"))
            return ValidationResult.Failure("Invalid email format");

        return base.Validate(registration);
    }
}

public class PasswordStrengthValidator : ValidatorBase
{
    public override ValidationResult Validate(UserRegistration registration)
    {
        if (registration.Password.Length < 8)
            return ValidationResult.Failure("Password must be at least 8 characters");

        if (!registration.Password.Any(char.IsUpper))
            return ValidationResult.Failure("Password must contain uppercase letter");

        if (!registration.Password.Any(char.IsDigit))
            return ValidationResult.Failure("Password must contain a number");

        return base.Validate(registration);
    }
}

public class UniqueUsernameValidator : ValidatorBase
{
    private readonly IUserRepository _userRepository;

    public UniqueUsernameValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public override ValidationResult Validate(UserRegistration registration)
    {
        if (_userRepository.UsernameExists(registration.Username))
            return ValidationResult.Failure("Username already taken");

        return base.Validate(registration);
    }
}

// Usage
var requiredFields = new RequiredFieldsValidator();
var emailFormat = new EmailFormatValidator();
var passwordStrength = new PasswordStrengthValidator();
var uniqueUsername = new UniqueUsernameValidator(userRepository);

// Build validation chain
requiredFields
    .SetNext(emailFormat)
    .SetNext(passwordStrength)
    .SetNext(uniqueUsername);

// Validate registration
var result = requiredFields.Validate(userRegistration);
if (!result.IsValid)
{
    Console.WriteLine($"Validation failed: {result.ErrorMessage}");
}
```

### .NET Framework Examples

**ASP.NET Core Middleware Pipeline**
```csharp
// ASP.NET Core middleware is a perfect example of Chain of Responsibility
public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        // Each middleware is a handler in the chain
        app.UseExceptionHandler("/Error");  // Handler 1
        app.UseHttpsRedirection();          // Handler 2
        app.UseStaticFiles();               // Handler 3
        app.UseRouting();                   // Handler 4
        app.UseAuthentication();            // Handler 5
        app.UseAuthorization();             // Handler 6
        app.UseEndpoints(endpoints =>       // Handler 7
        {
            endpoints.MapControllers();
        });
    }
}

// Custom middleware handler
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next; // Next handler in chain

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Process request
        Console.WriteLine($"Request: {context.Request.Path}");

        // Forward to next handler in chain
        await _next(context);

        // Post-process response
        Console.WriteLine($"Response: {context.Response.StatusCode}");
    }
}

// Register middleware
app.UseMiddleware<RequestLoggingMiddleware>();
```

**HttpClient DelegatingHandler Chain**
```csharp
// Custom handlers for HTTP client pipeline
public class LoggingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Sending request to {request.RequestUri}");

        // Forward to next handler
        var response = await base.SendAsync(request, cancellationToken);

        Console.WriteLine($"Received response: {response.StatusCode}");
        return response;
    }
}

public class RetryHandler : DelegatingHandler
{
    private readonly int _maxRetries = 3;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        for (int i = 0; i < _maxRetries; i++)
        {
            try
            {
                // Forward to next handler
                return await base.SendAsync(request, cancellationToken);
            }
            catch (HttpRequestException) when (i < _maxRetries - 1)
            {
                Console.WriteLine($"Request failed, retrying ({i + 1}/{_maxRetries})");
                await Task.Delay(1000 * (i + 1));
            }
        }

        throw new HttpRequestException("Max retries exceeded");
    }
}

public class AuthenticationHandler : DelegatingHandler
{
    private readonly string _apiKey;

    public AuthenticationHandler(string apiKey)
    {
        _apiKey = apiKey;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Add authentication header
        request.Headers.Add("X-Api-Key", _apiKey);

        // Forward to next handler
        return await base.SendAsync(request, cancellationToken);
    }
}

// Build handler chain
var httpClient = new HttpClient(
    new LoggingHandler
    {
        InnerHandler = new RetryHandler
        {
            InnerHandler = new AuthenticationHandler("api-key-123")
            {
                InnerHandler = new HttpClientHandler()
            }
        }
    });

// Chain: Logging → Retry → Authentication → HttpClientHandler
```

**Exception Handling Chain**
```csharp
// Custom exception handler chain
public interface IExceptionHandler
{
    IExceptionHandler SetNext(IExceptionHandler handler);
    bool Handle(Exception exception);
}

public abstract class ExceptionHandlerBase : IExceptionHandler
{
    private IExceptionHandler? _nextHandler;

    public IExceptionHandler SetNext(IExceptionHandler handler)
    {
        _nextHandler = handler;
        return handler;
    }

    public virtual bool Handle(Exception exception)
    {
        if (_nextHandler != null)
        {
            return _nextHandler.Handle(exception);
        }

        return false; // Unhandled
    }
}

public class ValidationExceptionHandler : ExceptionHandlerBase
{
    public override bool Handle(Exception exception)
    {
        if (exception is ValidationException validationEx)
        {
            Console.WriteLine($"Validation Error: {validationEx.Message}");
            return true; // Handled
        }

        return base.Handle(exception);
    }
}

public class NotFoundExceptionHandler : ExceptionHandlerBase
{
    public override bool Handle(Exception exception)
    {
        if (exception is NotFoundException notFoundEx)
        {
            Console.WriteLine($"Not Found: {notFoundEx.Message}");
            return true; // Handled
        }

        return base.Handle(exception);
    }
}

public class GeneralExceptionHandler : ExceptionHandlerBase
{
    public override bool Handle(Exception exception)
    {
        Console.WriteLine($"Unexpected Error: {exception.Message}");
        // Log to error tracking service
        return true; // All exceptions handled
    }
}

// Usage
var validationHandler = new ValidationExceptionHandler();
var notFoundHandler = new NotFoundExceptionHandler();
var generalHandler = new GeneralExceptionHandler();

validationHandler.SetNext(notFoundHandler).SetNext(generalHandler);

try
{
    // Some operation
}
catch (Exception ex)
{
    validationHandler.Handle(ex);
}
```

## 5. Modern Approach: Dependency Injection

In modern C# and ASP.NET Core applications, the Chain of Responsibility pattern is typically integrated with dependency injection.

```csharp
// Handler interface
public interface IRequestHandler<TRequest, TResponse>
{
    IRequestHandler<TRequest, TResponse>? Next { get; set; }
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}

// Base handler
public abstract class RequestHandlerBase<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
{
    public IRequestHandler<TRequest, TResponse>? Next { get; set; }

    public virtual async Task<TResponse> HandleAsync(
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        // Default: forward to next handler
        if (Next != null)
        {
            return await Next.HandleAsync(request, cancellationToken);
        }

        throw new InvalidOperationException("Request not handled by any handler");
    }
}

// Concrete handlers
public class ValidationHandler : RequestHandlerBase<OrderRequest, OrderResponse>
{
    private readonly ILogger<ValidationHandler> _logger;

    public ValidationHandler(ILogger<ValidationHandler> logger)
    {
        _logger = logger;
    }

    public override async Task<OrderResponse> HandleAsync(
        OrderRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Validating order");

        if (request.Items == null || !request.Items.Any())
        {
            return OrderResponse.Failure("Order must contain at least one item");
        }

        if (request.TotalAmount <= 0)
        {
            return OrderResponse.Failure("Total amount must be greater than zero");
        }

        _logger.LogInformation("Validation passed, forwarding to next handler");
        return await base.HandleAsync(request, cancellationToken);
    }
}

public class InventoryHandler : RequestHandlerBase<OrderRequest, OrderResponse>
{
    private readonly IInventoryService _inventoryService;
    private readonly ILogger<InventoryHandler> _logger;

    public InventoryHandler(
        IInventoryService inventoryService,
        ILogger<InventoryHandler> logger)
    {
        _inventoryService = inventoryService;
        _logger = logger;
    }

    public override async Task<OrderResponse> HandleAsync(
        OrderRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Checking inventory");

        var available = await _inventoryService.CheckAvailabilityAsync(
            request.Items,
            cancellationToken);

        if (!available)
        {
            return OrderResponse.Failure("Items not available in inventory");
        }

        _logger.LogInformation("Inventory check passed, forwarding to next handler");
        return await base.HandleAsync(request, cancellationToken);
    }
}

public class PaymentHandler : RequestHandlerBase<OrderRequest, OrderResponse>
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentHandler> _logger;

    public PaymentHandler(
        IPaymentService paymentService,
        ILogger<PaymentHandler> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    public override async Task<OrderResponse> HandleAsync(
        OrderRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing payment");

        var paymentResult = await _paymentService.ProcessAsync(
            request.PaymentInfo,
            request.TotalAmount,
            cancellationToken);

        if (!paymentResult.Success)
        {
            return OrderResponse.Failure($"Payment failed: {paymentResult.Error}");
        }

        _logger.LogInformation("Payment processed successfully, forwarding to next handler");
        return await base.HandleAsync(request, cancellationToken);
    }
}

public class FulfillmentHandler : RequestHandlerBase<OrderRequest, OrderResponse>
{
    private readonly IFulfillmentService _fulfillmentService;
    private readonly ILogger<FulfillmentHandler> _logger;

    public FulfillmentHandler(
        IFulfillmentService fulfillmentService,
        ILogger<FulfillmentHandler> logger)
    {
        _fulfillmentService = fulfillmentService;
        _logger = logger;
    }

    public override async Task<OrderResponse> HandleAsync(
        OrderRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating fulfillment order");

        var orderId = await _fulfillmentService.CreateOrderAsync(
            request,
            cancellationToken);

        _logger.LogInformation("Order created successfully: {OrderId}", orderId);
        return OrderResponse.Success(orderId);
    }
}

// Chain builder service
public interface IHandlerChainBuilder<TRequest, TResponse>
{
    IRequestHandler<TRequest, TResponse> Build();
}

public class OrderHandlerChainBuilder : IHandlerChainBuilder<OrderRequest, OrderResponse>
{
    private readonly IServiceProvider _serviceProvider;

    public OrderHandlerChainBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IRequestHandler<OrderRequest, OrderResponse> Build()
    {
        // Resolve handlers from DI container
        var validation = _serviceProvider.GetRequiredService<ValidationHandler>();
        var inventory = _serviceProvider.GetRequiredService<InventoryHandler>();
        var payment = _serviceProvider.GetRequiredService<PaymentHandler>();
        var fulfillment = _serviceProvider.GetRequiredService<FulfillmentHandler>();

        // Build chain
        validation.Next = inventory;
        inventory.Next = payment;
        payment.Next = fulfillment;

        return validation;
    }
}

// Register in Program.cs
var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IFulfillmentService, FulfillmentService>();

// Register handlers
builder.Services.AddScoped<ValidationHandler>();
builder.Services.AddScoped<InventoryHandler>();
builder.Services.AddScoped<PaymentHandler>();
builder.Services.AddScoped<FulfillmentHandler>();

// Register chain builder
builder.Services.AddScoped<IHandlerChainBuilder<OrderRequest, OrderResponse>, OrderHandlerChainBuilder>();

var app = builder.Build();

// Controller using chain
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IHandlerChainBuilder<OrderRequest, OrderResponse> _chainBuilder;

    public OrdersController(IHandlerChainBuilder<OrderRequest, OrderResponse> chainBuilder)
    {
        _chainBuilder = chainBuilder;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
    {
        var chain = _chainBuilder.Build();
        var response = await chain.HandleAsync(request);

        return response.Success ? Ok(response) : BadRequest(response.Error);
    }
}
```

**Benefits of DI Integration:**
- **Testability**: Easy to mock dependencies in handlers
- **Flexibility**: Can reconfigure chain at runtime
- **Lifetime Management**: Container manages handler lifecycles
- **Logging**: Built-in logging infrastructure
- **Configuration**: Chain structure can be configured via settings

## 6. Expert Advice: When to Use vs When Not to Use

### ✅ When to Use Chain of Responsibility:

- **Multiple Handlers for Request**: More than one object can handle a request, but you don't know which one in advance
  ```csharp
  // Good: Approval workflow with multiple approval levels
  var chain = manager.SetNext(director).SetNext(vp).SetNext(ceo);
  var result = chain.ProcessRequest(expenseRequest);
  ```

- **Dynamic Handler Selection**: The handler isn't known until runtime
  ```csharp
  // Good: Request routing based on runtime conditions
  var handler = BuildHandlerChain(requestType, userRole, configuration);
  handler.Handle(request);
  ```

- **Sequential Processing**: Request needs to pass through multiple processing stages
  ```csharp
  // Good: Validation pipeline
  var chain = requiredFields.SetNext(format).SetNext(businessRules).SetNext(database);
  var result = chain.Validate(input);
  ```

- **Avoiding Tight Coupling**: Want to decouple sender from receiver
  ```csharp
  // Good: Middleware pipeline
  app.UseAuthentication().UseAuthorization().UseEndpoints(...);
  ```

- **Configurable Processing**: Need to add/remove/reorder handlers at runtime
  ```csharp
  // Good: Plugin architecture with configurable filters
  foreach (var plugin in enabledPlugins)
  {
      chain.AddHandler(plugin.CreateHandler());
  }
  ```

### ❌ When NOT to Use Chain of Responsibility:

- **Single Handler**: Only one object can handle the request
  ```csharp
  // Bad: Unnecessary chain for single handler
  public class SingleHandlerChain : IHandler
  {
      public void Handle(Request request)
      {
          // Just one handler
      }
  }

  // Better: Call the handler directly
  handler.Handle(request);
  ```

- **Known Handler at Compile Time**: Always know which handler to use
  ```csharp
  // Bad: Chain when handler is predetermined
  if (request.Type == RequestType.TypeA)
      handlerA.Handle(request);
  else
      handlerB.Handle(request);

  // Better: Direct call based on type
  var handler = _handlers[request.Type];
  handler.Handle(request);
  ```

- **Performance Critical**: Every millisecond matters and chain overhead is significant
  ```csharp
  // Bad: Chain in tight loop
  for (int i = 0; i < 1000000; i++)
  {
      chain.Handle(request); // Traverses entire chain each time
  }

  // Better: Direct handler invocation
  handler.Handle(request);
  ```

- **Simple If-Else Sufficient**: Logic is simple and won't change
  ```csharp
  // Bad: Overengineering simple logic
  var chain = new HandlerChain();
  // Just for: if (x > 10) return "big"; else return "small";

  // Better: Simple conditional
  return x > 10 ? "big" : "small";
  ```

### 🎯 Expert Recommendations:

**1. Use ASP.NET Core Middleware Pattern**
```csharp
// Good: Leverage built-in middleware pipeline
app.UseMiddleware<CustomLoggingMiddleware>();
app.UseMiddleware<CustomAuthenticationMiddleware>();
app.UseMiddleware<CustomCachingMiddleware>();

// Each middleware is a handler in the chain
```

**2. Implement Fluent Interface for Chain Building**
```csharp
// Good: Fluent API for readable chain construction
var chain = new HandlerChainBuilder()
    .Add<ValidationHandler>()
    .Add<AuthenticationHandler>()
    .Add<LoggingHandler>()
    .Add<ProcessingHandler>()
    .Build();
```

**3. Provide Default/Catch-All Handler**
```csharp
// Good: Final handler that handles all remaining requests
public class DefaultHandler : HandlerBase
{
    public override Response Handle(Request request)
    {
        // Handle all requests that weren't handled by previous handlers
        return Response.Default();
    }
}

// Add at end of chain
chain.SetNext(validationHandler).SetNext(processingHandler).SetNext(defaultHandler);
```

**4. Handle Null Results Gracefully**
```csharp
// Good: Check if request was handled
var result = chain.Handle(request);
if (result == null)
{
    _logger.LogWarning("Request was not handled by any handler");
    return Response.Unhandled();
}
```

**5. Avoid Infinite Loops**
```csharp
// Good: Track visited handlers
public abstract class SafeHandler : HandlerBase
{
    private readonly HashSet<IHandler> _visited = new();

    public override Response Handle(Request request)
    {
        if (_visited.Contains(this))
        {
            throw new InvalidOperationException("Circular chain detected");
        }

        _visited.Add(this);
        return ProcessRequest(request);
    }
}
```

**6. Use Async for I/O Operations**
```csharp
// Good: Async handlers for scalability
public interface IAsyncHandler<TRequest, TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken ct = default);
}

// Handlers can perform async operations
public override async Task<Response> HandleAsync(Request request, CancellationToken ct)
{
    var data = await _database.QueryAsync(request.Id, ct);
    // Process...
}
```

**7. Document Chain Order**
```csharp
/// <summary>
/// Order processing chain:
/// 1. ValidationHandler - Validates request fields
/// 2. InventoryHandler - Checks item availability
/// 3. PaymentHandler - Processes payment
/// 4. FulfillmentHandler - Creates order
/// </summary>
public class OrderHandlerChain
{
    // Clear documentation of chain order
}
```

**Real-World Decision Flow:**
1. Need to process request through multiple stages? → Use Chain of Responsibility
2. Multiple objects might handle request? → Use Chain of Responsibility
3. Building middleware/filter pipeline? → Use Chain of Responsibility
4. Handler known at compile time? → Don't use Chain of Responsibility
5. ASP.NET Core application? → Use built-in middleware pipeline
6. Need approval workflow? → Use Chain of Responsibility
7. Simple conditional logic? → Don't use Chain of Responsibility

The Chain of Responsibility pattern excels at creating flexible, configurable processing pipelines where handlers are independent and the handling sequence can change. Use it when you need to decouple request senders from handlers and allow multiple objects to handle requests dynamically. For ASP.NET Core, leverage the built-in middleware pipeline which is a production-ready implementation of this pattern.
