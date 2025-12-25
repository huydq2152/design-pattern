// The State pattern allows an object to alter its behavior when its internal state changes.
// The object will appear to change its class, promoting cleaner code by eliminating complex
// conditional logic and enabling state-specific behavior through polymorphism.

/// <summary>
/// The Context maintains a reference to the current State and delegates state-specific behavior.
///
/// KEY RESPONSIBILITIES:
/// - Holds reference to current state object
/// - Provides interface for clients to trigger operations
/// - Delegates all state-dependent operations to current state
/// - Allows state transitions initiated by states themselves
///
/// DESIGN DECISIONS:
/// - Context doesn't know about concrete state classes (except during initialization)
/// - State objects can trigger state transitions by calling Context.TransitionTo()
/// - Context provides public methods that delegate to state's handler methods
/// - Keeps client code simple - clients don't manage state transitions
///
/// CHARACTERISTICS:
/// - Maintains single current state at any time
/// - State transitions are explicit and traceable
/// - Context's behavior changes dynamically based on current state
/// - State logic is encapsulated within state classes
///
/// REAL-WORLD EXAMPLES:
/// - Document workflow (Draft -> Review -> Published)
/// - Connection state (Disconnected -> Connecting -> Connected -> Disconnected)
/// - Order status (Pending -> Processing -> Shipped -> Delivered)
/// - TCP connection states (Closed -> Listen -> Established -> etc.)
/// </summary>
public class Context
{
    /// <summary>
    /// Reference to the current state object.
    ///
    /// DESIGN NOTES:
    /// - Private field prevents external state manipulation
    /// - Changed only through TransitionTo method
    /// - Always points to a valid state object (never null after initialization)
    /// </summary>
    private State _state;

    /// <summary>
    /// Initializes the Context with an initial state.
    ///
    /// INITIALIZATION PATTERN:
    /// - Accepts initial state as constructor parameter
    /// - Immediately transitions to provided state (sets up bidirectional relationship)
    /// - Ensures context is never in an invalid state
    ///
    /// USAGE:
    /// - var context = new Context(new ConcreteStateA());
    /// - Context starts in specified state and is ready to use
    /// </summary>
    public Context(State state)
    {
        TransitionTo(state);
    }

    /// <summary>
    /// Transitions to a new state.
    ///
    /// STATE TRANSITION PATTERN:
    /// - Called by context during initialization
    /// - Called by state objects to trigger state changes
    /// - Logs the transition for traceability
    /// - Establishes bidirectional relationship (state knows its context)
    ///
    /// WORKFLOW:
    /// 1. Log the transition for debugging/auditing
    /// 2. Update current state reference
    /// 3. Call SetContext on new state (state gets reference to context)
    ///
    /// DESIGN CONSIDERATIONS:
    /// - Public method allows states to trigger transitions
    /// - Could be protected to restrict transitions to state objects only
    /// - Could add validation to prevent invalid transitions
    /// - Could implement state transition history for undo/debugging
    ///
    /// BENEFITS:
    /// - Centralized transition logic
    /// - Easy to add transition logging, validation, or events
    /// - States remain independent (don't hold context permanently)
    /// </summary>
    public void TransitionTo(State state)
    {
        Console.WriteLine($"Context: Transition to {state.GetType().Name}.");
        _state = state;
        _state.SetContext(this);
    }

    /// <summary>
    /// Handles Request1 by delegating to current state.
    ///
    /// REQUEST HANDLING:
    /// - Client calls this method regardless of current state
    /// - Context delegates to current state's Handle1 method
    /// - State object performs state-specific behavior
    /// - State may trigger transition to new state
    ///
    /// CLIENT PERSPECTIVE:
    /// - Client doesn't need to know which state is active
    /// - Same method call produces different behavior based on state
    /// - Eliminates client-side conditional logic
    ///
    /// POLYMORPHISM:
    /// - Each state implements Handle1 differently
    /// - Behavior changes automatically when state changes
    /// - No if/switch statements needed in context
    /// </summary>
    public void Request1()
    {
        _state.Handle1();
    }

