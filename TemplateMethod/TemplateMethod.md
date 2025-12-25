# Template Method Pattern

## 1. Definition

The Template Method pattern defines the skeleton of an algorithm in a base class, allowing subclasses to override specific steps of the algorithm without changing its overall structure. This pattern promotes code reuse by letting you implement the invariant parts of an algorithm once in the base class while letting subclasses implement the variant behavior.

**How it works:**
- **Create an abstract class** with a template method that defines the algorithm's structure
- **Implement common operations** in the abstract class (invariant parts)
- **Declare abstract methods** for operations that vary (variant parts)
- **Provide hook methods** (virtual with default implementation) for optional customization
- **Subclasses implement** the abstract methods and optionally override hooks

The key principle is **algorithm structure preservation**: the template method defines the sequence of steps and cannot be overridden, while subclasses provide specific implementations for certain steps without changing the overall flow.

## 2. Pros

- **Code Reuse**: Common algorithm steps are implemented once in the base class
- **Consistency**: All variations follow the same algorithm structure
- **Control Over Structure**: Base class controls the algorithm flow
- **Inversion of Control**: "Hollywood Principle" - "Don't call us, we'll call you"
- **Extensibility**: Easy to add new variations by creating new subclasses
- **Open/Closed Principle**: Open for extension (new subclasses), closed for modification
- **Single Responsibility**: Each class focuses on its specific variation
- **Eliminates Duplication**: Common code is centralized
- **Enforces Standards**: Template method ensures consistent algorithm execution

## 3. Cons

- **Rigidity**: Template method structure is fixed and cannot be easily changed
- **Inheritance Required**: Must use inheritance (cannot use composition)
- **Limited Flexibility**: Hard to change algorithm structure without modifying base class
- **Fragile Base Class Problem**: Changes to template method affect all subclasses
- **Tight Coupling**: Subclasses are tightly coupled to the abstract class
- **Increased Number of Classes**: Each variation requires a new subclass
- **Liskov Substitution**: Subclasses must strictly follow template method expectations
- **Complexity for Simple Cases**: Overkill if algorithm variations are minimal

## 4. Real-world Use Cases in C# & .NET

### Data Processing Pipeline

