using Conceptual.Products;

namespace Conceptual.Factories;

// Concrete factory for creating products from Family 2 (e.g., MacOS UI elements)
// All products created by this factory are designed to work together
public class ConcreteFactory2 : IAbstractFactory
{
    public IAbstractProductA CreateProductA()
    {
        // Returns a specific product from family 2 (A2)
        return new ConcreteProductA2();
    }

    public IAbstractProductB CreateProductB()
    {
        // Returns a specific product from family 2 (B2)
        // This product is designed to collaborate with ProductA2  
        return new ConcreteProductB2();
    }
}