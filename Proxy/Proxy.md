# Proxy Pattern

## 1. Definition

The Proxy pattern provides a surrogate or placeholder object that controls access to another object. A proxy implements the same interface as the real object, allowing it to be substituted transparently, and adds additional behavior (access control, caching, lazy loading, logging, etc.) before or after delegating requests to the real object.

**How it works:**
- **Create an interface** that both the RealSubject and Proxy implement
- **The Proxy holds a reference** to the RealSubject
- **Proxy intercepts requests** and adds behavior before/after delegating to RealSubject
- **Client uses the interface**, unaware whether it's working with a proxy or real object

The key principle is **transparent substitution**: clients work with the Subject interface and don't know (or care) whether they're using the real object or a proxy.

**Common Proxy Types:**
1. **Virtual Proxy**: Delays creation of expensive objects until needed (lazy loading)
2. **Protection Proxy**: Controls access based on permissions or other criteria
3. **Remote Proxy**: Represents objects in different address spaces (local representative of remote object)
4. **Caching Proxy**: Caches results to avoid repeated expensive operations
5. **Logging Proxy**: Logs all operations for auditing or debugging
6. **Smart Reference Proxy**: Adds behavior like reference counting, locking, or copy-on-write

## 2. Pros

- **Lazy Initialization**: Can defer expensive object creation until actually needed (Virtual Proxy)
- **Access Control**: Can protect sensitive objects from unauthorized access (Protection Proxy)
- **Performance Optimization**: Can cache results or batch operations (Caching Proxy)
- **Remote Access**: Can represent remote objects as if they were local (Remote Proxy)
- **Transparent to Client**: Client code doesn't change when proxy is introduced
- **Separation of Concerns**: RealSubject focuses on business logic, proxy handles cross-cutting concerns
- **Open/Closed Principle**: Can add new proxy types without modifying existing code
- **Lifecycle Management**: Proxy can control RealSubject creation, caching, and disposal
- **Additional Behavior**: Can add logging, monitoring, validation without changing RealSubject

## 3. Cons

- **Indirection Overhead**: Adds an extra layer that may slow down operations (usually negligible)
- **Increased Complexity**: More classes and potentially more complex object graphs
- **Debugging Difficulty**: Stack traces are deeper, making debugging harder
- **Interface Coupling**: Proxy and RealSubject must implement the same interface
- **Maintenance**: Changes to RealSubject interface require proxy updates
- **Proxy Creation Cost**: Creating and managing proxies has some overhead
- **Potential for Overuse**: Easy to add too many proxy layers, making system hard to understand
- **Synchronization Issues**: Multiple proxies to the same object can cause state management issues

## 4. Real-world Use Cases in C# & .NET

### Virtual Proxy - Lazy Loading of Expensive Objects

```csharp
// Expensive object that we want to load lazily
public interface IImage
{
    void Display();
    byte[] GetData();
}

public class HighResolutionImage : IImage
{
    private readonly string _filename;
    private byte[] _imageData;

    public HighResolutionImage(string filename)
    {
        _filename = filename;
        LoadImageFromDisk(); // Expensive operation!
    }

    private void LoadImageFromDisk()
    {
        Console.WriteLine($"Loading high-resolution image from {_filename}...");
        Thread.Sleep(2000); // Simulate expensive I/O
        _imageData = File.ReadAllBytes(_filename);
    }

    public void Display()
    {
        Console.WriteLine($"Displaying image: {_filename}");
    }

    public byte[] GetData() => _imageData;
}

// Virtual Proxy - delays loading until needed
public class ImageProxy : IImage
{
    private readonly string _filename;
    private HighResolutionImage? _realImage; // Lazy-loaded

    public ImageProxy(string filename)
    {
        _filename = filename;
        // Real image not loaded yet - just store the filename
    }

    public void Display()
    {
        // Load on first use
        EnsureImageLoaded();
        _realImage!.Display();
    }

    public byte[] GetData()
    {
        EnsureImageLoaded();
        return _realImage!.GetData();
    }

    private void EnsureImageLoaded()
    {
        if (_realImage == null)
        {
            Console.WriteLine("Proxy: Loading image on demand...");
            _realImage = new HighResolutionImage(_filename);
        }
    }
}

// Usage
var images = new List<IImage>
{
    new ImageProxy("photo1.jpg"), // Fast - not loaded yet
    new ImageProxy("photo2.jpg"), // Fast - not loaded yet
    new ImageProxy("photo3.jpg")  // Fast - not loaded yet
};

// Only load and display the one we actually need
images[1].Display(); // Now the expensive loading happens
```

