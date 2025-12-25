// The Mediator pattern defines an object that encapsulates how a set of objects interact.
// It promotes loose coupling by keeping objects from referring to each other explicitly,
// and it lets you vary their interaction independently.

/// <summary>
/// The IMediator interface declares the method used by components to notify the mediator about events.
///
/// KEY PRINCIPLES:
/// - Defines communication contract between mediator and components
/// - Centralizes complex communications and control logic
/// - Components don't communicate directly with each other
/// - All communication flows through the mediator
///
/// DESIGN BENEFITS:
/// - Reduces coupling between components (they only know the mediator)
/// - Centralizes control logic in one place
/// - Makes component interactions explicit and traceable
/// - Easy to understand the flow of communication
///
/// REAL-WORLD EXAMPLES:
/// - Air traffic control system (planes don't talk to each other)
/// - Chat room mediator (users communicate through chat room)
/// - MediatR library in .NET (CQRS mediator pattern)
/// - UI dialog coordinator (manages interactions between form controls)
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Notifies the mediator about an event from a component.
    ///
    /// PARAMETERS:
    /// - sender: The component that triggered the event
    /// - events: String identifier for the event type
    ///
    /// DESIGN APPROACH:
    /// - Event-driven communication pattern
    /// - Mediator decides how to react to each event
    /// - Can trigger operations on other components
    /// - Sender doesn't know what happens next
    ///
    /// ALTERNATIVE DESIGNS:
    /// - Could use strongly-typed events instead of strings
    /// - Could use event aggregator pattern with delegates
    /// - Could use message objects for complex event data
    /// </summary>
    void Notify(object sender, string events);
}

/// <summary>
/// ConcreteMediator implements coordination logic by orchestrating component interactions.
///
/// KEY RESPONSIBILITIES:
/// - Knows about all components in the system
/// - Coordinates communication between components
/// - Implements workflow logic (when X happens, do Y and Z)
/// - Keeps components decoupled from each other
///
/// DESIGN DECISIONS:
/// - Holds references to all components it coordinates
/// - Sets itself as the mediator for each component
/// - Contains all the interaction logic in one place
/// - Components remain independent and reusable
///
/// CHARACTERISTICS:
/// - Single point of coordination (centralized control)
/// - Easy to modify interaction logic without changing components
/// - Can become complex if managing many components
/// - Violates Open/Closed if event types change frequently
///
/// REAL-WORLD ANALOGS:
/// - Air traffic controller (coordinates planes)
/// - Chat room server (coordinates users)
/// - Event bus (coordinates events between modules)
/// - Workflow orchestrator (coordinates business process steps)
/// </summary>
public class ConcreteMediator : IMediator
{
    private readonly Component1 _component1;
    private readonly Component2 _component2;

    /// <summary>
    /// Initializes the mediator and registers itself with all components.
    ///
    /// INITIALIZATION PATTERN:
    /// - Accepts component instances via constructor
    /// - Calls SetMediator on each component to establish relationship
    /// - Creates bidirectional relationship (mediator knows components, components know mediator)
    ///
    /// DESIGN CONSIDERATIONS:
    /// - Components are created before mediator
    /// - Mediator wires up the relationships
    /// - Alternative: Components could register themselves with mediator
    /// - Alternative: Use dependency injection container for setup
    ///
    /// COUPLING:
    /// - Mediator is coupled to concrete components (knows their types)
    /// - Components are coupled only to IMediator interface
    /// - This asymmetry is intentional and acceptable
    /// </summary>
    public ConcreteMediator(Component1 component1, Component2 component2)
    {
        _component1 = component1;
        _component1.SetMediator(this);
        _component2 = component2;
        _component2.SetMediator(this);
    }

    /// <summary>
    /// Handles component notifications and orchestrates responses.
    ///
    /// COORDINATION LOGIC:
    /// - Event "A": Component1 triggered something, tell Component2 to do C
    /// - Event "D": Component2 triggered something, tell Component1 to do B and Component2 to do C
    ///
    /// WORKFLOW PATTERNS:
    /// - One-to-one: Single event triggers single action
    /// - One-to-many: Single event triggers multiple actions
    /// - Can implement complex workflows and business rules
    ///
    /// DESIGN CONSIDERATIONS:
    /// - String-based event matching (simple but not type-safe)
    /// - Could use switch expression for cleaner syntax
    /// - Could use strategy pattern for complex event handling
    /// - Could use event sourcing for audit trail
    ///
    /// BENEFITS:
    /// - All interaction logic in one place
    /// - Easy to understand the flow
    /// - Can modify workflow without changing components
    ///
    /// TRADE-OFFS:
    /// - Can become a "god object" if too many events
    /// - Not type-safe with string events
    /// - Need to modify mediator when adding new event types
    /// </summary>
    public void Notify(object sender, string ev)
    {
        // React to event A: Component1 did something, trigger Component2
        if (ev == "A")
        {
            Console.WriteLine("Mediator reacts on A and triggers following operations:");
            _component2.DoC();
        }

        // React to event D: Component2 did something, trigger multiple components
        if (ev == "D")
        {
            Console.WriteLine("Mediator reacts on D and triggers following operations:");
            _component1.DoB();
            _component2.DoC();
        }

        // Other events are ignored (no explicit handling)
        // Could log unhandled events for debugging
        // Could have default behavior for unknown events
    }
}

