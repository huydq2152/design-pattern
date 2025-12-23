namespace Conceptual.Creators;

public abstract class Creator
{
    // This method must be override in concrete delivery method classes (Fast/Regular/Cheap)
    // We can also provide a default implementation of the factory method that returns a default product. 
    public abstract IProduct FactoryMethod();

    // This method contains core logics.
    // It will perform the action based on the product returned by the factory method (FactoryMethod).
    public string SomeOperation()
    {
        var product = FactoryMethod();

        var result = $"Creator: The same creator's code has just worked with {product}";

        return result;
    }
}