using Conceptual.Models;
using Newtonsoft.Json;

namespace Conceptual.Flyweights;

// Flyweight class stores the intrinsic (shared) state
// Multiple car instances with the same Company, Model, Color share one Flyweight
//
// Key characteristics:
// - Stores only intrinsic state (immutable, shared data)
// - Receives extrinsic state as method parameters
// - Can be shared across many contexts without duplication
//
// Memory savings example:
// - Without Flyweight: 1000 cars = 1000 full objects
// - With Flyweight: 1000 cars = 50 flyweights + 1000 lightweight references
public class Flyweight
{
    // Intrinsic state: Shared across all cars of the same type
    // This is immutable (readonly) to prevent corruption of shared state
    // Stores Company, Model, and Color which define a car type
    private readonly Car _sharedState;

    // Constructor initializes the shared state
    // This state will never change after creation (immutability)
    public Flyweight(Car sharedState)
    {
        _sharedState = sharedState;
    }

    // Operation that combines intrinsic state with extrinsic state
    // The flyweight uses its stored shared state along with the unique state
    // passed as a parameter to perform the operation
    //
    // Pattern:
    // - Intrinsic state: Stored in _sharedState (Company, Model, Color)
    // - Extrinsic state: Passed as parameter uniqueState (Owner, Number)
    //
    // This allows one flyweight to service multiple unique car instances
    public void Operation(Car uniqueState)
    {
        var s = JsonConvert.SerializeObject(_sharedState);
        var u = JsonConvert.SerializeObject(uniqueState);
        Console.WriteLine($"Flyweight: Displaying shared {s} and unique {u} state.");
    }
}
