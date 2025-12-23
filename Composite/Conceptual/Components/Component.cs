namespace Conceptual.Components;

// Component is the base class for both Leaf and Composite objects
// It defines the common interface that allows treating individual objects
// and compositions uniformly (the core principle of the Composite pattern)
//
// Key responsibilities:
// - Declare operations common to both simple (Leaf) and complex (Composite) objects
// - Optionally define default behavior for child management operations
// - Provide a way to check if a component is composite or leaf
//
// Design decision: This uses an abstract class instead of an interface to provide
// default implementations for Add/Remove (throwing exceptions for leaves)
public abstract class Component
{
    // Abstract operation that all components must implement
    // This represents the core business logic that both leaves and composites support
    // In real scenarios, this could be Render(), Calculate(), Display(), etc.
    public abstract string Operation();

    // Virtual method for adding child components
    // Default implementation throws exception because leaves can't have children
    // Only Composite class will override this to actually add children
    //
    // Alternative design: Make this abstract and force all classes to implement
    // Trade-off: Current approach keeps Leaf classes simpler but less type-safe
    public virtual void Add(Component component)
    {
        throw new NotSupportedException("Cannot add child to a leaf component");
    }

    // Virtual method for removing child components
    // Default implementation throws exception because leaves can't have children
    // Only Composite class will override this to actually remove children
    public virtual void Remove(Component component)
    {
        throw new NotSupportedException("Cannot remove child from a leaf component");
    }

    // Allows client code to check if a component can have children
    // Returns true for Composite, false for Leaf
    // This can help avoid exceptions when trying to add children
    //
    // Note: This method slightly violates the principle of uniform treatment
    // In pure Composite pattern, clients shouldn't need to distinguish types
    // However, it's useful in practice for certain operations
    public virtual bool IsComposite()
    {
        return true; // Default to true, Leaf will override to return false
    }
}