```csharp
// Without Template Method - Code duplication
public class CsvProcessor
{
    public void Process(string filePath)
    {
        var data = File.ReadAllText(filePath);
        var lines = data.Split('\n');
        var parsed = ParseCsv(lines);
        Validate(parsed);
        Save(parsed);
    }

    private List<Record> ParseCsv(string[] lines) { /* CSV parsing */ }
    private void Validate(List<Record> records) { /* Validation */ }
    private void Save(List<Record> records) { /* Save */ }
}

public class JsonProcessor
{
    public void Process(string filePath)
    {
        var data = File.ReadAllText(filePath);
        var parsed = ParseJson(data);
        Validate(parsed); // Duplicated
        Save(parsed);     // Duplicated
    }

    private List<Record> ParseJson(string data) { /* JSON parsing */ }
    private void Validate(List<Record> records) { /* Duplicated! */ }
    private void Save(List<Record> records) { /* Duplicated! */ }
}

// With Template Method - Code reuse
public abstract class DataProcessor
{
    // Template method defines the algorithm structure
    public void Process(string filePath)
    {
        var rawData = LoadData(filePath);
        var parsedData = ParseData(rawData);
        ValidateData(parsedData);
        TransformData(parsedData);
        SaveData(parsedData);
        LogCompletion();
    }

    // Common operations (shared by all processors)
    protected string LoadData(string filePath)
    {
        return File.ReadAllText(filePath);
    }

    protected void ValidateData(List<Record> records)
    {
        if (records == null || records.Count == 0)
            throw new InvalidDataException("No records found");

        foreach (var record in records)
        {
            if (string.IsNullOrEmpty(record.Id))
                throw new InvalidDataException("Record missing ID");
        }
    }

    protected void SaveData(List<Record> records)
    {
        using var db = new DbContext();
        db.Records.AddRange(records);
        db.SaveChanges();
    }

    protected void LogCompletion()
    {
        Console.WriteLine($"Processed {GetType().Name}");
    }

    // Abstract operations (must be implemented by subclasses)
    protected abstract List<Record> ParseData(string rawData);

    // Hook method (optional customization point)
    protected virtual void TransformData(List<Record> records)
    {
        // Default: no transformation
        // Subclasses can override to add transformations
    }
}

public class CsvDataProcessor : DataProcessor
{
    protected override List<Record> ParseData(string rawData)
    {
        var lines = rawData.Split('\n');
        return lines.Select(line =>
        {
            var fields = line.Split(',');
            return new Record
            {
                Id = fields[0],
                Name = fields[1],
                Value = decimal.Parse(fields[2])
            };
        }).ToList();
    }
}

public class JsonDataProcessor : DataProcessor
{
    protected override List<Record> ParseData(string rawData)
    {
        return JsonSerializer.Deserialize<List<Record>>(rawData);
    }

    protected override void TransformData(List<Record> records)
    {
        // JSON processor adds custom transformation
        foreach (var record in records)
        {
            record.Name = record.Name.ToUpper();
        }
    }
}

public class XmlDataProcessor : DataProcessor
{
    protected override List<Record> ParseData(string rawData)
    {
        var doc = XDocument.Parse(rawData);
        return doc.Descendants("record").Select(x => new Record
        {
            Id = x.Element("id")?.Value,
            Name = x.Element("name")?.Value,
            Value = decimal.Parse(x.Element("value")?.Value ?? "0")
        }).ToList();
    }
}

// Usage
var csvProcessor = new CsvDataProcessor();
csvProcessor.Process("data.csv");

var jsonProcessor = new JsonDataProcessor();
jsonProcessor.Process("data.json");

var xmlProcessor = new XmlDataProcessor();
xmlProcessor.Process("data.xml");

// All three follow the same processing structure:
// Load -> Parse -> Validate -> Transform -> Save -> Log
// Only the parsing (and optionally transformation) varies
```

### Unit Test Framework

```csharp
// Test framework using template method pattern
public abstract class TestCase
{
    // Template method defines test execution flow
    public TestResult Run()
    {
        var result = new TestResult();

        try
        {
            SetUp();           // Hook: optional setup
            RunTest();         // Abstract: test implementation
            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Error = ex.Message;
        }
        finally
        {
            TearDown();        // Hook: optional cleanup
        }

        return result;
    }

    // Hook methods (optional override)
    protected virtual void SetUp()
    {
        // Default: no setup
    }

    protected virtual void TearDown()
    {
        // Default: no cleanup
    }

    // Abstract method (must implement)
    protected abstract void RunTest();
}

public class DatabaseTest : TestCase
{
    private DbConnection _connection;

    protected override void SetUp()
    {
        _connection = new SqlConnection(connectionString);
        _connection.Open();
        CleanDatabase();
    }

    protected override void RunTest()
    {
        // Actual test
        var repo = new UserRepository(_connection);
        repo.Add(new User { Name = "Alice" });

        var users = repo.GetAll();
        Assert.AreEqual(1, users.Count);
        Assert.AreEqual("Alice", users[0].Name);
    }

    protected override void TearDown()
    {
        _connection?.Close();
        _connection?.Dispose();
    }

    private void CleanDatabase()
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "DELETE FROM Users";
        cmd.ExecuteNonQuery();
    }
}

public class ApiTest : TestCase
{
    private HttpClient _client;

    protected override void SetUp()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("https://api.example.com")
        };
    }

    protected override void RunTest()
    {
        var response = await _client.GetAsync("/users/1");
        Assert.IsTrue(response.IsSuccessStatusCode);

        var user = await response.Content.ReadAsAsync<User>();
        Assert.IsNotNull(user);
        Assert.AreEqual("Alice", user.Name);
    }

    protected override void TearDown()
    {
        _client?.Dispose();
    }
}

// Usage
var dbTest = new DatabaseTest();
var result1 = dbTest.Run();
Console.WriteLine($"Database test: {(result1.Success ? "PASS" : "FAIL")}");

var apiTest = new ApiTest();
var result2 = apiTest.Run();
Console.WriteLine($"API test: {(result2.Success ? "PASS" : "FAIL")}");
```