### Protection Proxy - Access Control

```csharp
public interface IDocumentRepository
{
    Document GetDocument(int id);
    void SaveDocument(Document doc);
    void DeleteDocument(int id);
}

public class DocumentRepository : IDocumentRepository
{
    public Document GetDocument(int id)
    {
        // Direct database access
        Console.WriteLine($"Retrieving document {id} from database");
        return new Document { Id = id };
    }

    public void SaveDocument(Document doc)
    {
        Console.WriteLine($"Saving document {doc.Id} to database");
    }

    public void DeleteDocument(int id)
    {
        Console.WriteLine($"Deleting document {id} from database");
    }
}

public class SecureDocumentRepositoryProxy : IDocumentRepository
{
    private readonly DocumentRepository _repository;
    private readonly ICurrentUser _currentUser;

    public SecureDocumentRepositoryProxy(DocumentRepository repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public Document GetDocument(int id)
    {
        // Anyone can read
        return _repository.GetDocument(id);
    }

    public void SaveDocument(Document doc)
    {
        if (!_currentUser.HasPermission("Document.Write"))
        {
            throw new UnauthorizedAccessException("User does not have write permission");
        }

        _repository.SaveDocument(doc);
    }

    public void DeleteDocument(int id)
    {
        if (!_currentUser.HasPermission("Document.Delete"))
        {
            throw new UnauthorizedAccessException("User does not have delete permission");
        }

        _repository.DeleteDocument(id);
    }
}
```

### Caching Proxy - Performance Optimization

```csharp
public interface IProductService
{
    Task<Product> GetProductAsync(int id);
    Task<IEnumerable<Product>> SearchProductsAsync(string query);
}

public class ProductService : IProductService
{
    private readonly DbContext _dbContext;

    public ProductService(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Product> GetProductAsync(int id)
    {
        Console.WriteLine($"Fetching product {id} from database...");
        return await _dbContext.Products.FindAsync(id);
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string query)
    {
        Console.WriteLine($"Searching products for '{query}' in database...");
        return await _dbContext.Products
            .Where(p => p.Name.Contains(query))
            .ToListAsync();
    }
}

public class CachingProductServiceProxy : IProductService
{
    private readonly IProductService _productService;
    private readonly IMemoryCache _cache;

    public CachingProductServiceProxy(IProductService productService, IMemoryCache cache)
    {
        _productService = productService;
        _cache = cache;
    }

    public async Task<Product> GetProductAsync(int id)
    {
        string cacheKey = $"product_{id}";

        if (_cache.TryGetValue(cacheKey, out Product? cachedProduct))
        {
            Console.WriteLine($"Returning product {id} from cache");
            return cachedProduct!;
        }

        Console.WriteLine($"Cache miss for product {id}");
        var product = await _productService.GetProductAsync(id);

        _cache.Set(cacheKey, product, TimeSpan.FromMinutes(10));

        return product;
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string query)
    {
        string cacheKey = $"search_{query}";

        if (_cache.TryGetValue(cacheKey, out IEnumerable<Product>? cachedResults))
        {
            Console.WriteLine($"Returning search results for '{query}' from cache");
            return cachedResults!;
        }

        Console.WriteLine($"Cache miss for search '{query}'");
        var results = await _productService.SearchProductsAsync(query);

        _cache.Set(cacheKey, results, TimeSpan.FromMinutes(5));

        return results;
    }
}
```

### .NET Framework Examples

**.NET's Lazy&lt;T&gt; - Built-in Virtual Proxy**
```csharp
// Lazy<T> is a virtual proxy provided by .NET
public class ExpensiveService
{
    public ExpensiveService()
    {
        Console.WriteLine("Creating expensive service...");
        Thread.Sleep(2000);
    }

    public void DoWork() => Console.WriteLine("Working...");
}

// Using Lazy<T> as a virtual proxy
var lazyService = new Lazy<ExpensiveService>();

Console.WriteLine("Lazy service created (but not initialized yet)");
// No construction cost yet

lazyService.Value.DoWork(); // Now it's created
// Output: Creating expensive service...
//         Working...

lazyService.Value.DoWork(); // Reuses the same instance
// Output: Working...
```