/// <summary>
/// BaseComponent provides common infrastructure for all components that use a mediator.
///
/// KEY RESPONSIBILITIES:
/// - Stores reference to the mediator
/// - Provides SetMediator method for mediator registration
/// - Serves as base class for concrete components
///
/// DESIGN PATTERN:
/// - Template pattern foundation for component behavior
/// - Dependency Injection support through SetMediator
/// - Protected mediator field accessible to subclasses
///
/// CHARACTERISTICS:
/// - Components are loosely coupled to each other
/// - Components are coupled to IMediator interface (acceptable)
/// - Subclasses can notify mediator about events
/// - No knowledge of other components
///
/// BENEFITS:
/// - Reusable base for all mediator-aware components
/// - Reduces boilerplate in concrete components
/// - Provides consistent mediator access pattern
///
/// REAL-WORLD ANALOGS:
/// - Form control base class (knows about parent form)
/// - Actor base class in actor model (knows about actor system)
/// - Widget base class in UI framework (knows about event bus)
/// </summary>
public class BaseComponent
{
    protected IMediator? _mediator;

    /// <summary>
    /// Initializes component with optional mediator reference.
    ///
    /// FLEXIBILITY:
    /// - Mediator can be set via constructor (dependency injection)
    /// - Mediator can be null initially and set later
    /// - Allows component creation without mediator
    ///
    /// USE CASES:
    /// - Testing: Create component without mediator
    /// - Reusability: Use component in non-mediated contexts
    /// - Initialization: Create component before mediator exists
    /// </summary>
    protected BaseComponent(IMediator? mediator = null)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Sets or updates the mediator reference.
    ///
    /// SETTER PATTERN:
    /// - Allows mediator to register itself with component
    /// - Enables late binding of mediator
    /// - Can change mediator at runtime (rare but possible)
    ///
    /// USAGE:
    /// - Called by mediator during initialization
    /// - Can be called to swap mediators (runtime reconfiguration)
    /// - Components can be moved between mediators
    ///
    /// DESIGN NOTE:
    /// - Could be property instead of method
    /// - Method makes the intent clearer (explicit action)
    /// - Could validate that mediator is not null
    /// </summary>
    public void SetMediator(IMediator mediator)
    {
        _mediator = mediator;
    }
}

/// <summary>
/// Component1 is a concrete component that performs operations and notifies the mediator.
///
/// KEY CHARACTERISTICS:
/// - Independent business logic (DoA, DoB methods)
/// - Notifies mediator after performing operations
/// - No knowledge of Component2 or other components
/// - Can be reused in different contexts
///
/// DESIGN APPROACH:
/// - Performs its own work first
/// - Notifies mediator about what happened
/// - Mediator decides what to do next
/// - Component doesn't care about downstream effects
///
/// BENEFITS:
/// - Single Responsibility: Focused on its own operations
/// - Open/Closed: Can add new operations without changing mediator
/// - Liskov Substitution: Can be replaced by other components
/// - Reusability: Can work without mediator or with different mediators
///
/// REAL-WORLD EXAMPLES:
/// - Submit button (performs submit, notifies form)
/// - Text field (validates input, notifies form controller)
/// - Service method (performs work, notifies event bus)
/// </summary>
public class Component1 : BaseComponent
{
    /// <summary>
    /// Performs operation A and notifies the mediator.
    ///
    /// OPERATION PATTERN:
    /// 1. Perform local work
    /// 2. Notify mediator about the event
    /// 3. Mediator coordinates any follow-up actions
    ///
    /// NOTIFICATION:
    /// - Passes "this" as sender (mediator knows who triggered event)
    /// - Passes event name "A" (identifies what happened)
    /// - Doesn't specify what should happen next
    ///
    /// DECOUPLING:
    /// - Component doesn't know about Component2
    /// - Component doesn't know what mediator will do
    /// - Mediator has full control over workflow
    /// </summary>
    public void DoA()
    {
        Console.WriteLine("Component 1 does A.");

        // Notify mediator about event A
        // Mediator will decide if and how to respond
        _mediator?.Notify(this, "A");
    }

