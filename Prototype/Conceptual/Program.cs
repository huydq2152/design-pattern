using Conceptual.Prototypes;

// Client code demonstrating the Prototype pattern
// Shows the critical difference between shallow copy and deep copy
public static class Program
{
    public static void Main()
    {
        // Create original Person object with nested reference type (IdInfo)
        var p1 = new Person
        {
            Age = 42,
            DoB = Convert.ToDateTime("1977-01-01"),
            Name = "Jack Daniels",
            IdInfo = new IdInfo(666)
        };

        // SHALLOW COPY: Copies references to reference type properties
        // p2.IdInfo points to the SAME IdInfo object as p1.IdInfo
        // Changes to p1.IdInfo will affect p2.IdInfo and vice versa
        var p2 = p1.ShallowCopy();

        // DEEP COPY: Creates new instances of all reference type properties
        // p3.IdInfo is a completely NEW IdInfo object with copied values
        // Changes to p1.IdInfo will NOT affect p3.IdInfo
        var p3 = p1.DeepCopy();

        // Display initial values to show all three objects have same data
        Console.WriteLine("Original values of p1, p2, p3:");
        Console.WriteLine("   p1 instance values: ");
        DisplayValues(p1);
        Console.WriteLine("   p2 instance values:");
        DisplayValues(p2);
        Console.WriteLine("   p3 instance values:");
        DisplayValues(p3);

        // Modify p1's properties to demonstrate shallow vs deep copy behavior
        // Value type changes (Age, DoB) won't affect copies
        // String change (Name) won't affect copies because strings are immutable
        // Reference type change (IdInfo.IdNumber) will affect shallow copy (p2) but NOT deep copy (p3)
        p1.Age = 32;
        p1.DoB = Convert.ToDateTime("1900-01-01");
        p1.Name = "Frank";
        p1.IdInfo.IdNumber = 7878; // This change affects p2 but NOT p3

        Console.WriteLine("\nValues of p1, p2 and p3 after changes to p1:");
        Console.WriteLine("   p1 instance values: ");
        DisplayValues(p1);

        // p2 (shallow copy) - IdNumber changed because p2.IdInfo references same object as p1.IdInfo
        Console.WriteLine("   p2 instance values (IdInfo reference shared with p1):");
        DisplayValues(p2);

        // p3 (deep copy) - All values remain unchanged because p3 has independent copies
        Console.WriteLine("   p3 instance values (completely independent from p1):");
        DisplayValues(p3);
    }

    // Helper method to display Person object values
    private static void DisplayValues(Person p)
    {
        Console.WriteLine("      Name: {0:s}, Age: {1:d}, BirthDate: {2:MM/dd/yy}",
            p.Name, p.Age, p.DoB);
        Console.WriteLine("      ID#: {0:d}", p.IdInfo.IdNumber);
    }
}

/*
 * KEY TAKEAWAYS:
 *
 * SHALLOW COPY (MemberwiseClone):
 * - Copies all fields to a new object
 * - Value types: Actual values are copied (independent)
 * - Reference types: Only memory addresses (references) are copied (shared)
 * - Fast and simple but creates shared state for reference type properties
 * - Use when: Objects have no reference type properties or sharing is acceptable
 *
 * DEEP COPY (Manual cloning):
 * - Recursively creates new instances of all reference type properties
 * - Ensures complete independence between original and clone
 * - More complex to implement, especially with circular references
 * - Use when: Need complete object independence with no shared state
 *
 * WHEN TO USE EACH:
 * - Shallow Copy: Configuration objects with primitive types, DTOs with value types
 * - Deep Copy: Complex object graphs, undo/redo functionality, object caching
 */