### Web Request Handler

```csharp
public abstract class HttpRequestHandler
{
    // Template method defines request handling flow
    public async Task<HttpResponse> HandleRequestAsync(HttpRequest request)
    {
        try
        {
            // Step 1: Log incoming request
            LogRequest(request);

            // Step 2: Authenticate (common)
            if (!await AuthenticateAsync(request))
            {
                return new HttpResponse { StatusCode = 401, Body = "Unauthorized" };
            }

            // Step 3: Authorize (varies by handler)
            if (!await AuthorizeAsync(request))
            {
                return new HttpResponse { StatusCode = 403, Body = "Forbidden" };
            }

            // Step 4: Validate request (varies by handler)
            var validationResult = await ValidateRequestAsync(request);
            if (!validationResult.IsValid)
            {
                return new HttpResponse { StatusCode = 400, Body = validationResult.Error };
            }

            // Step 5: Process request (varies by handler)
            var response = await ProcessRequestAsync(request);

            // Step 6: Optional post-processing hook
            await OnAfterProcessingAsync(response);

            return response;
        }
        catch (Exception ex)
        {
            return new HttpResponse { StatusCode = 500, Body = ex.Message };
        }
        finally
        {
            // Step 7: Always log response
            LogCompletion(request);
        }
    }

    // Common operation
    protected async Task<bool> AuthenticateAsync(HttpRequest request)
    {
        var token = request.Headers["Authorization"];
        if (string.IsNullOrEmpty(token))
            return false;

        return await _authService.ValidateTokenAsync(token);
    }

    protected void LogRequest(HttpRequest request)
    {
        _logger.LogInformation("Request: {Method} {Path}", request.Method, request.Path);
    }

    protected void LogCompletion(HttpRequest request)
    {
        _logger.LogInformation("Completed: {Path}", request.Path);
    }

    // Abstract operations (must implement)
    protected abstract Task<bool> AuthorizeAsync(HttpRequest request);
    protected abstract Task<ValidationResult> ValidateRequestAsync(HttpRequest request);
    protected abstract Task<HttpResponse> ProcessRequestAsync(HttpRequest request);

    // Hook method (optional override)
    protected virtual Task OnAfterProcessingAsync(HttpResponse response)
    {
        return Task.CompletedTask;
    }
}

public class CreateUserHandler : HttpRequestHandler
{
    protected override async Task<bool> AuthorizeAsync(HttpRequest request)
    {
        // Only admins can create users
        var user = await _authService.GetCurrentUserAsync();
        return user.Role == "Admin";
    }

    protected override async Task<ValidationResult> ValidateRequestAsync(HttpRequest request)
    {
        var userData = await request.ReadAsAsync<CreateUserDto>();

        if (string.IsNullOrEmpty(userData.Email))
            return ValidationResult.Failure("Email is required");

        if (!IsValidEmail(userData.Email))
            return ValidationResult.Failure("Invalid email format");

        return ValidationResult.Success();
    }

    protected override async Task<HttpResponse> ProcessRequestAsync(HttpRequest request)
    {
        var userData = await request.ReadAsAsync<CreateUserDto>();

        var user = new User
        {
            Email = userData.Email,
            Name = userData.Name
        };

        await _userRepository.AddAsync(user);

        return new HttpResponse
        {
            StatusCode = 201,
            Body = JsonSerializer.Serialize(user)
        };
    }

    protected override async Task OnAfterProcessingAsync(HttpResponse response)
    {
        // Send welcome email after user creation
        await _emailService.SendWelcomeEmailAsync(user.Email);
    }
}

public class GetUserHandler : HttpRequestHandler
{
    protected override async Task<bool> AuthorizeAsync(HttpRequest request)
    {
        // Anyone authenticated can get users
        return true;
    }

    protected override Task<ValidationResult> ValidateRequestAsync(HttpRequest request)
    {
        var userId = request.RouteValues["id"];

        if (string.IsNullOrEmpty(userId))
            return Task.FromResult(ValidationResult.Failure("User ID is required"));

        return Task.FromResult(ValidationResult.Success());
    }

    protected override async Task<HttpResponse> ProcessRequestAsync(HttpRequest request)
    {
        var userId = request.RouteValues["id"];
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            return new HttpResponse
            {
                StatusCode = 404,
                Body = "User not found"
            };
        }

        return new HttpResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(user)
        };
    }
}

// Usage
var createHandler = new CreateUserHandler();
var response = await createHandler.HandleRequestAsync(request);

var getHandler = new GetUserHandler();
var response2 = await getHandler.HandleRequestAsync(request2);
```

