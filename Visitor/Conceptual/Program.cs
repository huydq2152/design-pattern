// The Visitor pattern lets you add new operations to existing object structures without
// modifying those structures. It achieves this by separating algorithms from the objects
// they operate on, promoting the Open/Closed Principle and clean separation of concerns.

/// <summary>
/// IComponent interface declares an Accept method that takes a visitor.
///
/// KEY RESPONSIBILITIES:
/// - Defines the Accept method that all concrete components must implement
/// - Provides entry point for visitor to access the component
/// - Enables double dispatch mechanism (visitor calls component, component calls visitor)
///
/// DESIGN DECISIONS:
/// - Accept method takes IVisitor parameter (all components accept all visitors)
/// - Component passes itself (this) to visitor's Visit method
/// - Interface ensures all components can be visited
/// - Visitors can work with any component implementing this interface
///
/// CHARACTERISTICS:
/// - Simple interface with single Accept method
/// - Component doesn't know what visitor will do
/// - Enables adding new operations without modifying components
/// - Foundation of the double dispatch pattern
///
/// REAL-WORLD EXAMPLES:
/// - AST nodes in compiler (Expression, Statement, Declaration)
/// - Document elements (Paragraph, Image, Table)
/// - File system items (File, Directory, Link)
/// - Shopping cart items (Product, Service, Subscription)
/// </summary>
public interface IComponent
{
    /// <summary>
    /// Accepts a visitor that will perform operations on this component.
    ///
    /// DOUBLE DISPATCH:
    /// - First dispatch: Client calls component.Accept(visitor)
    /// - Second dispatch: Component calls visitor.VisitXXX(this)
    /// - Resolves both component type and visitor type at runtime
    ///
    /// PATTERN:
    /// - Component accepts visitor
    /// - Component calls appropriate visitor method based on its own type
    /// - Visitor receives concrete component reference
    /// - Visitor can access component's specific methods
    ///
    /// EXAMPLE FLOW:
    /// - component.Accept(visitor) is called
    /// - Inside Accept: visitor.VisitConcreteComponentA(this)
    /// - Visitor knows exact component type and can operate on it
    ///
    /// BENEFITS:
    /// - New operations added by creating new visitors
    /// - Components remain unchanged when adding operations
    /// - Visitor gets strongly-typed component reference
    /// </summary>
    void Accept(IVisitor visitor);
}

/// <summary>
/// ConcreteComponentA is one type of component that can be visited.
///
/// KEY RESPONSIBILITIES:
/// - Implements Accept method to call appropriate visitor method
/// - Provides component-specific methods and data
/// - Allows visitors to access its unique features
///
/// DESIGN APPROACH:
/// - Accept method calls visitor.VisitConcreteComponentA(this)
/// - Provides ExclusiveMethodOfConcreteComponentA for A-specific operations
/// - Demonstrates component-specific functionality
/// - Enables type-safe visitor operations
///
/// CHARACTERISTICS:
/// - Has unique methods not shared with other components
/// - Accept implementation uses double dispatch
/// - Visitor gets ConcreteComponentA reference (not just IComponent)
/// - Can expose any public methods to visitors
///
/// REAL-WORLD EXAMPLES:
/// - NumberExpression in compiler (HasValue, GetValue)
/// - ImageElement in document (GetDimensions, GetFormat)
/// - RegularFile in file system (GetSize, GetChecksum)
/// - PhysicalProduct in shopping cart (GetWeight, GetDimensions)
/// </summary>
public class ConcreteComponentA : IComponent
{
    /// <summary>
    /// Accepts a visitor and dispatches to the component-specific visit method.
    ///
    /// DOUBLE DISPATCH IMPLEMENTATION:
    /// - Receives generic IVisitor parameter
    /// - Calls visitor.VisitConcreteComponentA(this)
    /// - Passes itself as ConcreteComponentA type (not IComponent)
    /// - Visitor receives strongly-typed component
    ///
    /// PATTERN:
    /// - Every concrete component implements Accept
    /// - Each calls its own specific visitor method
    /// - Enables visitor to handle different component types differently
    ///
    /// TYPE RESOLUTION:
    /// - At runtime, this is ConcreteComponentA
    /// - visitor.VisitConcreteComponentA is called (not a generic visit)
    /// - Visitor method receives ConcreteComponentA (can call ExclusiveMethodOfConcreteComponentA)
    ///
    /// BENEFITS:
    /// - No casting needed in visitor
    /// - Type-safe access to component-specific methods
    /// - Compiler ensures visitor handles this component type
    /// </summary>
    public void Accept(IVisitor visitor)
    {
        visitor.VisitConcreteComponentA(this);
    }

