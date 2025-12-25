// The Command pattern encapsulates a request as an object, allowing you to parameterize clients
// with different requests, queue requests, log requests, and support undoable operations.
// This conceptual example demonstrates the core structure and interactions of the pattern.

/// <summary>
/// The Receiver class contains the actual business logic to perform operations.
///
/// KEY RESPONSIBILITIES:
/// - Contains the actual business logic that performs work
/// - Knows how to carry out operations associated with a request
/// - Can be any class with methods that perform actions
/// - Has no knowledge of Command objects or Invokers
///
/// DESIGN DECISIONS:
/// - Focused on domain logic, not command orchestration
/// - Methods can accept parameters for context-specific operations
/// - Can have multiple methods for different operations
/// - Independent and reusable outside the command pattern
///
/// REAL-WORLD EXAMPLES:
/// - Document class with Cut(), Copy(), Paste(), Save() methods
/// - EmailService with SendEmail(), ScheduleEmail() methods
/// - DatabaseContext with Insert(), Update(), Delete() methods
/// - Player class with Move(), Jump(), Attack() methods (game development)
///
/// CHARACTERISTICS:
/// - Single Responsibility: Focused on business operations
/// - Reusable: Can be called directly or through commands
/// - Testable: Business logic can be tested independently
/// - No coupling: Doesn't depend on command infrastructure
/// </summary>
public class Receiver
{
    /// <summary>
    /// Performs operation A with the provided parameter.
    /// This represents a simple business operation that the receiver can perform.
    ///
    /// In real applications, this might be:
    /// - SaveDocument(document)
    /// - SendEmail(emailMessage)
    /// - ProcessPayment(paymentInfo)
    /// </summary>
    public void DoA(string a)
    {
        Console.WriteLine($"Receiver: Working on ({a}.)");
    }

    /// <summary>
    /// Performs operation B with the provided parameter.
    /// Demonstrates that receivers can have multiple operations.
    /// Commands can invoke one or multiple receiver methods.
    ///
    /// In real applications, this might be:
    /// - GenerateReport(reportType)
    /// - UpdateInventory(itemId)
    /// - NotifyUsers(message)
    /// </summary>
    public void DoB(string b)
    {
        Console.WriteLine($"Receiver: Working on ({b}.)");
    }
}

/// <summary>
/// The ICommand interface declares the execution method for all commands.
///
/// KEY PRINCIPLES:
/// - All concrete commands must implement Execute()
/// - Provides uniform interface for all commands
/// - Allows commands to be used interchangeably (polymorphism)
/// - Can be extended with Undo() for reversible operations
///
/// DESIGN BENEFITS:
/// - Invoker depends on interface, not concrete commands
/// - New commands can be added without changing existing code (Open/Closed)
/// - Commands can be stored, logged, queued, or serialized
/// - Enables command composition and macro commands
///
/// EXTENSIONS:
/// - void Undo(): For reversible commands (undo/redo functionality)
/// - bool CanExecute(): For conditional execution
/// - Task ExecuteAsync(): For asynchronous operations
/// - string Description { get; }: For command metadata
///
/// REAL-WORLD EXAMPLES:
/// - IRequest in MediatR library
/// - ICommand in CQRS pattern
/// - Action delegates in .NET (Func, Action)
/// - ICommand in WPF/MVVM pattern
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Executes the command.
    ///
    /// This is the core method that:
    /// - Performs the requested operation
    /// - May delegate to a Receiver for complex logic
    /// - Can store state for undo operations
    /// - Can be called by Invoker or client code
    ///
    /// EXECUTION PATTERNS:
    /// - Direct: Command performs operation itself (SimpleCommand)
    /// - Delegation: Command delegates to Receiver (ComplexCommand)
    /// - Composite: Command executes multiple sub-commands (MacroCommand)
    /// </summary>
    void Execute();
}