### .NET Framework Examples

**ASP.NET Core ControllerBase**
```csharp
// ASP.NET Core controllers use template method pattern
public abstract class ControllerBase
{
    // Template method
    public virtual Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        OnActionExecuting(context);  // Hook
        if (!context.Cancel)
        {
            var result = next();      // Execute action
            OnActionExecuted(context); // Hook
        }
        return Task.CompletedTask;
    }

    // Hook methods
    protected virtual void OnActionExecuting(ActionExecutingContext context) { }
    protected virtual void OnActionExecuted(ActionExecutedContext context) { }
}

// Custom controller with hooks
public class MyController : ControllerBase
{
    protected override void OnActionExecuting(ActionExecutingContext context)
    {
        // Add custom logic before action
        _logger.LogInformation("Action executing: {ActionName}", context.ActionDescriptor.DisplayName);
    }

    protected override void OnActionExecuted(ActionExecutedContext context)
    {
        // Add custom logic after action
        _logger.LogInformation("Action executed");
    }
}
```

**Entity Framework DbContext**
```csharp
// DbContext uses template method for SaveChanges
public class MyDbContext : DbContext
{
    public override int SaveChanges()
    {
        OnBeforeSaving();  // Hook

        var result = base.SaveChanges(); // Template method

        OnAfterSaving();   // Hook

        return result;
    }

    protected virtual void OnBeforeSaving()
    {
        // Add timestamps
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is IAuditable auditable)
            {
                if (entry.State == EntityState.Added)
                    auditable.CreatedAt = DateTime.UtcNow;

                auditable.ModifiedAt = DateTime.UtcNow;
            }
        }
    }

    protected virtual void OnAfterSaving()
    {
        // Clear cache, publish events, etc.
        _cacheService.InvalidateCache();
    }
}
```

**ASP.NET Core Middleware Pipeline**
```csharp
// Middleware uses template method pattern
public abstract class MiddlewareBase
{
    protected readonly RequestDelegate _next;

    protected MiddlewareBase(RequestDelegate next)
    {
        _next = next;
    }

    // Template method
    public async Task InvokeAsync(HttpContext context)
    {
        await OnBeforeAsync(context);  // Hook

        await _next(context);           // Process

        await OnAfterAsync(context);    // Hook
    }

    // Hook methods
    protected virtual Task OnBeforeAsync(HttpContext context) => Task.CompletedTask;
    protected virtual Task OnAfterAsync(HttpContext context) => Task.CompletedTask;
}

public class LoggingMiddleware : MiddlewareBase
{
    private readonly ILogger _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger logger) : base(next)
    {
        _logger = logger;
    }

    protected override Task OnBeforeAsync(HttpContext context)
    {
        _logger.LogInformation("Request: {Method} {Path}", context.Request.Method, context.Request.Path);
        return Task.CompletedTask;
    }

    protected override Task OnAfterAsync(HttpContext context)
    {
        _logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
        return Task.CompletedTask;
    }
}
```

### Report Generation