    /// <summary>
    /// Component-specific method exclusive to ConcreteComponentA.
    ///
    /// COMPONENT-SPECIFIC OPERATIONS:
    /// - Unique to ConcreteComponentA (not in ComponentB)
    /// - Visitors can call this method when visiting ComponentA
    /// - Demonstrates why visitors need typed component references
    /// - Returns component-specific data
    ///
    /// PURPOSE:
    /// - Provides access to ComponentA's unique features
    /// - Shows why double dispatch is necessary (visitor needs typed reference)
    /// - Enables visitor to perform A-specific operations
    ///
    /// USAGE IN VISITOR:
    /// - public void VisitConcreteComponentA(ConcreteComponentA component)
    /// - {
    /// -     var data = component.ExclusiveMethodOfConcreteComponentA();
    /// -     // Process data specific to ComponentA
    /// - }
    /// </summary>
    public string ExclusiveMethodOfConcreteComponentA()
    {
        return "A";
    }
}

/// <summary>
/// ConcreteComponentB is another type of component with different features.
///
/// KEY RESPONSIBILITIES:
/// - Implements Accept method to call appropriate visitor method
/// - Provides different component-specific methods than ComponentA
/// - Demonstrates polymorphism in visitor pattern
///
/// DESIGN APPROACH:
/// - Accept calls visitor.VisitConcreteComponentB(this)
/// - Provides SpecialMethodOfConcreteComponentB (different from A's method)
/// - Shows how visitors can handle different component types
/// - Each component type has its own visitor method
///
/// CHARACTERISTICS:
/// - Different interface than ConcreteComponentA
/// - Same Accept pattern but different visitor method
/// - Enables visitors to treat A and B differently
/// - Demonstrates heterogeneous object structures
///
/// REAL-WORLD EXAMPLES:
/// - StringExpression in compiler (GetValue, GetLength)
/// - TableElement in document (GetRows, GetColumns)
/// - Directory in file system (GetFiles, GetSubdirectories)
/// - DigitalProduct in shopping cart (GetDownloadLink, GetLicenseKey)
/// </summary>
public class ConcreteComponentB : IComponent
{
    /// <summary>
    /// Accepts a visitor and dispatches to the ComponentB-specific visit method.
    ///
    /// DOUBLE DISPATCH FOR COMPONENTB:
    /// - Calls visitor.VisitConcreteComponentB(this)
    /// - Different from ComponentA (calls different visitor method)
    /// - Passes itself as ConcreteComponentB type
    /// - Visitor receives typed reference to ComponentB
    ///
    /// POLYMORPHISM:
    /// - Same Accept signature as ComponentA
    /// - Different implementation (calls different visitor method)
    /// - Runtime dispatch selects appropriate visitor method
    /// - Enables visitor to handle multiple component types
    ///
    /// TYPE-SPECIFIC HANDLING:
    /// - ComponentA calls VisitConcreteComponentA
    /// - ComponentB calls VisitConcreteComponentB
    /// - Visitor can implement different logic for each type
    /// - No casting or type checking needed
    /// </summary>
    public void Accept(IVisitor visitor)
    {
        visitor.VisitConcreteComponentB(this);
    }

