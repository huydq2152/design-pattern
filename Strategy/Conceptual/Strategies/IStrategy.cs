namespace Conceptual.Strategies;

// Strategy interface defines the contract that all concrete strategies must follow
// This allows the context to work with any strategy without knowing specific implementations
public interface IStrategy
{
    // Core algorithm method that all concrete strategies must implement
    // Takes input data and returns processed result based on the specific algorithm
    object DoAlgorithm(object data);
}