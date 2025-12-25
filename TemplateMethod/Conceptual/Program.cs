// The Template Method pattern defines the skeleton of an algorithm in a base class,
// allowing subclasses to override specific steps without changing the algorithm's structure.
// This promotes code reuse and enforces a consistent algorithm structure across variations.

/// <summary>
/// AbstractClass defines the template method and declares abstract/virtual operations.
///
/// KEY RESPONSIBILITIES:
/// - Defines the template method with the algorithm skeleton
/// - Implements common operations shared by all subclasses
/// - Declares abstract methods that subclasses must implement
/// - Provides optional hook methods for customization points
///
/// DESIGN DECISIONS:
/// - Template method is public and calls private/protected helper methods
/// - Base operations are implemented in abstract class (code reuse)
/// - Required operations are abstract (must be implemented by subclasses)
/// - Hooks are virtual with empty/default implementation (optional override)
///
/// CHARACTERISTICS:
/// - Template method should NOT be overridden (can mark as sealed/final)
/// - Base operations provide default behavior
/// - Abstract operations enforce implementation in subclasses
/// - Hooks provide extension points without requiring override
///
/// REAL-WORLD EXAMPLES:
/// - Data processing pipeline (Load -> Transform -> Validate -> Save)
/// - Web request handling (Authenticate -> Authorize -> Process -> Log)
/// - Test framework setup (SetUp -> RunTest -> TearDown)
/// - Build process (Clean -> Compile -> Test -> Package -> Deploy)
/// </summary>
public abstract class AbstractClass
{
    /// <summary>
    /// The template method defines the skeleton of the algorithm.
    ///
    /// TEMPLATE METHOD PATTERN:
    /// - Defines the overall algorithm structure
    /// - Calls base operations, required operations, and hooks in specific order
    /// - Should NOT be overridden by subclasses (controls the workflow)
    /// - Each step is a separate method (primitive operations)
    ///
    /// ALGORITHM STRUCTURE:
    /// 1. BaseOperation1 - Common operation (implemented in abstract class)
    /// 2. RequiredOperations1 - Subclass-specific (abstract, must implement)
    /// 3. BaseOperation2 - Another common operation
    /// 4. Hook1 - Optional extension point (virtual, can override)
    /// 5. RequiredOperation2 - Another subclass-specific operation
    /// 6. BaseOperation3 - Final common operation
    /// 7. Hook2 - Another optional extension point
    ///
    /// DESIGN BENEFITS:
    /// - Enforces consistent algorithm structure across all subclasses
    /// - Defines the sequence of operations (invariant behavior)
    /// - Subclasses can't change the algorithm flow, only specific steps
    /// - Easy to understand the overall process by reading this method
    ///
    /// HOLLYWOOD PRINCIPLE:
    /// - "Don't call us, we'll call you"
    /// - Parent class calls subclass methods, not the other way around
    /// - Inverts control compared to typical inheritance usage
    ///
    /// BEST PRACTICES:
    /// - Keep template method simple (just orchestration)
    /// - Each step should be a separate method with clear name
    /// - Document the algorithm flow and purpose of each step
    /// - Consider making template method sealed to prevent override
    /// </summary>
    public void TemplateMethod()
    {
        // Step 1: Common operation implemented in base class
        BaseOperation1();

        // Step 2: Required operation that subclasses must implement
        RequiredOperations1();

        // Step 3: Another common operation
        BaseOperation2();

        // Step 4: Optional hook that subclasses can override
        Hook1();

        // Step 5: Another required operation
        RequiredOperation2();

        // Step 6: Final common operation
        BaseOperation3();

        // Step 7: Another optional hook
        Hook2();
    }

