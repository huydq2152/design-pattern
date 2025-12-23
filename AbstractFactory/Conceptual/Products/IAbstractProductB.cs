namespace Conceptual.Products;

// Abstract interface for product family B
// All variants of product B must implement this interface
public interface IAbstractProductB
{
    // Core functionality specific to product B
    string UsefulFunctionB();
    
    // Collaboration method - product B can work with product A
    // This demonstrates how products within the same family can collaborate
    string AnotherUsefulFunctionB(IAbstractProductA collaborator);
}