    /// <summary>
    /// Component-specific method exclusive to ConcreteComponentB.
    ///
    /// DIFFERENT FROM COMPONENTA:
    /// - ComponentA has ExclusiveMethodOfConcreteComponentA
    /// - ComponentB has SpecialMethodOfConcreteComponentB
    /// - Each component has its own interface
    /// - Visitors can access both through specific visit methods
    ///
    /// PURPOSE:
    /// - Provides access to ComponentB's unique features
    /// - Different name and potentially different signature than A's method
    /// - Shows heterogeneity in component structure
    /// - Visitor can use both ExclusiveMethod and SpecialMethod
    ///
    /// USAGE IN VISITOR:
    /// - public void VisitConcreteComponentB(ConcreteComponentB component)
    /// - {
    /// -     var data = component.SpecialMethodOfConcreteComponentB();
    /// -     // Process data specific to ComponentB
    /// - }
    /// </summary>
    public string SpecialMethodOfConcreteComponentB()
    {
        return "B";
    }
}

/// <summary>
/// IVisitor interface declares visit methods for each concrete component type.
///
/// KEY RESPONSIBILITIES:
/// - Declares a visit method for each concrete component type
/// - Defines the operations that can be performed on components
/// - Enables adding new operations by implementing new visitors
///
/// DESIGN DECISIONS:
/// - One visit method per concrete component type
/// - Each method receives strongly-typed component reference
/// - All visitors must handle all component types
/// - New component type requires new visit method in all visitors
///
/// CHARACTERISTICS:
/// - Interface grows with number of component types
/// - Each visitor must implement all visit methods
/// - Visitors can access component-specific methods
/// - Enables type-safe operations on heterogeneous structures
///
/// TRADE-OFFS:
/// - Adding component type: Modify all visitors (violates OCP for components)
/// - Adding operation: Just create new visitor (follows OCP for operations)
/// - Good when component types are stable, operations change frequently
///
/// REAL-WORLD EXAMPLES:
/// - Compiler visitors (CodeGenerator, TypeChecker, Optimizer)
/// - Document visitors (PdfExporter, HtmlExporter, WordCounter)
/// - File system visitors (SizeCalculator, SecurityScanner, BackupCreator)
/// - Shopping cart visitors (TaxCalculator, ShippingCalculator, DiscountApplier)
/// </summary>
public interface IVisitor
{
    /// <summary>
    /// Visits a ConcreteComponentA instance.
    ///
    /// COMPONENT-SPECIFIC VISIT:
    /// - Receives ConcreteComponentA (not IComponent)
    /// - Can call component.ExclusiveMethodOfConcreteComponentA()
    /// - Implements A-specific operation logic
    /// - Type-safe access to ComponentA's interface
    ///
    /// IMPLEMENTATION:
    /// - Each concrete visitor implements this differently
    /// - Can access all public members of ConcreteComponentA
    /// - Performs visitor-specific operation on ComponentA
    ///
    /// EXAMPLE IMPLEMENTATIONS:
    /// - ExportVisitor: Exports ComponentA as JSON
    /// - CalculatorVisitor: Calculates value from ComponentA
    /// - ValidatorVisitor: Validates ComponentA's state
    /// </summary>
    void VisitConcreteComponentA(ConcreteComponentA element);

    /// <summary>
    /// Visits a ConcreteComponentB instance.
    ///
    /// DIFFERENT COMPONENT TYPE:
    /// - Receives ConcreteComponentB (different type than A)
    /// - Can call component.SpecialMethodOfConcreteComponentB()
    /// - Implements B-specific operation logic
    /// - Each visitor handles A and B differently
    ///
    /// POLYMORPHIC BEHAVIOR:
    /// - Same visitor can handle multiple component types
    /// - Each component type has its own visit method
    /// - Visitor implements different logic for each type
    /// - Enables complex operations on heterogeneous structures
    ///
    /// EXAMPLE:
    /// - ExportVisitor might export A and B in different formats
    /// - CalculatorVisitor might calculate different values from A vs B
    /// - ValidatorVisitor might apply different rules to A vs B
    /// </summary>
    void VisitConcreteComponentB(ConcreteComponentB element);
}