    /// <summary>
    /// Base operation with implementation in abstract class.
    ///
    /// BASE OPERATIONS:
    /// - Implemented in the abstract class (not abstract)
    /// - Provides common behavior shared across all subclasses
    /// - Can be protected (only accessible to subclasses)
    /// - Represents the invariant parts of the algorithm
    ///
    /// PURPOSE:
    /// - Code reuse: Write once, used by all subclasses
    /// - Consistent behavior: Same implementation for all concrete classes
    /// - Part of the algorithm skeleton that doesn't vary
    ///
    /// DESIGN NOTES:
    /// - Could be private if subclasses don't need to call it
    /// - Protected allows subclasses to reuse this operation
    /// - Could be virtual if subclasses need to extend (but not required to)
    /// </summary>
    protected void BaseOperation1()
    {
        Console.WriteLine("AbstractClass says: I am doing the bulk of the work");
    }

    /// <summary>
    /// Another base operation demonstrating shared functionality.
    ///
    /// COMMON FUNCTIONALITY:
    /// - This operation is the same for all subclasses
    /// - Reduces code duplication
    /// - Centralizes common logic in one place
    ///
    /// WHEN TO USE BASE OPERATIONS:
    /// - Logic is identical across all subclasses
    /// - Operation doesn't need customization
    /// - Want to ensure consistent behavior
    /// </summary>
    protected void BaseOperation2()
    {
        Console.WriteLine("AbstractClass says: But I let subclasses override some operations");
    }

    /// <summary>
    /// Third base operation showing the pattern of shared operations.
    ///
    /// ALGORITHM INVARIANTS:
    /// - These operations form the invariant parts
    /// - Subclasses inherit this behavior automatically
    /// - Changes here affect all subclasses
    /// - Demonstrates Template Method's code reuse benefit
    /// </summary>
    protected void BaseOperation3()
    {
        Console.WriteLine("AbstractClass says: But I am doing the bulk of the work anyway");
    }

    /// <summary>
    /// Required operation that concrete classes must implement.
    ///
    /// ABSTRACT OPERATIONS:
    /// - Must be implemented by all concrete subclasses
    /// - No default implementation in abstract class
    /// - Represents the variant parts of the algorithm
    /// - Each subclass provides its own specific behavior
    ///
    /// PURPOSE:
    /// - Forces subclasses to provide specific implementation
    /// - Allows algorithm customization at specific points
    /// - Ensures all subclasses handle this step
    ///
    /// DESIGN CONSIDERATIONS:
    /// - Use abstract when operation has no sensible default
    /// - Use virtual when you want to provide default but allow override
    /// - Choose method signature carefully (parameters, return type)
    ///
    /// NAMING:
    /// - Use descriptive names that indicate purpose
    /// - Consider naming convention like "Do[Action]" or "Process[Step]"
    /// </summary>
    protected abstract void RequiredOperations1();

    /// <summary>
    /// Another required operation for subclass-specific behavior.
    ///
    /// MULTIPLE ABSTRACT OPERATIONS:
    /// - Template can have multiple customization points
    /// - Each represents a step that varies between subclasses
    /// - Subclasses must implement all abstract operations
    /// - Together they define the variant parts of the algorithm
    ///
    /// FLEXIBILITY:
    /// - More abstract operations = more flexibility
    /// - Fewer abstract operations = more constraints
    /// - Balance between flexibility and consistency
    /// </summary>
    protected abstract void RequiredOperation2();

    /// <summary>
    /// Hook method with empty default implementation.
    ///
    /// HOOK METHODS:
    /// - Virtual methods with empty or minimal default implementation
    /// - Provide optional extension points in the algorithm
    /// - Subclasses CAN override but are NOT required to
    /// - Allow customization without forcing it
    ///
    /// PURPOSE:
    /// - Give subclasses chance to react at specific points
    /// - Provide extension points without breaking subclasses that don't need them
    /// - Allow gradual customization (override only what you need)
    ///
    /// DESIGN PATTERN:
    /// - Virtual (not abstract) - override is optional
    /// - Empty body or minimal default behavior
    /// - Called at strategic points in template method
    ///
    /// USE CASES:
    /// - Optional pre/post processing
    /// - Optional validation or logging
    /// - Extension points for future customization
    /// - Handling optional features
    ///
    /// BEST PRACTICES:
    /// - Name hooks descriptively: Before[Action], After[Action], On[Event]
    /// - Document when and why the hook is called
    /// - Keep default implementation empty or minimal
    /// - Don't make hooks required (that's what abstract is for)
    /// </summary>
    protected virtual void Hook1()
    {
        // Empty default implementation
        // Subclasses can override to add behavior at this point
    }

