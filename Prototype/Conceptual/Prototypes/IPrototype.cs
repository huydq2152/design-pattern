namespace Conceptual.Prototypes;

// Prototype interface defines the contract for cloneable objects
// Provides methods for both shallow and deep copying
public interface IPrototype<T>
{
    // Shallow copy: Copies value types and references to reference types
    // Changes to reference type properties affect both original and clone
    T ShallowCopy();

    // Deep copy: Recursively copies all value and reference type properties
    // Creates completely independent clone with no shared references
    T DeepCopy();
}