/// <summary>
/// SimpleCommand demonstrates a command that performs work directly without a Receiver.
///
/// KEY CHARACTERISTICS:
/// - Self-contained: Doesn't delegate to a Receiver
/// - Encapsulates all data needed for execution (payload)
/// - Useful for simple operations that don't require complex business logic
/// - Immutable: State is set at construction and doesn't change
///
/// USE CASES:
/// - Simple logging or notification operations
/// - Setting configuration values
/// - Triggering simple events
/// - Operations that don't require domain objects
///
/// DESIGN PATTERN:
/// - Command as data holder: Encapsulates operation parameters
/// - Self-executing: Contains its own execution logic
/// - Stateless (after construction): No mutable state
///
/// REAL-WORLD EXAMPLES:
/// - ShowMessageCommand("Hello") - displays a message
/// - LogCommand("User logged in") - writes to log
/// - NotifyCommand("Processing complete") - sends notification
/// - SetValueCommand(key, value) - updates a setting
/// </summary>
public class SimpleCommand : ICommand
{
    private readonly string _payload;

    /// <summary>
    /// Initializes the command with data needed for execution.
    ///
    /// DESIGN APPROACH:
    /// - Accept all necessary data via constructor
    /// - Store as readonly fields for immutability
    /// - No dependencies on external services for simple commands
    ///
    /// This makes commands:
    /// - Immutable: State can't change after creation
    /// - Serializable: Can be easily serialized for queuing or logging
    /// - Thread-safe: No mutable state to protect
    /// </summary>
    public SimpleCommand(string payload)
    {
        _payload = payload;
    }

    /// <summary>
    /// Executes the simple command using the encapsulated payload.
    ///
    /// EXECUTION APPROACH:
    /// - Uses only the data provided at construction
    /// - Performs operation directly without delegating
    /// - Suitable for simple, self-contained operations
    ///
    /// COMPARISON WITH COMPLEXCOMMAND:
    /// - SimpleCommand: Self-executing, no Receiver needed
    /// - ComplexCommand: Delegates to Receiver for business logic
    /// </summary>
    public void Execute()
    {
        Console.WriteLine($"SimpleCommand: See, I can do simple things like printing ({_payload})");
    }
}

/// <summary>
/// ComplexCommand demonstrates a command that delegates work to a Receiver.
///
/// KEY CHARACTERISTICS:
/// - Delegates to Receiver: Doesn't contain business logic itself
/// - Acts as bridge: Connects Invoker with Receiver
/// - Context holder: Stores parameters needed for receiver methods
/// - Separation of concerns: Command handles invocation, Receiver handles logic
///
/// DESIGN PATTERN:
/// - Command as adapter: Adapts Receiver's interface for command pattern
/// - Dependency injection: Receiver is injected via constructor
/// - Encapsulation: Hides receiver complexity from invoker
///
/// WHY USE RECEIVER:
/// - Business logic is complex and belongs in domain objects
/// - Receiver might be used by multiple commands
/// - Receiver has its own lifecycle and dependencies
/// - Need to keep command lightweight and focused
///
/// REAL-WORLD EXAMPLES:
/// - SaveDocumentCommand(document, documentService) - delegates to DocumentService
/// - SendEmailCommand(email, emailService) - delegates to EmailService
/// - ProcessOrderCommand(order, orderProcessor) - delegates to OrderProcessor
/// - BackupDatabaseCommand(config, backupService) - delegates to BackupService
///
/// CHARACTERISTICS:
/// - Command knows what to do, Receiver knows how to do it
/// - Command can coordinate multiple receiver operations
/// - Enables undo by storing receiver state or using compensating operations
/// </summary>
public class ComplexCommand : ICommand
{
    private readonly Receiver _receiver;
    private readonly string _a;
    private readonly string _b;

    /// <summary>
    /// Initializes the command with a Receiver and operation parameters.
    ///
    /// CONSTRUCTOR INJECTION:
    /// - Receiver: The object that will perform the actual work
    /// - Parameters (a, b): Context-specific data for the operations
    ///
    /// DESIGN DECISIONS:
    /// - Receiver dependency makes this command flexible and reusable
    /// - Parameters are stored for later execution (lazy execution)
    /// - All dependencies provided upfront (explicit dependencies)
    ///
    /// EXECUTION TIMING:
    /// - Construction: Command is created with all needed data
    /// - Execution: Command is executed later by Invoker
    /// - This separation enables queuing, scheduling, and logging
    ///
    /// ALTERNATIVE APPROACHES:
    /// - Could use dependency injection framework for Receiver
    /// - Could use factory pattern to create Receiver internally
    /// - Could accept Action delegates instead of Receiver instance
    /// </summary>
    public ComplexCommand(Receiver receiver, string a, string b)
    {
        _receiver = receiver;
        _a = a;
        _b = b;
    }

