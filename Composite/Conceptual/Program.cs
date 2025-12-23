using Conceptual.Components;
using Conceptual.Composite;
using Conceptual.Leaf;

// Client code demonstrating the Composite pattern
// Shows how to build tree structures and treat leaves and composites uniformly
public static class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Composite Pattern Demonstration ===\n");

        // Example 1: Working with a simple Leaf component
        // Demonstrates that leaves can be used standalone
        Console.WriteLine("--- Example 1: Simple Leaf Component ---");
        var simpleLeaf = new Leaf();
        Console.WriteLine("Client: Working with a simple leaf component:");
        ClientCode(simpleLeaf);

        // Example 2: Building a complex tree structure
        // Demonstrates the recursive composition capability
        Console.WriteLine("\n--- Example 2: Complex Tree Structure ---");

        // Create the root composite (tree)
        var tree = new Composite();

        // Create first branch with two leaves
        var branch1 = new Composite();
        branch1.Add(new Leaf());
        branch1.Add(new Leaf());

        // Create second branch with one leaf
        var branch2 = new Composite();
        branch2.Add(new Leaf());

        // Add branches to the root tree
        // This creates a tree structure: tree -> branch1 (leaf, leaf), branch2 (leaf)
        tree.Add(branch1);
        tree.Add(branch2);

        Console.WriteLine("Client: Working with a composite tree:");
        ClientCode(tree);

        // Example 3: Dynamic tree manipulation
        // Demonstrates that clients can add components without knowing their types
        Console.WriteLine("\n--- Example 3: Dynamic Tree Manipulation ---");
        Console.WriteLine("Client: Adding a leaf to the tree dynamically:");

        // The power of Composite pattern: we can add a leaf directly to the tree
        // Client doesn't need to check if 'tree' is a Composite - uniform interface
        if (tree.IsComposite())
        {
            tree.Add(simpleLeaf);
        }

        Console.WriteLine("Updated tree structure:");
        ClientCode(tree);

        // Example 4: Nested composition
        // Demonstrates unlimited nesting capability
        Console.WriteLine("\n--- Example 4: Deeply Nested Structure ---");

        var deepTree = new Composite();
        var level1 = new Composite();
        var level2 = new Composite();
        var level3 = new Composite();

        // Create a deep hierarchy: deepTree -> level1 -> level2 -> level3 -> leaves
        level3.Add(new Leaf());
        level3.Add(new Leaf());
        level2.Add(level3);
        level2.Add(new Leaf());
        level1.Add(level2);
        level1.Add(new Leaf());
        deepTree.Add(level1);

        Console.WriteLine("Client: Working with deeply nested structure:");
        ClientCode(deepTree);
    }

    // Client method that works uniformly with all components
    // This is the key benefit of Composite pattern: uniform treatment
    //
    // The client doesn't need to know if it's working with:
    // - A simple Leaf
    // - A Composite with a few children
    // - A deeply nested tree structure
    //
    // All are treated the same through the Component interface
    private static void ClientCode(Component component)
    {
        // Simply call Operation() - the component handles the complexity
        // If it's a Leaf: returns its own result
        // If it's a Composite: recursively delegates to children
        Console.WriteLine($"RESULT: {component.Operation()}");
    }
}

/*
 * KEY TAKEAWAYS:
 *
 * COMPOSITE PATTERN COMPONENTS:
 * 1. Component: Base class defining common interface for leaves and composites
 * 2. Leaf: End objects with no children, perform actual work
 * 3. Composite: Container objects that can have children (leaves or other composites)
 * 4. Client: Works with all objects uniformly through Component interface
 *
 * TREE STRUCTURE BUILDING:
 * - Leaves are the "building blocks" (primitive objects)
 * - Composites are "containers" that group components together
 * - Composites can contain other composites (recursive composition)
 * - No limit to tree depth or breadth
 *
 * UNIFORM TREATMENT:
 * - Client code treats leaves and composites the same way
 * - Client calls Operation() without knowing object type
 * - Component interface provides transparency
 * - No type checking needed (in most cases)
 *
 * WHEN TO USE COMPOSITE PATTERN:
 * - Part-whole hierarchies (tree structures)
 * - Need to treat individual objects and groups uniformly
 * - Recursive composition (containers within containers)
 * - File systems, UI component hierarchies, organization charts, menus
 *
 * TRANSPARENCY vs SAFETY TRADE-OFF:
 * - Transparency (current approach): All components have Add/Remove methods
 *   Benefit: Uniform interface, no type checking needed
 *   Drawback: Leaves throw exceptions for Add/Remove
 * - Safety (alternative): Only Composite has Add/Remove methods
 *   Benefit: Type-safe, no runtime exceptions
 *   Drawback: Need to check types and cast
 *
 * RECURSIVE BEHAVIOR:
 * - Operation() on Composite delegates to all children
 * - Each child's Operation() is called (might be Leaf or another Composite)
 * - Eventually reaches Leaf nodes that do actual work
 * - Results are combined and returned up the tree
 *
 * REAL-WORLD EXAMPLES:
 * - File System: Directory (Composite) contains Files (Leaf) and Subdirectories (Composite)
 * - UI Framework: Panel (Composite) contains Buttons (Leaf), TextBoxes (Leaf), nested Panels (Composite)
 * - Organization: Department (Composite) contains Employees (Leaf) and Sub-departments (Composite)
 * - Graphics: Group (Composite) contains Shapes (Leaf) and nested Groups (Composite)
 * - Menu System: Menu (Composite) contains MenuItems (Leaf) and SubMenus (Composite)
 *
 * BENEFITS:
 * - Simplified client code (no need to distinguish types)
 * - Easy to add new component types
 * - Naturally represents hierarchical structures
 * - Supports unlimited nesting through recursion
 */
