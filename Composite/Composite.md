# Composite Pattern
## 1. Definition
The Composite Pattern composes objects into tree structures to represent part-whole hierarchies. It lets clients treat individual objects and compositions of objects uniformly through a common interface. This pattern is particularly useful when you need to work with tree-like structures where leaf nodes and composite nodes should be treated the same way.

### Manual Implementation
1. Define a Component interface/abstract class declaring common operations for both simple and complex objects
2. Create a Leaf class representing end objects (no children) that implements the Component interface
3. Create a Composite class that can contain children (both Leaf and other Composite objects)
4. The Composite implements the Component interface and delegates operations to its children
5. Client code works with all objects through the Component interface, treating leaves and composites uniformly

## 2. Pros
- **Uniform Treatment**: Client code treats individual objects and compositions uniformly
- **Simplified Client Code**: Clients don't need to distinguish between leaf and composite objects
- **Easy to Add New Components**: Can easily add new types of components without changing existing code
- **Natural Tree Representation**: Naturally represents hierarchical structures (file systems, UI components, organization charts)
- **Recursive Composition**: Supports unlimited nesting of composite objects
- **Open/Closed Principle**: Can introduce new component types without breaking existing code

## 3. Cons
- **Overly General Design**: Component interface must support all operations, even when not applicable to all types
- **Type Safety Issues**: Hard to restrict component types (e.g., allowing only specific children in certain composites)
- **Complex Leaf Classes**: Leaf classes may need to implement/throw exceptions for composite-only operations
- **Difficult Constraints**: Hard to enforce constraints on composite structure (e.g., max depth, specific ordering)
- **Performance Overhead**: Tree traversal can be expensive for deep or wide structures

## 4. Real-world Use Cases in C# & .NET
- **File System Structures**: Files and directories treated uniformly
```csharp
// File system example
public abstract class FileSystemItem
{
    public abstract long GetSize();
    public abstract void Display(int depth);
}

public class File : FileSystemItem
{
    private readonly string _name;
    private readonly long _size;

    public File(string name, long size)
    {
        _name = name;
        _size = size;
    }

    public override long GetSize() => _size;

    public override void Display(int depth)
    {
        Console.WriteLine(new string('-', depth) + _name + $" ({_size} bytes)");
    }
}

public class Directory : FileSystemItem
{
    private readonly string _name;
    private readonly List<FileSystemItem> _items = new();

    public Directory(string name)
    {
        _name = name;
    }

    public void Add(FileSystemItem item) => _items.Add(item);

    public override long GetSize()
    {
        // Recursively calculate total size
        return _items.Sum(item => item.GetSize());
    }

    public override void Display(int depth)
    {
        Console.WriteLine(new string('-', depth) + _name + "/");
        foreach (var item in _items)
        {
            item.Display(depth + 2);
        }
    }
}
```
- **UI Component Hierarchies**: WPF/WinForms controls (Panel contains Buttons, TextBoxes, other Panels)
- **Graphics Systems**: Drawing shapes where groups of shapes can be treated like individual shapes
- **Organization Structures**: Employee hierarchies (employees and departments)
- **Menu Systems**: Menus containing menu items and sub-menus
- **XML/JSON Document Object Model**: Nodes can be elements or composite elements containing other nodes
- **Expression Trees**: Mathematical expressions with operators and operands

