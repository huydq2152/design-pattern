namespace Conceptual.Target;

// Target interface defines the domain-specific interface that the client expects
// This is the interface that the client code works with
// The Adapter will implement this interface to make the Adaptee compatible
//
// In real-world scenarios, this could be:
// - IPaymentProcessor, IDataReader, ILogger, INotificationService, etc.
public interface ITarget
{
    // Standard method that the client expects to call
    // The Adapter will translate this call to the Adaptee's incompatible interface
    string GetRequest();
}
