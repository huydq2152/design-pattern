namespace Conceptual.Models;

// Car represents the complete state of a car object
// The Flyweight pattern will split this into:
// - Intrinsic state (shared): Company, Model, Color
// - Extrinsic state (unique): Owner, Number
//
// This separation allows sharing common car types (e.g., "BMW M5 Red")
// across multiple owners, dramatically reducing memory usage
public class Car
{
    // Extrinsic state: Unique to each car instance
    // These properties differ for each car and cannot be shared
    public string Owner { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;

    // Intrinsic state: Shared across similar cars
    // These properties are the same for cars of the same type
    // Example: All "BMW M5 Red" cars share these values
    public string Company { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}