## 5. Modern Approach: LINQ and Recursive Operations
```csharp
// Modern approach using LINQ for tree operations
public abstract class Component
{
    public string Name { get; set; } = string.Empty;
    public abstract IEnumerable<Component> GetChildren();
    public abstract int GetValue();
}

public class Leaf : Component
{
    private readonly int _value;

    public Leaf(string name, int value)
    {
        Name = name;
        _value = value;
    }

    public override IEnumerable<Component> GetChildren()
    {
        return Enumerable.Empty<Component>();
    }

    public override int GetValue() => _value;
}

public class CompositeNode : Component
{
    private readonly List<Component> _children = new();

    public CompositeNode(string name)
    {
        Name = name;
    }

    public void Add(Component component) => _children.Add(component);

    public override IEnumerable<Component> GetChildren() => _children;

    public override int GetValue()
    {
        // Modern LINQ approach for recursive calculation
        return _children.Sum(child => child.GetValue());
    }

    // LINQ-based tree traversal
    public IEnumerable<Component> Descendants()
    {
        return _children.Concat(_children.SelectMany(c => c.GetChildren()));
    }

    // Find all leaves in the tree
    public IEnumerable<Leaf> GetAllLeaves()
    {
        return Descendants().OfType<Leaf>();
    }
}

// Register in DI if needed
builder.Services.AddScoped<IComponentFactory, ComponentFactory>();

// Usage with LINQ
var root = new CompositeNode("root");
var allValues = root.Descendants().Sum(c => c.GetValue());
var leafCount = root.GetAllLeaves().Count();
```

**Benefits over traditional Composite:**
- LINQ enables powerful tree queries and transformations
- Cleaner recursive operations with functional style
- Easy filtering, mapping, and aggregation over tree structures
- Better integration with modern C# features

## 6. Expert Advice: When to Use vs When Not to Use

### ✅ When to Use Composite:
- **Tree Structures**: Representing hierarchical data (file systems, org charts, UI components)
- **Part-Whole Hierarchies**: Objects composed of smaller objects of the same type
- **Uniform Treatment**: Need to treat leaves and composites the same way
- **Recursive Algorithms**: Operations that should work recursively on tree structures
- **Nested Grouping**: Items that can be grouped, and groups can contain other groups
- **Menu/Navigation Systems**: Menus with items and sub-menus
- **Scene Graphs**: Graphics where groups of objects behave like single objects

### ❌ When NOT to Use Composite:
- **No Hierarchical Structure**: Data is flat or has simple relationships
- **Different Leaf/Composite Operations**: Leaves and composites have completely different operations
- **Type Safety Critical**: Need strict compile-time type checking for children
- **Simple Collections**: Standard collections (List, Dictionary) are sufficient
- **Performance Critical**: Tree traversal overhead is unacceptable
- **Single-Level Structures**: Only one level of nesting, no recursion needed

### 🎯 Expert Recommendations:
- **Design Component Interface Carefully**: Include only operations common to both leaves and composites
```csharp
// Good: Common operations
public abstract class Component
{
    public abstract void Render();
    public abstract double GetCost();
}

// Avoid: Forcing all components to implement composite-specific operations
public abstract class Component
{
    public abstract void Add(Component c); // Bad: Leaves shouldn't need this
}
```
- **Use Virtual Methods for Composite Operations**: Make Add/Remove virtual with default implementations
```csharp
public abstract class Component
{
    // Virtual with default implementation (throws exception)
    public virtual void Add(Component component)
    {
        throw new NotSupportedException("Cannot add to a leaf");
    }

    // Only Composite overrides this
}
```
- **Implement Child Management in Composite Only**: Keep leaf classes simple
- **Cache Recursive Results**: For expensive calculations, cache results in composites
```csharp
public class Composite : Component
{
    private int? _cachedTotal;

    public override int GetTotal()
    {
        if (_cachedTotal == null)
        {
            _cachedTotal = _children.Sum(c => c.GetTotal());
        }
        return _cachedTotal.Value;
    }

    public override void Add(Component c)
    {
        _children.Add(c);
        _cachedTotal = null; // Invalidate cache
    }
}
```
- **Use Visitor Pattern for Complex Operations**: When you need many different operations on the tree
- **Implement IEnumerable**: Allow iteration over composite children
```csharp
public class Composite : Component, IEnumerable<Component>
{
    private readonly List<Component> _children = new();

    public IEnumerator<Component> GetEnumerator() => _children.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
```
- **Consider Immutability**: For thread-safe trees, use immutable composites
- **Add Parent References**: For bidirectional tree navigation (but watch for memory leaks)
- **Validate Tree Structure**: Check for cycles, depth limits, or invalid children
- **Use LINQ for Tree Queries**: Leverage SelectMany, Where, Aggregate for tree operations
