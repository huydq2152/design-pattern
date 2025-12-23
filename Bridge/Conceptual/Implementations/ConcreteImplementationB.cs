namespace Conceptual.Implementations;

// Concrete Implementation B provides an alternative platform-specific or variant-specific behavior
// This represents another implementation (e.g., Linux platform, MySQL database, SMS channel)
//
// The Bridge pattern allows abstractions to work with any implementation without modification
// You can add more concrete implementations (C, D, etc.) without changing abstractions
public class ConcreteImplementationB : IImplementation
{
    // Implements the low-level operation for Platform/Variant B
    // This could be Linux-specific rendering, MySQL queries, SMS sending, etc.
    // The implementation is completely different from Implementation A, but follows same interface
    public string OperationImplementation()
    {
        return "ConcreteImplementationB: Platform B specific result";
    }
}
