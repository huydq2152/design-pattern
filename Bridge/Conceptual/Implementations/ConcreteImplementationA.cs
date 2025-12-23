namespace Conceptual.Implementations;

// Concrete Implementation A provides platform-specific or variant-specific behavior
// This represents one specific implementation (e.g., Windows platform, SQL Server database, Email channel)
//
// Multiple concrete implementations can exist, each providing different behavior
// for the same interface operations. The abstraction can work with any of them.
public class ConcreteImplementationA : IImplementation
{
    // Implements the low-level operation for Platform/Variant A
    // This could be Windows-specific rendering, SQL Server queries, email sending, etc.
    public string OperationImplementation()
    {
        return "ConcreteImplementationA: Platform A specific result";
    }
}