    /// <summary>
    /// Handles Request2 by delegating to current state.
    ///
    /// PARALLEL REQUESTS:
    /// - Context can have multiple request methods
    /// - Each request is handled by current state
    /// - Different requests may trigger different state transitions
    /// - States control which requests trigger transitions
    ///
    /// DESIGN FLEXIBILITY:
    /// - Can add more request methods without changing state interface
    /// - Or add corresponding handlers to State base class
    /// - Allows fine-grained control over state behavior
    /// </summary>
    public void Request2()
    {
        _state.Handle2();
    }
}

/// <summary>
/// The State base class declares methods that all concrete states should implement.
///
/// KEY RESPONSIBILITIES:
/// - Defines interface for all concrete states
/// - Provides SetContext method for bidirectional relationship
/// - Declares abstract methods for state-specific behavior
/// - Stores reference to context (protected, accessible to subclasses)
///
/// DESIGN PATTERN:
/// - Abstract base class (could also be an interface)
/// - Template for concrete state implementations
/// - Provides shared infrastructure (context reference)
/// - Forces concrete states to implement all handlers
///
/// CHARACTERISTICS:
/// - Each state knows its context (can trigger transitions)
/// - States don't need to store context permanently (set when needed)
/// - Abstract methods ensure all states handle all requests
/// - Protected context field allows subclasses to access context
///
/// BENEFITS:
/// - Type safety: All states conform to same interface
/// - Polymorphism: Context can work with any state through base class
/// - Flexibility: Easy to add new states
/// - Encapsulation: Each state encapsulates its behavior
///
/// REAL-WORLD ANALOGS:
/// - State machine base state
/// - Workflow step base class
/// - Connection state base class
/// - Document status base class
/// </summary>
public abstract class State
{
    /// <summary>
    /// Reference to the context that owns this state.
    ///
    /// PROTECTED ACCESS:
    /// - Accessible to concrete state subclasses
    /// - Not exposed to external classes
    /// - Allows states to trigger transitions
    /// - Enables states to access context data if needed
    ///
    /// LIFECYCLE:
    /// - Set when state becomes active (via SetContext)
    /// - Used by state to trigger transitions
    /// - Could be used to access context's data or methods
    /// </summary>
    protected Context _context;

    /// <summary>
    /// Sets the context reference for this state.
    ///
    /// BIDIRECTIONAL RELATIONSHIP:
    /// - Context holds reference to current state
    /// - State holds reference to its context
    /// - Enables state to trigger transitions on context
    /// - Called by context when transitioning to this state
    ///
    /// DESIGN PATTERN:
    /// - Dependency injection pattern
    /// - Allows state to be created independently
    /// - Context injects itself when state becomes active
    /// - State doesn't need context in constructor
    ///
    /// BENEFITS:
    /// - States can be created without context
    /// - States can be reused across multiple contexts
    /// - Late binding of context reference
    /// - States can trigger transitions: _context.TransitionTo(new OtherState())
    /// </summary>
    public void SetContext(Context context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles Request1 in a state-specific manner.
    ///
    /// STATE-SPECIFIC BEHAVIOR:
    /// - Each concrete state implements this differently
    /// - Some states may transition to other states
    /// - Some states may perform actions without transitioning
    /// - Defines how this state responds to Request1
    ///
    /// ABSTRACT METHOD:
    /// - Forces all concrete states to provide implementation
    /// - Ensures context can call this method on any state
    /// - Enables polymorphic behavior
    /// </summary>
    public abstract void Handle1();

    /// <summary>
    /// Handles Request2 in a state-specific manner.
    ///
    /// MULTIPLE HANDLERS:
    /// - States can handle different types of requests
    /// - Each handler may have different transition logic
    /// - Allows fine-grained control over state behavior
    /// - Different handlers may transition to different states
    /// </summary>
    public abstract void Handle2();
}

/// <summary>
/// ConcreteStateA implements specific behavior for state A.
///
/// KEY CHARACTERISTICS:
/// - Represents one possible state of the context
/// - Implements state-specific logic for each handler
/// - Can transition to other states (like ConcreteStateB)
/// - Decides when to stay in current state vs transition
///
/// BEHAVIOR:
/// - Handle1: Performs work and transitions to StateB
/// - Handle2: Performs work without transitioning
///
/// STATE TRANSITIONS:
/// - Handle1 triggers transition to ConcreteStateB
/// - Handle2 remains in current state
/// - Demonstrates different transition patterns
///
/// DESIGN APPROACH:
/// - Encapsulates all StateA-specific behavior
/// - Self-contained logic (no dependencies on other states)
/// - Can create and transition to other states
/// - States control their own transition logic
///
/// REAL-WORLD EXAMPLES:
/// - Draft state (can transition to Review)
/// - Disconnected state (can transition to Connecting)
/// - Pending order (can transition to Processing)
/// </summary>
public class ConcreteStateA : State
{
    /// <summary>
    /// Handles Request1 in StateA by performing work and transitioning to StateB.
    ///
    /// STATE A BEHAVIOR:
    /// - Performs StateA-specific work
    /// - Logs the action and intent
    /// - Creates new StateB instance
    /// - Triggers transition to StateB
    ///
    /// TRANSITION PATTERN:
    /// - State creates next state object
    /// - State calls context.TransitionTo with new state
    /// - Context updates current state reference
    /// - New state becomes active
    ///
    /// WORKFLOW:
    /// 1. Log that StateA is handling request
    /// 2. Log that StateA wants to transition
    /// 3. Create new ConcreteStateB instance
    /// 4. Call _context.TransitionTo to change state
    ///
    /// DESIGN NOTES:
    /// - State determines when to transition (encapsulated logic)
    /// - State creates the next state (could use factory)
    /// - Transition is explicit and traceable through logging
    /// </summary>
    public override void Handle1()
    {
        Console.WriteLine("ConcreteStateA handles request1.");
        Console.WriteLine("ConcreteStateA wants to change the state of the context.");
        _context.TransitionTo(new ConcreteStateB());
    }

