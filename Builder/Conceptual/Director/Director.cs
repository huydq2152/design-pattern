using Conceptual.Builders;

namespace Conceptual.Director;

// Director class defines the order in which construction steps are executed
// It's optional - clients can work with builders directly
// The Director is useful when you have common construction sequences that should be reusable
//
// Key Responsibility: Encapsulates common product configurations
// The Director knows HOW to build products, but doesn't know WHAT concrete products are being built
public class Director
{
    // Reference to a builder instance
    // The Director works through the IBuilder interface, so it can work with any concrete builder
    private IBuilder _builder = null!;

    // Property to set the builder instance
    // Allows changing the builder at runtime to create different product representations
    // Using a property instead of constructor injection provides flexibility
    public IBuilder Builder
    {
        set => _builder = value;
    }

    // Builds a minimal product with only Part A
    // This method demonstrates that the Builder pattern supports partial construction
    // Not all parts need to be included in every product variant
    public void BuildMinimalProduct()
    {
        _builder.BuildPartA();
    }

    // Builds a complete product with all available parts
    // This method demonstrates a common construction sequence
    // The Director defines the order: A -> B -> C
    public void BuildFullFeaturedProduct()
    {
        _builder.BuildPartA();
        _builder.BuildPartB();
        _builder.BuildPartC();
    }

    // You can add more methods for different product configurations
    // Example: BuildProductWithPartsAAndC(), BuildCustomProduct(), etc.
    // This is where the Director adds value - encapsulating common build sequences
}
