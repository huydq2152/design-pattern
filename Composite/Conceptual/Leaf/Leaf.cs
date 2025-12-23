using Conceptual.Components;

namespace Conceptual.Leaf;

// Leaf represents the end objects in a composition - objects with no children
// Leaves are the "building blocks" of the tree structure
//
// Characteristics:
// - Cannot have children (Add/Remove inherited but throw exceptions)
// - Implements only the core operation (Operation())
// - Represents primitive objects like files, simple UI controls, individual employees, etc.
//
// Real-world examples:
// - File in a file system (vs Directory which is Composite)
// - Button in a UI (vs Panel which is Composite)
// - Individual employee (vs Department which is Composite)
// - Menu item (vs Menu which is Composite)
public class Leaf : Component
{
    // Implements the core operation for a leaf node
    // Leaf nodes perform actual work without delegating to children
    // In real scenarios, this might render a UI element, calculate a value,
    // display information, etc.
    public override string Operation()
    {
        return "Leaf";
    }

    // Override to indicate this component cannot have children
    // This allows client code to check before attempting to add children
    // and avoid the NotSupportedException from Add/Remove methods
    public override bool IsComposite()
    {
        return false;
    }

    // Note: Add() and Remove() are inherited from Component
    // They throw NotSupportedException, which is appropriate for leaves
    // This is the transparency vs. safety trade-off in Composite pattern:
    // - Transparency: All components have same interface (current approach)
    // - Safety: Only Composite has Add/Remove (requires type checking)
}
