using Conceptual.Components;

namespace Conceptual.Decorators;

// Base Decorator class that wraps a Component
// This abstract class implements Component and contains a reference to another Component
//
// Key responsibilities:
// - Maintains a reference to the wrapped component
// - Implements Component interface by delegating to wrapped component
// - Provides a base for concrete decorators to extend
//
// Design pattern: Uses composition (wrapping) instead of inheritance
// This allows adding behavior dynamically at runtime
public abstract class Decorator : Component
{
    // Reference to the wrapped component
    // This can be a ConcreteComponent or another Decorator (decorator stacking)
    // Nullable to allow flexible initialization (though typically set via constructor)
    protected Component? _component;

    // Constructor injection of the component to wrap
    // Ensures decorator always wraps something
    protected Decorator(Component component)
    {
        _component = component;
    }

    // Allows changing the wrapped component after construction
    // Provides flexibility but can lead to null reference issues
    // In practice, prefer immutable decorators (constructor-only initialization)
    public void SetComponent(Component component)
    {
        _component = component;
    }

    // Default implementation delegates to the wrapped component
    // Concrete decorators override this to add behavior before/after delegation
    //
    // Pattern:
    // 1. Perform pre-processing (logging, validation, etc.)
    // 2. Call wrapped component: base.Operation() or _component.Operation()
    // 3. Perform post-processing (logging, caching result, etc.)
    // 4. Return result (possibly modified)
    public override string Operation()
    {
        if (_component != null)
        {
            // Delegate to wrapped component
            return _component.Operation();
        }
        else
        {
            return string.Empty;
        }
    }
}
