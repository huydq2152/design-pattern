# Factory Method Pattern
## 1. Definition
The Factory Method Pattern defines an interface for creating an object, but lets subclasses decide which class to instantiate. This pattern lets a class defer instantiation to subclasses, promoting loose coupling by eliminating the need to bind application-specific classes into the code.

### Manual Implementation
1. Define a Product interface/abstract class that declares the operations that all concrete products must implement.
2. Create Concrete Product classes that implement the Product interface.
3. Define a Creator abstract class that declares the factory method, which returns an object of type Product. The Creator may also define some default implementation of the factory method.
4. Create Concrete Creator classes that override the factory method to return instances of Concrete Products.

## 2. Pros
- **Loose Coupling**: Client code doesn't depend on concrete product classes, only on the product interface
- **Open/Closed Principle**: Easy to add new product types without modifying existing creator code
- **Single Responsibility**: Product creation logic is separated from business logic
- **Polymorphism**: Subclasses can override factory method to create different product variants
- **Code Reusability**: Common creation logic can be shared across different creators

## 3. Cons
- **Complexity Increase**: Requires creating many subclasses, which can make code more complex
- **Inheritance Dependency**: Heavily relies on inheritance, which can be limiting in some scenarios
- **Indirect Object Creation**: Makes it harder to understand which concrete class is being instantiated
- **Runtime Performance**: Additional method calls and polymorphism can introduce slight overhead

## 4. Real-world Use Cases in C# & .NET
- **Database Connections**: Different database providers (SQL Server, MySQL, Oracle)
```csharp
// ADO.NET factory pattern
DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
DbConnection connection = factory.CreateConnection();
```
- **UI Components**: Different platform renderings (WPF, WinForms, Web)
- **Document Processors**: Creating different document types (PDF, Word, Excel)
- **Payment Processing**: Different payment gateways (PayPal, Stripe, Square)
- **Logging Frameworks**: Different log targets (File, Database, Console)
- **Storage Services**: Different storage providers (Azure Blob, AWS S3, Google Cloud Storage)

## 5. Modern Approach: Dependency Injection & Factory Services
```csharp
// Register factories in Program.cs
builder.Services.AddScoped<IPaymentProcessorFactory, PaymentProcessorFactory>();
builder.Services.AddScoped<IPaymentProcessor, PayPalProcessor>();
builder.Services.AddScoped<IPaymentProcessor, StripeProcessor>();

// Factory implementation
public class PaymentProcessorFactory : IPaymentProcessorFactory
{
    private readonly IServiceProvider _serviceProvider;
    
    public PaymentProcessorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public IPaymentProcessor CreateProcessor(PaymentType type)
    {
        return type switch
        {
            PaymentType.PayPal => _serviceProvider.GetRequiredService<PayPalProcessor>(),
            PaymentType.Stripe => _serviceProvider.GetRequiredService<StripeProcessor>(),
            _ => throw new ArgumentException("Unknown payment type")
        };
    }
}
```

**Benefits over traditional Factory Method:**
- Automatic dependency injection into created objects
- Better testability with mock factories
- Centralized dependency management
- Flexible lifetime management (Singleton, Scoped, Transient)

## 6. Expert Advice: When to Use vs When Not to Use

### ✅ When to Use Factory Method:
- **Multiple Product Variants**: When you have several implementations of the same interface
- **Runtime Object Selection**: When the concrete class to instantiate is determined at runtime
- **Complex Object Creation**: When object creation involves multiple steps or configuration
- **Library/Framework Development**: When you want to provide extension points for users
- **Platform-Specific Implementation**: Different implementations for Windows, Linux, macOS
- **Plugin Architecture**: When supporting third-party extensions

### ❌ When NOT to Use Factory Method:
- **Simple Object Creation**: When `new ClassName()` is sufficient
- **Single Implementation**: When there's only one concrete implementation
- **Known Dependencies**: When all dependencies are known at compile time
- **Performance Critical**: When the overhead of factory method calls matters
- **Small Applications**: When the abstraction adds unnecessary complexity

### 🎯 Expert Recommendations:
- **Start Simple**: Don't use factories until you actually need multiple implementations
- **Favor Dependency Injection**: Modern DI containers handle most factory scenarios
- **Use Abstract Factory** when you need families of related objects
- **Consider Generic Factories**: `IFactory<T>` for type-safe generic creation
- **Implement Async Factories**: For objects requiring async initialization
- **Document Factory Contracts**: Clearly specify what each factory method creates