/// <summary>
/// ConcreteVisitor1 implements one set of operations on components.
///
/// KEY RESPONSIBILITIES:
/// - Implements visit methods for all component types
/// - Defines first operation that can be performed
/// - Accesses component-specific methods
/// - Demonstrates one way to process component structure
///
/// DESIGN APPROACH:
/// - Each visit method handles one component type
/// - Uses component-specific methods (ExclusiveMethod, SpecialMethod)
/// - Performs Visitor1-specific logic
/// - Different from ConcreteVisitor2's implementation
///
/// CHARACTERISTICS:
/// - Must implement all visit methods from IVisitor
/// - Can maintain visitor-specific state
/// - Can perform complex operations using component data
/// - New visitor type adds new operation without modifying components
///
/// REAL-WORLD EXAMPLES:
/// - CodeGeneratorVisitor (generates code from AST)
/// - PdfExportVisitor (exports document elements to PDF)
/// - SizeCalculatorVisitor (calculates total size of file system)
/// - TaxCalculatorVisitor (calculates tax for shopping cart items)
/// </summary>
class ConcreteVisitor1 : IVisitor
{
    /// <summary>
    /// Visits ConcreteComponentA and performs Visitor1's operation.
    ///
    /// VISITOR1'S OPERATION ON A:
    /// - Calls component's ExclusiveMethodOfConcreteComponentA()
    /// - Combines with visitor identifier ("+ ConcreteVisitor1")
    /// - Demonstrates accessing component-specific methods
    /// - Output format specific to Visitor1
    ///
    /// IMPLEMENTATION DETAILS:
    /// - Gets data from ComponentA's exclusive method
    /// - Performs Visitor1-specific processing
    /// - Produces output combining component data and visitor logic
    ///
    /// EXAMPLE:
    /// - component.ExclusiveMethodOfConcreteComponentA() returns "A"
    /// - Visitor1 appends " + ConcreteVisitor1"
    /// - Result: "A + ConcreteVisitor1"
    ///
    /// DESIGN PATTERN:
    /// - Visitor encapsulates operation logic
    /// - Component provides data through its methods
    /// - Separation of data structure and algorithms
    /// </summary>
    public void VisitConcreteComponentA(ConcreteComponentA element)
    {
        Console.WriteLine(element.ExclusiveMethodOfConcreteComponentA() + " + ConcreteVisitor1");
    }

    /// <summary>
    /// Visits ConcreteComponentB and performs Visitor1's operation.
    ///
    /// VISITOR1'S OPERATION ON B:
    /// - Calls component's SpecialMethodOfConcreteComponentB()
    /// - Combines with visitor identifier ("+ ConcreteVisitor1")
    /// - Different method than A but same visitor logic pattern
    /// - Demonstrates handling different component types
    ///
    /// POLYMORPHISM:
    /// - Same visitor, different component type
    /// - Visitor1 handles both A and B
    /// - Each component provides different data
    /// - Visitor applies consistent operation pattern
    ///
    /// EXAMPLE:
    /// - component.SpecialMethodOfConcreteComponentB() returns "B"
    /// - Visitor1 appends " + ConcreteVisitor1"
    /// - Result: "B + ConcreteVisitor1"
    ///
    /// COMPARISON WITH VISITOR2:
    /// - Visitor1 appends " + ConcreteVisitor1"
    /// - Visitor2 will append " + ConcreteVisitor2"
    /// - Same component, different visitor = different behavior
    /// </summary>
    public void VisitConcreteComponentB(ConcreteComponentB element)
    {
        Console.WriteLine(element.SpecialMethodOfConcreteComponentB() + " + ConcreteVisitor1");
    }
}

