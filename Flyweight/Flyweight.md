# Flyweight Pattern
## 1. Definition
The Flyweight Pattern minimizes memory usage by sharing as much data as possible with similar objects. It separates intrinsic state (shared, immutable) from extrinsic state (unique, context-dependent), storing the intrinsic state in flyweight objects that can be shared across multiple contexts. This pattern is particularly effective when dealing with large numbers of similar objects.

### Manual Implementation
1. Identify intrinsic state (shared across objects) and extrinsic state (unique to each object)
2. Create a Flyweight class that stores only the intrinsic state
3. Create a FlyweightFactory that manages a pool of flyweight objects
4. Factory returns existing flyweight if shared state matches, or creates new one if needed
5. Client code stores extrinsic state and passes it to flyweight methods when needed
6. Use a key/hash to identify flyweights with identical intrinsic state

## 2. Pros
- **Memory Optimization**: Dramatically reduces memory usage by sharing common state
- **Performance**: Fewer objects mean less garbage collection overhead
- **Scalability**: Can handle large numbers of objects that would otherwise exhaust memory
- **Centralized Management**: FlyweightFactory manages object creation and sharing
- **Immutability**: Shared state is typically immutable, making flyweights thread-safe

## 3. Cons
- **Complexity**: Separating intrinsic/extrinsic state adds complexity
- **CPU vs Memory Trade-off**: May increase CPU usage (passing extrinsic state, lookups)
- **Factory Overhead**: Factory must maintain cache and perform lookups
- **Limited Applicability**: Only beneficial when many objects share common state
- **Extrinsic State Management**: Client must manage and pass extrinsic state
- **Debugging Difficulty**: Shared objects make debugging state issues harder

## 4. Real-world Use Cases in C# & .NET
- **String Interning**: .NET interns string literals to share identical strings
```csharp
// String interning - classic flyweight in .NET
string s1 = "hello";
string s2 = "hello";
Console.WriteLine(object.ReferenceEquals(s1, s2)); // True - same object!

// Manual interning
string s3 = new string("world".ToCharArray());
string s4 = string.Intern(s3);
string s5 = string.Intern("world");
Console.WriteLine(object.ReferenceEquals(s4, s5)); // True
```
- **Character Rendering**: Text editors share glyph objects for characters
```csharp
// Text editor example
public class Glyph // Flyweight
{
    private readonly char _character;
    private readonly string _fontFamily;
    private readonly int _fontSize;

    // Intrinsic state: character appearance
    public Glyph(char character, string fontFamily, int fontSize)
    {
        _character = character;
        _fontFamily = fontFamily;
        _fontSize = fontSize;
    }

    // Extrinsic state: position passed in
    public void Render(int x, int y, string color)
    {
        Console.WriteLine($"Rendering '{_character}' at ({x},{y}) in {color}");
    }
}

// Thousands of 'a' characters share the same Glyph object
```
- **Game Objects**: Sharing textures, models, sounds across multiple game entities
- **Database Connection Pooling**: Reusing connection objects
- **Caching Systems**: Object pools, sprite pools in games
- **UI Components**: Sharing icon/image objects across multiple controls

## 5. Modern Approach: Object Pooling with Memory<T>
```csharp
// Modern object pooling approach
using System.Buffers;

// Use ArrayPool for efficient memory reuse
public class DataProcessor
{
    public void ProcessData(int size)
    {
        // Rent array from pool instead of allocating new
        var buffer = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            // Use buffer
            ProcessBuffer(buffer, size);
        }
        finally
        {
            // Return to pool for reuse
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}

// Flyweight with modern C# features
public class ParticleFactory
{
    private readonly Dictionary<ParticleKey, Particle> _particles = new();

    public Particle GetParticle(string texture, string effect)
    {
        var key = new ParticleKey(texture, effect);

        if (!_particles.TryGetValue(key, out var particle))
        {
            particle = new Particle(texture, effect);
            _particles[key] = particle;
        }

        return particle;
    }

    // Use record for key - automatic value equality
    private record ParticleKey(string Texture, string Effect);
}

// Flyweight as immutable record
public record Particle(string Texture, string Effect)
{
    // Extrinsic state passed to methods
    public void Render(int x, int y, int velocity)
    {
        Console.WriteLine($"Rendering {Texture} at ({x},{y}) velocity {velocity}");
    }
}

// Register factory in DI
builder.Services.AddSingleton<ParticleFactory>();
```