    /// <summary>
    /// Another hook method for optional customization.
    ///
    /// MULTIPLE HOOKS:
    /// - Can have multiple hooks at different points in algorithm
    /// - Each provides an extension point
    /// - Subclasses choose which hooks to implement
    /// - Enables fine-grained customization
    ///
    /// EXAMPLE SCENARIOS:
    /// - Hook1: Before processing (e.g., validation)
    /// - Hook2: After processing (e.g., logging, cleanup)
    /// - BeforeOperation: Pre-processing hook
    /// - AfterOperation: Post-processing hook
    ///
    /// FLEXIBILITY VS COMPLEXITY:
    /// - More hooks = more flexibility but more complex interface
    /// - Fewer hooks = simpler but less flexible
    /// - Add hooks only where customization is likely needed
    /// </summary>
    protected virtual void Hook2()
    {
        // Empty default implementation
        // Optional extension point
    }
}

/// <summary>
/// ConcreteClass1 provides one implementation of the template.
///
/// KEY RESPONSIBILITIES:
/// - Implements all abstract operations (required)
/// - Inherits base operations from abstract class
/// - Can optionally override hook methods
/// - Provides specific behavior for this variation
///
/// DESIGN APPROACH:
/// - Focuses only on the variant parts (abstract operations)
/// - Reuses all base operations from parent class
/// - Doesn't override hooks (uses default empty implementation)
/// - Minimal implementation - only what's required
///
/// CHARACTERISTICS:
/// - Cannot change the template method (algorithm structure is fixed)
/// - Must implement RequiredOperations1 and RequiredOperation2
/// - Inherits all base operations automatically
/// - Template method works with this class's implementations
///
/// REAL-WORLD EXAMPLES:
/// - CSVDataReader (implements ReadLine for CSV format)
/// - JsonDataReader (implements ReadLine for JSON format)
/// - HttpRequestHandler (implements ProcessRequest for HTTP)
/// - EmailNotificationSender (implements Send for email)
/// </summary>
public class ConcreteClass1 : AbstractClass
{
    /// <summary>
    /// Implements required operation 1 with ConcreteClass1-specific behavior.
    ///
    /// IMPLEMENTATION SPECIFICS:
    /// - Provides behavior unique to ConcreteClass1
    /// - Called by template method at the designated point
    /// - Can access protected members of abstract class
    /// - Works within the algorithm structure defined by template
    ///
    /// DESIGN NOTES:
    /// - Implementation must fit within template method's expectations
    /// - Should not try to change the algorithm flow
    /// - Focuses on this specific step's behavior
    /// - Can call other protected methods if needed
    ///
    /// RESPONSIBILITY:
    /// - Implement the variant part of the algorithm
    /// - Provide ConcreteClass1's specific behavior
    /// - Fit into the overall algorithm structure
    /// </summary>
    protected override void RequiredOperations1()
    {
        Console.WriteLine("ConcreteClass1 says: Implemented Operation1");
    }

    /// <summary>
    /// Implements required operation 2 with ConcreteClass1-specific behavior.
    ///
    /// MULTIPLE REQUIRED OPERATIONS:
    /// - Each concrete class must implement all abstract operations
    /// - Each operation represents a customization point
    /// - Together they define this class's variant behavior
    /// - Template method orchestrates calling these operations
    ///
    /// CONSISTENCY:
    /// - All concrete classes implement same set of abstract operations
    /// - Ensures all variations provide complete algorithm implementation
    /// - Template method can rely on these operations existing
    /// </summary>
    protected override void RequiredOperation2()
    {
        Console.WriteLine("ConcreteClass1 says: Implemented Operation2");
    }

