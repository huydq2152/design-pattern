using Conceptual.Components;

namespace Conceptual.Decorators;

// ConcreteDecoratorA adds specific behavior to the wrapped component
// This decorator extends the base Decorator and adds "Feature A" functionality
//
// Characteristics:
// - Wraps a Component (concrete component or another decorator)
// - Adds behavior before/after delegating to wrapped component
// - Can be combined with other decorators (decorator stacking)
//
// Real-world examples:
// - LoggingDecorator: Adds logging around method calls
// - CachingDecorator: Adds caching to avoid repeated operations
// - ValidationDecorator: Adds input validation before delegating
// - RetryDecorator: Adds retry logic around operations
public class ConcreteDecoratorA : Decorator
{
    // Constructor ensures component is wrapped
    // Passes component to base Decorator constructor
    public ConcreteDecoratorA(Component component) : base(component)
    {
    }

    // Overrides Operation to add specific behavior
    // Pattern: Wrap the base operation with additional functionality
    //
    // This implementation:
    // 1. Calls base.Operation() to get result from wrapped component
    // 2. Wraps the result with "ConcreteDecoratorA(...)"
    // 3. Returns the enhanced result
    //
    // In real scenarios, this might:
    // - Log before/after the operation
    // - Validate input before delegating
    // - Transform the result
    // - Add error handling
    public override string Operation()
    {
        // Call the wrapped component's operation and enhance it
        // base.Operation() delegates to _component.Operation()
        return $"ConcreteDecoratorA({base.Operation()})";
    }

    // Note: Can add additional methods specific to this decorator
    // However, clients using Component interface won't see these methods
    // This is a trade-off: transparency vs. type safety
}