    /// <summary>
    /// Performs operation B and notifies the mediator.
    ///
    /// TRIGGERED BY MEDIATOR:
    /// - Often called by mediator in response to other events
    /// - Can also be called directly by client code
    /// - Still notifies mediator (could trigger cascade of events)
    ///
    /// EVENT CASCADING:
    /// - Event B notification could trigger more events
    /// - Mediator controls whether cascading happens
    /// - Need to avoid infinite loops in mediator logic
    /// </summary>
    public void DoB()
    {
        Console.WriteLine("Component 1 does B.");

        _mediator?.Notify(this, "B");
    }
}

/// <summary>
/// Component2 is another concrete component with different operations.
///
/// INDEPENDENCE:
/// - Completely independent of Component1
/// - Same pattern: do work, notify mediator
/// - Can be combined with Component1 via mediator
/// - Can be used in isolation or with different components
///
/// SYMMETRY:
/// - Same structure as Component1
/// - Different operations (DoC, DoD)
/// - Same communication pattern with mediator
/// - Demonstrates component interchangeability
///
/// FLEXIBILITY:
/// - Could have different number of operations
/// - Could have different notification patterns
/// - Could have state that influences behavior
/// - Mediator adapts to each component's events
/// </summary>
public class Component2 : BaseComponent
{
    /// <summary>
    /// Performs operation C and notifies the mediator.
    ///
    /// COMMON OPERATION:
    /// - Called by mediator in response to events A and D
    /// - Demonstrates reuse through mediator coordination
    /// - Component doesn't know why it's being called
    /// </summary>
    public void DoC()
    {
        Console.WriteLine("Component 2 does C.");

        _mediator?.Notify(this, "C");
    }

    /// <summary>
    /// Performs operation D and notifies the mediator.
    ///
    /// COMPLEX WORKFLOW:
    /// - Event D triggers multiple component actions
    /// - Demonstrates one-to-many communication pattern
    /// - Mediator orchestrates the workflow
    /// </summary>
    public void DoD()
    {
        Console.WriteLine("Component 2 does D.");

        _mediator?.Notify(this, "D");
    }
}

/// <summary>
/// Program demonstrates the Mediator pattern in action.
/// Shows how components communicate through a mediator instead of directly.
/// </summary>
public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Mediator Pattern Demonstration ===\n");

        Console.WriteLine("--- Example 1: Setting Up Mediator and Components ---\n");

        // Create components
        var component1 = new Component1();
        var component2 = new Component2();

        // Create mediator and wire up components
        // Mediator establishes relationships between components
        var concreteMediator = new ConcreteMediator(component1, component2);

        Console.WriteLine("Mediator created and components wired up.\n");

        Console.WriteLine("--- Example 2: Component1 Triggers Event A ---\n");
        Console.WriteLine("Client triggers operation A.");
        component1.DoA();
        // Flow: Component1.DoA() -> Mediator reacts to A -> Component2.DoC()
        // Component1 doesn't know about Component2
        // Mediator coordinates the interaction

        Console.WriteLine();

        Console.WriteLine("--- Example 3: Component2 Triggers Event D ---\n");
        Console.WriteLine("Client triggers operation D.");
        component2.DoD();
        // Flow: Component2.DoD() -> Mediator reacts to D -> Component1.DoB() and Component2.DoC()
        // One event triggers multiple component actions
        // Mediator orchestrates the workflow

        Console.WriteLine();

        Console.WriteLine("--- Example 4: Direct Component Call ---\n");
        Console.WriteLine("Client directly calls Component1.DoB():");
        component1.DoB();
        // Components can be called directly
        // They still notify mediator, but mediator might not react to all events

        Console.WriteLine();

        Console.WriteLine("--- Key Observations ---");
        Console.WriteLine("1. Components don't reference each other directly");
        Console.WriteLine("2. Mediator centralizes communication logic");
        Console.WriteLine("3. Easy to modify workflows by changing mediator");
        Console.WriteLine("4. Components remain reusable and independent");
    }
}