    // Note: Hook1 and Hook2 are NOT overridden
    // ConcreteClass1 uses the default empty implementations from AbstractClass
    // This demonstrates that hooks are optional
}

/// <summary>
/// ConcreteClass2 provides another implementation with hook customization.
///
/// KEY RESPONSIBILITIES:
/// - Implements all abstract operations (required)
/// - Optionally overrides hook methods for additional customization
/// - Demonstrates full customization capabilities
/// - Shows how hooks can be used to extend behavior
///
/// DESIGN APPROACH:
/// - Implements required operations (like ConcreteClass1)
/// - ALSO overrides Hook1 to add custom behavior
/// - Shows difference between required operations and hooks
/// - Demonstrates optional customization through hooks
///
/// CHARACTERISTICS:
/// - More customized than ConcreteClass1 (overrides hook)
/// - Still follows same template method structure
/// - Hook override doesn't change algorithm flow
/// - Adds behavior at extension point without breaking template
///
/// COMPARISON WITH CONCRETECLASS1:
/// - Both implement same abstract operations (required)
/// - ConcreteClass2 also overrides Hook1 (optional)
/// - Shows flexibility of hook methods
/// - Demonstrates varying levels of customization
///
/// REAL-WORLD EXAMPLES:
/// - XMLDataReader with validation hook
/// - HttpsRequestHandler with encryption hook
/// - PremiumEmailSender with priority handling hook
/// - EnhancedReportGenerator with formatting hook
/// </summary>
public class ConcreteClass2 : AbstractClass
{
    /// <summary>
    /// Implements required operation 1 with ConcreteClass2-specific behavior.
    ///
    /// VARIATION:
    /// - Different implementation than ConcreteClass1
    /// - Same operation, different behavior
    /// - Called at same point in template method
    /// - Demonstrates polymorphism in action
    ///
    /// TEMPLATE METHOD BENEFIT:
    /// - Same template method works with different implementations
    /// - Algorithm structure is consistent
    /// - Behavior varies based on concrete class
    /// - Client code doesn't need to know which concrete class is used
    /// </summary>
    protected override void RequiredOperations1()
    {
        Console.WriteLine("ConcreteClass2 says: Implemented Operation1");
    }

    /// <summary>
    /// Implements required operation 2 with ConcreteClass2-specific behavior.
    ///
    /// POLYMORPHISM:
    /// - Same method signature as ConcreteClass1
    /// - Different implementation
    /// - Template method doesn't care about the difference
    /// - Each concrete class provides its own behavior
    /// </summary>
    protected override void RequiredOperation2()
    {
        Console.WriteLine("ConcreteClass2 says: Implemented Operation2");
    }

    /// <summary>
    /// Overrides Hook1 to add optional custom behavior.
    ///
    /// HOOK OVERRIDE:
    /// - Optional - ConcreteClass1 didn't override this
    /// - Provides additional customization beyond required operations
    /// - Adds behavior at the hook point in template method
    /// - Doesn't break or change the algorithm structure
    ///
    /// EXTENSION POINT:
    /// - Hook methods provide places to inject custom behavior
    /// - ConcreteClass2 takes advantage of this extension point
    /// - Adds behavior without changing template method
    /// - Template method calls this when reaching Hook1()
    ///
    /// USE CASES FOR HOOKS:
    /// - Additional validation
    /// - Logging or auditing
    /// - Performance monitoring
    /// - Resource cleanup
    /// - Notification or events
    ///
    /// DESIGN BENEFIT:
    /// - Allows customization without requiring it
    /// - ConcreteClass1 works fine without overriding hooks
    /// - ConcreteClass2 adds behavior where needed
    /// - Gradual customization approach
    /// </summary>
    protected override void Hook1()
    {
        Console.WriteLine("ConcreteClass2 says: Overridden Hook1");
    }

