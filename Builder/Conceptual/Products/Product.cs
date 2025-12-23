namespace Conceptual.Products;

// Product class represents the complex object being constructed
// The builder pattern is especially useful when products have many parts or complex assembly
// This class stores parts that are added incrementally during the build process
public class Product
{
    // Internal collection to store product parts
    // Parts are added one by one through the builder's construction methods
    private readonly List<string> _parts = new();

    // Adds a part to the product
    // Called by builder methods (BuildPartA, BuildPartB, etc.)
    // This demonstrates incremental construction - the key feature of the Builder pattern
    public void Add(string part)
    {
        _parts.Add(part);
    }

    // Returns a string representation of all parts in the product
    // Used to display the final constructed product and verify the build process
    public string GetParts()
    {
        var result = string.Empty;
        foreach (var part in _parts)
        {
            result += $"{part}, ";
        }

        // Remove trailing comma and space if parts exist
        if (result.Length >= 2)
        {
            result = result.Remove(result.Length - 2);
        }

        return $"Product parts: {result}\n";
    }
}
