using Conceptual.Products;

namespace Conceptual.Factories;

// Abstract Factory interface defines methods to create each product type
// This ensures all concrete factories implement the same creation methods
public interface IAbstractFactory
{
    // Creates product from family A (e.g., Windows Button, MacOS Button)
    IAbstractProductA CreateProductA();
    
    // Creates product from family B (e.g., Windows Checkbox, MacOS Checkbox)  
    IAbstractProductB CreateProductB();
}