/// <summary>
/// ConcreteVisitor2 implements a different set of operations on components.
///
/// KEY RESPONSIBILITIES:
/// - Implements same visit methods as Visitor1
/// - Provides different operation implementation
/// - Demonstrates adding new operations without modifying components
/// - Shows polymorphism at visitor level
///
/// DESIGN APPROACH:
/// - Same interface as Visitor1 (implements IVisitor)
/// - Different implementation logic
/// - Accesses same component methods
/// - Produces different results
///
/// CHARACTERISTICS:
/// - New visitor = new operation on entire component structure
/// - No changes needed to component classes
/// - Can add unlimited visitors for unlimited operations
/// - Demonstrates Open/Closed Principle for operations
///
/// REAL-WORLD EXAMPLES:
/// - OptimizerVisitor (optimizes AST nodes)
/// - HtmlExportVisitor (exports document elements to HTML)
/// - SecurityScannerVisitor (scans files for threats)
/// - DiscountApplierVisitor (applies discounts to cart items)
/// </summary>
class ConcreteVisitor2 : IVisitor
{
    /// <summary>
    /// Visits ConcreteComponentA with Visitor2's operation.
    ///
    /// VISITOR2'S OPERATION ON A:
    /// - Same component method as Visitor1 used
    /// - Different visitor identifier ("+ ConcreteVisitor2")
    /// - Demonstrates different operation on same component
    /// - Shows visitor polymorphism
    ///
    /// COMPARISON WITH VISITOR1:
    /// - Visitor1.VisitConcreteComponentA: "A + ConcreteVisitor1"
    /// - Visitor2.VisitConcreteComponentA: "A + ConcreteVisitor2"
    /// - Same component, different result
    /// - New operation added without touching ComponentA
    ///
    /// PATTERN BENEFIT:
    /// - Adding Visitor2 didn't require modifying ComponentA
    /// - ComponentA's Accept method unchanged
    /// - New operation added by creating new visitor class
    /// - Open/Closed Principle in action
    /// </summary>
    public void VisitConcreteComponentA(ConcreteComponentA element)
    {
        Console.WriteLine(element.ExclusiveMethodOfConcreteComponentA() + " + ConcreteVisitor2");
    }

    /// <summary>
    /// Visits ConcreteComponentB with Visitor2's operation.
    ///
    /// VISITOR2'S OPERATION ON B:
    /// - Different from Visitor1's operation
    /// - Appends "ConcreteVisitor2" instead of "ConcreteVisitor1"
    /// - Same component type, different visitor
    /// - Produces different output
    ///
    /// EXAMPLE:
    /// - component.SpecialMethodOfConcreteComponentB() returns "B"
    /// - Visitor2 appends " + ConcreteVisitor2"
    /// - Result: "B + ConcreteVisitor2"
    ///
    /// EXTENSIBILITY:
    /// - Can add Visitor3, Visitor4, etc.
    /// - Each adds new operation
    /// - Components remain unchanged
    /// - No modification to component classes needed
    /// </summary>
    public void VisitConcreteComponentB(ConcreteComponentB element)
    {
        Console.WriteLine(element.SpecialMethodOfConcreteComponentB() + " + ConcreteVisitor2");
    }
}

/// <summary>
/// Client demonstrates how to use the visitor pattern.
///
/// KEY RESPONSIBILITIES:
/// - Creates component structure
/// - Applies visitors to components
/// - Demonstrates visitor pattern usage
/// - Shows polymorphic visitor application
///
/// DESIGN PATTERN:
/// - Client works with IComponent and IVisitor interfaces
/// - Doesn't depend on concrete types
/// - Can work with any component or visitor
/// - Iterates through components applying visitor
///
/// BENEFITS:
/// - Simple client code
/// - Same code works with different visitors
/// - Same code works with different component types
/// - Demonstrates separation of concerns
/// </summary>
public class Client
{
    /// <summary>
    /// Applies a visitor to a collection of components.
    ///
    /// VISITOR APPLICATION PATTERN:
    /// - Receives list of components (heterogeneous collection)
    /// - Receives visitor to apply
    /// - Iterates through components
    /// - Calls component.Accept(visitor) for each
    ///
    /// POLYMORPHISM:
    /// - Components can be different types (ComponentA, ComponentB)
    /// - All implement IComponent interface
    /// - Each calls its own specific visitor method via Accept
    /// - Visitor handles each component type appropriately
    ///
    /// DOUBLE DISPATCH IN ACTION:
    /// - Client calls component.Accept(visitor)
    /// - ComponentA calls visitor.VisitConcreteComponentA(this)
    /// - ComponentB calls visitor.VisitConcreteComponentB(this)
    /// - Correct visitor method called based on component type
    ///
    /// DESIGN BENEFITS:
    /// - Client doesn't need to know component types
    /// - No type checking or casting
    /// - Works with any visitor
    /// - Can process complex heterogeneous structures
    ///
    /// EXAMPLE USAGE:
    /// - Same ClientCode works with Visitor1 and Visitor2
    /// - Components list can contain mix of A and B
    /// - Each component handled appropriately by visitor
    /// </summary>
    public static void ClientCode(List<IComponent> components, IVisitor visitor)
    {
        foreach (var component in components)
        {
            component.Accept(visitor);
        }
    }
}

