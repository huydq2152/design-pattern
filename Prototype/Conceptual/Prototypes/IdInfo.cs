namespace Conceptual.Prototypes;

// Supporting class to demonstrate deep copy behavior
// Represents a reference type property that needs special handling during cloning
public class IdInfo
{
    public int IdNumber;

    public IdInfo(int idNumber)
    {
        IdNumber = idNumber;
    }
}
