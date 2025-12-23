# Prototype Pattern
## 1. Definition
The Prototype Pattern specifies the kinds of objects to create using a prototypical instance and creates new objects by copying this prototype. Instead of creating objects from scratch using constructors, the pattern clones existing objects, making it useful when object creation is costly or complex. This pattern supports both shallow copying (copying references) and deep copying (copying entire object graphs).

### Manual Implementation
1. Define a Prototype interface or abstract class with a Clone() method
2. Create Concrete Prototype classes that implement the Clone() method
3. Implement shallow copy using MemberwiseClone() for simple objects
4. Implement deep copy by manually cloning reference type properties for complex object graphs
5. Client code clones prototypes instead of creating new instances from scratch

## 2. Pros
- **Performance Optimization**: Faster object creation when instantiation is expensive (database queries, complex calculations)
- **Reduced Subclassing**: Avoids creating multiple factory classes for similar object variations
- **Runtime Configuration**: Can add/remove prototypes at runtime, providing dynamic object creation
- **Complex Object Cloning**: Simplifies copying complex object graphs without knowing their structure
- **Independent Copies**: Cloned objects are independent instances that can be modified separately
- **Hide Construction Complexity**: Encapsulates complex initialization logic within the prototype

## 3. Cons
- **Deep Copy Complexity**: Implementing deep copy for complex object graphs with circular references is challenging
- **Clone Method Maintenance**: Every new class must implement cloning, increasing maintenance burden
- **Shared State Issues**: Shallow copies share reference type properties, leading to unintended side effects
- **Difficult for Classes with Private Fields**: Cloning classes with many private fields or complex state requires careful implementation
- **Serialization Alternative**: Sometimes serialization/deserialization is more reliable than manual cloning

## 4. Real-world Use Cases in C# & .NET
- **Object Caching and Pooling**: Pre-create and cache expensive objects for quick retrieval
```csharp
// Cloning a cached database connection configuration
var defaultConfig = _cache.Get<DatabaseConfig>("default");
var customConfig = defaultConfig.Clone();
customConfig.ConnectionString = "custom-connection-string";
```
- **Game Development**: Cloning game entities (enemies, items, terrain) with preset configurations
- **Document Processing**: Creating document templates and cloning them with different content
- **Configuration Management**: Cloning base configurations and modifying specific properties
- **Undo/Redo Functionality**: Saving object state snapshots for rollback operations
- **Graphics Editors**: Cloning shapes, layers, or graphical elements

## 5. Modern Approach: Records and with Expressions
```csharp
// Use C# records for immutable prototypes with built-in copy semantics
public record PersonRecord(string Name, int Age, AddressRecord Address);
public record AddressRecord(string Street, string City);

// Clone with modifications using 'with' expression
var original = new PersonRecord("John", 30, new AddressRecord("123 Main St", "NYC"));
var clone = original with { Name = "Jane" }; // Non-destructive mutation
var deepClone = original with { Address = original.Address with { City = "LA" } };

// For complex cloning scenarios with dependency injection
public interface IPrototypeFactory<T> where T : ICloneable
{
    T CreateClone(T prototype);
}

// Register in Program.cs
builder.Services.AddSingleton<IPrototypeFactory<DocumentTemplate>, DocumentTemplateFactory>();
```

**Benefits over traditional Prototype:**
- Records provide built-in copy semantics with `with` expressions
- Immutable by default, preventing unintended state changes
- Value-based equality comparison
- Automatic deep copy for value types and shallow copy for reference types
- Integration with modern C# features (pattern matching, deconstruction)

## 6. Expert Advice: When to Use vs When Not to Use

### ✅ When to Use Prototype:
- **Expensive Object Creation**: Database entities, file system objects, or network resources
- **Complex Initialization**: Objects requiring multi-step setup, dependency resolution, or configuration
- **Runtime Object Composition**: When object structure is determined dynamically
- **Pre-configured Templates**: Document templates, email templates, configuration presets
- **Object Variations**: Many variations of similar objects differing in few properties
- **Avoiding Subclass Explosion**: When Factory Method would require too many subclasses
- **Snapshot/Memento Scenarios**: Saving object state for undo/redo functionality

### ❌ When NOT to Use Prototype:
- **Simple Object Creation**: When `new ClassName()` is sufficient and fast
- **Immutable Objects**: Use shared instances instead of cloning
- **Classes with Complex Dependencies**: Deep cloning dependencies is error-prone
- **Circular References**: Extremely difficult to handle in deep copy scenarios
- **Objects with Unique Constraints**: Database entities with unique IDs shouldn't be cloned
- **Tightly Coupled Systems**: When cloning breaks object relationships or invariants

### 🎯 Expert Recommendations:
- **Prefer Records for DTOs**: Use C# records with `with` expressions for simple cloning
- **Implement ICloneable Carefully**: Be explicit about shallow vs deep copy behavior
- **Consider Copy Constructors**: Often clearer than ICloneable for complex objects
- **Use Serialization for Deep Copies**: JSON serialization/deserialization for complex graphs
```csharp
// Deep copy using JSON serialization
public T DeepClone<T>(T obj)
{
    var json = JsonSerializer.Serialize(obj);
    return JsonSerializer.Deserialize<T>(json);
}
```
- **Document Clone Behavior**: Clearly specify whether Clone() performs shallow or deep copy
- **Handle Circular References**: Use visited object tracking for deep cloning graphs
- **Test Clone Independence**: Verify that cloned objects don't share mutable state
- **Consider AutoMapper**: For complex object-to-object copying scenarios
