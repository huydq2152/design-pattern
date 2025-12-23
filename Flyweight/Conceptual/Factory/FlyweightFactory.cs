using Conceptual.Flyweights;
using Conceptual.Models;

namespace Conceptual.Factory;

// FlyweightFactory manages the pool of flyweight objects
// It ensures that flyweights with the same intrinsic state are shared
//
// Key responsibilities:
// - Maintain a cache/pool of existing flyweights
// - Return existing flyweight if intrinsic state matches
// - Create new flyweight only if no match exists
// - Generate unique keys to identify flyweights
//
// This centralized management is crucial for the pattern to work
public class FlyweightFactory
{
    // Cache of flyweights with their identifying keys
    // Tuple<Flyweight, string>: (flyweight object, unique key)
    // The key identifies the intrinsic state (Company + Model + Color)
    private readonly List<Tuple<Flyweight, string>> _flyweights = new();

    // Constructor can pre-populate the cache with common flyweights
    // This is an optimization: frequently-used car types are pre-created
    public FlyweightFactory(params Car[] initialCars)
    {
        foreach (var car in initialCars)
        {
            _flyweights.Add(new Tuple<Flyweight, string>(
                new Flyweight(car),
                GetKey(car)
            ));
        }
    }

    // Generates a unique key for a car's intrinsic state
    // The key represents the combination of Company, Model, and Color
    //
    // Important: Key generation must be consistent
    // - Same intrinsic state always produces same key
    // - Different intrinsic state produces different keys
    //
    // Implementation: Sorts properties and joins with underscore
    // This ensures consistent ordering regardless of input order
    public string GetKey(Car car)
    {
        // Note: Only includes intrinsic state (Company, Model, Color)
        // Extrinsic state (Owner, Number) is NOT part of the key
        List<string> elements = new()
        {
            car.Model,
            car.Color,
            car.Company
        };

        // Sort to ensure consistent key generation
        // "BMW_M5_Red" same as "M5_BMW_Red" after sorting
        elements.Sort();

        return string.Join("_", elements);
    }

    // Returns an existing flyweight or creates a new one
    // This is the core of the Flyweight pattern: sharing objects
    //
    // Process:
    // 1. Generate key from shared state
    // 2. Check if flyweight with this key exists in cache
    // 3. If exists: return existing flyweight (memory saved!)
    // 4. If not: create new flyweight and add to cache
    public Flyweight GetFlyweight(Car sharedState)
    {
        var key = GetKey(sharedState);

        // Check if flyweight exists in cache
        var existingFlyweight = _flyweights.FirstOrDefault(t => t.Item2 == key);

        if (existingFlyweight == null)
        {
            // No match found: create new flyweight
            Console.WriteLine("FlyweightFactory: Can't find a flyweight, creating new one.");
            var newFlyweight = new Flyweight(sharedState);
            _flyweights.Add(new Tuple<Flyweight, string>(newFlyweight, key));
            return newFlyweight;
        }
        else
        {
            // Match found: reuse existing flyweight
            Console.WriteLine("FlyweightFactory: Reusing existing flyweight.");
            return existingFlyweight.Item1;
        }
    }

    // Utility method to display all cached flyweights
    // Useful for debugging and understanding memory savings
    public void ListFlyweights()
    {
        var count = _flyweights.Count;
        Console.WriteLine($"\nFlyweightFactory: I have {count} flyweights:");
        foreach (var flyweight in _flyweights)
        {
            Console.WriteLine($"  - {flyweight.Item2}");
        }
    }
}