**Entity Framework Dynamic Proxies**
```csharp
public class Blog
{
    public int Id { get; set; }
    public string Name { get; set; }

    // Virtual property enables lazy loading proxy
    public virtual ICollection<Post> Posts { get; set; }
}

using var context = new BloggingContext();
var blog = context.Blogs.First(); // Blog loaded

// EF creates a proxy that lazy-loads Posts when accessed
foreach (var post in blog.Posts) // Posts loaded here on first access
{
    Console.WriteLine(post.Title);
}
```

**WCF Client Proxy - Remote Proxy**
```csharp
// WCF generates a client proxy for remote service
var client = new CalculatorServiceClient();

// Looks like local call, but actually makes network request
var result = client.Add(5, 3); // Remote procedure call

// Proxy handles: serialization, network communication, deserialization
```

## 5. Modern Approach: Dependency Injection and Dynamic Proxies

In modern .NET applications, proxies are commonly used with dependency injection and can be created dynamically at runtime.

### Static Proxy with Dependency Injection

```csharp
// Subject interface
public interface IUserService
{
    Task<User> GetUserAsync(int id);
    Task CreateUserAsync(User user);
}

// Real implementation
public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}

// Logging proxy
public class LoggingUserServiceProxy : IUserService
{
    private readonly IUserService _innerService;
    private readonly ILogger<LoggingUserServiceProxy> _logger;

    public LoggingUserServiceProxy(IUserService innerService, ILogger<LoggingUserServiceProxy> logger)
    {
        _innerService = innerService;
        _logger = logger;
    }

    public async Task<User> GetUserAsync(int id)
    {
        _logger.LogInformation("Getting user {UserId}", id);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var user = await _innerService.GetUserAsync(id);
            _logger.LogInformation("Retrieved user {UserId} in {ElapsedMs}ms", id, stopwatch.ElapsedMilliseconds);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {UserId}", id);
            throw;
        }
    }

    public async Task CreateUserAsync(User user)
    {
        _logger.LogInformation("Creating user {UserEmail}", user.Email);

        try
        {
            await _innerService.CreateUserAsync(user);
            _logger.LogInformation("Created user {UserEmail}", user.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user {UserEmail}", user.Email);
            throw;
        }
    }
}

// Registration in Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>();

// Register the real service and wrap it with the proxy
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IUserService>(provider =>
{
    var realService = provider.GetRequiredService<UserService>();
    var logger = provider.GetRequiredService<ILogger<LoggingUserServiceProxy>>();
    return new LoggingUserServiceProxy(realService, logger);
});

// Or using Scrutor library for decoration
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.Decorate<IUserService, LoggingUserServiceProxy>();
```

### Dynamic Proxy with DispatchProxy (.NET Built-in)

```csharp
// Create a generic logging proxy using DispatchProxy
public class LoggingProxy<T> : DispatchProxy where T : class
{
    private T? _target;
    private ILogger? _logger;

    public static T Create(T target, ILogger logger)
    {
        var proxy = Create<T, LoggingProxy<T>>() as LoggingProxy<T>;
        proxy!._target = target;
        proxy._logger = logger;
        return proxy as T ?? throw new InvalidOperationException();
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        _logger?.LogInformation("Calling {Method} with args: {Args}",
            targetMethod?.Name,
            string.Join(", ", args?.Select(a => a?.ToString() ?? "null") ?? Array.Empty<string>()));

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = targetMethod?.Invoke(_target, args);

            _logger?.LogInformation("Method {Method} completed in {ElapsedMs}ms",
                targetMethod?.Name,
                stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in {Method}", targetMethod?.Name);
            throw;
        }
    }
}

// Usage
var realService = new UserService(dbContext);
var logger = loggerFactory.CreateLogger<UserService>();

// Create dynamic proxy
var proxiedService = LoggingProxy<IUserService>.Create(realService, logger);

// All calls are automatically logged
await proxiedService.GetUserAsync(1);
```

### Castle DynamicProxy (Advanced AOP)