    /// <summary>
    /// Handles Request2 in StateA without transitioning.
    ///
    /// NON-TRANSITIONING HANDLER:
    /// - Performs state-specific work
    /// - Does NOT change state
    /// - Context remains in StateA
    /// - Demonstrates that not all handlers need to transition
    ///
    /// USE CASES:
    /// - State-specific operations that don't affect state
    /// - Query operations (get information from state)
    /// - Actions that can be performed multiple times in same state
    ///
    /// PATTERN:
    /// - Handle request
    /// - Perform work
    /// - Return control (state unchanged)
    /// </summary>
    public override void Handle2()
    {
        Console.WriteLine("ConcreteStateA handles request2.");
    }
}

/// <summary>
/// ConcreteStateB implements specific behavior for state B.
///
/// KEY CHARACTERISTICS:
/// - Represents second state in the state machine
/// - Different behavior than StateA
/// - Can transition back to StateA (demonstrates state cycling)
/// - Complements StateA to form a simple state machine
///
/// BEHAVIOR:
/// - Handle1: Performs work without transitioning
/// - Handle2: Performs work and transitions back to StateA
///
/// STATE MACHINE:
/// - StateA.Handle1 -> StateB
/// - StateB.Handle2 -> StateA
/// - Forms a cycle: A -> B -> A
/// - Demonstrates bidirectional state transitions
///
/// DESIGN SYMMETRY:
/// - Mirror image of StateA in some ways
/// - Different transition pattern (Handle2 transitions, Handle1 doesn't)
/// - Shows flexibility in when states transition
/// - States are independent but form coherent state machine
///
/// REAL-WORLD EXAMPLES:
/// - Review state (can transition to Published or back to Draft)
/// - Connected state (can transition to Disconnected)
/// - Processing order (can transition to Shipped)
/// </summary>
public class ConcreteStateB : State
{
    /// <summary>
    /// Handles Request1 in StateB without transitioning.
    ///
    /// STATEB BEHAVIOR:
    /// - Performs StateB-specific work
    /// - Does not change state
    /// - Remains in StateB
    /// - Demonstrates different handling than StateA for same request
    ///
    /// POLYMORPHISM:
    /// - Same method signature as StateA.Handle1
    /// - Completely different behavior
    /// - StateA transitions, StateB doesn't
    /// - Shows how state pattern enables different behaviors
    ///
    /// NOTE: Missing newline in WriteLine (Console.Write vs WriteLine)
    /// - Intentional in original code to show variation
    /// - Could be Console.WriteLine for consistency
    /// </summary>
    public override void Handle1()
    {
        Console.Write("ConcreteStateB handles request1.");
    }

