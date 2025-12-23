namespace Conceptual.Products;

// Concrete implementation of Product B from family 2  
// Designed to work seamlessly with ConcreteProductA2
// Example: MacOS Checkbox, Modern Table, etc.
public class ConcreteProductB2 : IAbstractProductB
{
    public string UsefulFunctionB()
    {
        return "Product B2";
    }

    public string AnotherUsefulFunctionB(IAbstractProductA collaborator)
    {
        var result = collaborator.UsefulFunctionA();

        // Products from the same family work together harmoniously
        return $"Product B2 collaborating with ({result})";
    }
}