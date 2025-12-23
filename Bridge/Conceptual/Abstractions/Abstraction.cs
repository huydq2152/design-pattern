using Conceptual.Implementations;

namespace Conceptual.Abstractions;

// Abstraction defines the high-level interface for client code
// It contains a reference to an Implementation object - this is the "bridge"
//
// Key responsibilities:
// - Provides high-level operations built on top of low-level implementation operations
// - Delegates implementation-specific work to the Implementation object
// - Can be extended through inheritance to add more high-level operations
//
// The Abstraction and Implementation can vary independently:
// - You can add new abstraction subclasses without changing implementations
// - You can add new implementations without changing abstractions
public class Abstraction
{
    // The bridge: Reference to the implementation
    // This is protected to allow refined abstractions to access it
    // The abstraction delegates implementation-specific operations to this object
    protected IImplementation _implementation;

    // Constructor injection of the implementation
    // This allows any implementation to be combined with any abstraction at runtime
    // Demonstrates the flexibility of the Bridge pattern
    public Abstraction(IImplementation implementation)
    {
        _implementation = implementation;
    }

    // High-level operation that uses the low-level implementation
    // The abstraction defines WHAT to do (high-level logic)
    // The implementation defines HOW to do it (platform-specific details)
    public virtual string Operation()
    {
        // High-level logic combines with low-level implementation
        // This method can be overridden by refined abstractions
        return "Abstraction: Base operation with:\n" +
               _implementation.OperationImplementation();
    }
}
