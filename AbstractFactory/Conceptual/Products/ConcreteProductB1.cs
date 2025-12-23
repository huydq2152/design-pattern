namespace Conceptual.Products;

// Concrete implementation of Product B from family 1
// Designed to work seamlessly with ConcreteProductA1
// Example: Windows Checkbox, Victorian Table, etc.
public class ConcreteProductB1 : IAbstractProductB
{
    public string UsefulFunctionB()
    {
        return "Product B1";
    }

    public string AnotherUsefulFunctionB(IAbstractProductA collaborator)
    {
        var result = collaborator.UsefulFunctionA();

        // Products from the same family work together harmoniously
        return $"Product B1 collaborating with ({result})";
    }
}