```csharp
public abstract class ReportGenerator
{
    // Template method
    public void GenerateReport(ReportRequest request)
    {
        var data = CollectData(request);
        var processedData = ProcessData(data);
        var formattedContent = FormatContent(processedData);
        ApplyStyles(formattedContent);
        var output = ExportReport(formattedContent);
        SaveReport(output, request.OutputPath);
        NotifyCompletion(request);
    }

    // Abstract operations (vary by report type)
    protected abstract object CollectData(ReportRequest request);
    protected abstract object ProcessData(object rawData);
    protected abstract string FormatContent(object processedData);
    protected abstract byte[] ExportReport(string content);

    // Common operations (shared by all reports)
    protected void ApplyStyles(string content)
    {
        // Apply common styling
        Console.WriteLine("Applying standard styles");
    }

    protected void SaveReport(byte[] data, string path)
    {
        File.WriteAllBytes(path, data);
        Console.WriteLine($"Report saved to {path}");
    }

    protected void NotifyCompletion(ReportRequest request)
    {
        if (!string.IsNullOrEmpty(request.NotificationEmail))
        {
            _emailService.Send(request.NotificationEmail, "Report ready", "Your report is ready");
        }
    }
}

public class PdfReportGenerator : ReportGenerator
{
    protected override object CollectData(ReportRequest request)
    {
        return _database.Query(request.Query);
    }

    protected override object ProcessData(object rawData)
    {
        var data = (IEnumerable<Record>)rawData;
        return data.GroupBy(r => r.Category)
                   .Select(g => new { Category = g.Key, Total = g.Sum(r => r.Amount) });
    }

    protected override string FormatContent(object processedData)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<html><body>");
        foreach (var item in (IEnumerable<dynamic>)processedData)
        {
            sb.AppendLine($"<p>{item.Category}: ${item.Total}</p>");
        }
        sb.AppendLine("</body></html>");
        return sb.ToString();
    }

    protected override byte[] ExportReport(string content)
    {
        return _pdfConverter.ConvertHtmlToPdf(content);
    }
}

public class ExcelReportGenerator : ReportGenerator
{
    protected override object CollectData(ReportRequest request)
    {
        return _database.Query(request.Query);
    }

    protected override object ProcessData(object rawData)
    {
        return rawData; // No processing needed for Excel
    }

    protected override string FormatContent(object processedData)
    {
        // Format as CSV
        var sb = new StringBuilder();
        sb.AppendLine("Category,Amount");
        foreach (var record in (IEnumerable<Record>)processedData)
        {
            sb.AppendLine($"{record.Category},{record.Amount}");
        }
        return sb.ToString();
    }

    protected override byte[] ExportReport(string content)
    {
        return Encoding.UTF8.GetBytes(content);
    }
}

// Usage
var pdfReport = new PdfReportGenerator();
pdfReport.GenerateReport(new ReportRequest
{
    Query = "SELECT * FROM Sales",
    OutputPath = "report.pdf",
    NotificationEmail = "user@example.com"
});

var excelReport = new ExcelReportGenerator();
excelReport.GenerateReport(new ReportRequest
{
    Query = "SELECT * FROM Sales",
    OutputPath = "report.csv"
});
```

## 5. Modern Approach: Dependency Injection

In modern C# and ASP.NET Core applications, the Template Method pattern can be integrated with dependency injection for better testability and flexibility.