```csharp
// Using Castle.Core NuGet package
public class LoggingInterceptor : IInterceptor
{
    private readonly ILogger _logger;

    public LoggingInterceptor(ILogger logger)
    {
        _logger = logger;
    }

    public void Intercept(IInvocation invocation)
    {
        _logger.LogInformation("Calling {Method}", invocation.Method.Name);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            invocation.Proceed(); // Call the actual method

            _logger.LogInformation("Method {Method} completed in {ElapsedMs}ms",
                invocation.Method.Name,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {Method}", invocation.Method.Name);
            throw;
        }
    }
}

// Create proxy with Castle
var generator = new ProxyGenerator();
var logger = loggerFactory.CreateLogger("Proxy");
var interceptor = new LoggingInterceptor(logger);

var userService = new UserService(dbContext);
var proxy = generator.CreateInterfaceProxyWithTarget<IUserService>(
    userService,
    interceptor);

// All calls go through the interceptor
await proxy.GetUserAsync(1);
```

**Benefits of Dynamic Proxies:**
- No need to write proxy classes for each interface
- Can apply cross-cutting concerns (logging, caching, security) declaratively
- Aspect-Oriented Programming (AOP) capabilities
- Less boilerplate code

## 6. Expert Advice: When to Use vs When Not to Use

### ✅ When to Use Proxy:

- **Lazy Loading Heavy Objects**: Objects are expensive to create and may not always be needed
  ```csharp
  // Good: Large datasets loaded on demand
  var proxy = new Lazy<HugeDataset>(() => LoadFromDatabase());
  // Only loaded when proxy.Value is accessed
  ```

- **Access Control Required**: Need to enforce permissions or authorization
  ```csharp
  // Good: Admin-only operations protected by proxy
  services.AddScoped<IAdminService>(provider =>
      new AuthorizationProxy<IAdminService>(
          provider.GetRequiredService<AdminService>(),
          provider.GetRequiredService<ICurrentUser>()));
  ```

- **Remote Object Access**: Need local representation of remote objects
  ```csharp
  // Good: WCF, gRPC, HTTP clients acting as proxies
  var client = new OrderServiceClient("http://api.example.com");
  ```

- **Caching Expensive Operations**: Operations are costly and results can be reused
  ```csharp
  // Good: Database queries cached by proxy
  services.Decorate<IProductService, CachingProductServiceProxy>();
  ```

- **Logging and Auditing**: Need to track all operations for compliance
  ```csharp
  // Good: All repository operations logged for audit
  services.Decorate<IRepository, AuditLoggingProxy>();
  ```

- **Smart References**: Need reference counting, copy-on-write, or similar
  ```csharp
  // Good: Reference counting for resource management
  var smartRef = new ReferenceCountingProxy<ExpensiveResource>(resource);
  ```

### ❌ When NOT to Use Proxy:

- **Simple Objects**: Object creation is cheap and fast
  ```csharp
  // Bad: Unnecessary proxy for simple POCO
  public class PersonProxy : IPerson
  {
      private readonly Person _person = new Person(); // Cheap to create
      // No value added
  }
  ```

- **No Additional Behavior Needed**: Proxy would just pass through to real object
  ```csharp
  // Bad: Proxy that adds no value
  public class PointlessProxy : IService
  {
      private readonly IService _service;
      public void DoWork() => _service.DoWork(); // Just forwarding
  }
  // Better: Use the service directly
  ```

- **Performance Critical Code**: The indirection overhead matters
  ```csharp
  // Bad: Proxy in tight loop where every nanosecond counts
  for (int i = 0; i < 1000000; i++)
  {
      proxyCalculator.Add(i, 1); // Indirection overhead adds up
  }
  ```

- **Changing Requirements**: Interface changes frequently, making proxy maintenance hard

- **Already Have Middleware/Filters**: Framework provides better mechanisms
  ```csharp
  // Bad: Custom logging proxy in ASP.NET Core
  // Better: Use middleware, action filters, or logging infrastructure
  app.UseMiddleware<LoggingMiddleware>();
  ```

### 🎯 Expert Recommendations:

**1. Choose the Right Proxy Type**
```csharp
// Virtual Proxy: Lazy loading
var image = new Lazy<HighResImage>(() => new HighResImage(path));

// Protection Proxy: Access control
var secureRepo = new SecureRepositoryProxy(repo, currentUser);

// Caching Proxy: Performance
var cachedService = new CachingProxy(service, cache);

// Remote Proxy: Distributed systems
var client = new HttpClientProxy("http://api.example.com");

// Logging Proxy: Auditing
var auditedService = LoggingProxy<IService>.Create(service, logger);
```

**2. Use .NET's Built-in Proxies When Possible**
```csharp
// Good: Use Lazy<T> instead of writing custom virtual proxy
var lazyService = new Lazy<ExpensiveService>();

// Good: Use middleware instead of custom proxy for HTTP
app.UseMiddleware<CachingMiddleware>();

// Good: Use IHttpClientFactory with DelegatingHandler
services.AddHttpClient<IMyService, MyService>()
    .AddHttpMessageHandler<LoggingHandler>();
```

**3. Combine Proxies with Dependency Injection**
```csharp
// Good: Register proxies in DI container
services.AddScoped<IUserService, UserService>();
services.Decorate<IUserService, CachingUserServiceProxy>();
services.Decorate<IUserService, LoggingUserServiceProxy>();

// Creates chain: LoggingProxy -> CachingProxy -> UserService
```

**4. Keep Proxy and RealSubject Interface Consistent**
```csharp
// Good: Both implement same interface
public interface IDocument
{
    Task<string> GetContentAsync();
}

public class Document : IDocument { }
public class CachedDocumentProxy : IDocument { }

// Client code works with either transparently
IDocument doc = useCache ? new CachedDocumentProxy(realDoc) : realDoc;
```

**5. Consider Dynamic Proxies for Cross-Cutting Concerns**
```csharp
// Good: Use DispatchProxy or Castle for generic concerns
services.AddScoped(provider =>
{
    var service = ActivatorUtilities.CreateInstance<UserService>(provider);
    var logger = provider.GetRequiredService<ILogger<UserService>>();
    return LoggingProxy<IUserService>.Create(service, logger);
});

// Applies logging to any service without writing custom proxy classes
```

**6. Document Proxy Behavior**
```csharp
/// <summary>
/// Caching proxy for IProductService.
/// Caches GetProduct for 10 minutes.
/// Caches SearchProducts for 5 minutes.
/// Cache is cleared on SaveProduct or DeleteProduct.
/// </summary>
public class CachingProductServiceProxy : IProductService
{
    // Clear documentation helps maintainers understand behavior
}
```

**7. Test Proxy and RealSubject Separately**
```csharp
// Test real implementation
[Fact]
public async Task UserService_GetUser_ReturnsUser()
{
    var service = new UserService(mockContext.Object);
    var user = await service.GetUserAsync(1);
    Assert.NotNull(user);
}

// Test proxy behavior
[Fact]
public async Task CachingProxy_SecondCall_UsesCache()
{
    var mockService = new Mock<IUserService>();
    var proxy = new CachingUserServiceProxy(mockService.Object, cache);

    await proxy.GetUserAsync(1);
    await proxy.GetUserAsync(1);

    // Verify underlying service called only once
    mockService.Verify(s => s.GetUserAsync(1), Times.Once);
}
```

**8. Be Careful with Proxy Chains**
```csharp
// Be mindful of order in proxy chains
services.Decorate<IUserService, CachingProxy>();     // Check cache first
services.Decorate<IUserService, AuthorizationProxy>(); // Then check permissions
services.Decorate<IUserService, LoggingProxy>();       // Log everything

// Flow: LoggingProxy -> AuthorizationProxy -> CachingProxy -> UserService
// Consider if this order makes sense for your use case
```

**Real-World Decision Flow:**
1. Need to delay expensive object creation? → Virtual Proxy (or Lazy&lt;T&gt;)
2. Need access control? → Protection Proxy
3. Need to cache results? → Caching Proxy
4. Accessing remote service? → Remote Proxy
5. Need to log all operations? → Logging Proxy
6. Just wrapping with no added behavior? → Don't use Proxy

The Proxy pattern excels at controlling access to objects and adding cross-cutting concerns transparently. Use it when you need to add behavior without modifying the original object or when you need to control object lifecycle. Avoid it when the indirection adds no value or when better framework-specific mechanisms exist (middleware, filters, etc.).
