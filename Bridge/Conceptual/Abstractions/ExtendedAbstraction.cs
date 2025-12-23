using Conceptual.Implementations;

namespace Conceptual.Abstractions;

// Extended Abstraction (Refined Abstraction) extends the base abstraction
// Adds more high-level operations or modifies existing ones
//
// Key points:
// - Inherits from Abstraction, gaining access to the implementation bridge
// - Can add new high-level operations or override existing ones
// - Works with any implementation through the inherited bridge reference
// - Demonstrates how abstractions can evolve independently from implementations
//
// Examples in real scenarios:
// - AdvancedRemote extends RemoteControl (works with any IDevice)
// - UrgentNotification extends Notification (works with any INotificationChannel)
// - RoundButton extends Button (works with any IRenderer)
public class ExtendedAbstraction : Abstraction
{
    // Constructor passes implementation to base class
    // The extended abstraction can work with any implementation
    // just like the base abstraction
    public ExtendedAbstraction(IImplementation implementation) : base(implementation)
    {
    }

    // Override the base operation to provide extended/refined behavior
    // Still uses the same implementation bridge, but adds different high-level logic
    // Demonstrates how abstractions can vary while using the same implementations
    public override string Operation()
    {
        // Extended abstraction provides different high-level behavior
        // but still delegates to the same implementation interface
        return "ExtendedAbstraction: Extended operation with:\n" +
               base._implementation.OperationImplementation();
    }

    // You can also add entirely new operations specific to this refined abstraction
    // public void AdditionalOperation() { ... }
}
