using Conceptual.Target;
using AdapteeClass = Conceptual.Adaptee.Adaptee;

namespace Conceptual.Adapter;

// Adapter class implements the Target interface and wraps the Adaptee
// This is the core of the Adapter pattern - it makes incompatible interfaces compatible
//
// How it works:
// 1. Implements the ITarget interface (what the client expects)
// 2. Contains an instance of the Adaptee (composition pattern)
// 3. Translates calls from ITarget methods to Adaptee methods
//
// This is an "Object Adapter" using composition (preferred in C#)
// Alternative: "Class Adapter" using multiple inheritance (not supported in C#)
public class Adapter : ITarget
{
    // Reference to the Adaptee being adapted
    // The Adapter uses composition to wrap the incompatible class
    // This allows the Adapter to reuse existing Adaptee functionality
    private readonly AdapteeClass _adaptee;

    // Constructor injection of the Adaptee
    // The Adapter takes responsibility for managing the Adaptee instance
    // In DI scenarios, both Adapter and Adaptee can be injected
    public Adapter(AdapteeClass adaptee)
    {
        _adaptee = adaptee;
    }

    // Implements ITarget.GetRequest() by adapting Adaptee.GetSpecificRequest()
    // This method performs the interface translation:
    // - Client calls GetRequest() on ITarget
    // - Adapter translates to GetSpecificRequest() on Adaptee
    // - May transform parameters, return values, or add additional logic
    public string GetRequest()
    {
        // Call the Adaptee's incompatible method and adapt the result
        // You can transform data, convert formats, or add wrapper logic here
        return $"Adapter: (Translated) {_adaptee.GetSpecificRequest()}";
    }
}