**Benefits over traditional Flyweight:**
- ArrayPool provides efficient memory reuse
- Records provide automatic equality comparison for keys
- Immutable records are thread-safe by default
- Dictionary<TKey, TValue> provides efficient lookups
- Span<T> and Memory<T> for zero-allocation scenarios

## 6. Expert Advice: When to Use vs When Not to Use

### ✅ When to Use Flyweight:
- **Large Number of Similar Objects**: Thousands or millions of objects with shared state
- **Memory Constraints**: Application runs out of memory with naive approach
- **Immutable Shared State**: Intrinsic state doesn't change after creation
- **Clear Intrinsic/Extrinsic Split**: Easy to separate shared vs unique state
- **Performance Critical**: Memory allocation/GC is a bottleneck
- **Text/Character Rendering**: Editors, terminals, document processors
- **Game Development**: Particles, bullets, trees, buildings sharing textures/models
- **Data Visualization**: Large datasets with repeated visual elements

### ❌ When NOT to Use Flyweight:
- **Few Objects**: Small number of objects (< 1000) won't benefit significantly
- **No Shared State**: Objects have mostly unique data
- **Mutable State**: Shared state needs to change frequently
- **Simple Application**: Memory usage is not a concern
- **Complex Extrinsic State**: Managing extrinsic state is too complex
- **Premature Optimization**: Profile first before adding complexity

### 🎯 Expert Recommendations:
- **Profile First**: Measure memory usage before applying pattern
```csharp
// Use a profiler to identify memory issues
// Don't apply Flyweight prematurely
```
- **Make Intrinsic State Immutable**: Prevents bugs from shared state modification
```csharp
// Good: Immutable flyweight
public class TreeType
{
    public TreeType(string name, string texture, string color)
    {
        Name = name;
        Texture = texture;
        Color = color;
    }

    public string Name { get; } // Read-only
    public string Texture { get; }
    public string Color { get; }
}

// Avoid: Mutable flyweight (shared state can be corrupted)
public class TreeType
{
    public string Name { get; set; } // Bad - can be changed!
}
```
- **Use Dictionary for Factory Cache**: Efficient O(1) lookups
```csharp
private readonly Dictionary<string, Flyweight> _cache = new();

public Flyweight GetFlyweight(string key)
{
    if (!_cache.TryGetValue(key, out var flyweight))
    {
        flyweight = new Flyweight(key);
        _cache[key] = flyweight;
    }
    return flyweight;
}
```
- **Consider Weak References**: Allow GC to collect rarely-used flyweights
```csharp
private readonly Dictionary<string, WeakReference<Flyweight>> _cache = new();

public Flyweight GetFlyweight(string key)
{
    if (_cache.TryGetValue(key, out var weakRef) &&
        weakRef.TryGetTarget(out var flyweight))
    {
        return flyweight; // Reuse existing
    }

    flyweight = new Flyweight(key);
    _cache[key] = new WeakReference<Flyweight>(flyweight);
    return flyweight;
}
```
- **Use ValueTuple or Record for Keys**: Efficient and clean key generation
```csharp
// Modern approach with records
private record FlyweightKey(string Type, string Subtype, string Color);

private readonly Dictionary<FlyweightKey, Flyweight> _cache = new();

public Flyweight Get(string type, string subtype, string color)
{
    var key = new FlyweightKey(type, subtype, color);
    // Dictionary uses record's automatic value equality
    if (!_cache.TryGetValue(key, out var fw))
    {
        fw = new Flyweight(type, subtype, color);
        _cache[key] = fw;
    }
    return fw;
}
```
- **Document Intrinsic vs Extrinsic**: Clearly indicate which properties are which
```csharp
public class Flyweight
{
    // Intrinsic state (shared)
    public string Type { get; }
    public string Texture { get; }

    // Extrinsic state should NOT be stored here
    // Pass it as method parameters instead
    public void Render(int x, int y, int size) // Extrinsic: position, size
    {
        // Render using intrinsic state + extrinsic parameters
    }
}
```
- **Consider ArrayPool**: For temporary buffers in performance-critical code
- **Thread Safety**: Flyweights are typically shared across threads - ensure immutability
- **Lazy Initialization**: Create flyweights on-demand rather than pre-populating
- **Cache Eviction**: Consider LRU cache if flyweight pool grows too large
- **Measure Impact**: Verify memory savings justify the complexity
```csharp
// Before: 1,000,000 objects × 100 bytes = 100 MB
// After: 100 flyweights × 100 bytes + 1,000,000 refs × 8 bytes = 18 MB
// Savings: 82 MB (82% reduction)
```
