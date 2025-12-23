using Conceptual.Products;

namespace Conceptual.Factories;

// Concrete factory for creating products from Family 1 (e.g., Windows UI elements)
// All products created by this factory are designed to work together
public class ConcreteFactory1 : IAbstractFactory
{
    public IAbstractProductA CreateProductA()
    {
        // Returns a specific product from family 1 (A1)
        return new ConcreteProductA1();
    }

    public IAbstractProductB CreateProductB()
    {
        // Returns a specific product from family 1 (B1) 
        // This product is designed to collaborate with ProductA1
        return new ConcreteProductB1();
    }
}