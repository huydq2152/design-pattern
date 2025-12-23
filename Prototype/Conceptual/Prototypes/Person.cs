namespace Conceptual.Prototypes;

// Concrete Prototype class demonstrating both shallow and deep copy implementations
// Shows the critical difference between copying references vs copying entire object graphs
public class Person : IPrototype<Person>
{
    // Value type properties - automatically copied in both shallow and deep copy
    public int Age;
    public DateTime DoB;

    // String is a reference type but immutable in C# - behaves like value type in copying
    public string Name;

    // Reference type property - demonstrates the difference between shallow and deep copy
    // In shallow copy: only the reference is copied (both objects point to same IdInfo)
    // In deep copy: a new IdInfo instance is created (independent objects)
    public IdInfo IdInfo;

    // Shallow Copy Implementation
    // Uses MemberwiseClone() which creates a new object and copies all fields
    // For value types: copies the actual values
    // For reference types: copies only the memory address (reference)
    public Person ShallowCopy()
    {
        // MemberwiseClone is a protected method inherited from System.Object
        // It performs a shallow copy of the current object
        return (Person)MemberwiseClone();
    }

    // Deep Copy Implementation
    // Manually creates new instances of all reference type properties
    // Ensures complete independence between original and cloned object
    public Person DeepCopy()
    {
        // Start with shallow copy to handle value types
        var result = (Person)MemberwiseClone();

        // Manually clone reference type properties
        // Create a new IdInfo instance instead of copying the reference
        result.IdInfo = new IdInfo(IdInfo.IdNumber);

        // String is immutable, so we can safely assign it
        // Even though it's a reference type, strings behave like value types
        result.Name = Name;

        return result;
    }
}