/// <summary>
/// Program demonstrates the Visitor pattern in action.
/// Shows how to add new operations to object structures without modifying the structures.
/// </summary>
public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Visitor Pattern Demonstration ===\n");

        Console.WriteLine("--- Example 1: Creating Component Structure ---\n");

        // Create heterogeneous collection of components
        // Mix of ComponentA and ComponentB
        List<IComponent> components = new List<IComponent>
        {
            new ConcreteComponentA(),
            new ConcreteComponentB()
        };

        Console.WriteLine("Component structure created: [ComponentA, ComponentB]\n");

        Console.WriteLine("--- Example 2: Applying Visitor1 ---");
        Console.WriteLine("The client code works with all visitors via the base Visitor interface:\n");

        // Create first visitor and apply to all components
        var visitor1 = new ConcreteVisitor1();
        Client.ClientCode(components, visitor1);
        // Output:
        // - "A + ConcreteVisitor1" (from ComponentA)
        // - "B + ConcreteVisitor1" (from ComponentB)
        // Visitor1's operation applied to entire structure

        Console.WriteLine();

        Console.WriteLine("--- Example 3: Applying Visitor2 ---");
        Console.WriteLine("It allows the same client code to work with different types of visitors:\n");

        // Create second visitor and apply to same components
        var visitor2 = new ConcreteVisitor2();
        Client.ClientCode(components, visitor2);
        // Output:
        // - "A + ConcreteVisitor2" (from ComponentA)
        // - "B + ConcreteVisitor2" (from ComponentB)
        // Visitor2's operation applied to same structure
        // Different operation without modifying components

        Console.WriteLine();

        Console.WriteLine("--- Key Observations ---");
        Console.WriteLine("1. Same components processed by different visitors");
        Console.WriteLine("2. Each visitor produces different results");
        Console.WriteLine("3. Components unchanged when adding new visitor");
        Console.WriteLine("4. Double dispatch enables type-safe operations");
        Console.WriteLine("5. New operations added by creating new visitors");
    }
}

