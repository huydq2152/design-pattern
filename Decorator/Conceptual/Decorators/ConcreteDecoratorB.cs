using Conceptual.Components;

namespace Conceptual.Decorators;

// ConcreteDecoratorB adds different behavior than DecoratorA
// This demonstrates that multiple decorators can add different responsibilities
//
// Characteristics:
// - Independent from DecoratorA (different behavior)
// - Can be stacked with DecoratorA in any order
// - Follows the same pattern: wrap and enhance
//
// Real-world examples:
// - CompressionDecorator: Adds compression to streams
// - EncryptionDecorator: Adds encryption to data
// - RateLimitingDecorator: Adds rate limiting to services
// - CircuitBreakerDecorator: Adds circuit breaker pattern
public class ConcreteDecoratorB : Decorator
{
    // Constructor ensures component is wrapped
    // Can wrap ConcreteComponent, DecoratorA, or any other Component
    public ConcreteDecoratorB(Component component) : base(component)
    {
    }

    // Adds "Feature B" behavior to the operation
    // Similar pattern to DecoratorA but with different enhancement
    //
    // Decorator stacking example:
    // - DecoratorB(DecoratorA(ConcreteComponent))
    // - Result: "ConcreteDecoratorB(ConcreteDecoratorA(ConcreteComponent))"
    //
    // Order matters:
    // - DecoratorA(DecoratorB(...)) would produce different result
    // - For logging+caching: often want logging outside caching to log cache hits/misses
    public override string Operation()
    {
        // Add different enhancement than DecoratorA
        // Both decorators can coexist in the same chain
        return $"ConcreteDecoratorB({base.Operation()})";
    }

    // Can add decorator-specific methods if needed
    // Example: public void SpecificBehavior() { ... }
    // Trade-off: Loses transparency if clients need to know about specific decorator
}
