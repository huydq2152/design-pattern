# Strategy Pattern
## 1. Definition
The Strategy Pattern defines a family of algorithms, encapsulates each one, and makes them interchangeable. This pattern lets the algorithm vary independently from clients that use it. It allows selecting algorithms at runtime and promotes the Open/Closed Principle by making it easy to add new algorithms without modifying existing code.

## 2. Pros
- **Runtime Algorithm Selection**: Can switch algorithms dynamically without changing the context
- **Open/Closed Principle**: Easy to add new strategies without modifying existing code
- **Eliminates Conditional Logic**: Replaces complex if/else or switch statements with polymorphism
- **Improved Testability**: Each strategy can be tested independently with mock objects
- **Code Reusability**: Strategies can be reused across different contexts
- **Single Responsibility**: Each strategy class has one reason to change

## 3. Cons
- **Increased Complexity**: Requires creating many strategy classes for simple scenarios
- **Client Awareness**: Clients must know about different strategies to choose appropriately
- **Object Overhead**: Creates additional objects which may impact memory in resource-constrained environments
- **Interface Rigidity**: All strategies must follow the same interface, which may not fit all algorithms
- **Indirection**: Additional layer of abstraction can make code harder to follow

## 4. Real-world Use Cases in C# & .NET
- **Sorting Algorithms**: Different sorting strategies (QuickSort, MergeSort, BubbleSort)
```csharp
// Payment processing with different strategies
IPaymentStrategy strategy = paymentType switch
{
    PaymentType.CreditCard => new CreditCardStrategy(),
    PaymentType.PayPal => new PayPalStrategy(),
    PaymentType.BankTransfer => new BankTransferStrategy(),
    _ => throw new ArgumentException("Unknown payment type")
};
```
- **Validation Rules**: Different validation strategies for forms, APIs, or business rules
- **Compression Algorithms**: ZIP, RAR, 7Z compression strategies
- **Discount Calculations**: Percentage, fixed amount, buy-one-get-one discounts
- **Authentication Methods**: OAuth, JWT, Basic Auth, API Key strategies
- **Caching Strategies**: In-Memory, Redis, Database caching approaches

## 5. Modern Approach: Dependency Injection & Strategy Services
```csharp
// Register strategies in Program.cs
builder.Services.AddScoped<IDiscountStrategy, PercentageDiscountStrategy>();
builder.Services.AddScoped<IDiscountStrategy, FixedAmountDiscountStrategy>();
builder.Services.AddScoped<IDiscountStrategyFactory, DiscountStrategyFactory>();

// Strategy factory implementation
public class DiscountStrategyFactory : IDiscountStrategyFactory
{
    private readonly IServiceProvider _serviceProvider;
    
    public DiscountStrategyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public IDiscountStrategy CreateStrategy(DiscountType type)
    {
        return type switch
        {
            DiscountType.Percentage => _serviceProvider.GetRequiredService<PercentageDiscountStrategy>(),
            DiscountType.FixedAmount => _serviceProvider.GetRequiredService<FixedAmountDiscountStrategy>(),
            _ => throw new ArgumentException($"Unknown discount type: {type}")
        };
    }
}

// Usage in service
public class OrderService
{
    private readonly IDiscountStrategyFactory _strategyFactory;
    
    public OrderService(IDiscountStrategyFactory strategyFactory)
    {
        _strategyFactory = strategyFactory;
    }
    
    public decimal CalculateTotal(Order order, DiscountType discountType)
    {
        var strategy = _strategyFactory.CreateStrategy(discountType);
        return strategy.ApplyDiscount(order.Subtotal, order.DiscountValue);
    }
}
```

**Benefits over traditional Strategy:**
- Automatic dependency injection into strategy implementations
- Centralized strategy management through DI container
- Better testability with mock strategies and factories
- Configuration-based strategy selection
- Integration with logging, configuration, and other services

## 6. Expert Advice: When to Use vs When Not to Use

### ✅ When to Use Strategy:
- **Multiple Algorithm Variants**: When you have several ways to perform the same task
- **Runtime Algorithm Selection**: When the algorithm choice depends on user input or configuration
- **Complex Conditional Logic**: When you have many if/else or switch statements for algorithm selection
- **Varying Business Rules**: Different calculation methods, validation rules, or processing workflows
- **Plugin Architecture**: When supporting third-party or configurable algorithms
- **A/B Testing**: When you need to compare different implementations

### ❌ When NOT to Use Strategy:
- **Single Algorithm**: When there's only one way to perform the task
- **Simple Conditional Logic**: When a simple if/else statement is sufficient
- **Algorithms Never Change**: When the algorithm is stable and won't have variants
- **Performance Critical**: When the overhead of strategy objects matters significantly
- **Tightly Coupled Algorithms**: When algorithms require specific context knowledge

### 🎯 Expert Recommendations:
- **Use Enum for Strategy Types**: Define strategy types clearly with enums
- **Implement Strategy Factory**: Use factories to manage strategy creation and selection
- **Consider Generic Strategies**: `IStrategy<TInput, TOutput>` for type-safe implementations
- **Cache Strategy Instances**: For expensive-to-create strategies, consider caching
- **Document Strategy Contracts**: Clearly specify input/output expectations and side effects
- **Validate Strategy Input**: Always validate input parameters in strategy implementations
- **Plan for Strategy Evolution**: Design strategy interfaces with future requirements in mind