    /// <summary>
    /// Handles Request2 in StateB by performing work and transitioning to StateA.
    ///
    /// STATEB TRANSITION:
    /// - Performs StateB-specific work
    /// - Logs the action and intent
    /// - Creates new StateA instance
    /// - Triggers transition back to StateA
    ///
    /// STATE CYCLING:
    /// - Transitions back to StateA
    /// - Creates potential cycle: A -> B -> A -> B...
    /// - Allows context to oscillate between states
    /// - Demonstrates bidirectional state machine
    ///
    /// WORKFLOW:
    /// 1. Log that StateB is handling request
    /// 2. Log that StateB wants to transition
    /// 3. Create new ConcreteStateA instance
    /// 4. Call _context.TransitionTo to change state
    ///
    /// PATTERN:
    /// - StateA and StateB can transition to each other
    /// - Different requests trigger different transitions
    /// - Forms a simple two-state state machine
    /// - Extensible: could add StateC, StateD, etc.
    /// </summary>
    public override void Handle2()
    {
        Console.WriteLine("ConcreteStateB handles request2.");
        Console.WriteLine("ConcreteStateB wants to change the state of the context.");
        _context.TransitionTo(new ConcreteStateA());
    }
}

/// <summary>
/// Program demonstrates the State pattern in action.
/// Shows how object behavior changes based on internal state without conditional logic.
/// </summary>
public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== State Pattern Demonstration ===\n");

        Console.WriteLine("--- Example 1: Creating Context with Initial State ---\n");

        // Create context starting in StateA
        // Context immediately transitions to StateA and sets up relationship
        var context = new Context(new ConcreteStateA());
        Console.WriteLine("Context created and initialized with ConcreteStateA.\n");

        Console.WriteLine("--- Example 2: Request1 in StateA ---\n");
        Console.WriteLine("Client calls Request1 (context is in StateA):");

        // Context delegates to StateA.Handle1()
        // StateA handles request and transitions to StateB
        context.Request1();
        // After this call, context is now in StateB
        Console.WriteLine();

        Console.WriteLine("--- Example 3: Request2 in StateB ---\n");
        Console.WriteLine("Client calls Request2 (context is now in StateB):");

        // Context delegates to StateB.Handle2()
        // StateB handles request and transitions back to StateA
        context.Request2();
        // After this call, context is back in StateA
        Console.WriteLine();

        Console.WriteLine("--- Key Observations ---");
        Console.WriteLine("1. Context behavior changes based on its state");
        Console.WriteLine("2. No conditional logic in context or client code");
        Console.WriteLine("3. States encapsulate state-specific behavior");
        Console.WriteLine("4. States control when transitions occur");
        Console.WriteLine("5. Easy to add new states without modifying existing code");
    }
}