    // Note: Hook2 is NOT overridden
    // ConcreteClass2 only customizes Hook1, not Hook2
    // Demonstrates selective hook override
}

/// <summary>
/// The Client class demonstrates how to use the template method.
///
/// KEY RESPONSIBILITIES:
/// - Provides interface for working with AbstractClass instances
/// - Demonstrates polymorphic usage of template method
/// - Shows that client code is the same regardless of concrete class
///
/// DESIGN PATTERN:
/// - Client depends only on AbstractClass (not concrete classes)
/// - Client calls template method, which orchestrates the algorithm
/// - Client doesn't need to know about the internal steps
/// - Works with any concrete class through polymorphism
///
/// BENEFITS:
/// - Client code is simple and consistent
/// - No conditional logic needed
/// - Can work with new concrete classes without changes
/// - Demonstrates Open/Closed Principle
/// </summary>
public class Client
{
    /// <summary>
    /// Executes the template method on any AbstractClass instance.
    ///
    /// POLYMORPHIC USAGE:
    /// - Accepts AbstractClass parameter (base type)
    /// - Works with ConcreteClass1, ConcreteClass2, or any future concrete classes
    /// - Calls template method, which internally calls appropriate overrides
    /// - Demonstrates dependency on abstraction, not concrete classes
    ///
    /// CLIENT PERSPECTIVE:
    /// - Client just calls TemplateMethod()
    /// - Doesn't know or care which concrete class it is
    /// - Template method handles the complexity
    /// - Different behavior based on actual concrete class
    ///
    /// DESIGN BENEFITS:
    /// - Simple client code
    /// - No knowledge of algorithm internals
    /// - Same code works with all variations
    /// - Easy to add new concrete classes
    ///
    /// HOLLYWOOD PRINCIPLE:
    /// - Client calls template method
    /// - Template method calls concrete class methods
    /// - Inversion of control compared to typical method calls
    /// </summary>
    public static void ClientCode(AbstractClass abstractClass)
    {
        // Client simply calls the template method
        // Template method orchestrates the entire algorithm
        // Behavior varies based on which concrete class was passed
        abstractClass.TemplateMethod();
    }
}

/// <summary>
/// Program demonstrates the Template Method pattern in action.
/// Shows how the same algorithm structure produces different results with different concrete classes.
/// </summary>
public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Template Method Pattern Demonstration ===\n");

        Console.WriteLine("--- Example 1: Using ConcreteClass1 ---");
        Console.WriteLine("Same client code can work with different subclasses:\n");

        // Create ConcreteClass1 instance
        // Client code doesn't need to know implementation details
        Client.ClientCode(new ConcreteClass1());
        // Output shows:
        // - BaseOperation1 (from abstract class)
        // - RequiredOperations1 (from ConcreteClass1)
        // - BaseOperation2 (from abstract class)
        // - Hook1 (not overridden, default empty)
        // - RequiredOperation2 (from ConcreteClass1)
        // - BaseOperation3 (from abstract class)
        // - Hook2 (not overridden, default empty)

        Console.Write("\n");

        Console.WriteLine("--- Example 2: Using ConcreteClass2 ---");
        Console.WriteLine("Same client code can work with different subclasses:\n");

        // Create ConcreteClass2 instance
        // Same client code, different behavior
        Client.ClientCode(new ConcreteClass2());
        // Output shows:
        // - BaseOperation1 (from abstract class)
        // - RequiredOperations1 (from ConcreteClass2 - different than Class1)
        // - BaseOperation2 (from abstract class)
        // - Hook1 (overridden by ConcreteClass2 - adds custom behavior)
        // - RequiredOperation2 (from ConcreteClass2 - different than Class1)
        // - BaseOperation3 (from abstract class)
        // - Hook2 (not overridden, default empty)

        Console.WriteLine("\n--- Key Observations ---");
        Console.WriteLine("1. Same template method structure for both classes");
        Console.WriteLine("2. Base operations are shared (code reuse)");
        Console.WriteLine("3. Required operations vary between classes (customization)");
        Console.WriteLine("4. Hooks provide optional extension points");
        Console.WriteLine("5. Client code is identical for both classes (polymorphism)");
        Console.WriteLine("6. Algorithm structure is enforced by template method");
    }
}

