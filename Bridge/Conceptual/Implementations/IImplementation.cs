namespace Conceptual.Implementations;

// Implementation interface defines the contract for low-level operations
// This is the "bridge" interface that connects abstraction to concrete implementations
//
// Key characteristics:
// - Contains primitive/low-level operations
// - Platform-specific or technology-specific methods
// - Can vary independently from the abstraction hierarchy
//
// In real-world scenarios, this could be:
// - IRenderer (for UI rendering on different platforms)
// - IDatabase (for different database providers)
// - IDevice (for different device types)
// - INotificationChannel (for different messaging channels)
public interface IImplementation
{
    // Low-level operation that concrete implementations must provide
    // Different implementations (Platform A, Platform B, etc.) will provide specific behavior
    // The abstraction layer will call this method through the bridge reference
    string OperationImplementation();
}