    /// <summary>
    /// Executes the command by delegating to the Receiver.
    ///
    /// DELEGATION PATTERN:
    /// - Command orchestrates which receiver methods to call
    /// - Command provides parameters to receiver methods
    /// - Receiver performs the actual business logic
    ///
    /// CHARACTERISTICS:
    /// - Command can call multiple receiver methods (as shown)
    /// - Command defines the execution order
    /// - Command can add pre/post-processing logic
    /// - Receiver remains independent of command pattern
    ///
    /// EXTENSIBILITY:
    /// - Could add error handling around receiver calls
    /// - Could add logging before/after receiver operations
    /// - Could store receiver state for undo functionality
    /// - Could validate parameters before calling receiver
    ///
    /// REAL-WORLD EXAMPLE:
    /// ProcessOrderCommand might:
    /// 1. Call ValidateOrder(order)
    /// 2. Call ReserveInventory(items)
    /// 3. Call ProcessPayment(payment)
    /// 4. Call SendConfirmation(customer)
    /// </summary>
    public void Execute()
    {
        Console.WriteLine("ComplexCommand: Complex stuff should be done by a receiver object.");
        // Command orchestrates the workflow by calling receiver methods
        _receiver.DoA(_a);
        _receiver.DoB(_b);
    }
}

/// <summary>
/// The Invoker is responsible for initiating command execution.
///
/// KEY RESPONSIBILITIES:
/// - Stores references to command objects
/// - Initiates command execution by calling Execute()
/// - Decoupled from concrete commands and receivers
/// - Can maintain command history for undo/redo
///
/// DESIGN DECISIONS:
/// - Depends only on ICommand interface (Dependency Inversion)
/// - Doesn't know about receivers or command implementations
/// - Can be configured with different commands at runtime
/// - Can execute commands at appropriate times
///
/// INVOKER PATTERNS:
/// - Immediate execution: Execute command as soon as set
/// - Deferred execution: Store commands and execute later (shown here)
/// - Queue execution: Maintain queue of commands to execute
/// - Scheduled execution: Execute commands at specific times
/// - Conditional execution: Execute based on conditions
///
/// REAL-WORLD EXAMPLES:
/// - Button in UI: Executes command when clicked
/// - Menu item: Executes command when selected
/// - Scheduler: Executes commands at scheduled times
/// - Transaction manager: Executes commands within transaction
/// - Macro recorder: Records and replays sequences of commands
///
/// CHARACTERISTICS:
/// - Can store multiple commands (as shown with onStart and onFinish)
/// - Can execute commands in specific order or based on events
/// - Can maintain command history for undo functionality
/// - Completely decoupled from command implementation details
/// </summary>
public class Invoker
{
    private ICommand _onStart;

    private ICommand _onFinish;

    /// <summary>
    /// Sets the command to execute at the start of the operation.
    ///
    /// CONFIGURATION PATTERN:
    /// - Invoker is configured with commands before execution
    /// - Commands can be changed dynamically
    /// - Enables different behaviors without changing Invoker code
    ///
    /// This is an example of the Strategy pattern applied to commands.
    /// </summary>
    public void SetOnStart(ICommand command)
    {
        _onStart = command;
    }

    /// <summary>
    /// Sets the command to execute at the end of the operation.
    ///
    /// HOOK PATTERN:
    /// - Provides extension points (hooks) for custom behavior
    /// - onStart and onFinish act as before/after hooks
    /// - Similar to middleware or event handlers
    ///
    /// Real-world analogs:
    /// - BeforeCommit/AfterCommit in transactions
    /// - OnOpen/OnClose in resource management
    /// - PreProcess/PostProcess in pipelines
    /// </summary>
    public void SetOnFinish(ICommand command)
    {
        _onFinish = command;
    }