/*
 * KEY TAKEAWAYS:
 *
 * PATTERN COMPONENTS:
 * 1. AbstractClass - Defines template method and primitive operations
 * 2. Template Method - Defines algorithm skeleton, calls primitive operations
 * 3. Base Operations - Implemented in abstract class (code reuse)
 * 4. Abstract Operations - Must be implemented by concrete classes (variation points)
 * 5. Hook Methods - Optional override points (extension points)
 * 6. ConcreteClass - Implements abstract operations, optionally overrides hooks
 *
 * WHEN TO USE:
 * - Multiple classes implement similar algorithms with slight variations
 * - Want to avoid code duplication in similar algorithms
 * - Need to control the algorithm structure but allow customization
 * - Want to enforce a specific sequence of operations
 * - Have an algorithm with both invariant and variant parts
 *
 * BENEFITS:
 * 1. Code Reuse: Common operations defined once in abstract class
 * 2. Consistency: All subclasses follow same algorithm structure
 * 3. Control: Template method controls the flow, subclasses can't change it
 * 4. Extensibility: Easy to add new variations by creating new concrete classes
 * 5. Hollywood Principle: "Don't call us, we'll call you" - inversion of control
 * 6. Open/Closed: Open for extension (new concrete classes), closed for modification
 * 7. Single Responsibility: Each class focuses on its specific variation
 *
 * REAL-WORLD EXAMPLES:
 * - Data Processing Pipeline: Load -> Transform -> Validate -> Save
 *   public abstract class DataProcessor
 *   {
 *       public void Process()
 *       {
 *           var data = LoadData();      // Abstract - varies by source
 *           data = Transform(data);     // Abstract - varies by format
 *           Validate(data);             // Base - common validation
 *           SaveData(data);             // Abstract - varies by destination
 *       }
 *   }
 *
 * - Unit Test Framework: SetUp -> RunTest -> TearDown
 *   public abstract class TestCase
 *   {
 *       public void Run()
 *       {
 *           SetUp();        // Hook - optional setup
 *           RunTest();      // Abstract - test implementation
 *           TearDown();     // Hook - optional cleanup
 *       }
 *   }
 *
 * - Web Request Handling: Authenticate -> Authorize -> Process -> Log
 *   public abstract class RequestHandler
 *   {
 *       public void HandleRequest()
 *       {
 *           Authenticate();       // Base - common authentication
 *           Authorize();          // Abstract - varies by endpoint
 *           ProcessRequest();     // Abstract - varies by handler
 *           LogRequest();         // Base - common logging
 *       }
 *   }
 *
 * - Document Generation: CollectData -> FormatContent -> ApplyStyle -> Export
 *   public abstract class DocumentGenerator
 *   {
 *       public void Generate()
 *       {
 *           var data = CollectData();     // Abstract - varies by source
 *           var content = Format(data);   // Abstract - varies by format
 *           ApplyStyles(content);         // Base - common styling
 *           Export(content);              // Abstract - varies by format
 *       }
 *   }
 *
 * - Build Process: Clean -> Compile -> Test -> Package -> Deploy
 *   public abstract class BuildProcess
 *   {
 *       public void Build()
 *       {
 *           Clean();            // Base - common cleanup
 *           Compile();          // Abstract - varies by language
 *           RunTests();         // Base - common test execution
 *           Package();          // Abstract - varies by package type
 *           Deploy();           // Hook - optional deployment
 *       }
 *   }
 *
 * - Game AI: SenseEnvironment -> Analyze -> Decide -> Act
 *   public abstract class GameAI
 *   {
 *       public void Update()
 *       {
 *           var input = SenseEnvironment();  // Abstract - varies by AI type
 *           var situation = Analyze(input);  // Abstract - varies by AI
 *           var decision = Decide(situation);// Abstract - varies by AI
 *           Act(decision);                   // Base - common action execution
 *       }
 *   }
 *
 * TRADE-OFFS:
 * - Rigidity: Template method enforces structure (sometimes too rigid)
 * - Inheritance Required: Must use inheritance (can't use composition)
 * - Limited Flexibility: Can't change algorithm structure without modifying base class
 * - Fragile Base Class: Changes to template method affect all subclasses
 * - Tight Coupling: Subclasses are coupled to abstract class structure
 *
 * VARIATIONS:
 * 1. All Abstract Operations: No base operations, all steps are abstract
 *    - Maximum flexibility
 *    - Less code reuse
 *
 * 2. All Base Operations with Hooks: Most operations implemented, few abstract
 *    - Maximum code reuse
 *    - Less customization
 *
 * 3. Factory Method as Step: Template uses factory method to create objects
 *    public abstract class Processor
 *    {
 *        public void Process()
 *        {
 *            var handler = CreateHandler(); // Factory method
 *            handler.Handle();
 *        }
 *        protected abstract Handler CreateHandler();
 *    }
 *
 * 4. Strategy Pattern Hybrid: Template uses strategy for some steps
 *    public abstract class Processor
 *    {
 *        private IStrategy _strategy;
 *        public void Process()
 *        {
 *            Load();
 *            _strategy.Execute(); // Strategy instead of abstract method
 *            Save();
 *        }
 *    }
 *
 * IMPLEMENTATION CONSIDERATIONS:
 * - Access Modifiers: Template method public, primitive operations protected
 * - Sealed Template: Consider sealing template method to prevent override
 * - Naming: Use descriptive names for primitive operations
 * - Documentation: Document the algorithm flow and purpose of each step
 * - Number of Steps: Balance between flexibility and complexity
 * - Hook Placement: Add hooks where extension is likely needed
 *
 * MODERN .NET USAGE:
 * - ASP.NET Core Middleware: Pipeline with delegates
 *   app.Use(async (context, next) =>
 *   {
 *       // Before
 *       await next();
 *       // After
 *   });
 *
 * - Entity Framework: DbContext SaveChanges
 *   public override int SaveChanges()
 *   {
 *       OnBeforeSaving();      // Hook
 *       var result = base.SaveChanges();
 *       OnAfterSaving();       // Hook
 *       return result;
 *   }
 *
 * - xUnit/NUnit Test Lifecycle:
 *   public class TestClass
 *   {
 *       [SetUp] public void Setup() { }      // Hook
 *       [Test] public void TestMethod() { }  // Abstract/required
 *       [TearDown] public void Cleanup() { } // Hook
 *   }
 *
 * COMPARISON WITH OTHER PATTERNS:
 * - Strategy: Template Method uses inheritance; Strategy uses composition
 * - Factory Method: Often used as a step within Template Method
 * - Command: Template Method defines algorithm; Command encapsulates request
 *
 * BEST PRACTICES:
 * 1. Keep template method simple: Just orchestrate, don't implement logic
 * 2. Use descriptive names: Name operations after what they do
 * 3. Document the algorithm: Explain the flow and purpose of each step
 * 4. Minimize abstract operations: Too many makes subclasses complex
 * 5. Provide sensible hooks: Add extension points where customization is likely
 * 6. Consider sealing template: Prevent template method override
 * 7. Use protected access: Primitive operations should be protected
 * 8. Avoid excessive nesting: Keep operations simple and focused
 */