```csharp
// Base class with DI support
public abstract class OrderProcessor
{
    protected readonly ILogger _logger;
    protected readonly IInventoryService _inventory;
    protected readonly IPaymentService _payment;
    protected readonly IShippingService _shipping;

    protected OrderProcessor(
        ILogger logger,
        IInventoryService inventory,
        IPaymentService payment,
        IShippingService shipping)
    {
        _logger = logger;
        _inventory = inventory;
        _payment = payment;
        _shipping = shipping;
    }

    // Template method
    public async Task<OrderResult> ProcessOrderAsync(Order order)
    {
        _logger.LogInformation("Processing order {OrderId}", order.Id);

        try
        {
            // Step 1: Validate
            await ValidateOrderAsync(order);

            // Step 2: Check inventory (common)
            await CheckInventoryAsync(order);

            // Step 3: Process payment (varies)
            await ProcessPaymentAsync(order);

            // Step 4: Reserve items (common)
            await ReserveItemsAsync(order);

            // Step 5: Create shipment (varies)
            await CreateShipmentAsync(order);

            // Step 6: Optional hook
            await OnOrderCompletedAsync(order);

            return OrderResult.Success(order.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order {OrderId}", order.Id);
            await OnOrderFailedAsync(order, ex);
            return OrderResult.Failure(ex.Message);
        }
    }

    // Common operation
    protected async Task CheckInventoryAsync(Order order)
    {
        foreach (var item in order.Items)
        {
            var available = await _inventory.CheckAvailabilityAsync(item.ProductId, item.Quantity);
            if (!available)
            {
                throw new InvalidOperationException($"Product {item.ProductId} not available");
            }
        }
    }

    protected async Task ReserveItemsAsync(Order order)
    {
        foreach (var item in order.Items)
        {
            await _inventory.ReserveAsync(item.ProductId, item.Quantity);
        }
    }

    // Abstract operations (must implement)
    protected abstract Task ValidateOrderAsync(Order order);
    protected abstract Task ProcessPaymentAsync(Order order);
    protected abstract Task CreateShipmentAsync(Order order);

    // Hook methods (optional override)
    protected virtual Task OnOrderCompletedAsync(Order order)
    {
        _logger.LogInformation("Order {OrderId} completed", order.Id);
        return Task.CompletedTask;
    }

    protected virtual Task OnOrderFailedAsync(Order order, Exception ex)
    {
        _logger.LogWarning("Order {OrderId} failed", order.Id);
        return Task.CompletedTask;
    }
}

// Standard order processing
public class StandardOrderProcessor : OrderProcessor
{
    public StandardOrderProcessor(
        ILogger<StandardOrderProcessor> logger,
        IInventoryService inventory,
        IPaymentService payment,
        IShippingService shipping)
        : base(logger, inventory, payment, shipping)
    {
    }

    protected override async Task ValidateOrderAsync(Order order)
    {
        if (order.Items.Count == 0)
            throw new InvalidOperationException("Order has no items");

        if (order.TotalAmount <= 0)
            throw new InvalidOperationException("Invalid order amount");
    }

    protected override async Task ProcessPaymentAsync(Order order)
    {
        var result = await _payment.ProcessCreditCardAsync(order.TotalAmount, order.PaymentDetails);

        if (!result.Success)
            throw new InvalidOperationException($"Payment failed: {result.Error}");

        order.PaymentId = result.TransactionId;
    }

    protected override async Task CreateShipmentAsync(Order order)
    {
        var shipment = await _shipping.CreateStandardShipmentAsync(
            order.ShippingAddress,
            order.Items.Select(i => new ShipmentItem(i.ProductId, i.Quantity)).ToList()
        );

        order.TrackingNumber = shipment.TrackingNumber;
    }
}

// Premium order processing with expedited shipping
public class PremiumOrderProcessor : OrderProcessor
{
    private readonly INotificationService _notification;

    public PremiumOrderProcessor(
        ILogger<PremiumOrderProcessor> logger,
        IInventoryService inventory,
        IPaymentService payment,
        IShippingService shipping,
        INotificationService notification)
        : base(logger, inventory, payment, shipping)
    {
        _notification = notification;
    }

    protected override async Task ValidateOrderAsync(Order order)
    {
        // More lenient validation for premium customers
        if (order.Items.Count == 0)
            throw new InvalidOperationException("Order has no items");
    }

    protected override async Task ProcessPaymentAsync(Order order)
    {
        // Premium customers can use stored payment method
        var result = await _payment.ProcessStoredPaymentMethodAsync(
            order.CustomerId,
            order.TotalAmount
        );

        if (!result.Success)
            throw new InvalidOperationException($"Payment failed: {result.Error}");

        order.PaymentId = result.TransactionId;
    }

    protected override async Task CreateShipmentAsync(Order order)
    {
        // Premium customers get expedited shipping
        var shipment = await _shipping.CreateExpeditedShipmentAsync(
            order.ShippingAddress,
            order.Items.Select(i => new ShipmentItem(i.ProductId, i.Quantity)).ToList()
        );

        order.TrackingNumber = shipment.TrackingNumber;
    }

    protected override async Task OnOrderCompletedAsync(Order order)
    {
        await base.OnOrderCompletedAsync(order);

        // Send premium customer notification
        await _notification.SendPremiumOrderConfirmationAsync(
            order.CustomerId,
            order.Id,
            order.TrackingNumber
        );
    }
}

// Register in Program.cs
var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IShippingService, ShippingService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Register order processors
builder.Services.AddScoped<StandardOrderProcessor>();
builder.Services.AddScoped<PremiumOrderProcessor>();

// Factory to select processor
builder.Services.AddScoped<IOrderProcessorFactory, OrderProcessorFactory>();

var app = builder.Build();

// Factory for selecting appropriate processor
public interface IOrderProcessorFactory
{
    OrderProcessor GetProcessor(CustomerType customerType);
}

public class OrderProcessorFactory : IOrderProcessorFactory
{
    private readonly IServiceProvider _serviceProvider;

    public OrderProcessorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public OrderProcessor GetProcessor(CustomerType customerType)
    {
        return customerType switch
        {
            CustomerType.Premium => _serviceProvider.GetRequiredService<PremiumOrderProcessor>(),
            _ => _serviceProvider.GetRequiredService<StandardOrderProcessor>()
        };
    }
}

// Controller using template method processors
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderProcessorFactory _processorFactory;

    public OrdersController(IOrderProcessorFactory processorFactory)
    {
        _processorFactory = processorFactory;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var order = MapToOrder(request);
        var processor = _processorFactory.GetProcessor(request.CustomerType);

        var result = await processor.ProcessOrderAsync(order);

        return result.Success
            ? Ok(new { OrderId = result.OrderId, TrackingNumber = order.TrackingNumber })
            : BadRequest(new { Error = result.Error });
    }
}
```

