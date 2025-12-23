namespace Conceptual.Builders;

// Builder interface defines the construction steps for creating product parts
// All concrete builders must implement these methods, allowing different builders
// to create different representations of the product using the same construction process
//
// Key Insight: The interface specifies WHAT to build, not HOW to build it
// Each concrete builder decides the specific implementation details
public interface IBuilder
{
    // Builds part A of the product
    // Different concrete builders can create different versions of Part A
    void BuildPartA();

    // Builds part B of the product
    // The order of calling these methods can be controlled by the Director
    void BuildPartB();

    // Builds part C of the product
    // Not all parts need to be built - client can choose which parts to include
    void BuildPartC();
}
