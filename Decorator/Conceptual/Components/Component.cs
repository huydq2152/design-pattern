namespace Conceptual.Components;

// Component is the base interface/class for both concrete components and decorators
// It defines the common interface that allows decorators to wrap components uniformly
//
// Key characteristics:
// - Declares operations that can be altered by decorators
// - Both ConcreteComponent and Decorator implement this
// - Allows decorators to be used in place of concrete components (substitutability)
//
// In real-world scenarios, this could be:
// - INotificationService, IRepository, ILogger, Stream, etc.
public abstract class Component
{
    // Core operation that both concrete components and decorators implement
    // Concrete components provide base implementation
    // Decorators add behavior before/after delegating to wrapped component
    public abstract string Operation();
}
