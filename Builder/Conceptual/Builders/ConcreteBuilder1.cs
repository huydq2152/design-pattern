using Conceptual.Products;

namespace Conceptual.Builders;

// Concrete Builder implements the construction steps defined in IBuilder
// This builder creates a specific representation of the Product
// Different concrete builders can create different product variants (ConcreteBuilder2, etc.)
public class ConcreteBuilder1 : IBuilder
{
    // Private product instance that's being constructed
    // This is the core of the Builder pattern - the builder maintains state during construction
    private Product _product = new();

    // Constructs part A with a specific implementation
    // This builder creates "PartA1", but another builder might create "PartA2"
    public void BuildPartA()
    {
        _product.Add("PartA1");
    }

    // Constructs part B with a specific implementation
    // Each build method adds specific parts to the product
    public void BuildPartB()
    {
        _product.Add("PartB1");
    }

    // Constructs part C with a specific implementation
    // The builder allows partial construction - not all parts are required
    public void BuildPartC()
    {
        _product.Add("PartC1");
    }

    // Resets the builder to initial state
    // Called after GetProduct() to prepare the builder for constructing a new product
    // This allows builder reuse without creating new builder instances
    private void Reset()
    {
        _product = new Product();
    }

    // Returns the constructed product and resets the builder
    // The Reset() call ensures the builder is ready to build another product
    // This prevents accidentally modifying previously built products
    public Product GetProduct()
    {
        var result = _product;
        Reset(); // Important: Reset after returning the product
        return result;
    }
}