/*
 * KEY TAKEAWAYS:
 *
 * PATTERN COMPONENTS:
 * 1. Context - Maintains current state, delegates operations to state
 * 2. State (abstract) - Defines interface for state-specific behavior
 * 3. ConcreteStateA/B - Implement specific behaviors for each state
 *
 * WHEN TO USE:
 * - Object behavior depends significantly on its state
 * - Complex conditional logic based on state
 * - State-specific behavior varies significantly
 * - Need to add new states without modifying existing code
 * - State transitions have clear rules and patterns
 *
 * BENEFITS:
 * 1. Single Responsibility: Each state class handles one state's behavior
 * 2. Open/Closed: Can add new states without modifying context
 * 3. Eliminates Conditionals: Replaces if/switch with polymorphism
 * 4. Explicit States: States are first-class objects
 * 5. State Transitions: Clear and traceable transitions
 * 6. Easy to Understand: State-specific logic is localized
 * 7. Maintainability: Changes to one state don't affect others
 * 8. Testability: Each state can be tested independently
 *
 * REAL-WORLD EXAMPLES:
 * - Document Workflow: Draft -> Review -> Published -> Archived
 *   public class Document
 *   {
 *       private DocumentState _state;
 *       public void Submit() => _state.Submit(this);
 *       public void Approve() => _state.Approve(this);
 *   }
 *
 * - TCP Connection States: Closed -> Listen -> SynSent -> Established
 *   public class TcpConnection
 *   {
 *       private TcpState _state;
 *       public void Open() => _state.Open(this);
 *       public void Close() => _state.Close(this);
 *   }
 *
 * - Media Player: Stopped -> Playing -> Paused
 *   public class MediaPlayer
 *   {
 *       private PlayerState _state;
 *       public void Play() => _state.Play(this);
 *       public void Pause() => _state.Pause(this);
 *       public void Stop() => _state.Stop(this);
 *   }
 *
 * - Order Processing: Pending -> Validated -> Paid -> Shipped -> Delivered
 *   public class Order
 *   {
 *       private OrderState _state;
 *       public void Validate() => _state.Validate(this);
 *       public void Pay() => _state.Pay(this);
 *       public void Ship() => _state.Ship(this);
 *   }
 *
 * - Vending Machine: Idle -> HasMoney -> Dispensing -> OutOfStock
 *   public class VendingMachine
 *   {
 *       private MachineState _state;
 *       public void InsertMoney() => _state.InsertMoney(this);
 *       public void SelectProduct() => _state.SelectProduct(this);
 *   }
 *
 * - Authentication: LoggedOut -> LoggingIn -> LoggedIn -> SessionExpired
 *   public class AuthContext
 *   {
 *       private AuthState _state;
 *       public void Login() => _state.Login(this);
 *       public void Logout() => _state.Logout(this);
 *   }
 *
 * TRADE-OFFS:
 * - More Classes: Each state is a separate class
 * - State Object Creation: Creating new state objects on transitions (could use flyweight)
 * - Complexity for Simple Cases: Overkill if only 2-3 states with simple logic
 * - Shared State: Need to pass context data to states if they need it
 *
 * VARIATIONS:
 * 1. Singleton States: Reuse state instances instead of creating new ones
 *    private static readonly StateA _instance = new StateA();
 *
 * 2. State with Data: States can hold state-specific data
 *    public class PlayingState : State
 *    {
 *        private TimeSpan _position;
 *    }
 *
 * 3. Hierarchical States: States can have sub-states
 *    public abstract class ActiveState : State { }
 *    public class PlayingState : ActiveState { }
 *    public class PausedState : ActiveState { }
 *
 * 4. State Transition Guards: Validate transitions before allowing them
 *    public void TransitionTo(State newState)
 *    {
 *        if (_state.CanTransitionTo(newState))
 *            _state = newState;
 *    }
 *
 * IMPLEMENTATION CONSIDERATIONS:
 * - Who Creates States: States create next state, or context/factory creates them
 * - State Instance Reuse: Create new instances vs reuse (Flyweight pattern)
 * - Context Data Access: How much context data should states access
 * - Transition Validation: Allow any transition or enforce valid transitions only
 * - State History: Track state history for debugging or undo
 * - Thread Safety: Consider if context will be used concurrently
 *
 * MODERN .NET USAGE:
 * - ASP.NET Core: Request processing pipeline states
 * - Entity Framework: Entity state tracking (Added, Modified, Deleted, Unchanged)
 * - SignalR: Connection states (Connecting, Connected, Reconnecting, Disconnected)
 * - Workflow Foundation: Workflow states and activities
 *
 * COMPARISON WITH OTHER PATTERNS:
 * - Strategy: Both use composition, but State allows object to change strategy based on state
 * - State Machine Libraries: State pattern is manual implementation of state machine
 * - Chain of Responsibility: Chain passes request along; State delegates to current state
 *
 * BEST PRACTICES:
 * 1. Keep states focused: Each state handles one state's behavior
 * 2. Use abstract base class: Provides common infrastructure for states
 * 3. Consider Flyweight: Reuse state objects if they're stateless
 * 4. Document transitions: Make state machine diagram explicit
 * 5. Validate transitions: Prevent invalid state transitions
 * 6. Use enums with states: public enum StateType { A, B, C } for clarity
 * 7. Log transitions: Track state changes for debugging
 * 8. Consider state machines: Use library like Stateless for complex state machines
 */