/*
 * KEY TAKEAWAYS:
 *
 * PATTERN COMPONENTS:
 * 1. IComponent - Interface with Accept method
 * 2. ConcreteComponentA/B - Components with specific methods
 * 3. IVisitor - Interface with visit method for each component type
 * 4. ConcreteVisitor1/2 - Visitors implementing different operations
 * 5. Client - Applies visitors to component structures
 *
 * WHEN TO USE:
 * - Need to perform many different operations on object structure
 * - Object structure is stable (component types rarely change)
 * - Operations change frequently or new operations added often
 * - Want to keep related operations together in one class
 * - Need to accumulate state while traversing object structure
 *
 * BENEFITS:
 * 1. Open/Closed for Operations: Add operations without modifying components
 * 2. Single Responsibility: Operations grouped in visitor classes
 * 3. Accumulate State: Visitor can maintain state across component visits
 * 4. Type Safety: No casting needed, strongly-typed component references
 * 5. Related Operations Together: All logic for one operation in one visitor
 * 6. Works with Heterogeneous Structures: Components can be different types
 *
 * REAL-WORLD EXAMPLES:
 * - Compiler: AST visitors for code generation, optimization, type checking
 *   public class CodeGenerator : IVisitor
 *   {
 *       public void VisitNumberExpression(NumberExpression expr)
 *       {
 *           Emit("PUSH " + expr.Value);
 *       }
 *       public void VisitBinaryExpression(BinaryExpression expr)
 *       {
 *           expr.Left.Accept(this);
 *           expr.Right.Accept(this);
 *           Emit(expr.Operator);
 *       }
 *   }
 *
 * - Document Processing: Export to different formats (PDF, HTML, Word)
 *   public class PdfExportVisitor : IVisitor
 *   {
 *       public void VisitParagraph(Paragraph p) => AddTextToPdf(p.Text);
 *       public void VisitImage(Image img) => AddImageToPdf(img.Data);
 *       public void VisitTable(Table t) => AddTableToPdf(t.Rows);
 *   }
 *
 * - File System: Calculate size, find files, create backups
 *   public class SizeCalculatorVisitor : IVisitor
 *   {
 *       private long _totalSize;
 *       public void VisitFile(File f) => _totalSize += f.Size;
 *       public void VisitDirectory(Directory d)
 *       {
 *           foreach (var item in d.Items)
 *               item.Accept(this);
 *       }
 *   }
 *
 * - Shopping Cart: Calculate tax, shipping, discounts
 *   public class TaxCalculatorVisitor : IVisitor
 *   {
 *       private decimal _totalTax;
 *       public void VisitPhysicalProduct(PhysicalProduct p)
 *       {
 *           _totalTax += p.Price * 0.08m; // 8% tax
 *       }
 *       public void VisitDigitalProduct(DigitalProduct p)
 *       {
 *           _totalTax += p.Price * 0.05m; // 5% tax
 *       }
 *   }
 *
 * - UI Elements: Rendering, validation, serialization
 *   public class RenderVisitor : IVisitor
 *   {
 *       public void VisitButton(Button b) => RenderButton(b);
 *       public void VisitTextBox(TextBox t) => RenderTextBox(t);
 *       public void VisitCheckBox(CheckBox c) => RenderCheckBox(c);
 *   }
 *
 * TRADE-OFFS:
 * - Adding Component Type: Must modify all visitors (violates OCP)
 * - Many Component Types: IVisitor interface becomes large
 * - Breaks Encapsulation: Visitors need access to component internals
 * - Circular Dependency: Components depend on visitors, visitors depend on components
 * - Complexity: Double dispatch can be confusing
 *
 * DOUBLE DISPATCH EXPLAINED:
 * 1. Client calls: component.Accept(visitor)
 * 2. Component calls: visitor.VisitConcreteComponentA(this)
 * 3. Result: Correct visitor method called based on runtime types of BOTH
 *
 * VARIATIONS:
 * 1. Acyclic Visitor: Avoids circular dependency using interfaces
 * 2. Extension Visitor: Uses extension methods instead of Accept
 * 3. Reflective Visitor: Uses reflection to find visit methods
 *
 * IMPLEMENTATION CONSIDERATIONS:
 * - Component Count: Pattern works best with stable component types
 * - Operation Count: Ideal when operations change more than components
 * - State Management: Visitors can accumulate state during traversal
 * - Return Values: Visit methods can return values instead of void
 * - Error Handling: Consider how visitors handle errors
 *
 * MODERN .NET USAGE:
 * - Roslyn Compiler: Syntax tree visitors
 *   public class MyVisitor : CSharpSyntaxWalker
 *   {
 *       public override void VisitMethodDeclaration(MethodDeclarationSyntax node) { }
 *   }
 *
 * - Expression Trees: Expression visitors
 *   public class ParameterReplacer : ExpressionVisitor
 *   {
 *       protected override Expression VisitParameter(ParameterExpression node) { }
 *   }
 *
 * COMPARISON WITH OTHER PATTERNS:
 * - Composite: Often used with Visitor to traverse tree structures
 * - Iterator: Visitor traverses structure, Iterator provides access to elements
 * - Strategy: Both encapsulate algorithms, but Visitor works on object structure
 *
 * BEST PRACTICES:
 * 1. Use when component types are stable
 * 2. Group related operations in single visitor
 * 3. Consider return values from visit methods
 * 4. Document visitor dependencies on component structure
 * 5. Use meaningful visitor and visit method names
 * 6. Consider thread safety if visitors maintain state
 * 7. Provide base visitor class for common functionality
 * 8. Consider visitor factory if creation is complex
 */
