using Conceptual.Components;

namespace Conceptual.Composite;

// Composite represents complex components that can have children
// It can contain both Leaf objects and other Composite objects
// This creates the tree structure where branches can contain other branches and leaves
//
// Characteristics:
// - Stores child components in a collection
// - Implements Add/Remove to manage children
// - Delegates Operation() to children (recursive behavior)
// - Can treat children uniformly regardless of whether they're leaves or composites
//
// Real-world examples:
// - Directory in a file system (contains files and subdirectories)
// - Panel in a UI (contains buttons, text boxes, other panels)
// - Department in an organization (contains employees and sub-departments)
// - Menu in a menu system (contains menu items and sub-menus)
public class Composite : Component
{
    // Collection of child components
    // Children can be Leaf objects, other Composite objects, or a mix
    // This is what enables the tree structure and recursive composition
    private readonly List<Component> _children = new();

    // Implements the operation by delegating to all children
    // This is the recursive behavior of the Composite pattern:
    // - Composite delegates to children
    // - Children might be other Composites that further delegate
    // - Eventually reaches Leaf nodes that perform actual work
    public override string Operation()
    {
        int i = 0;
        string result = "Branch(";

        // Iterate through all children and call their Operation() method
        // This demonstrates uniform treatment: we don't care if children are leaves or composites
        foreach (Component component in _children)
        {
            result += component.Operation();
            if (i != _children.Count - 1)
            {
                result += "+";
            }
            i++;
        }

        return result + ")";
    }

    // Override to add child components to the collection
    // Enables building the tree structure dynamically
    // Can add both Leaf and Composite objects (uniform treatment)
    public override void Add(Component component)
    {
        _children.Add(component);
    }

    // Override to remove child components from the collection
    // Allows modifying the tree structure at runtime
    public override void Remove(Component component)
    {
        _children.Remove(component);
    }

    // Composite returns true for IsComposite()
    // Uses default implementation from Component base class
    // This allows client code to safely add children without exceptions
}