    /// <summary>
    /// Performs the main operation, executing configured commands at appropriate points.
    ///
    /// TEMPLATE METHOD PATTERN:
    /// - Defines the skeleton of an algorithm (do something important)
    /// - Delegates specific steps to commands (extension points)
    /// - Fixed structure with variable behavior
    ///
    /// EXECUTION FLOW:
    /// 1. Check if onStart command is set
    /// 2. Execute onStart command (optional pre-processing)
    /// 3. Perform core operation
    /// 4. Check if onFinish command is set
    /// 5. Execute onFinish command (optional post-processing)
    ///
    /// BENEFITS:
    /// - Invoker's main logic remains unchanged
    /// - Behavior is customized through command injection
    /// - Commands can be added, removed, or changed without modifying Invoker
    /// - Supports Open/Closed Principle
    ///
    /// REAL-WORLD EXAMPLES:
    /// - Application startup: Execute initialization commands
    /// - Request processing: Execute before/after request commands
    /// - Batch job: Execute pre-processing and post-processing commands
    /// - Game loop: Execute commands at specific game events
    ///
    /// NULL CHECKING:
    /// - Uses pattern matching (is ICommand) to check for null
    /// - Could use null-conditional operator (_onStart?.Execute())
    /// - Could use null object pattern with NullCommand implementation
    /// </summary>
    public void DoSomethingImportant()
    {
        Console.WriteLine("Invoker: Does anybody want something done before I begin?");
        if (_onStart is ICommand)
        {
            // Execute pre-processing command
            _onStart.Execute();
        }

        Console.WriteLine("Invoker: ...doing something really important...");
        // This is where the invoker's core business logic would go
        // Commands provide extension points before and after this logic

        Console.WriteLine("Invoker: Does anybody want something done after I finish?");
        if (_onFinish is ICommand)
        {
            // Execute post-processing command
            _onFinish.Execute();
        }
    }
}

/// <summary>
/// Program demonstrates the Command pattern in action.
/// Shows how commands encapsulate requests and enable flexible request handling.
/// </summary>
public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Command Pattern Demonstration ===\n");

        Console.WriteLine("--- Example 1: Configuring Invoker with Different Commands ---\n");

        // The client code creates and configures commands
        // Client decides WHAT to execute, Invoker decides WHEN to execute
        Invoker invoker = new Invoker();

        // Configure simple command for startup
        // SimpleCommand doesn't need a receiver - it's self-contained
        invoker.SetOnStart(new SimpleCommand("Say Hi!"));

        // Create receiver for complex command
        Receiver receiver = new Receiver();

        // Configure complex command for finish
        // ComplexCommand delegates to receiver for actual work
        invoker.SetOnFinish(new ComplexCommand(receiver, "Send email", "Save report"));

        // Invoker executes commands at appropriate times
        // Client doesn't need to know when or how commands are executed
        invoker.DoSomethingImportant();

        Console.WriteLine();

        Console.WriteLine("--- Example 2: Changing Command Behavior at Runtime ---\n");

        // Commands can be changed dynamically
        invoker.SetOnStart(new SimpleCommand("Starting second operation"));
        invoker.SetOnFinish(new ComplexCommand(receiver, "Archive data", "Send notification"));

        invoker.DoSomethingImportant();

        Console.WriteLine();

        Console.WriteLine("--- Example 3: Direct Command Execution ---\n");

        // Commands can be executed directly without an Invoker
        // This shows command flexibility - they're standalone objects
        ICommand directCommand = new SimpleCommand("Direct execution");
        Console.WriteLine("Executing command directly:");
        directCommand.Execute();

        Console.WriteLine();

        Console.WriteLine("--- Real-World Use Cases ---");
        Console.WriteLine("1. Undo/Redo: Store commands in stack for undo functionality");
        Console.WriteLine("2. Macro Commands: Combine multiple commands into one");
        Console.WriteLine("3. Queuing: Queue commands for asynchronous execution");
        Console.WriteLine("4. Logging: Log commands before execution for audit trail");
        Console.WriteLine("5. Transaction: Group commands and commit/rollback together");
        Console.WriteLine("6. Remote Execution: Serialize and send commands over network");
        Console.WriteLine("7. UI Actions: Button clicks, menu items trigger commands");
        Console.WriteLine("8. CQRS: Separate command objects for write operations");
    }
}

