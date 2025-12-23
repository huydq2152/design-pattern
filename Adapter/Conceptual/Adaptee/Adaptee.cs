namespace Conceptual.Adaptee;

// Adaptee is an existing class with an incompatible interface
// This represents third-party libraries, legacy code, or external services
// that have useful functionality but don't match the interface expected by the client
//
// Key characteristics:
// - Has useful functionality that the client needs
// - Has an incompatible interface (different method names, parameters, or return types)
// - Cannot or should not be modified (third-party code, legacy system, etc.)
//
// Real-world examples:
// - Third-party payment gateway with different method signatures
// - Legacy database client with outdated API
// - External API with non-standard naming conventions
public class Adaptee
{
    // Method with an incompatible name and potentially different signature
    // The client cannot call this directly because it doesn't match the ITarget interface
    // The Adapter will bridge this gap by wrapping this method
    public string GetSpecificRequest()
    {
        return "Specific request from Adaptee";
    }
}