**Benefits of DI Integration:**
- **Testability**: Easy to mock dependencies in tests
- **Flexibility**: Can swap service implementations
- **Logging**: Built-in logging support
- **Async Support**: Async operations throughout
- **Service Access**: Template method can use injected services

## 6. Expert Advice: When to Use vs When Not to Use

### ✅ When to Use Template Method:

- **Similar Algorithms with Variations**: Multiple classes implement similar algorithms with slight differences
  ```csharp
  // Good: Data processors with same flow, different parsing
  public abstract class DataProcessor
  {
      public void Process() // Same structure
      {
          Load(); Parse(); Validate(); Save();
      }
      protected abstract void Parse(); // Varies
  }
  ```

- **Code Duplication**: Same algorithm steps repeated across classes
  ```csharp
  // Good: Avoid duplicating Load, Validate, Save
  // Only Parse varies between CSV, JSON, XML processors
  ```

- **Enforcing Algorithm Structure**: Need to ensure all variations follow same sequence
  ```csharp
  // Good: All tests must run: SetUp -> Test -> TearDown
  public abstract class TestCase
  {
      public void Run() { SetUp(); RunTest(); TearDown(); }
  }
  ```

- **Hollywood Principle**: "Don't call us, we'll call you"
  ```csharp
  // Good: Base class controls flow, calls subclass methods
  // Inversion of control compared to typical inheritance
  ```

### ❌ When NOT to Use Template Method:

- **Algorithm Structure Varies**: Each variation has different structure
  ```csharp
  // Bad: Forced to use template when algorithms differ
  public abstract class Processor
  {
      public void Process()
      {
          Step1(); Step2(); Step3(); // But some don't need Step2!
      }
  }

  // Better: Use Strategy pattern for varying algorithms
  ```

