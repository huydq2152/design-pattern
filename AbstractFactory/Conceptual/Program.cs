using Conceptual.Factories;
using Conceptual.Products;

// Client class demonstrates how to use the Abstract Factory pattern
// The client works with factories through the abstract interface,
// making it independent of specific product families
class Client
{
    public void Main()
    {
        // Test with first factory (Family 1 products)
        Console.WriteLine("Client: Testing client code with the first factory type...");
        ClientMethod(new ConcreteFactory1());
        Console.WriteLine();

        // Test with second factory (Family 2 products)  
        Console.WriteLine("Client: Testing the same client code with the second factory type...");
        ClientMethod(new ConcreteFactory2());
    }

    // This method works with any factory that implements IAbstractFactory
    // It doesn't know which concrete products are being created
    private void ClientMethod(IAbstractFactory factory)
    {
        var productA = factory.CreateProductA();
        var productB = factory.CreateProductB();

        // Demonstrate product functionality
        Console.WriteLine(productB.UsefulFunctionB());
        
        // Demonstrate product collaboration within the same family
        Console.WriteLine(productB.AnotherUsefulFunctionB(productA));
    }
}

internal static class Program
{
    public static void Main(string[] args)
    {
        new Client().Main();
    }
}