/*
 * KEY TAKEAWAYS:
 *
 * PATTERN COMPONENTS:
 * 1. IMediator - Interface for mediator contract
 * 2. ConcreteMediator - Implements coordination logic, knows all components
 * 3. BaseComponent - Base class with mediator reference
 * 4. Component1/Component2 - Concrete components that notify mediator
 *
 * WHEN TO USE:
 * - Complex communication between multiple objects
 * - Want to reduce coupling between components
 * - Interaction logic is complex and centralized control is beneficial
 * - Components should be reusable in different contexts
 * - Need to vary component interactions without modifying components
 *
 * BENEFITS:
 * 1. Loose Coupling: Components don't reference each other
 * 2. Centralized Control: All interaction logic in one place
 * 3. Reusability: Components can be reused in different contexts
 * 4. Single Responsibility: Components focus on their work, mediator on coordination
 * 5. Easy to Understand: Clear flow of communication
 * 6. Easy to Modify: Change workflows by modifying mediator
 *
 * REAL-WORLD EXAMPLES:
 * - MediatR Library: CQRS command/query mediator in .NET
 *   public class CreateOrderCommand : IRequest<OrderResult> { }
 *   await _mediator.Send(new CreateOrderCommand(...));
 *
 * - ASP.NET Core Razor Pages: PageModel mediates between UI and services
 *   public class IndexModel : PageModel
 *   {
 *       private readonly IMediator _mediator;
 *       public async Task<IActionResult> OnPost() => await _mediator.Send(...);
 *   }
 *
 * - UI Dialog Coordinator: Manages form control interactions
 *   public class DialogMediator
 *   {
 *       // Coordinates between text boxes, buttons, labels
 *       // When text changes, enable/disable buttons
 *       // When button clicked, validate all fields
 *   }
 *
 * - Chat Application: Chat room mediates between users
 *   public class ChatRoom : IMediator
 *   {
 *       public void Notify(User sender, string message)
 *       {
 *           // Send message to all other users
 *           foreach (var user in _users.Where(u => u != sender))
 *               user.ReceiveMessage(sender.Name, message);
 *       }
 *   }
 *
 * - Air Traffic Control: Coordinates airplane communications
 *   public class AirTrafficControl : IMediator
 *   {
 *       public void Notify(Airplane sender, string event)
 *       {
 *           // Coordinate landing, takeoff permissions
 *           // Planes don't communicate directly
 *       }
 *   }
 *
 * - Event Aggregator: Loosely couples event publishers and subscribers
 *   public class EventAggregator : IMediator
 *   {
 *       public void Publish<T>(T eventData)
 *       {
 *           // Notify all subscribers of event type T
 *       }
 *   }
 *
 * TRADE-OFFS:
 * - God Object Risk: Mediator can become too complex
 * - Single Point of Failure: All communication depends on mediator
 * - Difficult to Reuse Mediator: Often specific to a set of components
 * - Performance: Extra indirection for all communications
 * - Debugging: Flow is less obvious than direct calls
 *
 * VARIATIONS:
 * 1. Event-based Mediator: Uses events/delegates instead of method calls
 * 2. Message-based Mediator: Components send message objects
 * 3. Request-Response Mediator: Like MediatR with IRequest/IRequestHandler
 * 4. Pub/Sub Mediator: Event aggregator pattern with subscriptions
 *
 * IMPLEMENTATION CONSIDERATIONS:
 * - Type Safety: Use strongly-typed events instead of strings
 * - Async Support: Make Notify async for async operations
 * - Error Handling: Decide how mediator handles component failures
 * - Event Loops: Prevent infinite loops from cascading notifications
 * - Thread Safety: Consider if mediator will be used concurrently
 *
 * MODERN .NET USAGE:
 * - MediatR: Industry-standard mediator for CQRS
 *   builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
 *
 * - Event Aggregator: Prism, ReactiveUI event aggregation
 *   _eventAggregator.GetEvent<OrderCreatedEvent>().Publish(order);
 *
 * - SignalR Hubs: Real-time communication mediator
 *   public class ChatHub : Hub
 *   {
 *       public async Task SendMessage(string user, string message)
 *       {
 *           await Clients.All.SendAsync("ReceiveMessage", user, message);
 *       }
 *   }
 *
 * - Workflow Orchestrators: Mediate between workflow steps
 *   public class OrderWorkflowMediator
 *   {
 *       // Coordinates: Validation -> Payment -> Inventory -> Shipping
 *   }
 *
 * COMPARISON WITH OTHER PATTERNS:
 * - Facade: Simplifies interface; Mediator: Coordinates interactions
 * - Observer: One-to-many notifications; Mediator: Centralized coordination
 * - Command: Encapsulates requests; Mediator: Coordinates components
 * - Chain of Responsibility: Sequential handling; Mediator: Centralized handling
 *
 * BEST PRACTICES:
 * 1. Keep mediator focused: Don't let it become a god object
 * 2. Use interfaces: Components depend on IMediator, not concrete mediator
 * 3. Consider event types: Use enums or message objects instead of strings
 * 4. Document workflows: Make interaction patterns explicit
 * 5. Avoid circular dependencies: Be careful with cascading events
 * 6. Use dependency injection: Register mediator in DI container
 * 7. Split large mediators: Create multiple mediators for different concerns
 * 8. Consider alternatives: Sometimes direct communication is better
 */