- **Simple Variations**: Only one or two steps differ
  ```csharp
  // Bad: Overkill for simple cases
  public abstract class Formatter
  {
      public string Format(string s) { return DoFormat(s); }
      protected abstract string DoFormat(string s);
  }

  // Better: Use Strategy or just a delegate/func
  Func<string, string> formatter = s => s.ToUpper();
  ```

- **Need Composition Over Inheritance**: Prefer flexibility of composition
  ```csharp
  // Bad: Forced inheritance hierarchy
  // Better: Use Strategy pattern with composition
  public class Processor
  {
      private IStrategy _strategy;
      public void Process() => _strategy.Execute();
  }
  ```

- **Frequently Changing Structure**: Algorithm structure changes often
  ```csharp
  // Bad: Every structure change affects all subclasses
  // Better: Use Chain of Responsibility or Pipeline pattern
  ```

### 🎯 Expert Recommendations:

**1. Keep Template Method Simple**
```csharp
// Good: Template just orchestrates
public void Process()
{
    Load();
    Parse();
    Validate();
    Save();
}

// Bad: Logic in template method
public void Process()
{
    var data = File.ReadAllText(path); // Logic here!
    Parse(data);
}
```

**2. Use Descriptive Method Names**
```csharp
// Good: Clear what each step does
protected abstract void ParseCsvData();
protected abstract void ValidateRecords();
protected abstract void SaveToDatabase();

// Bad: Generic names
protected abstract void Process1();
protected abstract void Process2();
```

**3. Consider Sealing Template Method**
```csharp
// Good: Prevent subclasses from changing structure
public sealed override void Process()
{
    Load(); Parse(); Save();
}
```

**4. Minimize Abstract Operations**
```csharp
// Good: Only 1-2 abstract operations
public abstract class Processor
{
    public void Process()
    {
        Load();          // Common
        Transform();     // Abstract - varies
        Save();          // Common
    }
}

// Bad: Too many abstract operations
// Forces every subclass to implement many methods
```

**5. Provide Sensible Hooks**
```csharp
// Good: Optional extension points
protected virtual void OnBeforeProcessing() { }
protected virtual void OnAfterProcessing() { }

// Subclasses can override if needed, but not required
```

**6. Document the Algorithm**
```csharp
/// <summary>
/// Processes data using the following steps:
/// 1. LoadData - Reads from file
/// 2. ParseData - Converts to objects (varies by format)
/// 3. ValidateData - Checks for errors
/// 4. SaveData - Persists to database
/// </summary>
public void Process() { }
```

**7. Consider Strategy Pattern Alternative**
```csharp
// Template Method: Use inheritance
public abstract class Processor
{
    public void Process() { Parse(); }
    protected abstract void Parse();
}

// Strategy: Use composition (often more flexible)
public class Processor
{
    private IParser _parser;
    public void Process() { _parser.Parse(); }
}
```

**8. Use Protected Access Modifiers**
```csharp
// Good: Primitive operations are protected
protected abstract void ParseData();
protected void CommonOperation() { }

// Bad: Public primitive operations
public abstract void ParseData(); // Clients shouldn't call this directly
```

**Real-World Decision Flow:**
1. Multiple classes with similar algorithm structure? → Consider Template Method
2. Want to enforce consistent algorithm sequence? → Use Template Method
3. Significant code duplication in algorithm steps? → Use Template Method
4. Algorithm structure varies between cases? → Don't use Template Method
5. Only 1-2 steps vary and they're simple? → Use Strategy or delegate instead
6. Need to change algorithm structure frequently? → Don't use Template Method
7. Prefer composition over inheritance? → Use Strategy pattern instead
8. Building test framework or pipeline? → Template Method is ideal

The Template Method pattern excels at defining consistent algorithm structures while allowing variations in specific steps. Use it when you have a clear, stable algorithm structure with well-defined variation points. For more flexible algorithm composition, consider the Strategy pattern instead.