/*
 * KEY TAKEAWAYS:
 *
 * PATTERN COMPONENTS:
 * 1. ICommand - Interface declaring Execute() method
 * 2. ConcreteCommand (Simple/Complex) - Implements Execute(), may delegate to Receiver
 * 3. Receiver - Contains actual business logic for operations
 * 4. Invoker - Stores and executes commands without knowing implementation details
 * 5. Client - Creates commands and configures invoker
 *
 * WHEN TO USE:
 * - Need to parameterize objects with operations (configurable behavior)
 * - Need to queue operations, schedule execution, or execute remotely
 * - Need to support undo/redo functionality
 * - Need to log changes or maintain audit trail of operations
 * - Need to structure system around high-level operations
 * - Want to decouple object that invokes operation from object that knows how to perform it
 *
 * BENEFITS:
 * 1. Single Responsibility: Separates invocation from execution
 * 2. Open/Closed: Can add new commands without changing existing code
 * 3. Undo/Redo: Can implement by storing command state
 * 4. Deferred Execution: Create command now, execute later
 * 5. Composite Commands: Build complex commands from simple ones (macro commands)
 * 6. Logging and Auditing: Easy to log operations as command objects
 * 7. Queueing: Commands are objects, can be queued for execution
 * 8. Serialization: Commands can be serialized for remote execution or persistence
 *
 * REAL-WORLD EXAMPLES IN .NET:
 * - ICommand in WPF/MVVM: Button commands, menu commands
 * - IRequest in MediatR: CQRS command handling
 * - Action/Func delegates: Lightweight command pattern
 * - Task and async/await: Deferred command execution
 * - IDbCommand in ADO.NET: Database command pattern
 * - Middleware pipeline: Chain of command-like handlers
 * - Event handlers: Commands triggered by events
 * - ASP.NET Core endpoint handlers: Request commands
 *
 * COMMAND PATTERN VARIATIONS:
 * 1. Simple Command: Self-executing, no receiver
 * 2. Complex Command: Delegates to receiver
 * 3. Macro Command: Executes sequence of commands
 * 4. Undoable Command: Implements Undo() method
 * 5. Transactional Command: Supports commit/rollback
 * 6. Asynchronous Command: Returns Task for async execution
 * 7. Parameterized Command: Uses generics for type-safe parameters
 *
 * TRADE-OFFS:
 * - More classes: Each operation becomes a separate command class
 * - Indirection: Extra layer between invoker and receiver
 * - Complexity: May be overkill for simple operations
 * - Memory: Storing command history uses memory (for undo/redo)
 * - Learning curve: Team needs to understand the abstraction
 *
 * COMPARISON WITH OTHER PATTERNS:
 * - Strategy: Encapsulates algorithms; Command: Encapsulates requests
 * - Memento: Stores object state; Command: Stores operations
 * - Chain of Responsibility: Passes request along chain; Command: Direct execution
 * - Observer: One-to-many notification; Command: One-to-one execution
 *
 * MODERN .NET USAGE:
 * - MediatR library for CQRS: Command/query separation
 * - WPF/XAML ICommand: UI actions bound to commands
 * - ASP.NET Core endpoint handlers: HTTP commands
 * - Azure Functions: Event-driven commands
 * - Action delegates: Lightweight commands (Action, Func<T>)
 * - Task-based operations: Deferred command execution
 *
 * UNDO/REDO IMPLEMENTATION:
 * - Add Undo() method to ICommand interface
 * - Store executed commands in a stack (command history)
 * - Undo: Pop command from history and call Undo()
 * - Redo: Keep separate stack of undone commands
 * - Store state needed to reverse operation (Memento pattern)
 *
 * BEST PRACTICES:
 * 1. Keep commands focused: One command = one operation
 * 2. Make commands immutable: Set all state in constructor
 * 3. Separate simple and complex: Use Receiver for complex logic
 * 4. Use interfaces: Enable polymorphism and testability
 * 5. Consider async: Use Task for long-running commands
 * 6. Add CanExecute: Check if command can be executed
 * 7. Log commands: Easy audit trail and debugging
 * 8. Use DI: Inject dependencies into commands
 * 9. Keep invoker generic: Invoker shouldn't know about specific commands
 * 10. Document side effects: Make command behavior clear
 */
